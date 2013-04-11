using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111121105013)]
    public class AddDataOrganisationToDataManagementPlanTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataDocumentationDirectoryStructure', 'DataOrganisationDirectoryStructure', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataDocumentationVersionControl', 'DataStorageVersionControl', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataDocumentationVersionControlDescription', 'DataStorageVersionControlDescription', 'COLUMN'");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN [DataDocumentationFileNamingConvention]");
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataOrganisationDirectoryStructure', 'DataDocumentationDirectoryStructure', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageVersionControl', 'DataDocumentationVersionControl', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataManagementPlan.DataStorageVersionControlDescription', 'DataDocumentationVersionControlDescription', 'COLUMN'");
            Database.AddColumn("[DataManagementPlan]", new Column("DataDocumentationFileNamingConvention", DbType.String, DbString.MaxLength));
        }
    }
}
