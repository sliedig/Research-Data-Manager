using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111102154052)]
    public class AddDataDepositTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[DataDeposit]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("ResearchDataDescription", DbType.String, DbString.MaxLength),
                              new Column("MaxDataSize", DbType.Int32),
                              new Column("Availability", DbType.Int32),
                              new Column("AvailabilityDate", DbType.DateTime),
                              new Column("ShareAccess", DbType.Int32),
                              new Column("LicensingArrangement", DbType.Int32),
                              new Column("Status", DbType.Int32),
                              new Column("CreationDate", DbType.DateTime)
                              );

            Database.AddColumn("[Project]", new Column("DataDepositId", DbType.Int32));
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [DataDepositId]");

            Database.RemoveTable("[DataDeposit]");
        }
    }
}
