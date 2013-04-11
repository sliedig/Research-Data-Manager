using System.Data;
using FluentMigrator;
using FluentMigrator.Legacy.MigratorDotNet;

namespace Urdms.Dmp.Database.Migrations
{
    [Migration(20120323094000)]
    public class AddProvisioningRequestIdColumnToProject : MigratorDotNetMigration
    {
        public override void Up()
        {
            Database.AddColumn("[Project]", new Column("ProvisioningRequestId", DbType.Int32, ColumnProperty.Null));
        }

        public override void Down()
        {
            Database.RemoveColumn("[Project]", "ProvisioningRequestId");

        }
    }
}
