using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using Urdms.Dmp.Database.Repositories;
using Urdms.Dmp.Models.DataCollectionModels;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Controllers.ModelBinders
{
    public class DataCollectionListViewModelBinder : DefaultModelBinder
    {
        public IDataCollectionRepository DataCollectionRepository { private get; set; }

        public DataCollectionListViewModelBinder()
        {
            DataCollectionRepository = DependencyResolver.Current.GetService<IDataCollectionRepository>();
        }

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var requestForm =  controllerContext.HttpContext.Request.Form;

            var model = new DataCollectionListViewModel
                            {
                                DataCollectionItems = GetCollectionDescriptionItems(requestForm)
                            };

            bindingContext.ModelMetadata.Model = model;

            return base.BindModel(controllerContext, bindingContext); 
        }

        private IEnumerable<DataCollectionItemViewModel> GetCollectionDescriptionItems(NameValueCollection formParams)
        {
            const string prefix = "IsUserSubmitted-";
            var viewModels = from o in formParams.AllKeys.Where(o => o.StartsWith(prefix))
                             let id = int.Parse(o.Substring(prefix.Length))
                             let submitted = formParams[o].Contains("true")
                             let entity = this.DataCollectionRepository.Get(id)
                             let status = submitted ? DataCollectionStatus.Submitted : DataCollectionStatus.Draft
                             where entity != null
                             select new DataCollectionItemViewModel
                                        {
                                            IsUserSubmitted = submitted,
                                            Id = id, 
                                            RecordCreationDate = entity.RecordCreationDate.ToShortDateString(),
                                            Title = entity.Title,
                                            Status = status
                                        };
            return viewModels.ToList();
        }
    }
}