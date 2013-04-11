using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815144200)]
    public class CreateDataRetentionTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[DataRetention]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("RetentionPeriod", DbType.Int32, ColumnProperty.NotNull),
                new Column("Responsibilities", DbType.Int32, ColumnProperty.NotNull),
                new Column("ResponsibilitiesDescription", DbType.String, DbString.MaxLength),
                new Column("Locations", DbType.Int32, ColumnProperty.NotNull),
                new Column("LocationsDescription", DbType.String, DbString.MaxLength),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[DataRetention]");
        }
    }
}
