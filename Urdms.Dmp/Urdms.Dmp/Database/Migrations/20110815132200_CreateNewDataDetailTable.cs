using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815132200)]
    public class CreateNewDataDetailTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[NewDataDetail]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("ResearchDataDescription", DbType.String, DbString.MaxLength),
                new Column("ResearchDataProcess", DbType.String, DbString.MaxLength),
                new Column("DataOwners", DbType.Int32, ColumnProperty.NotNull),
                new Column("DataOwnersDescription", DbType.String, DbString.MaxLength),
                new Column("MaxDataSize", DbType.Int32, ColumnProperty.NotNull),
                new Column("FileFormats", DbType.String, DbString.MaxLength),
                new Column("DataUpdateFrequency", DbType.Int32, ColumnProperty.NotNull),
                new Column("IsVersioned", DbType.Boolean, ColumnProperty.NotNull),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[NewDataDetail]");
        }
    }
}
