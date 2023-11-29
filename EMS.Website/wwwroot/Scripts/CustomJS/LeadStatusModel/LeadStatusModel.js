(function ($) {
    function index() {
        var $this = this, grid;

        function attachEventCKEditor(instance) {
            CKEDITOR.instances[instance].on("instanceReady", function (e) {
                e.editor.document.on("keyup", function () {
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        }

        function initializeModalWithForm() {
            $("#modal-add-LeadStatusModel").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                }, null, function (result) {
                    if (result.isSuccess) {
                        modal.modal('hide');
                        $this.refreshTaskGrid();
                        Global.ShowMessage(result.message, true, 'MessageDiv');
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                    }
                });

                CKEDITOR.replace('MailContent');
                attachEventCKEditor('MailContent');

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-delete-LeadStatusModel").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                }, null, function (result) {
                    if (result.isSuccess) {
                        modal.modal('hide');
                        $this.refreshTaskGrid();
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

        }

        function loadtaskgrid() {
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-LeadStatusModelList', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "LeadStatusModel/index",
                        type: "Post",
                    },


                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "2%", "targets": 1 },
                    { "width": "91%", "targets": 2 },
                    { "width": "6%", "targets": 3 }

                ],
                columns:
                    [
                       { name: "StatusId", data: "statusId", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "StatusName", data: "statusName", title: "Status", sortable: false, searchable: false, visible: true },
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, dataRow, meta) {
                               console.log(data);
                               var actionButtons = $("<a/>", {
                                   id: "editLeadStatusModel",
                                   title: "edit",
                                   href: domain + "LeadStatusModel/add/" + data.statusId,
                                   'data-toggle': "modal",
                                   'data-target': "#modal-add-LeadStatusModel",
                                   'data-backdrop': "static",
                                   html: $("<i/>", {
                                       class: "glyphicon glyphicon-edit",
                                       style: "color:black"
                                   }),
                               }).get(0).outerHTML + "&nbsp; " + $("<a/>", {
                                   href: domain + "LeadStatusModel/delete/" + data.statusId,
                                   id: "deleteLeadStatusModel",
                                   title: "delete",
                                   'data-toggle': "modal",
                                   'data-target': "#modal-delete-LeadStatusModel",
                                   'data-backdrop': "static",
                                   html: $("<i/>", {
                                       class: "glyphicon glyphicon-trash",
                                       style: "color:black"
                                   }),
                               }).get(0).outerHTML + "&nbsp;"

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

        $this.refreshTaskGrid = function () {
            grid.ajax.reload();
        };

        $this.init = function () {
            loadtaskgrid();
            initializeModalWithForm();
        };
    }
    $(function () {       
        var self = new index();
        self.init();
        $.fn.TaskGridIndex = self;
    });
}(jQuery));