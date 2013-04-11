using System;
using NServiceBus;

namespace Curtin.Urdms.DocumentBuilderService.Commands
{
    public class RegenerateDmpCommand : ICommand
    {
        public int ProjectId { get; set; }
        public string SiteUri { get; set; }
    }
}
