using System;
using System.Configuration;
using System.Data.SqlClient;
using Urdms.Approvals.VivoPublisher.Database.Entities;
using Urdms.Approvals.VivoPublisher.Database.Enums;
using Urdms.Approvals.VivoPublisher.Helpers;
using NServiceBus.Logging;

namespace Urdms.Approvals.VivoPublisher.Database.Repositories
{
    public interface IDataCollectionRepository
    {
        DataCollection Get(int id);
    }

    public class DataCollectionRepository : IDataCollectionRepository
    {
        // private readonly SqlConnection _sqlConnection;
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string _connectionString;

        #region SQL Statements

        const string ConstSqlGetDatacollection = "SELECT * FROM DataCollection AS dc WHERE dc.[Id] = {0}";

        const string ConstSqlGetParties = "SELECT * FROM DataCollectionParty dcp " +
                                   "INNER JOIN Party AS p ON dcp.[PartyId] = p.[Id] " +
                                   "WHERE dcp.[DataCollectionId] = {0}";

        const string ConstSqlGetSeo = "SELECT * FROM DataCollectionSocioEconomicObjective dcseo " +
                                 "WHERE dcseo.[DataCollectionId] = {0}";

        const string ConstSqlGetFor = "SELECT * FROM DataCollectionFieldOfResearch dcfor " +
                           "WHERE dcfor.[DataCollectionId] = {0}";

        #endregion

        public DataCollectionRepository() : this(ConfigurationManager.AppSettings["DataCollectionDBConnection"])
        {
            
        }

        public DataCollectionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DataCollection Get(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    var dataCollection = GetDataCollection(connection, id);
                    if (dataCollection != null)
                    {
                        PopulateUsers(connection, dataCollection);
                        PopulateSeoCodes(connection, dataCollection);
                        PopulateForCodes(connection, dataCollection);
                    }
                    return dataCollection;
                }
                catch (Exception ex)
                {
                   var message = string.Format("Could not retrieve data collection {0}", id);
                   Log.Error(message,ex);
                   throw;
                }
                finally
                {
                    connection.Close();
                }
                
                
            }

        }


        private static DataCollection GetDataCollection(SqlConnection conn, int id)
        {
            DataCollection collection = null;
            using (var command = new SqlCommand(string.Format(ConstSqlGetDatacollection, id), conn))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        collection = new DataCollection
                                         {
                                             Id = reader.GetValue<int>("Id"),
                                             ProjectId = reader.GetValue<int>("ProjectId"),
                                             Title = reader.GetStringValue("Title"),
                                             ResearchDataDescription = reader.GetStringValue("ResearchDataDescription"),
                                             StartDate = reader.GetValue<DateTime>("StartDate"),
                                             EndDate = reader.GetValue<DateTime>("EndDate"),
                                             Type = reader.GetEnumValue<DataCollectionType>("Type"),
                                             DataLicensingRights = reader.GetEnumValue<DataLicensingType>("DataLicensingRights"),
                                             ShareAccess = reader.GetEnumValue<ShareAccess>("ShareAccess"),
                                             ShareAccessDescription = reader.GetStringValue("ShareAccessDescription"),
                                             Keywords = reader.GetStringValue("Keywords"),
                                             Availability = reader.GetEnumValue<DataSharingAvailability>("Availability"),
                                             AvailabilityDate = reader.GetNullableValue<DateTime>("AvailabilityDate"),
                                             DataCollectionIdentifier = reader.GetValue<DataCollectionIdentifier>("DataCollectionIdentifier"),
                                             DataCollectionIdentifierValue = reader.GetStringValue("DataCollectionIdentifierValue")
                                         };
                    }
                }
            }
            return collection;
        }

        private static void PopulateUsers(SqlConnection conn, DataCollection dataCollection)
        {
            var cmd = new SqlCommand(string.Format(ConstSqlGetParties, dataCollection.Id), conn);

            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var userId = reader.GetStringValue("UserId");
                    var fullName = reader.GetStringValue("FullName");
                    var email = reader.GetStringValue("Email");
                    var organisation = reader.GetStringValue("Organisation");
                    var relationship = reader.GetEnumValue<DataCollectionRelationshipType>("Relationship");
                    dataCollection.Parties.Add(new DataCollectionParty
                                                    {
                                                        UserId = userId,
                                                        Email = email,
                                                        FullName = fullName,
                                                        Organisation = organisation,
                                                        Relationship = relationship
                                                    });

                }
            }
        }

        private static void PopulateSeoCodes(SqlConnection conn, DataCollection dataCollection)
        {
            var cmd = new SqlCommand(string.Format(ConstSqlGetSeo, dataCollection.Id), conn);

            using (var sqlReader = cmd.ExecuteReader())
            {
                while (sqlReader.Read())
                {
                    var socioEconomicObjectiveId = sqlReader.GetStringValue("SocioEconomicObjectiveId");
                    dataCollection.SocioEconomicObjectives.Add(new DataCollectionSocioEconomicObjective { seocode = socioEconomicObjectiveId });
                }
            }

        }

        private static void PopulateForCodes(SqlConnection conn, DataCollection dataCollection)
        {
            var cmd = new SqlCommand(string.Format(ConstSqlGetFor, dataCollection.Id), conn);
            
            using (var sqlReader = cmd.ExecuteReader())
            {
                while (sqlReader.Read())
                {
                    var dataCollectionFieldOfResearchId = sqlReader.GetStringValue("FieldOfResearchId");
                    dataCollection.FieldsOfResearch.Add(new DataCollectionFieldOfResearch { forcode = dataCollectionFieldOfResearchId });
                }
            }
        }
    }
}

