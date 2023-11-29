(function ($) {
    function index() {
        var $this = this, grid;
        function GetFilter() {
            var data = {
                user: parseInt($('#selectUserId').val()),
            };
            return data;
        }

        function loadUserReportgrid() {
            grid = new Global.GridHelper('#grid-UserProjectList', {
                serverSide: true,
                destroy: true,
                searchDelay: 800,
                language: {
                    searchPlaceholder: "NAME"
                },
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                    {
                        url: domain + "Report/UserReport",
                        type: "Post",
                        data: GetFilter()
                    },


                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "2%", "targets": 1 },
                    { "width": "16%", "targets": 2 },
                    { "width": "25%", "targets": 3 },
                    { "width": "15%", "targets": 4 },
                    { "width": "40%", "targets":  5}
                ],
                columns:
                    [
                       { name: "Uid", data: "uid", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "Name", data: "name", title: "Name", sortable: false, searchable: false, visible: true },
                       { name: "Department", data: "department", title: "Department", sortable: false, searchable: false, visible: true},
                       { name: "ReportTo", data: "reportTo", title: "Report To", sortable: false, searchable: false },
                       { name: "AssignedProject", data: "assignedProject", title: "Assigned Project", sortable: false, searchable: false },
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                },
                "fnDrawCallback": function (oSettings) {
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                }
            });
        }
        function initilizeControls() {

            $('#CustomSearch').click(function () {
                loadUserReportgrid();
            });
        }

        $this.init = function () {
            loadUserReportgrid();
            initilizeControls();
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));