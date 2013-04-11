using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using AutofacContrib.NSubstitute;
using Curtin.Framework.Common.Extensions;
using Curtin.Framework.Common.UserService;
using Urdms.Approvals.ApprovalService.Commands;
using FizzWare.NBuilder;
using NServiceBus;
using NSubstitute;
using NUnit.Framework;
using Urdms.Dmp.Controllers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Entities.Components;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Integration.UserService;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.DataCollectionModels;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Controllers
{
    [TestFixture]
    class DataCollectionControllerShould
    {
        private AutoSubstitute _autoSubstitute;
        private DataCollectionController _controller;
        private const int ProjectId = 1;
        private const int CollectionId = 0;
        private ControllerContext _context;
        private NameValueCollection _form;
        private ICurtinUserService _lookup;
        private IProjectRepository _projectRepository;
        private IFieldOfResearchRepository _fieldOfResearchRepository;
        private ISocioEconomicObjectiveRepository _socioEconomicObjectiveRepository;
        private IDataCollectionRepository _dataCollectionRepository;
        private IBus _bus;

        [SetUp]
        public void SetUp()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _controller = _autoSubstitute.GetController<DataCollectionController>();
            _projectRepository = _autoSubstitute.Resolve<IProjectRepository>();
            _dataCollectionRepository = _autoSubstitute.Resolve<IDataCollectionRepository>();
            _context = _autoSubstitute.Resolve<ControllerContext>();
            _form = _context.HttpContext.Request.Form;
            _lookup = _autoSubstitute.Resolve<ICurtinUserService>();
            _fieldOfResearchRepository = _autoSubstitute.Resolve<IFieldOfResearchRepository>();
            _socioEconomicObjectiveRepository = _autoSubstitute.Resolve<ISocioEconomicObjectiveRepository>();
            _bus = _autoSubstitute.Resolve<IBus>();
        }

        private void CreateUser(string userId)
        {
            var user = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = userId).Build();
            UserIs.AuthenticatedAs(_autoSubstitute, userId, new[] { "Administrators" });
            _lookup.GetUser(Arg.Is(userId)).Returns(user);
        }

        #region List Tests

        [Test]
        public void Return_ProjectNotFound_for_invalid_project_on_get_to_index()
        {
            _projectRepository.Get(ProjectId).Returns(x => null);

            _controller.WithCallTo(c => c.Index(ProjectId)).ShouldRenderView("ProjectNotFound");
        }

       
        [TestCase(SourceProjectType.DMP)]
        [TestCase(SourceProjectType.DEPOSIT)]
        public void Return_list_of_collections_for_get_to_index(SourceProjectType projectType)
        {
            DataDeposit dd = projectType == SourceProjectType.DEPOSIT ? Builder<DataDeposit>.CreateNew().Build() : null;
            DataManagementPlan dmp = projectType == SourceProjectType.DMP ? Builder<DataManagementPlan>.CreateNew().Build() : null; 
            // Arrange
            var collections = Builder<DataCollection>.CreateListOfSize(2)
              .TheFirst(1)
              .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Draft, DateTime.Now))
              .And(dc => dc.IsFirstCollection = true)
              .TheNext(1)
              .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Draft, DateTime.Now))
              .And(dc => dc.IsFirstCollection = false)
              .Build();
            var project = Builder<Project>.CreateNew()
                .With(p => p.Id = ProjectId)
                .And(p => p.DataManagementPlan = dmp)
                .And(p => p.DataDeposit = dd)
                .And(p => p.DataCollections.AddRange(collections))
                .And(p => p.SourceProjectType = projectType)
                .Build();

            _projectRepository.Get(project.Id).Returns(project);
          
            _dataCollectionRepository.GetByProject(project.Id).Returns(collections);

            // Act
            _controller.WithCallTo(c => c.Index(ProjectId)).ShouldRenderDefaultView()
                .WithModel<DataCollectionListViewModel>(m =>
                                                                   {
                                                                       Assert.That(m.ProjectId, Is.EqualTo(project.Id),
                                                                                   "Project ID not correct");
                                                                       Assert.That(m.ProjectTitle,
                                                                                   Is.EqualTo(project.Title),
                                                                                   "Project title not correct");
                                                                       Assert.That(
                                                                           m.DataCollectionItems.Count(),
                                                                           Is.EqualTo(collections.Count));
                                                                       collections.Do(
                                                                           c =>
                                                                           Assert.That(
                                                                               m.DataCollectionItems.Any(
                                                                                   cc => cc.Id == c.Id)));
                                                                       return true;
                                                                   });

            _projectRepository.Received().Get(Arg.Is(project.Id));
            _dataCollectionRepository.Received().GetByProject(Arg.Is(project.Id));
            _dataCollectionRepository.DidNotReceive().Get(Arg.Any<int>());
        }


       
        [TestCase(SourceProjectType.DMP,"DmpNotSubmitted")]
        [TestCase(SourceProjectType.DEPOSIT,"DataDepositNotSubmitted")]
        public void Return_NoSubmission_for_get_to_index_when_first_collection_not_created(SourceProjectType projectType, string notSubmittedView)
        {
            DataDeposit dd = projectType == SourceProjectType.DEPOSIT ? Builder<DataDeposit>.CreateNew().Build() : null;
            DataManagementPlan dmp = projectType == SourceProjectType.DMP ? Builder<DataManagementPlan>.CreateNew().Build() : null; 

            var project = Builder<Project>.CreateNew()
                .With(o => o.SourceProjectType = projectType)
                .And(o => o.DataManagementPlan = dmp)
                .And(o => o.DataDeposit = dd)
                .Build();

            _projectRepository.Get(project.Id).Returns(project);
            _controller.WithCallTo(c => c.Index(project.Id))
                .ShouldRenderView(notSubmittedView);

            _projectRepository.Received().Get(Arg.Is(project.Id));
            _dataCollectionRepository.DidNotReceive().GetByProject(Arg.Is(project.Id));

        }
    
        [TestCase(SourceProjectType.DMP, "DmpNotSubmitted")]
        [TestCase(SourceProjectType.DEPOSIT, "DataDepositNotSubmitted")]
        public void Return_NoSubmission_for_post_to_index_when_first_collection_not_created(SourceProjectType projectType, string notSubmittedView)
        {
            DataDeposit dd = projectType == SourceProjectType.DEPOSIT ? Builder<DataDeposit>.CreateNew().Build() : null;
            DataManagementPlan dmp = projectType == SourceProjectType.DMP ? Builder<DataManagementPlan>.CreateNew().Build() : null;

            var project = Builder<Project>.CreateNew()
                .With(o => o.SourceProjectType = projectType)
                .And(o => o.DataManagementPlan = dmp)
                .And(o => o.DataDeposit = dd)
                .Build();

            var vm = Builder<DataCollectionListViewModel>.CreateNew()
                .With(o => o.ProjectId = project.Id)
                .And(o => o.ProjectTitle = project.Title)
                .Build();


            _projectRepository.Get(project.Id).Returns(project);
            _controller.WithCallTo(c => c.Index(vm))
                .ShouldRenderView(notSubmittedView);

            _projectRepository.Received().Get(Arg.Is(project.Id));
            _dataCollectionRepository.DidNotReceive().GetByProject(Arg.Is(project.Id));
            _dataCollectionRepository.DidNotReceive().Save(Arg.Any<DataCollection>());

        }

        [Test]
        public void Return_ProjectWithNoDmp_for_project_with_no_Dmp_on_get_to_index()
        {
            var project = Builder<Project>.CreateNew()
                .With(p => p.SourceProjectType = SourceProjectType.DMP)
                .And(p => p.DataManagementPlan = null)
                .Build();
            _projectRepository.Get(ProjectId).Returns(project);
            _controller.WithCallTo(c => c.Index(ProjectId)).ShouldRenderView("ProjectWithNoDmp");
        }

        [Test]
        public void Return_ProjectWithNoDataDeposit_for_project_with_no_DataDeposit_on_get_to_index()
        {
            var project = Builder<Project>.CreateNew()
                .With(p => p.SourceProjectType = SourceProjectType.DEPOSIT)
                .And(p => p.DataDeposit = null)
                .Build();
            _projectRepository.Get(ProjectId).Returns(project);
            _controller.WithCallTo(c => c.Index(ProjectId)).ShouldRenderView("ProjectWithNoDataDeposit");
        }

        #endregion

        #region New Tests

        [Test]
        public void Redirect_to_collection_form_for_step1_on_get_to_new()
        {
            _controller.WithCallTo(c => c.New(ProjectId))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Step1", new[] { typeof(int), typeof(int) }));
        }

        #endregion

        #region Step 1 Tests
        [Test]
        public void Return_view_ProjectNotFound_when_GetProject_null_on_get_to_step1()
        {
            _autoSubstitute.Resolve<IProjectRepository>().Get(ProjectId).Returns(n => null);

            _controller.WithCallTo(c => c.Step1(ProjectId, null))
                .ShouldRenderView("ProjectNotFound");
        }

        [Test]
        public void Return_empty_view_model_for_null_collection_id_on_get_to_step1()
        {
            var project = SetUpFullProjectWithAuthentication();
            var dataCollection = SetUpFullDataCollection(project);
            _projectRepository.Get(ProjectId).Returns(project);
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);

            _controller.WithCallTo(c => c.Step1(project.Id, null))
                .ShouldRenderView("DataCollectionStep1")
                .WithModel<DataCollectionViewModelStep1>(vm => vm.Id == 0);

            _projectRepository.Received().Get(Arg.Is(project.Id));
            _dataCollectionRepository.DidNotReceive().Get(Arg.Is(dataCollection.Id));
        }

        [Test]
        public void Return_404_when_collection_not_found_on_get_to_step1()
        {
            var project = Builder<Project>.CreateNew().With(p => p.Id = ProjectId).Build();
            _autoSubstitute.Resolve<IProjectRepository>().Get(ProjectId).Returns(project);
            _autoSubstitute.Resolve<IDataCollectionRepository>().Get(CollectionId).Returns(n => null);

            _controller.WithCallTo(c => c.Step1(ProjectId, CollectionId)).ShouldGiveHttpStatus(404);
        }

        [TestCase(SourceProjectType.DMP)]
        public void Redirect_to_read_only_view_if_data_collection_not_in_draft_state_on_get_to_step1(SourceProjectType projectType)
        {
            const int id = 1;
            var dataCollection = Builder<DataCollection>.CreateNew()
               .With(dc => dc.ProjectId = id)
               .And(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Submitted, DateTime.Now))
               .And(dc => dc.IsFirstCollection = true)
               .Build();

            var project = Builder<Project>.CreateNew()
                .With(p => p.Id = id)
                .And(p => p.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                .And(p => p.SourceProjectType = projectType)
                .And(p => p.DataCollections.Add(dataCollection))
                .Build();
           
            _projectRepository.Get(project.Id).Returns(project);
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);

            _controller.WithCallTo(c => c.Step1(project.Id, dataCollection.Id))
                .ShouldRedirectTo(_controller.GetType().GetMethod("ViewReadOnlyDataCollection", new[] { typeof(int), typeof(int) }));

            _projectRepository.Received().Get(Arg.Is(project.Id));
            _dataCollectionRepository.Received().Get(Arg.Is(dataCollection.Id));
        }

        [Test]
        public void Return_DataCollectionInvalidState_view_if_data_collection_not_in_draft_state_on_post_to_step1()
        {
            var vm = Builder<DataCollectionViewModelStep1>.CreateNew().Build();
            var dataCollection = Builder<DataCollection>.CreateNew()
                .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Submitted, DateTime.Now))
                .And(dc => dc.Id = vm.Id)
                .Build();
            _autoSubstitute.Resolve<IDataCollectionRepository>().Get(dataCollection.Id).Returns(dataCollection);

            _controller.WithCallTo(c => c.Step1(vm)).ShouldRenderView("DataCollectionInvalidState");
        }

        [Test]
        public void Return_collection_form_for_step1_on_get_to_step1()
        {
            var project = SetUpFullProjectWithAuthentication();
            var collection = SetUpFullDataCollection(project);
            _autoSubstitute.Resolve<IProjectRepository>().Get(project.Id).Returns(project);
            _dataCollectionRepository.Get(CollectionId).Returns(collection);

            _controller.WithCallTo(c => c.Step1(project.Id, collection.Id))
                .ShouldRenderView("DataCollectionStep1")
                .WithModel<DataCollectionViewModelStep1>(vm => vm.Id == collection.Id);
        }

        [Test]
        public void Return_Step1_when_viewmodel_invalid_on_post_to_step1()
        {
            var collectionDescriptionViewModelStep1 =
                Builder<DataCollectionViewModelStep1>.CreateNew().With(o => o.Id = CollectionId).Build();

            _controller.ModelState.AddModelError("Error", "Exception here.");

            _controller.WithCallTo(c => c.Step1(collectionDescriptionViewModelStep1))
                .ShouldRenderView("DataCollectionStep1")
                .WithModel<DataCollectionViewModelStep1>(m => m.Id == collectionDescriptionViewModelStep1.Id);

        }
        
        [TestCase(SourceProjectType.DMP)]
        public void Save_model_for_new_collection_and_redirect_to_next_step_on_post_to_step1(SourceProjectType projectType)
        {
            const string userId = "XX12345";
            const int id = 1;
            CreateUser(userId);
            var objectives = Builder<SocioEconomicObjective>.CreateListOfSize(5).Build().Select(q => new ProjectSocioEconomicObjective {SocioEconomicObjective = q});
            var fieldsOfResearch = Builder<FieldOfResearch>.CreateListOfSize(6).Build().Select(q => new ProjectFieldOfResearch {FieldOfResearch = q});
            
            var dataCollection = Builder<DataCollection>.CreateNew()
                .With(o => o.ProjectId = id)
                .And(o => o.IsFirstCollection = true)
                .Build();
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);

            var project = Builder<Project>.CreateNew()
                .With(o => o.Id = id)
                .And(o => o.SourceProjectType = projectType)
                .And(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                .And(o => o.SocioEconomicObjectives.AddRange(objectives))
                .And(o => o.FieldsOfResearch.AddRange(fieldsOfResearch))
                .And(o => o.DataCollections.Add(dataCollection))
                .Build();
            


            var vm = Builder<DataCollectionViewModelStep1>.CreateNew()
                .With(o => o.Id = 0)
                .And(o => o.ProjectId = project.Id)
                .Build();

            _projectRepository.Get(project.Id).Returns(project);
            _controller.WithCallTo(c => c.Step1(vm))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Step2", new[] { typeof(int), typeof(int) }));

            _dataCollectionRepository.DidNotReceive().Get(Arg.Is(dataCollection.Id));
            _projectRepository.Received().Get(Arg.Is(project.Id));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.Id == 0));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.FieldsOfResearch.All(q => fieldsOfResearch.SingleOrDefault(r => r.FieldOfResearch.Id == q.FieldOfResearch.Id) != null)));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.SocioEconomicObjectives.All(q => objectives.SingleOrDefault(r => r.SocioEconomicObjective.Id == q.SocioEconomicObjective.Id) != null)));
        }

        [Test]
        public void Save_parties_for_new_collection_on_post_to_step1()
        {
            const string userId = "XX12345";
            const int id = 1;
            CreateUser(userId);

            var dataCollection = Builder<DataCollection>.CreateNew()
                .With(o => o.ProjectId = id)
                .And(o => o.IsFirstCollection = true)
                .Build();

            var parties = Builder<Party>.CreateListOfSize(2).Build();

            var projectParties = Builder<ProjectParty>.CreateListOfSize(2)
                .All()
                .With(p => p.Role = AccessRole.Members)
                .TheFirst(1)
                .With(p => p.Party = parties[0])
                .TheLast(1)
                .With(p => p.Party = parties[1])
                .Build();

            var project = Builder<Project>.CreateNew()
                .With(o => o.Id = id)
                .And(o => o.SourceProjectType = SourceProjectType.DMP)
                .And(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                .And(o => o.DataCollections.Add(dataCollection))
                .Do(o => o.Parties.AddRange(projectParties))
                .Build();

            var vm = Builder<DataCollectionViewModelStep1>.CreateNew()
                .With(o => o.Id = 0)
                .And(o => o.ProjectId = project.Id)
                .Build();

            _projectRepository.Get(id).Returns(project);

            _controller.WithCallTo(c => c.Step1(vm))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Step2", new[] { typeof(int), typeof(int) }));

            // Owner plus our two added parties
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.Id == 0 && o.Parties.Count() == 3));
        }

        [Test]
        public void Return_view_with_errors_on_post_to_step1_using_same_data_collection_title_as_existing_record()
        {
            const string userId = "XX12345";
            CreateUser(userId);

            const int projectId = 1;
            const int dataCollectionId = 10;
            const string title = "I've already been used for this project!";
            var vm = Builder<DataCollectionViewModelStep1>.CreateNew()
                .With(o => o.Id = 0)
                .And(o => o.ProjectId = projectId)
                .And(o => o.Id = dataCollectionId)
                .And(o => o.Title = title)
                .Build();

            _dataCollectionRepository.TitleExistsAlreadyForProject(Arg.Is(dataCollectionId), Arg.Is(projectId), Arg.Is(title)).Returns(true);

            _controller.WithCallTo(c => c.Step1(vm))
                .ShouldRenderView("DataCollectionStep1");
            
            Assert.That(_controller.ViewData.ModelState.Count, Is.EqualTo(1));
        }

        [Test]
        public void Map_model_to_viewmodel_on_post_to_step1()
        {
            var vm = Builder<DataCollectionViewModelStep1>
                .CreateNew()
                .With(o => o.ProjectId = 1)
                .With(o => o.Title = "Collection Description Title")
                .With(o => o.StartDate = new DateTime(2011, 8, 22))
                .With(o => o.EndDate = new DateTime(2012, 12, 12))
                .With(o => o.ResearchDataDescription = "This is a description")
                .Build();
            _dataCollectionRepository.Save(Arg.Is<DataCollection>(o => o.ProjectId == vm.ProjectId));
            _dataCollectionRepository.Save(Arg.Is<DataCollection>(o => o.Title == vm.Title));
            _dataCollectionRepository.Save(Arg.Is<DataCollection>(o => o.StartDate == vm.StartDate));
            _dataCollectionRepository.Save(Arg.Is<DataCollection>(o => o.ResearchDataDescription == vm.ResearchDataDescription));
            _dataCollectionRepository.Save(Arg.Is<DataCollection>(o => o.EndDate == vm.EndDate));
            _dataCollectionRepository.Save(Arg.Is<DataCollection>(o => o.FieldsOfResearch == null));
        }

     
        [TestCase(SourceProjectType.DMP,"DmpNotSubmitted")]
        [TestCase(SourceProjectType.DEPOSIT,"DataDepositNotSubmitted")]
        public void Return_NotSubmitted_for_a_get_to_step1_when_attempting_to_create_a_data_collection_and_the_first_collection_is_not_created(SourceProjectType projectType, string notSubmittedView)
        {
            DataDeposit dd = projectType == SourceProjectType.DEPOSIT ? Builder<DataDeposit>.CreateNew().Build() : null;
            DataManagementPlan dmp = projectType == SourceProjectType.DMP ? Builder<DataManagementPlan>.CreateNew().Build() : null;

            var project = Builder<Project>.CreateNew()
                .With(o => o.SourceProjectType = projectType)
                .And(o => o.DataManagementPlan = dmp)
                .And(o => o.DataDeposit = dd)
                .Build();

            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.Step1(project.Id, null))
                .ShouldRenderView(notSubmittedView);

            _projectRepository.Received().Get(Arg.Is(project.Id));
            _dataCollectionRepository.DidNotReceive().GetByProject(Arg.Any<int>());
            _dataCollectionRepository.DidNotReceive().Save(Arg.Any<DataCollection>());
        }
        
        [TestCase(SourceProjectType.DMP, "DmpNotSubmitted")]
        [TestCase(SourceProjectType.DEPOSIT, "DataDepositNotSubmitted")]
        public void Return_NotSubmitted_for_a_post_to_step1_when_attempting_to_create_a_data_collection_and_the_first_collection_is_not_created(SourceProjectType projectType, string notSubmittedView)
        {
            DataDeposit dd = projectType == SourceProjectType.DEPOSIT ? Builder<DataDeposit>.CreateNew().Build() : null;
            DataManagementPlan dmp = projectType == SourceProjectType.DMP ? Builder<DataManagementPlan>.CreateNew().Build() : null;
            CreateUser("XX12345");
            var project = Builder<Project>.CreateNew()
                .With(o => o.SourceProjectType = projectType)
                .And(o => o.DataManagementPlan = dmp)
                .And(o => o.DataDeposit = dd)
                .Build();

            _projectRepository.Get(project.Id).Returns(project);

            var vm = Builder<DataCollectionViewModelStep1>.CreateNew()
                .With(o => o.ProjectId = project.Id)
                .And(o => o.Id = 0)
                .And(o => o.ProjectTitle = project.Title)
                .Build();

            _controller.WithCallTo(c => c.Step1(vm))
                .ShouldRenderView(notSubmittedView);

            _projectRepository.Received().Get(Arg.Is(project.Id));
            _dataCollectionRepository.DidNotReceive().Get(Arg.Any<int>());
            _dataCollectionRepository.DidNotReceive().GetByProject(Arg.Any<int>());
            _dataCollectionRepository.DidNotReceive().Save(Arg.Any<DataCollection>());
        }

        [Test]
        public void Update_DataCollection_for_step1()
        {
            var project = SetUpFullProjectWithAuthentication();
            var dataCollection = SetUpFullDataCollection(project);
            var vm = new DataCollectionViewModelStep1
                         {
                             AvailabilityDate = DateTime.Today.AddYears(1),
                             AwareOfEthics = true,
                             DataLicensingRights = PickHelper.RandomEnumExcept(dataCollection.DataLicensingRights),
                             EndDate = DateTime.Today.AddYears(1),
                             Id = dataCollection.Id,
                             ProjectId = project.Id,
                             ProjectTitle = project.Title,
                             ResearchDataDescription = "Research notitia genus",
                             ShareAccess = PickHelper.RandomEnumExcept(dataCollection.ShareAccess,ShareAccess.NoAccess),
                             ShareAccessDescription = "Partis obvius genus",
                             Title = "Notitia Contraho Titulus",
                             StartDate = DateTime.Today.AddYears(-1),
                             Type = PickHelper.RandomEnumExcept(dataCollection.Type),
                             DataCollectionIdentifier = PickHelper.RandomEnumExcept(DataCollectionIdentifier.None, dataCollection.DataCollectionIdentifier),
                             DataCollectionIdentifierValue = "notitia contraho identifier pendo",
                             DataStoreAdditionalDetails = "notitia repono additional retineo",
                             DataStoreLocationName = "Project Storage Space",
                             DataStoreLocationUrl = "http://www.test.edu.au/datastore/ABCDEF/",
                             IsFirstCollection = true
                         };

            _controller.WithCallTo(c => c.Step1(vm))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Step2",new[]{typeof(int),typeof(int)}));

            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.AvailabilityDate == vm.AvailabilityDate));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.AwareOfEthics == vm.AwareOfEthics));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.EthicsApprovalNumber == vm.EthicsApprovalNumber));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.CurrentState.State == DataCollectionStatus.Draft));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.DataLicensingRights == vm.DataLicensingRights));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.EndDate == vm.EndDate));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.Id == vm.Id));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.ProjectId == vm.ProjectId));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.ResearchDataDescription == vm.ResearchDataDescription));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.ShareAccess == vm.ShareAccess));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.ShareAccessDescription == vm.ShareAccessDescription));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.Title == vm.Title));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.Type == vm.Type));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.DataCollectionIdentifier == vm.DataCollectionIdentifier));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.DataCollectionIdentifierValue == vm.DataCollectionIdentifierValue));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.DataStoreAdditionalDetails == vm.DataStoreAdditionalDetails));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.DataStoreLocationName != vm.DataStoreLocationName));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.DataStoreLocationUrl != vm.DataStoreLocationUrl));

            _dataCollectionRepository.ClearReceivedCalls();
            vm.AwareOfEthics = false;
            _controller.WithCallTo(c => c.Step1(vm))
               .ShouldRedirectTo(_controller.GetType().GetMethod("Step2", new[] { typeof(int), typeof(int) }));

            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.AwareOfEthics == false)); 
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.EthicsApprovalNumber == null));

            _dataCollectionRepository.ClearReceivedCalls();

            dataCollection.IsFirstCollection = false;
            _controller.WithCallTo(c => c.Step1(vm))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Step2", new[] { typeof(int), typeof(int) }));

            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.DataStoreLocationName == vm.DataStoreLocationName));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.DataStoreLocationUrl == vm.DataStoreLocationUrl));
            
        }

        #endregion

        #region Step 2 Tests

        [Test]
        public void Return_view_ProjectNotFound_when_GetProject_null_on_get_to_step2()
        {
            _autoSubstitute.Resolve<IProjectRepository>().Get(ProjectId).Returns(n => null);

            _controller.WithCallTo(c => c.Step2(ProjectId, CollectionId))
                .ShouldRenderView("ProjectNotFound");
        }

        [Test]
        public void Return_view_DataCollectionNotFound_when_collection_not_found_on_get_to_step2()
        {
            var project = Builder<Project>.CreateNew().With(p => p.Id = ProjectId).Build();
            _autoSubstitute.Resolve<IProjectRepository>().Get(ProjectId).Returns(project);
            _autoSubstitute.Resolve<IDataCollectionRepository>().Get(CollectionId).Returns(n => null);

            _controller.WithCallTo(c => c.Step2(ProjectId, CollectionId)).ShouldRenderView("DataCollectionNotFound");
        }

        [Test]
        public void Redirect_to_read_only_view_if_data_collection_not_in_draft_state_on_get_to_step2()
        {
            var project = Builder<Project>.CreateNew().With(p => p.Id = ProjectId).Build();
            var dataCollection = Builder<DataCollection>.CreateNew()
                .With(dc => dc.ProjectId = project.Id)
                .And(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Submitted, DateTime.Now))
                .Build();
            _autoSubstitute.Resolve<IProjectRepository>().Get(project.Id).Returns(project);
            _autoSubstitute.Resolve<IDataCollectionRepository>().Get(dataCollection.Id).Returns(dataCollection);

            _controller.WithCallTo(c => c.Step2(project.Id, dataCollection.Id))
                .ShouldRedirectTo(_controller.GetType().GetMethod("ViewReadOnlyDataCollection", new[] { typeof(int), typeof(int) }));
        }

        [Test]
        public void Return_DataCollectionInvalidState_view_if_data_collection_not_in_draft_state_on_post_to_step2()
        {
            var vm = Builder<DataCollectionViewModelStep2>.CreateNew().Build();
            var dataCollection = Builder<DataCollection>.CreateNew()
                .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Submitted, DateTime.Now))
                .And(dc => dc.Id = vm.Id)
                .Build();
            _autoSubstitute.Resolve<IDataCollectionRepository>().Get(dataCollection.Id).Returns(dataCollection);

            _controller.WithCallTo(c => c.Step2(vm)).ShouldRenderView("DataCollectionInvalidState");
        }

        [Test]
        public void Return_collection_form_for_step2_on_get_to_step2()
        {
            var project = Builder<Project>.CreateNew().With(p => p.Id = ProjectId).Build();
            _autoSubstitute.Resolve<IProjectRepository>().Get(ProjectId).Returns(project);
            var collection = Builder<DataCollection>.CreateNew().With(o => o.Id = CollectionId).Build();
            _dataCollectionRepository.Get(CollectionId).Returns(collection);
            const string userId = "XX12345";
            collection.Parties.Add(Builder<DataCollectionParty>.CreateNew()
                .With(o => o.Party = Builder<Party>.CreateNew().With(p => p.UserId = userId).Build())
                .And(o => o.Relationship = DataCollectionRelationshipType.Manager)
                .Build());

            _controller.WithCallTo(c => c.Step2(ProjectId, CollectionId))
                .ShouldRenderView("DataCollectionStep2")
                .WithModel<DataCollectionViewModelStep2>(vm => vm.Id == CollectionId);
        }

        [Test]
        public void Return_Step2_when_viewmodel_invalid_on_post_to_step2()
        {
            var collectionDescriptionViewModelStep2 =
                Builder<DataCollectionViewModelStep2>.CreateNew().With(o => o.Id = CollectionId).Build();

            _controller.ModelState.AddModelError("Error", "Exception here.");

            _controller.WithCallTo(c => c.Step2(collectionDescriptionViewModelStep2))
                .ShouldRenderView("DataCollectionStep2")
                .WithModel<DataCollectionViewModelStep2>(m => m.Id == collectionDescriptionViewModelStep2.Id);
        }

        [Test]
        [Ignore("Model Binder needs to be created for DataCollectionViewModelStep2 to populate Manager and ProjectName")]
        public void Add_a_user_to_user_access_list_for_a_valid_staff_id()
        {
            _dataCollectionRepository.Get(Arg.Any<int>()).Returns(new DataCollection());
            var vm = new DataCollectionViewModelStep2 { FindUserId = "GA37493" };
            _lookup.GetUser(vm.FindUserId)
                .Returns(new UrdmsUser { FullName = "Joe Research", EmailAddress = "j.research@domain.edu.au", CurtinId = vm.FindUserId });

			_controller.WithCallTo(x => x.AddUrdmsUser(vm)).ShouldRenderView("DataCollectionStep2").WithModel
                <DataCollectionViewModelStep2>(
                    m =>
                    {
                        Assert.That(m.UrdmsUsers, Is.Not.Null, "URDMS users does not exist");
                        Assert.That(m.UrdmsUsers.Count, Is.EqualTo(1), "URDMS users list does not match DMP list size");
                        Assert.That(m.UrdmsUsers[0].UserId, Is.EqualTo("GA37493"), "URDMS user's ID not the same");
                        return true;
                    });

        }

        [Test]
        public void Add_a_non_urdms_user_to_user_access_list()
        {
            _dataCollectionRepository.Get(Arg.Any<int>()).Returns(new DataCollection());
            var vm = new DataCollectionViewModelStep2 { NonUrdmsNewUserName = "Romesh" };

			_controller.WithCallTo(x => x.AddNonUrdmsUser(vm)).ShouldRenderView("DataCollectionStep2").WithModel
                <DataCollectionViewModelStep2>(
                    m =>
                    {
                        Assert.That(m.NonUrdmsUsers, Is.Not.Null, "URDMS users does not exist");
                        Assert.That(m.NonUrdmsUsers.Count, Is.EqualTo(1), "URDMS users list does not match dmp list size");
                        Assert.That(m.NonUrdmsUsers[0].FullName, Is.EqualTo("Romesh"), "Non URDMS user's name not the same");
                        return true;
                    });

        }

        [Test]
        public void Save_model_and_redirect_to_Index_on_post_to_step2()
        {
            _dataCollectionRepository.Get(Arg.Any<int>()).Returns(new DataCollection());
            var vm = Builder<DataCollectionViewModelStep2>.CreateNew().Build();

            _controller.WithCallTo(c => c.Step2(vm))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Index", new[] { typeof(int) }));

            _dataCollectionRepository.Received().Save(Arg.Any<DataCollection>());
        }

        [Test]
        public void Save_model_and_redirect_to_Step1_on_post_from_Save_and_Previous()
        {
            _dataCollectionRepository.Get(Arg.Any<int>()).Returns(new DataCollection());
            var vm = Builder<DataCollectionViewModelStep2>.CreateNew().Build();

            _controller.WithCallTo(c => c.BackToStep1(vm))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Step1", new[] { typeof(int), typeof(int) }));

            _dataCollectionRepository.Received().Save(Arg.Any<DataCollection>());
        }

        [Test]
        public void Should_add_seo_code_at_step2()
        {
            var newSeoCode = new SocioEconomicObjective
            {
                Id = int.MaxValue.ToString(),
                Name = "New SeoCode"
            };
            var vm = GetDataCollectionViewModelStep2();
            vm.SocioEconomicObjectiveCode = newSeoCode.Id;
            _socioEconomicObjectiveRepository.GetSocioEconomicObjective(newSeoCode.Id).Returns(newSeoCode);
            _controller
                .WithCallTo(c => c.AddSeoCode(vm))
                .ShouldRenderView("DataCollectionStep2");

            Assert.That(vm.SocioEconomicObjectives.Count(o => o.Code.Id == newSeoCode.Id && o.Code.Name == newSeoCode.Name), Is.EqualTo(1), "Seo Code not added");
        }

        [Test]
        public void Should_add_for_code_at_step2()
        {
            var newFoRCode = new FieldOfResearch
            {
                Id = int.MaxValue.ToString(),
                Name = "New FoRCode"
            };
            var vm = GetDataCollectionViewModelStep2();
            vm.FieldOfResearchCode = newFoRCode.Id;
            _fieldOfResearchRepository.GetFieldOfResearch(newFoRCode.Id).Returns(newFoRCode);
            _controller
                .WithCallTo(c => c.AddForCode(vm))
                .ShouldRenderView("DataCollectionStep2");

            Assert.That(vm.FieldsOfResearch.Count(o => o.Code.Id == newFoRCode.Id && o.Code.Name == newFoRCode.Name), Is.EqualTo(1), "FoR Code not added");
        }

        [Test]
        public void Should_delete_for_codes_at_step2()
        {
            var vm = GetDataCollectionViewModelStep2();
            var fieldsOfResearch = vm.FieldsOfResearch.Take(2).ToList();
		    var expectedCount = vm.FieldsOfResearch.Count - 2;
            foreach (var fieldOfResearch in fieldsOfResearch)
            {
                var key = string.Format("RemoveForCode_{0}", fieldOfResearch.Code.Id);
                _form.Add(key, "true");
                _form.Add(key, "false");
            }

            _controller
                .WithCallTo(c => c.DeleteForCodes(vm))
                .ShouldRenderView("DataCollectionStep2");

            Assert.That(vm.FieldsOfResearch.Any(o => fieldsOfResearch.Any(q => q.Code.Id == o.Code.Id)), Is.False, "FoRCode not deleted");
            Assert.That(vm.FieldsOfResearch.Count, Is.EqualTo(expectedCount), "FoRCode not deleted");
        }

        [Test]
        public void Should_delete_seo_codes_at_step2()
        {
            var vm = GetDataCollectionViewModelStep2();
            var objectives = vm.SocioEconomicObjectives.Take(2).ToList();
		    var expectedCount = vm.SocioEconomicObjectives.Count - 2;
            foreach (var objective in objectives)
            {
                var key = string.Format("RemoveSeoCode_{0}", objective.Code.Id);
                _form.Add(key, "true");
                _form.Add(key, "false");
            }

            _controller
                .WithCallTo(c => c.DeleteSeoCodes(vm))
                .ShouldRenderView("DataCollectionStep2");
            Assert.That(vm.SocioEconomicObjectives.Any(o => objectives.Any(q => q.Code.Id == o.Code.Id)), Is.False, "SeoCode not deleted");
            Assert.That(vm.SocioEconomicObjectives.Count, Is.EqualTo(expectedCount), "SeoCode not deleted");
        }


        [Test]
        public void Update_DataCollection_for_step2()
        {
            var project = SetUpFullProjectWithAuthentication();
            var dataCollection = SetUpFullDataCollection(project);
            var values = new
                             {
                                 DataCollectionType = dataCollection.Type,
                                 ProjectTitle = project.Title,
                                 DataCollectionManager = dataCollection.Parties.Single(o => o.Relationship == DataCollectionRelationshipType.Manager)
                             };

            var forCode = new DataCollectionFieldOfResearch
                              {
                                  Code = Builder<FieldOfResearch>.CreateNew()
                                      .With(o => o.Id = Pick<string>.RandomItemFrom(new[] {"ABC", "DEF", "GHI"}))
                                      .And(o => o.Name = string.Format("{0} Name", o.Id))
                                      .Build()
                              };

            var seoCode = new DataCollectionSocioEconomicObjective
                              {
                                  Code = Builder<SocioEconomicObjective>.CreateNew()
                                      .With(o => o.Id = Pick<string>.RandomItemFrom(new[] {"XYZ", "RST", "UVW"}))
                                      .And(o => o.Name = string.Format("{0} Name", o.Id))
                                      .Build()
                              };

            var parties = Builder<DataCollectionParty>.CreateListOfSize(2)
                .All()
                    .With(o => o.Id = 0)
                    .And(o => o.Relationship = PickHelper.RandomEnumExcept(DataCollectionRelationshipType.Manager, DataCollectionRelationshipType.None))
                .TheFirst(1)
                    .With(o => o.Party = Builder<Party>.CreateNew()
                        .With(q => q.FirstName = Pick<string>.RandomItemFrom(new[] {"Neil", "Nathan", "Nelly"}))
                        .And(q => q.LastName = Pick<string>.RandomItemFrom(new[] {"Lee", "Lim", "Tan"}))
                        .And(q => q.UserId = string.Format("{0}{1}{2}",q.FirstName.Substring(0,3).ToUpper(), q.LastName.Substring(0,3).ToUpper(), Pick<int>.RandomItemFrom(Enumerable.Range(1,10).ToList())))
                        .And(q => q.Email = string.Format("{0}.{1}@yourdomain.edu.au",q.FirstName.ToLower(), q.LastName.ToLower()))
                        .And(q => q.Organisation = "")
                        .Build())
                .TheLast(1)
                    .With(o => o.Party = Builder<Party>.CreateNew()
                        .With(q => q.FirstName = Pick<string>.RandomItemFrom(new[]{"Samuel","Samantha","Steve"}))
                        .And(q => q.LastName = Pick<string>.RandomItemFrom(new[]{"Matt","Majors","Mellencamp"}))
                        .And(q => q.UserId = null)
                        .And(q => q.Email = null)
                        .And(q => q.Organisation = "Edinburgh University")
                        .Build())
                        
                .Do(o =>
                        {
                            o.Party.FullName = string.Format("{0} {1}", o.Party.FirstName, o.Party.LastName);
                        })
                .Build();

            var vm = new DataCollectionViewModelStep2
                         {
                             
                             FieldsOfResearch = dataCollection.FieldsOfResearch.Except(dataCollection.FieldsOfResearch.Take(1)).Union(new[]{forCode}).Cast<ClassificationBase>().ToList(),
                             SocioEconomicObjectives = dataCollection.SocioEconomicObjectives.Except(dataCollection.SocioEconomicObjectives.Take(1)).Union(new[]{seoCode}).Cast<ClassificationBase>().ToList(),
                             Manager = values.DataCollectionManager.Party,
                             Keywords = "exertus, scientia, scisco",
                             Id = dataCollection.Id,
                             ProjectId = dataCollection.ProjectId,
                             UrdmsUsers = parties.Take(1)
                                .Union(dataCollection.Parties
                                    .Where(o => o.Relationship != DataCollectionRelationshipType.Manager && 
                                        !string.IsNullOrWhiteSpace(o.Party.UserId))
                                    .Skip(1))
                                .Select(p => new UrdmsUserViewModel
                                    {
                                        UserId = p.Party.UserId,
                                        FullName = p.Party.FullName,
                                        Id = p.Id,
                                        PartyId = p.Party.Id,
                                        Relationship = (int) p.Relationship
                                    }).ToList(),
                             NonUrdmsUsers = parties.Skip(1)
                                .Union(dataCollection.Parties
                                    .Where(o => string.IsNullOrWhiteSpace(o.Party.UserId))
                                    .Skip(1))
                                .Select(p => new NonUrdmsUserViewModel
                                    {
                                        FullName = p.Party.FullName,
                                        Id = p.Id,
                                        PartyId = p.Party.Id,
                                        Organisation = p.Party.Organisation,
                                        Relationship = (int)p.Relationship

                                    }).ToList()


                         };

            var allParties = vm.UrdmsUsers
                .Select(o => new DataCollectionParty
                                 {
                                     Id = o.Id,
                                     Relationship = (DataCollectionRelationshipType) o.Relationship,
                                     Party = new Party
                                                 {
                                                     Id = o.PartyId,
                                                     UserId = o.UserId,
                                                     FullName = o.FullName,
                                                     Organisation = ""
                                                 }
                                 })
                .Union(vm.NonUrdmsUsers
                           .Select(o => new DataCollectionParty
                                            {
                                                Id = o.Id,
                                                Relationship = (DataCollectionRelationshipType)o.Relationship,
                                                Party = new Party
                                                            {
                                                                Id = o.PartyId,
                                                                FullName = o.FullName,
                                                                Organisation = o.Organisation
                                                            }
                                            }))
                .Union(new[]{values.DataCollectionManager})
                .ToList();

            _controller.WithCallTo(c => c.Step2(vm))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Index", new[] { typeof(int)}));
            
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.CurrentState.State == DataCollectionStatus.Draft));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.Id == vm.Id));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.Type == values.DataCollectionType));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.FieldsOfResearch.All(q => vm.FieldsOfResearch.Any(r => r.Code.Id == q.Code.Id))));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.SocioEconomicObjectives.All(q => vm.SocioEconomicObjectives.Any(r => r.Code.Id == q.Code.Id))));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.Keywords == vm.Keywords));
            _dataCollectionRepository.Received().Save(Arg.Is<DataCollection>(o => o.Parties
                    .Any(q => allParties
                            .Any(r =>
                                r.Id == q.Id &&
                                r.Party.Id == q.Party.Id &&
                                r.Relationship == q.Relationship &&
                                r.Party.FullName == q.Party.FullName &&
                                r.Party.Organisation == q.Party.Organisation &&
                                (r.Party.UserId ?? "") == (q.Party.UserId ?? "")))));
           
        }

        private static DataCollectionViewModelStep2 GetDataCollectionViewModelStep2()
        {
            var model = Builder<DataCollectionViewModelStep2>.CreateNew().Build();
            model.FieldsOfResearch.AddRange(GetForCodes());
            model.SocioEconomicObjectives.AddRange(GetSeoCodes());
            return model;
        }

        private static IEnumerable<DataCollectionFieldOfResearch> GetForCodes(int codeCount = 5)
        {
            var forCodes = Builder<FieldOfResearch>.CreateListOfSize(codeCount).Build();
            var dataCollectionForCodes = Builder<DataCollectionFieldOfResearch>.CreateListOfSize(codeCount).Build();
            for (int i = 0; i < codeCount; i++)
            {
                var dataCollectionForCode = dataCollectionForCodes[i];
                var forCode = forCodes[i];
                dataCollectionForCode.FieldOfResearch = forCode;
            }
            return dataCollectionForCodes;
        }

        private static IEnumerable<DataCollectionSocioEconomicObjective> GetSeoCodes(int codeCount = 3)
        {
            var seoCodes = Builder<SocioEconomicObjective>.CreateListOfSize(codeCount).Build();
            var dataCollectionSeoCodes = Builder<DataCollectionSocioEconomicObjective>.CreateListOfSize(codeCount).Build();
            for (int i = 0; i < codeCount; i++)
            {
                var dataCollectionSeoCode = dataCollectionSeoCodes[i];
                var seoCode = seoCodes[i];
                dataCollectionSeoCode.SocioEconomicObjective = seoCode;
            }
            return dataCollectionSeoCodes;
        }
        #endregion

        #region Submit For Approval

        [Ignore]
        [TestCase(SourceProjectType.DEPOSIT)]
        [TestCase(SourceProjectType.DMP)]
        public void Send_submit_for_approval_command_to_bus(SourceProjectType projectType)
        {
            DataDeposit dd = projectType == SourceProjectType.DEPOSIT ? Builder<DataDeposit>.CreateNew().Build() : null;
            DataManagementPlan dmp = projectType == SourceProjectType.DMP ? Builder<DataManagementPlan>.CreateNew().Build() : null;
            
            var project = SetUpFullProjectWithAuthentication();
            project.SourceProjectType = projectType;
            project.DataDeposit = dd;
            project.DataManagementPlan = dmp;
            var collections = SetUpFullDataCollections(project, 2).ToList();
            collections.ForEach(o => o.CurrentState = new DataCollectionState(DataCollectionStatus.Submitted, DateTime.Now));
            // Arrange
            var vm = Builder<DataCollectionListViewModel>
                .CreateNew()
                .With(o => o.ProjectId = project.Id)
                .And(o => o.DataCollectionItems = Builder<DataCollectionItemViewModel>.CreateListOfSize(3).Build())
                .Build();

            var collection = collections[1];
            

            _bus.When(c => c.Send(Arg.Any<Action<SubmitForApproval>>())).Do(a =>
            {
                // Trace.WriteLine("Collection Id: " + collection.Id);

                // Arrange
                var rsc = new SubmitForApproval();
                var lambda = a.Arg<Action<SubmitForApproval>>();

                // Act
                lambda(rsc);

                Trace.WriteLine("Collection Id: " + collection.Id + " rsc: " + rsc.DataCollectionId);


                // Assert
                Assert.That(rsc.DataCollectionId, Is.EqualTo(collection.Id), "Invalid collection id passed to the bus");
                Assert.That(rsc.ApprovedOn, Is.EqualTo(collection.CurrentState.StateChangedOn).Within(1).Minutes, "Invalid submission date passed to the bus");
                Assert.That(rsc.ApprovedBy, Is.EqualTo(collection.Parties.Single(o => o.Relationship == DataCollectionRelationshipType.Manager).Party.UserId), "Invalid manager id passed to the bus");
            });

            // Act
            _projectRepository.Get(vm.ProjectId).Returns(project);
            _dataCollectionRepository.Get(collections[0].Id).Returns(collections[0]);
            _dataCollectionRepository.Get(collections[1].Id).Returns(collections[1]);

            // Assert
            _controller.WithCallTo(c => c.Index(vm)).ShouldRenderView("Submitted").WithModel<SubmittedDataCollectionsViewModel>((m =>
            {
                Assert.That(ModelCollectionIsEqualTo(vm, m));
                return true;
            }));
            _bus.Received().Send(Arg.Any<Action<SubmitForApproval>>());
        }

        #endregion

        #region Read Only View

        [Test]
        public void Return_default_view_on_get_to_ViewReadOnlyDataCollection_when_data_collection_not_in_draft_status()
        {
            var manager = Builder<DataCollectionParty>.CreateNew()
                .With(m => m.Party = Builder<Party>.CreateNew().Build())
                .And(m => m.Relationship = DataCollectionRelationshipType.Manager)
                .Build();
            var dataCollection = Builder<DataCollection>.CreateNew()
                .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Submitted, DateTime.Now))
                .Build();
            var project = Builder<Project>.CreateNew().With(p => p.Id = dataCollection.ProjectId).Build();
            dataCollection.Parties.Add(manager);
            _projectRepository.Get(project.Id).Returns(project);
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);

            _controller.WithCallTo(c => c.ViewReadOnlyDataCollection(dataCollection.ProjectId, dataCollection.Id))
                .ShouldRenderView("ReadOnly").WithModel<DataCollectionReadOnlyViewModel>(vm =>
                    {
                        Assert.That(vm.Id, Is.EqualTo(dataCollection.Id));
                        Assert.That(vm.ProjectId, Is.EqualTo(dataCollection.ProjectId));
                        Assert.That(vm.Manager.FullName, Is.EqualTo(dataCollection.Parties.Where(p => p.Relationship == DataCollectionRelationshipType.Manager).Single().Party.FullName));
                        return true;
                    });
        }

        [Test]
        public void Redirect_to_edit_action_on_get_to_ViewReadOnlyDataCollection_when_data_collection_in_draft_status()
        {
            var manager = Builder<DataCollectionParty>.CreateNew().With(m => m.Relationship = DataCollectionRelationshipType.Manager).Build();
            var dataCollection = Builder<DataCollection>.CreateNew()
                .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Draft, DateTime.Now))
                .Build();
            var project = Builder<Project>.CreateNew().With(p => p.Id = dataCollection.ProjectId).Build();
            dataCollection.Parties.Add(manager);
            _projectRepository.Get(project.Id).Returns(project);
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);

            _controller.WithCallTo(c => c.ViewReadOnlyDataCollection(dataCollection.ProjectId, dataCollection.Id))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Step1", new[] { typeof(int), typeof(int) }));
        }

        [Test]
        public void Return_projectnotfound_on_get_to_ViewReadOnlyDataCollection_when_project_Id_invaild()
        {
            var manager = Builder<DataCollectionParty>.CreateNew().With(m => m.Relationship = DataCollectionRelationshipType.Manager).Build();
            var dataCollection = Builder<DataCollection>.CreateNew()
                .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Draft, DateTime.Now))
                .Build();
            dataCollection.Parties.Add(manager);
            _projectRepository.Get(dataCollection.ProjectId).Returns(p => null);
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);

            _controller.WithCallTo(c => c.ViewReadOnlyDataCollection(dataCollection.ProjectId, dataCollection.Id))
                .ShouldRenderView("ProjectNotFound");
        }

        [Test]
        public void Return_datacollectionnotfound_on_get_to_ViewReadOnlyDataCollection_when_datacollection_Id_invaild()
        {
            var manager = Builder<DataCollectionParty>.CreateNew().With(m => m.Relationship = DataCollectionRelationshipType.Manager).Build();
            var dataCollection = Builder<DataCollection>.CreateNew()
                .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Draft, DateTime.Now))
                .Build();
            var project = Builder<Project>.CreateNew().With(p => p.Id = dataCollection.ProjectId).Build();
            dataCollection.Parties.Add(manager);
            _projectRepository.Get(project.Id).Returns(project);
            _dataCollectionRepository.Get(dataCollection.Id).Returns(d => null);

            _controller.WithCallTo(c => c.ViewReadOnlyDataCollection(dataCollection.ProjectId, dataCollection.Id))
                .ShouldRenderView("DataCollectionNotFound");
        }

        #endregion

        private static bool ModelCollectionIsEqualTo(DataCollectionListViewModel source, SubmittedDataCollectionsViewModel result)
        {
            if (!result.PublishedDataCollectionItems.All(o => source.DataCollectionItems.SingleOrDefault(q => o.Id == q.Id && q.IsUserSubmitted) != null))
            {
                return false;
            }
            if (result.PublishedDataCollectionItems.Any(o => source.DataCollectionItems.Any(q => o.Id == q.Id && o.Title != q.Title)))
            {
                return false;
            }
            return true;
        }

        private Project SetUpFullProjectWithAuthentication(string userId = "XX12345")
        {
            CreateUser(userId);
            var dmp = Builder<DataManagementPlan>.CreateNew()
                .With(o => o.ExistingDataDetail = Builder<ExistingDataDetail>.CreateNew()
                    .With(q => q.UseExistingData = PickHelper.RandomBoolean())
                    .And(q => q.ExistingDataAccessTypes = PickHelper.RandomEnumsExcept(ExistingDataAccessTypes.None))
                    .Build())
                .And(o => o.NewDataDetail = Builder<NewDataDetail>.CreateNew()
                    .With(q => q.DataOwners = PickHelper.RandomEnumsExcept(DataOwners.None))
                    .Build())
                .And(o => o.DataRelationshipDetail = Builder<DataRelationshipDetail>.CreateNew()
                    .With(q => q.RelationshipBetweenExistingAndNewData = PickHelper.RandomEnumExcept(DataRelationship.None))
                    .Build())
                .And(o => o.DataDocumentation = Builder<DataDocumentation>.CreateNew()
                    .Build())
                .And(o => o.Ethic = Builder<Ethic>.CreateNew()
                    .With(q => q.EthicRequiresClearance = PickHelper.RandomBoolean())
                    .Build())
                .And(o => o.Confidentiality = Builder<Confidentiality>.CreateNew()
                    .With(q => q.IsSensitive = PickHelper.RandomBoolean())
                    .Build())
                .And(o => o.DataStorage = Builder<DataStorage>.CreateNew()
                    .With(q => q.InstitutionalStorageTypes = PickHelper.RandomEnumExcept(InstitutionalStorageTypes.ProjectStorageSpace))
                    .And(q => q.ExternalStorageTypes = PickHelper.RandomEnumExcept(ExternalStorageTypes.None))
                    .And(q => q.PersonalStorageTypes = PickHelper.RandomEnumExcept(PersonalStorageTypes.None))
                    .And(q => q.MaxDataSize = PickHelper.RandomEnumExcept(MaxDataSize.None))
                    .And(q => q.VersionControl = PickHelper.RandomEnumsExcept(VersionControl.None))
                    .Build())
                .And(o => o.DataRetention = Builder<DataRetention>.CreateNew()
                    .With(q => q.DataRetentionLocations = PickHelper.RandomEnumExcept(DataRetentionLocations.None))
                    .And(q => q.DataRetentionResponsibilities = PickHelper.RandomEnumExcept(DataResponsibilities.None))
                    .And(q => q.DataRetentionPeriod = PickHelper.RandomEnumExcept<DataRetentionPeriod>())
                    .Build())
                .And(o => o.DataSharing = Builder<DataSharing>.CreateNew()
                    .With(q => q.DataSharingAvailability = PickHelper.RandomEnumExcept<DataSharingAvailability>())
                    .And(q => q.DataSharingAvailabilityDate = DateTime.Today.AddYears(1))
                    .And(q => q.ShareAccess = PickHelper.RandomEnumExcept(ShareAccess.NoAccess))
                    .And(q => q.DataLicensingType = PickHelper.RandomEnumExcept<DataLicensingType>())
                    .Build())
               .Build();

            var socioEconomicObjectives = Builder<SocioEconomicObjective>.CreateListOfSize(5).Build();
            var fieldsOfResearch = Builder<FieldOfResearch>.CreateListOfSize(6).Build();
            var parties = Builder<Party>.CreateListOfSize(7)
                .TheFirst(1)
                    .With(o => o.UserId = userId)
                    .And(o => o.FirstName = Pick<string>.RandomItemFrom(new[] { "Alan", "Albert", "Adrian" }))
                    .And(o => o.LastName = Pick<string>.RandomItemFrom(new[] { "Wallace", "Willis", "Waylan" }))
                .TheNext(3)
                    .And(o => o.FirstName = Pick<string>.RandomItemFrom(new[] { "Bastian", "Bruce", "Brian", "Julian", "James", "Jones" }))
                    .And(o => o.LastName = Pick<string>.RandomItemFrom(new[] { "Dallas", "Donga", "Dulles", "Frost", "Feller", "Frist" }))
                .TheNext(3)
                    .With(o => o.UserId = null)
                    .And(o => o.Organisation = null)
                    .And(o => o.Email = null)
                    .And(o => o.FirstName = Pick<string>.RandomItemFrom(new[] { "George", "Gerald", "Gordon", "Hally", "Harvey", "Harry" }))
                    .And(o => o.LastName = Pick<string>.RandomItemFrom(new[] { "Pepper", "Prince", "Pulse", "Tommy", "Thors", "Tallis" }))
                .All()
                    .With(o => o.FullName = string.Format("{0} {1}", o.FirstName, o.LastName))
                .TheFirst(4)
                    .With(o => o.Organisation = "Your University")
                    .And(o => o.Email = string.Format("{0}.{1}@yourdomain.edu.au", o.FirstName, o.LastName))
                .Build();
           
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = dmp)
                .And(o => o.ProvisioningStatus = PickHelper.RandomEnumExcept<ProvisioningStatus>())
                .And(o => o.Status = PickHelper.RandomEnumExcept(ProjectStatus.Completed))
                .And(o => o.SourceProjectType = SourceProjectType.DMP)
                .And(o => o.StartDate = DateTime.Today.AddMonths(-3))
                .And(o => o.EndDate = DateTime.Today.AddMonths(9))
                .Do(o =>
                {
                    o.Parties.AddRange(Builder<ProjectParty>.CreateListOfSize(parties.Count)
                        .All()
                            .With(q => q.Party = parties[q.Id - 1])
                        .TheFirst(1)
                            .With(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                            .And(q => q.Role = AccessRole.Owners)
                        .TheNext(3)
                            .With(q => q.Relationship = Pick<ProjectRelationship>.RandomItemFrom(new[] { ProjectRelationship.Student, ProjectRelationship.SupportStaff, ProjectRelationship.Investigator, ProjectRelationship.PartnerInvestigator }))
                            .And(q => q.Role = Pick<AccessRole>.RandomItemFrom(new[] { AccessRole.Members, AccessRole.Visitors }))
                        .TheNext(3)
                            .With(q => q.Relationship = ProjectRelationship.ExternalResearcher)
                            .And(q => q.Role = Pick<AccessRole>.RandomItemFrom(new[] { AccessRole.Members, AccessRole.Visitors }))
                         .Build()
                       );

                    o.SocioEconomicObjectives.AddRange(Builder<ProjectSocioEconomicObjective>.CreateListOfSize(socioEconomicObjectives.Count)
                        .All()
                        .With(q => q.Code = socioEconomicObjectives[q.Id - 1])
                        .Build());

                    o.FieldsOfResearch.AddRange(Builder<ProjectFieldOfResearch>.CreateListOfSize(fieldsOfResearch.Count)
                        .All()
                        .With(q => q.Code = fieldsOfResearch[q.Id = 1])
                        .Build());

                    o.Funders.AddRange(Builder<ProjectFunder>.CreateListOfSize(2)
                        .TheFirst(1)
                            .With(q => q.Funder = Funder.ARC)
                        .TheLast(1)
                            .With(q => q.Funder = Funder.NMHRC)
                        .All()
                            .With(q => q.GrantNumber = Pick<string>.RandomItemFrom(new[] { "ABC123", "DEF456", "GHI789" }))
                        .Build());
                }).Build();

            _projectRepository.Get(Arg.Is(project.Id)).Returns(project);

            return project;
        }

        private IEnumerable<DataCollection> SetUpFullDataCollections(Project project, int instances = 1)
        {
            return Enumerable.Range(1,instances).Select(instance => SetUpFullDataCollection(project));
        }

        private DataCollection SetUpFullDataCollection(Project project)
        {
            var dataCollection = Builder<DataCollection>.CreateNew()
                .With(o => o.ProjectId = project.Id)
                .And(o => o.Id = project.DataCollections.Count == 0 ? 1 : project.DataCollections.Max(q => q.Id) + 1)
                .And(o => o.IsFirstCollection = !project.DataCollections.Any(q => q.IsFirstCollection))
                .And(o => o.Availability = DataSharingAvailability.AfterASpecifiedEmbargoPeriod)
                .And(o => o.AvailabilityDate = DateTime.Today.AddMonths(1))
                .And(o => o.AwareOfEthics = PickHelper.RandomBoolean())
                .And(o => o.DataCollectionIdentifier = PickHelper.RandomEnumExcept(DataCollectionIdentifier.None))
                .And(o => o.DataLicensingRights = PickHelper.RandomEnumExcept<DataLicensingType>())
                .And(o => o.EndDate = DateTime.Today.AddMonths(9))
                .And(o => o.Keywords = project.Keywords)
                .And(o => o.ShareAccess = PickHelper.RandomEnumExcept(ShareAccess.NoAccess))
                .And(o => o.StartDate = DateTime.Today.AddMonths(-3))
                .And(o => o.Type = PickHelper.RandomEnumExcept<DataCollectionType>())
                .Do(o =>
                        {
                            o.FieldsOfResearch.AddRange(Builder<DataCollectionFieldOfResearch>.CreateListOfSize(project.FieldsOfResearch.Count)
                                .All()
                                .With(q => q.FieldOfResearch = project.FieldsOfResearch[q.Id - 1].FieldOfResearch)
                                .Build());

                            o.SocioEconomicObjectives.AddRange(Builder<DataCollectionSocioEconomicObjective>.CreateListOfSize(project.SocioEconomicObjectives.Count)
                                .All()
                                .With(q => q.SocioEconomicObjective = project.SocioEconomicObjectives[q.Id - 1].SocioEconomicObjective)
                                .Build());

                            o.Parties.AddRange(Builder<DataCollectionParty>.CreateListOfSize(project.Parties.Count)
                                .All()
                                .With(q => q.Party = project.Parties[q.Id - 1].Party)
                                .And(q => q.Relationship = project.Parties[q.Id - 1].Relationship == ProjectRelationship.PrincipalInvestigator 
                                    ? DataCollectionRelationshipType.Manager : PickHelper.RandomEnumExcept(DataCollectionRelationshipType.Manager, DataCollectionRelationshipType.None))
                                .Build());
                        })
                .Build();
            project.DataCollections.Add(dataCollection);
            dataCollection.Parties.Do(o => o.DataCollection = dataCollection);

            _dataCollectionRepository.Get(Arg.Is(dataCollection.Id)).Returns(dataCollection);
            return dataCollection;
        }
    }
}
