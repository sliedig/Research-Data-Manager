using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110921112200)]
    public class AddDmpStorageTypes : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataStorageType");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataStorageTypeDescription");

            Database.AddColumn("DataManagementPlan", "DataStorageCurtinTypes", DbType.Int32);
            Database.AddColumn("DataManagementPlan", "DataStorageCurtinOtherTypeDescription", DbType.String, DbString.MaxLength);
            Database.AddColumn("DataManagementPlan", "DataStorageExternalTypes", DbType.Int32);
            Database.AddColumn("DataManagementPlan", "DataStorageExternalOtherTypeDescription", DbType.String, DbString.MaxLength);
            Database.AddColumn("DataManagementPlan", "DataStoragePersonalTypes", DbType.Int32);
            Database.AddColumn("DataManagementPlan", "DataStoragePersonalOtherTypeDescription", DbType.String, DbString.MaxLength);
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataStorageCurtinTypes");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataStorageCurtinOtherTypeDescription");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataStorageExternalTypes");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataStorageExternalOtherTypeDescription");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataStoragePersonalTypes");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN DataStoragePersonalOtherTypeDescription");

            Database.AddColumn("DataManagementPlan","DataStorageType",DbType.Int32);
            Database.AddColumn("DataManagementPlan", "DataStorageTypeDescription", DbType.String, DbString.MaxLength);

        }
    }
}
