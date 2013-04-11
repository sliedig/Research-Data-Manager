using System.Collections.Generic;
using System.Web.Mvc;
using Urdms.Dmp.Web.Controllers.Filters;

namespace Urdms.Dmp.Controllers.Filters
{
    public class FilterProvider : IFilterProvider
    {
        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
#if !DEBUG
            yield return new Filter(new RedirectToHttpsAttribute(), FilterScope.Global, App.HttpsRedirectOrder);
#endif
            yield return new Filter(new RequiresAllRoles("staff"), FilterScope.Global, App.ActiveAuthenticationOrder);
        }
    }
}
