(function ($) {
    function Index() {
        var $this = this, grid;

        function loadGrid() {
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-logList', {
                serverSide: true,
                destroy: true,
                "pageLength": 15,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "pilog/index",
                    type: "POST",
                    data: getGridFilters()
                },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "1%", "targets": 0 },
                    { "width": "2%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "20%", "targets": 3 },
                    { "width": "5%", "targets": 4 },
                    { "width": "5%", "targets": 5 },
                    { "width": "5%", "targets": 6 },
                    { "width": "7%", "targets": 7 }
                ],
                columns:
                    [
                        { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "ProcessName", data: "processName", title: "Process Name", sortable: false, searchable: false, visible: true },
                        { name: "PotentialImprovementArea", data: "potentialImprovementArea", title: "Potential Improvement Area", sortable: false, searchable: false, visible: true },
                        { name: "CreateDate", data: "createDate", title: "Date Of Request", sortable: false, searchable: false, visible: true },
                        { name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true },
                        { name: "SuggestedBy", data: "suggestedBy", title: "Suggested By", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '';
                                if (row.editAllowed) {
                                    actionButtons += $("<a/>", {
                                        id: "addEdit",
                                        class: "btn btn-default btn-sm",
                                        href: domain + "pilog/addedit/" + row.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-add-pilog",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "fa fa-pencil",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp; Edit",
                                    }).get(0).outerHTML + "&nbsp; ";
                                }

                                actionButtons += $("<a/>", {
                                    id: "reqApproval",
                                    class: "btn btn-default btn-sm",
                                    title: "Edit",
                                    href: domain + "pilog/requestapproval/" + row.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-approve-pilog",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "fa fa-" + (row.approvalAllowed || row.rollOutAllowed ? "check" : "info-circle"),
                                        style: "color:black"
                                    }).get(0).outerHTML + "&nbsp; " + (row.approvalAllowed ? "Process" : row.rollOutAllowed ? "Roll-Out" : "Details"),
                                }).get(0).outerHTML + "&nbsp; ";

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

            $("#modal-add-pilog").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "NotificationMessage"
                }, null,
                    function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload();
                            Global.ShowMessage(result.message, true, "divMessage");
                        } else {
                            form.find("#NotificationMessage").html(result);
                        }
                    });
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-approve-pilog").on('loaded.bs.modal', function () {

                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "NotificationMessage",
                    validateSettings: { ignore: ':disabled' }
                }, null, function (result) {
                    if (result.isSuccess) {
                        modal.modal('hide');
                        grid.ajax.reload();
                        Global.ShowMessage(result.message, true, "divMessage");
                    } else {
                        form.find("#NotificationMessage").html(result);
                    }
                });

                form.find("#EstimatedSchedule").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    minDate: 0,
                    changeMonth: true,
                    changeYear: true,
                    numberOfMonths: 1
                });

                form.on("change", "#Status", function () {
                    switch (parseInt(this.value)) {
                        case 2:
                            form.find("#divCancelReason").addClass('hidden').find(":input").prop("disabled", true);
                            form.find("#divRemarks").removeClass('hidden').find(":input").prop("disabled", false);
                            form.find("#divEstimatedSchedule").addClass('hidden').find(":input").prop("disabled", true);
                            break;
                        case 3:
                            form.find("#divCancelReason").addClass('hidden').find(":input").prop("disabled", true);
                            form.find("#divRemarks").removeClass('hidden').find(":input").prop("disabled", false);
                            form.find("#divEstimatedSchedule").removeClass('hidden').find(":input").prop("disabled", false);
                            break;
                        case 10:
                            form.find("#divCancelReason").removeClass('hidden').find(":input").prop("disabled", false);
                            form.find("#divRemarks").addClass('hidden').find(":input").prop("disabled", true);
                            form.find("#divEstimatedSchedule").addClass('hidden').find(":input").prop("disabled", true);
                            break;
                        default:
                            form.find("#divCancelReason").addClass('hidden').find(":input").prop("disabled", true);
                            form.find("#divRemarks").addClass('hidden').find(":input").prop("disabled", true);
                            form.find("#divEstimatedSchedule").addClass('hidden').find(":input").prop("disabled", true);
                            break;
                    }
                });

                form.find("#Status").change();

                form.on('click', '#btn-rollOut', function (e) {

                    e.preventDefault();
                    var id = parseInt(form.find('#Id').val());

                    if (id) {
                        if (confirm('Are you sure?\nProcess Improvement suggestion will be Roll Out for all users.')) {
                            form.attr('action', domain + 'pilog/rollout/' + id).submit();
                        }
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

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