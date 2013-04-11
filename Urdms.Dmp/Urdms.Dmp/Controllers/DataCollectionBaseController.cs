using System;
using System.Linq;
using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Elmah.Contrib.Mvc;
using NServiceBus;
using Urdms.Dmp.Controllers.Filters;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Mappers;
using Urdms.Dmp.Models.DataCollectionModels;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers
{
    [ElmahHandleError]
    public abstract class DataCollectionBaseController<TViewModelStep1,TViewModelStep2> : BaseController
        where TViewModelStep1 : DataCollectionViewModelStep1, new()
        where TViewModelStep2 : DataCollectionViewModelStep2, new()
    {
        protected IDataCollectionRepository DataCollectionRepository { get; private set; }
        protected IProjectRepository ProjectRepository { get; private set; }
        protected IFieldOfResearchRepository FieldOfResearchRepository { get; private set; }
        protected ISocioEconomicObjectiveRepository SocioEconomicObjectiveRepository { get; private set; }
        protected IDataCollectionHashCodeRepository HashCodeRepository { get; private set; }
        protected IBus Bus { get; private set; }

        protected DataCollectionBaseController(ICurtinUserService lookupService, IDataCollectionRepository dataCollectionRepository,
            IProjectRepository projectRepository, IFieldOfResearchRepository fieldOfResearchRepository,
            ISocioEconomicObjectiveRepository socioEconomicObjectiveRepository,IDataCollectionHashCodeRepository hashCodeRepository, IBus bus)
            : base(lookupService)
        {
            this.DataCollectionRepository = dataCollectionRepository;
            this.ProjectRepository = projectRepository;
            this.FieldOfResearchRepository = fieldOfResearchRepository;
            this.SocioEconomicObjectiveRepository = socioEconomicObjectiveRepository;
            this.HashCodeRepository = hashCodeRepository;
            this.Bus = bus;
        }


        [HttpPost]
        [ProvideDataCollectionSteps]
        public virtual ActionResult Step1(TViewModelStep1 vm)
        {
            if(DataCollectionRepository.TitleExistsAlreadyForProject(vm.Id, vm.ProjectId, vm.Title))
            {
                ModelState.AddModelError("Title", "A data collection with this title exists already within this project, please choose another title");
            }

            if (!ModelState.IsValid)
            {
                return View("DataCollectionStep1", vm);
            }

            var collection = vm.Id == 0 ? new DataCollection() : DataCollectionRepository.Get(vm.Id);
            collection.MapFrom(vm);

            if (vm.Id == 0)
            {
                collection.Parties.Add(new DataCollectionParty
                {
                    Party = new Party
                    {
                        UserId = CurrentUser.CurtinId,
                        Email = CurrentUser.EmailAddress,
                        FirstName = CurrentUser.FirstName,
                        FullName = CurrentUser.FullName,
                        LastName = CurrentUser.LastName,
						Organisation = ""	// TODO: Insert your organisation here
                    },
                    DataCollection = collection,
                    Relationship = DataCollectionRelationshipType.Manager
                });
            }
            return OnPostToStep1(collection);
        }



        [HttpPost]
        [AcceptMethodByParameter(Name = "AddForCode")]
        [ActionName("Step2")]
        [ProvideDataCollectionSteps]
        public ActionResult AddForCode(TViewModelStep2 vm)
        {
            ModelState.Clear();

            try
            {
                FieldOfResearchRepository.PopulateForCodes<DataCollectionFieldOfResearch>(vm, ControllerContext.HttpContext.Request.Form);
                ClearTransientFields(vm);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Source, e.Message);
            }

            return View("DataCollectionStep2", vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "AddSeoCode")]
        [ActionName("Step2")]
        [ProvideDataCollectionSteps]
        public ActionResult AddSeoCode(TViewModelStep2 vm)
        {
            ModelState.Clear();

            try
            {
                SocioEconomicObjectiveRepository.PopulateSeoCodes<DataCollectionSocioEconomicObjective>(vm, ControllerContext.HttpContext.Request.Form);
                ClearTransientFields(vm);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Source, e.Message);
            }

            return View("DataCollectionStep2", vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "DeleteForCodes")]
        [ActionName("Step2")]
        [ProvideDataCollectionSteps]
        public ActionResult DeleteForCodes(TViewModelStep2 vm)
        {
            ModelState.Clear();
            try
            {
                vm.DeleteForCodes(ControllerContext.HttpContext.Request.Form);
                ClearTransientFields(vm);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Source, e.Message);
            }
            return View("DataCollectionStep2", vm);
        }


        [HttpPost]
        [AcceptMethodByParameter(Name = "DeleteSeoCodes")]
        [ActionName("Step2")]
        [ProvideDataCollectionSteps]
        public ActionResult DeleteSeoCodes(TViewModelStep2 vm)
        {
            ModelState.Clear();
            try
            {
                vm.DeleteSeoCodes(ControllerContext.HttpContext.Request.Form);
                ClearTransientFields(vm);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(e.Source, e.Message);
            }
            return View("DataCollectionStep2", vm);
        }


        [HttpPost]
        [AcceptMethodByParameter(Name = "AddUrdmsUser")]
        [ActionName("Step2")]
        [ProvideDataCollectionSteps]
        public ActionResult AddUrdmsUser(TViewModelStep2 vm)
        {

            if (!string.IsNullOrWhiteSpace(vm.FindUserId))
            {
                var urdmsUser = GetUser(vm.FindUserId.Trim());
                if (urdmsUser != null)
                {
                    var dataCollection = DataCollectionRepository.Get(vm.Id);
                    vm.AddUrdmsUser(urdmsUser, DataCollectionRelationshipType.AssociatedResearcher);

                    var entityUser = dataCollection.Parties
                        .Where(user => user.Party.UserId != null && string.Compare(user.Party.UserId.Trim(), urdmsUser.CurtinId.Trim(), true) == 0)
                        .Take(1)
                        .FirstOrDefault();

                    var modelUser = vm.UrdmsUsers
                        .Where(user => string.Compare(user.UserId.Trim(), urdmsUser.CurtinId.Trim(), true) == 0)
                        .Take(1)
                        .FirstOrDefault();

                    if (entityUser != null && modelUser != null)
                        modelUser.Id = entityUser.Party.Id;
                }

                vm.FindUserId = null;
            }

            return View("DataCollectionStep2", vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "AddNonUrdmsUser")]
        [ActionName("Step2")]
        [ProvideDataCollectionSteps]
		public ActionResult AddNonUrdmsUser(TViewModelStep2 vm)
        {

            if (!string.IsNullOrWhiteSpace(vm.NonUrdmsNewUserName))
            {
                var dataCollection = DataCollectionRepository.Get(vm.Id);
                vm.AddNonUrdmsUser(vm.NonUrdmsNewUserName, DataCollectionRelationshipType.AssociatedResearcher);

                var entityUser = dataCollection.Parties
                        .Where(user => string.Compare(user.Party.FullName.Trim(), vm.NonUrdmsNewUserName.Trim(), true) == 0)
                        .Take(1)
                        .FirstOrDefault();

                var modelUser = vm.NonUrdmsUsers
                    .Where(user => string.Compare(user.FullName.Trim(), vm.NonUrdmsNewUserName.Trim(), true) == 0)
                    .Take(1)
                    .FirstOrDefault();

                if (entityUser != null && modelUser != null)
                    modelUser.Id = entityUser.Party.Id;

                vm.NonUrdmsNewUserName = null;
            }

            return View("DataCollectionStep2", vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "DeleteUrdmsUser")]
        [ActionName("Step2")]
        [ProvideDataCollectionSteps]
        public ActionResult RemoveUrdmsUser(TViewModelStep2 vm)
        {
            ClearTransientFields(vm);
            return View("DataCollectionStep2", vm);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "DeleteNonUrdmsUser")]
        [ActionName("Step2")]
        [ProvideDataCollectionSteps]
		public ActionResult RemoveNonUrdmsUser(TViewModelStep2 vm)
        {
            ClearTransientFields(vm); 
            return View("DataCollectionStep2", vm);
        }

        [HttpPost]
		[DenyMethodByParameter(Name = "saveAndPrevious,AddForCode,AddSeoCode,DeleteForCodes,DeleteSeoCodes,AddUrdmsUser,AddNonUrdmsUser,DeleteUrdmsUser,DeleteNonUrdmsUser")]
        [ProvideDataCollectionSteps]
        public virtual ActionResult Step2(TViewModelStep2 vm)
        {
            if (!ModelState.IsValid)
            {
                return View("DataCollectionStep2", vm);
            }

            var collection = DataCollectionRepository.Get(vm.Id);
            collection.MapFrom(vm, true);
            return OnPostToStep2(collection);
        }

        [HttpPost]
        [AcceptMethodByParameter(Name = "saveAndPrevious")]
        [ActionName("Step2")]
        [ProvideDataCollectionSteps]
        public ActionResult BackToStep1(TViewModelStep2 vm)
        {
            if (!ModelState.IsValid)
            {
                return View("DataCollectionStep2", vm);
            }

            var collection = DataCollectionRepository.Get(vm.Id);
            collection.MapFrom(vm, true);

            DataCollectionRepository.Save(collection);
            return OnRedirectToStep1(collection);
        }

        protected abstract ActionResult OnSaveToStep2(DataCollection collection);

        protected abstract ActionResult OnRedirectToStep1(DataCollection collection);

        protected abstract ActionResult OnRedirectToStep2(DataCollection collection);

        protected virtual ActionResult OnPostToStep1(DataCollection collection)
        {
            // enforce business rule relationships
            if (collection.Availability == DataSharingAvailability.Never)
            {
                collection.AwareOfEthics = false;
            }
            if (!collection.AwareOfEthics)
            {
                collection.EthicsApprovalNumber = null;
            }
            if (collection.Availability != DataSharingAvailability.AfterASpecifiedEmbargoPeriod)
            {
                collection.AvailabilityDate = null;
            }
            DataCollectionRepository.Save(collection);
            return OnRedirectToStep2(collection);
        }

        protected virtual ActionResult OnPostToStep2(DataCollection collection)
        {
            DataCollectionRepository.Save(collection);
            return OnSaveToStep2(collection);
        }

        protected virtual void ClearTransientFields(TViewModelStep2 vm)
        {
            vm.FieldOfResearchCode = null;
            vm.SocioEconomicObjectiveCode = null;
            vm.FindUserId = null;
        }
    }
}
