using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110816120800)]
    public class UpdateForeignKeys : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[Dmp]", "ProjectId", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN DmpId");

            Database.AddColumn("[Dmp]", "NewDataDetailId", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [NewDataDetail] DROP COLUMN DmpId");

            Database.AddColumn("[Dmp]", "ExistingDataDetailId", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [ExistingDataDetail] DROP COLUMN DmpId");

            Database.AddColumn("[Dmp]", "DataDocumentationId", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [DataDocumentation] DROP COLUMN DmpId");

            Database.AddColumn("[Dmp]", "EthicId", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [Ethic] DROP COLUMN DmpId");

            Database.AddColumn("[Dmp]", "IntellectualPropertyId", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [IntellectualProperty] DROP COLUMN DmpId");

            Database.AddColumn("[Dmp]", "BackupPolicyId", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [BackupPolicy] DROP COLUMN DmpId");

            Database.AddColumn("[Dmp]", "DataRetentionId", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [DataRetention] DROP COLUMN DmpId");

            Database.AddColumn("[Dmp]", "DataSharingId", DbType.Int32);
            Database.ExecuteNonQuery("ALTER TABLE [DataSharing] DROP COLUMN DmpId");
        }

        public override void Down()
        {
            Database.AddColumn("[Project]", "DmpId", DbType.Int32, ColumnProperty.NotNull);
            Database.ExecuteNonQuery("ALTER TABLE [Dmp] DROP COLUMN ProjectId");

            Database.AddColumn("[NewDataDetail]", "DmpId", DbType.Int32, ColumnProperty.NotNull);
            Database.ExecuteNonQuery("ALTER TABLE [Dmp] DROP COLUMN NewDataDetailId");

            Database.AddColumn("[ExistingDataDetail]", "DmpId", DbType.Int32, ColumnProperty.NotNull);
            Database.ExecuteNonQuery("ALTER TABLE [Dmp] DROP COLUMN ExistingDataDetailId");

            Database.AddColumn("[DataDocumentation]", "DmpId", DbType.Int32, ColumnProperty.NotNull);
            Database.ExecuteNonQuery("ALTER TABLE [Dmp] DROP COLUMN DataDocumentationId");

            Database.AddColumn("[Ethic]", "DmpId", DbType.Int32, ColumnProperty.NotNull);
            Database.ExecuteNonQuery("ALTER TABLE [Dmp] DROP COLUMN EthicId");

            Database.AddColumn("[IntellectualProperty]", "DmpId", DbType.Int32, ColumnProperty.NotNull);
            Database.ExecuteNonQuery("ALTER TABLE [Dmp] DROP COLUMN IntellectualPropertyId");

            Database.AddColumn("[BackupPolicy]", "DmpId", DbType.Int32, ColumnProperty.NotNull);
            Database.ExecuteNonQuery("ALTER TABLE [Dmp] DROP COLUMN BackupPolicyId");

            Database.AddColumn("[DataRetention]", "DmpId",DbType.Int32, ColumnProperty.NotNull);
            Database.ExecuteNonQuery("ALTER TABLE [Dmp] DROP COLUMN DataRetentionId");

            Database.AddColumn("[DataSharing]", "DmpId", DbType.Int32, ColumnProperty.NotNull);
            Database.ExecuteNonQuery("ALTER TABLE [Dmp] DROP COLUMN DataSharingId");
        }
    }
}
