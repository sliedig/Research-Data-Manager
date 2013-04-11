// Ensure the Urdms.FlowWeb namespace exists
var Urdms = Urdms || {};
Urdms.FlowWeb = Urdms.FlowWeb || {};

Urdms.FlowWeb.formStart = function () {
    // depends(id, dependsOn, dependsOnValue, negate, dependsOnIsId, hideFieldOnly
    depends("ExistingDataDetail_AccessTypesDescription", "ExistingDataDetail_ExistingDataAccessTypesOther", "Other", false, true, true);
};

$(function () {
    var existingDataDetailRadioButton = $("input[name=ExistingDataDetail.UseExistingData]"),
        existingDataDetailOwner = $("#ExistingDataDetail_ExistingDataOwner"),
        existingDataDetailAccessTypes = $("input[name=ExistingDataDetail.ExistingDataAccessTypes]"),
        existingDataDetailAccessTypesDescription = $("#ExistingDataDetail_AccessTypesDescription"),
        dataRelationshipSection = $("#data-relationship");
        
    /*** Set up initial states of fields ***/
    if ($(existingDataDetailRadioButton + ":checked").val() === "false") {
        hideAndResetFields();
    }

    /*** Set up event listeners ***/
    $(existingDataDetailRadioButton).click(function () {
        $(existingDataDetailRadioButton + ":checked").val() === "true" ? showFields() : hideAndResetFields();
    });

    function showFields() {
        // Hide dt and dd
        existingDataDetailOwner.parent().prev().show();
        existingDataDetailOwner.parent().show();

        // Hide dt and dd
        existingDataDetailAccessTypes.parent().parent().parent().prev().show();
        existingDataDetailAccessTypes.parent().parent().parent().show();

        dataRelationshipSection.show();
    }

    function hideAndResetFields() {
        existingDataDetailOwner.val("");
        // Hide dt and dd
        existingDataDetailOwner.parent().prev().hide();
        existingDataDetailOwner.parent().hide();

        $("input[name=ExistingDataDetail.ExistingDataAccessTypes]:checked").attr("checked", false);
        
        // Hide dt and dd
        existingDataDetailAccessTypes.parent().parent().parent().prev().hide();
        existingDataDetailAccessTypes.parent().parent().parent().hide();
        
        existingDataDetailAccessTypesDescription.val("");
        // Hide dt and dd
        existingDataDetailAccessTypesDescription.parent().prev().hide();
        existingDataDetailAccessTypesDescription.parent().hide();

        $("input[name=DataRelationshipDetail.RelationshipBetweenExistingAndNewData]:checked").attr("checked", false);
        dataRelationshipSection.hide();
    }
});