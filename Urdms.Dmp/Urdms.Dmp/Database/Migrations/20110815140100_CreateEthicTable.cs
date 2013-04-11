using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815140100)]
    public class CreateEthicTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[Ethic]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("RequiresClearance", DbType.Boolean, ColumnProperty.NotNull),
                new Column("Comments", DbType.String, DbString.MaxLength),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[Ethic]");
        }
    }
}
