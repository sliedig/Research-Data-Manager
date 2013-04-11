using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110921160100)]
    public class RenameSomeNewDataDetailsColumnsToDataStorageOnDataManagementPlanTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.NewDataDetailMaxDataSize', 'DataStorageMaxDataSize', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.NewDataDetailFileFormats', 'DataStorageFileFormats', 'COLUMN'");
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageMaxDataSize', 'NewDataDetailMaxDataSize', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageFileFormats', 'NewDataDetailFileFormats', 'COLUMN'");
        }
    }
}
