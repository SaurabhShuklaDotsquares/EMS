/*global jQuery, Global,secureDomain */
(function () {

    function ManageLeave() {

        var $this = this, grid, holidayTable;
        var startDate = '', endDate = '';

        function initializeForm() {

            holidayTable = $("#div_holidayType");

            formLogin = new Global.FormValidationReset('#form1');
            $('.searchfilter').on('change', function () {
                if ($('#selfleave').length > 0 && $('#selfleave').prop('checked')) {
                    $('#user').val('');
                    if ($('#pm').length > 0 && $('#pm').val() != '') {
                        $('#pm').val('');
                        GetProjectManagerUsers();
                    }
                    $('#user').attr('disabled', true);
                    $('#pm').attr('disabled', true);
                }
                else {
                    $('#user').removeAttr('disabled');
                    $('#pm').removeAttr('disabled');
                }
                LoadLeavesGrid();
                LoadHolidayTypeTable();
                //grid.ajax.reload(true);
            });
            $('.searchfilterpm').on('change', function () {
                GetProjectManagerUsers();
                if ($('#user') && $('#user').length > 0) {
                    $('#user').val("0");
                }
                LoadLeavesGrid();
                LoadHolidayTypeTable();
            });
        }

        function BindDatePickers() {

            $("#StartDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("#EndDate").datepicker("option", "minDate", selectedDate);

                }
            });

            $("#EndDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("#StartDate").datepicker("option", "maxDate", selectedDate);
                }
            });

            $('#searchDate').on('click', function () {
                LoadLeavesGrid();
                LoadHolidayTypeTable();
            });           
        }

        function GetProjectManagerUsers() {
            var pmid = $("#pm").val();
            if (pmid == '')
                pmid = 0;
            var emp = $("#user");
            $.ajax
                ({
                    url: domain + 'Leave/GetEmployeesByPM',
                    type: 'POST',
                    data: {pmid: pmid},
                    //datatype: 'application/json',
                    //contentType: 'application/json',
                    //data: JSON.stringify({
                    //    pmid: pmid
                    //}),
                    success: function (result) {
                        emp.empty().append('<option value="">All Employee</option>');
                        $.each(result, function () {
                            emp.append($("<option></option>").val(this['value']).html(this['text']));
                        });
                    },
                    error: function (ex) {
                        alert("Whooaaa! Something went wrong.." + ex);
                    },
                });
        }

        function LoadLeavesGrid() {
            startDate = $('#StartDate').val();
            endDate = $('#EndDate').val();
            var data = { user: (($('#selfleave').length > 0 && $('#selfleave').prop('checked')) ? $('#selfleave').val() : $('#user').val()), status: $('#status').val(), leavetype: $('#leavetype').val(), leavecatagory: $('#leavecatagory').val(), pm: $('#pm').val(), startDate: $('#StartDate').val(), endDate: $('#EndDate').val() };
            grid = new Global.GridHelper('#grid-NormalLeaves', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bSort": false,
                "bLengthChange": false,
                ajax: {
                    url: domain + 'Leave/GetLeaves',
                    type: 'POST',
                    data: data
                },               
                "columnDefs": [
                    //{ "width": "0%", "targets": 0 },
                    //{ "width": "5%", "targets": 1 },
                ],
                columns: [
                    { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
                    { name: 'userId', data: 'userId', title: "userId", sortable: false, searchable: false, visible: false },
                    { name: 'leaveId', data: 'leaveId', title: "leaveId", sortable: false, searchable: false, visible: false },
                    { name: 'UserName', data: 'userName', title: "Name", sortable: true, searchable: false },
                    { name: 'StartDate', data: 'startDate', title: "Start Date", sortable: true, searchable: false },
                    { name: 'EndDate', data: 'endDate', title: "End Date", sortable: false, searchable: false },
                    {
                        name: 'LeaveCategory', data: 'leaveCategory', title: "Leave Category", sortable: false, searchable: false, render: function (data, type, full, meta){
                            return '<div style="max-width:200px;">' + data + '</div>';
                        }
                    },
                    {
                        name: 'Reason', data: 'reason', title: "Reason", sortable: false, searchable: false, render: function (data, type, full, meta) {
                            return '<div style="max-width:300px;">' + data + '</div>';
                        }
                    },
                    { name: 'IsHalf', data: 'isHalf', title: "Leave Type", sortable: false, searchable: false },
                    { name: 'Status', data: 'status', title: "Status ", sortable: true, searchable: false },                  
                    { name: 'DateAdded', data: 'dateAdded', title: "Apply Date", sortable: true, searchable: false },
                    { name: 'Modify By', data: 'modifyBy', title: "Modify By", sortable: true, searchable: false },
                    { name: 'Modify Date', data: 'modifyDate', title: "Modify Date", sortable: true, searchable: false },
                    {
                        name: 'action', data: null, title: "Action", className: "text-center", sortable: false, searchable: false, render: function (data, type, full, meta) {
                            if (full.isEdit)
                                return '<a class="trans-btn"  href="' + domain + 'Leave/ManageLeave/' + full.leaveId + '" "><i class="fa fa-edit"></i></a>';
                            else
                                return '';
                        }
                    }
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
                "fnInitComplete": function (oSettings, json) {

                    if (startDate == undefined) {
                        startDate = '';
                    }
                    if (endDate == undefined) {
                        endDate = '';
                    }

                    if (json.totalLeave != undefined && json.sickLeaveCount != undefined && json.holidayLeaveCount!=undefined) {
                        var htmlLeave = '<table class="table table-stats"><tbody><tr><td colspan="3">Name: <b>' + json.holidayLeaveUserName + '</b></td></tr>';
                        htmlLeave = htmlLeave + '<tr><td>From: <b>' + json.startDate + '</b></td><td colspan="2">To: <b>' + json.endDate + '</b></td></tr>';

                        if (json.isIndianUser == "0") {
                            htmlLeave = htmlLeave + '<tr><td>Sick Leave: <b>' + json.sickLeaveCount + '</b></td><td>Holiday Leave: <b>' + json.holidayLeaveCount + '</b></td><td>Pending Leave: <b>' + json.totalLeave + '</b></td></tr>';
                        } else {
                            htmlLeave = htmlLeave + '<tr><td>Holiday Leave: <b>' + json.holidayLeaveCount + '</b></td><td colspan="2" >Pending Leave: <b>' + json.totalLeave + '</b></td></tr>';
                            //htmlLeave = htmlLeave + '<tr><td colspan="3">Pending Leave: <b>' + json.totalLeave + '</b></td></tr>';
                        }

                        //if (json.isIndianUser == "0") {
                        //    htmlLeave = htmlLeave + '<tr><td>Sick Leave: ' + (json.sickLeaveCount == 0 ? '-' : json.sickLeaveCount + (json.sickLeaveCount == 1 ? ' Day' : ' Days')) + '</td><td>Holiday Leave: ' + (json.holidayLeaveCount == 0 ? '-' : json.holidayLeaveCount + (json.holidayLeaveCount == 1 ? ' Day' : ' Days')) + '</td><td>Total Applied Leave: ' + json.totalLeave + (json.totalLeave == 1 ? ' Day' : ' Days') + '</td></tr>';
                        //} else {
                        //    htmlLeave = htmlLeave + '<tr><td colspan="3">Total Applied Leave: ' + json.totalLeave + (json.totalLeave == 1 ? ' Day' : ' Days') + '</td></tr>';
                        //}

                        //var htmlLeave = '<table class="table table-stats"><tbody><tr><td colspan="3">Name:  ' + json.holidayLeaveUserName + '</td></tr>';
                        //htmlLeave = htmlLeave + '<tr><td colspan="3">From:  ' + json.startDate + '  to  ' + json.endDate + '</td></tr>';
                        //if (json.isIndianUser == "0") {
                        //    htmlLeave = htmlLeave + '<tr><td>Sick Leave: ' + (json.sickLeaveCount == 0 ? '-' : json.sickLeaveCount + (json.sickLeaveCount == 1 ? ' Day' : ' Days')) + '</td><td>Holiday Leave: ' + (json.holidayLeaveCount == 0 ? '-' : json.holidayLeaveCount + (json.holidayLeaveCount == 1 ? ' Day' : ' Days')) + '</td><td>Total Applied Leave:  ' + json.totalLeave + (json.totalLeave == 1 ? ' Day' : ' Days') + '</td></tr>';
                        //} else {
                        //    htmlLeave = htmlLeave + '<tr><td colspan="3">Total Applied Leave:  ' + json.totalLeave + (json.totalLeave == 1 ? ' Day' : ' Days') + '</td></tr>';
                        //}

                        htmlLeave = htmlLeave + '</tbody></table>';
                        $('.dataTables_wrapper > div.row:first > div:first').html(htmlLeave);
                    }

                    //if (json.holidayLeaveCount != undefined) {
                    //    var html1 = '<table class="table table-stats"><thead><tr><th class="rowCenterText">Sick Leave</th><th class="rowCenterText">Holiday Leave</th></tr ></thead><tbody><tr><td class="rowCenterText">' + (json.sickLeaveCount == 0 ? '-' : json.sickLeaveCount) + '</td><td class="rowCenterText">' + (json.holidayLeaveCount == 0 ? '-' : json.holidayLeaveCount) + '</td></tr ></tbody></table >';
                    //    $('.dataTables_wrapper > div.row:first > div:first').html(html1);
                    //}

                    var html = json.recordsTotal != 0 ? '<a style="float:right;margin-left: 15px;" class="btn btn-warning" href="' + domain + 'leave/downloadleavedataexcel"><i class="fa fa-file-excel-o" aria-hidden="true"></i> Export Excel</a>' : '';
                    html = html + '<input type="button" id="searchDate" class="btn btn-warning pull-right" value="Search" />';
                    html = html + '<div class="col-md-3 pull-right margin-right10"><input type="text"  autoComplete="off" name="EndDate" id="EndDate" class="form-control searchfilter" placeholder="End Date" value="' + endDate + '" /></div>';
                    html = html + '<div class="col-md-3 pull-right"><input type="text" autoComplete="off" name="StartDate" id="StartDate" class="form-control searchfilter" placeholder="Start Date" value="' + startDate + '" /></div>';

                    //if (json.totalLeave != undefined && json.totalLeave != 0) {
                    //    html = html + '<div class="col-md-4 pull-right text-right"><span class="label label-info info-block2" style="font-size:15px;font-weight:bold;display: inline-block;max-width: 100%;margin:5px;padding:5px;">Total Applied Leave: ' + json.totalLeave + (json.totalLeave == 1 ? ' Day' : ' Days') + '</span></div>'
                    //}
                    
                    $('.dataTables_wrapper > div.row:first > div:last').html(html);
                    BindDatePickers();
                }
            });          
        }

        function getfilter() {

            var data = {
                user: (($('#selfleave').length > 0 && $('#selfleave').prop('checked')) ? $('#selfleave').val() : $('#user').val()),
                status: $('#status').val(),
                leavetype: $('#leavetype').val(),
                leavecatagory: $('#leavecatagory').val(),
                pm: $('#pm').val(),
                startDate: $('#StartDate').val(),
                endDate: $('#EndDate').val()
            };
            return data;
        }

        function LoadHolidayTypeTable() {
            //var isDirectorRole = false;
            //if (!isDirectorRole) {
            //    holidayTable.empty();
            //    var data = getfilter();
            //    $.ajax({
            //        url: domain + "Leave/HolidayType",
            //        type: 'POST',
            //        data: data,
            //        success: function (result) {
            //            holidayTable.html(result);
            //        },
            //        error: function (ex) {

            //        }
            //    });
            //}
        }
        function LoadLeaveBalance() {
                $.ajax({
                    url: domain + "LeaveBalance/Index",
                    contentType: 'application/html; charset=utf-8',
                    type: 'GET',
                    dataType: 'html',
                    success: function (result) {
                        $('#LeaveBalance').html(result);
                    },
                    error:(function (xhr, status) {
                        alert(status);
                    })
                });
            }
        


        $this.init = function () {
            initializeForm();
            LoadLeavesGrid();
            LoadHolidayTypeTable();
            LoadLeaveBalance();
        }
    }

    $(function () {
        var self = new ManageLeave;
        self.init();
    });

}(jQuery));
