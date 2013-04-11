using System;
using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models.DataCollectionModels
{
    // Note: Updates to this class may affect the read-only view DataCollectionReadOnlyViewModel also
    public class DataCollectionViewModelStep1
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.AwareOfEthics)]
        public bool AwareOfEthics { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.AvailabilityDate)]
        [DataType(DataType.Date)]
        public DateTime? AvailabilityDate { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.ProjectTitle)]
        public string ProjectTitle { get; set; }

        [Required]
        [Display(Name = DataCollectionViewModelTitles.Title)]
        public string Title { get; set; }

        public bool IsFirstCollection { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.Type)]
        public DataCollectionType Type { get; set; }

        [Display(Name = DataCollectionViewModelTitles.ResearchDataDescription)]
        [DataType(DataType.MultilineText)]
        [Required]
        public string ResearchDataDescription { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.StartDate)]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.EndDate)]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.DataLicensingRights)]
        public DataLicensingType DataLicensingRights { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.ShareAccess)]
        public ShareAccess ShareAccess { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.ShareAccessDescription)]
        public string ShareAccessDescription { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.EthicsApprovalNumber)]
        public string EthicsApprovalNumber { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.DataStoreLocationName)]
        public string DataStoreLocationName { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.DataStoreLocationUrl)]
        public string DataStoreLocationUrl { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.DataStoreAdditionalDetails)]
        [DataType(DataType.MultilineText)]
        public string DataStoreAdditionalDetails { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.DataCollectionIdentifier)]
        public DataCollectionIdentifier DataCollectionIdentifier { get; set; }

        [NotRequired]
        [Display(Name = DataCollectionViewModelTitles.DataCollectionIdentifierValue)]
        public string DataCollectionIdentifierValue { get; set; }
    }
}
