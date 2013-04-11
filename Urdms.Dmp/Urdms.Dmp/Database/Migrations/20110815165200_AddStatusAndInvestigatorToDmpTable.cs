using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110815165200)]
    public class AddStatusToDmpTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("Dmp", "Status", DbType.Int32, 4, ColumnProperty.NotNull, 0);
            Database.AddColumn("[Dmp]", "[PrincipalInvestigator]", DbType.String, ColumnProperty.NotNull);
        }

        public override void Down()
        {
            Database.RemoveConstraint("Dmp", "DF_DataManagementPlan_Status");
            Database.RemoveColumn("[Dmp]", "[Status]");
            Database.RemoveColumn("[Dmp]", "[PrincipalInvestigator]");
        }
    }
}
