using System;
using System.Collections.Generic;
using System.Linq;
using AutofacContrib.NSubstitute;
using Curtin.Framework.Common.UserService;
using Urdms.Approvals.ApprovalService.Commands;
using FizzWare.NBuilder;
using NServiceBus;
using NSubstitute;
using NUnit.Framework;
using Urdms.Dmp.Controllers;
using Urdms.Dmp.Controllers.Filters;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Entities.Components;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Integration.UserService;
using Urdms.Dmp.Models.ApprovalModels;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Tests.Helpers;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Tests.Controllers
{
    [TestFixture]
    class ApprovalControllerShould
    {
        private AutoSubstitute _autoSubstitute;
        private ApprovalController _controller;
        private IDataCollectionRepository _dataCollectionRepository;
        private IDataCollectionHashCodeRepository _hashCodeRepository;
        private IProjectRepository _projectRepository;
        private ICurtinUserService _lookup;
        private IBus _bus;
        private const string QaId = "GA37493";
        private const string SecondaryApproverId = "XE24758";
       
        [SetUp]
        public void SetUp()
        {
            _autoSubstitute = AutoSubstituteContainer.Create();
            _controller = _autoSubstitute.GetController<ApprovalController>();
            _dataCollectionRepository = _autoSubstitute.Resolve<IDataCollectionRepository>();
            _hashCodeRepository = _autoSubstitute.Resolve<IDataCollectionHashCodeRepository>();
            _lookup = _autoSubstitute.Resolve<ICurtinUserService>();
            _bus = _autoSubstitute.Resolve<IBus>();
            _projectRepository = _autoSubstitute.Resolve<IProjectRepository>();
        }

        [Test]
        public void Render_view_index_when_accessed_by_user_with_QaApprover_group_membership()
        {
            var party = GetPartyWithManager();
            CreateQaUser();
            var dataCollections = Builder<DataCollection>.CreateListOfSize(3)
                .TheFirst(2)
                .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.Submitted, DateTime.Now))
                .And(dc => dc.Parties.Add(party))
                .TheLast(1)
                .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.SecondaryApproved, DateTime.Now))
                .And(dc => dc.Parties.Add(party))
                .Build();
            _dataCollectionRepository.GetByStatus(Arg.Any<List<DataCollectionStatus>>()).Returns(dataCollections);

            _controller.WithCallTo(c => c.Index()).ShouldRenderDefaultView()
                .WithModel<DataCollectionApprovalListViewModel>(alm =>
                                                                    {
                                                                        Assert.That(alm.DataCollectionItems.Where(dc => dc.Status == DataCollectionStatus.Submitted).Count(), Is.EqualTo(2));
                                                                        Assert.That(alm.DataCollectionItems.Where(dc => dc.Status == DataCollectionStatus.SecondaryApproved).Count(), Is.EqualTo(1));
                                                                        return true;
                                                                    });
        }

        [Test]
        public void Render_view_index_when_accessed_by_user_with_SecondaryApprover_group_membership()
        {
            var party = GetPartyWithManager();
            CreateOrdApprover();
            var dataCollections = Builder<DataCollection>.CreateListOfSize(3)
                .TheFirst(2)
                .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.QaApproved, DateTime.Now))
                .And(dc => dc.Parties.Add(party))
                .TheLast(1)
                .With(dc => dc.CurrentState = new DataCollectionState(DataCollectionStatus.RecordAmended, DateTime.Now))
                .And(dc => dc.Parties.Add(party))
                .Build();
            _dataCollectionRepository.GetByStatus(Arg.Any<List<DataCollectionStatus>>()).Returns(dataCollections);

            _controller.WithCallTo(c => c.Index()).ShouldRenderView("Index")
                .WithModel<DataCollectionApprovalListViewModel>(alm =>
                {
                    Assert.That(alm.DataCollectionItems.Where(dc => dc.Status == DataCollectionStatus.QaApproved).Count(), Is.EqualTo(2));
                    Assert.That(alm.DataCollectionItems.Where(dc => dc.Status == DataCollectionStatus.RecordAmended).Count(), Is.EqualTo(1));
                    return true;
                });
        }

        [Test]
        public void Render_view_index_when_accessed_by_user_with_SecondaryApprover_group_membership_with_no_data_collections_awaiting_approval()
        {
            CreateOrdApprover();
            var dataCollections = new List<DataCollection>();
            _dataCollectionRepository.GetByStatus(Arg.Any<List<DataCollectionStatus>>()).Returns(dataCollections);

            _controller.WithCallTo(c => c.Index()).ShouldRenderDefaultView()
                .WithModel<DataCollectionApprovalListViewModel>(vm =>
                                                                    {
                                                                        Assert.That(vm.DataCollectionItems.Count(), Is.EqualTo(0));
                                                                        return true;
                                                                    });
        }

        [Test]
        public void Render_view_index_when_access_by_user_with_QaApprover_group_membership_with_no_data_collections_awaiting_approval()
        {
            CreateQaUser();
            var dataCollections = new List<DataCollection>();
            _dataCollectionRepository.GetByStatus(Arg.Any<List<DataCollectionStatus>>()).Returns(dataCollections);

            _controller.WithCallTo(c => c.Index()).ShouldRenderDefaultView()
                .WithModel<DataCollectionApprovalListViewModel>(vm =>
                                                                    {
                                                                        Assert.That(vm.DataCollectionItems.Count(), Is.EqualTo(0));
                                                                        return true;
                                                                    });
        }

        [Test]
        public void Show_step1_if_user_is_a_qa_approver_with_a_submitted_collection()
        {
            CreateQaUser();
            var project = CreateProject();

            var collection = Builder<DataCollection>.CreateNew()
                .With(o => o.CurrentState = Builder<DataCollectionState>.CreateNew()
                    .WithConstructor(() => new DataCollectionState(DataCollectionStatus.Submitted,DateTime.Now)).Build())
                .And(o => o.ProjectId = project.Id)
                .Build();

            var party = Builder<DataCollectionParty>.CreateNew()
                .With(o => o.Relationship = DataCollectionRelationshipType.Manager)
                .And(o => o.Party = Builder<Party>.CreateNew().Build())
                .And(o => o.DataCollection = collection)
                .Build();
            collection.Parties.Add(party);

            _dataCollectionRepository.Get(Arg.Is(collection.Id)).Returns(collection);
            _projectRepository.Get(Arg.Is(project.Id)).Returns(project);
            _controller.WithCallTo(c => c.Step1(collection.Id)).ShouldRenderView("DataCollectionStep1");

        }

        [Test]
        public void Show_step1_if_user_is_a_qa_approver_with_a_secondary_approved_collection()
        {
            var project = CreateProject();

            var collection = Builder<DataCollection>.CreateNew()
               .With(o => o.CurrentState = Builder<DataCollectionState>.CreateNew()
                   .WithConstructor(() => new DataCollectionState(DataCollectionStatus.SecondaryApproved, DateTime.Now)).Build())
               .And(o => o.ProjectId = project.Id)
               .Build();
            CreateQaUser();
            _dataCollectionRepository.Get(Arg.Is(collection.Id)).Returns(collection);
            _projectRepository.Get(Arg.Is(project.Id)).Returns(project);

            _controller.WithCallTo(c => c.Step1(collection.Id)).ShouldRenderView("DataCollectionStep1");
        }

        [Test]
        public void Do_not_show_step1_if_user_is_a_qa_approver_with_unallowed_collection_status()
        {
            var collection = Builder<DataCollection>.CreateNew()
               .With(o => o.CurrentState = Builder<DataCollectionState>.CreateNew().Build())
               .Build();
            CreateQaUser();
            _dataCollectionRepository.Get(collection.Id).Returns(collection);
            _controller.WithCallTo(c => c.Step1(collection.Id)).ShouldRenderView("DataCollectionNotFound");
        }

        [Test]
        public void Redirect_to_confirm_on_post_to_step2()
        {
            var dataCollection = Builder<DataCollection>.CreateNew()
               .With(o => o.CurrentState = Builder<DataCollectionState>.CreateNew()
                   .WithConstructor(() => new DataCollectionState(DataCollectionStatus.SecondaryApproved, DateTime.Now)).Build())
               .Build();
            CreateQaUser();
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);
            var model = Builder<DataCollectionApprovalViewModelStep2>.CreateNew()
                .With(vm => vm.Id = dataCollection.Id)
                .Build();

            _controller.WithCallTo(c => c.Step2(model)).ShouldRedirectTo(_controller.GetType().GetMethod("Confirm", new[] { typeof(Int32) }));
            _dataCollectionRepository.Received().Save(dataCollection);
        }

        [Test]
        public void Show_confirmation_step_if_user_is_a_secondary_approver_with_a_qa_approved_collection()
        {
            var collection = Builder<DataCollection>.CreateNew()
               .With(o => o.CurrentState = Builder<DataCollectionState>.CreateNew()
                   .WithConstructor(() => new DataCollectionState(DataCollectionStatus.QaApproved, DateTime.Now)).Build())
               .Build();
            CreateOrdApprover();
            _dataCollectionRepository.Get(collection.Id).Returns(collection);

            _controller.WithCallTo(c => c.Confirm(collection.Id)).ShouldRenderDefaultView()
                .WithModel<ApprovalConfirmationViewModel>(vm =>
                                                                       {
                                                                           Assert.That(vm.State, Is.EqualTo(DataCollectionStatus.QaApproved));
                                                                           return true;
                                                                       });
        }

        [Test]
        public void Show_does_not_require_approval_view_if_user_is_an_ord_approver_with_get_to_confirm_and_collection_status_is_submitted()
        {
            var collection = Builder<DataCollection>.CreateNew()
               .With(o => o.CurrentState = Builder<DataCollectionState>.CreateNew()
                   .WithConstructor(() => new DataCollectionState(DataCollectionStatus.Submitted, DateTime.Now)).Build())
               .Build();
            CreateOrdApprover();
            _dataCollectionRepository.Get(collection.Id).Returns(collection);

            _controller.WithCallTo(c => c.Confirm(collection.Id)).ShouldRenderView("DataCollectionInvalidState");
        }

        [Test]
        public void Show_confirmation_step_if_user_is_a_qa_approver_with_a_secondary_approved_collection()
        {
            var collection = Builder<DataCollection>.CreateNew()
               .With(o => o.CurrentState = Builder<DataCollectionState>.CreateNew()
                   .WithConstructor(() => new DataCollectionState(DataCollectionStatus.SecondaryApproved, DateTime.Now)).Build())
               .Build();
            CreateQaUser();
            _dataCollectionRepository.Get(collection.Id).Returns(collection);

            _controller.WithCallTo(c => c.Confirm(collection.Id)).ShouldRenderDefaultView()
                .WithModel<ApprovalConfirmationViewModel>(vm =>
                {
                    Assert.That(vm.State, Is.EqualTo(DataCollectionStatus.SecondaryApproved));
                    return true;
                });
        }

        [Test]
        public void Show_DataCollectionNotFound_view_when_an_invalid_data_collection_is_submitted_to_comfirm_step()
        {
            var vm = Builder<ApprovalConfirmationViewModel>.CreateNew().Build();
            _dataCollectionRepository.Get(vm.DataCollectionId).Returns(n => null);

            _controller.WithCallTo(c => c.Confirm(vm)).ShouldRenderView("DataCollectionNotFound");
        }

        [Test]
        public void Show_DataCollectionInvalid_State_view_when_an_invalid_data_collection_for_the_users_role_is_submitted_to_confirm_step()
        {
            // Create data collection in submitted state. The secondary approver should NOT be able to access this
            var dataCollection = CreateDataCollectionWithState(DataCollectionStatus.Submitted);
            var vm = Builder<ApprovalConfirmationViewModel>.CreateNew()
                .With(m => m.DataCollectionId = dataCollection.Id)
                .And(m => m.State = DataCollectionStatus.Submitted)
                .Build();
            CreateOrdApprover();
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);

            _controller.WithCallTo(c => c.Confirm(vm)).ShouldRenderView("DataCollectionInvalidState");
        }

        [Test]
        public void Return_confirm_view_with_validation_error_if_qa_approver_does_not_confirm_approval_checkbox()
        {
            var dataCollection = CreateDataCollectionWithState(DataCollectionStatus.Submitted);
            var vm = Builder<ApprovalConfirmationViewModel>.CreateNew()
                .With(m => m.DataCollectionId = dataCollection.Id)
                .And(m => m.State = DataCollectionStatus.Submitted)
                .And(m => m.IsQaApproved = false)
                .Build();
            var hashCode = new DataCollectionHashCode();
            hashCode.UpdateHashCode(dataCollection);
            CreateQaUser();
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);
            _hashCodeRepository.GetByDataCollectionId(dataCollection.Id).Returns(hashCode);

            _controller.WithCallTo(c => c.Confirm(vm)).ShouldRenderDefaultView()
                .WithModel<ApprovalConfirmationViewModel>();
            Assert.That(_controller.ModelState.IsValid, Is.False);
        }

        [Test]
        public void Send_SubmitForOrdApproval_command_to_bus_for_a_submitted_data_collection_once_qa_approved()
        {
            // QA actions to qa approve the data collection (should pass on to secondary approver)
            var dataCollection = CreateDataCollectionWithState(DataCollectionStatus.Submitted);
            var vm = Builder<ApprovalConfirmationViewModel>.CreateNew()
                .With(m => m.DataCollectionId = dataCollection.Id)
                .And(m => m.State = DataCollectionStatus.Submitted)
                .And(m => m.IsQaApproved = true)
                .Build();
            var hashCode = new DataCollectionHashCode();
            hashCode.UpdateHashCode(dataCollection);
            CreateQaUser();

            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);
            _hashCodeRepository.GetByDataCollectionId(dataCollection.Id).Returns(hashCode);
            
            _bus.When(c => c.Send(Arg.Any<Action<SubmitForSecondaryApproval>>())).Do(a =>
            {
                // Arrange
				var rsc = new SubmitForSecondaryApproval();
				var lambda = a.Arg<Action<SubmitForSecondaryApproval>>();

                // Act
                lambda(rsc);

                // Assert
                Assert.That(rsc.DataCollectionId, Is.EqualTo(dataCollection.Id), "Invalid data collection id passed to the bus");
                Assert.That(rsc.ApprovedOn, Is.EqualTo(dataCollection.CurrentState.StateChangedOn).Within(1).Minutes, "Invalid approval date passed to the bus");
                Assert.That(rsc.ApprovedBy, Is.EqualTo(QaId), "Invalid approver id passed to the bus");
            });

            _controller.WithCallTo(c => c.Confirm(vm)).ShouldRenderView("Approved")
                .WithModel<ApprovalConfirmationViewModel>(m => m.ProposedState == DataCollectionStatus.QaApproved);
        }

        [Test]
        public void Send_SubmitForFinalApproval_command_to_bus_for_a_qa_approved_data_collection_once_secondary_approved()
        {
            // Secondary approver actions to secondary approve the data collection (should pass by to qa approver)
            var dataCollection = CreateDataCollectionWithState(DataCollectionStatus.QaApproved);
            var vm = Builder<ApprovalConfirmationViewModel>.CreateNew()
                .With(m => m.DataCollectionId = dataCollection.Id)
                .And(m => m.State = DataCollectionStatus.QaApproved)
                .And(m => m.DoesNotViolateAgreements = true)
                .And(m => m.DoesNotViolateConfidentialityAndEthics = true)
                .Build();
            var hashCode = new DataCollectionHashCode();
            hashCode.UpdateHashCode(dataCollection);
            CreateOrdApprover();

            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);
            _hashCodeRepository.GetByDataCollectionId(dataCollection.Id).Returns(hashCode);

            _bus.When(c => c.Send(Arg.Any<Action<SubmitForFinalApproval>>())).Do(a =>
            {
                var rsc = new SubmitForFinalApproval();
                var lambda = a.Arg<Action<SubmitForFinalApproval>>();

                lambda(rsc);

                Assert.That(rsc.DataCollectionId, Is.EqualTo(dataCollection.Id), "Invalid data collection id passed to the bus");
                Assert.That(rsc.ApprovedOn, Is.EqualTo(dataCollection.CurrentState.StateChangedOn).Within(1).Minutes, "Invalid approval date passed to the bus");
                Assert.That(rsc.ApprovedBy, Is.EqualTo(SecondaryApproverId), "Invalid approver id passed to the bus");
            });

            _controller.WithCallTo(c => c.Confirm(vm)).ShouldRenderView("Approved")
                .WithModel<ApprovalConfirmationViewModel>(m => m.ProposedState == DataCollectionStatus.SecondaryApproved);
        }

        [Test]
        public void Send_PublishDataCollection_command_to_bus_for_a_secondary_approved_data_collection_once_publication_approved()
        {
            // QA Approver actions to publish the data collection
            var dataCollection = CreateDataCollectionWithState(DataCollectionStatus.SecondaryApproved);
            var vm = Builder<ApprovalConfirmationViewModel>.CreateNew()
                .With(m => m.DataCollectionId = dataCollection.Id)
                .And(m => m.State = DataCollectionStatus.SecondaryApproved)
                .And(m => m.IsPublicationApproved = true)
                .Build();
            var hashCode = new DataCollectionHashCode();
            hashCode.UpdateHashCode(dataCollection);
            CreateQaUser();

            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);
            _hashCodeRepository.GetByDataCollectionId(dataCollection.Id).Returns(hashCode);

            _bus.When(c => c.Send(Arg.Any<Action<PublishDataCollection>>())).Do(a =>
            {
                var rsc = new PublishDataCollection();
                var lambda = a.Arg<Action<PublishDataCollection>>();

                lambda(rsc);

                Assert.That(rsc.DataCollectionId, Is.EqualTo(dataCollection.Id), "Invalid data collection id passed to the bus");
                Assert.That(rsc.ApprovedOn, Is.EqualTo(dataCollection.CurrentState.StateChangedOn).Within(1).Minutes, "Invalid approval date passed to the bus");
                Assert.That(rsc.ApprovedBy, Is.EqualTo(QaId), "Invalid approver id passed to the bus");
            });

            _controller.WithCallTo(c => c.Confirm(vm)).ShouldRenderView("Approved")
                .WithModel<ApprovalConfirmationViewModel>(m => m.ProposedState == DataCollectionStatus.Publishing);
        }

        [Test]
        [Ignore("Uses a separate action so this test needs to be refactored")]
        public void Send_SubmitForOrdReApproval_command_to_bus_for_a_secondary_approved_data_collection_if_changes_have_been_made()
        {
            // Qa has made changes to the data collection so must be advised that it will return to secondary for reapproval
            var dataCollection = CreateDataCollectionWithState(DataCollectionStatus.SecondaryApproved);
            dataCollection.Title = "No changes yet";
            var vm = Builder<ApprovalConfirmationViewModel>.CreateNew()
                .With(m => m.DataCollectionId = dataCollection.Id)
                .And(m => m.State = DataCollectionStatus.SecondaryApproved)
                .Build();
            var oldHashCode = new DataCollectionHashCode();
            oldHashCode.UpdateHashCode(dataCollection);
            CreateQaUser();

            dataCollection.Title = "We've made a change";
            _dataCollectionRepository.Get(dataCollection.Id).Returns(dataCollection);
            _hashCodeRepository.GetByDataCollectionId(dataCollection.Id).Returns(oldHashCode);

			_bus.When(c => c.Send(Arg.Any<Action<SubmitForSecondaryApproval>>())).Do(a =>
            {
				var rsc = new SubmitForSecondaryApproval();
				var lambda = a.Arg<Action<SubmitForSecondaryApproval>>();

                lambda(rsc);

                Assert.That(rsc.DataCollectionId, Is.EqualTo(dataCollection.Id), "Invalid data collection id passed to the bus");
                Assert.That(rsc.ApprovedOn, Is.EqualTo(dataCollection.CurrentState.StateChangedOn).Within(1).Minutes, "Invalid approval date passed to the bus");
                Assert.That(rsc.ApprovedBy, Is.EqualTo(QaId), "Invalid approver id passed to the bus");
            });

            _controller.WithCallTo(c => c.SubmitForReapproval(vm)).ShouldRedirectTo(x => x.Index);
			_bus.Received().Send(Arg.Any<Action<SubmitForSecondaryApproval>>());
            _hashCodeRepository.Received().Delete(oldHashCode);
        }

        [Test]
        public void Verify_approvals_controller_is_decorated_with_RequiresAnyRoleType_attribute()
        {
            var methodInfo = _controller.GetType().GetCustomAttributes(typeof(RequiresAnyRoleType), false);
            Assert.That(methodInfo.Any(), Is.True, "RequiresAnyRoleType not found on the Approvals controller, this is needed!");
        }

        private static DataCollection CreateDataCollectionWithState(DataCollectionStatus status)
        {
            const int dataCollectionId = 1;
            return Builder<DataCollection>.CreateNew()
                .With(dc => dc.Id = dataCollectionId)
                .And(dc => dc.CurrentState = new DataCollectionState(status, DateTime.Now))
                .Build();
        }

        private static DataCollectionParty GetPartyWithManager()
        {
            return Builder<DataCollectionParty>.CreateNew().With(
                    p => p.Relationship = DataCollectionRelationshipType.Manager).And(
                        p => p.Party = Builder<Party>.CreateNew().Build()).Build();
        }

        private void CreateOrdApprover()
        {
            var user = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = SecondaryApproverId).Build();
            UserIs.AuthenticatedAs(_autoSubstitute, SecondaryApproverId, new[] { ApplicationRole.SecondaryApprover.GetDescription() });
            _lookup.GetUser(Arg.Is(SecondaryApproverId)).Returns(user);
        }

        private void CreateQaUser()
        {
			var user = Builder<UrdmsUser>.CreateNew().With(o => o.CurtinId = QaId).Build();
            UserIs.AuthenticatedAs(_autoSubstitute, QaId, new[] { ApplicationRole.QaApprover.GetDescription() });
            _lookup.GetUser(Arg.Is(QaId)).Returns(user);
        }

        private Project CreateProject(string curtinId = "XX12345")
        {
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
                    .With(o => o.UserId = curtinId)
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


            return project;
        }
    }
}
