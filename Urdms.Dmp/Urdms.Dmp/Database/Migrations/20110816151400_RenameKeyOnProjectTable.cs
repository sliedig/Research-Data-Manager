using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110816151400)]
    public class RenameKeyOnProjectTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[Project]", "ProjectKey", DbType.String, DbString.MaxLength);
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [Key]");
        }

        public override void Down()
        {
            Database.AddColumn("[Project]", "[Key]", DbType.String, DbString.MaxLength);
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN ProjectKey");
        }
    }
}
