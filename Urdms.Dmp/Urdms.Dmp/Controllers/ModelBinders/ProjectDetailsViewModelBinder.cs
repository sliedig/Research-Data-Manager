using System.Web.Mvc;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Controllers.ModelBinders
{
    public class ProjectDetailsViewModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var requestForm = controllerContext.HttpContext.Request.Form;
            var model = new ProjectDetailsViewModel();
            model.ExtractForCodes<ProjectFieldOfResearch>(requestForm);
            model.ExtractSeoCodes<ProjectSocioEconomicObjective>(requestForm);
            if (controllerContext.Controller.GetType() == typeof(DataDepositController))
            {
                model.SourceProjectType = SourceProjectType.DEPOSIT;
            }
            bindingContext.ModelMetadata.Model = model;
            return base.BindModel(controllerContext, bindingContext); 
        }
    }
}