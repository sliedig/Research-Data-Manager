using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111021125951)]
    public class RemoveStatusDatesFromDataCollection : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataCollection] DROP COLUMN [DateSubmitted]");
            Database.ExecuteNonQuery("ALTER TABLE [DataCollection] DROP COLUMN [DateQaApproved]");
            Database.ExecuteNonQuery("ALTER TABLE [DataCollection] DROP COLUMN [DateOrdApproved]");
            Database.ExecuteNonQuery("ALTER TABLE [DataCollection] DROP COLUMN [Status]");

            Database.AddColumn("DataCollection", "CurrentStateState", DbType.Int32);
            Database.AddColumn("DataCollection", "CurrentStateStateChangedOn", DbType.DateTime);
            
        }

        public override void Down()
        {
            Database.AddColumn("DataCollection", "DateSubmitted", DbType.DateTime);
            Database.AddColumn("DataCollection", "DateQaApproved", DbType.DateTime);
            Database.AddColumn("DataCollection", "DateOrdApproved", DbType.DateTime);
            Database.AddColumn("DataCollection", "Status", DbType.Int32);

            Database.ExecuteNonQuery("ALTER TABLE [DataCollection] DROP COLUMN [CurrentStateState]");
            Database.ExecuteNonQuery("ALTER TABLE [DataCollection] DROP COLUMN [CurrentStateStateChangedOn]");
        }
    }
}
