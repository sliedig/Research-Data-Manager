using System;
using System.Configuration;
using System.Data.SqlClient;
using Urdms.Approvals.ApprovalService.Events;
using NServiceBus.Logging;

namespace Urdms.Approvals.ViewModelUpdater.Database.Repositories
{
    public interface IDataCollectionRepository
    {
        void UpdateStatus(int id, DataCollectionApprovalState approvalState, DateTime stateChangedOn);
    }

    public class DataCollectionRepository : IDataCollectionRepository
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _connectionString;
        private const string ConstSql = "UPDATE DataCollection SET State = {0}, StateChangedOn = '{1}' WHERE Id = {2}";

        public DataCollectionRepository()
        {
            _connectionString = ConfigurationManager.AppSettings["DmpDBConnection"];
        }


        /// <summary>
        /// Updates the approvalState and modified dates.
        /// </summary>
        /// <param name="id">The DataCollection id.</param>
        /// <param name="approvalState">The DataCollectionApprovalState.</param>
        /// <param name="stateChangedOn">The date and tome the state was changed.</param>
        public void UpdateStatus(int id, DataCollectionApprovalState approvalState, DateTime stateChangedOn)
        {
            Log.InfoFormat("[URDMS] Received UpdateStatus for id:{0}, approvalState:{1}, stateStangedOn:{2}", id, approvalState, stateChangedOn);

            var queryString = string.Format(ConstSql, (int)approvalState, stateChangedOn.ToString("yyyyMMdd HH:mm:ss"), id);
           
            Log.Info("[URDMS] Created query string: " + queryString);
            
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand(queryString, connection);
                    command.Connection.Open();
                    command.ExecuteNonQuery();
                    command.Dispose();
                }
            }
            catch (Exception ex)
            {
                Log.Error("[URDMS] Error occurred updating DataCollection table in the URDMS databse. Exception: " + ex);
            }

            Log.Info("[URDMS] Call to UpdateStatus completed");
        }
    }
}

