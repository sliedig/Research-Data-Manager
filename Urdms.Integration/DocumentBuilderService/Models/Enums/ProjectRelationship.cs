using System.ComponentModel;

namespace Urdms.DocumentBuilderService.Models.Enums
{
    public enum ProjectRelationship
    {
        None,
        [Description("Principal Investigator")]
        PrincipalInvestigator,
        Investigator,
        [Description("Support Staff")]
        SupportStaff,
        Student,
        [Description("External Researcher")]
        ExternalResearcher,
        [Description("Partner Investigator")]
        PartnerInvestigator,
        [Description("Overseas Investigator")]
        OverseasInvestigator
    }
}