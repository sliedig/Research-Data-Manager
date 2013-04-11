using System.ComponentModel;

namespace Urdms.Dmp.Models.Enums
{
    public enum SourceProjectType
    {
        None = 0,

        ARC = 1,
        NMHRC = 2,

        [Description("Research-Initiated Project")]
        DMP = 3,

        [Description("Data Deposit Project")]
        DEPOSIT = 4
    }
}

