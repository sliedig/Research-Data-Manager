using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    // This class is so that the last migration is always a dummy one, which
    //  saves a problem experienced with the MigrationsShould.Go_down_and_up
    //  test leaving the database in an odd state for other tests
    [Migration(20200101000000)]
    public class Dummy : MigratorDotNetMigration
    {
        public override void Up()
        { }

        public override void Down()
        { }
    }
}
