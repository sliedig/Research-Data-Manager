using Urdms.Approvals.VivoPublisher.Database.Enums;

namespace Urdms.Approvals.VivoPublisher.Database.Entities
{
    public class DataCollectionParty
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Organisation { get; set; }
        public string FullName { get; set; }
        public DataCollectionRelationshipType Relationship { get; set; }
    }
}