using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;

namespace Urdms.Dmp.Models
{
    public class DataOrganisationViewModel
    {
        [Display(Name = DataManagementViewModelTitles.DataOrganisation.DirectoryStructure)]
        [DataType(DataType.MultilineText)]
        [NotRequired]
        public string DirectoryStructure { get; set; }
    }
}
