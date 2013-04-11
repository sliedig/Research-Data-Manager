using System;
using System.Linq;
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
using Urdms.Dmp.Models.DataDeposit;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Controllers
{
    [TestFixture]
    class DataDepositControllerShould
    {
        private AutoSubstitute _autoSubstitute;
        private DataDepositController _controller;
        private ConfirmController _confirmController;
        private ProjectController _projectController;
        private ControllerContext _context;
        private ICurtinUserService _lookup;
        private IProjectRepository _projectRepository;
        private const int ProjectId = 1;
        private const string UserId = "XX12345";
        
        [SetUp]
        public void SetUp()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _controller = _autoSubstitute.GetController<DataDepositController>();
            _confirmController = _autoSubstitute.GetController<ConfirmController>();
            _projectController = _autoSubstitute.GetController<ProjectController>();
            _context = _autoSubstitute.Resolve<ControllerContext>();
            _lookup = _autoSubstitute.Resolve<ICurtinUserService>();
            _projectRepository = _autoSubstitute.Resolve<IProjectRepository>();
        }

        private void CreateUser(string userId)
        {
            var user = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = userId).Build();
            UserIs.AuthenticatedAs(_autoSubstitute, userId, new[] { "Administrators" });
            _lookup.GetUser(Arg.Is(userId)).Returns(user);
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
        public void Render_project_view_with_get_to_project_for_new_project()
        {
            _controller.WithCallTo(c => c.Project((int?) null)).ShouldRenderView("Project").WithModel<ProjectDetailsViewModel>();
        }

        [Test]
        public void Render_project_view_with_get_to_project_for_existing_project()
        {
            CreateUser(UserId);
            var project = Builder<Project>.CreateNew()
                .With(p => p.Id = 1)
                .And(p => p.SourceProjectType = SourceProjectType.DEPOSIT)
                .And(p => p.Parties.Add(ProjectParty()))
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.Project(project.Id)).ShouldRenderView("Project").WithModel<ProjectDetailsViewModel>();
        }

        [Test]
        public void Render_ProjectNotFound_view_with_get_to_project_when_project_not_found()
        {
            const int projectId = 1;
            _projectRepository.Get(projectId).Returns(x => null);

            _controller.WithCallTo(c => c.Project(projectId)).ShouldRenderView("ProjectNotFound");
        }

        [Test]
        public void Render_IncorrectProjectType_view_with_get_to_project_when_project_not_of_data_deposit_type()
        {
            var project = Builder<Project>.CreateNew().With(x => x.Id = 1)
                .With(o => o.SourceProjectType = SourceProjectType.DMP)
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.Project(project.Id)).ShouldRenderView("IncorrectProjectType");
        }

        [Test]
        public void Render_NoProjectAccessRight_view_with_get_to_project_for_existing_project_but_invalid_user()
        {
            var project = Builder<Project>.CreateNew().With(x => x.Id = 1)
                .With(o => o.SourceProjectType = SourceProjectType.DEPOSIT)
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.Project(project.Id)).ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Save_new_project_and_return_New_action_on_post_to_project()
        {
            CreateUser(UserId);
            var vm = Builder<ProjectDetailsViewModel>.CreateNew()
                .With(x => x.Id = 0)
                .Build();
            var project = Builder<Project>.CreateNew()
                .With(x => x.Id = 0)
                .And(x => x.SourceProjectType = SourceProjectType.DEPOSIT)
                .And(x => x.Parties.Add(ProjectParty()))
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.Project(vm)).ShouldRedirectTo(_controller.GetType().GetMethod("New", new[] { typeof(int) }));
        }

        [Test]
        public void Render_NoProjectAccessRight_view_with_post_to_project_for_invalid_user()
        {
            CreateUser("234567k");
            var vm = Builder<ProjectDetailsViewModel>.CreateNew()
                .With(x => x.Id = 1)
                .Build();
            var project = Builder<Project>.CreateNew()
                .With(x => x.Id = 1)
                .And(x => x.SourceProjectType = SourceProjectType.DEPOSIT)
                .And(x => x.Parties.Add(ProjectParty()))
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.Project(vm)).ShouldRenderView("NoProjectAccessRight");
        }

