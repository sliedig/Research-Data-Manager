using System;
using System.Linq;
using System.Web.Mvc;
using AutofacContrib.NSubstitute;
using Curtin.Framework.Common.UserService;
using Urdms.DocumentBuilderService.Commands;
using Urdms.ProvisioningService.Commands;
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
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Controllers
{
    [TestFixture]
    class ConfirmControllerShould
    {
        private AutoSubstitute _autoSubstitute;
        private IProjectRepository _projectRepository;
        private IDataCollectionRepository _dataCollectionRepository;
        private ConfirmController _controller;
        private ICurtinUserService _lookup;
        private IBus _bus;
        private Project _project;

        #region Test Setup

        [SetUp]
        public void SetUp()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();

            _projectRepository = _autoSubstitute.Resolve<IProjectRepository>();
            _dataCollectionRepository = _autoSubstitute.Resolve<IDataCollectionRepository>();
            _controller = _autoSubstitute.GetController<ConfirmController>();
            _lookup = _autoSubstitute.Resolve<ICurtinUserService>();
            var user = CreateUser("XX12345");
            var dmp = Builder<DataManagementPlan>.CreateNew()
                .With(o => o.NewDataDetail = Builder<NewDataDetail>.CreateNew().Build())
                .And(o => o.ExistingDataDetail = Builder<ExistingDataDetail>.CreateNew().Build())
                .And(o => o.DataSharing = Builder<DataSharing>.CreateNew().Build())
                .And(o => o.DataRelationshipDetail = Builder<DataRelationshipDetail>.CreateNew().Build())
                .Build();
            var dd = Builder<DataDeposit>.CreateNew().Build();
            _project = Builder<Project>.CreateNew()
                .With(o => o.DataManagementPlan = dmp)
                .And(o => o.DataDeposit = dd)
                .And(p => p.Description = "TestProject")
                .And(o => o.Keywords = "1,2,3,4,5,6,7,8,9,10,11,12")
                .Build();
            _project.FieldsOfResearch.AddRange(Builder<ProjectFieldOfResearch>
                                                   .CreateListOfSize(5)
                                                   .All()
                                                   .With(p => p.FieldOfResearch = Builder<FieldOfResearch>.CreateNew().Build())
                                                   .Build());
            _project.SocioEconomicObjectives.AddRange(Builder<ProjectSocioEconomicObjective>
                                                          .CreateListOfSize(7)
                                                          .All()
                                                          .With(p => p.SocioEconomicObjective = Builder<SocioEconomicObjective>.CreateNew().Build())
                                                          .Build());
            _project.Parties.AddRange(Builder<ProjectParty>.CreateListOfSize(8)
                                          .TheFirst(1)
                                          .With(o => o.Role = AccessRole.Members)
                                          .And(o => o.Party = Builder<Party>.CreateNew().With(p => p.Id = 0).Build())
                                          .TheNext(1)
                                          .With(o => o.Role = AccessRole.Owners)
                                          .And(o => o.Party = Builder<Party>.CreateNew().With(p => p.Id = 0).And(p => p.UserId = user.CurtinId).Build())
                                          .And(o => o.Relationship = ProjectRelationship.PrincipalInvestigator)
                                          .TheNext(1)
                                          .With(o => o.Role = AccessRole.Visitors)
                                          .And(o => o.Party = Builder<Party>.CreateNew().With(p => p.Id = 0).Build())
                                          .TheNext(1)
                                          .With(o => o.Role = AccessRole.None)
                                          .And(o => o.Party = Builder<Party>.CreateNew().With(p => p.Id = 0).Build())
                                          .TheNext(1)
                                          .With(o => o.Role = AccessRole.Members)
                                          .And(
                                              o =>
                                              o.Party =
                                              Builder<Party>.CreateNew().With(p => p.UserId = "FF24587").Build())
                                          .TheNext(1)
                                          .With(o => o.Role = AccessRole.Visitors)
                                          .And(
                                              o =>
                                              o.Party =
                                              Builder<Party>.CreateNew().With(p => p.UserId = "GA37493").Build())
                                          .TheNext(1)
                                          .With(o => o.Role = AccessRole.None)
                                          .And(
                                              o =>
                                              o.Party =
                                              Builder<Party>.CreateNew().With(p => p.UserId = "KK25344").Build())
                                          .TheNext(1)
                                          .With(o => o.Role = AccessRole.Owners)
                                          .And(
                                              o =>
                                              o.Party =
                                              Builder<Party>.CreateNew().With(p => p.UserId = "DD25265").Build())
                                          .Build());

            _bus = _autoSubstitute.Resolve<IBus>();
            _projectRepository.Get(Arg.Is(_project.Id)).Returns(_project);
            _projectRepository.GetByDataManagementPlanId(Arg.Is(_project.DataManagementPlan.Id)).Returns(_project);

            var resolver = Substitute.For<IDependencyResolver>();
            DependencyResolver.SetResolver(resolver);
        }



        #endregion

        [Test]
        public void Render_default_view_with_ConfirmDataManagementPlanViewModel_on_get_to_review()
        {
            _controller.WithCallTo(c => c.Review(_project.Id)).ShouldRenderDefaultView().WithModel<ConfirmDataManagementPlanViewModel>();
        }

        [Test]
        public void Render_default_view_with_ConfirmDataDepositViewModel_on_get_to_ReviewDataDeposit()
        {
            _project.SourceProjectType = SourceProjectType.DEPOSIT;

            _controller.WithCallTo(c => c.ReviewDataDeposit(_project.Id)).ShouldRenderDefaultView().WithModel<ConfirmDataDepositViewModel>();
        }

        [Test]
        public void Render_incorrect_project_type_view_for_invalid_review_data_deposit()
        {
            _project.SourceProjectType = SourceProjectType.DMP;

            _controller.WithCallTo(c => c.ReviewDataDeposit(_project.Id)).ShouldRenderView("IncorrectProjectType");
        }

        [Test]
        public void Render_not_found_view_on_get_to_ReviewDataDeposit_for_nonexisting_datadeposit()
        {
            _project.SourceProjectType = SourceProjectType.DEPOSIT;
            _projectRepository.Get(0).Returns(x => null);

            _controller.WithCallTo(c => c.ReviewDataDeposit(0)).ShouldRenderView("ProjectNotFound");
        }

        [Test]
        public void Render_no_access_view_on_get_to_ReviewDataDeposit_for_non_principal_investigator()
        {
            _project.SourceProjectType = SourceProjectType.DEPOSIT;
            CreateUser("KK00000");

            _controller.WithCallTo(c => c.ReviewDataDeposit(_project.Id)).ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Render_DataDepositProvisioned_view_on_get_to_ReviewDataDeposit()
        {
            _project.ProvisioningStatus = ProvisioningStatus.Provisioned;
            _project.SourceProjectType = SourceProjectType.DEPOSIT;

            _controller.WithCallTo(c => c.ReviewDataDeposit(_project.Id)).ShouldRenderView("DataDepositProvisioned");
        }

        [Test]
        public void Render_no_access_view_on_get_to_review_for_non_principal_investigator()
        {
            CreateUser("KK00000");

            _controller.WithCallTo(c => c.Review(_project.Id)).ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Render_not_found_view_on_get_to_review_for_nonexisting_dmp()
        {
            _projectRepository.GetByDataManagementPlanId(0).Returns(x => null);
            _controller.WithCallTo(c => c.Review(0)).ShouldRenderView("DmpNotFound");
        }

        [Test]
        public void Display_default_view_on_get_to_submitted()
        {
            _controller.WithCallTo(c => c.Submitted(_project.Id))
                .ShouldRenderDefaultView();
        }

        [Test]
        public void Render_no_access_view_on_get_to_submitted_for_non_principal_investigator()
        {
			CreateUser("KK00000");
            _controller.WithCallTo(c => c.Submitted(_project.Id))
                .ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Render_not_found_view_on_get_to_submitted_for_nonexisting_dmp()
        {
            _project.DataManagementPlan = null;
            _controller.WithCallTo(c => c.Submitted(_project.Id))
                .ShouldRenderView("DmpNotFound");
        }

        [Test]
        public void Submit_confirmation_for_DataManagementPlan_invokes_bus_send_request_site_collection_command()
        {  
            // Arrange
            var vm = Builder<ConfirmDataManagementPlanViewModel>
                .CreateNew()
                .With(o => o.DataManagementPlanId = _project.DataManagementPlan.Id)
                .Build();
            _bus.When(c => c.Send(Arg.Any<Action<SiteRequestCommand>>())).Do(a =>
            {
                // Arrange
                var rsc = new SiteRequestCommand();
                var lambda = a.Arg<Action<SiteRequestCommand>>();

                // Act
                lambda(rsc);

                // Assert
                Assert.That(rsc.ProjectId, Is.EqualTo(_project.Id), "Invalid project id passed to the bus");
                Assert.That(rsc.ProjectTitle, Is.EqualTo(_project.Title), "Invalid project name passed to the bus");
                Assert.That(rsc.ProjectDescription, Is.EqualTo(_project.Description), "Invalid project description passed to the bus");

                foreach (var party in _project.Parties.Where(p => p.Role != AccessRole.None))
                {
                    Assert.That(rsc.UserRoles.Any(u => u.Value.Contains(party.Party.UserId) && u.Key == party.Role.ToString()), "No site user passed to the bus for party " + party.Party.UserId);
                }
            });

            // Assert
            _controller.WithCallTo(c => c.Review(vm)).ShouldRedirectTo(_controller.GetType().GetMethod("Submitted"));
        }

        [Test]
        public void Perform_review_submission_confirmation_for_a_data_deposit_project()
        {
            _project.SourceProjectType = SourceProjectType.DEPOSIT;
            _project.DataManagementPlan = null;

            // Arrange
            var vm = Builder<ConfirmDataDepositViewModel>
                .CreateNew()
                .With(o => o.ProjectId = _project.Id)
                .And(o => o.ProjectType = SourceProjectType.DEPOSIT)
                .Build();
            _bus.When(c => c.Send(Arg.Any<Action<SiteRequestCommand>>())).Do(a =>
            {
                // Arrange
                var command = new SiteRequestCommand();
                var lambda = a.Arg<Action<SiteRequestCommand>>();

                // Act
                lambda(command);

                // Assert
                Assert.That(command.ProjectId, Is.EqualTo(_project.Id), "Invalid project id passed to the bus");
                Assert.That(command.ProjectTitle, Is.EqualTo(_project.Title), "Invalid project name passed to the bus");
                Assert.That(command.ProjectDescription, Is.EqualTo(_project.Description), "Invalid project description passed to the bus");

                foreach (var party in _project.Parties.Where(u => u.Role != AccessRole.None))
                {
                    Assert.That(command.UserRoles.Any(u => u.Value.Contains(party.Party.UserId) && u.Key == party.Role.ToString()), "No site user passed to the bus for party " + party.Party.UserId);
                }
            });

            // Assert
            _controller.WithCallTo(c => c.ReviewDataDeposit(vm)).ShouldRedirectTo(_controller.GetType().GetMethod("SubmittedDataDeposit", new[] { typeof(int) }));

            _dataCollectionRepository.Received().Save(Arg.Any<DataCollection>());
        }

        [Test]
        public void Not_submit_confirmation_for_non_principal_investigator()
        {
			CreateUser("KK00000");
            var vm = Builder<ConfirmDataManagementPlanViewModel>.CreateNew().With(o => o.DataManagementPlanId = _project.DataManagementPlan.Id).Build();

            _controller.WithCallTo(c => c.Review(vm)).ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Render_dmp_not_found_view_for_submit_confirmation_of_invalid_dmp()
        {
            _project.DataManagementPlan = null;
            var vm = Builder<ConfirmDataManagementPlanViewModel>.CreateNew().With(o => o.DataManagementPlanId = 0).Build();
            _projectRepository.GetByDataManagementPlanId(vm.DataManagementPlanId).Returns(_project);

            _controller.WithCallTo(c => c.Review(vm)).ShouldRenderView("DmpNotFound");
        }

        [Test]
        public void Render_republish_view_for_provisioned_dmp_in_republish_get_action()
        {
            _project.ProvisioningStatus = ProvisioningStatus.Provisioned;
            _controller.WithCallTo(c => c.Republish(_project.DataManagementPlan.Id)).ShouldRenderDefaultView();
        }

        [Test]
        public void Render_not_found_view_for_nonexisting_dmp_in_republish_get_action()
        {
            _project.DataManagementPlan = null;

            _controller.WithCallTo(c => c.Republish(_project.Id)).ShouldRenderView("DmpNotFound");
        }

        [Test]
        public void Render_no_project_access_view_for_non_principal_investigator_in_republish_get_action()
        {
			CreateUser("KK00000");

            _controller.WithCallTo(c => c.Republish(_project.DataManagementPlan.Id)).ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Render_dmp_not_provisioned_view_for_unprovisioned_dmp_in_republish_get_action()
        {
            _project.ProvisioningStatus = ProvisioningStatus.Pending;

            _controller.WithCallTo(c => c.Republish(_project.DataManagementPlan.Id)).ShouldRenderView("DmpNotProvisioned");

        }

        [Test]
        public void Render_not_found_view_for_nonexisting_dmp_in_republish_post_action()
        {
            _project.DataManagementPlan = null;
            var vm = Builder<DataManagementPlanInfoViewModel>.CreateNew()
                .With(o => o.ProjectId = _project.Id)
                .Build();
            _controller.WithCallTo(c => c.Republish(vm))
                .ShouldRenderView("DmpNotFound");
        }

        [Test]
        public void Render_dmp_not_provisioned_view_for_unprovisioned_dmp_in_republish_post_action()
        {
            _project.ProvisioningStatus = ProvisioningStatus.Pending;
            var vm = Builder<DataManagementPlanInfoViewModel>.CreateNew()
                .With(o => o.DataManagementPlanId = _project.DataManagementPlan.Id)
                .Build();
            _controller.WithCallTo(c => c.Republish(vm))
                .ShouldRenderView("DmpNotProvisioned");
        }

        [Test]
        public void Render_no_project_access_view_for_non_principal_investigator_in_republish_post_action()
        {
			CreateUser("KK00000");
            _project.ProvisioningStatus = ProvisioningStatus.Provisioned;
            var vm = Builder<DataManagementPlanInfoViewModel>.CreateNew()
               .With(o => o.DataManagementPlanId = _project.DataManagementPlan.Id)
               .Build();
            _controller.WithCallTo(c => c.Republish(vm))
                .ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Render_when_republishing()
        {
            _project.ProvisioningStatus = ProvisioningStatus.Provisioned;
            var url = string.Format(@"http://urdms.yourdomain.edu.au/projects/researchproject{0}", _project.Id);
          
            var vm = Builder<DataManagementPlanInfoViewModel>.CreateNew()
               .With(o => o.DataManagementPlanId = _project.DataManagementPlan.Id)
               .Build();

            _bus.When(c => c.Send(Arg.Any<Action<GenerateDmpCommand>>()))
                .Do(a =>
                            {
                                var message = new GenerateDmpCommand();
                                var lambda = a.Arg<Action<GenerateDmpCommand>>();

                                lambda(message);

                                Assert.That(message.ProjectId, Is.EqualTo(_project.Id), "Invalid project id passed to the bus");
                            });
            _controller
                .WithCallTo(c => c.Republish(vm))
                .ShouldRedirectTo(_controller.GetType().GetMethod("Republished"));
        }

        [Test]
        public void Render_when_in_progress()
        {
            _project.ProvisioningStatus = ProvisioningStatus.Pending;
            _controller.WithCallTo(c => c.Pending(_project.DataManagementPlan.Id))
                .ShouldRenderDefaultView();
        }

        [Test]
        public void Render_not_found_view_for_nonexisting_dmp_for_inprogress_action()
        {
            _project.DataManagementPlan = null;
            _controller.WithCallTo(c => c.Pending(_project.Id))
                .ShouldRenderView("DmpNotFound");
        }

        [Test]
        public void Render_no_project_access_view_for_non_principal_investigator_for_inprogress_action()
        {
            CreateUser("KK00000");
            _project.ProvisioningStatus = ProvisioningStatus.Pending;
            _controller.WithCallTo(c => c.Pending(_project.DataManagementPlan.Id))
                .ShouldRenderView("NoProjectAccessRight");
        }

        [Test]
        public void Render_dmp_not_in_progress_for_inprogress_action()
        {
            _project.ProvisioningStatus = ProvisioningStatus.Provisioned;
            _controller.WithCallTo(c => c.Pending(_project.DataManagementPlan.Id))
                .ShouldRenderView("DmpNotInProgress");
        }

        [Test]
        public void Perform_review_submission_for_a_non_deposit_project()
        {
            // Arrange
            var vm = Builder<ConfirmDataManagementPlanViewModel>
                .CreateNew()
                .With(o => o.DataManagementPlanId = 1)
                .Build();
            _project.SourceProjectType = PickHelper.RandomEnumExcept(SourceProjectType.DEPOSIT, SourceProjectType.None);
            _project.DataDeposit = null;

            // Act
            _projectRepository.GetByDataManagementPlanId(Arg.Is(vm.DataManagementPlanId)).Returns(_project);


            _bus.When(c => c.Send(Arg.Any<Action<SiteRequestCommand>>()))
                .Do(a =>
                {
                    var message = new SiteRequestCommand();
                    var lambda = a.Arg<Action<SiteRequestCommand>>();

                    lambda(message);

                    Assert.That(message.ProjectId, Is.EqualTo(_project.Id), "Invalid project id passed to the bus");
                    Assert.That(message.ProjectDescription, Is.EqualTo(_project.Description), "Invalid project description passed to the bus");
                    Assert.That(message.ProjectTitle, Is.EqualTo(_project.Title), "Invalid project name passed to the bus");
                });
            // Assert
            _controller.WithCallTo(c => c.Review(vm)).ShouldRedirectTo(_controller.GetType().GetMethod("Submitted"));
            // Saving a new data collection
            _dataCollectionRepository.Received().Save(Arg.Any<DataCollection>());
            _projectRepository.Received().GetByDataManagementPlanId(Arg.Is(vm.DataManagementPlanId));
        }

        [Test]
        public void Only_create_an_initial_data_collection_if_one_does_not_exist_already()
        {
            var vm = Builder<ConfirmDataManagementPlanViewModel>
                .CreateNew()
                .With(o => o.DataManagementPlanId = _project.DataManagementPlan.Id)
                .Build();
            _project.DataCollections.Add(Builder<DataCollection>.CreateNew().With(dc => dc.IsFirstCollection = true).Build());
            _project.SourceProjectType = SourceProjectType.DMP;

            _projectRepository.GetByDataManagementPlanId(Arg.Is(vm.DataManagementPlanId)).Returns(_project);

            _controller.WithCallTo(c => c.Review(vm)).ShouldRedirectTo(_controller.GetType().GetMethod("Submitted"));

            // Should not create or save another data collection
            _dataCollectionRepository.DidNotReceive().Save(Arg.Any<DataCollection>());
        }

        private UrdmsUser CreateUser(string curtinId)
        {
            var user = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = curtinId).Build();
            UserIs.AuthenticatedAs(_autoSubstitute, curtinId, new[] { "Administrators" });
            _lookup.GetUser(Arg.Is(curtinId)).Returns(user);
            return user;
        }
    }
}
