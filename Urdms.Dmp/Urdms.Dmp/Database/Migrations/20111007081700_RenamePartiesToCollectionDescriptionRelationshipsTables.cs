using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111007081700)]
    public class RenamePartiesToCollectionDescriptionRelationshipsTables : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.RenameTable("[PartiesToCollectionDescriptionRelationships]", "CollectionDescriptionRelationshipsToParties");
        }

        public override void Down()
        {
            Database.RenameTable("[CollectionDescriptionRelationshipsToParties]", "PartiesToCollectionDescriptionRelationships");
        }
    }
}
