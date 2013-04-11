using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815150600)]
    public class CreateDataSharingTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[DataSharing]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("Availability", DbType.Int32, ColumnProperty.NotNull),
                new Column("AvailabilityDescription", DbType.String, DbString.MaxLength),
                new Column("ShareAccess", DbType.Int32, ColumnProperty.NotNull),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[DataSharing]");
        }
    }
}
