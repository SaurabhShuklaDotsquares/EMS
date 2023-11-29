/*global jQuery, Global,secureDomain */
(function ($) {

    $('#btn_search').on('click', function () {
        if ($('form').valid()) {
            LoadTimeSheetList();
        }
        return false;
    });

    //$('#btn_search').on('click', function () {
    //    if ($('form').valid()) {
    //        $.get(domain + 'timesheetReport/getTimesheetWeeklyReport', { month: $('#TimeSheetMonth').val(), year: $('#TimeSheetYear').val(), userID: $('#ddl_User').val(), timeSheetType: $('#ddl_timesheetType').val() }, function (data) {
    //            $(".export-btn").hide();
    //            $("#div_Table").html(data);
    //            if (!$("#div_Table tbody .tr-norecord").length > 0) {
    //                $(".export-btn").show();
    //                var _href = $(".export-btn").data("href");
    //                $(".export-btn").attr("href", _href + "?month=" + $('#TimeSheetMonth').val() + "&year=" + $('#TimeSheetYear').val() + "&userID=" + $('#ddl_User').val() + "&timeSheetType=" + $('#ddl_timesheetType').val());
    //            }
    //        });
    //    }
    //    return false;
    //});
    $('#ddl_Pm').on('change', function () {
        $("#ddl_User").html("");
        if ($('#ddl_Pm').val() != '') {
            $.get(domain + 'timesheetReport/getUserPMWise', { pmID: $('#ddl_Pm').val() }, function (data) {
                // clear before appending new list 
                $.each(data, function (i, user) {
                    $("#ddl_User").append(
                        $('<option></option>').val(user.Value).html(user.Text));
                });
            });
        }
        else {
            $("#ddl_User").append(
                      $('<option></option>').val("").html("--User--"));
        }
    });




    $('form').validate({
        highlight: function (element, errorClass, validClass) {
            $(element).removeClass(errorClass);
        }
    });

    function LoadTimeSheetList() {

        var data1 = { month: $('#TimeSheetMonth').val(), year: $('#TimeSheetYear').val(), userID: $('#ddl_User').val(), timeSheetType: $('#ddl_timesheetType').val()};
        
        var timesheetWeeklyReportGrid = new Global.GridHelper('#grid-timesheetWeeklyReport', {
            serverSide: true,
            destroy: true,
            "pageLength": 50,
            "bFilter": false,
            "bSort": false,
            ajax: {
                url: domain + 'TimeSheetReport/GetTimesheetWeeklyReportList',
                type: 'POST',
                data: data1
            },
            order: [[0, 'desc']],
            "columnDefs": [
          { "width": "0%", "targets": 0 },
          { "width": "8%", "targets": 2 },
            ],
            columns: [
                { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
                { name: 'date', data: 'date', width: "12%", title: "Date", sortable: false, searchable: false },
                //{ name: 'user', data: 'user', title: "Name", sortable: false, searchable: false },
                { name: 'hours', data: 'hours', title: "Hours", sortable: false, searchable: false },
                { name: 'description', data: 'description', title: "Description", sortable: false, searchable: false },
                
            ],
            "fnDrawCallback": function (oSettings) {
                if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                    $('.dataTables_paginate').hide();
                }
                else {
                    $('.dataTables_paginate').show();
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
        return timesheetWeeklyReportGrid;

    }
}(jQuery));