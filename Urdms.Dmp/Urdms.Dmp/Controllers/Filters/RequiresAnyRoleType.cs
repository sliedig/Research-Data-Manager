using System;
using Urdms.Dmp.Web.Controllers.Filters;
using Urdms.Dmp.Models.Enums;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp.Controllers.Filters
{
    public class RequiresAnyRoleType : RequiresAnyRole
    {
        public RequiresAnyRoleType(params ApplicationRole[] roles) : base(Array.ConvertAll(roles, r => Convert.ToString(r.GetDescription()))) { }
    }
}
