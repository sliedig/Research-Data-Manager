using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    public enum ProjectStatus
    {
        Draft,
        [Description("Pending Approval")]
        PendingApproval, 
        Approved, 
        [Description("In Progress")]
        InProgress,
        Completed
    }
}

