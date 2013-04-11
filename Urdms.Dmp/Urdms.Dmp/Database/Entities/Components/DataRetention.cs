using Urdms.Dmp.Models.Enums;

namespace Urdms.Dmp.Database.Entities.Components
{
    public class DataRetention
    {
        public virtual DataRetentionPeriod DataRetentionPeriod { get; set; }
        public virtual DataResponsibilities DataRetentionResponsibilities { get; set; }
        public virtual string DataRetentionResponsibilitiesDescription { get; set; }
        public virtual DataRetentionLocations DataRetentionLocations { get; set; }
        public virtual string DataRetentionLocationsDescription { get; set; }
        public virtual bool DepositToRepository { get; set; }
    }
}