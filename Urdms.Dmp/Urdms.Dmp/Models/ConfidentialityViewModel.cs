using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;

namespace Urdms.Dmp.Models
{
	public class ConfidentialityViewModel
	{

        [Display(Name = DataManagementViewModelTitles.Confidentiality.IsSensitive)]
        [NotRequired]
        public bool IsSensitive { get; set; }

        [Display(Name = DataManagementViewModelTitles.Confidentiality.ConfidentialityComments)]
        [NotRequired]
        [DataType(DataType.MultilineText)]
		public string ConfidentialityComments { get; set; }
	}
}
