using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Urdms.DocumentBuilderService.Database.Entities;
using Urdms.DocumentBuilderService.Helpers;
using Urdms.DocumentBuilderService.Models.Enums;

namespace Urdms.DocumentBuilderService.Database.Repositories
{
    public interface IDataManagementPlanRepository
    {
        DataManagementPlan GetDataManagementPlanByProjectId(int id);
    }

    public class DataManagementPlanRepository : IDataManagementPlanRepository
    {
        private readonly SqlConnection _sqlConnection;

        #region sql strings

		private const string DmpSqlString = "SELECT p.[Title] AS ProjectTitle, " +
                                           "p.[Description] AS ProjectDescription, " +
                                           "dmp.[Id]," +
                                           // need to add PrincipalInvestigator and users...
                                           "dmp.[CreationDate]," +
                                           // Backup
                                           "dmp.[BackupLocations]," +
                                           "dmp.[BackupPolicyLocationsDescription]," +
                                           "dmp.[BackupPolicyResponsibilities]," +
                                           "dmp.[BackupPolicyResponsibilitiesDescription]," +
                                           // Data documentation
                                           "dmp.[MetadataStandards]," +
                                           // Data organisation
                                           "dmp.[DirectoryStructure]," +
                                           // Relationship between new and pre-existing data
                                           "dmp.[RelationshipBetweenExistingAndNewData]," +
                                           // Data retention
                                           "dmp.[DataRetentionPeriod]," +
                                           "dmp.[DataRetentionResponsibilities]," +
                                           "dmp.[DataRetentionResponsibilitiesDescription]," +
                                           "dmp.[DataRetentionLocations]," +
                                           "dmp.[DataRetentionLocationsDescription]," +
                                           "dmp.[DepositToRepository]," +
                                           // Data sharing
                                           "dmp.[DataSharingAvailability]," +
                                           "dmp.[ShareAccess]," +
                                           "dmp.[ShareAccessDescription]," +
                                           "dmp.[DataLicensingType], " +
                                           "dmp.[ReuseByOrganisations]," +
                                           "dmp.[DataSharingAvailabilityDate]," +
                                           // Ethical requirements
                                           "dmp.[EthicRequiresClearance]," +
                                           "dmp.[EthicComments]," +
                                           // Pre-existing research data
                                           "dmp.[UseExistingData]," +
                                           "dmp.[ExistingDataOwner]," +
                                           "dmp.[ExistingDataAccessTypes]," +
                                           "dmp.[AccessTypesDescription]," +
                                           // New research data
                                           "dmp.[ResearchDataDescription]," +
                                           "dmp.[DataOwners]," +
                                           "dmp.[DataOwnersDescription]," +
                                           "dmp.[DataActionFrequency]," +
                                           "dmp.[IsVersioned]," +
                                           // Data storage
										   "dmp.[InstitutionalStorageTypes]," +
										   "dmp.[InstitutionalOtherTypeDescription]," +
                                           "dmp.[ExternalStorageTypes]," +
                                           "dmp.[ExternalOtherTypeDescription]," +
                                           "dmp.[PersonalStorageTypes]," +
                                           "dmp.[PersonalOtherTypeDescription]," +
                                           "dmp.[MaxDataSize]," +
                                           "dmp.[FileFormats]," +
                                           "dmp.[VersionControl]," +
                                           "dmp.[VersionControlDescription]," +
                                           // Privacy and confidentiality requirements
                                           "dmp.[IsSensitive]," +
                                           "dmp.[ConfidentialityComments]" +

                                           " FROM Project AS p INNER JOIN DataManagementPlan AS dmp " +
                                           " ON dmp.[Id] = p.[DataManagementPlanId] " + "WHERE p.[Id] = {0}";

        private const string PrincipalinvestigatorByProjectId = "SELECT Party.FullName"
                                                               + " FROM Party INNER JOIN ProjectParty ON Party.Id = ProjectParty.PartyId "
                                                               + " WHERE (ProjectParty.ProjectId = {0}) AND (ProjectParty.Relationship = {1})";

        private const string PartiesByProjectId = "SELECT Party.UserId, Party.FullName, ProjectParty.Role "
                                                 + " FROM Party INNER JOIN ProjectParty ON Party.Id = ProjectParty.PartyId "
                                                 + " WHERE (ProjectParty.ProjectId = {0}) AND (ProjectParty.Role <> '{1}')"; 

	    #endregion

      

