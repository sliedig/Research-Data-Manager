using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111103101440)]
    public class RemoveStep2FieldsFromProject : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [ResearchInvolvesExperimentalOrganisms]");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [HasReceivedOhsApproval]");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [ResearchInvolvesDepositionOfBiologicalMaterials]");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [ResearchInvolvesIonisingRadiation]");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [ResearchInvolvesGeneticManipulation]");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [ResearchInvolvesSocialScienceDatasets]");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [ResearchInvolvesAnimals]");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN [ResearchInvolvesHumanParticipants]");
        }

        public override void Down()
        {
            Database.AddColumn("[Project]", new Column("ResearchInvolvesExperimentalOrganisms", DbType.Boolean));
            Database.AddColumn("[Project]", new Column("HasReceivedOhsApproval", DbType.Boolean));
            Database.AddColumn("[Project]", new Column("ResearchInvolvesDepositionOfBiologicalMaterials", DbType.Boolean));
            Database.AddColumn("[Project]", new Column("ResearchInvolvesIonisingRadiation", DbType.Boolean));
            Database.AddColumn("[Project]", new Column("ResearchInvolvesGeneticManipulation", DbType.Boolean));
            Database.AddColumn("[Project]", new Column("ResearchInvolvesSocialScienceDatasets", DbType.Boolean));
            Database.AddColumn("[Project]", new Column("ResearchInvolvesAnimals", DbType.Boolean));
            Database.AddColumn("[Project]", new Column("ResearchInvolvesHumanParticipants", DbType.Boolean));
        }
    }
}
