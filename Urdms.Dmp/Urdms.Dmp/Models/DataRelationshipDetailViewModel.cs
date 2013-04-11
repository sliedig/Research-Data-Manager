using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public class DataRelationshipDetailViewModel
    {
        [Display(Name = DataManagementViewModelTitles.DataRelationshipDetail.RelationshipBetweenExistingAndNewData)]
        [NotRequired]
        public DataRelationship RelationshipBetweenExistingAndNewData { get; set; }
    }
}
