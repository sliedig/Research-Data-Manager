using System.Diagnostics;
using NServiceBus;
using NServiceBus.Saga;
using Urdms.Approvals.ApprovalService.Commands;
using Urdms.Approvals.ApprovalService.Events;
using Urdms.Approvals.ApprovalService.Messages;
using Urdms.Approvals.VivoPublisher.Messages;
using Urdms.NotificationService.Messages;

namespace Urdms.Approvals.ApprovalService
{
    public class ApprovalsSaga : Saga<ApprovalsSagaData>,
        IAmStartedByMessages<SubmitForApproval>,
        IHandleMessages<SubmitForSecondaryApproval>,
        IHandleMessages<SubmitForFinalApproval>,
        IHandleMessages<SubmitForSecondaryReApproval>,
        IHandleMessages<PublishDataCollection>,
        IHandleMessages<ExportToVivoResponse>
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public override void ConfigureHowToFindSaga()
        {
            ConfigureMapping<SubmitForApproval>(s => s.DataCollectionId, m => m.DataCollectionId);
            ConfigureMapping<SubmitForSecondaryApproval>(s => s.DataCollectionId, m => m.DataCollectionId);
            ConfigureMapping<SubmitForFinalApproval>(s => s.DataCollectionId, m => m.DataCollectionId);
            ConfigureMapping<SubmitForSecondaryReApproval>(s => s.DataCollectionId, m => m.DataCollectionId);
            ConfigureMapping<PublishDataCollection>(s => s.DataCollectionId, m => m.DataCollectionId);
            ConfigureMapping<ExportToVivoResponse>(s => s.DataCollectionId, m => m.DataCollectionId);
        }

        /// <summary>
        /// Handles the SubmitForApproval message.
        /// </summary>
        /// <param name="message">SubmitForApproval message.</param>
        public void Handle(SubmitForApproval message)
        {
            Log.InfoFormat("[URDMS] Received SubmitForApproval message id:{0}, approvedBy:{1}, approvedOn:{2}.", message.DataCollectionId, message.ApprovedBy, message.ApprovedOn);


            if (Data.DataCollectionId != 0)
            {
                // An instance already exists for this DataCollection. There cannot be more than one.
                Log.Warn("[URDMS] Saga Instance already exists for this Data Collection. Saga will not continue processing this message.");
                Bus.DoNotContinueDispatchingCurrentMessageToHandlers();

            }
            else
            {
                // Save state
                Data.DataCollectionId = message.DataCollectionId;
                Data.ApprovalState = DataCollectionApprovalState.Submitted;
                Data.CollectionOwner = message.ApprovedBy;
                Data.StateChangedOn = message.ApprovedOn;
                Data.Approver = message.ApprovedBy;

                Log.InfoFormat("[URDMS] Publishing ApprovalStateChanged for id:{0}", message.DataCollectionId);

                // Change the approvalState of the DataCollection
                Bus.Publish<ApprovalStateChanged>(m =>
                        {
                            m.DataCollectionId = message.DataCollectionId;
                            m.ApprovalState = DataCollectionApprovalState.Submitted;
                            m.StateChangedOn = message.ApprovedOn;
                            m.Approver = message.ApprovedBy;
                        });

                Bus.Send<NotifyApprovalStateChanged>(m =>
                    {
                        m.DataCollectionId = message.DataCollectionId;
                        m.ApprovalState = DataCollectionApprovalState.Submitted.ToString();
                        m.Approver = message.ApprovedBy;
                    });
            }
        }


