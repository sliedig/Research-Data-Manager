using System.Collections.Generic;

namespace Urdms.Dmp.Utils
{
    public interface IDirectoryEntryService
    {
        IEnumerable<string> GetGroupMembership(string path, string domainUserName, string password, string userName);
    }
}