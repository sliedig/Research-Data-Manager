using System;
using NServiceBus.Testing;
using NUnit.Framework;
using Urdms.Approvals.ApprovalService;
using Urdms.Approvals.ApprovalService.Commands;
using Urdms.Approvals.ApprovalService.Events;
using Urdms.Approvals.ApprovalService.Messages;
using Urdms.Approvals.VivoPublisher.Messages;
using Urdms.NotificationService.Messages;

namespace ApprovalService.Tests
{
    [TestFixture]
    public class ApprovalServiceTests
    {
        private DateTime _now;
        private SubmitForApproval _submitForApprovalMessage;
        private SubmitForSecondaryApproval _submitForSecondaryApprovalMessage;
        private SubmitForFinalApproval _submitForFinalApprovalMessage;
        private SubmitForSecondaryReApproval _submitForSecondaryReApprovalMessage;
        private PublishDataCollection _publishDataCollectionMessage;
        private ExportToVivoResponse _exportToVivoResponse;


        [TestFixtureSetUp]
        public void Setup()
        {

            Test.Initialize();


            _now = DateTime.Now;

            _submitForApprovalMessage = new SubmitForApproval
                        {
                            ApprovedBy = "GH13579",
                            ApprovedOn = _now,
                            DataCollectionId = 1
                        };

            _submitForSecondaryApprovalMessage = new SubmitForSecondaryApproval
                        {
                            DataCollectionId = 1,
                            ApprovedBy = "FH13545",
                            ApprovedOn = _submitForApprovalMessage.ApprovedOn.AddDays(1)
                        };

            _submitForFinalApprovalMessage = new SubmitForFinalApproval
                        {
                            DataCollectionId = 1,
                            ApprovedBy = "787878r",
                            ApprovedOn = _submitForSecondaryApprovalMessage.ApprovedOn.AddDays(1)
                        };

            _submitForSecondaryReApprovalMessage = new SubmitForSecondaryReApproval
            {
                DataCollectionId = 1,
                ApprovedBy = "454545k",
                ApprovedOn = _submitForFinalApprovalMessage.ApprovedOn.AddDays(1)
            };


            _publishDataCollectionMessage = new PublishDataCollection
                                                {
                                                    DataCollectionId = 1,
                                                    ApprovedBy = "321312w",
                                                    ApprovedOn = _submitForFinalApprovalMessage.ApprovedOn.AddDays(2)
                                                };

            _exportToVivoResponse = new ExportToVivoResponse
                                        {
                                            DataCollectionId = 1
                                        };

        }


