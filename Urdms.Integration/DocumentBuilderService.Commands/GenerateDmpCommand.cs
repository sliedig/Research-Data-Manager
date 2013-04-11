using NServiceBus;

namespace Urdms.DocumentBuilderService.Commands
{
    public class GenerateDmpCommand : ICommand
    {
        public int ProjectId { get; set; }
        public string SiteUrl { get; set; }
    }
}
