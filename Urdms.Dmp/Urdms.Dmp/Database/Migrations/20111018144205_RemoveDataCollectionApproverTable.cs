using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20111018144205)]
    public class RemoveDataCollectionApproverTable : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.RemoveTable("[DataCollectionApprover]");
        }

        public override void Down()
        {
            Database.AddTable("[DataCollectionApprover]",
                  new Column("Id", DbType.Int32, ColumnProperty.PrimaryKeyWithIdentity),
                  new Column("DataCollectionId", DbType.Int32),
                  new Column("ApprovalType", DbType.Int32),
                  new Column("ApproverId", DbType.String, 100, ColumnProperty.NotNull),
                  new Column("ApproverName", DbType.String, 100, ColumnProperty.NotNull),
                  new Column("ApprovedOn", DbType.DateTime, ColumnProperty.NotNull));
        }
    }
}