#region New
        [Test]
        public void Return_ProjectNotFound_view_with_get_to_new_when_invalid_project_id_passed()
        {
            _projectRepository.Get(ProjectId).Returns(n => null);

            _controller.WithCallTo(c => c.New(ProjectId)).ShouldRenderView("ProjectNotFound");
        }

        [Test]
        public void Return_NoProjectAccessRight_view_with_get_to_new_with_invalid_user()
        {
            CreateUser("234567k");
            var project = Builder<Project>.CreateNew()
                .With(x => x.Id = 1)
                .And(x => x.Parties.Add(ProjectParty()))
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.New(ProjectId)).ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Return_CannotCreateDataDeposit_view_with_get_to_new_with_invalid_project_type()
        {
            CreateUser(UserId);
            var project = Builder<Project>.CreateNew()
                .With(x => x.Id = 1)
                .And(x => x.Parties.Add(ProjectParty()))
                .And(x => x.SourceProjectType = SourceProjectType.DMP)
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.New(ProjectId)).ShouldRenderView("CannotCreateDataDeposit");
        }

        [Test]
        public void Return_DataDepositAlreadyExists_view_with_get_to_new_with_completed_data_deposit_project()
        {
            CreateUser(UserId);
            var project = Builder<Project>.CreateNew()
                .With(x => x.Id = 1)
                .And(x => x.Parties.Add(ProjectParty()))
                .And(x => x.SourceProjectType = SourceProjectType.DEPOSIT)
                .And(x => x.DataDeposit = Builder<DataDeposit>.CreateNew().Build())
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.New(ProjectId)).ShouldRenderView("DataDepositAlreadyExists");
        }

        [Test]
        public void Render_default_view_with_get_to_new_with_valid_project()
        {
            CreateUser(UserId);
            var project = Builder<Project>.CreateNew()
                .With(x => x.Id = 1)
                .And(x => x.Parties.Add(ProjectParty()))
                .And(x => x.SourceProjectType = SourceProjectType.DEPOSIT)
                .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.New(ProjectId)).ShouldRenderDefaultView().WithModel<DataDepositViewModel>();
        }

        [Test]
        public void Return_ProjectNotFound_on_post_to_new_with_invalid_project_id()
        {
            var vm = Builder<DataDepositViewModel>.CreateNew().Build();

            _projectRepository.Get(vm.ProjectId).Returns(n => null);

            _controller.WithCallTo(c => c.New(vm)).ShouldRenderView("ProjectNotFound");
        }

        [Test]
        public void Return_NoProjectAccessRight_on_post_to_new_when_user_is_not_the_principal_investigator()
        {
            CreateUser(UserId);
            var vm = Builder<DataDepositViewModel>.CreateNew().Build();
            var party = Builder<ProjectParty>.CreateNew()
                .With(pp => pp.Role = AccessRole.Owners)
                .And(pp => pp.Party = new Party {UserId = "other"})
                .Build();
            var project = Builder<Project>.CreateNew()
                .With(p => p.Id = vm.ProjectId)
                .And(p => p.Parties.Add(party))
                .Build();

            _projectRepository.Get(vm.ProjectId).Returns(project);

            _controller.WithCallTo(c => c.New(vm)).ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Return_CannotCreateDataDeposit_view_on_post_to_new_with_invalid_project_type()
        {
            CreateUser(UserId);
            var vm = Builder<DataDepositViewModel>.CreateNew().Build();
            var project = Builder<Project>.CreateNew()
                .With(x => x.Id = vm.ProjectId)
                .And(x => x.Parties.Add(ProjectParty()))
                .And(x => x.SourceProjectType = SourceProjectType.DMP)
                .Build();

            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.New(vm)).ShouldRenderView("CannotCreateDataDeposit");
        }

        [Test]
        public void Return_DataDepositAlreadyExists_view_on_post_to_new_with_completed_data_deposit_project()
        {
            CreateUser(UserId);
            var vm = Builder<DataDepositViewModel>.CreateNew().Build();
            var project = Builder<Project>.CreateNew()
                .With(x => x.Id = vm.ProjectId)
                .And(x => x.Parties.Add(ProjectParty()))
                .And(x => x.SourceProjectType = SourceProjectType.DEPOSIT)
                .And(x => x.DataDeposit = Builder<DataDeposit>.CreateNew().Build())
                .Build();

            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.New(vm)).ShouldRenderView("DataDepositAlreadyExists");
        }

        [Test]
        public void Save_DataDeposit_and_redirect_confirm_controller_confirm_action_on_post_to_New()
        {
            CreateUser(UserId);
            var user1 = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = "123456A").Build();
            _lookup.GetUser(Arg.Is(user1.CurtinId)).Returns(user1);
			var user2 = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = "231231B").Build();
            _lookup.GetUser(Arg.Is(user2.CurtinId)).Returns(user2);
			var user3 = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = null).Build();
            _lookup.GetUser(Arg.Is(user3.CurtinId)).Returns(user3);
            var vm = Builder<DataDepositViewModel>.CreateNew()
                .With(x => x.Id = 1)
                .And(x => x.PrincipalInvestigator = Builder<Party>.CreateNew().With(o => o.UserId = UserId).Build())
                .And(x => x.UrdmsUsers = Builder<UrdmsUserViewModel>.CreateListOfSize(2)
                    .TheFirst(1)
                    .With(u => u.UserId = user1.CurtinId)
                    .And(u => u.FullName = user1.FirstName)
                    .TheNext(1)
                    .With(u => u.UserId = user2.CurtinId)
                    .And(u => u.FullName = user2.FirstName)
                    .Build())
                .And(x => x.NonUrdmsUsers = Builder<NonUrdmsUserViewModel>.CreateListOfSize(1)
                    .TheFirst(1)
                    .With(u => u.FullName = user3.FirstName)
                    .Build())
                .Build();
            var project = Builder<Project>.CreateNew()
               .With(x => x.Id = 1)
               .And(x => x.Parties.Add(ProjectParty()))
               .And(x => x.SourceProjectType = SourceProjectType.DEPOSIT)
               .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.New(vm)).ShouldRedirectTo<ConfirmController>(_confirmController.GetType().GetMethod("ReviewDataDeposit", new[] { typeof(int) }));
        }
