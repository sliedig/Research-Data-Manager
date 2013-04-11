using System.Collections.Generic;
using NServiceBus;

namespace Urdms.NotificationService.Messages
{
    public class NotifyRequestForSiteReceived : IMessage
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<string> UserIds { get; set; }
    }
}
