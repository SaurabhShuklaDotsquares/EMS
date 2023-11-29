(function ($) {
    function index() {
        var $this = this, grid;

        function intializeGrid() {

            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-appraise-table', {
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
                        url: domain + "appraise/index",
                        type: "POST"
                    },
                
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "40%", "targets": 1 },
                    { "width": "20%", "targets": 2 },
                    { "width": "20%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "5%", "targets": 5 },
                ],
                columns:
                    [
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "Name", data: "name", title: "EMPLOYEE NAME", sortable: false, searchable: false, visible: true },
                       { name: "Type", data: "type", title: "Type", sortable: false, searchable: false, visible: true },
                       { name: "AddedBy", data: "addedBy", title: "Added By", sortable: false, searchable: false, visible: true },
                       { name: "AddedDate", data: "addedDate", title: "Added Date", sortable: false, searchable: false, visible: true },
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, dataRow, meta) {
                               var actionButtons = "";
                               if (dataRow.allowEditDelete) {
                                   actionButtons = $("<a/>", {
                                       id: "editappraise",
                                       title: "edit",
                                       href: domain + "appraise/add/" + dataRow.id,
                                       'data-toggle': "modal",
                                       'data-target': "#modal-add-edit-appraise",
                                       'data-backdrop': "static",
                                       html: $("<i/>", {
                                           class: "glyphicon glyphicon-edit",
                                           style: "color:black"
                                       }),
                                   }).get(0).outerHTML + "&nbsp; " + $("<a/>", {
                                       href: domain + "appraise/delete/" + dataRow.id,
                                       id: "deleteappraise",
                                       title: "delete",
                                       'data-toggle': "modal",
                                       'data-target': "#modal-delete-appraise",
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

            $("#modal-add-edit-appraise").on('loaded.bs.modal', function () {

                var modal = $(this);

                modal.find("#appraiseDate").datepicker({
                    dateFormat: "dd/mm/yy"
                });

                modal.find("input[type='radio'][name='AppraiseId']").change(function () {
                    var value = this.value;
                    if (value == "2") {
                        modal.find("#clientAppraise").removeClass("hidden").find(":input").prop("disabled", false);
                        modal.find("#TlComment").data("rule-required", false).removeClass("error").blur();
                        modal.find("#ClientComment").data("rule-required", true);
                    }
                    else {
                        modal.find("#clientAppraise").addClass("hidden").find(":input").prop("disabled", true);;
                        modal.find("#ClientComment").data("rule-required", false);
                    }
                });

                modal.find("input[type='radio'][name='AppraiseId']:checked").change();

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

            $("#modal-delete-appraise").on('loaded.bs.modal', function () {

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