#endregion

        [Test]
        public void Render_default_view_with_post_to_AddUrdmsUser_when_input_empty()
        {
            var vm = Builder<DataDepositViewModel>.CreateNew().With(o => o.FindUserId = string.Empty).Build();

			_controller.WithCallTo(c => c.AddUrdmsUser(vm)).ShouldRenderView("New").WithModel<DataDepositViewModel>();
        }

        [Test]
		public void Render_ProjectNotFound_view_with_post_to_AddUrdmsUser_when_project_is_invalid()
        {
            var vm = Builder<DataDepositViewModel>.CreateNew().Build();
            _projectRepository.Get(vm.ProjectId).Returns(x => null);

			_controller.WithCallTo(c => c.AddUrdmsUser(vm)).ShouldRenderView("ProjectNotFound");
        }

        [Test]
        public void Update_datadeposit_project()
        {
            var project = SetUpFullProjectWithAuthentication();
            
            var forCode = new ProjectFieldOfResearch
            {
                Code = Builder<FieldOfResearch>.CreateNew()
                    .With(o => o.Id = Pick<string>.RandomItemFrom(new[] { "ABC", "DEF", "GHI" }))
                    .And(o => o.Name = string.Format("{0} Name", o.Id))
                    .Build()
            };

            var seoCode = new ProjectSocioEconomicObjective
            {
                Code = Builder<SocioEconomicObjective>.CreateNew()
                    .With(o => o.Id = Pick<string>.RandomItemFrom(new[] { "XYZ", "RST", "UVW" }))
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
                             StartDate = string.Format("{0:dd-MM-yyyy}", (project.StartDate ?? DateTime.Today).AddDays(-10)),
                             Description = "aut viam inveniam aut faciam",
                             FieldsOfResearch = project.FieldsOfResearch.Except(project.FieldsOfResearch.Take(1)).Union(new[] {forCode}).Cast<ClassificationBase>().ToList(),
                             SocioEconomicObjectives = project.SocioEconomicObjectives.Except(project.SocioEconomicObjectives.Take(1)).Union(new[] {seoCode}).Cast<ClassificationBase>().ToList(),
                             PrincipalInvestigator = project.Parties.Single(o => o.Relationship == ProjectRelationship.PrincipalInvestigator).Party,
                             ArcFunder = new ArcProjectFunderViewModel {GrantNumber = "BYC980", IsFunded = true},
                             NmhrcFunder = new NmhrcProjectFunderViewModel {IsFunded = false},
                             Keywords = "other keywords",
                             Title = "crede quod habes, et habes"
                         };

            _controller.WithCallTo(c => c.Project(vm)).ShouldRedirectTo<ProjectController>(typeof(ProjectController).GetMethod("Index"));

            _projectRepository.Received().Save(Arg.Is<Project>(p =>
                   p.Description == vm.Description &&
                   p.Title == vm.Title &&
                   p.StartDate == DateTime.Parse(vm.StartDate) &&
                   p.EndDate == DateTime.Parse(vm.EndDate) &&
                   p.Keywords == vm.Keywords &&
                   p.Status == ProjectStatus.Completed &&
                   p.SourceProjectType == SourceProjectType.DEPOSIT &&
                   p.Funders.Count() == 0 &&
                   p.SocioEconomicObjectives.All(o => vm.SocioEconomicObjectives.Any(q => q.Code.Id == o.Code.Id)) &&
                   p.FieldsOfResearch.All(o => vm.FieldsOfResearch.Any(q => q.Code.Id == o.Code.Id))));
        }

        [Test]
        public void Render_datadeposit_edit_view_with_get_to_edit_datadeposit()
        {
            CreateUser(UserId);
            var project = Builder<Project>.CreateNew()
                .With(p => p.Id = 1)
                .And(p => p.SourceProjectType = SourceProjectType.DEPOSIT)
                .And(p => p.DataDeposit = Builder<DataDeposit>.CreateNew().Build())
                .And(p => p.Parties.Add(ProjectParty()))
                .Build();
            _projectRepository.GetByDataDepositId(project.DataDeposit.Id).Returns(project);

            _controller.WithCallTo(c => c.Edit(project.DataDeposit.Id)).ShouldRenderView("New").WithModel<DataDepositViewModel>();
        }

        [Test]
        public void Render_CannotEditDataDeposit_view_with_get_to_edit_datadeposit()
        {
            CreateUser(UserId);
            var project = Builder<Project>.CreateNew()
                .With(p => p.Id = 1)
                .And(p => p.SourceProjectType = SourceProjectType.DEPOSIT)
                .And(p => p.DataDeposit = Builder<DataDeposit>.CreateNew().Build())
                .And(p => p.Parties.Add(ProjectParty()))
                .And(p => p.ProvisioningStatus = ProvisioningStatus.Pending)
                .Build();
            _projectRepository.GetByDataDepositId(project.DataDeposit.Id).Returns(project);

            _controller.WithCallTo(c => c.Edit(project.DataDeposit.Id)).ShouldRenderView("CannotEditDataDeposit");
        }

        private Project SetUpFullProjectWithAuthentication(string userId = "XX12345")
        {
            CreateUser(userId);
            var dataDeposit = Builder<DataDeposit>.CreateNew()
                .With(o => o.Availability = DataSharingAvailability.AfterASpecifiedEmbargoPeriod)
                .And(o => o.AvailabilityDate = DateTime.Today.AddMonths(9))
                .And(o => o.LicensingArrangement = PickHelper.RandomEnumExcept<DataLicensingType>())
                .And(o => o.MaxDataSize = PickHelper.RandomEnumExcept(MaxDataSize.None))
                .And(o => o.ShareAccess = PickHelper.RandomEnumExcept(ShareAccess.NoAccess))
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
                .With(o => o.DataDeposit = dataDeposit)
                .And(o => o.ProvisioningStatus = PickHelper.RandomEnumExcept<ProvisioningStatus>())
                .And(o => o.Status = ProjectStatus.Completed)
                .And(o => o.SourceProjectType = SourceProjectType.DEPOSIT)
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

                   
                }).Build();

            _projectRepository.Get(Arg.Is(project.Id)).Returns(project);

            return project;
        }
    }
}
