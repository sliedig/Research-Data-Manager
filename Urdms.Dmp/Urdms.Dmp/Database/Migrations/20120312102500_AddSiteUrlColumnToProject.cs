using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20120312102500)]
    public class AddSiteUrlColumnToProject : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[Project]", new Column("SiteUrl", DbType.String, 255, ColumnProperty.Null));
        }

        public override void Down()
        {
            Database.RemoveColumn("[Project]", "SiteUrl");

        }
    }
}
