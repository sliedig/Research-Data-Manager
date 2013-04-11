using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110906163800)]
    public class UpdateProjectSeoAndProjectForCodes : MigratorDotNetMigration
    {

        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [ProjectSocioEconomicObjective] DROP COLUMN [SocioEconomicObjectiveId]");
            Database.ExecuteNonQuery("ALTER TABLE [ProjectFieldOfResearch] DROP COLUMN [FieldOfResearchId]");

            Database.AddColumn("[ProjectSocioEconomicObjective]", new Column("SocioEconomicObjectiveId", DbType.String, 255));
            Database.AddColumn("[ProjectFieldOfResearch]", new Column("FieldOfResearchId", DbType.String, 255));

        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [ProjectSocioEconomicObjective] DROP COLUMN [SocioEconomicObjectiveId]");
            Database.ExecuteNonQuery("ALTER TABLE [ProjectFieldOfResearch] DROP COLUMN [FieldOfResearchId]");

            Database.AddColumn("[ProjectSocioEconomicObjective]", new Column("SocioEconomicObjectiveId", DbType.Int32));
            Database.AddColumn("[ProjectFieldOfResearch]", new Column("FieldOfResearchId", DbType.Int32));
        }
    }
}
