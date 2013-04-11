using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    public enum ProvisioningStatus
    {
        NotStarted = 0,
        [Description("In Progress")]
        Pending = 1,
        Provisioned = 2,
        Error = 3,
        TimeOut = 4
    }

}