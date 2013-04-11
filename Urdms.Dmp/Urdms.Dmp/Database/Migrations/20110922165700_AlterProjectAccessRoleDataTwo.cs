using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110922165700)]
    public class AlterProjectAccessRoleDataTwo : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("UPDATE [AccessRole] SET Name = 'Viewer' WHERE Name = 'Researcher'");
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("UPDATE [AccessRole] SET Name = 'Researcher' WHERE Name = 'Viewer'");
        }
    }
}
