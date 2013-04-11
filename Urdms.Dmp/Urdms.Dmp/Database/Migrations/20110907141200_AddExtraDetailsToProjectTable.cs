using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110907141200)]
    public class AddExtraDetailsToProjectTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("Project", "ResearchInvolvesExperimentalOrganisms", DbType.Boolean);
            Database.AddColumn("Project", "ResearchInvolvesHumanParticipants", DbType.Boolean);
            Database.AddColumn("Project", "ResearchInvolvesAnimals", DbType.Boolean);
            Database.AddColumn("Project", "ResearchInvolvesSocialScienceDatasets", DbType.Boolean);
            Database.AddColumn("Project", "ResearchInvolvesGeneticManipulation", DbType.Boolean);
            Database.AddColumn("Project", "ResearchInvolvesIonisingRadiation", DbType.Boolean);
            Database.AddColumn("Project", "ResearchInvolvesDepositionOfBiologicalMaterials", DbType.Boolean);
            Database.AddColumn("Project", "HasReceivedOhsApproval", DbType.Boolean);
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN ResearchInvolvesExperimentalOrganisms");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN ResearchInvolvesHumanParticipants");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN ResearchInvolvesAnimals");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN ResearchInvolvesSocialScienceDatasets");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN ResearchInvolvesGeneticManipulation");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN ResearchInvolvesIonisingRadiation");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN ResearchInvolvesDepositionOfBiologicalMaterials");
            Database.ExecuteNonQuery("ALTER TABLE [Project] DROP COLUMN HasReceivedOhsApproval");
        }
    }
}
