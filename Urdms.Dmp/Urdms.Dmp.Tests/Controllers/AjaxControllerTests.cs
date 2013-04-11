using System.Collections.Generic;
using System.Web.Mvc;
using AutofacContrib.NSubstitute;
using Curtin.Framework.Common.UserService;
using FizzWare.NBuilder;
using NSubstitute;
using NUnit.Framework;
using Urdms.Dmp.Controllers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Integration.UserService;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;

namespace Urdms.Dmp.Tests.Controllers
{
    [TestFixture]
    public class AjaxControllerShould
    {
        private AutoSubstitute _autoSubstitute;
        private IFieldOfResearchRepository _fieldOfResearchRepository;
        private ISocioEconomicObjectiveRepository _socioEconomicObjectiveRepository;
        private ICurtinUserService _lookupService;
        private AjaxController _controller;
        private IDataCollectionRepository _dataCollectionRepository;

        [SetUp]
        public void Setup()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _fieldOfResearchRepository = _autoSubstitute.Resolve<IFieldOfResearchRepository>();
            _socioEconomicObjectiveRepository = _autoSubstitute.Resolve<ISocioEconomicObjectiveRepository>();
            _controller = _autoSubstitute.GetController<AjaxController>();
            _lookupService = _autoSubstitute.Resolve<ICurtinUserService>();
            _dataCollectionRepository = _autoSubstitute.Resolve<IDataCollectionRepository>();
        }

#region FoR codes
        [Test]
        public void GetForList_with_valid_term_return_for_codes()
        { 
            string term = "test";
            var newList = Builder<FieldOfResearch>
                 .CreateListOfSize(2)
                 .Build();

            _fieldOfResearchRepository.GetMatching(Arg.Is(term))
                .Returns(newList);

            _controller.WithCallTo(c => c.GetForList(term))
                .ShouldReturnJson();

        }

        [Test]
        public void GetForList_with_invalid_term_returns_empty_list()
        {
            string term = "xxx";
                        var forList = new List<FieldOfResearch>();

            _fieldOfResearchRepository.GetMatching(Arg.Is(term))
                .Returns(forList);

            _controller.WithCallTo(c => c.GetForList(term))
                .ShouldReturnJson(r =>
                {
                    Assert.That(r, Is.Empty);
                });
        }

