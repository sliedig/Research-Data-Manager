using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Elmah.Contrib.Mvc;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    public class AjaxController : BaseController
    {
        private readonly IFieldOfResearchRepository _fieldOfResearchRepository;
        private readonly ISocioEconomicObjectiveRepository _socioEconomicObjectiveRepository;
        private readonly IDataCollectionRepository _dataCollectionRepository;

        /// <summary>
        /// Initializes a new instance of the AjaxController class.
        /// </summary>
        /// <param name="fieldOfResearchRepository"></param>
        /// <param name="socioEconomicObjectiveRepository"></param>
        /// <param name="lookupService"></param>
        public AjaxController(IFieldOfResearchRepository fieldOfResearchRepository, ISocioEconomicObjectiveRepository socioEconomicObjectiveRepository, IDataCollectionRepository dataCollectionRepository, ICurtinUserService lookupService) : base(lookupService)
        {
            _fieldOfResearchRepository = fieldOfResearchRepository;
            _socioEconomicObjectiveRepository = socioEconomicObjectiveRepository;
            _dataCollectionRepository = dataCollectionRepository;
        }


        /// <summary>
        /// Gets for list.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>List of FoR Codes</returns>
        public JsonResult GetForList(string term)
        {
            var codes = _fieldOfResearchRepository.GetMatching(term);

            return GenerateJsonList(codes);
        }


        /// <summary>
        /// Gets the seo list.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>List of SEO Codes</returns>
        public JsonResult GetSeoList(string term)
        {
            var codes = _socioEconomicObjectiveRepository.GetMatching(term);

            return GenerateJsonList(codes);
        }


        private JsonResult GenerateJsonList<T>(IEnumerable<T> codes, bool allowGet = true) where T : Code
        {
            var generateJsonList = Json(codes.Select(code => new { value = code.Id, label = String.Format("{0} - {1}", code.Id, code.Name) }).ToList());

            if (allowGet) generateJsonList.JsonRequestBehavior = JsonRequestBehavior.AllowGet;

            return generateJsonList;
        }


        /// <summary>
        /// Gets a FoR code and inserts it into a suitable model for the partial view.
        /// </summary>
        /// <param name="term">The FoR code.</param>
        /// <returns>Partial view</returns>
        public ActionResult GetNewForCode(string term)
        {
            var fieldOfResearch = _fieldOfResearchRepository.GetFieldOfResearch(term);

            if (fieldOfResearch == null)
            {
                return new EmptyResult();
            }

            var newRow = new ClassificationBase {Id = 0, Code = fieldOfResearch};

            return PartialView("_FieldOfResearch", newRow);
        }


        /// <summary>
        /// Gets a SEO code and inserts it into a suitable model for the partial view.
        /// </summary>
        /// <param name="term">The SEO code.</param>
        /// <returns>Partial view</returns>
        public ActionResult GetNewSeoCode(string term)
        {
            var socioEconomicObjective = _socioEconomicObjectiveRepository.GetSocioEconomicObjective(term);

            if(socioEconomicObjective == null)
            {
                return new EmptyResult();
            }

            var newRow = new ClassificationBase {Id = 0, Code = socioEconomicObjective};

            return PartialView("_SocioEconomicObjective", newRow);
        }


        public ActionResult GetNewUrdmsUserForApproval(string term, int dataCollectionId)
        {
            if (string.IsNullOrWhiteSpace(term) || dataCollectionId < 1)
            {
                return new EmptyResult();
            }

            var dataCollection = _dataCollectionRepository.Get(dataCollectionId);

            if(dataCollection == null)
            {
                return new EmptyResult();
            }

            var manager = dataCollection.Parties.Single(p => p.Relationship == DataCollectionRelationshipType.Manager).Party;

            if(term.Equals(manager.UserId, StringComparison.InvariantCultureIgnoreCase))
            {
                return new EmptyResult();
            }

            var urdmsUser = GetUser(term);

            if (urdmsUser == null)
            {
                return new EmptyResult();
            }

            var values = Enum.GetValues(typeof(DataCollectionRelationshipType)).Cast<DataCollectionRelationshipType>().Except(new[] { DataCollectionRelationshipType.Manager, DataCollectionRelationshipType.None });
            ViewBag.Relationships = values.ToDictionary(o => (int)o, o => o.GetDescription());
            var newRow = new UrdmsUserViewModel
                             {
                                 Id = 0,
                                 UserId = urdmsUser.CurtinId,
                                 FullName = urdmsUser.FullName,
                                 Relationship = (int)DataCollectionRelationshipType.AssociatedResearcher
                             };

            return PartialView("_UrdmsUser", newRow);
        }


        /// <summary>
        /// Gets a URDMS user and inserts it into a suitable model for the partial view.
        /// </summary>
        /// <param name="term">A URDMS User Id.</param>
        /// <param name="userType">Type of user ('project' or 'data-collection')</param>
        /// <returns>Partial view</returns>
        public ActionResult GetNewUrdmsUser(string term, string userType)
        {
            if(string.IsNullOrWhiteSpace(term))
            {
                return new EmptyResult();
            }

            var urdmsUser = GetUser(term);

            if (urdmsUser == null || CurrentUser.CurtinId == urdmsUser.CurtinId)
            {
                return new EmptyResult();
            }

            var newRow = new UrdmsUserViewModel { Id = 0, UserId = urdmsUser.CurtinId, FullName = urdmsUser.FullName };

            switch (userType)
            {
                case "project":
                    {
                        var values = Enum.GetValues(typeof (AccessRole)).Cast<AccessRole>().Except(new[] {AccessRole.Owners, AccessRole.None});
                        ViewBag.Relationships = values.ToDictionary(o => (int) o, o => o.GetDescription());
                        newRow.Relationship = (int) AccessRole.Visitors;
                    }
                    break;
                case "data-collection":
                    {
                        var values = Enum.GetValues(typeof (DataCollectionRelationshipType)).Cast<DataCollectionRelationshipType>().Except(new[] {DataCollectionRelationshipType.Manager, DataCollectionRelationshipType.None});
                        ViewBag.Relationships = values.ToDictionary(o => (int) o, o => o.GetDescription());
                        newRow.Relationship = (int) DataCollectionRelationshipType.AssociatedResearcher;
                    }
                    break;
                default:
                    return new EmptyResult();
            }

            return PartialView("_UrdmsUser", newRow);
        }


        /// <summary>
        /// Gets a Non-URDMS user and inserts it into a suitable model for the partial view.
        /// </summary>
        /// <param name="term">A Non URDMS user full name.</param>
        /// <param name="userType">Type of user ('project' or 'data-collection')</param>
        /// <returns>Partial view</returns>
        public ActionResult GetNewNonUrdmsUser(string term, string userType)
        {
            if(string.IsNullOrWhiteSpace(term))
            {
                return new EmptyResult();
            }

            var newRow = new NonUrdmsUserViewModel { Id = 0, FullName = term };

            switch (userType)
            {
                case "project":
                    {
                        var values = Enum.GetValues(typeof(AccessRole)).Cast<AccessRole>().Except(new[] { AccessRole.Owners, AccessRole.None });
                        ViewBag.Relationships = values.ToDictionary(o => (int)o, o => o.GetDescription());
                        newRow.Relationship = (int)AccessRole.Visitors;
                    }
                    break;
                case "data-collection":
                    {
                        var values = Enum.GetValues(typeof(DataCollectionRelationshipType)).Cast<DataCollectionRelationshipType>().Except(new[] { DataCollectionRelationshipType.Manager, DataCollectionRelationshipType.None });
                        ViewBag.Relationships = values.ToDictionary(o => (int)o, o => o.GetDescription());
                        newRow.Relationship = (int)DataCollectionRelationshipType.AssociatedResearcher;
                    }
                    break;
                default:
                    return new EmptyResult();
            }

            return PartialView("_NonUrdmsUser", newRow);
        }
    }
}
