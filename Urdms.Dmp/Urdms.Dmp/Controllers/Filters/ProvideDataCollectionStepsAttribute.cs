using System.Collections.Generic;
using System.Web.Mvc;
using Urdms.Dmp.Web.FlowForms;

namespace Urdms.Dmp.Controllers.Filters
{
    public class ProvideDataCollectionStepsAttribute : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var url = new UrlHelper(filterContext.RequestContext);
            var id = filterContext.RouteData.Values["id"];

            if (filterContext.Controller.GetType() == typeof(DataCollectionController))
            {
                var projectId = filterContext.RouteData.Values["projectId"];

                filterContext.Controller.ViewBag.Steps = new List<Step>
                                                             {
                                                                 new Step {Url = url.Action("Step1", "DataCollection", new {id, projectId}), Name = "Section 1"},
                                                                 new Step {Url = url.Action("Step2", "DataCollection", new {id, projectId}), Name = "Section 2"}
                                                             };
                filterContext.Controller.ViewBag.MaxStep = 2;
            }
            else if (filterContext.Controller.GetType() == typeof(ApprovalController))
            {
                filterContext.Controller.ViewBag.Steps = new List<Step>
                                                             {
                                                                 new Step {Url = url.Action("Step1", "Approval", new {id }), Name = "Section 1"},
                                                                 new Step {Url = url.Action("Step2", "Approval", new {id }), Name = "Section 2"},
                                                                 new Step {Url = url.Action("Confirm", "Approval", new {id}), Name = "Confirm"}
                                                             };
                filterContext.Controller.ViewBag.MaxStep = 2;
            }
        }
    }
}
