using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815141700)]
    public class CreateNonCurtinUserTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[NonCurtinUser]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("UserName", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                new Column("CanWrite", DbType.Boolean, ColumnProperty.NotNull),
                new Column("Organisation", DbType.String, DbString.MaxLength),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[NonCurtinUser]");
        }
    }
}
