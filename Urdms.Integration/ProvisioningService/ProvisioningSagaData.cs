using System;
using NServiceBus.Saga;

namespace Urdms.ProvisioningService
{
    public class ProvisioningSagaData : IContainSagaData
    {
        // Required by nservicebus
        public virtual Guid Id { get; set; }
        public virtual string OriginalMessageId { get; set; }
        public virtual string Originator { get; set; }

        // stuff we will need to manage state.
        public virtual int ProjectId { get; set; }
        public virtual int RequestId { get; set; }
        public virtual string SiteUrl { get; set; }
        public virtual string ProvisioningRequestStatus { get; set; }


    }
}
