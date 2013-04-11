$(function () {
    var isApproval = $("#IsForApproval").length != 0 && $("#IsForApproval").val().toLowerCase() == "true" ? true : false;
    var loaderImg = "<img id='loaderImage' src='/Content/Images/ajax-loader.gif' alt='loading image' />";

    // Perform initial state initialisation
    hideUrdmsUsersGrid();
    hideNonUrdmsUsersGrid();
    $("#FindUserId").attr("autocomplete", "off");
    $("#NonUrdmsNewUserName").attr("autocomplete", "off");

    // URDMS user addition click listener
    $("#AddUrdmsUser").click(function () { addUrdmsUser(isApproval, loaderImg); return false; });

    // URDMS user "enter" capture to prevent default form submission on the wrong fields
    $("#FindUserId").keydown(function (event) {
        if ((event.keyCode == 13)) {
            addUrdmsUser(isApproval, loaderImg);
            event.preventDefault();
        }
        return true;
    });

    // URDMS user deletion click listener
    $("#DeleteUrdmsUser").click(function () {
        $("#urdmsUsersGrid input:checked").parents('tr').fadeOut(function () {
            $(this).remove();
            hideUrdmsUsersGrid();
        });

        $(this).blur();

        return false;
    });

    // Non URDMS user addition click listener
    $("#AddNonUrdmsUser").click(function () { addNonUrdmsUser(loaderImg); return false; });

    // Non URDMS user "enter" capture to prevent default form submission on the wrong fields
    $("#NonUrdmsNewUserName").keydown(function (event) {
        if ((event.keyCode == 13)) {
        	addNonUrdmsUser(loaderImg);
            event.preventDefault();
        }
        return true;
    });

    // Non URDMS user deletion click listener
    $("#DeleteNonUrdmsUser").click(function () {
        $("#nonUrdmsUsersGrid input:checked").parents('tr').fadeOut(function () {
            $(this).remove();
            hideNonUrdmsUsersGrid();
        });

        $(this).blur();

        return false;
    });
});

function showUrdmsUsersGrid() {
    if ($("#urdmsUsersGrid tbody tr").length > 0) {
        $("#urdmsUsers").show();
    }
}

function hideUrdmsUsersGrid() {
    if ($("#urdmsUsersGrid tbody tr").length == 0) {
    	$("#urdmsUsers").hide();
    }
}

function showNonUrdmsUsersGrid() {
    if ($("#nonUrdmsUsersGrid tbody tr").length > 0) {
        $("#nonUrdmsUsers").show();
    }
}

function hideNonUrdmsUsersGrid() {
    if ($("#nonUrdmsUsersGrid tbody tr").length == 0) {
        $("#nonUrdmsUsers").hide();
    }
}

function addErrorMsg(container, msg) {
    $("form.flow").addClass("erroneous");
    container.addClass("invalid");
    container.removeClass("inlineField");
    container.prepend("<p class='error_text'>" + msg + "</p>");
}

function resetErrors(el, container) {
    el.val("");
    $("form.flow").removeClass("erroneous");
    container.removeClass("invalid");
    container.addClass("inlineField");
}

function addUrdmsUser(isApproval, loaderImg) {
    var el = $("#FindUserId");
    var button = $("#AddUrdmsUser");
    var container = el.parent();
    var errorMsg = isApproval ? "URDMS user not found or attempted to add manager, please ensure the staff id is correct."
        : "URDMS user not found or attempted to add principal investigator, please ensure the staff id is correct.";

    // loop through existing values first.
    var blnContinue = true;
    $("#urdmsUsersGrid tbody tr").each(function () {
        if ($(this).find("td:first").text().toLowerCase() == el.val().toLowerCase()) {
            blnContinue = false;
        }
    });
    if (!blnContinue) {
        button.blur();
        resetErrors(el, container);

        return false;
    }

    button.after(loaderImg);

    var dataPayLoad = { term: el.val() };
    var url = "/Ajax/GetNewUrdmsUser/";
    if (isApproval) {
        url = "/Ajax/GetNewUrdmsUserForApproval/";
        dataPayLoad.dataCollectionId = $("#Id").val();
    }
    else {
        dataPayLoad.userType = button.attr('class');
    }

    $.ajax({
        url: url,
        data: dataPayLoad,
        type: "POST",
        success: function (data) {
            container.find(".error_text").remove();
            button.blur();

            if (data.length > 0) {
                resetErrors(el, container);

                $("#urdmsUsersGrid tbody").append(data);
                showUrdmsUsersGrid();
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

function addNonUrdmsUser(loaderImg) {
    var el = $("#NonUrdmsNewUserName");
    var button = $("#AddNonUrdmsUser");
    var container = el.parent();
    var errorMsg = "There was an error adding the non-URDMS user, please try again.";

    // loop through existing values first.
    var blnContinue = true;
    $("#nonUrdmsUsersGrid tbody tr").each(function () {
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
        url: "/Ajax/GetNewNonUrdmsUser/",
        data: {
            term: el.val(),
            userType: button.attr('class')
        },
        type: "POST",
        success: function (data) {
            container.find(".error_text").remove();
            button.blur();

            if (data.length > 0) {
                resetErrors(el, container);

                $("#nonUrdmsUsersGrid tbody").append(data);
                showNonUrdmsUsersGrid();
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