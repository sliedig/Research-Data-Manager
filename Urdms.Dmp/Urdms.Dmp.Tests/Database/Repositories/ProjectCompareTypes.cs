using System;

namespace Urdms.Dmp.Tests.Database.Repositories
{
    [Flags]
    internal enum ProjectCompareTypes
    {
        None,
        DataManagementPlan = 1,
        NewDataDetail = 4,
        BackupPolicy = 8,
        DataDocumentation = 16,
        DataRetention = 32,
        DataSharing = 64,
        IntellectualProperty = 128,
        ExistingDataDetail = 256,
        Members = 512,
        Ethic = 1024,
        Basic = 2048,
        DataRelationshipDetail = 4096,
        Project = 8192,
        DataStorage = 16384,
        SocioEconomicResearch = 32678,
        FieldOfResearch = 65536,
        All = DataManagementPlan | NewDataDetail | BackupPolicy | DataDocumentation | DataRetention | DataSharing | IntellectualProperty |
            ExistingDataDetail | Ethic | DataRelationshipDetail | Project | DataStorage | SocioEconomicResearch | FieldOfResearch
    }
}
