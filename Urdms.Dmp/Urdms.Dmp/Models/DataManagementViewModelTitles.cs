namespace Urdms.Dmp.Models
{
    public static class DataManagementViewModelTitles
    {
		public const string FindUserId = "Institutional members (enter Institutional user ID)";
		public const string NonUrdmsNewUserName = "Non-Institutional members (enter member's name)";
        public const string PrincipalInvestigator = "Principal Investigator";
        public static class DataStorage
        {
            public const string InstitutionStorageTypes = "Institution-based storage solutions (highly recommended)";
            public const string ExternalStorageTypes = "External storage solutions (suitable for research projects with very large data sets and sizes)";
            public const string PersonalStorageTypes = "Personal storage solutions (not recommended)";
            public const string MaxDataSize = "What is the expected size of all the research data that will be captured or created during the life of the research project?";
            public const string FileFormats = "What are the predominant file formats that you will use for your research data?";
            public const string VersionControl = "What version control measure will be applied to the research data?";
            public const string VersionControlDescription = "Provide details";
        }

        public static class DataOrganisation
        {
            public const string DirectoryStructure = "What standards will you apply to ensure the research data is well organised and structured?";
        }

        public static class NewDataDetail
        {
            public const string ResearchDataDescription = "Provide a description of the research data being generated or collected in this project";
            public const string ResearchDataProcess = "Provide a description of the process by which the research data will be generated or collected";
            public const string DataOwners = "Who owns the research data that will be generated or collected in this project?";
            public const string DataOwnersDescription = "Provide additional information on data ownership (if required)";
            public const string DataActionFrequency = "Once created or captured, how often will the research data updated?";
            public const string IsVersioned = "Will the files be overwritten or will new file versions be maintained?";
        }

        public static class ExistingDataDetail
        {
            public const string UseExistingData = "Will you be using pre-existing research data in this project?";
            public const string Owner = "Who owns the pre-existing research data?";
            public const string AccessTypes = "How will you obtain access or copy of the pre-existing research data?";
            public const string AccessTypesDescription = "Specify the other ways by which you will obtain access or copy to pre-existing research data";
        }

        public static class DataDocumentation
        {
            public const string MetadataStandards = "What metadata standards or discipline specific descriptive information will you use?";
        }

        public static class Ethic
        {
            public const string EthicRequiresClearance = "Are there any ethical issues or considerations related to the research data?";
            public const string EthicComments = "Provide details of the ethical considerations or how you will manage them in this project";
        }

        public static class Confidentiality
        {
            public const string IsSensitive = "Will the research data contain confidential or sensitive information?";
            public const string ConfidentialityComments = "Provide details of the confidentiality and sensitivity of the data and how you will manage them in this project";
        }

        public static class BackupPolicy
        {
            public const string BackupLocations = "Where will the research data be backed up?";
            public const string BackupPolicyLocationsDescription = "Provide details of the other backup location(s)";
            public const string BackupPolicyResponsibilities = "Who is responsible for backup?";
            public const string BackupPolicyResponsibilitiesDescription = "Provide details for 'others'";
        }

        public static class DataRetention
        {
            public const string DataRetentionPeriod = "Which of the following retention periods will apply to your research data upon completion of the project?";
            public const string DataRetentionResponsibilities = "Who is responsible for the long-term preservation of the research data?";
            public const string DataRetentionResponsibilitiesDescription = "Provide details";
            public const string DataRetentionLocations = "Where or in which repository will the data be deposited?";
            public const string DataRetentionLocationsDescription = "Provide name of the repository (use comma to separate multiple names)";
            public const string DepositToRepository = "Will you deposit the research data in a repository or archive?";
        }

        public static class DataSharing
        {
            public const string DataSharingAvailability = "When will the research data be made available to others?";
            public const string DataSharingAvailabilityDate = "Research data can be made available after";
            public const string ShareAccess = "What access will be provided to the research data if it will be made available to others?";
            public const string ReuseByOrganisations = "Which groups or organisations are likely to be interested in the data created or captured in this project?";
            public const string DataLicensingType = "What data licensing arrangement will be used for allowing others to reuse the research data generated or collected in this project?";
            public const string ShareAccessDescription = "Specify";
        }

        public static class DataRelationshipDetail
        {
            public const string RelationshipBetweenExistingAndNewData = "How will you manage integration between the newly generated and pre-existing research data?";
        }
    }
}