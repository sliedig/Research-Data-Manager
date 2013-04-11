using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111018135347)]
    public class RenameCollectionDescriptionToDataCollection : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.ExecuteNonQuery("sp_rename 'CollectionDescription', 'DataCollection'");
            Database.ExecuteNonQuery("sp_rename 'CollectionDescriptionApprover', 'DataCollectionApprover'");
            Database.ExecuteNonQuery("sp_rename 'CollectionDescriptionFieldOfResearch', 'DataCollectionFieldOfResearch'");
            Database.ExecuteNonQuery("sp_rename 'CollectionDescriptionParty', 'DataCollectionParty'");
            Database.ExecuteNonQuery("sp_rename 'CollectionDescriptionSocioEconomicObjective', 'DataCollectionSocioEconomicObjective'");

            Database.ExecuteNonQuery("sp_rename 'DataCollectionApprover.CollectionDescriptionId', 'DataCollectionId', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataCollectionFieldOfResearch.CollectionDescriptionId', 'DataCollectionId', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataCollectionParty.CollectionDescriptionId', 'DataCollectionId', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataCollectionSocioEconomicObjective.CollectionDescriptionId', 'DataCollectionId', 'COLUMN'");
        }

        public override void Down()
        {
            Database.ExecuteNonQuery("sp_rename 'DataCollectionApprover.DataCollectionId', 'CollectionDescriptionId', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataCollectionFieldOfResearch.DataCollectionId', 'CollectionDescriptionId', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataCollectionParty.DataCollectionId', 'CollectionDescriptionId', 'COLUMN'");
            Database.ExecuteNonQuery("sp_rename 'DataCollectionSocioEconomicObjective.DataCollectionId', 'CollectionDescriptionId', 'COLUMN'");

            Database.ExecuteNonQuery("sp_rename 'DataCollection', 'CollectionDescription'");
            Database.ExecuteNonQuery("sp_rename 'DataCollectionApprover', 'CollectionDescriptionApprover'");
            Database.ExecuteNonQuery("sp_rename 'DataCollectionFieldOfResearch', 'CollectionDescriptionFieldOfResearch'");
            Database.ExecuteNonQuery("sp_rename 'DataCollectionParty', 'CollectionDescriptionParty'");
            Database.ExecuteNonQuery("sp_rename 'DataCollectionSocioEconomicObjective', 'CollectionDescriptionSocioEconomicObjective'");
        }
    }
}
