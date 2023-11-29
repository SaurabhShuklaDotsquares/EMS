/*global jQuery, Global,secureDomain */
(function ($) {

    var config = {
        '.chosen-select': {},
        '.chosen-select-deselect': { allow_single_deselect: true },
        '.chosen-select-no-single': { disable_search_threshold: 10 },
        '.chosen-select-no-results': { no_results_text: 'Oops, nothing found!' },
        '.chosen-select-width': { width: "95%" }
    }
    for (var selector in config) {
        $(selector).chosen(config[selector]);
    }

    var away = false;

    //$("#modal-projectStatus").on('hidden.bs.modal', function (e) {
    //    $(this).removeData('bs.modal');
    //});

    $('#pbGo').click(function () {
        var id = null;
        var projectName = null;
        var isAdditional = false;
        var isManagingProject = false;

        if ($("#rdo_working").is(':checked')) {
            id = $("#cbProjectsList").val();
            projectName = $("#cbProjectsList option:selected").text();
        }
        else if ($("#rdo_additional").is(':checked')) {
            id = $("#cbProjectsListAdditional").val();
            projectName = $("#cbProjectsListAdditional option:selected").text();
            isAdditional = true;
        }
        else if ($("#rdo_managingproject").is(':checked')) {
            id = $("#ManagingProject").val().join();
            isManagingProject = true;
        }

        if (id == null || id == "-1") {
            alert("Please select any project from list or choose another option.");
            return false;
        }

        $("#ProgressBar_Status").show();

        var data = {
            id: id,
            text: projectName,
            IsAdditional: isAdditional,
            IsManagingProject: isManagingProject,
        }

        GoClick(data, 500);
    });

    $('#pbFree').click(function () {
        var currentUserPMUId = $("#currentUserPMUID").val();
        var ashishTeamPMUId = $("#AshishTeamPMUID").val();


        if ($("#txtFree").val().trim() == '') {
            alert("Please enter the text what you are doing.");
            return false;
        }

        if (currentUserPMUId == ashishTeamPMUId) {
            //in case of ashish team login

            $.confirm({
                columnClass: 'large',
                title: '<b>Confirm!</b>',
                content: '<div style="font-size:16px">Make sure you are not working on any hold/completed project.</br> You should have approval request if you are working, please ask your TL/BA.</br> Don\'t proceed if you don\'t have <b>ADDITIONAL REQUEST APPROVAL</b> email in your inbox.  </div>',
                buttons: {
                    Confirm: {
                        btnClass: 'btn-orange',
                        action: function () {

                            $("#ProgressBar_Status").show();
                            if (away == true) {
                                var data = {
                                    id: null,
                                    text: null,
                                    FreeProject: $("#txtFree").val(),
                                    IsAway: true
                                }

                                $.post(domain + "user/SelectProject", data, function (result) {
                                    if (result != null) {
                                        $("#projectstatus").html('(' + result.userstatus + ')');
                                        $("#projectName").html(result.projectname);

                                        $("#pbAway").attr("disabled", true);
                                        $("#pbAvailable").attr("disabled", false);
                                        $("#pbGo").attr("disabled", false);
                                        $("#pbExit").attr("disabled", false);

                                        $(".available").html('[Away]');
                                        $(".available").css("forecolor", "LightGreen");

                                        // Available and away image changes

                                        $("#pbAvailable").attr('src', 'images/popup/available_Btn.png');
                                        $("#pbAway").attr('src', 'images/popup/dis_away_btn.png');

                                        $('#modal-projectStatus').modal('toggle');
                                    }
                                    $("#ProgressBar_Status").hide();
                                });
                            }
                            else {
                                var data1 = {
                                    id: null,
                                    text: null,
                                    FreeProject: $("#txtFree").val(),
                                    IsFree: true
                                }

                                $.post(domain + "user/SelectProject", data1, function (result) {
                                    if (result != null) {
                                        $("#projectstatus").html('(' + result.userstatus + ')');
                                        $("#projectName").html(result.projectname);

                                        $("#pbAway").attr("disabled", false);
                                        $("#pbAvailable").attr("disabled", true);
                                        $("#pbGo").attr("disabled", true);
                                        $("#pbExit").attr("disabled", false);

                                        $(".available").html('[Available]');
                                        $(".available").css("forecolor", "LightGreen");

                                        //----------------
                                        $('#modal-projectStatus').modal('toggle');
                                    }
                                    $("#ProgressBar_Status").hide();
                                });
                            }
                        }



                    },
                    cancel: function () {

                    },
                }
            });

        }
        else {
            //in case of other team login

            $("#ProgressBar_Status").show();
            if (away == true) {
                var data = {
                    id: null,
                    text: null,
                    FreeProject: $("#txtFree").val(),
                    IsAway: true
                }

                $.post(domain + "user/SelectProject", data, function (result) {
                    if (result != null) {
                        $("#projectstatus").html('(' + result.userstatus + ')');
                        $("#projectName").html(result.projectname);

                        $("#pbAway").attr("disabled", true);
                        $("#pbAvailable").attr("disabled", false);
                        $("#pbGo").attr("disabled", false);
                        $("#pbExit").attr("disabled", false);

                        $(".available").html('[Away]');
                        $(".available").css("forecolor", "LightGreen");

                        // Available and away image changes

                        $("#pbAvailable").attr('src', 'images/popup/available_Btn.png');
                        $("#pbAway").attr('src', 'images/popup/dis_away_btn.png');

                        $('#modal-projectStatus').modal('toggle');
                    }
                    $("#ProgressBar_Status").hide();
                });
            }
            else {
                var data1 = {
                    id: null,
                    text: null,
                    FreeProject: $("#txtFree").val(),
                    IsFree: true
                }

                $.post(domain + "user/SelectProject", data1, function (result) {
                    if (result != null) {
                        $("#projectstatus").html('(' + result.userstatus + ')');
                        $("#projectName").html(result.projectname);

                        $("#pbAway").attr("disabled", false);
                        $("#pbAvailable").attr("disabled", true);
                        $("#pbGo").attr("disabled", true);
                        $("#pbExit").attr("disabled", false);

                        $(".available").html('[Available]');
                        $(".available").css("forecolor", "LightGreen");

                        //----------------
                        $('#modal-projectStatus').modal('toggle');
                    }
                    $("#ProgressBar_Status").hide();
                });
            }
        }








    });

    $('#pbExit').click(function () {
        var r = confirm('Are you sure you want to leave for the day.')
        if (r == true) {
            $("#ProgressBar_Status").show();

            var data = {
                id: null,
                text: null,
                IsExit: true
            }

            $.post(domain + "user/SelectProject", data, function (result) {
                if (result != null) {
                    $("#projectstatus").html('(' + result.userstatus + ')');
                    $("#projectName").html(result.projectname);

                    $("#pbAway").attr("disabled", false);
                    $("#pbAvailable").attr("disabled", true);
                    $("#pbExit").attr("disabled", true);

                    $(".available").html('');
                    $('#modal-projectStatus').modal('toggle');
                }
                $("#ProgressBar_Status").hide();
            });
        }
    });

    $('#dialogfree i.fa-close').click(function () {
        $("#rdo_working").prop('checked', true).change();
    });

    $('input[type=radio][name=workstatus]').change(function () {

        var status = $('input[type=radio][name=workstatus]:checked')[0];

        if (status.value == 'working') {
            $("#dialogfree").slideUp();
            $("#cbProjectsList_chosen").show();
            $("#cbProjectsListAdditional_chosen").hide();
            $("#ManagingProject_chosen").hide();
        }
        else if (status.value == 'free') {
            $("#dialogfree").slideDown();
            $("#cbProjectsList_chosen").hide();
            $("#cbProjectsListAdditional_chosen").hide();
            $("#ManagingProject_chosen").hide();
        }
        else if (status.value == 'additional') {
            $("#dialogfree").slideUp();
            $("#cbProjectsList_chosen").hide();
            $("#cbProjectsListAdditional_chosen").show();
            $("#ManagingProject_chosen").hide();
        }
        else if (status.value == 'managingproject') {
            $("#dialogfree").slideUp();
            $("#cbProjectsList_chosen").hide();
            $("#cbProjectsListAdditional_chosen").hide();
            $("#ManagingProject_chosen").show();
        }
    });

    $("#aclosefree").click(function () {
        $("#dialogfree").slideUp();
        away = false;
    });

    $("#anctopclose").click(function () {
        //  $.colorbox.close();
        $('#modal-projectStatus').modal('toggle');
        away = false;
    });

    $("#ancFree").click(function () {
        $("#dialogfree").slideDown();
    });

    $("#pbAway").click(function () {

        // Temperory comment
        ////$("#dialogfree").slideDown();
        ////away = true;
    });

    $("#cbProjectsList").change(function (e) {
        $("#pbGo").prop("disabled", false);
    });

    $("#rdo_working").change();
    $("#cbProjectsListAdditional_chosen").css('width', '100%');

    if (window.location.pathname.toLowerCase().indexOf("/leads") >= 0) {
        $("#imgtopclose").attr('src', '../images/popup/closeRed.png');
        $("#imgFree").attr('src', '../images/popup/freeBtn.png');
        $("#pbExit").attr('src', '../images/popup/btn_leave.png');
        $("#Image1").attr('src', '../images/popup/closeRed.png');
        $("#btnGo").attr('src', '../images/popup/btnGo.png');
    }

    function showawaymsg() {
        $("#hdnstatus").val("1");
        $("#dialogfree").slideDown();
        return false;
    }

    function ShowProgress(type) {

        if (type == 'exit') {
            var r = confirm('Are you sure you want to leave for the day.')
            if (r == true) {
                $("#ProgressBar_Status").show();
                return true;
            } else {
                return false;
            }
        }
        else if (type == 'go') {
            if (($("#rdo_working").is(':checked') && $("#cbProjectsList").val() == "-1") ||
                ($("#rdo_additional").is(':checked') && $("#cbProjectsListAdditional").val() == "-1")) {
                alert("Please select any project from list or choose another option.");
                return false;
            }
            else if ($("#rdo_free").is(':checked') && $("#txtFree").val().trim() == '') {
                alert("Please enter the text what you are doing.");
                return false;
            }

            $("#ProgressBar_Status").show();
            return true;
        }
        else {
            $("#ProgressBar_Status").show();
            return true;
        }
    }

    function GoClick(data, delay) {
        $.post(domain + "user/SelectProject", data, function (result) {
            if (result) {
                if (result.addAdditionalSupport) {
                    delay = delay || 0;

                    setTimeout(function () {
                        $("#modal-projectStatusAdditional").modal({
                            remote: domain + "project/addadditionalsupport/" + result.projectId,
                            backdrop: 'static',
                            keyboard: false
                        });
                    }, delay);
                }
                else {

                    $("#projectstatus").html('(' + result.userstatus + ')');
                    $("#projectName").html(result.projectname);

                    $("#pbAway").attr("disabled", false);
                    $("#pbAvailable").attr("disabled", true);
                    $("#pbGo").attr("disabled", true);
                    $("#pbExit").attr("disabled", false);

                    $(".available").html('[Available]');
                    $(".available").css("forecolor", "LightGreen");

                    $('#modal-projectStatus').modal('hide');
                }
            }
            $("#ProgressBar_Status").hide();
        });
    }

    function initializeModalWithForm() {
        $("#modal-projectStatusAdditional").on('loaded.bs.modal', function () {
            var modal = $(this);
            var form = new Global.FormHelper(modal.find("form"), {
                updateTargetId: "validation-summary",
                validateSettings: { ignore: "" }
            }, null,
                function (result) {
                    if (result.isSuccess) {
                        $("#modal-projectStatusAdditional").modal('hide');
                        var data = {
                            id: result.data.projectId,
                            text: result.data.projectName,
                            IsAdditional: true
                        }
                        GoClick(data, 500);
                        alert(result.message);
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                    }
                });

            form.find("#EndDate").datepicker({
                dateFormat: "dd/mm/yy",
                minDate: 0,
                onSelect: function (selectedDate) {
                    form.find("#StartDate").datepicker("option", "maxDate", selectedDate);
                }
            });

            form.on("click", "#CancelAdditionalSupport", function () {
                if (confirm("Do you want to cancel Add. Support Request for this Project?")) {
                    $("#modal-projectStatusAdditional").modal('hide');
                }
            });

            form.on("click", "#SubmitAddSupportContinue", function (e) {
                e.preventDefault();
                form.find("#AddDescription").data("rule-required", false).removeClass("error");
                form.submit();
            });

            form.on("click", "#SubmitAddSupport", function (e) {
                e.preventDefault();
                form.find("#AddDescription").data("rule-required", true);
                form.submit();
            });

        }).on('hidden.bs.modal', function () {
            $(this).removeData('bs.modal');
            $(this).find('.modal-content').empty();
        });
    }

    $(function () {
        initializeModalWithForm();
    });
}(jQuery));
