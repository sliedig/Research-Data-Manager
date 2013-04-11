using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;

namespace Urdms.Dmp.Models
{
	public class EthicViewModel
	{
        [Display(Name = DataManagementViewModelTitles.Ethic.EthicRequiresClearance)]
        [NotRequired]
		public bool EthicRequiresClearance { get; set; }

        [Display(Name = DataManagementViewModelTitles.Ethic.EthicComments)]
        [NotRequired]
        [DataType(DataType.MultilineText)]
		public string EthicComments { get; set; }
	}
}