        [Test]
        public void Return_empty_result_for_invalid_FoR_code_on_ajax_call_to_GetNewForCode()
        {
            const string forCode = "invalid code";
            _fieldOfResearchRepository.GetFieldOfResearch(forCode).Returns(x => null);

            _controller.WithCallTo(c => c.GetNewForCode(forCode)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_partial_view_for_valid_FoR_code_on_ajax_call_to_GetNewForCode()
        {
            const string forCode = "010101";
            var fieldOfResearch = Builder<FieldOfResearch>.CreateNew().With(f => f.Id = forCode).And(f => f.Name = "description of FoR code").Build();

            _fieldOfResearchRepository.GetFieldOfResearch(Arg.Is(forCode)).Returns(fieldOfResearch);

            _controller.WithCallTo(c => c.GetNewForCode(forCode)).ShouldRenderPartialView("_FieldOfResearch").WithModel<ClassificationBase>(m =>
            {
                Assert.That(m.Id, Is.EqualTo(0));
                Assert.That(m.Code.Id, Is.EqualTo(forCode));
                Assert.That(m.Code.Name, Is.EqualTo(fieldOfResearch.Name));
                return true;
            });
        }
#endregion

#region SEO codes
        [Test]
        public void GetSeoList_with_valid_term_return_seo_codes()
        {
            string term = "test";
            var seoList = Builder<SocioEconomicObjective>
                .CreateListOfSize(2)
                .Build();


            _socioEconomicObjectiveRepository.GetMatching(Arg.Is(term))
                .Returns(seoList);

            _controller.WithCallTo(c => c.GetSeoList(term))
                .ShouldReturnJson();

        }

        [Test]
        public void GetSeoList_with_invalid_term_returns_empty_list()
        {
            string term = "xxx";
            var seoList = new List<SocioEconomicObjective>();

            _socioEconomicObjectiveRepository.GetMatching(Arg.Is(term))
                .Returns(seoList);

            _controller.WithCallTo(c => c.GetSeoList(term))
                .ShouldReturnJson(r => Assert.That(r, Is.Empty));
        }

        [Test]
        public void Return_empty_result_for_invalid_SEO_code_on_ajax_call_to_GetNewSeoCode()
        {
            const string seoCode = "invalid code";
            _socioEconomicObjectiveRepository.GetSocioEconomicObjective(seoCode).Returns(x => null);

            _controller.WithCallTo(c => c.GetNewSeoCode(seoCode)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_partial_view_for_valid_SEO_code_on_ajax_call_to_GetNewSeoCode()
        {
            const string seoCode = "010101";
            var socioEconomicObjectiveCode = Builder<SocioEconomicObjective>.CreateNew().With(s => s.Id = seoCode).And(s => s.Name = "description").Build();

            _socioEconomicObjectiveRepository.GetSocioEconomicObjective(Arg.Is(seoCode)).Returns(socioEconomicObjectiveCode);

            _controller.WithCallTo(c => c.GetNewSeoCode(seoCode)).ShouldRenderPartialView("_SocioEconomicObjective").WithModel<ClassificationBase>(m =>
            {
                Assert.That(m.Id, Is.EqualTo(0));
                Assert.That(m.Code.Id, Is.EqualTo(seoCode));
                Assert.That(m.Code.Name, Is.EqualTo(socioEconomicObjectiveCode.Name));
                return true;
            });
        }
#endregion

#region Get URDMS users
        [Test]
        public void Return_empty_result_when_no_user_id_passed_on_ajax_call_to_GetNewUrdmsUser_for_a_data_collection()
        {
            const string userId = "";
            const string userType = "data-collection";
            
            _controller.WithCallTo(c => c.GetNewUrdmsUser(userId, userType)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_empty_result_for_invalid_user_id_on_ajax_call_to_GetNewUrdmsUser_for_a_data_collection()
        {
            const string userId = "invalid user id";
            const string userType = "data-collection";

            _lookupService.GetUser(userId).Returns(new UrdmsUser());

            _controller.WithCallTo(c => c.GetNewUrdmsUser(userId, userType)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_empty_result_if_user_id_is_principal_investigator_on_ajax_call_to_GetNewUrdmsUser_for_a_dmp()
        {
            const string userId = "EE21168";
            const string userType = "project";
            CreateUser(userId);

            _controller.WithCallTo(c => c.GetNewUrdmsUser(userId, userType)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_partial_view_for_valid_user_id_on_ajax_call_to_GetNewUrdmsUser_for_a_data_collection()
        {
            const string userId = "EE21168";
            const string userType = "data-collection";
            const string principalInvestigatorId = "111222A";
            CreateUser(principalInvestigatorId);
            var urdmsUser = Builder<UrdmsUser>.CreateNew().With(cu => cu.CurtinId = userId).Build();

            _lookupService.GetUser(userId).Returns(urdmsUser);

            _controller.WithCallTo(c => c.GetNewUrdmsUser(userId, userType)).ShouldRenderPartialView("_UrdmsUser").WithModel<UrdmsUserViewModel>(m =>
            {
                Assert.That(m.Id, Is.EqualTo(0));
                Assert.That(m.Relationship, Is.EqualTo((int)DataCollectionRelationshipType.AssociatedResearcher));
                Assert.That(m.UserId, Is.EqualTo(userId), "User ID does not match");
                Assert.That(m.FullName, Is.EqualTo(urdmsUser.FullName));
                return true;
            });
        }

        [Test]
        public void Return_partial_view_for_valid_user_id_on_ajax_call_to_GetNewUrdmsUser_for_a_project()
        {
            const string userId = "EE21168";
            const string userType = "project";
            const string principalInvestigatorId = "111222A";
            CreateUser(principalInvestigatorId);
            var urdmsUser = Builder<UrdmsUser>.CreateNew().With(cu => cu.CurtinId = userId).Build();

            _lookupService.GetUser(userId).Returns(urdmsUser);

            _controller.WithCallTo(c => c.GetNewUrdmsUser(userId, userType)).ShouldRenderPartialView("_UrdmsUser").WithModel<UrdmsUserViewModel>(m =>
            {
                Assert.That(m.Relationship, Is.EqualTo((int)AccessRole.Visitors));
                return true;
            });
        }

        [Test]
        public void Return_empty_result_on_ajax_call_to_GetNewUrdmsUser_for_an_invalid_type()
        {
            const string userId = "EE21168";
            const string userType = "invalid-type!";
            const string principalInvestigatorId = "111222A";
            CreateUser(principalInvestigatorId);
            var urdmsUser = Builder<UrdmsUser>.CreateNew().With(cu => cu.CurtinId = userId).Build();

            _lookupService.GetUser(userId).Returns(urdmsUser);

            _controller.WithCallTo(c => c.GetNewUrdmsUser(userId, userType)).ShouldRenderEmptyResult();
        }
#endregion

#region Get non URDMS users
        [Test]
        public void Return_empty_result_for_empty_value_on_ajax_call_to_GetNewNonUrdmsUser_for_data_collection()
        {
            const string userFullName = "";
            const string userType = "data-collection";

            _controller.WithCallTo(c => c.GetNewNonUrdmsUser(userFullName, userType)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_partial_view_on_ajax_call_to_GetNewNonUrdmsUser_for_data_collection()
        {
            const string userFullName = "Joe Bloggs";
            const string userType = "data-collection";

            _controller.WithCallTo(c => c.GetNewNonUrdmsUser(userFullName, userType))
                .ShouldRenderPartialView("_NonUrdmsUser")
                .WithModel<NonUrdmsUserViewModel>(m =>
                {
                    Assert.That(m.Id, Is.EqualTo(0));
                    Assert.That(m.Relationship, Is.EqualTo((int)DataCollectionRelationshipType.AssociatedResearcher));
                    Assert.That(m.FullName, Is.EqualTo(userFullName));
                    return true;
                });
        }

        [Test]
        public void Return_partial_view_on_ajax_call_to_GetNewNonUrdmsUser_for_project()
        {
            const string userFullName = "Joe Bloggs";
            const string userType = "project";

            _controller.WithCallTo(c => c.GetNewNonUrdmsUser(userFullName, userType))
                .ShouldRenderPartialView("_NonUrdmsUser")
                .WithModel<NonUrdmsUserViewModel>(m =>
                {
                    Assert.That(m.Relationship, Is.EqualTo((int)AccessRole.Visitors));
                    return true;
                });
        }

        [Test]
        public void Return_empty_result_on_ajax_call_to_GetNewNonUrdmsUser_for_invalid_type()
        {
            const string userFullName = "Joe Bloggs";
            const string userType = "invalid-type";

            _controller.WithCallTo(c => c.GetNewNonUrdmsUser(userFullName, userType))
                .ShouldRenderEmptyResult();
        }
#endregion

#region Get URDMS users for approval
        [Test]
        public void Return_empty_result_when_no_user_id_passed_on_ajax_call_to_GetNewUrdmsUserForApproval_for_a_data_collection()
        {
            const string userId = "";
            const int dataCollectionId = 1;

            _controller.WithCallTo(c => c.GetNewUrdmsUserForApproval(userId, dataCollectionId)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_empty_result_when_data_collection_id_less_than_one_on_ajax_call_to_GetNewUrdmsUserForApproval_for_a_data_collection()
        {
            const string userId = "EE21168";
            const int dataCollectionId = 0;

            _controller.WithCallTo(c => c.GetNewUrdmsUserForApproval(userId, dataCollectionId)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_empty_result_when_invalid_data_collection_id_passed_on_ajax_call_to_GetNewUrdmsUserForApproval_for_a_data_collection()
        {
            const string userId = "EE21168";
            const int dataCollectionId = 1;
            _dataCollectionRepository.Get(dataCollectionId).Returns(x => null);

            _controller.WithCallTo(c => c.GetNewUrdmsUserForApproval(userId, dataCollectionId)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_empty_result_when_user_id_matches_manager_on_ajax_call_to_GetNewUrdmsUserForApproval_for_a_data_collection()
        {
            const string userId = "EE21168";
            const int dataCollectionId = 1;
            var dataCollection = CreateDataCollection(userId);

            _dataCollectionRepository.Get(dataCollectionId).Returns(dataCollection);

            _controller.WithCallTo(c => c.GetNewUrdmsUserForApproval(userId, dataCollectionId)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_empty_result_when_user_id_does_not_exist_on_ajax_call_to_GetNewUrdmsUserForApproval_for_a_data_collection()
        {
            const string userId = "EE21168";
            const string managerId = "333444a";
            const int dataCollectionId = 1;
            var dataCollection = CreateDataCollection(managerId);

            _dataCollectionRepository.Get(dataCollectionId).Returns(dataCollection);

            var user = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = null).Build();
            _lookupService.GetUser(Arg.Is(userId)).Returns(user);

            _controller.WithCallTo(c => c.GetNewUrdmsUserForApproval(userId, dataCollectionId)).ShouldRenderEmptyResult();
        }

        [Test]
        public void Return_partial_view_on_ajax_call_to_GetNewUrdmsUserForApproval_for_a_data_collection()
        {
            const string userId = "EE21168";
            const string managerId = "333444a";
            const int dataCollectionId = 1;
            var dataCollection = CreateDataCollection(managerId);
            CreateUser(userId);

            _dataCollectionRepository.Get(dataCollectionId).Returns(dataCollection);

            _controller.WithCallTo(c => c.GetNewUrdmsUserForApproval(userId, dataCollectionId)).ShouldRenderPartialView("_UrdmsUser");
        }
#endregion

        private void CreateUser(string userId)
        {
            var user = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = userId).Build();
            UserIs.AuthenticatedAs(_autoSubstitute, userId, new[] { "Administrators" });
            _lookupService.GetUser(Arg.Is(userId)).Returns(user);
        }

        private static DataCollection CreateDataCollection(string userId)
        {
            var party = Builder<Party>.CreateNew().With(p => p.UserId = userId).Build();
            var dataCollectionParty = Builder<DataCollectionParty>
                .CreateNew()
                .With(p => p.Party = party)
                .And(p => p.Relationship = DataCollectionRelationshipType.Manager)
                .Build();
            var dataCollection = Builder<DataCollection>
                .CreateNew()
                .Do(dc => dc.Parties.Add(dataCollectionParty))
                .Build();
            return dataCollection;
        }
    }
}
