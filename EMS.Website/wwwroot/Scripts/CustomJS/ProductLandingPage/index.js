(function ($) {
    function Index() {
        var $this = this, grid;

        function loadGrid() {
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-requestList', {
                serverSide: true,
                destroy: true,
                ordering: false,
                "pageLength": 15,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "productlandingpage/index",
                        type: "POST",
                        data: getGridFilters()
                    },

                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "15%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "7%", "targets": 4 },
                    { "width": "7%", "targets": 5 }
                ],
                columns:
                    [
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "ProductName", data: "productName", title: "Product Name", sortable: false, searchable: false, visible: true },
                       { name: "CreateDate", data: "createDate", title: "Date Of Request", sortable: false, searchable: false, visible: true },
                       { name: "CreatedBy", data: "createdBy", title: "Added By", sortable: false, searchable: false, visible: true },
                       { name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true },
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, row, meta) {
                               var actionButtons = '';
                               if (row.editAllowed) {
                                   actionButtons += $("<a/>", {
                                       id: "addEdit",
                                       class: "btn btn-default btn-sm",
                                       href: domain + "productlandingpage/addedit/"+ row.id,
                                       html: $("<i/>", {
                                           class: "fa fa-pencil",
                                           style: "color:black"
                                       }).get(0).outerHTML + "&nbsp; Edit",
                                   }).get(0).outerHTML + "&nbsp;";
                               }

                               if (row.viewAllowed) {
                                   actionButtons += $("<a/>", {
                                       id: "view",
                                       class: "btn btn-default btn-sm",
                                       href: domain + "productlandingpage/view/"+ row.id,
                                       html: $("<i/>", {
                                           class: "fa fa-eye",
                                           style: "color:black"
                                       }).get(0).outerHTML + "&nbsp; View",
                                   }).get(0).outerHTML + "&nbsp;";

                                   actionButtons += $("<a/>", {
                                       id: "clone",
                                       class: "btn btn-default btn-sm",
                                       href: domain + "productlandingpage/clonetemplate/" + row.id,
                                       html: $("<i/>", {
                                           class: "fa fa-clone",
                                           style: "color:black"
                                       }).get(0).outerHTML + "&nbsp; Clone",
                                   }).get(0).outerHTML;
                               }

                               return actionButtons;
                           }
                       },

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

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
                }
            });
        }

        function getGridFilters() {
            return {
                pmId: $("#PMId").val(),
                status: $("#Status").val(),
            }
        }

        function initialize() {

            $("#btnSearch").on("click", function () {
                loadGrid();
            });
        }

        $this.init = function () {
            loadGrid();
            initialize();
        };
    }
    $(function () {
        var self = new Index();
        self.init();
    });
}(jQuery));