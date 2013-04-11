using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111130140000)]
    public class MoreDataCollectionChanges : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("DataCollection", "IsFirstCollection", DbType.Boolean);
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE DataCollection DROP COLUMN IsFirstCollection");
        }
    }
}
