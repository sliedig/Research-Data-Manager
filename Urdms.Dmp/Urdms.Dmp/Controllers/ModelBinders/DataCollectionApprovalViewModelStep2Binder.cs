using System.Linq;
using System.Web.Mvc;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.ApprovalModels;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Controllers.ModelBinders
{
    public class DataCollectionApprovalViewModelStep2Binder : DefaultModelBinder
    {
        public IDataCollectionRepository DataCollectionRepository { get; set; }

        public DataCollectionApprovalViewModelStep2Binder()
        {
            DataCollectionRepository = DependencyResolver.Current.GetService<IDataCollectionRepository>();
        }
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var requestForm =  controllerContext.HttpContext.Request.Form;
            var model = new DataCollectionApprovalViewModelStep2();
            model.ExtractForCodes<DataCollectionFieldOfResearch>(requestForm);
            model.ExtractSeoCodes<DataCollectionSocioEconomicObjective>(requestForm);
            int id;
            if (int.TryParse(requestForm["Id"], out id))
            {
                var collection = DataCollectionRepository.Get(id);

                model.Manager = collection.Parties
                                .Where(p => p.Party.UserId != null && p.Relationship == DataCollectionRelationshipType.Manager)
                                .Single().Party;
                model.ProjectId = collection.ProjectId;
            }
            UserManagementHelper.DeserializeUrdmsUsers<DataCollectionRelationshipType>(model, requestForm);
            UserManagementHelper.DeserializeNonUrdmsUsers<DataCollectionRelationshipType>(model, requestForm);

            bindingContext.ModelMetadata.Model = model;

            return base.BindModel(controllerContext, bindingContext); 
        }
    }
}