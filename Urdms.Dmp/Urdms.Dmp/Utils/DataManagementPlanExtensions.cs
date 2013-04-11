using Urdms.Dmp.Database.Entities;

namespace Urdms.Dmp.Utils
{
    public static class DataManagementPlanExtensions
    {
        public static DataManagementPlan Clone(this DataManagementPlan srcDmp)
        {
            return new DataManagementPlan
            {
                Id = 0,
                BackupPolicy = srcDmp.BackupPolicy,
                DataDocumentation = srcDmp.DataDocumentation,
                DataRelationshipDetail = srcDmp.DataRelationshipDetail,
                DataRetention = srcDmp.DataRetention,
                DataSharing = srcDmp.DataSharing,
                DataStorage = srcDmp.DataStorage,
                Ethic = srcDmp.Ethic,
                ExistingDataDetail = srcDmp.ExistingDataDetail,
                Confidentiality = srcDmp.Confidentiality,
                NewDataDetail = srcDmp.NewDataDetail
            };
        }
    }
}
