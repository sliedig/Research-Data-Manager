using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110816115000)]
    public class RemoveEmailFromCurtinUserTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CurtinUser] DROP COLUMN Email");
        }

        public override void Down()
        {
            Database.AddColumn("[CurtinUser]", "Email", DbType.String, DbString.MaxLength, ColumnProperty.NotNull);
        }
    }
}
