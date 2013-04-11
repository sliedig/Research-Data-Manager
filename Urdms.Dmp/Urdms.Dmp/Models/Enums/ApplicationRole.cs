using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    public enum ApplicationRole
    {
        [Description("QA-Approver")]
        QaApprover,
        [Description("Secondary-Approver")]
        SecondaryApprover
    }
}