        /// <summary>
        /// Handles the SubmitForSecondaryApproval message.
        /// </summary>
        /// <param name="message">SubmitForSecondaryApproval message.</param>
        public void Handle(SubmitForSecondaryApproval message)
        {
            Log.InfoFormat("[URDMS] Received SubmitForSecondaryApproval message id:{0}, approvedBy:{1}, approvedOn:{2}.", message.DataCollectionId, message.ApprovedBy, message.ApprovedOn);

            Debug.Assert(Data.ApprovalState == DataCollectionApprovalState.Submitted);

            // Ensure that the current expected state is Submitted. 
            // If not, then this handler should not be processing the message.
            if (Data.ApprovalState != DataCollectionApprovalState.Submitted)
            {
                // An instance already exists for this DataCollection. There cannot be more than one.
                Log.WarnFormat("[URDMS] Saga instance is in state {0}, expected Submitted. Saga will not continue processing this message.", Data.ApprovalState);
                Bus.DoNotContinueDispatchingCurrentMessageToHandlers();
            }
            else
            {
                // Update State
                Data.Approver = message.ApprovedBy;
                Data.StateChangedOn = message.ApprovedOn;
                Data.ApprovalState = DataCollectionApprovalState.QaApproved;

                Log.InfoFormat("[URDMS] Publishing ApprovalStateChanged for id:{0}", message.DataCollectionId);
                // Change the approvalState of the DataCollection
                Bus.Publish<ApprovalStateChanged>(m =>
                        {
                            m.DataCollectionId = message.DataCollectionId;
                            m.ApprovalState = DataCollectionApprovalState.QaApproved;
                            m.StateChangedOn = message.ApprovedOn;
                            m.Approver = message.ApprovedBy;
                        });

                Bus.Send<NotifyApprovalStateChanged>(m =>
                {
                    m.DataCollectionId = message.DataCollectionId;
                    m.ApprovalState = DataCollectionApprovalState.QaApproved.ToString();
                    m.Approver = message.ApprovedBy;
                });
            }
        }

        /// <summary>
        /// Handles the SubmitForFinalApproval message.
        /// </summary>
        /// <param name="message">SubmitForFinalApproval message.</param>
        public void Handle(SubmitForFinalApproval message)
        {
            Log.InfoFormat("[URDMS] Received SubmitForFinalApproval message id:{0}, approvedBy:{1}, approvedOn:{2}.", message.DataCollectionId, message.ApprovedBy, message.ApprovedOn);

            Debug.Assert(Data.ApprovalState == DataCollectionApprovalState.QaApproved || Data.ApprovalState == DataCollectionApprovalState.RecordAmended);

            // Ensure that the current expected state is QaApproved. 
            // If not, then this handler should not be processing the message.
            if (Data.ApprovalState != DataCollectionApprovalState.QaApproved && Data.ApprovalState != DataCollectionApprovalState.RecordAmended)
            {
                // An instance already exists for this DataCollection. There cannot be more than one.
                Log.WarnFormat("[URDMS] Saga instance is in state {0}, expected QaApproved or RecordAmended. Saga will not continue processing this message.", Data.ApprovalState);
                Bus.DoNotContinueDispatchingCurrentMessageToHandlers();
            }
            else
            {
                // Update State
                Data.Approver = message.ApprovedBy;
                Data.StateChangedOn = message.ApprovedOn;
                Data.ApprovalState = DataCollectionApprovalState.SecondaryApproved;

                Log.InfoFormat("[URDMS] Publishing ApprovalStateChanged for id:{0}", message.DataCollectionId);
                // Change the approvalState of the DataCollection
                Bus.Publish<ApprovalStateChanged>(m =>
                {
                    m.DataCollectionId = message.DataCollectionId;
                    m.ApprovalState = DataCollectionApprovalState.SecondaryApproved;
                    m.StateChangedOn = message.ApprovedOn;
                    m.Approver = message.ApprovedBy;
                });

                Bus.Send<NotifyApprovalStateChanged>(m =>
                {
                    m.DataCollectionId = message.DataCollectionId;
                    m.ApprovalState = DataCollectionApprovalState.SecondaryApproved.ToString();
                    m.Approver = message.ApprovedBy;
                });

            }
        }

