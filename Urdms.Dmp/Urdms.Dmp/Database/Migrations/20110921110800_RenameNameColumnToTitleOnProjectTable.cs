using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110921110800)]
    public class RenameNameColumnToTitleOnProjectTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("sp_rename 'Project.Name', 'Title', 'COLUMN'");
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("sp_rename 'Project.Title', 'Name', 'COLUMN'");
        }
    }
}
