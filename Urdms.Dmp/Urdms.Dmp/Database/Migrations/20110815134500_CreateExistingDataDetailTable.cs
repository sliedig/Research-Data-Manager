using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815134500)]
    public class CreateExistingDataDetailTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[ExistingDataDetail]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("UseExistingData", DbType.Boolean, ColumnProperty.NotNull),
                new Column("Owner", DbType.String, DbString.MaxLength),
                new Column("AccessTypes", DbType.Int32, ColumnProperty.NotNull),
                new Column("AccessTypesDescription", DbType.String, DbString.MaxLength),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[ExistingDataDetail]");
        }
    }
}
