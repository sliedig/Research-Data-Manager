$(function () {
    var loaderImg = "<img id='loaderImage' src='/Content/Images/ajax-loader.gif' alt='loading image' />";

    // Perform initial state initialisation
    hideForGrid();
    hideSeoGrid();

    // FoR code addition click listener
    $("#AddForCode").click(function () { addForCode(loaderImg); return false; });

    $("#FieldOfResearchCode").keydown(function (event) {
        if (event.keyCode == 13) {
            addForCode(loaderImg);
            event.preventDefault();
        }
        return true;
    });

    // FoR code deletion click listener
    $("#DeleteForCodes").click(function () {
        $("#fieldsOfResearchGrid input:checked").parents('tr').fadeOut(function () {
            $(this).remove();
            hideForGrid();
        });

        $(this).blur();

        return false;
    });

    // SEO code addition click listener
    $("#AddSeoCode").click(function () { addSeoCode(loaderImg); return false; });

    $("#SocioEconomicObjectiveCode").keydown(function (event) {
        if (event.keyCode == 13) {
            addSeoCode(loaderImg);
            event.preventDefault();
        }
        return true;
    });

    // SEO code deletion click listener
    $("#DeleteSeoCodes").click(function () {
        $("#socioEconomicObjectivesGrid input:checked").parents('tr').fadeOut(function () {
            $(this).remove();
            hideSeoGrid();
        });

        $(this).blur();

        return false;
    });
});

function resetErrors(el, container) {
    el.val("");
    $("form.flow").removeClass("erroneous");
    container.removeClass("invalid");
}

function addErrorMsg(container, msg) {
    $("form.flow").addClass("erroneous");
    container.addClass("invalid");
    container.prepend("<p class='error_text'>" + msg + "</p>");
}

function hideForGrid() {
    if ($("#fieldsOfResearchGrid tbody tr").length == 0) {
        $("#fieldsOfResearch").hide();
    }
}

function showForGrid() {
    if ($("#fieldsOfResearchGrid tbody tr").length > 0) {
        $("#fieldsOfResearch").show();
    }
}

function hideSeoGrid() {
    if ($("#socioEconomicObjectivesGrid tbody tr").length == 0) {
        $("#socioEconomicObjectives").hide();
    }
}

function showSeoGrid() {
    if ($("#socioEconomicObjectivesGrid tbody tr").length > 0) {
        $("#socioEconomicObjectives").show();
    }
}

function addForCode(loaderImg) {
    var el = $("#FieldOfResearchCode");
    var button = $("#AddForCode");
    var container = el.parent();
    var errorMsg = "Field of research code not found, please ensure code is correct.";

    // loop through existing values first.
    var blnContinue = true;
    $("#fieldsOfResearchGrid tbody tr").each(function () {
        if ($(this).find("td:first").text().toLowerCase() == el.val().toLowerCase()) {
            blnContinue = false;
        }
    });
    if (!blnContinue) {
        button.blur();
        resetErrors(el, container);

        return false;
    }

    $(this).after(loaderImg);

    $.ajax({
        url: "/Ajax/GetNewForCode/",
        data: {
            term: $("#FieldOfResearchCode").val()
        },
        type: "POST",
        success: function (data) {
            container.find(".error_text").remove();
            button.blur();

            if (data.length > 0) {
                resetErrors(el, container);

                $("#fieldsOfResearchGrid tbody").append(data);
                showForGrid();
            }
            else {
                addErrorMsg(container, errorMsg);
            }
        },
        error: function () {
            addErrorMsg(container, errorMsg);
        },
        complete: function () {
            $("#loaderImage").fadeOut(function () { $(this).remove(); });
        }
    });
};

function addSeoCode(loaderImg) {
    var el = $("#SocioEconomicObjectiveCode");
    var button = $("#AddSeoCode");
    var container = el.parent();
    var errorMsg = "Socio economic objective code not found, please ensure code is correct.";

    // loop through existing values first.
    var blnContinue = true;
    $("#socioEconomicObjectivesGrid tbody tr").each(function () {
        if ($(this).find("td:first").text().toLowerCase() == el.val().toLowerCase()) {
            blnContinue = false;
        }
    });
    if (!blnContinue) {
        button.blur();
        resetErrors(el, container);

        return false;
    }

    $(this).after(loaderImg);

    $.ajax({
        url: "/Ajax/GetNewSeoCode/",
        data: {
            term: el.val()
        },
        type: "POST",
        success: function (data) {
            container.find(".error_text").remove();
            button.blur();

            if (data.length > 0) {
                resetErrors(el, container);

                $("#socioEconomicObjectivesGrid tbody").append(data);
                showSeoGrid();
            }
            else {
                addErrorMsg(container, errorMsg);
            }
        },
        error: function () {
            addErrorMsg(container, errorMsg);
        },
        complete: function () {
            $("#loaderImage").fadeOut(function () { $(this).remove(); });
        }
    });
}