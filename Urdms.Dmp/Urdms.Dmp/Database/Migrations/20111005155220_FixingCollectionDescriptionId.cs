using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111005155220)]
    public class FixingCollectionDescriptionId : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionKeyword] DROP COLUMN CollectionDescriptionId");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionSocioEconomicObjective] DROP COLUMN CollectionDescriptionId");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionFieldOfResearch] DROP COLUMN CollectionDescriptionId");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionApprover] DROP COLUMN CollectionDescriptionId");

            Database.AddColumn("[CollectionDescriptionKeyword]", new Column("CollectionDescriptionId", DbType.Guid));
            Database.AddColumn("[CollectionDescriptionSocioEconomicObjective]", new Column("CollectionDescriptionId", DbType.Guid));
            Database.AddColumn("[CollectionDescriptionFieldOfResearch]", new Column("CollectionDescriptionId", DbType.Guid));
            Database.AddColumn("[CollectionDescriptionApprover]", new Column("CollectionDescriptionId", DbType.Guid));
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionKeyword] DROP COLUMN CollectionDescriptionId");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionSocioEconomicObjective] DROP COLUMN CollectionDescriptionId");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionFieldOfResearch] DROP COLUMN CollectionDescriptionId");
            Database.ExecuteNonQuery("ALTER TABLE [CollectionDescriptionApprover] DROP COLUMN CollectionDescriptionId");

            Database.AddColumn("[CollectionDescriptionKeyword]", new Column("CollectionDescriptionId", DbType.Int32));
            Database.AddColumn("[CollectionDescriptionSocioEconomicObjective]", new Column("CollectionDescriptionId", DbType.Int32));
            Database.AddColumn("[CollectionDescriptionFieldOfResearch]", new Column("CollectionDescriptionId", DbType.Int32));
            Database.AddColumn("[CollectionDescriptionApprover]", new Column("CollectionDescriptionId", DbType.Int32));
        }
    }
}
