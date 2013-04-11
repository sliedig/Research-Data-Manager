using System.Collections.Generic;
using NServiceBus;

namespace Urdms.ProvisioningService.Commands
{
    public class SiteRequestCommand : ICommand
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public string ProjectDescription { get; set; }

        /// <summary>
        /// A dictionary of roles (Owners, Members and Visitors) and their corresponding users.
        /// <remarks>Use the role names for keys and add a comma seperated list of user ids as their values.</remarks>
        /// </summary>
        public Dictionary<string, string> UserRoles { get; set; }
    }
}