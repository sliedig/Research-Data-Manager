using System;
using System.Web.Mvc;
using Urdms.Dmp.Models;
using Urdms.Dmp.Models.ApprovalModels;
using Urdms.Dmp.Models.DataCollectionModels;
using Urdms.Dmp.Models.DataDeposit;

namespace Urdms.Dmp.Controllers.ModelBinders
{
    public class AppModelBinderProvider : IModelBinderProvider
    {
        /// <summary>
        /// Returns the model binder for the specified type.
        /// </summary>
        /// <returns>
        /// The model binder for the specified type.
        /// </returns>
        /// <param name="modelType">The type of the model.</param>
        public IModelBinder GetBinder(Type modelType)
        {
            if (modelType.IsEnum && modelType.IsDefined(typeof(FlagsAttribute), false))
            {
                return new FlagEnumerationModelBinder();
            }
            if(modelType == typeof(CreateProjectViewModel))
            {
                return new CreateProjectViewModelBinder();
            }
            if (modelType == typeof(DataCollectionViewModelStep2))
            {
                return new DataCollectionViewModelStep2Binder();
            }
            if (modelType == typeof(DataCollectionListViewModel))
            {
                return new DataCollectionListViewModelBinder();
            }
            if (modelType == typeof(ProjectDetailsViewModel))
            {
                return new ProjectDetailsViewModelBinder();
            }
            if (modelType == typeof(DataCollectionApprovalViewModelStep1))
            {
                return new DataCollectionApprovalViewModelStep1Binder();
            }
            if (modelType == typeof(DataCollectionApprovalViewModelStep2))
            {
                return new DataCollectionApprovalViewModelStep2Binder();
            }
            if (modelType == typeof(DataManagementPlanViewModel))
            {
                return new DataManagementPlanViewModelBinder();
            }
            if (modelType == typeof(DataDepositViewModel))
            {
                return new DataDepositViewModelBinder();
            }
            return null;
        }
    }
}