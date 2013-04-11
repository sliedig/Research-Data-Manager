using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111117163800)]
    public class AddFieldToDataDepositTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[DataDeposit]", new Column("ShareAccessDescription", DbType.String, DbString.MaxLength));
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataDeposit] DROP COLUMN [ShareAccessDescription]");
        }
    }
}
