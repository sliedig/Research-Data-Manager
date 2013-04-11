using System.ComponentModel;
using Curtin.Framework.Common.Attributes;

namespace Urdms.Dmp.Web.Pages {
    [NoCoverage]
    public enum Classification {
        [Description("Campus Life")]
        CampusLife,
        [Description("Community Relations")]
        CommunityRelations,
        [Description("Facilities")]
        Facilities,
        [Description("Finance")]
        Finance,
        [Description("Human Resources")]
        HumanResources,
        [Description("Information Management")]
        InformationManagement,
        [Description("Legal")]
        Legal,
        [Description("Library Services")]
        LibraryServices,
        [Description("Research")]
        Research,
        [Description("Strategic Management")]
        StrategicManagement,
        [Description("Students")]
        Students,
        [Description("Teaching & Learning")]
        TeachingAndLearning,
    }
}
