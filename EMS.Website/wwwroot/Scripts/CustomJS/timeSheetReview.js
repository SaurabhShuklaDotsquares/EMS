/*global jQuery, Global,secureDomain */

(function ($) {

    //$(document).ready(function () {
    //    var isChecked = $('#chkAll').is(":checked");
    //    Checked = isChecked;
    //    $(".select-checkbox").each(function (index, obj) {
    //        $(this).prop("checked", isChecked);
    //    });
    //});

    //Get timesheet for specific user
    $(document).delegate('#ddl_User', 'change', function () {
        LoadTimeSheetList();
    });

    //Select checkbox
    $(document).delegate("#chkAll", 'click', function () {
        var isChecked = $(this).is(":checked");
        $(".select-checkbox").each(function (index, obj) {
            $(this).prop("checked", isChecked);
        });
    });

    //Mark Verify timesheet
    $(document).delegate("#btn_verified", 'click', function () {
        var userId = $('#ddl_User').val();
        var selectedTimesheetIds = [];
        $(".select-checkbox").each(function (index, obj) {
            if ($(this).is(":checked")) {
                selectedTimesheetIds.push($(this).val());
            }
        });

        if (selectedTimesheetIds.length > 0) {

            $.confirm({
                title: 'Confirm!',
                content: 'Are you sure want to update this record?',
                buttons: {
                    Confirm: {
                        btnClass: 'btn-orange',
                        action: function () {
                            $.post(domain + 'TimeSheetReport/UpdateTimesheetStatus', { userID: $('#ddl_User').val(), userTimesheetIds: selectedTimesheetIds }, function (data) {
                                if (data.success) {
                                    LoadTimeSheetList();
                                    CustomAlerts.success("Success !!!", data.message);
                                } else {
                                    $.alert({
                                        title: 'Alert!',
                                        content: data.message,
                                    });
                                    return false;
                                }
                            });
                        }
                    },
                    cancel: function () {
                        $(".select-checkbox").each(function (index, obj) {
                            $(this).prop("checked", false);
                        });
                        $(".chkAll").each(function (index, obj) {
                            $(this).prop("checked", false);
                        });
                    },
                }
            });
            
        } else {
            $.alert({
                title: 'Alert!',
                content: 'Please select atleast one record.',
            });
            return false;
        }

    });
}(jQuery));

//function LoadTimeSheetList() {
//    $.get(domain + 'TimeSheetReport/GetTimeSheetList', { userID: $('#ddl_User').val() }, function (data) {
//        $("#btn_verified").hide();
//        $("#div_Table").html(data);
//        if (!$("#div_Table tbody .tr-norecord").length > 0) {
//            $("#btn_verified").show();
//        }
//    });
//}


//Data bind in TimesheetReview table// created by hemendra
function LoadTimeSheetList() {
    var data1 = { userID: $('#ddl_User').val() };
    var timesheetReviewGrid = new Global.GridHelper('#grid-timesheetReview', {
        serverSide: true,
        destroy: true,
        "pageLength": 25,
        "bFilter": false,
        ajax: {
            url: domain + 'TimesheetReport/GetTimesheetReviewList',
            type: 'POST',
            data: data1
        },
        order: [[0, 'desc']],
        "columnDefs": [
      { "width": "2%", "targets": 1 },
      { "width": "10%", "targets": 2 },
        { "width": "20%", "targets": 3 },
      { "width": "15%", "targets": 4 },
        { "width": "10%", "targets": 5 },
      { "width": "30%", "targets": 6 },
        { "width": "8%", "targets": 7 }
        ],
        columns: [

            { name: 'userTimeSheetID', data: 'userTimeSheetID', title: "ID", sortable: false, searchable: false, visible: false },
            { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
            { name: 'addedDate', data: 'addedDate', width: "12%", title: "Date", sortable: false, searchable: false },
            { name: 'projectName', data: 'projectName', title: "Project", sortable: false, searchable: false },
            { name: 'developerName', data: 'developerName', title: "Virtual Developer", sortable: false, searchable: false },
            { name: 'workHours', data: 'workHours', title: "Hours", sortable: false, searchable: false },
            { name: 'description', data: 'description', title: "Description", sortable: false, searchable: false },
            {
                name: 'action', data: null, title: "Verify All " + "<input id='chkAll' type='checkbox' class='chkAll'/>", className: "text-center", sortable: false, searchable: false, render: function (data, type, row, meta) {
                   
                    return "<input type='checkbox' class='select-checkbox icheck' value='" + row.userTimeSheetID + "'/>"

                }
            },
        ],
        "fnDrawCallback": function (oSettings) {
            if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                $('.dataTables_paginate').hide();
            }
            else {
                $('.dataTables_paginate').show();
            }
            if (oSettings.fnRecordsDisplay() > 0) {
                $("#btn_verified").show();
            }
            else {
                $("#btn_verified").hide();
            }
            $('.pagination .active a').css('background-color', '#e99701');
            $('.pagination .active a').css('border-color', '#e99701');
        },
        "fnRowCallback": function (row, data, index) {
            if (data.reviewBy == "Pending") {
                $('td', row).css('background-color', '#f7efef');
            }
            else {
                $('td', row).css('background-color', '');
            }
        }
    });
    return timesheetReviewGrid;
}