        public DataManagementPlanRepository()
        {
            var connectionString = ConfigurationManager.AppSettings["DmpDBConnection"];
            if (!String.IsNullOrWhiteSpace(connectionString))
            {
                _sqlConnection = new SqlConnection(connectionString);
            }
        }

        public DataManagementPlan GetDataManagementPlanByProjectId(int id)
        {
            var dataManagementPlan = new DataManagementPlan();
            try
            {
                _sqlConnection.Open();
                using (var sqlCommand = new SqlCommand(string.Format(DmpSqlString, id), _sqlConnection))
                {
                    using (var sqlReader = sqlCommand.ExecuteReader())
                    {
                        while (sqlReader.Read())
                        {
                            dataManagementPlan.Id = sqlReader.GetValue<int>("Id");
                            // Project details
                            dataManagementPlan.ProjectTitle = sqlReader.GetStringValue("ProjectTitle");
                            dataManagementPlan.ProjectDescription = sqlReader.GetStringValue("ProjectDescription");
                            dataManagementPlan.CreationDate = sqlReader.GetValue<DateTime>("CreationDate");
                            // Backup
                            dataManagementPlan.BackupLocations = sqlReader.GetEnumValue<BackupLocations>("BackupLocations");
                            dataManagementPlan.BackupPolicyLocationsDescription = sqlReader.GetStringValue("BackupPolicyLocationsDescription");
                            dataManagementPlan.BackupPolicyResponsibilities = sqlReader.GetEnumValue<DataResponsibilities>("BackupPolicyResponsibilities");
                            dataManagementPlan.BackupPolicyResponsibilitiesDescription = sqlReader.GetStringValue("BackupPolicyResponsibilitiesDescription");
                            // Data documentation
                            dataManagementPlan.MetadataStandards = sqlReader.GetStringValue("MetadataStandards");
                            // Data organisation
                            dataManagementPlan.DirectoryStructure = sqlReader.GetStringValue("DirectoryStructure");
                            // Relationship between new and pre-existing data
                            dataManagementPlan.RelationshipBetweenExistingAndNewData = sqlReader.GetEnumValue<DataRelationship>("RelationshipBetweenExistingAndNewData");
                            // Data retention
                            dataManagementPlan.DataRetentionPeriod = sqlReader.GetEnumValue<DataRetentionPeriod>("DataRetentionPeriod");
                            dataManagementPlan.DataRetentionResponsibilities = sqlReader.GetEnumValue<DataResponsibilities>("DataRetentionResponsibilities");
                            dataManagementPlan.DataRetentionResponsibilitiesDescription = sqlReader.GetStringValue("DataRetentionResponsibilitiesDescription");
                            dataManagementPlan.DataRetentionLocations = sqlReader.GetEnumValue<DataRetentionLocations>("DataRetentionLocations");
                            dataManagementPlan.DataRetentionLocationsDescription = sqlReader.GetStringValue("DataRetentionLocationsDescription");
                            dataManagementPlan.DepositToRepository = sqlReader.GetNullableValue<bool>("DepositToRepository");
                            // Data sharing
                            dataManagementPlan.DataSharingAvailability = sqlReader.GetEnumValue<DataSharingAvailability>("DataSharingAvailability");
                            dataManagementPlan.ShareAccess = sqlReader.GetEnumValue<ShareAccess>("ShareAccess");
                            dataManagementPlan.ShareAccessDescription = sqlReader.GetStringValue("ShareAccessDescription");
                            dataManagementPlan.DataLicensingType = sqlReader.GetEnumValue<DataLicensingType>("DataLicensingType");
                            dataManagementPlan.ReuseByOrganisations = sqlReader.GetStringValue("ReuseByOrganisations");
                            dataManagementPlan.DataSharingAvailabilityDate = sqlReader.GetNullableValue<DateTime>("DataSharingAvailabilityDate");
                            // Ethical requirements
                            dataManagementPlan.EthicRequiresClearance = sqlReader.GetNullableValue<bool>("EthicRequiresClearance");
                            dataManagementPlan.EthicComments = sqlReader.GetStringValue("EthicComments");
                            // Pre-existing research data
                            dataManagementPlan.UseExistingData = sqlReader.GetNullableValue<bool>("UseExistingData");
                            dataManagementPlan.ExistingDataOwner = sqlReader.GetStringValue("ExistingDataOwner");
                            dataManagementPlan.ExistingDataAccessTypes = sqlReader.GetEnumValue<ExistingDataAccessTypes>("ExistingDataAccessTypes");
                            dataManagementPlan.AccessTypesDescription = sqlReader.GetStringValue("AccessTypesDescription");
                            // New research data
                            dataManagementPlan.ResearchDataDescription = sqlReader.GetStringValue("ResearchDataDescription");
                            dataManagementPlan.DataOwners = sqlReader.GetEnumValue<DataOwners>("DataOwners");
                            dataManagementPlan.DataOwnersDescription = sqlReader.GetStringValue("DataOwnersDescription");
                            dataManagementPlan.DataActionFrequency = sqlReader.GetEnumValue<DataActionFrequency>("DataActionFrequency");
                            dataManagementPlan.IsVersioned = sqlReader.GetNullableValue<bool>("IsVersioned");
                            // Data storage
                            dataManagementPlan.InstitutionalStorageTypes = sqlReader.GetEnumValue<InstitutionalStorageTypes>("InstitutionalStorageTypes");
                            dataManagementPlan.InstitutionalOtherTypeDescription = sqlReader.GetStringValue("InstitutionalOtherTypeDescription");
                            dataManagementPlan.ExternalStorageTypes = sqlReader.GetEnumValue<ExternalStorageTypes>("ExternalStorageTypes");
                            dataManagementPlan.ExternalOtherTypeDescription = sqlReader.GetStringValue("ExternalOtherTypeDescription");
                            dataManagementPlan.PersonalStorageTypes = sqlReader.GetEnumValue<PersonalStorageTypes>("PersonalStorageTypes");
                            dataManagementPlan.PersonalOtherTypeDescription = sqlReader.GetStringValue("PersonalOtherTypeDescription");
                            dataManagementPlan.MaxDataSize = sqlReader.GetEnumValue<MaxDataSize>("MaxDataSize");
                            dataManagementPlan.FileFormats = sqlReader.GetStringValue("FileFormats");
                            dataManagementPlan.VersionControl = sqlReader.GetEnumValue<VersionControl>("VersionControl");
                            dataManagementPlan.VersionControlDescription = sqlReader.GetStringValue("VersionControlDescription");
                            // Privacy and confidentiality requirements
                            dataManagementPlan.IsSensitive = sqlReader.GetNullableValue<bool>("IsSensitive");
                            dataManagementPlan.ConfidentialityComments = sqlReader.GetStringValue("ConfidentialityComments");
                        }
                        sqlReader.Close();
                    }
                }

                PopulatePrincipalInvestigatorByProjectId(_sqlConnection, dataManagementPlan, id);
                PopulateUsersByProjectId(_sqlConnection, dataManagementPlan, id);
                dataManagementPlan.DateModified = DateTime.Now;
            }
            finally
            {
                _sqlConnection.Close();
            }
            return dataManagementPlan;
        }

