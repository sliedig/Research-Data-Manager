using NServiceBus;

namespace Urdms.ProvisioningService.Messages
{
    public class CreateSiteResponse : IMessage
    {
        public int ProjectId { get; set; }
        public int RequestId { get; set; }
        public string SiteUrl { get; set; }
        public ProvisioningRequestStatus ProvisioningRequestStatus { get; set; }
    }

    public enum ProvisioningRequestStatus
    {
        Pending = 1,
        Provisioned = 2,
        Error = 3,
        TimeOut = 4
    }
}

