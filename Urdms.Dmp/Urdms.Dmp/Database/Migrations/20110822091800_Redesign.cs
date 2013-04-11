using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110822091800)]
    public class Redesign : MigratorDotNetMigration
    {
        public override void Up()
        {

            Database.RemoveTable("CurtinUser");
            Database.RemoveTable("NonCurtinUser");
            Database.RemoveTable("BackupPolicy");
            Database.RemoveTable("DataDocumentation");
            Database.RemoveTable("DataRetention");
            Database.RemoveTable("DataSharing");
            Database.RemoveTable("Ethic");
            Database.RemoveTable("ExistingDataDetail");
            Database.RemoveTable("IntellectualProperty");
            Database.RemoveTable("NewDataDetail");
            Database.RemoveTable("Project");
            Database.RemoveTable("DataManagementPlan");

            Database.AddTable("[UserAccess]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("StaffId", DbType.String, 100),
                              new Column("CanWrite", DbType.Boolean),
                              new Column("Organisation", DbType.String, DbString.MaxLength),
                              new Column("UserName", DbType.String, DbString.MaxLength),
                              new Column("DataManagementPlanId", DbType.Int32));

            Database.AddTable("DataManagementPlan",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("RelationshipBetweenExistingAndNewData", DbType.Int32),
                              new Column("DataStorageType", DbType.Int32),
                              new Column("DataStorageTypeDescription", DbType.String, DbString.MaxLength),
                              new Column("ReuseByOrganisations", DbType.String, DbString.MaxLength),
                              new Column("Status", DbType.Int32),
                              new Column("ProjectPrincipalInvestigator", DbType.String, 255),
                              new Column("ProjectIdentifier", DbType.String, DbString.MaxLength),
                              new Column("ProjectName", DbType.String, DbString.MaxLength),
                              new Column("ProjectDescription", DbType.String, DbString.MaxLength),
                              new Column("NewDataDetailResearchDataDescription", DbType.String, DbString.MaxLength),
                              new Column("NewDataDetailResearchDataProcess", DbType.String, DbString.MaxLength),
                              new Column("NewDataDetailOwners", DbType.String, DbString.MaxLength),
                              new Column("NewDataDetailOwnersDescription", DbType.String, DbString.MaxLength),
                              new Column("NewDataDetailMaxDataSize", DbType.Int32),
                              new Column("NewDataDetailFileFormats", DbType.String, DbString.MaxLength),
                              new Column("NewDataDetailUpdateFrequency", DbType.Int32),
                              new Column("NewDataDetailIsVersioned", DbType.Boolean),
                              new Column("ExistingDataDetailUseExistingData", DbType.Boolean),
                              new Column("ExistingDataDetailOwner", DbType.String, DbString.MaxLength),
                              new Column("ExistingDataDetailAccessTypes", DbType.Int32),
                              new Column("ExistingDataDetailAccessTypesDescription", DbType.String, DbString.MaxLength),
                              new Column("DataDocumentationMetadataStandards", DbType.String, DbString.MaxLength),
                              new Column("DataDocumentationDirectoryStructure", DbType.String, DbString.MaxLength),
                              new Column("DataDocumentationFileNamingConvention", DbType.String, DbString.MaxLength),
                              new Column("DataDocumentationVersionControl", DbType.Int32),
                              new Column("DataDocumentationVersionControlDescription", DbType.String, DbString.MaxLength),
                              new Column("EthicRequiresClearance", DbType.Boolean),
                              new Column("EthicComments", DbType.String, DbString.MaxLength),
                              new Column("IntellectualPropertyIsCopyrighted", DbType.Boolean),
                              new Column("IntellectualPropertyCopyrightOwners", DbType.String, DbString.MaxLength),
                              new Column("BackupPolicyLocations", DbType.Int32),
                              new Column("BackupPolicyLocationsDescription", DbType.String, DbString.MaxLength),
                              new Column("BackupPolicyFrequency", DbType.Int32),
                              new Column("BackupPolicyResponsibilities", DbType.Int32),
                              new Column("BackupPolicyResponsibilitiesDescription", DbType.String, DbString.MaxLength),
                              new Column("DataRetentionPeriod", DbType.Int32),
                              new Column("DataRetentionResponsibilities", DbType.Int32),
                              new Column("DataRetentionResponsibilitiesDescription", DbType.String, DbString.MaxLength),
                              new Column("DataRetentionLocations", DbType.Int32),
                              new Column("DataRetentionLocationsDescription", DbType.String, DbString.MaxLength),
                              new Column("DataSharingAvailability", DbType.Int32),
                              new Column("DataSharingAvailabilityDescription", DbType.String, DbString.MaxLength),
                              new Column("DataSharingShareAccess", DbType.Int32));

        }

        public override void Down()
        {
            Database.AddTable("[CurtinUser]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("StaffId", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                              new Column("CanWrite", DbType.Boolean, ColumnProperty.NotNull),
                              new Column("DataManagementPlanId", DbType.Int32, ColumnProperty.NotNull));

            Database.AddTable("[NonCurtinUser]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("UserName", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                              new Column("CanWrite", DbType.Boolean, ColumnProperty.NotNull),
                              new Column("Organisation", DbType.String, DbString.MaxLength),
                              new Column("DataManagementPlanId", DbType.Int32, ColumnProperty.NotNull));

            Database.RemoveTable("[UserAccess]");
            Database.RemoveTable("DataManagementPlan");
            Database.AddTable("DataManagementPlan",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("RelationshipBetweenExistingAndNewData", DbType.Int32, ColumnProperty.NotNull),
                              new Column("DataStorageType", DbType.Int32, ColumnProperty.NotNull),
                              new Column("DataStorageTypeDescription", DbType.String, DbString.MaxLength),
                              new Column("ReuseByOrganisations", DbType.String, DbString.MaxLength),
                              new Column("Status", DbType.Int32, 4, ColumnProperty.NotNull, 0),
                              new Column("[PrincipalInvestigator]", DbType.String, ColumnProperty.NotNull),
                              new Column("ProjectId", DbType.Int32),
                              new Column("NewDataDetailId", DbType.Int32),
                              new Column("ExistingDataDetailId", DbType.Int32),
                              new Column("DataDocumentationId", DbType.Int32),
                              new Column("EthicId", DbType.Int32),
                              new Column("IntellectualPropertyId", DbType.Int32),
                              new Column("BackupPolicyId", DbType.Int32),
                              new Column("DataRetentionId", DbType.Int32),
                              new Column("DataSharingId", DbType.Int32));

            Database.AddTable("[Project]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("ProjectKey", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                              new Column("Name", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                              new Column("Description", DbType.String, DbString.MaxLength, ColumnProperty.NotNull));

            Database.AddTable("[NewDataDetail]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("ResearchDataDescription", DbType.String, DbString.MaxLength),
                              new Column("ResearchDataProcess", DbType.String, DbString.MaxLength),
                              new Column("DataOwners", DbType.Int32, ColumnProperty.NotNull),
                              new Column("DataOwnersDescription", DbType.String, DbString.MaxLength),
                              new Column("MaxDataSize", DbType.Int32, ColumnProperty.NotNull),
                              new Column("FileFormats", DbType.String, DbString.MaxLength),
                              new Column("DataUpdateFrequency", DbType.Int32, ColumnProperty.NotNull),
                              new Column("IsVersioned", DbType.Boolean, ColumnProperty.NotNull));

            Database.AddTable("[ExistingDataDetail]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("UseExistingData", DbType.Boolean, ColumnProperty.NotNull),
                              new Column("Owner", DbType.String, DbString.MaxLength),
                              new Column("AccessTypes", DbType.Int32, ColumnProperty.NotNull),
                              new Column("AccessTypesDescription", DbType.String, DbString.MaxLength));

            Database.AddTable("[DataDocumentation]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("MetadataStandards", DbType.String, DbString.MaxLength),
                              new Column("DirectoryStructure", DbType.String, DbString.MaxLength),
                              new Column("FileNamingConvention", DbType.String, DbString.MaxLength),
                              new Column("VersionControl", DbType.Int32, ColumnProperty.NotNull),
                              new Column("VersionControlDescription", DbType.String, DbString.MaxLength));

            Database.AddTable("[Ethic]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("RequiresClearance", DbType.Boolean, ColumnProperty.NotNull),
                              new Column("Comments", DbType.String, DbString.MaxLength));

            Database.AddTable("[IntellectualProperty]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("IsCopyrighted", DbType.Boolean, ColumnProperty.NotNull),
                              new Column("CopyrightOwners", DbType.String, DbString.MaxLength));

            Database.AddTable("[BackupPolicy]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("Locations", DbType.Int32, ColumnProperty.NotNull),
                              new Column("LocationsDescription", DbType.String, DbString.MaxLength),
                              new Column("Frequency", DbType.Int32, ColumnProperty.NotNull),
                              new Column("Responsibilities", DbType.Int32, ColumnProperty.NotNull),
                              new Column("ResponsibilitiesDescription", DbType.String, DbString.MaxLength));

            Database.AddTable("[DataRetention]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("RetentionPeriod", DbType.Int32, ColumnProperty.NotNull),
                              new Column("Responsibilities", DbType.Int32, ColumnProperty.NotNull),
                              new Column("ResponsibilitiesDescription", DbType.String, DbString.MaxLength),
                              new Column("Locations", DbType.Int32, ColumnProperty.NotNull),
                              new Column("LocationsDescription", DbType.String, DbString.MaxLength));

            Database.AddTable("[DataSharing]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("Availability", DbType.Int32, ColumnProperty.NotNull),
                              new Column("AvailabilityDescription", DbType.String, DbString.MaxLength),
                              new Column("ShareAccess", DbType.Int32, ColumnProperty.NotNull));
        }
    }
}
