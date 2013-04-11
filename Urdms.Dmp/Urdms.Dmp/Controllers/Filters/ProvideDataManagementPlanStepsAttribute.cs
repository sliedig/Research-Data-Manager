using System.Collections.Generic;
using System.Web.Mvc;
using Urdms.Dmp.Web.FlowForms;

namespace Urdms.Dmp.Controllers.Filters
{
    public class ProvideDataManagementPlanStepsAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var url = new UrlHelper(filterContext.RequestContext);
            var id = filterContext.RouteData.Values["id"];
            filterContext.Controller.ViewBag.Steps = new List<Step> {
                new Step { Url = url.Action("Edit", "Dmp", new {step = 1, id}), Name = "Section 1" },
                new Step { Url = url.Action("Edit", "Dmp", new {step = 2, id}), Name = "Section 2" },
                new Step { Url = url.Action("Edit", "Dmp", new {step = 3, id}), Name = "Section 3" },
                new Step { Url = url.Action("Edit", "Dmp", new {step = 4, id}), Name = "Section 4" },
                new Step { Url = url.Action("Edit", "Dmp", new {step = 5, id}), Name = "Section 5" },
                new Step { Url = url.Action("Review", "Confirm", new { id }), Name = "Confirm" }
            };
            filterContext.Controller.ViewBag.MaxStep = 5;
        }
    }

    
}
