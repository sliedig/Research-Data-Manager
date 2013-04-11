using Urdms.Dmp.Controllers.Validators;

namespace Urdms.Dmp.Models
{
    public class ConfirmDataManagementPlanViewModel
    {
        public int DataManagementPlanId { get; set; }
        public string ProjectTitle { get; set; }

        [BooleanRequiredToBeTrue(ErrorMessage = "You must confirm this field before proceeding")]
        public bool HasConfirmed { get; set; }
    }
}