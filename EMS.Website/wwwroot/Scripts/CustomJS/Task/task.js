(function ($) {
    function index() {
        var $this = this, grid;

        function attachEventCKEditor() {
            if (CKEDITOR === undefined) {
                return;
            }
            CKEDITOR.on('instanceReady', function () {
                $.each(CKEDITOR.instances, function (instance) {
                    CKEDITOR.instances[instance].document.on("keyup", function (e) {
                        CKEDITOR.instances[instance].updateElement();
                    });
                });
            });
        }

        function initializeModalWithForm() {
            $("#modal-add-task").on('loaded.bs.modal', function () {
                var modal = $(this);

                $.validator.addMethod("priorityvalidate", function (value, element) {
                    if ($("#Priority").val() == '0') {
                        return false;
                    } else {
                        return true;
                    }
                }, '*required');

                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: {
                        ignore: [],
                        rules: {
                            Priority: { priorityvalidate: true }
                        }
                    }
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

                modal.find(".multiple").select2({
                    placeholder: "---Select Assign Users---",
                    allowClear: true,
                    width: '100%'
                });

                modal.find("#taskEndDate").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    changeMonth: true,
                    changeYear: true,
                    numberOfMonths: 1,
                    minDate: 0
                });

                modal.find("#Assign").on("change", function () {
                    $(this).removeClass('hidden').removeClass('error').prop('disabled', false).next('label.error').hide();
                });

                if ($("#Remark") && CKEDITOR !== undefined) {
                    try {
                        CKEDITOR.replace('Remark', { toolbar: "Basic" });
                    } catch (e) {
                        console.log(e);
                    }
                }

                attachEventCKEditor();

                CompleteTaskForAll();
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-delete-task").on('loaded.bs.modal', function () {

                var modal = $("#modal-delete-task");

                var form = new Global.FormHelper($(this).find("form"),
                    {
                        updateTargetId: "validation-summary",
                        validateSettings: { ignore: '' }
                    }, null, function (result) {
                        //debugger;
                        if (result.isSuccess) {
                            modal.modal('hide');
                            $this.refreshTaskGrid();
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

            $("#selectUserId").on('change', function () {
                loadtaskgrid();
            });
            $("#selectTaskStatusId").on('change', function () {
                loadtaskgrid();
            });

        }
        function getFilter() {
            var data = {
                otherTeamMembers: $("#chkOtherTeamMembers").is(':checked'),
                assignedByMe: $("#chkAssignedByMe").is(':checked'),
                assignedToMe: $("#chkMyTasks").is(':checked'),
                TaskStatusId: $("#selectTaskStatusId").val(),
                userId: $("#selectUserId").val()
            };

            return data;
        }

        function loadtaskgrid() {
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-taskList', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                "ordering": false,
                ajax:
                {
                    url: domain + "task/index",
                    type: "Post",
                    //data: { userId: $("#selectUserId").val() }
                    data: getFilter()
                },
                //order: [[0, "desc"]],
                "columnDefs": [
                    //{ "width": "2%", "targets": 0 },
                    { "width": "2%", "targets": 0 },
                    { "width": "9%", "targets": 1 },
                    { "width": "23%", "targets": 2 },
                    { "width": "17%", "targets": 3 },
                    { "width": "8%", "targets": 4 },
                    { "width": "9%", "targets": 5 },
                    { "width": "9%", "targets": 6 },
                    { "width": "7%", "targets": 7 },
                    { "width": "7%", "targets": 8 },
                    { "width": "9%", "targets": 9 }
                ],
                columns:
                    [
                        //{ name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "TaskId", data: "taskId", title: "Task Id", sortable: false, searchable: false, visible: true },
                        {
                            name: "Source", data: null, title: "Task", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                if (data.source === "MOM") {
                                    //return data.taskName + "<a target='_target'" + "href='" + domain + "minutesOfMeeting/viewtask/" + data.meetingId +
                                    //    "?tl=true&st=" + data.status + "'><sup style='color:#112eca;font-weight:bold;'><b>MOM: " + data.updatedDate + "</b></sup></a>";
                                    return "<a style='color:#112eca;font-size:11px;' target='_blank'" + "href='" + domain + "minutesOfMeeting/viewtask/" + data.meetingId +
                                        "?tl=true&st=" + data.meetingStatus + "'><b>MOM# " + data.updatedDate + "</b></a><br/>" + data.taskName;
                                }
                                else {
                                    return data.taskName;
                                }
                            }
                        },
                        { name: "AssignTo", data: "assignTo", title: "Assign To", sortable: false, searchable: false, visible: true },
                        { name: "AssignBy", data: "assignBy", title: "Assign By", sortable: false, searchable: false, visible: true },
                        { name: "UpdatedDate", data: "updatedDate", title: "Task Add Date", sortable: false, searchable: false, visible: true },
                        { name: "TaskEndDate", data: "taskEndDate", title: "Task End Date", sortable: false, searchable: false, visible: true },
                        { name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true },
                        { name: "Priority", data: "priority", title: "Priority", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = $("<a/>", {
                                    id: "commenttask",
                                    title: "comment",
                                    href: domain + "task/comment/" + data.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-comment-task",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "fa fa-comments blue",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp; "

                                if (data.assignid == data.currentUserId) {
                                    actionButtons += $("<a/>", {
                                        id: "chasetask",
                                        title: "chase",
                                        href: domain + "task/chase/" + data.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-chase-task",
                                        'data-backdrop': "black",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-share",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; " + $("<a/>", {
                                        id: "edittask",
                                        title: "edit",
                                        href: domain + "task/add/" + data.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-add-task",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-edit",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; " + $("<a/>", {
                                        href: domain + "task/delete/" + data.id,
                                        id: "deletetask",
                                        title: "delete",
                                        'data-toggle': "modal",
                                        'data-target': "#modal-delete-task",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-trash",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp;"
                                }
                                return actionButtons;
                            }
                        },
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    if (aData["status"] == "Completed") {
                        $(nRow).addClass("Completed_border FillCellBasicStyleForGrid");
                        //$('td', nRow).css('background-color', '#9DFF9D');
                        //$('td', nRow).css('color', 'black');
                    }
                    else if (aData["status"] == "Process") {
                        $(nRow).addClass("Process_border FillCellBasicStyleForGrid");
                        //$('td', nRow).css('background-color', 'yellow');
                        //$('td', nRow).css('color', 'black');
                    }
                    else if (aData["status"] == "Pending") {
                        $(nRow).addClass("Pending_border FillCellBasicStyleForGrid");
                        //$('td', nRow).css('background-color', '#FF3333');
                        //$('td', nRow).css('color', 'white');
                    }
                    else if (aData["status"] == "Closed") {
                        $(nRow).addClass("Closed_border FillCellBasicStyleForGrid");
                        //$('td', nRow).css('background-color', 'Orange');
                        //$('td', nRow).css('color', 'white');
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



        function tasks() {
            $('.Chktasks').on('change', function () {


                if ($(this).is(":checked")) {

                    if ($(this).val() == "1") {
                        $("#divMyTasks").addClass('chkMyTasks');
                        $("#chkMyTasks").prop('checked', true);

                        $("#chkAssignedByMe").prop('checked', false); // unchecking assigned by me and otherTeammembers
                        $("#divAssignedByMe").removeClass('chkAssignedByMe');

                        $("#chkOtherTeamMembers").prop('checked', false);
                        $("#divOherTeamMembers").removeClass('chkOtherTeamMembers');

                        $("#chkCompleted").prop('checked', false);
                        $("#divCompleted").removeClass('chkCompletedCls');

                    }
                    else if ($(this).val() == "2") {
                        $("#divAssignedByMe").addClass('chkAssignedByMe');
                        $("#chkAssignedByMe").prop('checked', true);

                        $("#chkMyTasks").prop('checked', false);  //unchecking My tasks and otherTeam members
                        $("#divMyTasks").removeClass('chkMyTasks');

                        $("#chkOtherTeamMembers").prop('checked', false);
                        $("#divOherTeamMembers").removeClass('chkOtherTeamMembers');

                        $("#chkCompleted").prop('checked', false);
                        $("#divCompleted").removeClass('chkCompletedCls');

                    }
                    else if ($(this).val() == "3") {
                        $("#divOherTeamMembers").addClass('chkOtherTeamMembers');
                        $("#chkOtherTeamMembers").prop('checked', true);

                        $("#chkMyTasks").prop('checked', false); //unchecking My tasks and Assigned by me
                        $("#divMyTasks").removeClass('chkMyTasks');

                        $("#chkAssignedByMe").prop('checked', false);
                        $("#divAssignedByMe").removeClass('chkAssignedByMe');

                        $("#chkCompleted").prop('checked', false);
                        $("#divCompleted").removeClass('chkCompletedCls');
                    }
                    else if ($(this).val() == "4") {
                        $("#divCompleted").addClass('chkCompletedCls');
                        $("#chkCompleted").prop('checked', true);

                        $("#chkMyTasks").prop('checked', false); //unchecking My tasks and Assigned by me
                        $("#divMyTasks").removeClass('chkMyTasks');

                        $("#chkAssignedByMe").prop('checked', false);
                        $("#divAssignedByMe").removeClass('chkAssignedByMe');

                        $("#chkOtherTeamMembers").prop('checked', false);
                        $("#divOherTeamMembers").removeClass('chkOtherTeamMembers');
                    }
                    loadtaskgrid();
                }
                //else { // Commented for single click

                //    if ($(this).val() == "1") {
                //        $("#chkMyTasks").prop('checked', false);
                //        $("#divMyTasks").removeClass('chkMyTasks');
                //    }
                //    else if ($(this).val() == "2") {
                //        $("#chkAssignedByMe").prop('checked', false);
                //        $("#divAssignedByMe").removeClass('chkAssignedByMe');
                //    }
                //    else if ($(this).val() == "3") {
                //        $("#chkOtherTeamMembers").prop('checked', false);
                //        $("#divOherTeamMembers").removeClass('chkOtherTeamMembers');
                //    }

                //    loadtaskgrid();
                //}
            });
        }

        function CompleteTaskForAll() {
            $('.btn_submit_task').on('click', function () {
                if ($('#TaskStatusId').val() === "3" || $('#TaskStatusId').val() === "4") {
                    var status = $('#TaskStatusId').val() === "3" ? "complete" : "closed ";
                    var message = 'Are you sure, you would like ' + status + ' this task for all participants?';

                    if (confirm(message)) {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            });
        }

        $this.refreshTaskGrid = function () {
            grid.ajax.reload();
        };

        $this.init = function () {
            loadtaskgrid();
            initializeModalWithForm();
            tasks();
        };
    }
    $(function () {
        var self = new index();
        self.init();
        $.fn.TaskGridIndex = self;
    });
}(jQuery));