using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111118140700)]
    public class AddTimerTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("FormTimer",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKey),
                              new Column("Step", DbType.Int32, ColumnProperty.PrimaryKey),
                              new Column("UserId", DbType.String, DbString.MaxLength),
                              new Column("StartTime", DbType.DateTime),
                              new Column("EndTime", DbType.DateTime)
                              );
        }

        public override void Down()
        {
            Database.RemoveTable("FormTimer");
        }
    }
}
