using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public class DataRetentionViewModel
    {
        [Display(Name = DataManagementViewModelTitles.DataRetention.DataRetentionPeriod)]
        [NotRequired]
        public DataRetentionPeriod DataRetentionPeriod { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataRetention.DataRetentionResponsibilities)]
        [NotRequired]
        public DataResponsibilities DataRetentionResponsibilities { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataRetention.DataRetentionResponsibilitiesDescription)]
        [NotRequired]
        [DataType(DataType.MultilineText)]
        public string DataRetentionResponsibilitiesDescription { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataRetention.DataRetentionLocations)]
        [NotRequired]
        public DataRetentionLocations DataRetentionLocations { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataRetention.DataRetentionLocationsDescription)]
        [NotRequired]
        public string DataRetentionLocationsDescription { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataRetention.DepositToRepository)]
        public bool DepositToRepository { get; set; }
    }
}
