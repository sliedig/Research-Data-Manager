$(function () {
    var inputs = ["input[name=ResearchInvolvesExperimentalOrganisms]",
                    "input[name=ResearchInvolvesHumanParticipants]",
                    "input[name=ResearchInvolvesAnimals]",
                    "input[name=ResearchInvolvesSocialScienceDatasets]",
                    "input[name=ResearchInvolvesGeneticManipulation]"],
        ethicsAndSafetyDisclaimer = $("#ethicsAndSafety"),
        ionisingRadiationDisclaimer = $("#ionisingRadiation"),
        ionisingRadiationInput = "input[name=ResearchInvolvesIonisingRadiation]",
        biologicalMaterialsDisclaimer = $("#biologicalMaterials"),
        biologicalMaterialsInput = "input[name=ResearchInvolvesDepositionOfBiologicalMaterials]";

    /*** Set up initial states of fields ***/
    // Hide ethics section unless any first five questions selected
    if (getSelectedCount(inputs) == 0) {
        ethicsAndSafetyDisclaimer.hide();
    }

    if ($(ionisingRadiationInput + ":checked").val() === "false") {
        ionisingRadiationDisclaimer.hide();
    }

    if ($(biologicalMaterialsInput + ":checked").val() === "false") {
        biologicalMaterialsDisclaimer.hide();
    }


    /*** Set up event listeners ***/
    $(inputs[0] + ", " + inputs[1] + ", " + inputs[2] + ", " + inputs[3] + ", " + inputs[4]).click(function () {
        var selectedCount = getSelectedCount(inputs);
        selectedCount > 0 ? ethicsAndSafetyDisclaimer.fadeIn() : ethicsAndSafetyDisclaimer.fadeOut();
        if (selectedCount === 1 && $(this).val() === "true") {
            $('html, body').animate({
                scrollTop: ethicsAndSafetyDisclaimer.offset().top - 5
            }, "slow");
        }
    });

    $(ionisingRadiationInput).click(function () {
        $(ionisingRadiationInput + ":checked").val() === "true" ? ionisingRadiationDisclaimer.fadeIn() : ionisingRadiationDisclaimer.fadeOut();
    });

    $(biologicalMaterialsInput).click(function () {
        $(biologicalMaterialsInput + ":checked").val() === "true" ? biologicalMaterialsDisclaimer.fadeIn() : biologicalMaterialsDisclaimer.fadeOut();
    });
});


function getSelectedCount(selectedInputs) {
    var selectedCount = 0;
    for (var i = 0; i < selectedInputs.length; i++) {
        if ($(selectedInputs[i] + ":checked").val() === "true") {
            selectedCount++;
        }
    }
    return selectedCount;
}