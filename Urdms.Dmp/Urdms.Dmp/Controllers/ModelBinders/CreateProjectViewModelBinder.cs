using System;
using System.Web.Mvc;
using Curtin.Framework.Common.UserService;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models;

namespace Urdms.Dmp.Controllers.ModelBinders
{
    public class CreateProjectViewModelBinder : DefaultModelBinder
    {
        public IProjectRepository ProjectRepository { get; set; }
        public ICurtinUserService LookUpService { get; set; }

        public CreateProjectViewModelBinder()
        {
            // Get repositories from DI container using property injection to allow easier testing
            ProjectRepository = DependencyResolver.Current.GetService<IProjectRepository>();
            LookUpService = DependencyResolver.Current.GetService<ICurtinUserService>();
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            var vm = new CreateProjectViewModel();
           
            if (!controllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }

            var currentUser = LookUpService.GetUser(controllerContext.HttpContext.User.Identity.Name);
            vm.Principalinvestigator = currentUser.FullName;
   
            bindingContext.ModelMetadata.Model = vm;

            return base.BindModel(controllerContext, bindingContext);
        }
    }
}
