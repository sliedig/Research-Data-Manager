using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111003084500)]
    public class RefactorNewDataDetailOwnersFromStringToInt : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN [NewDataDetailOwners]");
            Database.AddColumn("[DataManagementPlan]", new Column("NewDataDetailOwners", DbType.Int32));
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN [NewDataDetailOwners]");
            Database.AddColumn("[DataManagementPlan]", new Column("NewDataDetailOwners", DbType.String, DbString.MaxLength));
        }
    }
}
