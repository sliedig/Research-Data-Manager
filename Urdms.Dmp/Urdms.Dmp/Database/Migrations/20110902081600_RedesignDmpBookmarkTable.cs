using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20110902081600)]
    public class RedesignDmpBookMarkTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.RemoveTable("[DmpBookmark]");

            Database.AddTable("[DataManagementPlanBookmark]",
                             new Column("DataManagementPlanId", DbType.Int32, ColumnProperty.PrimaryKey),
                             new Column("HighestCompletedStep", DbType.Int32, ColumnProperty.NotNull));
            
        }

        public override void Down()
        {
            Database.RemoveTable("[DataManagementPlanBookmark]");

            Database.AddTable("[DmpBookmark]",
                             new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),   
                             new Column("DmpId", DbType.Int32, ColumnProperty.NotNull),
                             new Column("HighestCompletedStep", DbType.Int32, ColumnProperty.NotNull));
        }
    }
}
