using System.Web.Mvc;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.ApprovalModels;

namespace Urdms.Dmp.Controllers.ModelBinders
{
    public class DataCollectionApprovalViewModelStep1Binder : DefaultModelBinder
    {
        public IDataCollectionRepository DataCollectionRepository { get; set; }

        public DataCollectionApprovalViewModelStep1Binder()
        {
            DataCollectionRepository = DependencyResolver.Current.GetService<IDataCollectionRepository>();
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var requestForm =  controllerContext.HttpContext.Request.Form;
            var model = new DataCollectionApprovalViewModelStep1();
            int id;
            if (int.TryParse(requestForm["Id"],out id))
            {
                var collection = DataCollectionRepository.Get(id);
                model.ProjectId = collection.ProjectId;
            }
            bindingContext.ModelMetadata.Model = model;
            return base.BindModel(controllerContext, bindingContext); 
        }
    }
}