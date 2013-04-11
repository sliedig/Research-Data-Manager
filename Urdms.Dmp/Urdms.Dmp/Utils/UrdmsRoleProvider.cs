using System.Collections.Generic;
using System.Web.Mvc;
using Curtin.Framework.Common.Auth;

namespace Urdms.Dmp.Utils
{
    public class UrdmsRoleProvider : IRoleProvider
    {
        public IEnumerable<string> RolesFor(string userName, IEnumerable<string> roles = null)
        {
            var appSettings = DependencyResolver.Current.GetService<IAppSettingsService>();
            var domainUserName = appSettings.LdapUser;
            var password = appSettings.LdapPassword;
            var path = appSettings.LdapUri;

            var directoryEntry = DependencyResolver.Current.GetService<IDirectoryEntryService>();
            return directoryEntry.GetGroupMembership(path, domainUserName, password, userName);
        }
    }
}
