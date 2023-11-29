(function ($) {
    function index() {
        var $this = this, grid, formAddEdit;
        //$("#search_lateHour").on('change', function () {
        //    loadGrid();
        //});
        //$("#EmployeeId").on('change', function () {
        //    loadGrid();
        //});
        $("#btnSearch").on('click', function () {
            loadGrid();
        });
        
        $("#btn_reset").on('click', function () {
            $("#search_lateHour").val('');
            $("#EmployeeId").val('');
            $("#dateFrom").val('');
            $("#dateTo").val('');
            loadGrid();
        })
        function InitializeControl()
        {
            $("#dateFrom").datepicker({
                ////defaultDate: "+1w",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                //numberOfMonths: 1,
                //minDate: 0
            });

            $("#dateTo").datepicker({
                //defaultDate: "+1w",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                //numberOfMonths: 1,
                //minDate: 0
            });
        }

        function loadGrid() {
            $('.divoverlay').removeClass('hide');
            var search_text = $("#search_lateHour").val();
            var EmployeeId = $("#EmployeeId").val();
            var dateFrom = $("#dateFrom").val();
            var dateTo = $("#dateTo").val();
            var lateHourGrid = new Global.GridHelper('#grid-latehour', {
                serverSide: true,
                destroy: true,
                "pageLength": 25,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                "bSearch": true,
                //language: {
                //    searchPlaceholder: "Search By Name"
                //},
                ajax:
                    {
                        url: domain + "leave/latehour",
                        type: "Post",
                        data: { search_text: search_text, EmployeeId: EmployeeId, dateFrom: dateFrom, dateTo: dateTo }
                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "2%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "25%", "targets": 4 },
                    { "width": "46%", "targets": 5 },
                ],
                columns:
                    [
                       { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "EmpName", data: "empName", title: "Employee Name", sortable: false, searchable: false, visible: true },
                       { name: "DayOfDate", data: "dayOfDate", title: "Day OF Date", sortable: false, searchable: false, visible: true },
                       { name: "LateHour", data: "lateHour", title: "Office Time Difference", sortable: false, searchable: false, visible: true },
                       { name: "Reason", data: "reason", title: "Reason", sortable: false, searchable: false, visible: true }

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    console.log(aData)
                },
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
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
                    if (json.recordsTotal == 0)
                    {
                        $(".manage-Project-page").find('a').addClass('hidden');
                    }
                    else
                    {
                        $(".manage-Project-page").find('a').removeClass('hidden');
                    }
                    var html = '<div class="row"><div class="col-sm-12"><label id="totalLateDays" style="color:red;font-weight:bold;">';
                    if (json.TotalLateTime != undefined) {
                        html = html + '<label style="color:black;font-weight:bold;">Total Late Hour:</label> ' + json.TotalLateTime + '';
                    }
                    html = html + '</label></div></div>';
                    $('.dataTables_wrapper > div.row:first > div:last').html(html);

                    //$('#totalLateDays').html("");
                    //if (json.TotalLateTime != undefined) {
                    //    $('#totalLateDays').html(json.TotalLateTime);
                    //}
                }
            })
            return lateHourGrid;
        }

        $this.init = function () {
            loadGrid();
            InitializeControl();
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));