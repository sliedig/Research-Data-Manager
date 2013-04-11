using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110921152400)]
    public class CreatePrivacyRequirements : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN IntellectualPropertyIsCopyrighted");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN IntellectualPropertyCopyrightOwners");

            Database.AddColumn("DataManagementPlan", "ConfidentialityIsSensitive", DbType.Boolean);
            Database.AddColumn("DataManagementPlan", "ConfidentialityComments", DbType.String, DbString.MaxLength);
            
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN ConfidentialityIsSensitive");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN ConfidentialityComments");

            Database.AddColumn("DataManagementPlan", "IntellectualPropertyIsCopyrighted", DbType.Boolean);
            Database.AddColumn("DataManagementPlan", "IntellectualPropertyCopyrightOwners", DbType.String, DbString.MaxLength);

        }
    }
}
