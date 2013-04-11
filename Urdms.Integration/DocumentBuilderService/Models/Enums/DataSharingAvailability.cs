using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    public enum DataSharingAvailability
    {
        Never = 0,

        [Description("At end of project")]
        AtEndOfProject = 1,

        [Description("After a specified embargo period")]
        AfterASpecifiedEmbargoPeriod = 2
    }
}