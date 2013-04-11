using System.Collections.Specialized;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutofacContrib.NSubstitute;
using NSubstitute;

namespace Urdms.Dmp.Tests.Helpers
{
    public static class AutoSubstituteContainer
    {
        public static AutoSubstitute Create()
        {
            var autoSubstitute = new AutoSubstitute();

            var httpContext = Substitute.For<HttpContextBase>();
            autoSubstitute.Provide(httpContext);

            var server = Substitute.For<HttpServerUtilityBase>();
            httpContext.Server.Returns(server);

            var request = Substitute.For<HttpRequestBase>();
            var parameters = new NameValueCollection();
            request.Params.Returns(parameters);
            var formParameters = new NameValueCollection();
            request.Form.Returns(formParameters);
            var headers = new NameValueCollection();
            request.Headers.Returns(headers);
            autoSubstitute.Provide(request);
            httpContext.Request.Returns(request);

            request.Cookies.Returns(new HttpCookieCollection());

            var response = Substitute.For<HttpResponseBase>();
            autoSubstitute.Provide(response);
            httpContext.Response.Returns(response);

            var routeBase = Substitute.For<RouteBase>();
            autoSubstitute.Provide(routeBase);

            var routeData = new RouteData(routeBase, Substitute.For<IRouteHandler>());

            var requestContext = Substitute.For<RequestContext>();
            requestContext.RouteData = routeData;
            requestContext.HttpContext = httpContext;
            autoSubstitute.Provide(requestContext);

            var actionExecutingContext = Substitute.For<ActionExecutingContext>();
            actionExecutingContext.HttpContext.Returns(httpContext);
            actionExecutingContext.RouteData.Returns(routeData);
            actionExecutingContext.RequestContext = requestContext;
            autoSubstitute.Provide(actionExecutingContext);

            var controller = Substitute.For<ControllerBase>();
            autoSubstitute.Provide(controller);
            actionExecutingContext.Controller.Returns(controller);

            var controllerContext = Substitute.For<ControllerContext>();
            controllerContext.HttpContext = httpContext;
            controllerContext.RouteData = routeData;
            controllerContext.RequestContext = requestContext;
            controllerContext.Controller = controller;
            autoSubstitute.Provide(controllerContext);
            controller.ControllerContext = controllerContext;

            var iView = Substitute.For<IView>();
            autoSubstitute.Provide(iView);

            var viewDataDictionary = new ViewDataDictionary();
            autoSubstitute.Provide(viewDataDictionary);

            var iViewDataContainer = Substitute.For<IViewDataContainer>();
            iViewDataContainer.ViewData.Returns(viewDataDictionary);
            autoSubstitute.Provide(iViewDataContainer);

            var textWriter = Substitute.For<TextWriter>();
            autoSubstitute.Provide(textWriter);

            var viewContext = new ViewContext(controllerContext, iView, viewDataDictionary, new TempDataDictionary(), textWriter);
            viewContext.HttpContext = httpContext;
            viewContext.RouteData = routeData;
            viewContext.RequestContext = requestContext;
            viewContext.Controller = controller;
            autoSubstitute.Provide(viewContext);

            return autoSubstitute;
        }

        public static HtmlHelper<T> GetHtmlHelper<T>(this AutoSubstitute autoSubstitute)
        {
            return new HtmlHelper<T>(autoSubstitute.Resolve<ViewContext>(), autoSubstitute.Resolve<IViewDataContainer>());
        }

        public static T GetController<T>(this AutoSubstitute autoSubstitute) where T : Controller
        {
            var controller = autoSubstitute.Resolve<T>();
            controller.ControllerContext = autoSubstitute.Resolve<ControllerContext>();
            return controller;
        }

        public static void IsAjaxRequest(this AutoSubstitute autoSubstitute)
        {
            autoSubstitute.Resolve<HttpRequestBase>().Headers["X-Requested-With"] = "XMLHttpRequest";
        }
    }
}
