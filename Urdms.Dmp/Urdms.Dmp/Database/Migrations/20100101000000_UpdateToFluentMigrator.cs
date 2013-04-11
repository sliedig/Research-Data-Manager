using System;
using System.Data.SqlClient;
using System.Diagnostics;
using Curtin.Framework.Database.Migrations;
using FluentMigrator;

namespace Urdms.Dmp.Database.Migrations
{
    internal static class SqlCommandExtensions
    {
        public static int GetTableCount(this SqlConnection connection, string tableName)
        {
            try
            {
                var command = new SqlCommand(string.Format("SELECT count(*) FROM [{0}]", tableName), connection);
                var reader = command.ExecuteReader();
                reader.Read();
                var numRows = reader.GetInt32(0);
                reader.Close();
                return numRows;
            }
            catch(Exception)
            {
                return 0;
            }
        }
    }

    [Migration(20100101000000)]
    public class UpdateToFluentMigrator : Migration
    {

        public override void Up()
        {
            // Check if this is the first time the Fluent Migrations have been run
            var connection = new SqlConnection(FluentRunner.ConnectionString);
            connection.Open();
            if (connection.GetTableCount("VersionInfo") != 0 || connection.GetTableCount("SchemaInfo") == 0)
            {
                Trace.WriteLine("Already converted to FluentMigrator. Nothing to do here.");
                connection.Close();
                return;
            }
            connection.Close();

            throw new Exception(@"You need to upgrade the database to FluentMigrator; run the following SQL:

CREATE TABLE [dbo].VersionInfo (
   [Version] bigint not null
   CONSTRAINT [PK_Version] PRIMARY KEY CLUSTERED ([Version] ASC)
);

INSERT INTO [VersionInfo] SELECT [Version] FROM [SchemaInfo];

DELETE FROM [SchemaInfo];
            ");
        }

        public override void Down() {}
    }
}
