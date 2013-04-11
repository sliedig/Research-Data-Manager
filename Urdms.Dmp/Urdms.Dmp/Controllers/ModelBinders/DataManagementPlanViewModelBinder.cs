using System.Web.Mvc;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Controllers.ModelBinders
{
    public class DataManagementPlanViewModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var form = controllerContext.HttpContext.Request.Form;
            var model = new DataManagementPlanViewModel();
            model.DeserializeUrdmsUsers<AccessRole>(form);
            model.DeserializeNonUrdmsUsers<AccessRole>(form);
            bindingContext.ModelMetadata.Model = model;
            return base.BindModel(controllerContext, bindingContext); 
        }
    }
}