using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public class NmhrcProjectFunderViewModel : ProjectFunderViewModel
    {
        [NotRequired]
        [Display(Name = "Will this research project be funded by NMHRC grant?")]
        public override bool IsFunded { get; set; }

        public override Funder Funder
        {
            get { return Funder.NMHRC; }
            
        }

        [NotRequired]
        [Display(Name = "NMHRC grant number")]
        public override string GrantNumber { get; set; }
    }
}
