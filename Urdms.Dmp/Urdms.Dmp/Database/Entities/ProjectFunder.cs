using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities
{
    public class ProjectFunder
    {
        public virtual int Id { get; set; }
        public virtual Funder Funder { get; set; }
        public virtual string GrantNumber { get; set; }
    }
}