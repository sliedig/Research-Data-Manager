using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    // If changes are made here, the Integration project also needs updating.

    public enum DataSharingAvailability
    {
        Never = 0,
        [Description("At any stage of the project")]
        AtAnyStage = 3,
        [Description("At end of project")]
        AtEndOfProject = 1,
        [Description("After a specified embargo period")]
        AfterASpecifiedEmbargoPeriod = 2
    }
}