using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Controllers.Validators;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public class DataStorageViewModel
    {
        [Display(Name = DataManagementViewModelTitles.DataStorage.InstitutionStorageTypes)]
        [NotRequired]
        public InstitutionalStorageTypes InstitutionalStorageTypes { get; set; }

        [OtherEnumRequired("InstitutionalStorageTypes", InstitutionalStorageTypes.Other)]
        [DataType(DataType.Text)]
        public string InstitutionalOtherTypeDescription { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataStorage.ExternalStorageTypes)]
        [NotRequired]
        public ExternalStorageTypes ExternalStorageTypes { get; set; }

        [OtherEnumRequired("ExternalStorageTypes", ExternalStorageTypes.Other)]
        [DataType(DataType.Text)]
        public string ExternalOtherTypeDescription { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataStorage.PersonalStorageTypes)]
        [NotRequired]
        public PersonalStorageTypes PersonalStorageTypes { get; set; }

        [OtherEnumRequired("PersonalStorageTypes",PersonalStorageTypes.Other)]
        [DataType(DataType.Text)]
        public string PersonalOtherTypeDescription { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataStorage.MaxDataSize)]
        [NotRequired]
        public MaxDataSize MaxDataSize { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataStorage.FileFormats)]
        [DataType(DataType.MultilineText)]
        [NotRequired]
        public string FileFormats { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataStorage.VersionControl)]
        [NotRequired]
        public VersionControl VersionControl { get; set; }

        [Display(Name = DataManagementViewModelTitles.DataStorage.VersionControlDescription)]
        [DataType(DataType.MultilineText)]
        [NotRequired]
        public string VersionControlDescription { get; set; }
    }
}
