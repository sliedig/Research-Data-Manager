using NServiceBus;

namespace Urdms.ProvisioningService.Commands
{
    public class ForceProvisioningCompletionCommand : ICommand
    {
        public int ProjectId { get; set; }
        public string SiteUrl { get; set; }
    }
}