        private void PopulatePrincipalInvestigatorByProjectId(SqlConnection sqlConnection, DataManagementPlan dataManagementPlan, int projectId)
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                var sqlCommand = new SqlCommand(string.Format(PrincipalinvestigatorByProjectId, projectId, (int)ProjectRelationship.PrincipalInvestigator), _sqlConnection);
                var sqlReader = sqlCommand.ExecuteReader();
                while (sqlReader.Read())
                {
                    dataManagementPlan.PricipalInvestigator= (string) sqlReader["FullName"];
                }
                sqlReader.Close();
            }
            
        }

        private void PopulateUsersByProjectId(SqlConnection sqlConnection, DataManagementPlan dataManagementPlan, int projectId)
        {
            var urdmsUsers = new List<UrdmsUser>();
            var nonUrdmsUsers = new List<User>();
            
            if (sqlConnection.State == ConnectionState.Open)
            {
                var sqlCommand = new SqlCommand(string.Format(PartiesByProjectId, projectId, (int)AccessRole.Owners), _sqlConnection);
                var sqlReader = sqlCommand.ExecuteReader();
                while (sqlReader.Read())
                {
                    int role;
                    var userId = sqlReader["UserId"].ToString();
                    var fullName = sqlReader["FullName"].ToString();
                    int.TryParse(sqlReader["Role"].ToString(), out role);

                    if (!string.IsNullOrWhiteSpace(userId))
                    {
                        urdmsUsers.Add(new UrdmsUser {Id = userId, Name = fullName, Role = role});
                    }
                    else
                    {
                        nonUrdmsUsers.Add(new User {Name = fullName, Role = role});
                    }
                }
                sqlReader.Close();
            }
            dataManagementPlan.UrdmsUsers = urdmsUsers;
            dataManagementPlan.NonUrdmsUsers = nonUrdmsUsers;
        }
    }
}

