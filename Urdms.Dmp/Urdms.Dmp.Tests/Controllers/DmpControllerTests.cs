using System;
using System.Collections.Specialized;
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

namespace Urdms.Dmp.Tests.Controllers
{

    [TestFixture]
    class DmpControllerShould
    {
        private AutoSubstitute _autoSubstitute;
        private DmpController _controller;
        private ITimerRepository _timerRepository;
        private ControllerContext _context;
        private const int InvalidProjectId = 5;
        private NameValueCollection _form;
        private ICurtinUserService _lookup;
        private IProjectRepository _projectRepository;

        [SetUp]
        public void SetUp()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _controller = _autoSubstitute.GetController<DmpController>();
            _projectRepository = _autoSubstitute.Resolve<IProjectRepository>();
            _timerRepository = _autoSubstitute.Resolve<ITimerRepository>();
            _context = _autoSubstitute.Resolve<ControllerContext>();
            _form = _context.HttpContext.Request.Form;
            _lookup = _autoSubstitute.Resolve<ICurtinUserService>();
        }



        [Test]
        public void Render_project_not_found_view_when_new_called_with_invalid_id()
        {
            _projectRepository.Get(InvalidProjectId).Returns(x => null);

            _controller.WithCallTo(c => c.New(InvalidProjectId)).ShouldRenderView("ProjectNotFound");
        }

