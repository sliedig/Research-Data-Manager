// Ensure the Urdms.FlowWeb namespace exists
var Urdms = Urdms || {};
Urdms.FlowWeb = Urdms.FlowWeb || {};

Urdms.FlowWeb.formStart = function () {
    // depends(id, dependsOn, dependsOnValue, negate, dependsOnIsId, hideFieldOnly
    depends("DataStorage_InstitutionalOtherTypeDescription", "DataStorage_InstitutionalStorageTypesOther", "Other", false, true, true);
    depends("DataStorage_ExternalOtherTypeDescription", "DataStorage_ExternalStorageTypesOther", "Other", false, true, true);
    depends("DataStorage_PersonalOtherTypeDescription", "DataStorage_PersonalStorageTypesOther", "Other", false, true, true);
    depends("BackupPolicy_BackupPolicyLocationsDescription", "BackupPolicy_BackupLocationsOther", "Other", false, true, true);
    depends("BackupPolicy_BackupPolicyResponsibilitiesDescription", "BackupPolicy_BackupPolicyResponsibilitiesOther", "Other", false, true, true);
};