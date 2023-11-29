/*global jQuery, Global,secureDomain */
(function ($) {

    clearSearch();

    //Get timesheet acccording  to filter
    $("#alertbox").hide();
    $(document).delegate('#btn_search', 'click', function () {
        console.log("select");
        if ($('#txt_dateFrom').val() == "" && $('#txt_dateTo').val() == "" && $('#ddl_project').val() == "0" && $('#ddl_virtualdeveloper').val() == "0" && $('#ddl_user').val() == "0") {

            //$.post(domain + 'TimeSheetReport/AlertMessage', { messagetype: false, message: "Please Select Atleast One Selection Creteria ..." }, function (data) {
            //    if (data.Success) {
            //        location.reload();
            //        return false;
            //    }
            //});
            CustomAlerts.info("Information !!!", "Please Select Atleast One Selection Creteria ...");
            return false;
        }
        //else if ($('#ddl_virtualdeveloper').val() == "0" && $('#ddl_user').val() == "0") {
        //    $.post(domain + 'TimeSheetReport/AlertMessage', { messagetype: false, message: "Please Select User Or Virtual Developer (Any One) ...." }, function (data) {
        //        if (data.Success) {
        //            location.reload();
        //            return false;
        //        }
        //    });
        //    //CustomAlerts.info("Information !!!", "Please Select User Or Virtual Developer (Any One) ....");
        //    //return false;
        //}
        else {
            console.log("-2-");
            //LoadTimeSheetList();
            loadfeedbackreason();
        }
    });

    $("#modal-edit-projectuserreport").on('loaded.bs.modal', function () {
        if ($('#WorkHoours').val() == "00:00") {
            $('#btnsubmit').prop("disabled", true);
        }
        var date = new Date();
        date.setDate(date.getDate());

        var developerId = $("#hdnDeveloperid").val()
        getVirtualDeveloperList(developerId);

        $(document).delegate('#ProjectId', 'change', function () {
            getVirtualDeveloperList();
        });

        $('.clsdatepicker').datepicker({
            keyboardNavigation: false,
            forceParse: false,
            toggleActive: false,
            autoclose: true,
            maxDate: date,
            dateFormat: 'dd/mm/yy'
        }).inputmask("dd/mm/yyyy", { "placeholder": "dd/mm/yyyy" });

        $(".clstimepicker").mask("99:99", { clearIncomplete: false });
        $(".clstimepicker").blur(function () {
            var currentMask = '';
            var arr = $(this).val().split('');
            if (arr[1] == '_' && arr[0] != '_') {
                arr[1] = arr[0];
                arr[0] = '0';
            }

            if (arr[4] == '_' && arr[3] != '_') {
                arr[4] = arr[3];
                arr[3] = '0';
            }

            $(arr).each(function (index, value) {
                if (value == '_')
                    arr[index] = '0';
                currentMask += arr[index];
            });
            var time = currentMask.split(':');
            if (time[0] == "" || time[0] == 'undefined' || time[0] == '__' || parseInt(time[0]) > 23 && (time[1] == "" || time[1] == 'undefined' || time[1] == '__' || parseInt(time[1]) > 59)) {
                alert("Please enter correct working time");
            }
            else {
                if (time[0] == "" || time[0] == 'undefined' || time[0] == '__' || parseInt(time[0]) > 23)
                    alert("Please enter correct working hours");

                if (time[1] == "" || time[1] == 'undefined' || time[1] == '__' || parseInt(time[1]) > 59)
                    alert("Please enter correct working time");

                if (time[0] + ":" + time[1] == "00:00")
                    alert("Please enter working time");
            }
            var newVal = time[0] + ":" + time[1];
            if (newVal.indexOf("undefined") != -1) {
                newVal = "00:00";
            }
            $(this).val(newVal);
            if (newVal != "00:00") {
                $('#btnsubmit').prop("disabled", false);
            } else {
                $('#btnsubmit').prop("disabled", true);
            }
            //$(".clstimepicker").val(newVal);
        });

        $(document).delegate('#btnsubmit', 'click', function () {
            var form = $('#frm-edit-projectuserreport form');
            if (form.valid()) {
                $.post(domain + 'TimeSheetReport/EditReport', { addedDate: $('#AddedDate').val(), workHoours: $('#WorkHoours').val(), projectID: $('#ProjectId').val(), developerId: $('#DeveloperId').val(), userTimeSheetID: $('#UserTimeSheetID').val(), description: $('#Description').val() }, function (data) {
                    if (data.success) {
                        $('#modal-edit-projectuserreport').modal('hide');
                        LoadTimeSheetList();
                        CustomAlerts.success("Success !!!", data.message);
                    } else {
                        $('#modal-edit-projectuserreport').modal('hide');
                        CustomAlerts.error("Error !!!", data.message);
                    }
                });
            }
            else { return false; }
        });

        //formAddEdit = new Global.FormHelper($("#frm-edit-projectuserreport form"), { updateTargetId: "validation-summary" });
        //return formAddEdit;

    }).on('hidden.bs.modal', function () {
        $(this).removeData('bs.modal');
    });

    // Bind datepicker from date
    $("#txt_dateFrom").datepicker({
        dateFormat: "dd/mm/yy",
        onClose: function (selectedDate) {
            $("#txt_dateTo").datepicker("option", "minDate", selectedDate);
            $("#txt_dateTo").datepicker("option", "defaultDate", selectedDate);
        }
    });
    // Bind datepicker dateTO
    $("#txt_dateTo").datepicker({
        defaultDate: "+1w",
        dateFormat: "dd/mm/yy",
        numberOfMonths: 1,
        onClose: function (selectedDate) {
            $("#txt_dateFrom").datepicker("option", "maxDate", selectedDate);
        }
    });

    $(".clrFilterDate").click(function () {
        $(this).prev('input[type=text]').val('')
    });

    // Bind project,virtual developer and developer according to pm.
    $(document).delegate('#ddl_user', 'change', function () {
        $("#dk_dl_project").html("");
        if ($('#ddl_user').val() != '0') {
            $.get(domain + 'timesheetReport/getprojectpmwise', { pmID: $('#ddl_user').val() }, function (data) {
                console.log(data);
                $.each(data.projectList, function (i, user) {
                    $("#dk_dl_project").append(
                        $('<option></option>').val(user.value).html(user.text));
                });
            });
        }
        else {
            $("#dk_dl_project").append($('<option></option>').val("0").html("-Project-"));
        }
    });

    //$(document).delegate('#ddl_project', 'change', function () {
    //    $("#ddl_virtualdeveloper").html("");
    //    var DDLItems = "";
    //    if ($('#ddl_project').val() != '0') {
    //        $.get(domain + 'timesheetReport/GetVirtualDevelperByProjectID', { projectId: $('#ddl_project').val() }, function (data) {
    //            console.log(data)
    //            if (data != null) {
    //                $.each(data.virtualDeveloperList, function (i, developer) {
    //                    DDLItems += "<option value='" + developer.value + "'>" + developer.text + "</option>";
    //                });
    //                $("#ddl_virtualdeveloper").html(DDLItems);
    //            }
    //            else {
    //                DDLItems += "<option value='" + "0" + "'>" + "-Virtual Developer-" + "</option>";
    //                $("#ddl_virtualdeveloper").html(DDLItems);
    //                //$("#DeveloperId").append($('<option></option>').val("0").html("-Virtual Develope-"));
    //            }
    //        });
    //    }
    //    else {
    //        DDLItems += "<option value='" + "0" + "'>" + "-Virtual Developer-" + "</option>";
    //        $("#ddl_virtualdeveloper").html(DDLItems);
    //    }
    //});

    //delete selected timesheet
    $(document).delegate("#btn_delete", 'click', function () {
        var selectedTimesheetIds = [];
        $(".select-checkbox").each(function (index, obj) {
            if ($(this).is(":checked")) {
                selectedTimesheetIds.push($(this).val());
            }
        });

        if (selectedTimesheetIds.length > 0) {
            var result = false;
            $.confirm({
                title: 'Confirm!',
                content: 'Are you sure want to delete this record?',
                buttons: {
                    Confirm: {
                        btnClass: 'btn-orange',
                        action: function () {
                            $.post(domain + 'TimeSheetReport/DeleteTimesheet', { userTimesheetIds: selectedTimesheetIds }, function (data) {
                                if (data.success) {
                                    //location.reload();
                                    LoadTimeSheetList();
                                    CustomAlerts.success("Success !!!", data.message);
                                } else {
                                    $.alert({
                                        title: 'Alert!',
                                        content: data.message,
                                    });
                                    //CustomAlerts.error("Error !!!", data.Message);
                                }
                            });
                        }
                    },
                    cancel: function () {
                        $(".select-checkbox").each(function (index, obj) {
                            $(this).prop("checked", false);
                        });
                    },
                    //somethingElse: {
                    //    text: 'Something else',
                    //    btnClass: 'btn-blue',
                    //    keys: ['enter', 'shift'],
                    //    action: function () {
                    //        $.alert('Something else?');
                    //    }
                    //}
                }
            });

            //var result = confirm("Are you sure want to delete this record?");
            //if (result == true) {

            //}
            //else {
            //    return false;
            //}
        } else {
            window.scrollTo(0, 0);
            $.alert({
                title: 'Alert!',
                btnClass: 'btn-blue',
                content: 'Please select atleast one record.',
            });
            return false;
            //window.scrollTo(0, 0);
            //setTimeout(function () {
            //    CustomAlerts.error("Error !!!", "Please select atleast one record.");
            //}, 100);

        }
    });
}(jQuery));
function loadfeedbackreason() {
    $('#grid-timesheetGraph').html('');
    $('#grid-timesheetGraph').html('<table id="grid-timesheet" class="display postable table table-stats table-condensed"></table>');
    var data1 = { dateFrom: $('#txt_dateFrom').val(), dateTo: $('#txt_dateTo').val(), porjectID: $('#dk_dl_project').val(), virtualDeveloperID: $('#ddl_virtualdeveloper').val(), userID: $('#ddl_user').val() };
    //console.log(data1);
    $.ajax({
        type: "POST",
        url: domain + "timesheetreport/GetProjectUserGraphReportList",
        data: data1,
        success: function (data) {
            var itemArray = [];

            $.each(data, function (i, item) {
                if (item.totalTime > 0) {
                    itemArray.push({
                        name: item.virtualCategoryName,
                        y: item.totalTime,
                        color: item.color,
                        itemTotalTime: item.itemTotalTime,
                        totalWork: item.totalWork
                    });
                }
            });
            //var chart = new Highcharts.Chart(options);
            var chart = Highcharts.chart('container-chart', {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: "Timesheet Report" + " (" + data[0].totalWork + ")",
                    style: { "font-weight": "bold" }
                },
                //tooltip: {
                //    //pointFormat: '{series.name}: <b>{point.percentage:.1f}</b>'
                //    pointFormat: '{series.name}: <b>{point.y:.1f}</b>'
                //},
                tooltip: {
                    formatter: function () {
                        return '<b>' + this.point.name + '</b>: ' + this.y + "%" + " (" + this.point.itemTotalTime + ")";
                    }
                },
                plotOptions: {
                    pie: {
                        size: '100%',
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            style: {
                                textOverflow: 'clip'
                            },
                            formatter: function () {
                                if (this.y != 0) {
                                    return '<b>' + this.point.name + '</b>: ' + this.y + '%' + ' (' + this.point.itemTotalTime + ')';
                                } else {
                                    return null;
                                }
                            }
                        },
                        point: {
                            events: {
                                click: function () {
                                    console.log(this);
                                    LoadTimeSheetList(this.name);
                                }
                            }
                        }
                    }
                },
                series: [{
                    data: itemArray
                }]
            },
                function (chart) { // on complete
                    if (itemArray.length == 0) {
                        chart.renderer.text('No Data Available, try Another Search.', 140, 395)
                            .css({
                                color: 'Red',
                                fontSize: '15px'
                            })
                            .add();
                    }
                });
            $('.highcharts-credits').css("display", "none");
        }
    });
}

