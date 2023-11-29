(function ($) {
    function index() {
        var $this = this, grid;

        function intializeGrid() {

            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-improvement-table', {
                serverSide: true,
                destroy: true,
                ordering: false,
                searchDelay: 800,
                "pageLength": 25,
                "bFilter": true,
                "bAutoWidth": false,
                "bLengthChange": false,
                "language": {
                    searchPlaceholder: "Search By Name"
                },
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                {
                    url: domain + "OrgImprovement/Index",
                    type: "POST"
                },

                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "25%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "15%", "targets": 4 },
                    { "width": "15%", "targets": 5 },
                    { "width": "10%", "targets": 6 }
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "Title", data: "title", title: "Title", sortable: false, searchable: false, visible: true },
                        { name: "Type", data: "type", title: "Type", sortable: false, searchable: false, visible: true },
                        { name: "Name", data: "name", title: "Employee Name", sortable: false, searchable: false, visible: true },
                        { name: "ImprovementDate", data: "improvementDate", title: "Improvement Date", sortable: false, searchable: false, visible: true },
                        { name: "AddedBy", data: "addedBy", title: "Added By", sortable: false, searchable: false, visible: true },
                        
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = "";
                                if (dataRow.allowEditDelete) {
                                    actionButtons = $("<a/>", {
                                        id: "editimprovement",
                                        title: "edit",
                                        href: domain + "OrgImprovement/add/" + dataRow.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-add-edit-improvement",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-edit",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; " + $("<a/>", {
                                        href: domain + "OrgImprovement/delete/" + dataRow.id,
                                        id: "deleteimprovement",
                                        title: "delete",
                                        'data-toggle': "modal",
                                        'data-target': "#modal-delete-improvement",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-trash",
                                            style: "color:black"
                                        }),
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

        function intializeModalWithForm() {

            $("#modal-add-edit-improvement").on('loaded.bs.modal', function () {

                var modal = $(this);

                modal.find('#ImprovementDate').datepicker({
                    dateFormat: "dd/mm/yy"
                });

                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                }, null, function (result) {
                    if (result.isSuccess) {
                        modal.modal('hide');
                        grid.ajax.reload(null, false);
                        Global.ShowMessage(result.message, true, 'MessageDiv');
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-delete-improvement").on('loaded.bs.modal', function () {

                var modal = $(this);

                var form = new Global.FormHelper($(this).find("form"),
                    {
                        updateTargetId: "validation-summary",
                        validateSettings: { ignore: '' }
                    }, null, function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload(null, false);
                            Global.ShowMessage(result.message, true, 'MessageDiv');
                        }
                        else {
                            Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageDiv');
                        }
                    });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
        }

        $this.init = function () {
            intializeGrid();
            intializeModalWithForm();
        };
    }

    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));