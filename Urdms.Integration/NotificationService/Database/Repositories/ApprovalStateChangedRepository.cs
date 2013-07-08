using System;
using System.Configuration;
using System.Data.SqlClient;
using Urdms.NotificationService.Database.Enums;
using log4net;
using NServiceBus.Logging;

namespace Urdms.NotificationService.Database.Repositories
{
    public interface IApprovalStateChangedRepository
    {
        ApprovalStateChangedEmailData Get(int dataCollectionId);
    }


    public class ApprovalStateChangedRepository : IApprovalStateChangedRepository
    {
        private static readonly ILog Log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private readonly string _connectionString;

        private const string ConstSql =
                "SELECT     DataCollection.Title AS DataCollectionTitle, Project.Title AS ProjectTite, Party.FullName, Party.UserId"
                + " FROM    DataCollection INNER JOIN"
                + "         Project ON DataCollection.ProjectId = Project.Id INNER JOIN"
                + "         DataCollectionParty ON DataCollection.Id = DataCollectionParty.DataCollectionId INNER JOIN"
                + "         Party ON DataCollectionParty.PartyId = Party.Id"
                + " WHERE   (DataCollection.Id = {0}) AND (DataCollectionParty.Relationship = {1})";

        public ApprovalStateChangedRepository()
        {
            _connectionString = ConfigurationManager.AppSettings["DmpDBConnection"];
        }



        public ApprovalStateChangedEmailData Get(int dataCollectionId)
        {
            var queryString = string.Format(ConstSql, dataCollectionId, (int)DataCollectionRelationshipType.Manager);
            Log.Info("[URDMS] Created query string: " + queryString);

            var data = new ApprovalStateChangedEmailData();
            
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(queryString, connection);
                    command.Connection.Open();

                    var reader = command.ExecuteReader();
                    
                    while (reader.Read())
                    {
                        data.DataCollectionTitle = (string)reader["DataCollectionTitle"];
                        data.ProjectTitle = (string)reader["ProjectTite"];
                        data.Manager = (string) reader["FullName"];
                        data.ManagerId = (string) reader["UserId"];
                    }

                    command.Dispose();
                }

                Log.Info("[URDMS] Call to Get completed.");
                return data;
            }
            catch (Exception ex)
            {
                Log.Error("[URDMS] Error retrieving project information. Exception: " + ex);
                throw;
            }


        }
    }
}
