using System.Linq;
using System.Web.Mvc;
using Urdms.Dmp.Controllers.Helpers;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.DataCollectionModels;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Controllers.ModelBinders
{
    public class DataCollectionViewModelStep2Binder : DefaultModelBinder
    {
        public IDataCollectionRepository DataCollectionRepository { get; set; }

        public DataCollectionViewModelStep2Binder()
        {
            DataCollectionRepository = DependencyResolver.Current.GetService<IDataCollectionRepository>();
        }
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var form =  controllerContext.HttpContext.Request.Form;
            var model = new DataCollectionViewModelStep2();
            model.ExtractForCodes<DataCollectionFieldOfResearch>(form);
            model.ExtractSeoCodes<DataCollectionSocioEconomicObjective>(form);
            int collectionDescriptionId;
            if (int.TryParse(form["Id"], out collectionDescriptionId))
            {
                var collection = DataCollectionRepository.Get(collectionDescriptionId);

                model.Manager = collection.Parties
                                .Where(p => p.Party.UserId != null && p.Relationship == DataCollectionRelationshipType.Manager)
                                .Single().Party;
            }
            model.DeserializeUrdmsUsers<DataCollectionRelationshipType>(form);
            model.DeserializeNonUrdmsUsers<DataCollectionRelationshipType>(form);

            bindingContext.ModelMetadata.Model = model;

            return base.BindModel(controllerContext, bindingContext); 
        }
    }
}