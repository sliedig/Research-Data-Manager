using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111121164100)]
    public class RemoveNewDataDetailResearchDataProcessFromDataManagementPlanTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE DataManagementPlan DROP COLUMN NewDataDetailResearchDataProcess");
        }

        public override void Down()
        {
            Database.AddColumn("[DataManagementPlan]", new Column("NewDataDetailResearchDataProcess", DbType.String, DbString.MaxLength));
        }
    }
}
