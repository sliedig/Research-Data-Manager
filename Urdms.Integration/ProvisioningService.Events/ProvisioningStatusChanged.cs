using NServiceBus;

namespace Urdms.ProvisioningService.Events
{
    public interface ProvisioningStatusChanged : IEvent
    {
        int ProjectId { get; set; }
        int ProvisioningRequestStatusId { get; set; }
        int RequestId { get; set; }
        string SiteUrl { get; set; }
    }
}