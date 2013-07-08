using System;
using NServiceBus;
using NServiceBus.Logging;
using Urdms.Approvals.ApprovalService.Events;
using Urdms.Approvals.ViewModelUpdater.Database.Repositories;

namespace Urdms.Approvals.ViewModelUpdater
{
    public class ApprovalStateChangedHandler : IHandleMessages<ApprovalStateChanged>
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void Handle(ApprovalStateChanged message)
        {
            Log.InfoFormat("[URDMS] Received ApprovalStateChanged message id:{0} approvalState:{1}.", message.DataCollectionId, message.ApprovalState);
            
            try
            {
                var repository = new DataCollectionRepository();
                repository.UpdateStatus(message.DataCollectionId, message.ApprovalState, message.StateChangedOn);
            }
            catch (Exception ex)
            {
                Log.Error("[URDMS] Error hanling ApprovalStateChanged", ex);
                throw;
            }

        }
    }
}