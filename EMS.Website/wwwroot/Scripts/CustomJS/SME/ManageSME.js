(function () {
    function ManageSME() {       
        var $this = this, grid;
        function getfilter() {
            var data = {
                SubjectMatter: $('#subjectmatter').val(),
            };
            return data;
        }


        function intializeGrid() {          
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-sme-table', {
                serverSide: true,
                destroy: true,
                ordering: false,
                searchDelay: 800,
                "pageLength": 50,
                "bFilter": true,
                "bAutoWidth": false,
                "bLengthChange": false,
                "language": {
                    searchPlaceholder: "Search By Subject Matter"
                },
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                {
                    url: domain + "sme/GetAllSmeData",
                    type: "POST",
                    data: getfilter()

                },

                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "11%", "targets": 1 },
                    { "width": "13%", "targets": 2 },
                    { "width": "13%", "targets": 3 },
                    { "width": "13%", "targets": 4 },
                    { "width": "13%", "targets": 5 },
                    { "width": "11%", "targets": 6 },
                    { "width": "5%", "targets": 7 },
                    { "width": "12%", "targets": 8 },
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "Subject", data: "subjectmatter", title: "Subject", sortable: false, searchable: false, visible: true },
                        { name: "Expert-1", data: "level1", title: "Expert-1", sortable: false, searchable: false, visible: true },
                        { name: "Expert-2", data: "level2", title: "Expert-2", sortable: false, searchable: false, visible: true },
                        { name: "Expert-3", data: "level3", title: "Expert-3", sortable: false, searchable: false, visible: true },
                        { name: "Expert-4", data: "level4", title: "Expert-4", sortable: false, searchable: false, visible: true },
                        { name: "Expert-5", data: "level5", title: "Expert-5", sortable: false, searchable: false, visible: true },
                        {
                            name: "Status",
                            data: "runningStatus",
                            title: "Status",
                            sortable: false,
                            searchable: false,
                            visible: true
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                //get                              
                                var actionButtons = "";
                                actionButtons += $("<a/>", {
                                    id: "editevent",
                                    title: "Edit",
                                    'class': "btn btn-default btn-sm",
                                    href: domain + "sme/Index?id=" + data.id,
                                    html: $("<i/>", {
                                        class: "fa fa-edit",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp; ";
                                actionButtons += $("<a/>", {
                                    id: "deleteevent",
                                    title: "Delete",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-sme",
                                    'data-backdrop': "static",
                                    'class': "btn btn-default btn-sm",
                                    href: domain + "sme/delete?id=" + data.id,                                   
                                    html: $("<i/>", {
                                        class: "fa fa-trash",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp; ";


                                return actionButtons;
                            }
                        },

                    ],

                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    var href = "SME/ExportReportToExcel";

                    tableCount = $("#grid-sme-table").DataTable().data().length
                    //debugger;
                    var tbl = $('#grid-sme-table').DataTable();
                    debugger;
                    if (tableCount > 0) {
                        $("#btnExport").attr('href', href + "?SubjectMatter=" + $('#subjectmatter').val());
                        $("#btnExport").removeAttr('disabled', 'disabled');
                    }
                    else {
                        $("#btnExport").attr('disabled', 'disabled');
                        $("#btnExport").attr('href', "javascript:void(0)");
                    }
                },
                "fnDrawCallback": function (oSettings) {

                    var href = "SME/ExportReportToExcel";

                   
                    tableCount = $("#grid-sme-table").DataTable().data().length
                    debugger;
                    if (tableCount > 0) {
                        $("#btnExport").attr('href', href + "?SubjectMatter=" + $('#subjectmatter').val());
                        $("#btnExport").removeAttr('disabled', 'disabled');
                    }
                    else {
                        $("#btnExport").attr('disabled', 'disabled');
                        $("#btnExport").attr('href', "javascript:void(0)");
                    }

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

            $('.dataTables_filter').hide();
            $('#searchButton').on('click', function () {
                debugger;
                intializeGrid();

                var href = "SME/ExportReportToExcel";

                
                tableCount = $("#grid-tds-table").DataTable().data().length
                if (tableCount > 0) {
                    $("#btnExport").attr('href', href + "?SubjectMatter=" + $('#subjectmatter').val());
                    $("#btnExport").removeAttr('disabled', 'disabled');
                }
                else {
                    $("#btnExport").attr('disabled', 'disabled');
                    $("#btnExport").attr('href', "javascript:void(0)");
                }
            });

            $('#resetButton').on('click', function () {
                window.location.reload();
                //resetSearchInput();
            });
        }

       

        function resetSearchInput() {
           
            var selectedValues = $('#subjectmatter').val(); // Get selected values as an array
            $('#subjectmatter').val([]); // Unselect all options

            // Uncheck the selected options
            if (selectedValues && selectedValues.length > 0) {
                $.each(selectedValues, function (index, value) {
                    $('#subjectmatter option[value="' + value + '"]').prop('checked', false);
                });
            }

            intializeGrid();
        }





        $this.init = function () {
            $('.selectsubjectmatter').fSelect({ placeholder: "Select Subject Matter" });
            intializeGrid();
        }
    }
    $(function () {
        var self = new ManageSME;
        self.init();
    });

}(jQuery));
