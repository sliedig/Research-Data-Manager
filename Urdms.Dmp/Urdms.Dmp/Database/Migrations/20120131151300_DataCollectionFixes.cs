using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20120131151300)]
    public class DataCollectionFixes : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE DataCollection ALTER COLUMN DataStoreAdditionalDetails NVARCHAR(MAX)");
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE DataCollection ALTER COLUMN DataStoreAdditionalDetails NVARCHAR(255)");
        }
    }
}
