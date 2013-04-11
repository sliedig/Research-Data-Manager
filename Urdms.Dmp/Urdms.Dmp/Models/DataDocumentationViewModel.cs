using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;

namespace Urdms.Dmp.Models
{
	public class DataDocumentationViewModel
	{
		[Display(Name = DataManagementViewModelTitles.DataDocumentation.MetadataStandards)]
		[DataType(DataType.MultilineText)]
		[NotRequired]
		public string MetadataStandards { get; set; }
	}
}
