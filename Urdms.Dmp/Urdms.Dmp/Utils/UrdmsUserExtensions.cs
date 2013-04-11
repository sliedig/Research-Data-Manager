using System;
using System.Linq;
using Curtin.Framework.Common.UserService;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Utils
{
    public static class UrdmsUserExtensions
    {
        public static bool IsPrincipalInvestigatorFor(this ICurtinUser user, Project entity)
        {
            return entity.Parties.Any(o => o.Party.UserId.Equals(user.CurtinId, StringComparison.InvariantCultureIgnoreCase) && o.Relationship == ProjectRelationship.PrincipalInvestigator);
        }
    }
}
