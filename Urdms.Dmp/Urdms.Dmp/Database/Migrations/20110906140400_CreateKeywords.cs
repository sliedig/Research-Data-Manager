using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110906140400)]
    public class CreateKeywords : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[Keyword]",
             new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
             new Column("Value", DbType.String, 150, ColumnProperty.NotNull),
             new Column("ProjectId", DbType.Int32));

            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [Keywords]");
        }

        public override void Down()
        {
            Database.RemoveTable("[Keyword]");
            Database.AddColumn("[Project]", "[Keywords]", DbType.String, DbString.MaxLength);
        }
    }
}
