using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
      [Migration(20110909155200)]
    public class AlterDmpTable20110909155200 : MigratorDotNetMigration
    {
          public override void Up()
          {
              Database.AddColumn("[DataManagementPlan]", "[CreationDate]", DbType.DateTime);
          }

          public override void Down()
          {
              Database.RemoveColumn("[DataManagementPlan]", "[CreationDate]");
          }
    }
}
