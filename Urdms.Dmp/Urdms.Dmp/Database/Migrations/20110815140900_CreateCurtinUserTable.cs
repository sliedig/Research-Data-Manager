using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815140900)]
    public class CreateCurtinUserTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[CurtinUser]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("StaffId", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                new Column("CanWrite", DbType.Boolean, ColumnProperty.NotNull),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull));
        }

        public override void Down()
        {
            Database.RemoveTable("[CurtinUser]");
        }
    }
}
