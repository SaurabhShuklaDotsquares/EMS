(function ($) {
    function index() {
        var $this = this, grid;
        function GetFilter() {
            var data = {
                technology: parseInt($('#Technology').val()),
                model: parseInt($('#Model').val()),
                Name: $('#search').val()
            };          
            return data;
        }

        function loadProjectUserReportgrid() {
            grid = new Global.GridHelper('#grid-ProjectUserList', {
                serverSide: true,
                "ordering": false,
                destroy: true,
                searchDelay: 800,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                {
                    url: domain + "Report/ProjectUserReport",
                    type: "Post",
                    data: GetFilter()
                },


                "columnDefs": [
                    { "width": "3%", "targets": 0 },
                    { "width": "30%", "targets": 1 },
                    { "width": "22%", "targets": 2 },
                    { "width": "20%", "targets": 3 },
                    { "width": "15%", "targets": 4 },
                    { "width": "10%", "targets": 5 }

                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "Name", data: "name", title: "Project Name", sortable: false, searchable: false, visible: true },
                        { name: "AssignedUsers", data: "assignedUsers", title: "Assigned Users", sortable: false, searchable: false, visible: true },
                        { name: "ModelName", data: "modelName", title: "Bucket Model", sortable: false, searchable: false },
                        { name: "Technology", data: "technology", title: "Technology", sortable: false, searchable: false },
                        { name: "ProjectStartDate", data: "projectStartDate", title: "Start Date", sortable: false, searchable: false },
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
                loadProjectUserReportgrid();
            });

            $('#btn_reset').click(function () {
                $('#Technology').val('');
                $('#Model').val('');
                $('#search').val('');
                loadProjectUserReportgrid();
            });

        }

        $this.init = function () {
            loadProjectUserReportgrid();
            initilizeControls();
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));