function LoadTimeSheetList(catName) {
    setTimeout(function () {
        $(".export-btn").hide();
        $("#div_delete").hide();
    }, 100);
    var data1 = { dateFrom: $('#txt_dateFrom').val(), dateTo: $('#txt_dateTo').val(), porjectID: $('#dk_dl_project').val(), virtualDeveloperID: $('#ddl_virtualdeveloper').val(), userID: $('#ddl_user').val(), virtualDevloperCategory: catName };

    var timesheetGrid = new Global.GridHelper('#grid-timesheet', {
        serverSide: true,
        destroy: true,
        "pageLength": 1000,
        "bFilter": false,
        "bLengthChange": false,
        ajax: {
            url: domain + 'TimeSheetReport/GetProjectUserReportListByGraphClick',
            type: 'POST',
            data: data1
        },
        "columnDefs": [
            { "width": "5%", "targets": 0 },
            { "width": "5%", "targets": 1 },
            { "width": "15%", "targets": 2 },
            { "width": "30%", "targets": 3 },
            //{ "width": "17%", "targets": 5 },
            //{ "width": "7%", "targets": 3 },
            { "width": "50%", "targets": 4 },
            //{ "width": "15%", "targets": 8 },
        ],
        columns: [
            //{ name: 'userTimeSheetID', data: 'userTimeSheetID', title: "ID", sortable: false, searchable: false, visible: false },
            { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
            { name: 'addedDate', data: 'addedDate', width: "20%", title: "Date", sortable: false, searchable: false },
            {
                name: 'name', data: 'name', title: "Name", sortable: false, searchable: false
            },
            {
                name: 'projectName', data: 'projectName', title: "Project", sortable: false, searchable: false, render: function (data, type, row, meta) {
                    return row.projectName + "<br> <b>" + row.developerName + " (" + row.workHours + ")</b>";
                }
            },
            //{ name: 'developerName', data: 'developerName', title: "Virtual Developer", sortable: false, searchable: false },
            //{ name: 'workHours', data: 'workHours', title: "Hours", sortable: false, searchable: false },
            { name: 'description', data: 'description', title: "Description", sortable: false, searchable: false },
            //{ name: 'reviewBy', data: 'reviewBy', title: "Review By", sortable: false, searchable: false },
            //{
            //    name: 'action', data: null, title: "Delete", className: "text-center", sortable: false, searchable: false, visible: $('#hdn_PM').val() == "1" ? true : false, render: function (data, type, row, meta) {

            //        if (meta.row == 0) {
            //            if ($('#hdn_PM').val() == "1") {
            //                setTimeout(function () {
            //                    $("#div_delete").show();
            //                }, 100);

            //            }
            //            setTimeout(function () {
            //                $(".export-btn").show();
            //            }, 10);

            //            var _href = $(".export-btn").data("href");
            //            $(".export-btn").attr("href", _href + "?dateFrom=" + $('#txt_dateFrom').val() + "&dateTo=" + $('#txt_dateTo').val() + "&porjectID=" + $('#ddl_project').val() + "&virtualDeveloperID=" + $('#ddl_virtualdeveloper').val() + "&userID=" + $('#ddl_user').val());
            //        }
            //        if ($('#hdn_PM').val() == "1") {
            //            return "<input type='checkbox' class='select-checkbox' value='" + row.userTimeSheetID + "'/>"
            //            return data;
            //        }
            //        else {
            //            return "";
            //        }

            //    }
            //},
            //{
            //    name: 'action', data: null, title: "Action", className: "text-center", sortable: false, searchable: false, visible: $('#hdn_PM').val() == "1", render: function (data, type, row, meta) {

            //        if ($('#hdn_PM').val() == "1") {
            //            return '<a data-toggle="modal"  data-target="#modal-edit-projectuserreport" href="' + domain + 'TimesheetReport/EditReport/' + row.userTimeSheetID + '" class="trans-btn"><i class="fa fa-edit"></i></a>';
            //        }
            //        return '';
            //    }
            //}

        ],
        "fnDrawCallback": function (oSettings) {
            if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                $('.dataTables_paginate').hide();
            }
            else {
                $('.dataTables_paginate').show();
            }
            $('.pagination .active a').css('background-color', '#ffffff');
            $('.pagination .active a').css('border-color', '#e99701');
        },
        "fnRowCallback": function (row, data, index) {
            if (data.reviewBy == "Pending") {
                $('td', row).css('background-color', '#ffffff');
            }
            else {
                $('td', row).css('background-color', '');
            }
        },
        "fnInitComplete": function (oSettings, json) {
            if (json.isWorkingHoursExist) {
                $('.workingHours').show();
                $("#totalWorkingHours").html(json.totalWorkingHours);
            }
            else {
                $('.workingHours').hide();
            }
        }

    });
    return timesheetGrid;

}

