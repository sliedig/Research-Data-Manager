using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
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

