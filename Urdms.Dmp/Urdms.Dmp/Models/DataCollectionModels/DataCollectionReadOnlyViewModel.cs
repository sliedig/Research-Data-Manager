using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Database.Entities;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models.DataCollectionModels
{
    public class DataCollectionReadOnlyViewModel
    {
        public DataCollectionReadOnlyViewModel()
        {
            Manager = new Party();
            UrdmsUsers = new List<UrdmsUserViewModel>();
            NonUrdmsUsers = new List<NonUrdmsUserViewModel>();
            FieldsOfResearch = new List<ClassificationBase>();
            SocioEconomicObjectives = new List<ClassificationBase>();
        }

        public int Id { get; set; }
        public int ProjectId { get; set; }

        [Display(Name = DataCollectionViewModelTitles.AwareOfEthics)]
        public bool AwareOfEthics { get; set; }

        [Display(Name = DataCollectionViewModelTitles.EthicsApprovalNumber)]
        public string EthicsApprovalNumber { get; set; }

        [Display(Name = DataCollectionViewModelTitles.ReadOnlyAvailability)]
        public DataSharingAvailability Availability { get; set; }

        [Display(Name = DataCollectionViewModelTitles.AvailabilityDate)]
        [DataType(DataType.Date)]
        public DateTime? AvailabilityDate { get; set; }

        [Display(Name = DataCollectionViewModelTitles.Title)]
        public string Title { get; set; }

        [Display(Name = DataCollectionViewModelTitles.Type)]
        public DataCollectionType Type { get; set; }

        [Display(Name = DataCollectionViewModelTitles.ResearchDataDescription)]
        public string ResearchDataDescription { get; set; }

        [Display(Name = DataCollectionViewModelTitles.StartDate)]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = DataCollectionViewModelTitles.EndDate)]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Display(Name = DataCollectionViewModelTitles.DataLicensingRights)]
        public DataLicensingType DataLicensingRights { get; set; }

        [Display(Name = DataCollectionViewModelTitles.ShareAccess)]
        public ShareAccess ShareAccess { get; set; }

        [Display(Name = DataCollectionViewModelTitles.ShareAccessDescription)]
        public string ShareAccessDescription { get; set; }

        public string Keywords { get; set; }

        public IList<ClassificationBase> FieldsOfResearch { get; set; }

        public IList<ClassificationBase> SocioEconomicObjectives { get; set; }

        public IList<UrdmsUserViewModel> UrdmsUsers { get; set; }
        public IList<NonUrdmsUserViewModel> NonUrdmsUsers { get; set; }

        [Display(Name = DataCollectionViewModelTitles.Manager)]
        public Party Manager { get; set; }

        [Display(Name = DataCollectionViewModelTitles.ProjectTitle)]
        public string ProjectTitle { get; set; }

        [Display(Name = DataCollectionViewModelTitles.DataStoreLocationName)]
        public string DataStoreLocationName { get; set; }

        [Display(Name = DataCollectionViewModelTitles.DataStoreLocationUrl)]
        public string DataStoreLocationUrl { get; set; }

        [Display(Name = DataCollectionViewModelTitles.DataStoreAdditionalDetails)]
        public string DataStoreAdditionalDetails { get; set; }

        [Display(Name = DataCollectionViewModelTitles.DataCollectionIdentifier)]
        public DataCollectionIdentifier DataCollectionIdentifier { get; set; }

        [Display(Name = DataCollectionViewModelTitles.DataCollectionIdentifierValue)]
        public string DataCollectionIdentifierValue { get; set; }
    }
}
