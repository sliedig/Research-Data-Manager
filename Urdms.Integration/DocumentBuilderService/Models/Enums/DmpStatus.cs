using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    public enum DmpStatus
    {
        [Description("Not started")]
        NotStarted,
        [Description("In Progress")]
        InProgress,
        Submitted,
        Approved,
        Provisioned
    }

}