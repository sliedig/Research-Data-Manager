using System.Data;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110830172000)]
    public class RefactorProject : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN ProjectPrincipalInvestigator");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN ProjectIdentifier");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN ProjectName");
            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN ProjectDescription");

            Database.AddColumn("[DataManagementPlan]", "[ProjectId]", DbType.Int32, ColumnProperty.NotNull);

            Database.RemoveTable("[UserAccess]");

            Database.AddTable("[Project]",
                              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                              new Column("Name", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                              new Column("Description", DbType.String, DbString.MaxLength, ColumnProperty.NotNull),
                              new Column("StartDate", DbType.DateTime),
                              new Column("EndDate", DbType.DateTime),
                              new Column("SourceProjectType", DbType.Int32),                         
                              new Column("SourceId", DbType.String, 100),
                              new Column("Keywords", DbType.String, DbString.MaxLength),
                              new Column("Status", DbType.Int32));

            Database.AddTable("[Member]",
                             new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                             new Column("UserId", DbType.String, 100),
                             new Column("FirstName", DbType.String, DbString.MaxLength),
                             new Column("LastName", DbType.String, DbString.MaxLength),
                             new Column("FullName", DbType.String, DbString.MaxLength),
                             new Column("Email", DbType.String, DbString.MaxLength),
                             new Column("Organisation", DbType.String, DbString.MaxLength));

            Database.AddTable("[DmpBookmark]",
                new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                new Column("DmpId", DbType.Int32, ColumnProperty.NotNull),
                new Column("HighestCompletedStep", DbType.Int32, ColumnProperty.NotNull));

            Database.AddTable("[FieldOfResearch]",
               new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
               new Column("Code", DbType.String, 100, ColumnProperty.NotNull),
               new Column("Allocation", DbType.Double, ColumnProperty.NotNull));

            Database.AddTable("[SocioEconomicResearch]",
               new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
               new Column("Code", DbType.String, 100, ColumnProperty.NotNull),
               new Column("Allocation", DbType.Double, ColumnProperty.NotNull));

            Database.AddTable("[Role]",
              new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
              new Column("RoleName", DbType.String, 100, ColumnProperty.NotNull),
              new Column("DataAccessPermission", DbType.Int32, ColumnProperty.NotNull));

        }

        public override void Down()
        {
            Database.RemoveTable("[Project]");
            Database.RemoveTable("[Member]");
            Database.RemoveTable("[DmpBookmark]");
            Database.RemoveTable("[FieldOfResearch]");
            Database.RemoveTable("[SocioEconomicResearch]");
            Database.RemoveTable("[Role]");

            Database.ExecuteNonQuery("ALTER TABLE [DataManagementPlan] DROP COLUMN ProjectId");

            Database.AddColumn("[DataManagementPlan]", "ProjectPrincipalInvestigator", DbType.Int32, 0);
            Database.AddColumn("[DataManagementPlan]", "ProjectIdentifier", DbType.Int32, 0);
            Database.AddColumn("[DataManagementPlan]", "ProjectName", DbType.Int32, 0);
            Database.AddColumn("[DataManagementPlan]", "ProjectDescription", DbType.Int32, 0);

            Database.AddTable("[UserAccess]",
                             new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                             new Column("StaffId", DbType.String, 100),
                             new Column("CanWrite", DbType.Boolean),
                             new Column("Organisation", DbType.String, DbString.MaxLength),
                             new Column("UserName", DbType.String, DbString.MaxLength),
                             new Column("DataManagementPlanId", DbType.Int32));

        }
    }
}
