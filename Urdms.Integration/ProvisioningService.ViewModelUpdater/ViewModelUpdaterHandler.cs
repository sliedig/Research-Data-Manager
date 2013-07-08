using System;
using NServiceBus;
using NServiceBus.Logging;
using Urdms.ProvisioningService.Events;
using Urdms.ProvisioningService.ViewModelUpdater.Database.Repositories;

namespace Urdms.ProvisioningService.ViewModelUpdater
{
    public class ViewModelUpdaterHandler : IHandleMessages<ProvisioningStatusChanged>
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected IDataCollectionRepository Repository { get; private set; }

        public ViewModelUpdaterHandler() : this(new DataCollectionRepository()) {}

        public ViewModelUpdaterHandler(IDataCollectionRepository repository)
        {
            Repository = repository;
        }

        #region Implementation of IMessageHandler<ProvisioningStatusChanged>

        public void Handle(ProvisioningStatusChanged message)
        {
            Log.InfoFormat("[URDMS] Handling event for ProvisioningStatusChanged for Project Id:{0}.", message.ProjectId);
             try
            {
                Repository.UpdateStatusByProjectId(message.ProjectId, message.ProvisioningRequestStatusId, message.SiteUrl, message.RequestId);
            }
            catch (Exception ex)
            {
                Log.Error("[URDMS] Error handling event for ProvisioningStatusChanged.", ex);
                throw;
            }
        }

        #endregion
    }
}
