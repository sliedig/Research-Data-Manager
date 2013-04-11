using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110912122800)]
    public class AlterProjectAccessRoleData : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("UPDATE [AccessRole] SET Name = 'Owner' WHERE Name IN ('Principal Investigator','PrincipalInvestigator')");
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("UPDATE [AccessRole] SET Name = 'PrincipalInvestigator' WHERE Name = 'Owner'");
        }
    }
}