        [Test]
        public void Saga_should_publish_approval_state_changed_event_when_handling_submit_for_approval_command()
        {
            Test.Saga<ApprovalsSaga>()
                .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Submitted && m.StateChangedOn == _now)
                .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Submitted.ToString() && m.Approver == "GH13579")
                .When(x =>
                    {
                        x.Data.Approver = "GH13579";
                        x.Handle(_submitForApprovalMessage);
                    });
        }

        [Test]
        public void Saga_should_publish_approval_state_changed_event_when_handling_submit_ord_approval_command()
        {
            Test.Saga<ApprovalsSaga>()
                .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Submitted && m.StateChangedOn == _now)
                .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Submitted.ToString() && m.Approver == "GH13579")
                .When(x =>
                    {
                        x.Data.Approver = "GH13579";
                        x.Handle(_submitForApprovalMessage);
                    })
                .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.QaApproved && m.StateChangedOn == _submitForApprovalMessage.ApprovedOn.AddDays(1))
                .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.QaApproved.ToString() && m.Approver == "FH13545")
                .When(x =>
                    {
                        x.Data.Approver = "FH13545";
                        x.Handle(_submitForSecondaryApprovalMessage);
                    });
        }

        [Test]
        public void Saga_should_publish_approval_state_changed_event_when_handling_submit_final_approval_command()
        {
            Test.Saga<ApprovalsSaga>()
                .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Submitted && m.StateChangedOn == _now)
                .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Submitted.ToString() && m.Approver == "GH13579")
                .When(x =>
                    {
                        x.Data.Approver = "GH13579";
                        x.Handle(_submitForApprovalMessage);
                    })
                .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.QaApproved && m.StateChangedOn == _submitForApprovalMessage.ApprovedOn.AddDays(1))
                .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.QaApproved.ToString() && m.Approver == "FH13545")

                .When(x =>
                    {
                        x.Data.Approver = "FH13545";
                        x.Handle(_submitForSecondaryApprovalMessage);
                    })
                .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.SecondaryApproved && m.StateChangedOn == _submitForSecondaryApprovalMessage.ApprovedOn.AddDays(1))
                .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.SecondaryApproved.ToString() && m.Approver == "787878r")
                .When(x =>
                    {
                        x.Data.Approver = "787878r";
                        x.Handle(_submitForFinalApprovalMessage);
                    });
        }


        [Test]
        public void Saga_should_publish_approval_state_changed_event_when_handling_submit_for_ord_re_approval_command()
        {
            Test.Saga<ApprovalsSaga>()
                .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Submitted && m.StateChangedOn == _now)
                .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Submitted.ToString() && m.Approver == "GH13579")
                .When(x =>
                {
                    x.Data.Approver = "GH13579";
                    x.Handle(_submitForApprovalMessage);
                })
                .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.QaApproved && m.StateChangedOn == _submitForApprovalMessage.ApprovedOn.AddDays(1))
                .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.QaApproved.ToString() && m.Approver == "FH13545")
                .When(x =>
                {
                    x.Data.Approver = "FH13545";
                    x.Handle(_submitForSecondaryApprovalMessage);
                })
                .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.SecondaryApproved && m.StateChangedOn == _submitForSecondaryApprovalMessage.ApprovedOn.AddDays(1))
                .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.SecondaryApproved.ToString() && m.Approver == "787878r")
                .When(x =>
                {
                    x.Data.Approver = "787878r";
                    x.Handle(_submitForFinalApprovalMessage);
                })
                .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.RecordAmended && m.StateChangedOn == _submitForFinalApprovalMessage.ApprovedOn.AddDays(1))
                .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.RecordAmended.ToString() && m.Approver == "454545k")
                .When(x =>
                {
                    x.Data.Approver = "454545k";
                    x.Handle(_submitForSecondaryReApprovalMessage);
                });
        }

        [Test]
        public void Saga_should_publish_approval_state_changed_event_when_handling_publish_data_collection_command_and_complete()
        {
            Test.Saga<ApprovalsSaga>()
    .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Submitted && m.StateChangedOn == _now)
    .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Submitted.ToString() && m.Approver == "GH13579")
    .When(x =>
    {
        x.Data.Approver = "GH13579";
        x.Handle(_submitForApprovalMessage);
    })
    .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.QaApproved && m.StateChangedOn == _submitForApprovalMessage.ApprovedOn.AddDays(1))
    .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.QaApproved.ToString() && m.Approver == "FH13545")
    .When(x =>
    {
        x.Data.Approver = "FH13545";
        x.Handle(_submitForSecondaryApprovalMessage);
    })
    .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.SecondaryApproved && m.StateChangedOn == _submitForSecondaryApprovalMessage.ApprovedOn.AddDays(1))
    .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.SecondaryApproved.ToString() && m.Approver == "787878r")
    .When(x =>
    {
        x.Data.Approver = "787878r";
        x.Handle(_submitForFinalApprovalMessage);
    })
    .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Publishing)
    .ExpectSend<ExportToVivo>(m => m.DataCollectionId == 1)
    .When(x => x.Handle(_publishDataCollectionMessage))
    .ExpectPublish<ApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Published)
    .ExpectSend<NotifyApprovalStateChanged>(m => m.DataCollectionId == 1 && m.ApprovalState == DataCollectionApprovalState.Published.ToString() && m.Approver == "321312w")
        .When(x =>
        {
            x.Data.Approver = "321312w";
            x.Handle(_exportToVivoResponse);
        })
    .AssertSagaCompletionIs(true);



        }

    }
}
