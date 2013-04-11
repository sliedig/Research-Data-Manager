using System;
using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    [Flags]
    public enum DataResponsibilities
    {
        None = 0,
        [Description("Principal Investigator")]
        PrincipalInvestigator = 1,
        [Description("Member of the research project")]
        Researcher = 2,
        Other = 4
    }
}