function clearSearch() {
    $('#txt_dateFrom').val('');
    $('#txt_dateTo').val('');
    $('#ddl_project').val('0');
    $('#ddl_virtualdeveloper').val('0');
    $('#ddl_user').val('0');
}

function getVirtualDeveloperList(developerId) {
    $("#DeveloperId").html("");
    var DDLItems = "";
    if ($('#ProjectId').val() != '') {
        $.get(domain + 'timesheetReport/GetVirtualDevelperByProjectID', { projectId: $('#ProjectId').val(), uId: $('#Uid').val() }, function (data) {

            if (data != null) {
                $.each(data.virtualDeveloperList, function (i, developer) {

                    //$("#DeveloperId").append(
                    //    $('<option></option>').val(developer.Value).html(developer.Text));
                    DDLItems += "<option value='" + developer.value + "'>" + developer.text + "</option>";

                });
                $("#DeveloperId").html(DDLItems);
                $("#DeveloperId").val(developerId);
            }
            else {
                DDLItems += "<option value='" + "" + "'>" + "-Virtual Developer-" + "</option>";
                $("#DeveloperId").html(DDLItems);
                //$("#DeveloperId").append($('<option></option>').val("0").html("-Virtual Develope-"));
            }
        });
    }
    else {
        DDLItems += "<option value='" + "" + "'>" + "-Virtual Developer-" + "</option>";
        $("#DeveloperId").html(DDLItems);
        //$("#DeveloperId").append($('<option></option>').val("0").html("-Virtual Develope-"));
    }

}
