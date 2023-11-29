var rowNo = -1;
(function ($) {

    function Index() {
        var test = "oo";
        var openRow = [];
        var $this = this, grid;

        function format(meetings, mId, lastMeetingDate, firstMeetingId = 0) {
            var returnTable = '';
            if (meetings.length > 0) {
                var mCount = meetings.length;
                returnTable += '<table class="table table-hover table-responsive inner-table">' +
                    '<tr><th>#</th><th>Meeting Title</th><th>Venue</th><th>Date Of Meeting (Meeting Time)</th><th>Participants/Groups</th><th>Author By</th><th>Chaired By</th><th>Meeting Agenda</th><th>Action</th></tr>';

                $.each(meetings, function (i, value) {

                    var doc = '';

                    $.each(meetings[i].momDocuments, function (d, value) {
                        //var doclower = value.DocumentPath;
                        //doc += `<a href="/Upload/MomDocument/${value.documentPath}" class="btn-link" download="download" title='${value.DocumentPath}'> <i class='${(doclower.includes(".ppt") ? "fa fa-file-powerpoint-o" : doclower.includes(".doc") ? "fa fa-file-word-o" : doclower.includes(".xls") ? "fa fa-file-excel-o" : doclower.includes(".txt") ? "fa fa-file-text-o" : doclower.includes(".rar") || doclower.includes(".zip") ? "fa fa-file-zip-o" : "fa fa-file-image-o")}'></i> </a>`;
                        doc += value.documentPath + '<br />';
                    });

                    returnTable += '<tr>' +
                        '<td width="2%;">' + (i + 1) + '</td>' +
                        '<td width="22%;"><strong>' + meetings[i].meetingTitle + '</strong><br/><br/>' + doc + '</td>' +

                        '<td width="10%;">' + meetings[i].venue + '</td>' +
                        '<td width="10%;">' + meetings[i].dateOfMeeting + '</td>';
                    if (meetings[i].modalRequired) {
                        returnTable += '<td width="10%;"> <div id="divShowMore_"' + meetings[i].id + '" class="content divmore" style="max-height: 200px; max-width: auto; overflow-x: hidden; overflow-y: 100px;">' + meetings[i].participants + '</div>';
                        returnTable += '<div class=""><a href="minutesOfMeeting/showmeetingparticipant?masterId=' + meetings[i].meetingMasterId + '&id=' + meetings[i].id + '" class="" title="Full view"  data-toggle="modal" data-target="#modal-mom-participants" data-backdrop="static" style="color: red;margin-right:24px;margin-top:3px;"><i class="fa fa-desktop"></i> <span>Full view</span></a></div>';
                        returnTable += '</td>';
                    }
                    else {
                        returnTable += '<td width="10%;">' + meetings[i].participants + '</td>';
                    }
                    returnTable += '<td width="8%;">' + meetings[i].authorBy + '</td>' +
                        '<td width="8%;">' + meetings[i].chairedBy + '</td>';
                    //'<td width="10%;">' + meetings[i].aganda + '</td>' +

                    //if (meetings[i].showAgendaInModle == true) {
                    //    returnTable += '<td width="10%;"> <div id="divShowMore_"' + meetings[i].id + '" class="content">' + meetings[i].aganda + '</div>';
                    //    returnTable += '<div class="pull-right" style="margin-top:15px;"><a class="" title="Read more" href="minutesOfMeeting/showmeetingagenda?masterId=' + meetings[i].meetingMasterId + '&id=' + meetings[i].id + '" data-toggle="modal" data-target="#modal-mom-agenda" data-backdrop="static" style="color: red;"><i class="fa fa-search"></i> <span>Read more</span></a></div>';
                    //    returnTable += '</td>';
                    //} else {
                    //    returnTable += '<td width="10%;">' + meetings[i].aganda + '</td>';
                    //}


                    //if (meetings[i].showAgendaInModle == true) {
                    returnTable += '<td width="10%;"> <div id="divShowMore_"' + meetings[i].id + '" class="content divmore" style="max-height: 200px;overflow-x: hidden; overflow-y: auto;">' + meetings[i].agendaFullText + '</div>';
                    returnTable += '<div class="pull-right divmoreinner"><a class="" title="Full view" href="minutesOfMeeting/showmeetingagenda?masterId=' + meetings[i].meetingMasterId + '&id=' + meetings[i].id + '" data-toggle="modal" data-target="#modal-mom-agenda" data-backdrop="static" style="color: red;margin-right:24px;margin-top:3px;"><i class="fa fa-desktop"></i> <span>Full view</span></a></div>';
                    returnTable += '</td>';
                    //} else {
                    //    returnTable += '<td width="10%;">' + meetings[i].aganda + '</td>';
                    //}


                    //returnTable += '<td >' + doc + '</td>';

                    returnTable += '<td>';
                    returnTable += '<a class="btn btn-default btn-sm m2" title="Edit Meeting" href="minutesOfMeeting/ViewMeeting?masterId=' + meetings[i].meetingMasterId + '&id=' + meetings[i].id + '"data-toggle= "modal" data-target="#modal-view-mom" data-backdrop="static"><i class="fa fa-eye"></i> View Meeting</a>';
                    if (meetings[i].isEditable == true) {
                        returnTable += '<a class="btn btn-default btn-sm m2" title="Edit Meeting" href="minutesOfMeeting/addeditmeeting?masterId=' + meetings[i].meetingMasterId + '&id=' + meetings[i].id + '"data-toggle= "modal" data-target="#modal-add-mom" data-backdrop="static"><i class="fa fa-pencil"></i> Edit Meeting</a>';
                        //returnTable += '<a class="btn btn-default btn-sm m2" title="delete Meeting" href="minutesOfMeeting/deleteMeeting?id=' + meetings[i].id + '" data-toggle= "modal" data-target="#modal-delete-mom" data-backdrop="static"><i class="fa fa-trash-o"></i> Delete Meeting</a>';
                    }

                    if (meetings[i].isEditable == true && meetings[i].id === firstMeetingId) {
                        returnTable += '<a class="btn btn-default btn-sm m2" title="delete Meeting" href="minutesOfMeeting/deleteMeeting?id=' + meetings[i].id + '" data-toggle= "modal" data-target="#modal-delete-mom" data-backdrop="static"><i class="fa fa-trash-o"></i> Delete Meeting</a>';
                    }


                    if ((i + 1) === 1) {
                        returnTable += '<a class="btn btn-default btn-sm m2" title="View/Add Action"href="minutesOfMeeting/viewtask/' + meetings[i].id + '"><i class="fa fa-plus"></i>&nbsp;Start Meeting</a>';
                    }
                    else {
                        returnTable += '<a class="btn btn-default btn-sm m2" title="View/Add Action"href="minutesOfMeeting/viewtask/' + meetings[i].id + '"><i class="fa fa-plus"></i>&nbsp;View Action</a>';
                    }

                    returnTable += '<a class="btn btn-default btn-sm sendEmail m2" id=' + meetings[i].id + ' title="Send Email" href="minutesOfMeeting/SendMinutesOfMeetingEmail/' + meetings[i].id + '" data-toggle= "modal" data-target="#modal-add-mom" data-backdrop="static"><i class="fa fa-envelope-o"></i>&nbsp;Send Email</a>';
                    returnTable += '<a class="btn btn-default btn-sm m2" title="Download PDF" href="minutesOfMeeting/DownloadPDF?masterId=' + meetings[i].meetingMasterId + '&id=' + meetings[i].id + '"><i class="fa fa-file-pdf-o"></i> Download PDF</a></td>' +

                        '</tr>';
                });
                returnTable += '</table>';
            }
            else {
                returnTable = '<table class="table table-hover table-responsive">' +
                    '<tr>' +
                    '<td class="no-meeting-msg">No Meetings Added Yet..!!</td>' +
                    '</table>';
            }
            return returnTable;
        }
        function loadGrid() {
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-minutesOfMeeting', {
                serverSide: true,
                destroy: true,
                ordering: false,
                "pageLength": 15,
                "bFilter": true,
                "bAutoWidth": false,
                "bLengthChange": false,
                "processing": true,
                //"language": {
                //    processing: '<i class="fa fa-spinner fa-spin fa-3x fa-fw"></i><span class="sr-only">Loading...</span> '
                //},
                 
                ajax:
                {
                    url: domain + "minutesOfMeeting/index",
                    type: "Post"
                },

                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "52%", "targets": 1 },
                    { "width": "16%", "targets": 2 }
                ],
                columns:
                    [
                        {
                            name: "RowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true,

                        },
                        {
                            name: "MeetingTitle", data: "meetingTitle", title: "Meeting Topic", sortable: false, searchable: true, visible: true, class: 'meeting-topic',
                            render: function (data, type, row, meta) {
                                var actionButtons = '';

                                actionButtons += $("<span/>", {
                                    id: "" + row.id + "",
                                }).get(0).outerHTML + "";


                                actionButtons += $("<button/>", {
                                    id: "view",
                                    class: "btn btn-default btn-sm details-control",
                                    title: "Show Meetings",
                                    html: $("<i/>", {
                                        class: "fa fa-angle-down",
                                        style: "color:black"
                                    }).get(0).outerHTML + "",// + "&nbsp; View",
                                }).get(0).outerHTML + "&nbsp;&nbsp;&nbsp;&nbsp;" + row.meetingTitle + "&nbsp;&nbsp;(&nbsp;" + row.lastMeetingDate + "&nbsp;)";

                                if (row.editAllowed) {
                                    actionButtons += $("<a/>", {
                                        id: "addEdit",
                                        class: "m5",
                                        title: "Edit",
                                        href: domain + "minutesOfMeeting/addedit/" + row.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-add-mom",
                                        'data-backdrop': "static",
                                        html: $(" <i/>", {
                                            class: "fa fa-pencil btn-edit",
                                            style: "color:black"
                                        }).get(0).outerHTML + "",
                                    }).get(0).outerHTML + "&nbsp; ";
                                }
                                return actionButtons;
                            }
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var s = row;
                                var actionButtons = '';
                                actionButtons += $("<a/>", {
                                    id: "view",
                                    class: "btn btn-default btn-sm",
                                    title: "New Meeting",
                                    href: domain + "minutesOfMeeting/addeditmeeting?masterId=" + row.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-mom",
                                    'data-backdrop': "static",
                                    'onclick': "st(" + meta.row + ")",
                                    html: $("<i/>", {
                                        class: "fa fa-plus",
                                        style: "color:black"
                                    }).get(0).outerHTML + "&nbsp; New Meeting",
                                }).get(0).outerHTML + "&nbsp; ";

                                //if (row.editAllowed) {
                                //    actionButtons += $("<a/>", {
                                //        id: "addEdit",
                                //        class: "btn btn-default btn-sm",
                                //        title: "Edit",
                                //        href: domain + "minutesOfMeeting/addedit/" + row.id,
                                //        'data-toggle': "modal",
                                //        'data-target': "#modal-add-mom",
                                //        'data-backdrop': "static",
                                //        html: $("<i/>", {
                                //            class: "fa fa-pencil",
                                //            style: "color:black"
                                //        }).get(0).outerHTML + "&nbsp; Edit",
                                //    }).get(0).outerHTML + "&nbsp; ";
                                //}

                                actionButtons += $("<a/>", {
                                    id: "viewDecision",
                                    class: "btn btn-default btn-sm",
                                    title: "Decision",
                                    href: domain + "minutesOfMeeting/ViewMeetingDecision?masterId=" + row.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-view-mom",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "fa fa-comment",
                                        style: "color:black"
                                    }).get(0).outerHTML + "&nbsp; Decision",
                                }).get(0).outerHTML + "&nbsp; ";




                                //actionButtons += $("<a/>", {
                                //    id: "exportmeeting",
                                //    class: "btn btn-default btn-sm",
                                //    title: "Export",
                                //    href: domain + "minutesOfMeeting/ExportToExcel?masterId=" + row.id,
                                //    html: $("<i/>", {
                                //        class: "fa fa-plus",
                                //        style: "color:black"
                                //    }).get(0).outerHTML + "&nbsp; Export",
                                //}).get(0).outerHTML + "&nbsp; ";

                                return actionButtons;
                            }
                        },

                    ],
                //"fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                //    if (aData.momMeetings != []) {
                //        var tr = nRow;
                //        var row = grid.row(tr);

                //        if (openRow.length > 0 && jQuery.inArray(aData.id, openRow) > -1) {
                //            row.child('');

                //        }
                //        else {
                //            row.child('');
                //        }
                //    }
                //},
                "preDrawCallback": function (oSetting) {
                    $('#grid-minutesOfMeeting_processing').hide()

                },
                
                    //RowClick(rowNo)
               
                "fnDrawCallback": function (oSettings) {
                    RowClick(rowNo)
                    hidefullviewanchor();
                    $('.divoverlay').addClass('hide');
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');

                    var $tableContainer = $(this).parent();
                    $("<span/>", { html: " &nbsp; " }).appendTo($tableContainer.find("#grid-minutesOfMeeting_filter"));
                    $tableContainer.find("input[type=search]").addClass("form-control");

                }
            });

            $('#grid-minutesOfMeeting tbody').on('click', 'tr > .meeting-topic', function () {
                hidefullviewanchor();
                var tr = $(this).closest('tr');
                var row = grid.row(tr);
                //var rowId = $(this).prev('td').text();

                var topicId = $(this).find('span').attr('id');
                if (row.child.isShown()) {
                    $(this).find('i').not('.btn-edit').removeClass('fa-angle-up').addClass('fa-angle-down').attr('title', 'Show Meetings');
                    row.child('').hide();
                    rowNo = -1;

                    //if (!(rowNo > -1)) {
                    //    row.child('').hide();
                    //    rowNo = -1;
                    //}
                    //else {
                    //    rowNo = -1;

                    //}

                   
                }
                else {
                    $(this).find('i').not('.btn-edit').removeClass('fa-angle-down').addClass('fa-angle-up').attr('title', 'Hide Meetings');
                    //$('#grid-minutesOfMeeting_processing').show()
                    $.get(domain + "minutesOfMeeting/GetMOMByID?ID=" + row.data().id, function (data) {
                        row.child(format(data, row.data().id, row.data().lastMeetingDate, row.data().firstMeetingId)).show();
                        $('#grid-minutesOfMeeting_processing').hide()

                    });
                    openRow.push(parseInt(topicId));
                }


            });


        }
        function removeEditorBorder(editorName) {
            $('#' + editorName).css("border", "");
        }

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

            $("#modal-mom-agenda").on('loaded.bs.modal', function () {
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
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
            $("#modal-mom-participants").on('loaded.bs.modal', function () {
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
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
            //First modal
            $("#modal-add-mom").on('loaded.bs.modal', function () {

                var modal = $(this);
                var form = new Global.FormHelperWithFiles(modal.find("form"),
                    { updateTargetId: "NotificationMessage" },
                    function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            $('.divoverlay').removeClass('hide');
                            grid.ajax.reload();
                            Global.ShowMessage(result.message, true, "divMessage");
                           /// RowClick(rowNo)

                        } else {
                            form.find("#NotificationMessage").html(result);
                            $('.divoverlay').addClass('hide');
                        }
                    });

                if ($("#Agenda") && CKEDITOR !== undefined) {
                    try {
                        CKEDITOR.replace('Agenda', { toolbar: "Basic" });
                    } catch (e) {
                        console.log(e);
                    }
                }

                if ($("#Notes") && CKEDITOR !== undefined) {
                    try {
                        CKEDITOR.replace('Notes', { toolbar: "Basic" });
                    } catch (e) {
                        console.log(e);
                    }
                }

                attachEventCKEditor();


                form.find('.clstimepicker').mask("99:99", { clearIncomplete: false });


                form.find('.clstimepicker').blur(function () {

                    var currentMask = '';
                    if ($(this).val().length > 0) {

                        var arr = $(this).val().split('');
                        if (arr[1] == '_' && arr[0] != '_') {
                            arr[1] = arr[0];
                            arr[0] = '0';
                        }
                        if (arr[4] == '_' && arr[3] != '_') {
                            arr[4] = arr[3];
                            arr[3] = '0';
                        }
                        $(arr).each(function (index, value) {
                            if (value == '_')
                                arr[index] = '0';
                            currentMask += arr[index];
                        });

                        var time = currentMask.split(':');
                        if (time[0] == "" || time[0] == 'undefined' || time[0] == '__' || parseInt(time[0]) > 23 && (time[1] == "" || time[1] == 'undefined' || time[1] == '__' || parseInt(time[1]) > 59)) {
                            alert("Please enter correct working time");
                            time[0] = "00";
                            time[1] = "00";
                        }
                        else {
                            if (time[0] == "" || time[0] == 'undefined' || time[0] == '__' || parseInt(time[0]) > 23) {
                                alert("Please enter correct working hours");
                                time[0] = "00";
                                time[1] = "00";
                            }

                            if (time[1] == "" || time[1] == 'undefined' || time[1] == '__' || parseInt(time[1]) > 59) {
                                alert("Please enter correct working minutes");
                                time[0] = "00";
                                time[1] = "00";
                            }

                        }
                        var newVal = time[0] + ":" + time[1];
                        if (newVal.indexOf("undefined") != -1) {
                            newVal = "00:00";
                            alert("Please enter meeting hours (24 hours format)");
                        }


                        $(this).val(newVal);
                    }
                });
                form.find('input[type=radio][name=ParticipantType]').off("change").on('change', function () {
                    if (this.value == 'Individual' && this.checked) {
                        $('.ParticipantClass').removeClass('hidden');
                        $('.GroupClass').addClass('hidden');
                        $('#lblParticipant').text("Participants");
                        $('#Paticipants').attr('data-msg-required', '*required').attr('data-rule-required', 'true');
                        $('#Groups').removeAttr('data-msg-required', '*required').removeAttr('data-rule-required', 'true').removeClass('error');
                        $('#ParticipantType').val(this.value);
                    }
                    else if (this.value == 'Group' && this.checked) {
                        $('.ParticipantClass').addClass('hidden');
                        $('.GroupClass').removeClass('hidden');
                        $('#lblParticipant').text("Groups");
                        $('#Paticipants').removeAttr('data-msg-required', '*required').removeAttr('data-rule-required', 'true').removeClass('error');
                        $('#Groups').attr('data-msg-required', '*required').attr('data-rule-required', 'true');

                        $('#ParticipantType').val(this.value);
                    }
                });

                form.find(".multiple").select2({
                    allowClear: true,
                    width: '100%',
                    matcher: matchCustom
                });

                form.find('.Calender').datetimepicker({
                    format: 'DD/MM/YYYY',
                    ignoreReadonly: true
                    // maxDate: new Date()
                });

                if (form.find('input[type=hidden][id=ParticipantType]').val() == 'Individual') {
                    form.find('input[type=radio][name=ParticipantType][datattr="Individual"]').prop("Checked", "Checked");
                    form.find('input[type=radio][name=ParticipantType][datattr="Individual"]').change();
                }
                else {
                    form.find('input[type=radio][name=ParticipantType][datattr="Group"]').prop("Checked", "Checked");
                    form.find('input[type=radio][name=ParticipantType][datattr="Group"]').change();
                }





                //if (form.find('input[type=hidden][id=ParticipantType]').val() == "Individual") {
                //    form.find('input[type=radio][id=Individual]').change();
                //} else if (form.find('input[type=hidden][id=ParticipantType]').val() == "Group") {
                //    form.find('input[type=radio][id=Group]').change();
                //}

                var toUpdate = $('#Id').val();
                if (toUpdate > 0) {
                    form.find('input[type=radio][name=ParticipantType]').attr('disabled', "true");
                    // form.find('#AuthorByUID').attr('disabled', "true");
                }
                else {
                    //$('#Paticipants').attr('data-msg-required', '*required').attr('data-rule-required', 'true');
                }

                var ids = [];

                if (form.find("#hdnSelectedGroup").val() != undefined) {
                    ids = form.find("#hdnSelectedGroup").val().split(';');
                }
                $.each(ids, function (i, value) {
                    form.find('input[value="' + value + '"]').prop('checked', true);
                });
                form.find("#selectedGroup").val(ids);
                // if (form.find('#hdnParticipantType').val() == "Group") {
                form.find('.clsGroupForEmail').off('change').on('change', function () {
                    if ($(this).is(":checked")) {
                        ids.push($(this).val());
                    }
                    else if ($(this).is(":not(:checked)")) {
                        for (var i = 0; i <= ids.length; i++) {
                            if (ids[i] == $(this).val()) {
                                ids.splice(i, 1);
                            }
                        }
                    }

                    form.find("#selectedGroup").val(ids);
                });

                //}


                function CKupdate() {
                    for (instance in CKEDITOR.instances)
                        CKEDITOR.instances[instance].updateElement();
                }


                var partcipants = [];
                var checkEmail = '';
                form.find('button[type=submit]').click(function (e) {
                    CKupdate();
                    if (form.find("#selectedParticpants").val()) {
                        partcipants = form.find("#selectedParticpants").val().split(';');
                    }
                    $.each(partcipants, function (i, value) {
                        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
                        if (value != "") {
                            if (regex.test(value) == false) {
                                checkEmail = 'false';
                            }
                            else {
                                checkEmail = 'true';
                            }
                        }
                    });

                    if (checkEmail == 'false') {
                        alert("please enter valid email id");
                        return false;
                    }
                    else
                        return true;
                });


                form.find('.delete-document').click(function (e) {
                    var $button = $(this);
                    var isConfirm = confirm('Are you want to sure to delete this document?');
                    if (isConfirm) {
                        $.post($button.data('href'), function (result) {
                            if (result == true) {
                                $button.parents('li').remove();
                            }

                        });
                    }
                });



            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });


            //second modal
            $("#modal-add-topic").on('loaded.bs.modal', function () {

                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "NotificationMessage"
                }, null,
                    function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            $('.divoverlay').removeClass('hide');
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

            $("#modal-view-mom").on('loaded.bs.modal', function () {

                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "NotificationMessage"
                });
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-delete-mom").on('loaded.bs.modal', function () {

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

        $this.init = function () {
            loadGrid();
            initialize();
            //$(document).on('click','.sendEmail', function () {

            //    var id =$(this).attr('id');
            //    $.ajax({
            //        type: "GET",
            //        url: domain + 'minutesOfMeeting/SendMinutesOfMeetingEmail',
            //        data: { id: id },
            //        success: function (result) {
            //            $("#divMessageMail").removeClass('hide');
            //        }
            //    });
            //});

            $('body').click(function () {
                $('#divMessageMail').addClass('hide');
            });
        };
        function hidefullviewanchor() {
            setTimeout(function () {
                $(".divmore").each(function () {
                    if ($(this).height() < 200) {
                        $(this).next(".divmoreinner").hide();
                    }
                }
                );
            }, 2)

        }
    }
    $(function () {
        var self = new Index();
        self.init();
    });

    function showMoreAgenda(divShowMore) {
        alert(2);
        //$('#' + divShowMore).css("border", "");
    }

}(jQuery));
function st(p) {
    rowNo = p
}
function RowClick(p) {
    if (p>-1) {
        $("#grid-minutesOfMeeting").find("tbody tr:eq(" + p + ") td").trigger('click')

    }
}