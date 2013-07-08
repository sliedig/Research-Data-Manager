using System.Configuration;
using System.Data.SqlClient;
using NServiceBus.Logging;

namespace Urdms.ProvisioningService.ViewModelUpdater.Database.Repositories
{
    public interface IDataCollectionRepository
    {
        void UpdateStatusByProjectId(int projectId, int status, string siteUrl, int requestId);
    }

    public class DataCollectionRepository : IDataCollectionRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _connectionString;
        private const string Sql = "UPDATE Project SET ProvisioningStatus = {0}, SiteUrl='{1}', ProvisioningRequestId='{2}' WHERE Id = {3}";

        public DataCollectionRepository()
        {
            _connectionString = ConfigurationManager.AppSettings["DmpDBConnection"];
        }


        public void UpdateStatusByProjectId(int projectId, int status, string siteUrl, int requestId)
        {
            Log.InfoFormat("[URDMS] Received UpdateStatus for project:{0}, Status:{1}, ProvisioningRequestId:{2}", projectId, status, requestId);

            var queryString = string.Format(Sql, status, siteUrl, requestId, projectId);

            Log.Info("[URDMS] Created query string: " + queryString);

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(queryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
                command.Dispose();
            }

            Log.Info("[URDMS] Call to UpdateStatusByProjectId completed.");
        }
    }
}

