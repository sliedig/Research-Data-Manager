
using System.ComponentModel;

namespace Urdms.Approvals.VivoPublisher.Database.Enums
{   
    /// <summary>
    /// The values should not be changed once in use.
    /// </summary>
    public enum DataCollectionRelationshipType
    {
        None = 0,
        Manager = 1,
        [Description("Associated Researcher")]
        AssociatedResearcher = 2,
        Collector = 3
    }
}