        [Test]
        public void Redirect_to_edit_for_existing_project_when_new_called_with_invalid_id()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var project = Builder<Project>.CreateNew()
                 .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                            .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                            .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                            .Build()))
                 .Build();
            _projectRepository.Get(project.Id).Returns(project);

            _controller.WithCallTo(c => c.New(project.Id))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Edit", new[] { typeof(int), typeof(int) }));
        }

        [Test]
        public void Show_no_access_for_non_principal_investigator_when_attempting_to_create_dmp()
        {
            const string userId = "XX12345";
            CreateUser("456789");
            var project = Builder<Project>.CreateNew()
                 .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                            .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                            .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                            .Build()))
                 .Build();
            _projectRepository.Get(project.Id).Returns(project);
            _controller.WithCallTo(c => c.New(project.Id))
                .ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Throw_exception_when_trying_saving_invalid_model()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                 .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                            .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                            .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                            .Build()))
                 .Build();

            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);
            var model = Builder<DataManagementPlanViewModel>.CreateNew().With(o => o.Step = 2).Build();
            _controller.ModelState.AddModelError("", "");
            _controller.WithCallTo(c => c.Edit(model)).ShouldRenderView("Step" + model.Step);
        }

        [Test]
        public void Save_valid_model_and_increment_next_step()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                 .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                            .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                            .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                            .Build()))
                 .Build();

            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);
            _controller.HttpContext.Request.Form.Add("stepAction", "Save and Next");

            for (int i = 1; i <= DmpController.MaxStep; i++)
            {
                var model = Builder<DataManagementPlanViewModel>.CreateNew()
                    .With(o => o.Id = project.DataManagementPlan.Id)
                    .And(o => o.Step = i)
                    .Build();

                _controller.WithCallTo(c => c.Edit(model)).ShouldRedirectTo(_controller.GetType().GetMethod("Edit", new[] { typeof(int), typeof(int) }));
                _projectRepository.Received().Save(Arg.Any<Project>());

                if (i != DmpController.MaxStep)
                {
                    Assert.That(model.Step == i + 1);
                }
                else
                {
                    Assert.That(model.Step, Is.EqualTo(i));
                }
            }
        }

        [Test]
        public void Save_valid_model_and_decrement_next_step()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                 .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                            .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                            .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                            .Build()))
                 .Build();
            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);
            _controller.HttpContext.Request.Form.Add("stepAction", "Save and Previous");

            for (int i = DmpController.MaxStep; i > 0; i--)
            {
                var model = Builder<DataManagementPlanViewModel>.CreateNew()
                    .With(o => o.Id = project.DataManagementPlan.Id)
                    .And(o => o.Step = i)
                    .Build();

                _controller.WithCallTo(c => c.Edit(model)).ShouldRedirectTo(_controller.GetType().GetMethod("Edit", new[] { typeof(int), typeof(int) }));
                _projectRepository.Received().Save(Arg.Any<Project>());

                if (i > 1)
                {
                    Assert.That(model.Step == i - 1);
                }
                else
                {
                    Assert.That(model.Step, Is.EqualTo(i));
                }
            }
        }

        [Test]
        public void Render_error_view_when_new_called_and_exception_thrown()
        {
            _projectRepository.Get(InvalidProjectId).Returns(d => { throw new Exception(); });
            _controller.WithCallTo(c => c.New(InvalidProjectId)).ShouldRenderView("Error");


        }

        [Test]
        public void Render_IncorrectProjectType_when_new_called_for_data_deposit_project()
        {
            var project = Builder<Project>.CreateNew()
                .With(o => o.SourceProjectType = SourceProjectType.DEPOSIT)
                .Build();

            _projectRepository.Get(Arg.Is(project.Id)).Returns(project);
            _controller.WithCallTo(c => c.New(project.Id)).ShouldRenderView("IncorrectProjectType");

            _projectRepository.Received().Get(Arg.Is(project.Id));
        }

        [Test]
        public void Render_step1_view_when_edit_called_with_valid_dmpid_and_step()
        {
            const int step = 1;
            const string userId = "XX12345";
            CreateUser(userId);
            var dmp = Builder<DataManagementPlan>.CreateNew()
               .With(o => o.NewDataDetail = new NewDataDetail())
               .With(o => o.ExistingDataDetail = new ExistingDataDetail())
               .With(o => o.DataRelationshipDetail = new DataRelationshipDetail()).Build();
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = dmp)
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);

            _controller.WithCallTo(c => c.Edit(dmp.Id, step)).ShouldRenderView("Step1").WithModel<DataManagementPlanViewModel>(m =>
            {
                Assert.That(m.Step, Is.EqualTo(step));
                Assert.That(m.Id, Is.EqualTo(dmp.Id));
                Assert.That(m.NewDataDetail, Is.Not.Null, "NewDataDetail should not be null");
                Assert.That(m.ExistingDataDetail, Is.Not.Null, "ExistingDataDetail description should not be null");
                Assert.That(m.DataRelationshipDetail, Is.Not.Null, "RelationshipBetweenExistingAndNewData should not be null");
                return true;
            });
        }

        [Test]
        public void Render_step2_view_when_edit_called_with_valid_dmpid_and_step()
        {
            const int step = 2;
            const string userId = "XX12345";
            CreateUser(userId);
            var dmp = Builder<DataManagementPlan>.CreateNew()
               .With(o => o.NewDataDetail = new NewDataDetail())
               .With(o => o.ExistingDataDetail = new ExistingDataDetail())
               .With(o => o.DataRelationshipDetail = new DataRelationshipDetail()).Build();
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = dmp)
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            _projectRepository.GetByDataManagementPlanId(dmp.Id).Returns(project);

            _controller.WithCallTo(c => c.Edit(dmp.Id, step)).ShouldRenderView("Step2").WithModel<DataManagementPlanViewModel>(m =>
            {
                Assert.That(m.Step, Is.EqualTo(step));
                Assert.That(m.Id, Is.EqualTo(dmp.Id));
                Assert.That(m.DataDocumentation, Is.Not.Null, "DataDocumentation should not be null");
                return true;
            });
        }

        [Test]
        public void Confirm_dmp_when_it_is_not_started()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var dmp = Builder<DataManagementPlan>.CreateNew().Build();

            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = dmp)
                .And(o => o.ProvisioningStatus = ProvisioningStatus.NotStarted)
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();

            var vm = Builder<DataManagementPlanViewModel>.CreateNew()
                .With(o => o.ProjectId = project.Id)
                .And(o => o.ProjectTitle = project.Title)
                .And(o => o.PrincipalInvestigator = project.Parties[0].Party)
                .And(o => o.Id = dmp.Id)
                .Build();

            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);
            _controller.WithCallTo(c => c.Confirm(vm))
                .ShouldRedirectTo<ConfirmController>(typeof(ConfirmController).GetMethod("Review", new[] { typeof(int) }));



        }

        [Test]
        public void Confirm_dmp_when_it_is_provisioned()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var dmp = Builder<DataManagementPlan>.CreateNew().Build();

            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = dmp)
                .And(o => o.ProvisioningStatus = ProvisioningStatus.Provisioned)
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            var vm = Builder<DataManagementPlanViewModel>.CreateNew()
                .With(o => o.ProjectId = project.Id)
                .And(o => o.ProjectTitle = project.Title)
                .And(o => o.PrincipalInvestigator = project.Parties[0].Party)
                .And(o => o.Id = dmp.Id)
                .Build();

            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);

            _controller.WithCallTo(c => c.Confirm(vm))
                .ShouldRedirectTo<ConfirmController>(typeof(ConfirmController).GetMethod("Republish", new[] { typeof(int) }));

        }

        [Test]
        public void Confirm_dmp_when_it_is_in_progress()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var dmp = Builder<DataManagementPlan>.CreateNew().Build();

            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = dmp)
                .And(o => o.ProvisioningStatus = ProvisioningStatus.Pending)
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            var vm = Builder<DataManagementPlanViewModel>.CreateNew()
                .With(o => o.ProjectId = project.Id)
                .And(o => o.ProjectTitle = project.Title)
                .And(o => o.PrincipalInvestigator = project.Parties[0].Party)
                .And(o => o.Id = dmp.Id)
                .Build();

            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);

            _controller.WithCallTo(c => c.Confirm(vm))
                .ShouldRedirectTo<ConfirmController>(typeof(ConfirmController).GetMethod("Pending", new[] { typeof(int) }));

        }

        [Test]
        public void Render_error_view_when_edit_called_with_invalid_dmpid()
        {
            _projectRepository.GetByDataManagementPlanId(Arg.Any<int>()).Returns(o => null);
            _controller.WithCallTo(c => c.Edit(0, 1)).ShouldRenderView("DmpNotFound");
        }

        [Test]
        public void Render_no_project_access_for_dmp_edit_httpget_requests_for_non_principal_investigator()
        {
            const string userId = "XX12345";
            CreateUser("456789");
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);
            _controller.WithCallTo(c => c.Edit(project.DataManagementPlan.Id, 1))
                .ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Not_save_dmp_for_non_principal_investigator()
        {
            const string userId = "XX12345";
            CreateUser("456789");
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);
            var vm = Builder<DataManagementPlanViewModel>.CreateNew()
                .With(o => o.Id = project.DataManagementPlan.Id)
                .Build();
            _controller.WithCallTo(c => c.Edit(vm))
                .ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Render_page_not_found_view_when_edit_called_with_invalid_step()
        {
            const string userId = "XX12345";
            CreateUser(userId);
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);
            _controller.WithCallTo(c => c.Edit(project.DataManagementPlan.Id, 20))
                .ShouldRenderView("PageNotFound");
        }


        [Test]
		public void Keep_urdms_user_role_status_as_Researcher_when_RemoveUrdmsUser_call()
        {
            const int userIdToRemove = 55;
            var model = new DataManagementPlanViewModel();
            model.UrdmsUsers = Builder<UrdmsUserViewModel>.CreateListOfSize(1).TheFirst(1).Do(x =>
            {
                x.Id = userIdToRemove;
                x.Relationship = (int)AccessRole.Visitors;
            }).Build();
            _form["urdms.users.row"] = model.UrdmsUsers[0].Id.ToString() + "," + model.UrdmsUsers[0].FullName.GetHashCode().ToString() + "," + model.UrdmsUsers[0].FullName + ","
                                        + model.UrdmsUsers[0].UserId + "," + model.UrdmsUsers[0].Relationship.ToString();
            _form["RemoveUser" + userIdToRemove] = "true,false";
            _form["Relationship" + userIdToRemove] = AccessRole.Visitors.ToString();
			_controller.WithCallTo(x => x.RemoveUrdmsUser(model)).ShouldRenderView("Step" + model.Step).WithModel<DataManagementPlanViewModel>(m =>
            {
                Assert.That(m.UrdmsUsers.Count, Is.EqualTo(1), "URDMS user has read only access");
                Assert.That(m.UrdmsUsers[0].Relationship, Is.EqualTo((int)AccessRole.Visitors), "URDMS user has read only access");
                return true;
            });
        }

        [Test]
		public void Update_non_urdms_user_access_role_to_Contributor_when_RemoveNonUrdmsUser_call()
        {
            const int userIdToRemove = 55;
            var model = new DataManagementPlanViewModel();
            model.NonUrdmsUsers = Builder<NonUrdmsUserViewModel>.CreateListOfSize(1).TheFirst(1).Do(x =>
            {
                x.Id = userIdToRemove;
                x.Relationship = (int)AccessRole.Visitors;
            }).Build();
            _form["nonurdms.users.row"] = model.NonUrdmsUsers[0].Id.ToString() + "," + model.NonUrdmsUsers[0].FullName.GetHashCode().ToString() + "," + model.NonUrdmsUsers[0].FullName + ","
                                            + model.NonUrdmsUsers[0].Relationship.ToString();
            _form["RemoveUser" + userIdToRemove] = "true,false";
            _form["Relationship" + userIdToRemove] = AccessRole.Visitors.ToString();
			_controller.WithCallTo(x => x.RemoveNonUrdmsUser(model)).ShouldRenderView("Step" + model.Step).WithModel<DataManagementPlanViewModel>(m =>
            {
                Assert.That(m.NonUrdmsUsers.Count, Is.EqualTo(1), "Non URDMS user has read only access");
                Assert.That(m.NonUrdmsUsers[0].Relationship, Is.EqualTo((int)AccessRole.Visitors), "Non URDMS user has read only access");
                return true;
            });
        }

        [Test]
        public void Add_a_user_to_user_access_list_for_a_valid_staff_id()
        {
            _projectRepository.GetByDataManagementPlanId(Arg.Any<int>()).Returns(new Project());
            var vm = new DataManagementPlanViewModel { FindUserId = "GA37493", Step = 5, Id = 1 };
            _lookup.GetUser(vm.FindUserId)
                .Returns(new UrdmsUser { FullName = "Joe Research", EmailAddress = "j.research@domain.edu.au", CurtinId = vm.FindUserId });

			_controller.WithCallTo(x => x.AddUrdmsUser(vm)).ShouldRenderView("Step5").WithModel
                <DataManagementPlanViewModel>(
                    m =>
                    {
                        Assert.That(m.UrdmsUsers, Is.Not.Null, "URDMS users does not exist");
                        Assert.That(m.UrdmsUsers.Count, Is.EqualTo(1), "URDMS users list does not match dmp list size");
                        Assert.That(m.UrdmsUsers[0].FullName, Is.EqualTo("Joe Research"), "URDMS user name the same");
                        return true;
                    });

        }

        [Test]
        public void Add_a_user_to_user_access_list_given_a_name()
        {
            _projectRepository.GetByDataManagementPlanId(Arg.Any<int>()).Returns(new Project());
            var vm = new DataManagementPlanViewModel { NonUrdmsNewUserName = "Tom Simon", Step = 5, Id = 1 };

			_controller.WithCallTo(x => x.AddNonUrdmsUser(vm)).ShouldRenderView("Step5").WithModel
                <DataManagementPlanViewModel>(
                    m =>
                    {
                        Assert.That(m.NonUrdmsUsers, Is.Not.Null, "Users does not exist");
                        Assert.That(m.NonUrdmsUsers[0].FullName, Is.EqualTo("Tom Simon"), "User name the same");
                        return true;
                    });

        }

        [Test]
        public void Update_the_project_entity_when_saving_at_step1()
        {
            const int step = 1;
            _form["stepAction"] = "Save and Next";
            var project = SetUpFullProjectWithAuthentication();
            var dmp = project.DataManagementPlan;
            var existingDataDetail = Builder<ExistingDataDetailViewModel>.CreateNew()
                .With(o => o.UseExistingData = !dmp.ExistingDataDetail.UseExistingData)
                .And(o => o.AccessTypesDescription = "Updated Access Types Description")
                .And(o => o.ExistingDataAccessTypes = PickHelper.RandomEnumsExcept(ExistingDataAccessTypes.None))
                .And(o => o.ExistingDataOwner = "Updated Owner")
                .Build();

            var newDataDetail = Builder<NewDataDetailViewModel>.CreateNew()
                .With(o => o.DataOwners = PickHelper.RandomEnumsExcept(DataOwners.None))
                .And(o => o.ResearchDataDescription = "Updated Research Data Description")
                .Build();

            var dataRelationshipDetail = Builder<DataRelationshipDetailViewModel>.CreateNew()
                .With(o => o.RelationshipBetweenExistingAndNewData = PickHelper
                    .RandomEnumExcept(dmp.DataRelationshipDetail.RelationshipBetweenExistingAndNewData, DataRelationship.None))
                .Build();

            var vm = Builder<DataManagementPlanViewModel>.CreateNew()
                .With(o => o.Id = dmp.Id)
                .And(o => o.ProjectId = project.Id)
                .And(o => o.ProjectTitle = project.Title)
                .And(o => o.Step = step)
                .And(o => o.ExistingDataDetail = existingDataDetail)
                .And(o => o.NewDataDetail = newDataDetail)
                .And(o => o.DataRelationshipDetail = dataRelationshipDetail)
                .Build();

            var methodInfo = _controller.GetType().GetMethod("Edit", new[] { typeof(int), typeof(int) });
            Assert.That(methodInfo, Is.Not.Null);
            _controller.WithCallTo(x => x.Edit(vm)).ShouldRedirectTo(methodInfo);

            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.NewDataDetail.DataOwners == newDataDetail.DataOwners));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.NewDataDetail.ResearchDataDescription == newDataDetail.ResearchDataDescription));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataRelationshipDetail.RelationshipBetweenExistingAndNewData == dataRelationshipDetail.RelationshipBetweenExistingAndNewData));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.ExistingDataDetail.UseExistingData == existingDataDetail.UseExistingData));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.ExistingDataDetail.AccessTypesDescription == existingDataDetail.AccessTypesDescription));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.ExistingDataDetail.ExistingDataAccessTypes == existingDataDetail.ExistingDataAccessTypes));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.ExistingDataDetail.ExistingDataOwner == existingDataDetail.ExistingDataOwner));

        }

        [Test]
        public void Update_the_project_entity_when_saving_at_step2()
        {
            const int step = 2;
            _form["stepAction"] = "Save and Next";
            var project = SetUpFullProjectWithAuthentication();
            var dmp = project.DataManagementPlan;
            var dataDocumentation = Builder<DataDocumentationViewModel>.CreateNew()
                .With(o => o.MetadataStandards = "Updated Metadata Standards")
                .Build();

            var dataOrganisation = Builder<DataOrganisationViewModel>.CreateNew()
                .With(o => o.DirectoryStructure = "Updated Directory Structure")
                .Build();

            var vm = Builder<DataManagementPlanViewModel>.CreateNew()
               .With(o => o.Id = dmp.Id)
               .And(o => o.ProjectId = project.Id)
               .And(o => o.ProjectTitle = project.Title)
               .And(o => o.Step = step)
               .And(o => o.DataDocumentation = dataDocumentation)
               .And(o => o.DataOrganisation = dataOrganisation)
               .Build();

            var methodInfo = _controller.GetType().GetMethod("Edit", new[] { typeof(int), typeof(int) });
            Assert.That(methodInfo, Is.Not.Null);
            _controller.WithCallTo(x => x.Edit(vm)).ShouldRedirectTo(methodInfo);

            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataDocumentation.MetadataStandards == dataDocumentation.MetadataStandards));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataOrganisation.DirectoryStructure == dataOrganisation.DirectoryStructure));
        }

        [Test]
        public void Update_the_project_entity_when_saving_at_step3()
        {
            const int step = 3;
            _form["stepAction"] = "Save and Next";
            var project = SetUpFullProjectWithAuthentication();
            var dmp = project.DataManagementPlan;
            var ethic = Builder<EthicViewModel>.CreateNew()
               .With(o => o.EthicRequiresClearance = !dmp.Ethic.EthicRequiresClearance)
               .And(o => o.EthicComments = "Updated Ethic Comments")
               .Build();

            var confidentiality = Builder<ConfidentialityViewModel>.CreateNew()
                .With(o => o.IsSensitive = !dmp.Confidentiality.IsSensitive)
                .And(o => o.ConfidentialityComments = "Updated Confidentiality Comments")
                .Build();

            var vm = Builder<DataManagementPlanViewModel>.CreateNew()
              .With(o => o.Id = dmp.Id)
              .And(o => o.ProjectId = project.Id)
              .And(o => o.ProjectTitle = project.Title)
              .And(o => o.Step = step)
              .And(o => o.Ethic = ethic)
              .And(o => o.Confidentiality = confidentiality)
              .Build();


            var methodInfo = _controller.GetType().GetMethod("Edit", new[] { typeof(int), typeof(int) });
            Assert.That(methodInfo, Is.Not.Null);
            _controller.WithCallTo(x => x.Edit(vm)).ShouldRedirectTo(methodInfo);

            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.Ethic.EthicRequiresClearance == ethic.EthicRequiresClearance));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.Ethic.EthicComments == ethic.EthicComments));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.Confidentiality.IsSensitive == confidentiality.IsSensitive));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.Confidentiality.ConfidentialityComments == confidentiality.ConfidentialityComments));
        }

        [Test]
        public void Update_the_project_entity_when_saving_at_step4()
        {
            const int step = 4;
            _form["stepAction"] = "Save and Next";
            var project = SetUpFullProjectWithAuthentication();
            var dmp = project.DataManagementPlan;
            var dataStorage = Builder<DataStorageViewModel>.CreateNew()
                .With(o => o.InstitutionalStorageTypes = PickHelper.RandomEnumsExcept(InstitutionalStorageTypes.ProjectStorageSpace, dmp.DataStorage.InstitutionalStorageTypes))
                .And(o => o.InstitutionalOtherTypeDescription = "Updated Institutional Other Type Description")
                .And(o => o.ExternalStorageTypes = PickHelper.RandomEnumsExcept(ExternalStorageTypes.Other, dmp.DataStorage.ExternalStorageTypes))
                .And(o => o.ExternalOtherTypeDescription = "Updated External Other Type Description")
                .And(o => o.PersonalStorageTypes = PickHelper.RandomEnumsExcept(PersonalStorageTypes.None, dmp.DataStorage.PersonalStorageTypes))
                .And(o => o.PersonalOtherTypeDescription = "Updated Personal Other Type Description")
                .And(o => o.MaxDataSize = PickHelper.RandomEnumExcept(MaxDataSize.None, dmp.DataStorage.MaxDataSize))
                .And(o => o.FileFormats = "Updated File Formats")
                .And(o => o.VersionControl = PickHelper.RandomEnumsExcept(dmp.DataStorage.VersionControl, VersionControl.None))
                .And(o => o.VersionControlDescription = "Updated Version Control Description")
                .Build();

            var vm = Builder<DataManagementPlanViewModel>.CreateNew()
             .With(o => o.Id = dmp.Id)
             .And(o => o.ProjectId = project.Id)
             .And(o => o.ProjectTitle = project.Title)
             .And(o => o.DataStorage = dataStorage)
             .And(o => o.Step = step)
             .Build();

            var methodInfo = _controller.GetType().GetMethod("Edit", new[] {typeof (int), typeof (int)});
            Assert.That(methodInfo,Is.Not.Null);
            _controller.WithCallTo(x => x.Edit(vm)).ShouldRedirectTo(methodInfo);

            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataStorage.InstitutionalOtherTypeDescription == dataStorage.InstitutionalOtherTypeDescription));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataStorage.InstitutionalStorageTypes == dataStorage.InstitutionalStorageTypes));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataStorage.ExternalOtherTypeDescription == dataStorage.ExternalOtherTypeDescription));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataStorage.ExternalStorageTypes == dataStorage.ExternalStorageTypes));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataStorage.PersonalOtherTypeDescription == dataStorage.PersonalOtherTypeDescription));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataStorage.PersonalStorageTypes == dataStorage.PersonalStorageTypes));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataStorage.VersionControl == dataStorage.VersionControl));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataStorage.VersionControlDescription == dataStorage.VersionControlDescription));
        }

        [Test]
        public void Update_the_project_entity_when_saving_at_step5()
        {
            const int step = 5;
            _form["stepAction"] = "Save and Next";
            var project = SetUpFullProjectWithAuthentication();
            var dmp = project.DataManagementPlan;
            var dataRetention = Builder<DataRetentionViewModel>.CreateNew()
                .With(o => o.DataRetentionLocations = PickHelper.RandomEnumsExcept(DataRetentionLocations.None))
                .And(o => o.DataRetentionLocationsDescription = "Updated Locations Description")
                .And(o => o.DataRetentionPeriod = PickHelper.RandomEnumExcept(dmp.DataRetention.DataRetentionPeriod))
                .And(o => o.DataRetentionResponsibilities = PickHelper.RandomEnumsExcept(DataResponsibilities.None))
                .And(o => o.DataRetentionResponsibilitiesDescription = "Updated Responsibilities Description")
                .And(o => o.DepositToRepository = !project.DataManagementPlan.DataRetention.DepositToRepository)
                .Build();
            var dataSharing = Builder<DataSharingViewModel>.CreateNew()
                .With(o => o.DataSharingAvailability = PickHelper.RandomEnumExcept(dmp.DataSharing.DataSharingAvailability))
                .And(o => o.DataSharingAvailabilityDate = DateTime.Today)
                .And(o => o.DataLicensingType = PickHelper.RandomEnumExcept(dmp.DataSharing.DataLicensingType))
                .And(o => o.ReuseByOrganisations = "Updated Reuse By Organisations")
                .And(o => o.ShareAccess = PickHelper.RandomEnumExcept(ShareAccess.NoAccess, dmp.DataSharing.ShareAccess))
                .And(o => o.ShareAccessDescription = "Updated Share Access Description")
                .Build();

            var vm = Builder<DataManagementPlanViewModel>.CreateNew()
            .With(o => o.Id = dmp.Id)
            .And(o => o.ProjectId = project.Id)
            .And(o => o.ProjectTitle = project.Title)
            .And(o => o.DataRetention = dataRetention)
            .And(o => o.DataSharing = dataSharing)
            .And(o => o.Step = step)
            .Build();

            string actionName;
            switch (project.ProvisioningStatus)
            {
                case ProvisioningStatus.NotStarted:
                    actionName = "Review";
                    break;
                case ProvisioningStatus.Pending:
                    actionName = "Pending";
                    break;
                case ProvisioningStatus.Provisioned:
                    actionName = "Republish";
                    break;
                default:
                    throw new InvalidOperationException("Unknown provisioning status");
            }

            Console.WriteLine("actionName: {0}",actionName);
            var methodInfo = typeof (ConfirmController).GetMethod(actionName, new[] {typeof (int)});
            Assert.That(methodInfo,Is.Not.Null);

            _controller.WithCallTo(x => x.Confirm(vm)).ShouldRedirectTo<ConfirmController>(methodInfo);

            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataRetention.DataRetentionLocations == dataRetention.DataRetentionLocations));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataRetention.DataRetentionLocationsDescription == dataRetention.DataRetentionLocationsDescription));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataRetention.DataRetentionPeriod == dataRetention.DataRetentionPeriod));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataRetention.DataRetentionResponsibilities == dataRetention.DataRetentionResponsibilities));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataRetention.DataRetentionResponsibilitiesDescription == dataRetention.DataRetentionResponsibilitiesDescription));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataRetention.DepositToRepository == dataRetention.DepositToRepository));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataSharing.DataSharingAvailability == dataSharing.DataSharingAvailability));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataSharing.DataSharingAvailabilityDate == dataSharing.DataSharingAvailabilityDate));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataSharing.DataLicensingType == dataSharing.DataLicensingType));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataSharing.ReuseByOrganisations == dataSharing.ReuseByOrganisations));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataSharing.ShareAccess == dataSharing.ShareAccess));
            _projectRepository.Received().Save(Arg.Is<Project>(o => o.DataManagementPlan.DataSharing.ShareAccessDescription == dataSharing.ShareAccessDescription));
        }

        [Test]
        public void Save_step_1_to_4_completion_time_on_post_to_Edit()
        {
            var userId = "GA37493";
            CreateUser(userId);
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                 .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                            .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                            .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                            .Build()))
                 .Build();

            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);
            _controller.HttpContext.Request.Form.Add("stepAction", "Save and Next");

            for (int i = 1; i < 5; i++)
            {
                var model = Builder<DataManagementPlanViewModel>.CreateNew()
                    .With(o => o.Id = project.DataManagementPlan.Id)
                    .And(o => o.Step = i)
                    .Build();

                _controller.WithCallTo(c => c.Edit(model));

                _timerRepository.Received().Save(Arg.Any<FormTimer>());
            }
        }

        [Test]
        public void Save_step_5_completion_time_on_post_to_Confirm()
        {
            var userId = "GA37493";
            CreateUser(userId);
            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = Builder<DataManagementPlan>.CreateNew().Build())
                 .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                            .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                            .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                            .Build()))
                 .Build();

            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);
            _controller.HttpContext.Request.Form.Add("stepAction", "Save and Next");

            var model = Builder<DataManagementPlanViewModel>.CreateNew()
                    .With(o => o.Id = project.DataManagementPlan.Id)
                    .And(o => o.Step = 5)
                    .Build();

           _controller.WithCallTo(c => c.Confirm(model));

           _timerRepository.Received().Save(Arg.Any<FormTimer>());
           
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


            var project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = dmp)
                .And(o => o.ProvisioningStatus = PickHelper.RandomEnumExcept<ProvisioningStatus>(ProvisioningStatus.Error, ProvisioningStatus.TimeOut))
                .Do(o => o.Parties.Add(Builder<ProjectParty>.CreateNew()
                                           .With(q => q.Party = Builder<Party>.CreateNew().With(r => r.UserId = userId).Build())
                                           .And(q => q.Relationship = ProjectRelationship.PrincipalInvestigator)
                                           .Build()))
                .Build();
            project.Parties.Do(o => o.Project = project);
            _projectRepository.GetByDataManagementPlanId(project.DataManagementPlan.Id).Returns(project);

            return project;
        }

        private void CreateUser(string userId)
        {
            var user = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = userId).Build();
            UserIs.AuthenticatedAs(_autoSubstitute, userId, new[] { "Administrators" });
            _lookup.GetUser(Arg.Is(userId)).Returns(user);
        }
    }
}
