using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public class ExistingDataDetailViewModel
    {
        [Display(Name = DataManagementViewModelTitles.ExistingDataDetail.UseExistingData)]
        [NotRequired]
        public bool UseExistingData { get; set; }

        [Display(Name = DataManagementViewModelTitles.ExistingDataDetail.Owner)]
        [NotRequired]
        [DataType(DataType.MultilineText)]
        public string ExistingDataOwner { get; set; }

        [Display(Name = DataManagementViewModelTitles.ExistingDataDetail.AccessTypes)]
        [NotRequired]
        public ExistingDataAccessTypes ExistingDataAccessTypes { get; set; }

        [Display(Name = DataManagementViewModelTitles.ExistingDataDetail.AccessTypesDescription)]
        [NotRequired]
        [DataType(DataType.MultilineText)]
        public string AccessTypesDescription { get; set; }
    }
}
