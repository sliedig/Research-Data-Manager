using Urdms.Dmp.Database.Entities.Interfaces;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities
{
    public class DataCollectionParty : IUserManagementParty
    {
        public virtual int Id { get; set; }
        public virtual DataCollection DataCollection { get; set; }
        public virtual Party Party { get; set; }
        public virtual DataCollectionRelationshipType Relationship { get; set; }
    }
}