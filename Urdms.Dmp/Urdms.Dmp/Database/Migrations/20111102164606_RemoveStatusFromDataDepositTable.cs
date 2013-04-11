using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111102164606)]
    public class RemoveStatusFromDataDepositTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataDeposit] DROP COLUMN [Status]");
        }

        public override void Down()
        {
            Database.AddColumn("[DataDeposit]", new Column("Status", DbType.Int32));
        }
    }
}