        /// <summary>
        /// Handles the SubmitForSecondaryReApproval message.
        /// </summary>
        /// <param name="message">SubmitForSecondaryReApproval message.</param>
        public void Handle(SubmitForSecondaryReApproval message)
        {
            Log.InfoFormat("[URDMS] Received SubmitForSecondaryReApproval message id:{0}, approvedBy:{1}, approvedOn:{2}.", message.DataCollectionId, message.ApprovedBy, message.ApprovedOn);

            Debug.Assert(Data.ApprovalState == DataCollectionApprovalState.SecondaryApproved);

            // Ensure that the current expected state is OrdApproved or RecordAmended. 
            // If not, then this handler should not be processing the message.
            if (Data.ApprovalState != DataCollectionApprovalState.SecondaryApproved)
            {
                // An instance already exists for this DataCollection. There cannot be more than one.
                Log.WarnFormat("[URDMS] Saga instance is in state {0}, expected OrdApproved. Saga will not continue processing this message.", Data.ApprovalState);
                Bus.DoNotContinueDispatchingCurrentMessageToHandlers();
            }
            else
            {
                // Update State
                Data.Approver = message.ApprovedBy;
                Data.StateChangedOn = message.ApprovedOn;
                Data.ApprovalState = DataCollectionApprovalState.RecordAmended;

                Log.InfoFormat("[URDMS] Publishing ApprovalStateChanged for id:{0}", message.DataCollectionId);
                // Change the approvalState of the DataCollection
                Bus.Publish<ApprovalStateChanged>(m =>
                {
                    m.DataCollectionId = message.DataCollectionId;
                    m.ApprovalState = DataCollectionApprovalState.RecordAmended;
                    m.StateChangedOn = message.ApprovedOn;
                    m.Approver = message.ApprovedBy;
                });

                Bus.Send<NotifyApprovalStateChanged>(m =>
                {
                    m.DataCollectionId = message.DataCollectionId;
                    m.ApprovalState = DataCollectionApprovalState.RecordAmended.ToString();
                    m.Approver = message.ApprovedBy;
                });
            }
        }

        /// <summary>
        /// Handles the PublishDataCollection message.
        /// </summary>
        /// <param name="message">PublishDataCollection message.</param>
        public void Handle(PublishDataCollection message)
        {
            Log.InfoFormat("[URDMS] Received PublishDataCollection message id:{0}, approvedBy:{1}, approvedOn:{2}.", message.DataCollectionId, message.ApprovedBy, message.ApprovedOn);

            Debug.Assert(Data.ApprovalState == DataCollectionApprovalState.SecondaryApproved);

            // Ensure that the current expected state is OrdApproved or RecordAmended. 
            // If not, then this handler should not be processing the message.
            if (Data.ApprovalState != DataCollectionApprovalState.SecondaryApproved)
            {
                // An instance already exists for this DataCollection. There cannot be more than one.
                Log.WarnFormat("[URDMS] Saga instance is in state {0}, expected OrdApproved. Saga will not continue processing this message.", Data.ApprovalState);
                Bus.DoNotContinueDispatchingCurrentMessageToHandlers();
            }
            else
            {
                // Update State
                Data.Approver = message.ApprovedBy;
                Data.StateChangedOn = message.ApprovedOn;
                Data.ApprovalState = DataCollectionApprovalState.Publishing;

                Log.InfoFormat("[URDMS] Publishing ApprovalStateChanged for id:{0}", message.DataCollectionId);
                // Change the approvalState of the DataCollection
                Bus.Publish<ApprovalStateChanged>(m =>
                {
                    m.DataCollectionId = message.DataCollectionId;
                    m.ApprovalState = DataCollectionApprovalState.Publishing;
                    m.StateChangedOn = message.ApprovedOn;
                    m.Approver = message.ApprovedBy;
                });

                Log.InfoFormat("[URDMS] Sending ExportToVivo message to VivoPublisher id:{0}", message.DataCollectionId);
                Bus.Send<ExportToVivo>(m => m.DataCollectionId = message.DataCollectionId);
            }
        }

        /// <summary>
        /// Handles the ExportToVivoResponse message.
        /// </summary>
        /// <param name="message">ExportToVivoResponse message.</param>
        public void Handle(ExportToVivoResponse message)
        {
            Log.InfoFormat("[URDMS] Received ExportToVivoResponse message id:{0}, RecordPublishedOn:{1}.", message.DataCollectionId, message.RecordPublishedOn);

            // Update State
            Data.ApprovalState = DataCollectionApprovalState.Published;

            // Change the approvalState of the DataCollection
            Bus.Publish<ApprovalStateChanged>(m =>
            {
                m.DataCollectionId = Data.DataCollectionId;
                m.ApprovalState = DataCollectionApprovalState.Published;
                m.StateChangedOn = message.RecordPublishedOn;
                m.Approver = Data.Approver;
            });

            Bus.Send<NotifyApprovalStateChanged>(m =>
            {
                m.DataCollectionId = message.DataCollectionId;
                m.ApprovalState = DataCollectionApprovalState.Published.ToString();
                m.Approver = Data.Approver;
            });

            MarkAsComplete();
        }
    }
}
