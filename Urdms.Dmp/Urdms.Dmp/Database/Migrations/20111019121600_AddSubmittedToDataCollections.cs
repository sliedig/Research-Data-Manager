using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111019121600)]
    public class AddSubmittedToDataCollections : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("DataCollection", "DateSubmitted", DbType.DateTime);
            Database.AddColumn("DataCollection", "DateQaApproved", DbType.DateTime);
            Database.AddColumn("DataCollection", "DateOrdApproved", DbType.DateTime);
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataCollection] DROP COLUMN [DateSubmitted]");
            Database.ExecuteNonQuery("ALTER TABLE [DataCollection] DROP COLUMN [DateQaApproved]");
            Database.ExecuteNonQuery("ALTER TABLE [DataCollection] DROP COLUMN [DateOrdApproved]");
        }
    }
}
