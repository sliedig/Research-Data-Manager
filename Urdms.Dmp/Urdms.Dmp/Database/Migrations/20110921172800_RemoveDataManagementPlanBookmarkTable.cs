using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110921172800)]
    public class RemoveDataManagementPlanBookmarkTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.RemoveTable("[DataManagementPlanBookmark]");            
        }

        public override void Down()
        {
            Database.AddTable("[DataManagementPlanBookmark]",
                 new Column("DataManagementPlanId", DbType.Int32, ColumnProperty.PrimaryKey),
                 new Column("HighestCompletedStep", DbType.Int32, ColumnProperty.NotNull));
        }
    }
}
