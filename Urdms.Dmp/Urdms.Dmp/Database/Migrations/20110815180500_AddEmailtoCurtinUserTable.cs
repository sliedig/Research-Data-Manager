using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815180500)]
    public class AddEmailtoCurtinUserTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[CurtinUser]", "Email", DbType.String, DbString.MaxLength, ColumnProperty.NotNull);
        }

        public override void Down()
        {
            Database.RemoveColumn("[CurtinUser]", "Email");
        }
    }
}
