using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
	public class BackupPolicyViewModel
	{
        [Display(Name = DataManagementViewModelTitles.BackupPolicy.BackupLocations)]
        [NotRequired]
		public BackupLocations BackupLocations { get; set; }

        [Display(Name = DataManagementViewModelTitles.BackupPolicy.BackupPolicyLocationsDescription)]
        [NotRequired]
        public string BackupPolicyLocationsDescription { get; set; }

        [Display(Name = DataManagementViewModelTitles.BackupPolicy.BackupPolicyResponsibilities)]
        [NotRequired]
		public DataResponsibilities BackupPolicyResponsibilities { get; set; }

        [Display(Name = DataManagementViewModelTitles.BackupPolicy.BackupPolicyResponsibilitiesDescription)]
        [NotRequired]
        [DataType(DataType.MultilineText)]
		public string BackupPolicyResponsibilitiesDescription { get; set; }
	}
}
