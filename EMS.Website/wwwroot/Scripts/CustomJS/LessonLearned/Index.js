(function ($) {
    function Index() {
        var $this = this, grid;

        function Intializecontrol() {
            $("#btnSearch").on("click", function () {
                loadGrid();
            });
        }

        function loadGrid() {
            var data = {
                pmId: $("#PMId").val(),
            }

            grid = new Global.GridHelper('#grid-lessons', {
                serverSide: true,
                destroy: true,
                "pageLength": 20,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "lessonlearned/index",
                        type: "Post",
                        data: data
                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "1%", "targets": 0 },
                    { "width": "3%", "targets": 1 },
                    { "width": "20%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "5%", "targets": 4 },
                ],
                columns:
                    [
                       { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       {
                           name: "ProjectName", data: "projectName", title: "Project", sortable: false, searchable: false, visible: true
                       },
                       {
                           name: "CreateBy", data: "createBy", title: "Added By", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               return row.createBy + "<br>" + row.createDate
                           }
                       },
                       
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, row, meta) {
                               return ' <a class="btn btn-default btn-sm" href="' + domain + 'lessonlearned/detail/' + row.id + '"><i class="fa fa-eye"></i> View</a>';
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
                }
            });
        }

        $this.init = function () {
            loadGrid();
            Intializecontrol();
        };
    }
    $(function () {
        var self = new Index();
        self.init();
    });
}(jQuery));