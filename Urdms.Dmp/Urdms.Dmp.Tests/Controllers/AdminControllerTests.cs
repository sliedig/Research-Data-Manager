using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web.Mvc;
using AutofacContrib.NSubstitute;
using FizzWare.NBuilder;
using NSubstitute;
using NUnit.Framework;
using Urdms.Dmp.Controllers;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Entities.Components;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Controllers
{
    [TestFixture]
    public class AdminControllerShould
    {
        private AutoSubstitute _autoSubstitute;
        private AdminController _controller;
        private IProjectRepository _projectRepository;
        private IDataCollectionRepository _dataCollectionRepository ;
        private ControllerContext _context;
        private NameValueCollection _form;
        private ICsvHelper _csvHelper;

        [SetUp]
        public void Setup()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _projectRepository = _autoSubstitute.Resolve<IProjectRepository>();
            _dataCollectionRepository = _autoSubstitute.Resolve<IDataCollectionRepository>();
            _controller = _autoSubstitute.GetController<AdminController>();
            _context = _autoSubstitute.Resolve<ControllerContext>();
            _form = _context.HttpContext.Request.Form;
            _csvHelper = _autoSubstitute.Resolve<ICsvHelper>();
        }
        
        [Test]
        public void Render_default_view_for_get_to_Index()
        {
            var projects = Builder<Project>.CreateListOfSize(5)
               .All()
               .With(p => p.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
               .Build();
            _projectRepository.GetAll().Returns(projects);

            var dataCollections = Builder<DataCollection>.CreateListOfSize(5)
               .All()
               .Build();
            _dataCollectionRepository.GetAll().Returns(dataCollections);

            _controller.WithCallTo(c => c.Index()).ShouldRenderDefaultView();
        }

        [Test]
        public void Render_CSV_file_for_post_to_DataManagementPlanToCsv()
        {
            var vm = new CsvDumpViewModel {Projects = Builder<DmpListViewModel>.CreateListOfSize(3).Build()};
            _form["DataManagementPlanCsv"] = "Get Data Management Plan to CSV";
            _form["ProjectList"] = "1";
            var project = SetUpFullProject("GA37493");
            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);
            _csvHelper.ExportToCsv(Arg.Any<DataTable>()).Returns("");
            _csvHelper.DataManagementPlanToDataTable(Arg.Any<DataManagementPlan>(), Arg.Any<IList<ProjectParty>>()).Returns(new DataTable());
            _controller.WithCallTo(c => c.DataManagementPlanToCsv(vm)).ShouldRenderFile("text/csv");
            
            _csvHelper.Received().ExportToCsv(Arg.Any<DataTable>(), Arg.Any<string>());
            _csvHelper.Received().DataManagementPlanToDataTable(Arg.Any<DataManagementPlan>(), Arg.Any<IList<ProjectParty>>());
            const string headerValue = "attachment;filename=Feeding_habits_of_polarbears_2011_DataManagementPlan.csv";

            _context.HttpContext.Response.Received().AddHeader(Arg.Any<string>(), Arg.Is<string>(o => o == headerValue));
       }
        
        [Test]
        public void Render_DmpNotFound_view_for_post_to_Index_with_project_without_Dmp()
        {
            var vm = new CsvDumpViewModel { Projects = Builder<DmpListViewModel>.CreateListOfSize(3).Build() };
            _form["DataManagementPlanCsv"] = "Get Data Management Plan to CSV";
            _form["ProjectList"] = "5";
            _projectRepository.GetByDataManagementPlanId(5).Returns(x => null);
            var project = SetUpFullProject("GA37493");
            project.DataManagementPlan = null;
            _csvHelper.ExportToCsv(Arg.Any<DataTable>()).Returns("");
            _csvHelper.DataManagementPlanToDataTable(Arg.Any<DataManagementPlan>(), Arg.Any<IList<ProjectParty>>()).Returns(new DataTable());

            _controller.WithCallTo(c => c.DataManagementPlanToCsv(vm)).ShouldRenderView("DmpNotFound");

            _csvHelper.DidNotReceive().ExportToCsv(Arg.Any<DataTable>(), Arg.Any<string>());
            _csvHelper.DidNotReceive().DataManagementPlanToDataTable(Arg.Any<DataManagementPlan>(), Arg.Any<IList<ProjectParty>>());
        }

        [Test]
        public void Redirect_to_Index_for_post_to_DataManagementPlansToCsv_when_no_Projects()
        {
            var vm = new CsvDumpViewModel { Projects =new List<DmpListViewModel>() };
            _projectRepository.GetAll().Returns(new List<Project>());

            _controller.WithCallTo(c => c.AllDataManagementPlansToCsv(vm)).ShouldRedirectTo(_controller.GetType().GetMethod("Index"));
        }

        [Test]
        public void Render_CSV_file_for_post_to_DataManagementPlansToCsv()
        {
            var vm = new CsvDumpViewModel { Projects = Builder<DmpListViewModel>.CreateListOfSize(3).Build() };
            var projects = Builder<Project>.CreateListOfSize(5)
                .All()
                .With(p => p.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                .Build();
            _projectRepository.GetAll().Returns(projects);
            _csvHelper.ExportToCsv(Arg.Any<DataTable>()).Returns("");
            _csvHelper.DataManagementPlansToDataTable(Arg.Any<IList<Project>>()).Returns(new DataTable());
            _controller.WithCallTo(c => c.AllDataManagementPlansToCsv(vm)).ShouldRenderFile("text/csv");

            _csvHelper.Received().ExportToCsv(Arg.Any<DataTable>(), Arg.Any<string>());
            _csvHelper.Received().DataManagementPlansToDataTable(Arg.Any<IList<Project>>());
            
            _context.HttpContext.Response.Received().AddHeader(Arg.Any<string>(), Arg.Any<string>());
        }

        [Test]
        public void Render_CSV_file_for_post_to_DataCollectionToCsv()
        {
            var vm = new CsvDumpViewModel { DataCollections = Builder<CollectionListViewModel>.CreateListOfSize(3).Build() };
            _form["DataCollectionCsv"] = "Get Data Management Plan to CSV";
            _form["DataCollectionList"] = "1";
            var dataCollection = SetUpDataCollection("GA37493");
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);
            _csvHelper.ExportToCsv(Arg.Any<DataTable>()).Returns("");
            _csvHelper.DataCollectionToDataTable(Arg.Any<DataCollection>()).Returns(new DataTable());
            _controller.WithCallTo(c => c.DataCollectionToCsv(vm)).ShouldRenderFile("text/csv");

            _csvHelper.Received().ExportToCsv(Arg.Any<DataTable>(), Arg.Any<string>());
            _csvHelper.Received().DataCollectionToDataTable(Arg.Any<DataCollection>());
            const string headerValue = "attachment;filename=Feeding_habits_of_polarbears_2011_DataCollection.csv";

            _context.HttpContext.Response.Received().AddHeader(Arg.Any<string>(), Arg.Is<string>(o => o == headerValue));
        }

        [Test]
        public void Redirect_to_Index_for_post_to_DataCollectionsToCsv_when_no_DataCollections()
        {
            var vm = new CsvDumpViewModel { DataCollections = new List<CollectionListViewModel>() };
            _dataCollectionRepository.GetAll().Returns(new List<DataCollection>());
          
            _controller.WithCallTo(c => c.AllDataCollectionsToCsv(vm)).ShouldRedirectTo(_controller.GetType().GetMethod("Index"));
        }

        [Test]
        public void Render_CSV_file_for_post_to_DataCollectionsToCsv()
        {
            var vm = new CsvDumpViewModel { DataCollections = Builder<CollectionListViewModel>.CreateListOfSize(3).Build() };
            var dataCollections = Builder<DataCollection>.CreateListOfSize(5)
                .All()
                .Build();
            _dataCollectionRepository.GetAll().Returns(dataCollections);
            _csvHelper.ExportToCsv(Arg.Any<DataTable>()).Returns("");
            _csvHelper.DataCollectionsToDataTable(Arg.Any<IList<DataCollection>>()).Returns(new DataTable());
            _controller.WithCallTo(c => c.AllDataCollectionsToCsv(vm)).ShouldRenderFile("text/csv");

            _csvHelper.Received().ExportToCsv(Arg.Any<DataTable>(), Arg.Any<string>());
            _csvHelper.Received().DataCollectionsToDataTable(Arg.Any<IList<DataCollection>>());

            _context.HttpContext.Response.Received().AddHeader(Arg.Any<string>(), Arg.Any<string>());
        }

        private Project SetUpFullProject(string curtinId)
        {
            var dmp = Builder<DataManagementPlan>.CreateNew()
                .With(o => o.ExistingDataDetail = Builder<ExistingDataDetail>.CreateNew()
                    .With(q => q.UseExistingData = PickHelper.RandomBoolean())
                    .And(q => q.ExistingDataAccessTypes = PickHelper.RandomEnumsExcept(ExistingDataAccessTypes.None))
                    .Build())
                .And(o => o.BackupPolicy = Builder<BackupPolicy>.CreateNew().Build())
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

            var projectParties = Builder<ProjectParty>.CreateListOfSize(5)
                .TheFirst(1)
                .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = curtinId).Build())
                .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                .TheLast(4)
                .With(q => q.Party = Builder<Party>.CreateNew().Build())
                .And(q => q.Relationship = PickHelper.RandomEnumExcept(ProjectRelationship.None, ProjectRelationship.PrincipalInvestigator))
                .Build();

            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = dmp)
                .And(o => o.Title = "Feeding habits of polarbears (2011)")
                .And(o => o.ProvisioningStatus = PickHelper.RandomEnumExcept<ProvisioningStatus>())
                .Do(o => o.Parties.AddRange(projectParties))
                .Build();

            return project;
        }

        private DataCollection SetUpDataCollection(string curtinId)
        {
            var dataCollectionParties = Builder<DataCollectionParty>.CreateListOfSize(5)
                .TheFirst(1)
                .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = curtinId).Build())
                .And(q => q.Relationship = DataCollectionRelationshipType.Manager)
                .TheLast(4)
                .With(q => q.Party = Builder<Party>.CreateNew().Build())
                .And(q => q.Relationship = PickHelper.RandomEnumExcept(DataCollectionRelationshipType.None, DataCollectionRelationshipType.Manager))
                .Build();
            var forCodes = Builder<DataCollectionFieldOfResearch>.CreateListOfSize(5)
                .All()
                .With(f => f.FieldOfResearch = Builder<FieldOfResearch>.CreateNew().Build())
                .Build();
            var seoCodes = Builder<DataCollectionSocioEconomicObjective>.CreateListOfSize(5)
                .All()
                .With(f => f.SocioEconomicObjective = Builder<SocioEconomicObjective>.CreateNew().Build())
                .Build();
            var dataCollection = Builder<DataCollection>.CreateNew()
                .Do(d => d.Parties.AddRange(dataCollectionParties))
                .And(o => o.Title = "Feeding habits of polarbears (2011)")
                .And(d => d.FieldsOfResearch.AddRange(forCodes))
                .And(d => d.SocioEconomicObjectives.AddRange(seoCodes))
                .Build();
            return dataCollection;
        }

    }
}
