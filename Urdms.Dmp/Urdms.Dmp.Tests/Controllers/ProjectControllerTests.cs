using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using AutofacContrib.NSubstitute;
using Curtin.Framework.Common.Extensions;
using Curtin.Framework.Common.UserService;
using FizzWare.NBuilder;
using NSubstitute;
using NUnit.Framework;
using Urdms.Dmp.Controllers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Entities.Components;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Integration.UserService;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Controllers
{
    [TestFixture]
    class ProjectControllerShould
    {
        private ProjectController _controller;
        private DmpController _dmpController;
        private AutoSubstitute _autoSubstitute;
        private IProjectRepository _projectRepository;
        private ControllerContext _context;
        private ICurtinUserService _lookup;
        private NameValueCollection _form;
        private IFieldOfResearchRepository _fieldOfResearchRepository;
        private ISocioEconomicObjectiveRepository _socioEconomicObjectiveRepository;
        private const string UserId = "hsjd567";

        [SetUp]
        public void SetUp()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _controller = _autoSubstitute.GetController<ProjectController>();
            _dmpController = _autoSubstitute.GetController<DmpController>();
            _projectRepository = _autoSubstitute.Resolve<IProjectRepository>();
            _context = _autoSubstitute.Resolve<ControllerContext>();
            _form = _context.HttpContext.Request.Form;
            _lookup = _autoSubstitute.Resolve<ICurtinUserService>();
            _fieldOfResearchRepository = _autoSubstitute.Resolve<IFieldOfResearchRepository>();
            _socioEconomicObjectiveRepository = _autoSubstitute.Resolve<ISocioEconomicObjectiveRepository>();

            var appSettings = _autoSubstitute.Resolve<IAppSettingsService>();
            var dependencyResolver = _autoSubstitute.Resolve<IDependencyResolver>();
            DependencyResolver.SetResolver(dependencyResolver);
            dependencyResolver.GetService<IAppSettingsService>().Returns(appSettings);
        }

        private void CreateUser(string userId)
        {
            var user = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = userId).Build();
            UserIs.AuthenticatedAs(_autoSubstitute, userId, new[] { "Administrators" });
            _lookup.GetUser(Arg.Is(userId)).Returns(user);
        }

        [Test]
        public void Render_view_for_new_projects()
        {
            _controller.WithCallTo(c => c.NewProjects())
                .ShouldRenderDefaultView();
        }

        [Test]
        public void Render_empty_table_with_get_to_viewdmp_with_projects()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var projects = new List<Project>();
            _projectRepository.GetByPrincipalInvestigator(Arg.Is(userId)).Returns(projects);
            _controller.WithCallTo(c => c.Index()).ShouldRenderDefaultView();
        }

        [Test]
        public void Render_dmp_projects_list_with_get_to_viewdmp()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var member = Builder<ProjectParty>.CreateNew()
                .With(o => o.Party = Builder<Party>.CreateNew().With(p => p.UserId = userId).Build())
                .Build();
            var entities = Builder<Project>.CreateListOfSize(2)
                .All()
                .With(o => o.ProvisioningStatus = ProvisioningStatus.NotStarted)
                .TheFirst(1)
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>
                        .CreateNew()
                        .With(d => d.Id = 1)
                        .Build())
                .TheLast(1)
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>
                        .CreateNew()
                        .With(d => d.Id = 2)
                        .Build())
                .Build();

            foreach (var e in entities)
            {
                e.Parties.Add(member);
            }

            _projectRepository.GetByPrincipalInvestigator(Arg.Is(userId)).Returns(entities);
            _controller.WithCallTo(c => c.Index()).ShouldRenderDefaultView();
        }
        
        [Test]
        public void Render_view_for_an_existing_project()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var project = Builder<Project>.CreateNew()
                .With(o => o.SourceProjectType = PickHelper.RandomEnumExcept(SourceProjectType.None, SourceProjectType.DEPOSIT))
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            _projectRepository.Get(project.Id).Returns(project);
            _controller.WithCallTo(c => c.Project(project.Id))
                .ShouldRenderView("Project")
                .WithModel<ProjectDetailsViewModel>();
        }
        
        [Test]
        public void Not_render_view_for_an_existing_project_for_non_principal_investigator()
        {
            CreateUser("456787K");
            const string userId = "XX12345";
            var project = Builder<Project>.CreateNew()
               .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                          .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                          .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                          .Build()))
               .Build();
            _projectRepository.Get(project.Id).Returns(project);
            _controller.WithCallTo(c => c.Project(project.Id))
                .ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Save_project_details_with_post_and_redirect_to_copy_dmp_when_dmp_does_not_exist()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var project = Builder<Project>.CreateNew()
                .With(o => o.Id = 0)
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            _projectRepository.Get(project.Id).Returns(project);
            var vm = Builder<ProjectDetailsViewModel>.CreateNew()
                .With(o => o.Id = project.Id)
                .Build();
            _controller.WithCallTo(c => c.Project(vm))
                .ShouldRedirectTo(_controller.GetType().GetMethod("CopyDmp", new[] { typeof(int) }));
            _projectRepository.Received().Save(Arg.Any<Project>());
        }

        [Test]
        public void Not_save_project_details_for_non_principal_investigator()
        {
            CreateUser("KK45678");
            const string userId = "XX12345";
            var project = Builder<Project>.CreateNew()
               .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                          .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                          .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                          .Build()))
               .Build();
            _projectRepository.Get(project.Id).Returns(project);
            var vm = Builder<ProjectDetailsViewModel>.CreateNew().Build();
            _controller.WithCallTo(c => c.Project(vm))
                .ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Redirect_to_new_action_of_dmp_controller_with_get_call_to_copy_and_dmp_status_is_in_progress()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                .And(o => o.ProvisioningStatus = ProvisioningStatus.Pending)
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.CopyDmp(project.Id))
                .ShouldRedirectTo<DmpController>(_dmpController.GetType().GetMethod("New", new[] { typeof(int) }));
        }

        [Test]
        public void Save_and_proceed_to_dmp_without_using_an_existing_dmp_as_a_template()
        {
            var vm = Builder<CopyDataManagementPlanProjectViewModel>.CreateNew().Build();
            _form["saveAndProceed"] = "Save and Proceed to DMP";
            _controller.WithCallTo(c => c.CopyDmp(vm)).ShouldRedirectTo<DmpController>(_dmpController.GetType().GetMethod("New", new[] { typeof(int) }));
        }


        [Test]
        public void Copy_a_dmp_to_a_new_project()
        {
            var sourceProject = Builder<Project>
                .CreateNew()
                .With(x => x.Id = 1)
                .And(x => x.Title = "Project1")
                .And(x => x.DataManagementPlan = Builder<DataManagementPlan>
                        .CreateNew()
                        .With(d => d.Id = 1)
                        .And(d => d.Ethic = Builder<Ethic>.CreateNew().Build())
                        .Build())
                .Build();
            var destProject = Builder<Project>
                .CreateNew()
                .With(x => x.Id = 3)
                .And(x => x.Title = "Project3")
                .And(x => x.DataManagementPlan = Builder<DataManagementPlan>
                        .CreateNew()
                        .With(d => d.Id = 3)
                        .Build())
                .Build();
            var model = Builder<CopyDataManagementPlanProjectViewModel>
                .CreateNew()
                .With(x => x.DestinationProjectId = 3)
                .And(x => x.CopyFromExistingDmp = true)
                .And(x => x.AvailableProjects = Builder<ProjectListViewModel>
                    .CreateListOfSize(2)
                    .Build())
                .Build();
            _form["ProjectList"] = "1";
            _form["saveAndProceed"] = "Save and Proceed to DMP";
            _projectRepository.Get(1).Returns(sourceProject);
            _projectRepository.Get(model.DestinationProjectId).Returns(destProject);
            _controller.WithCallTo(c => c.CopyDmp(model)).ShouldRedirectTo<DmpController>(_dmpController.GetType().GetMethod("Edit", new[] { typeof(int), typeof(int) }));
        }
        

        [Test]
        public void Render_introduction()
        {
            _controller.WithCallTo(c => c.Introduction())
                .ShouldRenderView("ProjectIntroduction");
        }

        [Test]
        public void Render_project_action_redirect_when_posting_to_introduction()
        {
            _controller.WithCallTo(c => c.Introduction(new FormCollection()))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Project", new[] { typeof(int) }));

        }
        
        [Test]
        public void Should_add_seo_code()
        {
            var newSeoCode = new SocioEconomicObjective
                                 {
                                     Id = int.MaxValue.ToString(),
                                     Name = "New SeoCode"
                                 };
            var vm = GetProjectDetailsViewModel();
            vm.SocioEconomicObjectiveCode = newSeoCode.Id;
            _socioEconomicObjectiveRepository.GetSocioEconomicObjective(newSeoCode.Id).Returns(newSeoCode);
            _controller
                .WithCallTo(c => c.AddSeoCode(vm))
                .ShouldRenderView("Project");

            Assert.That(vm.SocioEconomicObjectives.Count(o => o.Code.Id == newSeoCode.Id && o.Code.Name == newSeoCode.Name), Is.EqualTo(1), "Seo Code not added");
        }

        [Test]
        public void Should_add_for_code()
        {
            var newFoRCode = new FieldOfResearch
                                 {
                                     Id = int.MaxValue.ToString(),
                                     Name = "New FoRCode"
                                 };
            var vm = GetProjectDetailsViewModel();
            vm.FieldOfResearchCode = newFoRCode.Id;
            _fieldOfResearchRepository.GetFieldOfResearch(newFoRCode.Id).Returns(newFoRCode);
            _controller
                .WithCallTo(c => c.AddForCode(vm))
                .ShouldRenderView("Project");

            Assert.That(vm.FieldsOfResearch.Count(o => o.Code.Id == newFoRCode.Id && o.Code.Name == newFoRCode.Name), Is.EqualTo(1), "FoR Code not added");
        }

        [Test]
        public void Should_delete_seo_codes()
        {
            var vm = GetProjectDetailsViewModel();
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
                .ShouldRenderView("Project");
            Assert.That(vm.SocioEconomicObjectives.Any(o => objectives.Any(q => q.Code.Id == o.Code.Id)), Is.False, "SeoCode not deleted");
            Assert.That(vm.SocioEconomicObjectives.Count, Is.EqualTo(expectedCount), "SeoCode lost");
        }

        [Test]
        public void Should_delete_for_codes()
        {
            var vm = GetProjectDetailsViewModel();
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
                .ShouldRenderView("Project");
            Assert.That(vm.FieldsOfResearch.Any(o => fieldsOfResearch.Any(q => q.Code.Id == o.Code.Id)), Is.False, "FoRCode not deleted");
            Assert.That(vm.FieldsOfResearch.Count, Is.EqualTo(expectedCount), "FoRCode lost");
        }

        [Test]
        public void Not_allow_copy_a_dmp_for_a_project_with_existing_dmp()
        {
            CreateUser("GA37493");
            var vm = GetProjectDetailsViewModel();
            var project = Builder<Project>.CreateNew()
                .With(o => o.SourceProjectType = SourceProjectType.DMP)
                .And(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                    .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(
                                               q =>
                                               q.Party =
                                               Builder<Party>.CreateNew().With(r => r.UserId = "GA37493").Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                                           .Build();

            _projectRepository.Get(vm.Id).Returns(project);

            _controller.WithCallTo(c => c.Project(vm)).ShouldRedirectTo<DmpController>(_dmpController.GetType().GetMethod("Edit", new [] { typeof(int),typeof(int) }));
        }

        [Test]
        public void Update_the_project()
        {
            var project = SetUpFullProjectWithAuthentication();

            var forCode = new ProjectFieldOfResearch
                              {
                                  Code = Builder<FieldOfResearch>.CreateNew()
                                      .With(o => o.Id = Pick<string>.RandomItemFrom(new[] {"ABC", "DEF", "GHI"}))
                                      .And(o => o.Name = string.Format("{0} Name", o.Id))
                                      .Build()
                              };

            var seoCode = new ProjectSocioEconomicObjective
                              {
                                  Code = Builder<SocioEconomicObjective>.CreateNew()
                                      .With(o => o.Id = Pick<string>.RandomItemFrom(new[] {"XYZ", "RST", "UVW"}))
                                      .And(o => o.Name = string.Format("{0} Name", o.Id))
                                      .Build()
                              };
            
            var vm = new ProjectDetailsViewModel
                            {
                                Id = project.Id,
                                Status = PickHelper.RandomEnumExcept(project.Status),
                                // deliberately change the source type to an invalid one
                                SourceProjectType = PickHelper.RandomEnumExcept(project.SourceProjectType, SourceProjectType.None),
                                EndDate = string.Format("{0:dd-MM-yyyy}", (project.EndDate ?? DateTime.Today).AddDays(10)),
                                StartDate = string.Format("{0:dd-MM-yyyy}",(project.StartDate ?? DateTime.Today).AddDays(-10)),
                                Description = "aut viam inveniam aut faciam",
                                FieldsOfResearch = project.FieldsOfResearch.Except(project.FieldsOfResearch.Take(1)).Union(new[] { forCode }).Cast<ClassificationBase>().ToList(),
                                SocioEconomicObjectives = project.SocioEconomicObjectives.Except(project.SocioEconomicObjectives.Take(1)).Union(new[] { seoCode }).Cast<ClassificationBase>().ToList(),
                                PrincipalInvestigator = project.Parties.Single(o => o.Relationship == ProjectRelationship.PrincipalInvestigator).Party,
                                ArcFunder = new ArcProjectFunderViewModel { GrantNumber = "BYC980", IsFunded = true},
                                NmhrcFunder = new NmhrcProjectFunderViewModel { IsFunded = false},
                                Keywords = "other keywords",
                                Title = "crede quod habes, et habes"
                            };

            _controller.WithCallTo(c => c.Project(vm)).ShouldRedirectTo<DmpController>(_dmpController.GetType().GetMethod("Edit", new[] { typeof(int), typeof(int) }));
            
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.Description == vm.Description));
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.Title == vm.Title));
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.StartDate == DateTime.Parse(vm.StartDate)));
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.EndDate == DateTime.Parse(vm.EndDate)));
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.Keywords == vm.Keywords));
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.Status == vm.Status));
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.SourceProjectType == SourceProjectType.DMP));
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.Funders.Count() == 1));
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.Funders.Count(o => o.Funder == Funder.ARC && o.GrantNumber == vm.ArcFunder.GrantNumber) == 1));
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.SocioEconomicObjectives.All(o => vm.SocioEconomicObjectives.Any(q => q.Code.Id == o.Code.Id))));
            _projectRepository.Received().Save(Arg.Is<Project>(p => p.FieldsOfResearch.All(o => vm.FieldsOfResearch.Any(q => q.Code.Id == o.Code.Id))));


        }

        [Test]
        public void Save_new_project_and_return_CopyDmp_action_on_post_to_project()
        {
            CreateUser(UserId);
            var vm = Builder<ProjectDetailsViewModel>.CreateNew()
                .With(x => x.Id = 0)
                .Build();
            var project = Builder<Project>.CreateNew()
                .With(x => x.Id = 0)
                .And(x => x.Parties.Add(ProjectParty()))
                .And(x => x.SourceProjectType = SourceProjectType.DMP)
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.Project(vm)).ShouldRedirectTo(_controller.GetType().GetMethod("CopyDmp", new[] { typeof(int) }));
        }

        private ProjectParty ProjectParty()
        {
            var party = Builder<Party>.CreateNew().With(x => x.UserId = UserId).Build();
            var projectParty = Builder<ProjectParty>.CreateNew()
                .With(x => x.Party = party)
                .And(x => x.Relationship = ProjectRelationship.PrincipalInvestigator)
                .Build();
            return projectParty;
        }

        private static ProjectDetailsViewModel GetProjectDetailsViewModel()
        {
            var vm = Builder<ProjectDetailsViewModel>.CreateNew().Build();
            vm.FieldsOfResearch.AddRange(GetForCodes());
            vm.SocioEconomicObjectives.AddRange(GetSeoCodes());
            return vm;

        }

        private static IEnumerable<ProjectFieldOfResearch> GetForCodes(int codeCount = 5)
        {
            var forCodes = Builder<FieldOfResearch>.CreateListOfSize(codeCount).Build();
            var projectForCodes = Builder<ProjectFieldOfResearch>.CreateListOfSize(codeCount).Build();
            for (int i = 0; i < codeCount; i++)
            {
                var projectForCode = projectForCodes[i];
                var forCode = forCodes[i];
                projectForCode.FieldOfResearch = forCode;
            }
            return projectForCodes;
        }

        private static IEnumerable<ProjectSocioEconomicObjective> GetSeoCodes(int codeCount = 3)
        {
            var seoCodes = Builder<SocioEconomicObjective>.CreateListOfSize(codeCount).Build();
            var projectSeoCodes = Builder<ProjectSocioEconomicObjective>.CreateListOfSize(codeCount).Build();
            for (int i = 0; i < codeCount; i++)
            {
                var projectSeoCode = projectSeoCodes[i];
                var seoCode = seoCodes[i];
                projectSeoCode.SocioEconomicObjective = seoCode;
            }
            return projectSeoCodes;
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
                    .And(q => q.DepositToRepository = PickHelper.RandomBoolean())
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
                    .And(o => o.FirstName = Pick<string>.RandomItemFrom(new[]{"Alan","Albert","Adrian"}))
                    .And(o => o.LastName = Pick<string>.RandomItemFrom(new[]{"Wallace","Willis","Waylan"}))
                .TheNext(3)
                    .And(o => o.FirstName = Pick<string>.RandomItemFrom(new[] { "Bastian", "Bruce", "Brian","Julian","James","Jones" }))
                    .And(o => o.LastName = Pick<string>.RandomItemFrom(new[] { "Dallas", "Donga", "Dulles", "Frost","Feller","Frist" }))
                .TheNext(3)
                    .With(o => o.UserId = null)
                    .And(o => o.Organisation = null)
                    .And(o => o.Email = null)
                    .And(o => o.FirstName = Pick<string>.RandomItemFrom(new[]{"George","Gerald","Gordon","Hally","Harvey","Harry"}))
                    .And(o => o.LastName = Pick<string>.RandomItemFrom(new []{"Pepper","Prince","Pulse","Tommy","Thors","Tallis"}))
                .All()
                    .With(o => o.FullName = string.Format("{0} {1}", o.FirstName, o.LastName))
                .TheFirst(4)
                    .With(o => o.Organisation = "Your University")
                    .And(o => o.Email = string.Format("{0}.{1}@domain.edu.au",o.FirstName, o.LastName))
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
                                    .With(q => q.Relationship = Pick<ProjectRelationship>.RandomItemFrom(new[]{ProjectRelationship.Student, ProjectRelationship.SupportStaff, ProjectRelationship.Investigator, ProjectRelationship.PartnerInvestigator}))
                                    .And(q => q.Role = Pick<AccessRole>.RandomItemFrom(new[]{AccessRole.Members, AccessRole.Visitors}))
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

            project.Parties.Do(o => o.Project = project);
            _projectRepository.Get(Arg.Is(project.Id)).Returns(project);

            return project;
        }

    }
}
