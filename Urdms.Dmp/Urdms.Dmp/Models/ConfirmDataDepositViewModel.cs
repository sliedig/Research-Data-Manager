using Urdms.Dmp.Controllers.Validators;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Models
{
    public class ConfirmDataDepositViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public SourceProjectType ProjectType { get; set; }

        [BooleanRequiredToBeTrue(ErrorMessage = "You must confirm this field before proceeding")]
        public bool HasConfirmed { get; set; }
    }
}