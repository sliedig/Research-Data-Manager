using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities.Components
{
    public class BackupPolicy
    {
        public virtual BackupLocations BackupLocations { get; set; }
        public virtual string BackupPolicyLocationsDescription { get; set; }
        public virtual DataResponsibilities BackupPolicyResponsibilities { get; set; }
        public virtual string BackupPolicyResponsibilitiesDescription { get; set; }
    }
}