using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815124300)]
    public class CreateDmpTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[Dmp]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("RelationshipBetweenExistingAndNewData", DbType.Int32, ColumnProperty.NotNull),
                new Column("DataStorageType", DbType.Int32, ColumnProperty.NotNull),
                new Column("DataStorageTypeDescription", DbType.String, DbString.MaxLength),
                new Column("ReuseByOrganisations", DbType.String, DbString.MaxLength),
                new Column("CreationDate", DbType.DateTime, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[Dmp]");
        }
    }
}
