using System;
using System.Reflection;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Autofac;
using Autofac.Integration.Mvc;
using Urdms.Dmp.Web.Container;
using Urdms.Dmp.Web.FlowForms;

namespace Urdms.Dmp.Web
{
    public class UrdmsApplication : HttpApplication
    {
        public const int HttpsRedirectOrder = -100;
        public const int ActiveAuthenticationOrder = -20;
        public const int PassiveAuthenticationOrder = -50;

        /// <summary>
        /// Registers default routes for the Urdms Web Framework; call after registering app specific routes since it has a catch-all for 404's.
        /// Includes:
        ///     Homepage        /               Page.Index
        ///     Authentication  /Auth/{action}  Auth.{action}
        ///     Error           /Error/{action} Error.{action}
        ///     NotFound        *               Error.NotFound
        /// </summary>
        /// <param name="routes">The routes collection that is being built</param>
        public static void RegisterDefaultRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("favicon.ico");

            // Authentication
            routes.MapRoute("Authentication", "Auth/{action}", new { controller = "Auth" });

            // Error
            routes.MapRoute("Error", "Error/{action}", new { controller = "Error" });

            // 404
            routes.MapRoute("NotFound", "{*path}", new { controller = "Error", action = "NotFound" });

        }

        /// <summary>
        /// Lambda that takes a container builder and returns nothing.
        /// @see RegisterDependencies().
        /// </summary>
        /// <param name="builder">Container builder parameter</param>
        public delegate void Dependencies(ContainerBuilder builder);

        /// <summary>
        /// Sets up Autofac to work with an MVC3 environment.
        /// Should be called from Application_Start
        /// </summary>
        /// <param name="d">A lambda that takes a container builder that then registers any dependencies within the application</param>
        /// <returns>The container so that the application can do any resolves necessary within Application_Start</returns>
        public IContainer RegisterDependencies(Dependencies d)
        {
            var builder = new ContainerBuilder();
            RegisterWebbyStuff(builder);
            builder.RegisterModule<AuthModule>();

            d(builder);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

            // Enable the flow forms model metadata provider
            ModelMetadataProviders.Current = new MetadataProvider();

            // Enable the flow forms Validator Provider to fix MVC bug
            DataAnnotationsModelValidatorProvider.RegisterDefaultValidatableObjectAdapterFactory((m, c) => new CustomValidatableObjectAdapter(m, c));

            return container;
        }

        private void RegisterWebbyStuff(ContainerBuilder builder)
        {
            builder.RegisterModelBinderProvider();
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterSource(new ViewRegistrationSource());

            // BaseType is the one defined on the WebProject. GetType is in the aspnet generated assembly
            builder.RegisterControllers(GetType().BaseType.Assembly);       
            builder.RegisterModelBinders(GetType().BaseType.Assembly);
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
        }

        protected virtual void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            var authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                var roles = authTicket.UserData.Split(',');
                var userPrincipal = new GenericPrincipal(new GenericIdentity(authTicket.Name), roles);
                Context.User = userPrincipal;
            }
        }

        [Obsolete("Reference Curtin.Framework.Database and call Migrations.UpdateDatabase", true)]
        protected static void UpdateDatabase(string connectionString, Assembly migrationAssembly) {}
        [Obsolete("Reference Curtin.Framework.Database and call Migrations.UpdateDatabase", true)]
        protected static void FluentlyUpdateDatabase(string connectionString, params Assembly[] migrationAssembly) {}
    }
}
