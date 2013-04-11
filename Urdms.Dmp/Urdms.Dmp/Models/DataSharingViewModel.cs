using System;
using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public class DataSharingViewModel
    {
        [Display(Name = DataManagementViewModelTitles.DataSharing.DataSharingAvailability)]
        [NotRequired]
        public DataSharingAvailability DataSharingAvailability { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataSharing.DataSharingAvailabilityDate)]
        [NotRequired]
        [DataType(DataType.Date)]
        public DateTime? DataSharingAvailabilityDate { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataSharing.ShareAccess)]
        [NotRequired]
        public ShareAccess ShareAccess { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataSharing.ReuseByOrganisations)]
        [NotRequired]
        [DataType(DataType.MultilineText)]
        public string ReuseByOrganisations { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataSharing.DataLicensingType)]
        [NotRequired]
        public DataLicensingType DataLicensingType { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataSharing.ShareAccessDescription)]
        [NotRequired]
        public string ShareAccessDescription { get; set; }
    }
}
