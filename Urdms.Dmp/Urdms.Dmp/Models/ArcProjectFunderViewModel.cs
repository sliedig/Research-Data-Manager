using System.ComponentModel.DataAnnotations;
using Urdms.Dmp.Web.FlowForms.ValidationAttributes;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public class ArcProjectFunderViewModel : ProjectFunderViewModel
    {
        [NotRequired]
        [Display(Name =  "Will this research project be funded by ARC grant?")]
        public override bool IsFunded { get; set; }

        public override Funder Funder 
        {
            get { return Funder.ARC; }
        }

        [NotRequired]
        [Display(Name = "ARC grant number")]
        public override string GrantNumber { get; set; }
    }
}
