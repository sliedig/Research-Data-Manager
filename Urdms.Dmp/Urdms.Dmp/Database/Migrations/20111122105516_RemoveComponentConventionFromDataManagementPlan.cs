using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111122105516)]
    public class RemoveComponentConventionFromDataManagementPlan : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.BackupPolicyLocations', 'BackupLocations', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ConfidentialityIsSensitive', 'IsSensitive', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataDocumentationMetadataStandards', 'MetadataStandards', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataOrganisationDirectoryStructure', 'DirectoryStructure', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataRelationshipDetailRelationshipBetweenExistingAndNewData', 'RelationshipBetweenExistingAndNewData', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataRetentionDepositToRepository', 'DepositToRepository', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataSharingShareAccess', 'ShareAccess', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataSharingShareAccessDescription', 'ShareAccessDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataSharingLicensingArrangement', 'DataLicensingType', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataSharingReuseByOrganisations', 'ReuseByOrganisations', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageCurtinTypes', 'InstitutionalStorageTypes', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageCurtinOtherTypeDescription', 'InstitutionalOtherTypeDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageExternalTypes', 'ExternalStorageTypes', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageExternalOtherTypeDescription', 'ExternalOtherTypeDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStoragePersonalTypes', 'PersonalStorageTypes', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStoragePersonalOtherTypeDescription', 'PersonalOtherTypeDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageMaxDataSize', 'MaxDataSize', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageFileFormats', 'FileFormats', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageVersionControl', 'VersionControl', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageVersionControlDescription', 'VersionControlDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ExistingDataDetailUseExistingData', 'UseExistingData', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ExistingDataDetailOwner', 'ExistingDataOwner', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ExistingDataDetailAccessTypes', 'ExistingDataAccessTypes', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ExistingDataDetailAccessTypesDescription', 'AccessTypesDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.NewDataDetailResearchDataDescription', 'ResearchDataDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.NewDataDetailOwners', 'DataOwners', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.NewDataDetailOwnersDescription', 'DataOwnersDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.NewDataDetailUpdateFrequency', 'DataActionFrequency', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.NewDataDetailIsVersioned', 'IsVersioned', 'COLUMN'");

            Database.ExecuteNonQuery("sp_rename 'DataCollection.CurrentStateState', 'State', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataCollection.CurrentStateStateChangedOn', 'StateChangedOn', 'COLUMN'");
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("sp_rename 'DataCollection.State', 'CurrentStateState', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataCollection.StateChangedOn', 'CurrentStateStateChangedOn', 'COLUMN'");

            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.BackupLocations', 'BackupPolicyLocations', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.IsSensitive', 'ConfidentialityIsSensitive', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.MetadataStandards', 'DataDocumentationMetadataStandards', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DirectoryStructure', 'DataOrganisationDirectoryStructure', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.RelationshipBetweenExistingAndNewData', 'DataRelationshipDetailRelationshipBetweenExistingAndNewData', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DepositToRepository', 'DataRetentionDepositToRepository', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ShareAccess', 'DataSharingShareAccess', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ShareAccessDescription', 'DataSharingShareAccessDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataLicensingType', 'DataSharingLicensingArrangement', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ReuseByOrganisations', 'DataSharingReuseByOrganisations', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.InstitutionalStorageTypes', 'DataStorageCurtinTypes', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.InstitutionalOtherTypeDescription', 'DataStorageCurtinOtherTypeDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ExternalStorageTypes', 'DataStorageExternalTypes', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ExternalOtherTypeDescription', 'DataStorageExternalOtherTypeDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.PersonalStorageTypes', 'DataStoragePersonalTypes', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.PersonalOtherTypeDescription', 'DataStoragePersonalOtherTypeDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.MaxDataSize', 'DataStorageMaxDataSize', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.FileFormats', 'DataStorageFileFormats', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.VersionControl', 'DataStorageVersionControl', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.VersionControlDescription', 'DataStorageVersionControlDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.UseExistingData', 'ExistingDataDetailUseExistingData', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ExistingDataOwner', 'ExistingDataDetailOwner', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ExistingDataAccessTypes', 'ExistingDataDetailAccessTypes', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.AccessTypesDescription', 'ExistingDataDetailAccessTypesDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.ResearchDataDescription', 'NewDataDetailResearchDataDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataOwners', 'NewDataDetailOwners', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataOwnersDescription', 'NewDataDetailOwnersDescription', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataActionFrequency', 'NewDataDetailUpdateFrequency', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.IsVersioned', 'NewDataDetailIsVersioned', 'COLUMN'");
        }
    }
}
