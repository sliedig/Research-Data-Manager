using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public class NewDataDetailViewModel
    {
        [Display(Name = DataManagementViewModelTitles.NewDataDetail.ResearchDataDescription)]
        [DataType(DataType.MultilineText)]
        [Required]
        public string ResearchDataDescription { get; set; }

        [Display(Name = DataManagementViewModelTitles.NewDataDetail.DataOwners)]
        [NotRequired]
        public DataOwners DataOwners { get; set; }

        [Display(Name = DataManagementViewModelTitles.NewDataDetail.DataOwnersDescription)]
        [DataType(DataType.MultilineText)]
        [NotRequired]
        public string DataOwnersDescription { get; set; }

        [Display(Name = DataManagementViewModelTitles.NewDataDetail.DataActionFrequency)]
        [NotRequired]
        public DataActionFrequency DataActionFrequency { get; set; }

        [Display(Name = DataManagementViewModelTitles.NewDataDetail.IsVersioned)]
        [NotRequired]
        public bool IsVersioned { get; set; }
    }
}
