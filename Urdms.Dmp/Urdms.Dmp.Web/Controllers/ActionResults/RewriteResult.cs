using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Curtin.Framework.Common.Extensions;

namespace Urdms.Dmp.Web.Controllers.ActionResults {
    public class RewriteResult : ActionResult
    {
        public object RouteValues { get; private set; }
        public string Url { get; private set; }

        public RewriteResult(string url)
        {
            Url = url;
        }

        public RewriteResult(object routeValues)
        {
            RouteValues = routeValues;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var urlHelper = new UrlHelper(context.RequestContext, RouteTable.Routes);

            Url = RouteValues != null ? urlHelper.RouteUrl(RouteValues) : urlHelper.Content(Url);

            if (HttpRuntime.UsingIntegratedPipeline && !RoleEnvironment.IsUsingDevFabric())
                context.HttpContext.Server.TransferRequest(Url, true);
            else
                context.HttpContext.Response.Redirect(Url, false);
        }
    }

    internal static class RoleEnvironment
    {
        public static Func<bool> IsAvailable = () =>
        {
            try
            {
                return Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.IsAvailable;
            }
            catch(Exception)
            {
                return false;
            }
        };
        public static Func<bool> IsUsingDevFabric = () => IsAvailable() && Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.DeploymentId.NullSafe().StartsWith("deployment(");
    }
}