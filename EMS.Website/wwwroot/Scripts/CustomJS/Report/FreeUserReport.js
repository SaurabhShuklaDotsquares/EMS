(function ($) {
    function index() {
        var $this = this, grid;
        function GetFilter() {
            var data = {
                depart: parseInt($('#selectUserId').val()),
            };
            return data;
        }

        function loadUserReportgrid() {
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-UserProjectList', {
                serverSide: true,
                destroy: true,
                timeout:500,
                language: {
                    searchPlaceholder: "NAME"
                },
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "Report/FreeUserReport",
                    type: "Post",
                    data: GetFilter()
                },


                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "20%", "targets": 1},
                    { "width": "20%", "targets": 2 },
                    { "width": "16%", "targets": 3 },
                    { "width": "12%", "targets": 4},
                    { "width": "30%", "targets": 5 }
                ],
                columns:
                    [                        
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "Name", data: "name", title: "Name", sortable: false, searchable: false, visible: true },
                        { name: "UserDesignation", data: "userDesignation", title: "Designation", sortable: false, searchable: false, visible: true },
                        { name: "DepartmentName", data: "departmentName", title: "Department", sortable: false, searchable: false, visible: true },
                        { name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true },
                        { name: "ProjectName", data: "projectName", title: "Working On", sortable: false, searchable: false, visible: true },
                    ],
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