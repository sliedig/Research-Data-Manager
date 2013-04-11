using System;
using System.Collections.Generic;
using Urdms.DocumentBuilderService.Models.Enums;

namespace Urdms.DocumentBuilderService.Database.Entities
{
    public interface IBackupPolicy
    {
        BackupLocations BackupLocations { get; }
        string BackupPolicyLocationsDescription { get; }
        DataResponsibilities BackupPolicyResponsibilities { get; }
        string BackupPolicyResponsibilitiesDescription { get; }
    }

    public interface IDocumentation
    {
        string MetadataStandards { get; }
    }

    public interface IDataOrganisation
    {
        string DirectoryStructure { get; }
    }

    public interface IDataRetention
    {
        DataRetentionPeriod DataRetentionPeriod { get; }
        DataResponsibilities DataRetentionResponsibilities { get; }
        string DataRetentionResponsibilitiesDescription { get; }
        DataRetentionLocations DataRetentionLocations { get; }
        string DataRetentionLocationsDescription { get; }
        bool? DepositToRepository { get; }
    }

    public interface IDataSharing
    {
        DataSharingAvailability DataSharingAvailability { get; }
        DateTime? DataSharingAvailabilityDate { get; }
        ShareAccess ShareAccess { get; }
        string ShareAccessDescription { get; }
        DataLicensingType DataLicensingType { get; }
        string ReuseByOrganisations { get; }
    }

    public interface IEthics
    {
        bool? EthicRequiresClearance { get; }
        string EthicComments { get; }
    }

    public interface IConfidentiality
    {
        bool? IsSensitive { get; }
        string ConfidentialityComments { get; }
    }

    public interface IAccess
    {
        IList<UrdmsUser> UrdmsUsers { get; }
        IList<User> NonUrdmsUsers { get; }
    }

    public interface IPreExistingResearchData
    {
        bool? UseExistingData { get; }
        string ExistingDataOwner { get; }
        ExistingDataAccessTypes ExistingDataAccessTypes { get; }
        string AccessTypesDescription { get; }
    }

    public interface INewResearchData
    {
        string ResearchDataDescription { get; }
        DataOwners DataOwners { get; }
        string DataOwnersDescription { get; }
        DataActionFrequency DataActionFrequency { get; }
        bool? IsVersioned { get; }
    }

    public interface IDataStorage
    {
        InstitutionalStorageTypes InstitutionalStorageTypes { get; }
        string InstitutionalOtherTypeDescription { get; }
        ExternalStorageTypes ExternalStorageTypes { get; }
        string ExternalOtherTypeDescription { get; }
        PersonalStorageTypes PersonalStorageTypes { get; }
        string PersonalOtherTypeDescription { get; }
        MaxDataSize MaxDataSize { get; }
        string FileFormats { get; }
        VersionControl VersionControl { get; }
        string VersionControlDescription { get; }
    }

    public interface IRelationshipBetweenNewAndPreExistingData
    {
        DataRelationship RelationshipBetweenExistingAndNewData { get; }
    }

    public class DataManagementPlan : PdfModel,
        IBackupPolicy, IDocumentation, IDataRetention, IDataSharing, IEthics, IConfidentiality, IAccess, IPreExistingResearchData, INewResearchData, IRelationshipBetweenNewAndPreExistingData, IDataStorage, IDataOrganisation
    {
        public int Id { get; set; }
        // Backup
        public BackupLocations BackupLocations { get; set; }
        public string BackupPolicyLocationsDescription { get; set; }
        public DataResponsibilities BackupPolicyResponsibilities { get; set; }
        public string BackupPolicyResponsibilitiesDescription { get; set; }

        // Data documentation
        public string MetadataStandards { get; set; }

        // Data organisation
        public string DirectoryStructure { get; set; }

        // Relationship between new and pre-existing data
        public DataRelationship RelationshipBetweenExistingAndNewData { get; set; }

        // Data retention
        public DataRetentionPeriod DataRetentionPeriod { get; set; }
        public DataResponsibilities DataRetentionResponsibilities { get; set; }
        public string DataRetentionResponsibilitiesDescription { get; set; }
        public DataRetentionLocations DataRetentionLocations { get; set; }
        public string DataRetentionLocationsDescription { get; set; }
        public bool? DepositToRepository { get; set; }

        // Data sharing
        public DataSharingAvailability DataSharingAvailability { get; set; }
        public DateTime? DataSharingAvailabilityDate { get; set; }
        public ShareAccess ShareAccess { get; set; }
        public string ShareAccessDescription { get; set; }
        public DataLicensingType DataLicensingType { get; set; }
        public string ReuseByOrganisations { get; set; }

        // Ethical requirements
        public bool? EthicRequiresClearance { get; set; }
        public string EthicComments { get; set; }

        // Pre-existing research data
        public bool? UseExistingData { get; set; }
        public string ExistingDataOwner { get; set; }
        public ExistingDataAccessTypes ExistingDataAccessTypes { get; set; }
        public string AccessTypesDescription { get; set; }

        // New research data
        public string ResearchDataDescription { get; set; }
        public DataOwners DataOwners { get; set; }
        public string DataOwnersDescription { get; set; }
        public DataActionFrequency DataActionFrequency { get; set; }
        public bool? IsVersioned { get; set; }

        // Data storage
        public InstitutionalStorageTypes InstitutionalStorageTypes { get; set; }
        public string InstitutionalOtherTypeDescription { get; set; }
        public ExternalStorageTypes ExternalStorageTypes { get; set; }
        public string ExternalOtherTypeDescription { get; set; }
        public PersonalStorageTypes PersonalStorageTypes { get; set; }
        public string PersonalOtherTypeDescription { get; set; }
        public MaxDataSize MaxDataSize { get; set; }
        public string FileFormats { get; set; }
        public VersionControl VersionControl { get; set; }
        public string VersionControlDescription { get; set; }

        // Privacy and confidentiality requirements
        public bool? IsSensitive { get; set; }
        public string ConfidentialityComments { get; set; }

        // Project info
        public string ProjectTitle { get; set; }
        public string PricipalInvestigator { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime DateModified { get; set; }

        public IList<UrdmsUser> UrdmsUsers { get; set; }
        public IList<User> NonUrdmsUsers { get; set; }

        public DataManagementPlan()
        {
            UrdmsUsers = new List<UrdmsUser>();
            NonUrdmsUsers = new List<User>();
        }
    }

    public class User
    {
        public string Name { get; set; }
        public int Role { get; set; }
    }

    public class UrdmsUser : User
    {
        public string Id { get; set; }
    }
}
