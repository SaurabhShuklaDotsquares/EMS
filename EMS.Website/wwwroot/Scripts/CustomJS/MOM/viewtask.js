(function ($) {

    function viewtask() {
        var meetingId = $('#Id').val();
        var topicId = $('#MeetingMasterID').val();
        var $this = this, grid;
        var statusId = [0];
        var filterParticipants = '';
        var isToDo = false;
        function loadGrid() {

            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-minutesOfMeetingTask', {
                serverSide: true,
                destroy: true,
                ordering: false,
                "pageLength": 0,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "minutesOfMeeting/ViewTask?meetingId=" + meetingId + "&topicId=" + topicId + "&statusId=" + statusId + "&filterParticipants=" + filterParticipants,
                    //url: domain + "minutesOfMeeting/ViewTask",
                    // data: { "meetingId": meetingId, "topicId": topicId, "statusId": statusId},
                    type: "Post"
                },
                "columnDefs": [
                    //{ "width": "2%", "targets": 0 },      //RowIndex
                    //{ "width": "23%", "targets": 1 }      //Task,
                    //{ "width": "14%", "targets": 2 }      //Participants,
                    //{ "width": "6%", "targets": 3 },      //TaskStatus
                    //{ "width": "6%", "targets": 4 },      //Priority
                    //{ "width": "12%", "targets": 5 }      //Remark,
                    //{ "width": "9%", "targets": 6 },      //TargetDate
                    //{ "width": "29%", "targets": 7 }      //Action

                    { "width": "2%", "targets": 0 },        //RowIndex
                    { "width": "23%", "targets": 1 },       //Task
                    { "width": "23%", "targets": 2 },       //Participants
                    { "width": "5%", "targets": 3 },        //TaskStatus
                    { "width": "5%", "targets": 4 },        //Priority
                    { "width": "15%", "targets": 5 },       //Remark
                    { "width": "8%", "targets": 6 },        //TargetDate
                    { "width": "20%", "targets": 7 }        //Action
                ],
                columns:
                    [
                        {
                            name: "RowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true,

                        },
                        {
                            name: "Task", data: "task", title: "Task", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var discussedcomment = '';
                                if (row.isDiscussed) {
                                    discussedcomment = data + "&nbsp;&nbsp;<span class='badge badge-pill badge-success scs'>" + $("<strong/>", {
                                        id: "discussedcomment",
                                        class: "alert-discussed"

                                    }).get(0).outerHTML + "Discussed</span>&nbsp;";
                                }
                                else {
                                    discussedcomment = data;
                                }
                                return discussedcomment;
                            }
                        },
                        {
                            name: "Participants", data: "paticipantsList", title: "Participants", sortable: false, searchable: false, visible: true,

                        },
                        {
                            name: "TaskStatus", data: "taskStatus", title: "Status", sortable: false, searchable: false, visible: true,

                        },
                        {
                            name: "Priority", data: "priority", title: "Priority", sortable: false, searchable: false, visible: true,

                        },
                        {
                            name: "Remark", data: "remark", title: "Comment", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var actionButtons = '';
                                if (data == "null" || data == null) {
                                    return actionButtons;
                                }
                                if (data.length > 100) {
                                    actionButtons += ' <div id="divShowMore_"' + row.id + '" class="content divmore">' + data.substring(0,100) + '...</div>';
                                    actionButtons += '<div class="pull-right divmoreinner"><a class="fullView" data-id="' + row.id + '" data-text="' + data + '" title="Full view" href="#!" data-toggle="modal" data-target="#modal-mom-agenda" data-backdrop="static" style="color: red;margin-right:24px;margin-top:3px;"><i class="fa fa-desktop"></i> <span>Full view</span></a></div>';
                                } else {
                                    actionButtons += data;
                                }
                                return actionButtons;
                            }

                        },
                        {
                            name: "TargetDate", data: "targetDate", title: "Target Date", sortable: false, searchable: false, visible: true,

                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '';
                                if (row.isEditable) {
                                    actionButtons += $("<a/>", {
                                        id: "view",
                                        class: "btn btn-default btn-sm btn-view",
                                        title: "Edit Action",
                                        href: domain + "minutesOfMeeting/addEditTask?meetingId=" + meetingId + "&id=" + row.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-add-mom",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "fa fa-pencil",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp; Edit",
                                    }).get(0).outerHTML + "&nbsp; ";
                                }

                                if (row.isNotCompleted) {
                                    actionButtons += $("<a/>", {
                                        id: "addcomment",
                                        class: "btn btn-default btn-sm",
                                        title: "Add Comment",
                                        href: domain + "minutesOfMeeting/addTaskComment?meetingId=" + row.meetingId + "&id=" + row.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-add-mom",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "fa fa-comments",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp; Comment",
                                    }).get(0).outerHTML + "&nbsp; ";
                                }
                                else {
                                    actionButtons += $("<a/>", {
                                        id: "adddecision",
                                        class: "btn btn-default btn-sm ",
                                        title: "Add Decision",
                                        href: domain + "minutesOfMeeting/addTaskDecision/" + row.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-decision-mom",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "fa fa-comments",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp; Decision",
                                    }).get(0).outerHTML + "&nbsp; ";
                                }

                                actionButtons += $("<a/>", {
                                    id: "viewhistory",
                                    class: "btn btn-default btn-sm",
                                    title: "Comments History",
                                    href: domain + "minutesOfMeeting/viewTaskComments/" + row.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-mom",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "fa fa-history",
                                        style: "color:black"
                                    }).get(0).outerHTML + "&nbsp; History",
                                }).get(0).outerHTML + "&nbsp;  ";

                                if (row.isEditable) {
                                    actionButtons += $("<a/>", {
                                        id: "deletetask",
                                        class: "btn btn-default btn-sm",
                                        title: "Delete",
                                        href: domain + "minutesOfMeeting/DeleteTask/" + row.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-decision-mom",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "fa fa-trash-o",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp; Delete",
                                    }).get(0).outerHTML + "&nbsp;  ";
                                }


                                return actionButtons;
                            }
                        },
                    ],

                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    if (aData.priority == 'Low') {
                        $(nRow).addClass('lowprior_border')
                    }
                    else if (aData.priority == 'Medium') {
                        $(nRow).addClass('mediumprior_border')
                    }
                    else if (aData.priority == 'High') {
                        $(nRow).addClass('highprior_border')
                    }
                    else if (aData.priority == 'Very High') {
                        $(nRow).addClass('veryhighprior_border')
                    }

                    if (aData.taskStatus == 'Pending') {
                        $(nRow).addClass('pending_border')
                    }
                    else if (aData.taskStatus == 'Ongoing') {
                        $(nRow).addClass('ongoing_border')
                    }
                    else if (aData.taskStatus == 'Delayed') {
                        $(nRow).addClass('delayed_border')
                    }
                    else if (aData.taskStatus == 'Completed') {
                        $(nRow).addClass('completed_border')
                    }
                    else if (aData.taskStatus == 'Future Action') {
                        $(nRow).addClass('futuretask_border')
                    }

                },

                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                    $('.dataTables_paginate').hide();
                    $('.dataTables_info').hide();
                    //if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                    //    $('.dataTables_paginate').hide();
                    //}
                    //else {
                    //    $('.dataTables_paginate').show();
                    //}
                    //$('.pagination .active a').css('background-color', '#e99701');
                    //$('.pagination .active a').css('border-color', '#e99701');
                }

            });

            $('#grid-minutesOfMeeting tbody').on('click', 'td button.details-control', function () {
                var tr = $(this).closest('tr');
                var row = grid.row(tr);
                if (row.child.isShown()) {
                    $(this).find('i').removeClass('fa-angle-up').addClass('fa-angle-down').attr('title', 'Show Meetings');
                    row.child.hide();
                }
                else {
                    $(this).find('i').removeClass('fa-angle-down').addClass('fa-angle-up').attr('title', 'Hide Meetings');
                    row.child.show();
                }
            });


        }

        $(document).on("click", ".fullView", function () {
            var id = $(this).data("id");
            $(".showComment").html($(this).data('text'));
        })

        function attachEventCKEditor() {
            if (CKEDITOR === undefined) {
                return;
            }
            CKEDITOR.on('instanceReady', function () {
                $.each(CKEDITOR.instances, function (instance) {
                    CKEDITOR.instances[instance].on('mode', function () {
                        if (this.mode === 'source') {
                            var editable = CKEDITOR.instances[instance].editable();
                            editable.attachListener(editable, 'input', function () {
                                CKEDITOR.instances[instance].updateElement();
                            });
                        } else {
                            CKEDITOR.instances[instance].document.on("keyup", function (e) {
                                CKEDITOR.instances[instance].updateElement();
                            });
                        }
                    });

                    CKEDITOR.instances[instance].document.on("keyup", function (e) {
                        CKEDITOR.instances[instance].updateElement();
                    });
                });
            });
        }


        function initialize() {

            $("#modal-add-mom").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = new Global.FormHelperWithFiles(modal.find("form"),
                    { updateTargetId: "NotificationMessage" },
                    function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload();
                            Global.ShowMessage(result.message, true, "divMessage");
                        } else {
                            form.find("#NotificationMessage").html(result);
                        }
                    });

                form.find('.Calender').datepicker({
                    dateFormat: 'dd/mm/yy',
                    ignoreReadonly: true
                });

                if (form.find('#exampleModalLabel').text() == 'Add Action') {

                    if ($("#Remark")) {
                        CKEDITOR.replace('Remark', { toolbar: "Basic" });
                        attachEventCKEditor('Remark');
                    }
                }
                else {
                    if ($("#Comment")) {
                        try {
                            CKEDITOR.replace('Comment', { toolbar: "Basic" });
                            attachEventCKEditor('Comment');
                        }
                        catch (ex) {
                        }
                    }
                    if ($("#Decision")) {
                        try {
                            CKEDITOR.replace('Decision', { toolbar: "Basic" });
                            attachEventCKEditor('Decision');
                        }
                        catch (ex) {
                        }
                    }
                }

                form.find(".multiple").select2({
                    allowClear: true,
                    width: '100%',
                    matcher: matchCustom
                });

                form.find('.Calender').datepicker({
                    dateFormat: 'dd/mm/yy',
                    ignoreReadonly: true
                });


                //if (form.find('#exampleModalLabel').text() == 'Add Action') {
                //    if ($("#Remark")) {
                //        CKEDITOR.replace('Remark', { toolbar: "Basic" });                      
                //        attachEventCKEditor('Remark');
                //    }
                //}
                //else {
                //    if ($("#Comment")) {
                //        try {
                //            CKEDITOR.replace('Comment', { toolbar: "Basic" });
                //            attachEventCKEditor('Comment');
                //        }
                //        catch (ex) {
                //        }
                //    }
                //    if ($("#Decision")) {
                //        try {
                //        CKEDITOR.replace('Decision', { toolbar: "Basic" }); 
                //        attachEventCKEditor('Decision');
                //        }
                //        catch (ex) {
                //        }
                //    }
                //}

                $('#btn-submit').on('click', function () {
                    form.find('#Comment').each(function () {
                        if ($(this)[0].validationMessage) {
                            this.nextElementSibling.after(this);
                            $('#cke_Comment').css("border", "1px solid red");
                        }
                    });
                    if ($('#Status').val() === "4") {
                        if (confirm('Are you sure, you would like complete this task for all participants?')) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                });

                form.find('.delete-document').click(function (e) {
                    var $button = $(this);
                    var isConfirm = confirm('Are you sure to delete this document?');
                    if (isConfirm) {
                        $.post($button.data('href'), function (result) {
                            if (result == true) {
                                $button.parents('li').remove();
                            }

                        });
                    }
                });

                var toUpdate = form.find('#Id').val();

                if (toUpdate == 0) {
                    $('.divStatus').hide();
                }

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
            function removeEditorBorder(editorName) {
                $('#' + editorName).css("border", "");
            }

            function attachEventCKEditorOLD_1(instance) {

                CKEDITOR.on('instanceReady', function (e) {
                    e.editor.document.on('keyup', function () {
                        CKEDITOR.instances[instance].updateElement();
                        $('#' + instance).removeClass('error').next('label').remove();

                        if ($('#Comment').length>0) {
                            if ($('#Comment')[0].validationMessage) {
                                $('#cke_Comment').css("border", "1px solid red");
                            }
                            else {
                                removeEditorBorder('cke_Comment');
                            }
                        }
                        $('#' + instance).blur();
                    });

                });
            }

            function handelBorder(instance) {
                if (CKEDITOR === undefined) {
                    return;
                }
                CKEDITOR.instances[instance].updateElement();
                $('#' + instance).removeClass('error').next('label').remove();

                if ($('#Comment').length > 0) {
                    if ($('#Comment')[0].validationMessage) {
                        $('#cke_Comment').css("border", "1px solid red");
                    }
                    else {
                        removeEditorBorder('cke_Comment');
                    }
                }
                $('#' + instance).blur();
            }

            //function attachEventCKEditor() {
            //    if (CKEDITOR === undefined) {
            //        return;
            //    }
            //    CKEDITOR.on('instanceReady', function () {
            //        $.each(CKEDITOR.instances, function (instance) {
            //            CKEDITOR.instances[instance].on('mode', function () {
            //                if (this.mode === 'source') {
            //                    var editable = CKEDITOR.instances[instance].editable();
            //                    editable.attachListener(editable, 'input', function () {
            //                        //CKEDITOR.instances[instance].updateElement();
            //                        handelBorder(instance);
            //                    });
            //                } else {
            //                    CKEDITOR.instances[instance].document.on("keyup", function (e) {
            //                        //CKEDITOR.instances[instance].updateElement();
            //                        handelBorder(instance);
            //                    });
            //                }
            //            });

            //            CKEDITOR.instances[instance].document.on("keyup", function (e) {
            //                //CKEDITOR.instances[instance].updateElement();
            //                handelBorder(instance);
            //            });
            //        });
            //    });
            //}


            $("#modal-decision-mom").on('loaded.bs.modal', function () {
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
                form.find(".multiple").select2({
                    allowClear: true,
                    width: '100%',
                    matcher: matchCustom
                });

                form.find('.Calender').datepicker({
                    dateFormat: 'dd/mm/yy',
                    ignoreReadonly: true,
                });

                var toUpdate = form.find('#Id').val();

                if (toUpdate == 0) {
                    $('.divStatus').hide();
                }

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

        }

        function matchCustom(params, data) {
            // If there are no search terms, return all of the data
            if ($.trim(params) === '') {
                return data;
            }

            // Do not display the item if there is no 'text' property
            if (typeof data === 'undefined') {
                return null;
            }

            var searchCritarea;
            if (data.indexOf("(") > -1) {
                var endIndex = data.indexOf("(");
                searchCritarea = data.substring(0, parseInt(endIndex));
                searchCritarea = $.trim(searchCritarea).toLowerCase();
            }
            else {
                searchCritarea = $.trim(data).toLowerCase();
            }


            if (searchCritarea.indexOf(params.toLowerCase()) > -1) {
                return data;
            }
        }

        function completetask() {

            $('.Chktaskstatus').off('click').on('click', function () {

                if ($(this).is(":checked")) {
                    if (parseInt($(this).val()) === 5) {
                        statusId.splice(0, statusId.length);
                        $("#chkAllTasks").prop('checked', false);
                        $("#divalltasks").removeClass('chkAllTasksCls');
                        $("#chkPending").prop('checked', false);
                        $("#divpending").removeClass('chkPendingCls');
                        $("#chkOngoing").prop('checked', false);
                        $("#divongoing").removeClass('chkOngoingCls');
                        $("#chkDelayed").prop('checked', false);
                        $("#divdelayed").removeClass('chkDelayedCls');
                        $("#chkCompleted").prop('checked', false);
                        $("#divcompleted").removeClass('chkCompletedCls');
                    }

                    if (parseInt($(this).val()) === 4) {
                        statusId.splice(0, statusId.length);
                        statusId = [4];
                        $("#chkCompleted").prop('checked', true);
                        $("#divcompleted").addClass('chkCompletedCls');
                        $("#divalltasks").removeClass('chkAllTasksCls');
                        $("#chkAllTasks").prop('checked', false);
                        $("#chkPending").prop('checked', false);
                        $("#divpending").removeClass('chkPendingCls');
                        $("#chkOngoing").prop('checked', false);
                        $("#divongoing").removeClass('chkOngoingCls');
                        $("#chkDelayed").prop('checked', false);
                        $("#divdelayed").removeClass('chkDelayedCls');
                        $("#chkFutureTask").prop('checked', false);
                        $("#divfuturetask").removeClass('chkfuturetaskCls');
                    }


                    if (parseInt($(this).val()) === 0) {
                        statusId.splice(0, statusId.length);
                        statusId = [0];
                        $("#divalltasks").addClass('chkAllTasksCls');
                        $("#chkAllTasks").prop('checked', true);
                        $("#chkPending").prop('checked', false);
                        $("#divpending").removeClass('chkPendingCls');
                        $("#chkOngoing").prop('checked', false);
                        $("#divongoing").removeClass('chkOngoingCls');
                        $("#chkDelayed").prop('checked', false);
                        $("#divdelayed").removeClass('chkDelayedCls');
                        $("#chkCompleted").prop('checked', false);
                        $("#divcompleted").removeClass('chkCompletedCls');
                        $("#chkFutureTask").prop('checked', false);
                        $("#divfuturetask").removeClass('chkfuturetaskCls');
                    }

                    if (parseInt($(this).val()) === 1) {
                        $("#divpending").addClass('chkPendingCls');
                        $("#chkFutureTask").prop('checked', false);
                        $("#divfuturetask").removeClass('chkfuturetaskCls');
                        $("#divalltasks").removeClass('chkAllTasksCls');
                        $("#chkAllTasks").prop('checked', false);
                        $("#chkCompleted").prop('checked', false);
                        $("#divcompleted").removeClass('chkCompletedCls');
                    }
                    else if (parseInt($(this).val()) === 2) {
                        $("#divongoing").addClass('chkOngoingCls');
                        $("#chkFutureTask").prop('checked', false);
                        $("#divfuturetask").removeClass('chkfuturetaskCls');
                        $("#divalltasks").removeClass('chkAllTasksCls');
                        $("#chkAllTasks").prop('checked', false);
                        $("#chkCompleted").prop('checked', false);
                        $("#divcompleted").removeClass('chkCompletedCls');
                    }
                    else if (parseInt($(this).val()) === 3) {
                        $("#divdelayed").addClass('chkDelayedCls');
                        $("#chkFutureTask").prop('checked', false);
                        $("#divfuturetask").removeClass('chkfuturetaskCls');
                        $("#divalltasks").removeClass('chkAllTasksCls');
                        $("#chkAllTasks").prop('checked', false);
                        $("#chkCompleted").prop('checked', false);
                        $("#divcompleted").removeClass('chkCompletedCls');

                    }
                    else if (parseInt($(this).val()) === 5) {
                        $("#divfuturetask").addClass('chkfuturetaskCls');
                    }

                    statusId.push(parseInt($(this).val()));

                    if (statusId.includes(parseInt(0)) || statusId.includes(parseInt(1)) || statusId.includes(parseInt(2)) || statusId.includes(parseInt(3)) || statusId.includes(parseInt(4))) {

                        console.log(statusId);
                        for (var i = 0; i <= statusId.length; i++) {
                            if (statusId[i] === 5 || statusId[i] === 0 || statusId[i] === 4) {
                                statusId.splice(i, 1);
                            }
                        }
                    }
                    loadGrid();
                }
                else {
                    if (statusId != '') {

                        for (var i = 0; i <= statusId.length; i++) {

                            if (statusId[i] === parseInt($(this).val())) {
                                statusId.splice(i, 1);
                            }

                            if (parseInt($(this).val()) === 0) {
                                statusId = [0];
                                $("#chkAllTasks").prop('checked', true);
                                $("#divalltasks").addClass('chkAllTasksCls');
                                $("#chkPending").prop('checked', false);
                                $("#divpending").removeClass('chkPendingCls');
                                $("#chkOngoing").prop('checked', false);
                                $("#divongoing").removeClass('chkOngoingCls');
                                $("#chkDelayed").prop('checked', false);
                                $("#divdelayed").removeClass('chkDelayedCls');
                                $("#chkCompleted").prop('checked', false);
                                $("#divcompleted").removeClass('chkCompletedCls');
                                $("#chkFutureTask").prop('checked', false);
                                $("#divfuturetask").removeClass('chkfuturetaskCls');
                            }
                            else if (parseInt($(this).val()) === 1) {
                                $("#chkPending").prop('checked', false);
                                $("#divpending").removeClass('chkPendingCls');
                            }
                            else if (parseInt($(this).val()) === 2) {
                                $("#chkOngoing").prop('checked', false);
                                $("#divongoing").removeClass('chkOngoingCls');
                            }
                            else if (parseInt($(this).val()) === 3) {
                                $("#chkDelayed").prop('checked', false);
                                $("#divdelayed").removeClass('chkDelayedCls');
                            }
                            else if (parseInt($(this).val()) === 4) {
                                statusId = [4];
                                $("#chkAllTasks").prop('checked', false);
                                $("#divalltasks").removeClass('chkAllTasksCls');
                                $("#chkPending").prop('checked', false);
                                $("#divpending").removeClass('chkPendingCls');
                                $("#chkOngoing").prop('checked', false);
                                $("#divongoing").removeClass('chkOngoingCls');
                                $("#chkDelayed").prop('checked', false);
                                $("#divdelayed").removeClass('chkDelayedCls');
                                $("#chkCompleted").prop('checked', false);
                                $("#divcompleted").removeClass('chkCompletedCls');
                                $("#chkFutureTask").prop('checked', false);
                                $("#divfuturetask").removeClass('chkfuturetaskCls');
                                $("#chkCompleted").prop('checked', true);
                                $("#divcompleted").addClass('chkCompletedCls');
                            }
                            else if (parseInt($(this).val()) === 5) {
                                $("#chkFutureTask").prop('checked', false);
                                $("#divfuturetask").removeClass('chkfuturetaskCls');
                            }
                            //else {
                            //    statusId = [0];  
                            //    $("#chkAllTasks").prop('checked', true);
                            //    $("#divalltasks").addClass('chkAllTasksCls');
                            //}
                        }
                    }

                    if (statusId.length === 0) {
                        statusId = [0];
                        $("#chkAllTasks").prop('checked', true);
                        $("#divalltasks").addClass('chkAllTasksCls');
                    }
                    loadGrid();
                }
            });
            IsFromToDO();

        }


        $this.init = function () {

            if (jQuery.inArray("0", statusId)) {
                $("#chkAllTasks").prop('checked', true);
                $("#divalltasks").addClass('chkAllTasksCls');
            }
            //removeEditorBorder('cke_Comment');          
            completetask();
            if (isToDo===false) {
                loadGrid();
            }
            initialize();

        };

        $("#Paticipants").on('change', function () {
            filterParticipants = $(this).val();
            loadGrid();
        });
        function GetURLParameterValue(sParam) {

            var sPageURL = window.location.search.substring(1);

            var sURLVariables = sPageURL.split('&');

            for (var i = 0; i < sURLVariables.length; i++) {

                var sParameterName = sURLVariables[i].split('=');

                if (sParameterName[0] == sParam) {
                    return sParameterName[1];
                }
            }
        }
        function IsFromToDO() {
            var fromtl = GetURLParameterValue("tl");
            if (fromtl === "true") {
                var status = GetURLParameterValue("st");
                if (status === "Completed") {
                    $("#chkAllTasks").prop('checked', false)
                    $("#chkCompleted").prop('checked', true);
                    isToDo = true; // setting true to avoid the repeated call to loadGrid() method in case coming from task list
                    $('.Chktaskstatus:checked').click();
                    //$("#chkCompleted").trigger('click');
                }
            }

        }
    }
    $(function () {
        var self = new viewtask();
        self.init();
    });



}(jQuery));