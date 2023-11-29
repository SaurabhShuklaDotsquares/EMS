(function ($) {
    function Index() {
        var $this = this, grid;

        function loadGrid() {
            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-ncLogList', {
                serverSide: true,
                destroy: true,
                "pageLength": 15,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "nclog/index",
                        type: "Post",
                        data: getGridFilters()
                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "1%", "targets": 0 },
                    { "width": "2%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "8%", "targets": 4 },
                    { "width": "7%", "targets": 5 },
                    { "width": "7%", "targets": 6 },
                    { "width": "10%", "targets": 7 },
                    { "width": "10%", "targets": 8 },
                    { "width": "8%", "targets": 9 },
                    { "width": "9%", "targets": 10 },
                    { "width": "6%", "targets": 11 }
                ],
                columns:
                    [
                       { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "ProjectName", data: "projectName", title: "Project", sortable: false, searchable: false, visible: true },
                       { name: "AuditDesc", data: "auditDesc", title: "Description of NC", sortable: false, searchable: false, visible: true },
                       { name: "AuditCycle", data: "auditCycle", title: "Cycle", sortable: false, searchable: false, visible: true },
                       { name: "AuditDate", data: "auditDate", title: "Date of Audit", sortable: false, searchable: false, visible: true },
                       { name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true },
                       { name: "Auditor", data: "auditor", title: "Auditor", sortable: false, searchable: false, visible: true },
                       { name: "Auditee", data: "auditee", title: "auditee", sortable: false, searchable: false, visible: true },
                       { name: "FollowUpDate", data: "followUpDate", title: "Follow-up Date", sortable: false, searchable: false, visible: true },
                       { name: "ClosedDate", data: "closedDate", title: "Closed Date", sortable: false, searchable: false, visible: true },
                       { name: "DaysTaken", data: "daysTaken", title: "Days Taken", sortable: false, searchable: false, visible: true },
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, row, meta) {
                               var actionButtons = $("<a/>", {
                                   id: "comment",
                                   class: "btn btn-default btn-sm",
                                   href: domain + "nclog/auditeecomments/" + row.id,
                                   'data-toggle': "modal",
                                   'data-target': "#modal-comment-nclog",
                                   'data-backdrop': "static",
                                   html: $("<i/>", {
                                       class: "fa fa-comments-o",
                                       style: "color:black"
                                   }).get(0).outerHTML + "&nbsp;" + (row.actionAllowed ? "Action" : "Details"),
                               }).get(0).outerHTML + "&nbsp; ";

                               if (row.editAllowed) {
                                   actionButtons += $("<a/>", {
                                       id: "edit",
                                       class: "btn btn-default btn-sm",
                                       title: "Edit",
                                       href: domain + "nclog/addedit/" + row.id,
                                       'data-toggle': "modal",
                                       'data-target': "#modal-add-nclog",
                                       'data-backdrop': "static",
                                       html: $("<i/>", {
                                           class: "fa fa-pencil",
                                           style: "color:black"
                                       }).get(0).outerHTML + "&nbsp; Edit",
                                   }).get(0).outerHTML + "&nbsp; ";
                               }

                               return actionButtons;
                           }
                       },

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    switch (aData.status) {
                        case "Closed":
                            $('td', nRow).css({ 'background-color': '#9dff9d', 'color': 'black' });
                            break;
                        case "Completed":
                            $('td', nRow).css({ 'background-color': '#ffed26', 'color': 'black' });
                            break;
                        case "Open":
                            if (aData.auditCycle.indexOf('Observation') > -1) {
                                $('td', nRow).css({ 'background-color': '#bfe8fb', 'color': 'black' });
                            }
                            else if (aData.auditCycle.indexOf('Major') > -1) {
                                $('td', nRow).css({ 'background-color': '#ff3333', 'color': 'white' });
                            }
                            else {
                                $('td', nRow).css({ 'background-color': '#ffb6b6', 'color': 'black' });
                            }
                            break;
                    }
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
                projectId: $("#ProjectId").val(),
            }
        }

        function initialize() {

            $("#modal-add-nclog").on('loaded.bs.modal', function () {

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

                form.find(".select2").select2({
                    placeholder: "--Select--",
                    allowClear: true,
                    width: '100%'
                });

                form.find("#AuditDate").datepicker({
                    dateFormat: "dd/mm/yy",
                    onSelect: function (selectedDate) {
                        form.find("#FollowUpDate").datepicker("option", "minDate", selectedDate);
                    }
                });

                form.find("#FollowUpDate").datepicker({
                    dateFormat: "dd/mm/yy",
                    onSelect: function (selectedDate) {
                        form.find("#AuditDate").datepicker("option", "maxDate", selectedDate);
                    }
                });

                form.on("change", "#AuditType", function () {
                    var type = parseInt(this.value);
                    if (type) {
                        var $followUpDate = form.find("#FollowUpDate");
                        var auditDate = form.find("#AuditDate").datepicker('getDate') || new Date();

                        switch (type) {
                            case 1:
                                $followUpDate.val('').data('rule-required', true).datepicker("option", "maxDate", new Date(auditDate.getTime() + (42 * 24 * 60 * 60 * 1000)));
                                break;
                            case 2:
                                $followUpDate.val('').data('rule-required', true).datepicker("option", "maxDate", new Date(auditDate.getTime() + (14 * 24 * 60 * 60 * 1000)));
                                break;
                            case 3:
                                $followUpDate.val('').data('rule-required', false).removeClass('error').next('label.error').remove();
                                $followUpDate.datepicker("option", "maxDate", new Date(auditDate.getTime() + (365 * 24 * 60 * 60 * 1000)));
                                break;
                        }
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-comment-nclog").on('loaded.bs.modal', function () {

                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "NotificationMessage"
                }, null,
                    function (result) {

                        if (result.isSuccess) {
                            modal.modal('hide');
                            $('#grid-ncLogList').DataTable().ajax.reload();

                            Global.ShowMessage(result.message, true, "divMessage");

                        } else {
                            form.find("#NotificationMessage").html(result);
                        }
                    });

                form.on('click', '#btn-closeLog', function (e) {

                    e.preventDefault();

                    if (form.find(':input').valid()) {

                        var id = parseInt(form.find('#Id').val());

                        if (id) {
                            if (confirm('Are you sure?\nLog will be marked as Closed.')) {
                                form.attr('action', domain + 'nclog/auditclose/' + id).submit();
                            }
                        }
                    }
                });

                form.find("#CompletedDate").datepicker({
                    dateFormat: "dd/mm/yy"
                });

                form.find("#ClosedDate").datepicker({
                    dateFormat: "dd/mm/yy"
                });

                form.on("change", "#Status", function () {
                    var type = parseInt(this.value);
                    if (type) {
                        var completedDate = form.find("#CompletedDate");
                        switch (type) {
                            case 1:
                                completedDate.data('rule-required', false).removeClass('error').next('label.error').remove();
                                break;
                            case 2:
                                completedDate.data('rule-required', true);
                                break;
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