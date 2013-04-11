using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110906123800)]
    public class CreateProjectRelationship : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddTable("[ProjectRelationship]",
             new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
             new Column("Name", DbType.String, 100, ColumnProperty.NotNull));

            Database.AddTable("[PartiesToProjectRelationships]",
              new Column("PartyId", DbType.Int32, ColumnProperty.NotNull),
              new Column("ProjectRelationshipId", DbType.Int32, ColumnProperty.NotNull));

        }

        public override void Down()
        {
            Database.RemoveTable("[ProjectRelationship]");
            Database.RemoveTable("[PartiesToProjectRelationships]");
        }
    }
}
