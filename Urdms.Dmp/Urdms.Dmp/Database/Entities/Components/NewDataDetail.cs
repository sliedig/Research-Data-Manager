using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities.Components
{
    public class NewDataDetail
    {
        public virtual string ResearchDataDescription { get; set; }
        public virtual DataOwners DataOwners { get; set; }
        public virtual string DataOwnersDescription { get; set; }
        public virtual DataActionFrequency DataActionFrequency { get; set; }
        public virtual bool IsVersioned { get; set; }
    }
}