using System.Configuration;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using BoC.Web.Mvc.PrecompiledViews;
using Curtin.Framework.Common.Attributes;
using Curtin.Framework.Common.Auth;
using Curtin.Framework.Database.Migrations;
using Urdms.Dmp.Web;
using Urdms.Dmp.Config.Autofac;
using Urdms.Dmp.Controllers.Filters;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Controllers.ModelBinders;
using Urdms.Dmp.Utils;

namespace Urdms.Dmp
{
    public static class AppRoutes
    {
        public static void Register(RouteCollection routes)
        {
            // PageController Actions
            routes.MapRoute("Homepage", "", new { controller = "Page", action = "Index", id = UrlParameter.Optional });
            routes.MapRoute("Instructions", "Instructions", new { controller = "Page", action = "Instructions" });

            // ProjectController Actions
            routes.MapRoute("MyProjects", "Projects", new { controller = "Project", action = "Index" });
            routes.MapRoute("NewProjects", "Projects/New", new { controller = "Project", action = "NewProjects" });
            routes.MapRoute("NewProject", "Project/New", new { controller = "Project", action = "New" });
            routes.MapRoute("ScriptProjectIntroduction", "Project/ScriptIntroduction", new { controller = "Project", action = "ScriptIntroduction" });
            routes.MapRoute("ScriptProjectSelection", "Project/Select", new { controller = "Project", action = "Select" });
            routes.MapRoute("ProjectIntroduction", "Project/Introduction", new { controller = "Project", action = "Introduction" });
            routes.MapRoute("LinkToExistingProject", "Project/LinkToExisting", new { controller = "Project", action = "LinkToExisting" });
            routes.MapRoute("CopyDmp", "Project/CopyDmp/{id}", new { controller = "Project", action = "CopyDmp", id = 0 });
            routes.MapRoute("ScriptProjectReadOnly", "Project/View/{id}", new { controller = "Project", action = "ReadOnlyScriptProject", id = 0 });
            routes.MapRoute("Project", "Project/{id}", new { controller = "Project", action = "Project", id = UrlParameter.Optional });

            // DmpController Actions
            routes.MapRoute("DmpNew", "Project/{id}/Dmp/New", new { controller = "Dmp", action = "New" });
            routes.MapRoute("DmpEdit", "Project/Dmp/Edit/{id}/{step}", new { controller = "Dmp", action = "Edit", id = 0, step = 1 });

			// ConfirmController Actions
            routes.MapRoute("DataDepositReview", "DataDepositProject/{projectid}/DataDeposit/Review", new { controller = "Confirm", action = "ReviewDataDeposit" });
            routes.MapRoute("DataDepositSubmitted", "DataDepositProject/{projectid}/DataDeposit/Submitted", new { controller = "Confirm", action = "SubmittedDataDeposit" });
            routes.MapRoute("DmpConfirmation", "Project/Dmp/{action}/{id}", new { controller = "Confirm", action = "Review" });
            routes.MapRoute("DmpRepublished", "Project/Dmp/Republished", new { controller = "Confirm", action = "Republished" });



            // DataCollection Controller Actions
            routes.MapRoute("ListDataCollections", "Project/{id}/DataCollections", new { controller = "DataCollection", action = "Index" });
            routes.MapRoute("DataCollectionsReadOnly", "Project/{projectId}/DataCollection/View/{id}", new { controller = "DataCollection", action = "ViewReadOnlyDataCollection", id = UrlParameter.Optional });
            routes.MapRoute("DataCollections", "Project/{projectId}/DataCollection/{action}/{id}", new { controller = "DataCollection", action = "Step1", id = UrlParameter.Optional });
            routes.MapRoute("DataCollectionsNew", "Project/{projectId}/DataCollection/New", new { controller = "DataCollection", action = "New" });

            // DataDeposit Controller Actions
            routes.MapRoute("DataDeposit", "DataDepositProject/{projectId}/DataDeposit/", new { controller = "DataDeposit", action = "New" });
            routes.MapRoute("DataDepositProject", "DataDepositProject/{id}", new { controller = "DataDeposit", action = "Project", id = UrlParameter.Optional });
            routes.MapRoute("DataDepositEdit", "DataDepositProject/DataDeposit/Edit/{id}", new { controller = "DataDeposit", action = "Edit", id = 0 });

            // Ajax Controller Actions
            routes.MapRoute("AjaxUsersForApproval", "Ajax/GetNewUrdmsUserForApproval/{term}/{dataCollectionId}", new { controller = "Ajax", action = "GetNewUrdmsUserForApproval" });
            routes.MapRoute("AjaxUsers", "Ajax/{action}/{term}/{userType}", new { controller = "Ajax" });
            routes.MapRoute("Ajax", "Ajax/{action}/{term}", new { controller = "Ajax", term = UrlParameter.Optional });

            // ApprovalsController Actions
            routes.MapRoute("Approvals", "Approvals/{action}/{id}", new { controller = "Approval", action = "Index", id = UrlParameter.Optional });

            // LibGuideController Actions
            routes.MapRoute("LibGuide", "LibGuide/{item}", new { controller = "LibGuide", action = "Index", account = 1470, item = 0 });

            // AdminController Actions
            routes.MapRoute("AdminHomepage", "Admin/{action}", new { controller = "Admin", action = "Index" });
        }
    }

    [NoCoverage]
    public class App : UrdmsApplication
    {

        protected void Application_Start()
        {
            Trace.WriteLine("Application_Start called.");

            ApplicationPartRegistry.Register(typeof(UrdmsApplication).Assembly);

            var connectionString = ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
            AreaRegistration.RegisterAllAreas();
            AppRoutes.Register(RouteTable.Routes);

            RegisterDefaultRoutes(RouteTable.Routes);
            RegisterDependencies(builder =>
            {
                builder.RegisterType<UrdmsRoleProvider>().As<IRoleProvider>();
                builder.RegisterType<LibGuideService>().As<ILibGuideService>();
                builder.RegisterType<SimpleWebRequestService>().As<ISimpleWebRequestService>();
                builder.RegisterType<AppSettingsService>().As<IAppSettingsService>();
                builder.RegisterType<CsvHelper>().As<ICsvHelper>();
                builder.RegisterModule(new NHibernateModule { ConnectionString = connectionString });
                builder.RegisterModule(new DataAccessModule());
                builder.RegisterModule(new NServiceBusModule());

                builder.RegisterType<DummyMembershipService>().As<IMembershipService>(); // TODO: Put your user service implementation here
                builder.RegisterType<DummyDirectoryEntryService>().As<IDirectoryEntryService>(); // TODO: Put your user service implementation here
				builder.RegisterModule(new UserServiceModule());
            });

            FilterProviders.Providers.Add(new FilterProvider());
            ModelBinderProviders.BinderProviders.Add(new AppModelBinderProvider());
            Trace.WriteLine("Updating database.");

            Migrations.UpdateDatabase(connectionString, typeof(App).Assembly);

            Trace.WriteLine("Application_Start ended.");
        }
    }
}
