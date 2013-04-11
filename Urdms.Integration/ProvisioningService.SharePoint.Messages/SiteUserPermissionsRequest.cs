using System;
using System.Collections.Generic;
using NServiceBus;

namespace Curtin.Urdms.ProvisioningService.SharePoint.Messages
{
    public class SiteUserPermissionsRequest : IMessage
    {
        public int ProjectId { get; set; }
        public string SiteUri { get; set; }
        public List<SiteUser> UsersInRoles { get; set; } // list of users and associated sharepoint roles
    }
}
