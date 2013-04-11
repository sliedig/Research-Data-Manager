using Urdms.Dmp.Database.Entities.Interfaces;
using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities
{
    public class ProjectParty : IUserManagementParty
    {
        public virtual int Id { get; set; }
        public virtual Project Project { get; set; }
        public virtual Party Party { get; set; }
        public virtual ProjectRelationship Relationship { get; set; }
        public virtual AccessRole Role { get; set; }
    }
}