using System;
using System.Collections.Generic;
using Curtin.Urdms.ProvisioningService.Domain;
using NServiceBus;

namespace Curtin.Urdms.ProvisioningService.SharePoint.Messages
{
    public class CreateSiteRequest : IMessage
    {
        public int ProjectId { get; set; }
        public string SiteTitle { get; set; }
        public string SiteDescription { get; set; }
        public List<SiteUser> UsersInRoles { get; set; } 

    }
}