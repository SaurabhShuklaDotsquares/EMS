(function ($) {
    function index() {
        var $this = this, grid;

        function loadGrid() {

            var data = {
                financialYear: $("#FinancialYear").val(),
                textSearch: $("#TextSearch").val(),
                pmId: $("#PMId").val(),
            }

            $('.divoverlay').removeClass('hide');         
            grid = new Global.GridHelper('#grid-investmentList', {
                serverSide: true,
                destroy: true,
                "pageLength": 10,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "investment/index",
                        type: "Post",
                        data: data
                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "3%", "targets": 1, "className": "text-center" },
                    { "width": "10%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "15%", "targets": 4 },
                    { "width": "15%", "targets": 5 },
                    { "targets": 8, "className": "text-center" },
                ],
                columns:
                    [
                       { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "Name", data: "name", title: "Name", sortable: false, searchable: false, visible: true },
                       { name: "FatherName", data: "fatherName", title: "Father Name", sortable: false, searchable: false, visible: true },
                       { name: "AttendanceCode", data: "attendanceCode", title: "Att. Code", sortable: false, searchable: false, visible: true },
                       { name: "FinancialYear", data: "financialYear", title: "Financial Year", sortable: false, searchable: false, visible: true },
                       { name: "CreateDate", data: "createDate", title: "Added On", sortable: false, searchable: false, visible: true },
                       { name: "ModifyDate", data: "modifyDate", title: "Modified On", sortable: false, searchable: false, visible: true },
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, dataRow, meta) {
                               var actionButtons = '';
                               if (data.allowEdit) {
                                   actionButtons += $("<a/>", {
                                       id: "investmentedit",
                                       title: "edit",
                                       href: domain + "investment/addedit/" + data.id,
                                       html: $("<i/>", {
                                           class: "fa fa-pencil blue",
                                           style: "color:black"
                                       }),
                                   }).get(0).outerHTML + "&nbsp; "
                               }
                               if (data.downloadPDF) {
                                   actionButtons += $("<a/>", {
                                       id: "investmentpdf",
                                       title: "PDF Report",
                                       target: "_blank",
                                       href: domain + "investment/downloadpdf/" + data.id,
                                       html: $("<i/>", {
                                           class: "fa fa-file-pdf-o",
                                           style: "color:red;font-size:15px"
                                       }),
                                   }).get(0).outerHTML + "&nbsp; "
                               }
                               if (data.downloadZip) {
                                   actionButtons += $("<a/>", {
                                       id: "investmentzip",
                                       title: "Zip documents",
                                       target: "_blank",
                                       href: domain + "investment/downloaddocs/" + data.id,
                                       html: $("<i/>", {
                                           class: "fa fa-file-archive-o",
                                           style: "color:black;font-size:15px"
                                       }),
                                   }).get(0).outerHTML + "&nbsp; "
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

        function initializeControl() {
            $("#btnSearch").on("click", function () {
                loadGrid();
            });
        }

        $this.init = function () {
            loadGrid();
            initializeControl();
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));