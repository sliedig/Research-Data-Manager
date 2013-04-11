using System;
using System.Collections.Generic;
using NServiceBus;

namespace Urdms.ProvisioningService.Messages
{
    public class CreateSiteRequest : IMessage
    {
        public int ProjectId { get; set; }
        public string SiteTitle { get; set; }
        public string SiteDescription { get; set; }
        public List<SiteUser> UsersInRoles { get; set; } 
    }

    [Serializable]
    public class SiteUser
    {
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}