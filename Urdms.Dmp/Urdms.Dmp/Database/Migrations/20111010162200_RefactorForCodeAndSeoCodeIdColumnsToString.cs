using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111010162200)]
    public class RefactorForCodeAndSeoCodeIdColumnsToString : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionFieldOfResearch] DROP COLUMN [FieldOfResearchId]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionSocioEconomicObjective] DROP COLUMN [SocioEconomicObjectiveId]");
            Database.AddColumn("[CollectionDescriptionFieldOfResearch]", new Column("FieldOfResearchId", DbType.String, 255));
            Database.AddColumn("[CollectionDescriptionSocioEconomicObjective]", new Column("SocioEconomicObjectiveId", DbType.String, 255));
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionFieldOfResearch] DROP COLUMN [FieldOfResearchId]");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionSocioEconomicObjective] DROP COLUMN [SocioEconomicObjectiveId]");
            Database.AddColumn("[CollectionDescriptionFieldOfResearch]", new Column("FieldOfResearchId", DbType.Int32));
            Database.AddColumn("[CollectionDescriptionSocioEconomicObjective]", new Column("SocioEconomicObjectiveId", DbType.Int32));
        }
    }
}
