(function () {

    function Index() {
        var $this = this, $meetingCalendar;

        function initializeForm() {

            var currentview = 'month';
            var currentDate = moment();

            var selectedEvent = null;
            var $confRoom = $("#CRId");
            var bookingModal = $('#modal_booking');
            var detailModal = $('#modal_details')

            $('.date').datetimepicker({
                minDate: new Date(),
                stepping: 30,
                enabledHours: [9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20],
                format: 'DD/MM/YYYY HH:mm'
            });

            $confRoom.change(function () {
                $('#calender').fullCalendar('refetchEvents');
            });

            $('#calender').fullCalendar({
                minTime: "07:00:00",
                maxTime: "21:00:00",
                slotDuration: '00:30:00',
                slotLabelInterval: 30,
                slotMinutes: 30,
                slotLabelFormat: 'hh:mmA',
                firstDay: 1,
                allDaySlot: false,
                timezone: 'local',
                contentHeight: 808,
                defaultDate: currentDate,
                defaultView: currentview,
                timeFormat: 'H(:mm)',
                header: {
                    left: 'prev,next today',
                    center: 'title',
                    right: 'month,basicWeek,agenda'
                },
                buttonText: {
                    agenda: 'day'
                },
                slotEventOverlap: false,
                disableResizing: false,
                eventDurationEditable: false,
                eventStartEditable: false,
                disableDragging: false,
                eventLimit: true,
                views: {
                    month: {
                        eventLimit: 3 // adjust to 2 only for month
                    }
                },
                eventColor: '#378006',
                eventSources: [{
                    url: domain + 'MeetingRoom/GetEvents',
                    data: function () {
                        return { id: $confRoom.val() };
                    },
                    success: function (data) {
                        var events = [];
                        //console.log(data)
                        $.each(data, function (i, v) {
                            events.push({
                                eventID: v.eventID,
                                title3: v.subjectTitle,
                                title: v.meetingSubject + ' ' + v.meetingTime + '<br> [' + v.location + "]" + "  <b>" + v.createdBy + "</b> (Host)",
                                title1: v.subject,
                                title2: v.meetingSubject,
                                location: v.location,
                                conferenceRoomId: v.conferenceRoomId,
                                createdby: v.createdBy,
                                description: v.description,
                                start: v.start,
                                end: v.end,
                                attendee: v.attendee,
                                color: 'white',
                                className: v.themeColor,
                                isdeletedallow: v.isDeletedAllow,
                                timedifference: v.timeDifference
                            });
                        });
                        //console.log(events)
                        return events;
                    },
                    failure: function () {
                        alert('There was an error while fetching meetings!');
                    }
                }],

                displayEventTime: false,
                columnHeaderFormat: {
                    basicWeek: 'MMM D YYYY'
                },
                viewRender: function (view, element) {
                    currentview = view.name;
                    currentDate = view.intervalStart;
                },
                eventClick: function (calEvent, jsEvent, view) {
                    selectedEvent = calEvent;
                    var $description = $('<div/>');
                    $description.append('<h5 class="schedule-title">' + calEvent.title2 + '</h5>');
                    $description.append('<div class="schedule-time meetingtime"><span class="fa fa-clock-o"></span>' + calEvent.start.format("DD/MM/YYYY") + ' <span class="meeting-clock-hr">' + calEvent.start.format("hh:mm A") + ' | ' + calEvent.timedifference + '</span></div>');
                    $description.append('<div class="schedule-time"><span class="fa fa-map-marker"></span>' + calEvent.location + '</div>');
                    $description.append('<div class="schedule-time"><span class="fa fa-user"></span>' + calEvent.createdby + '</div>');
                    $description.append('<div class="schedule-time"><span class="fa fa-group"></span><b> Attendees: </b>' + calEvent.attendee + '</div>');
                    $description.append('<div class="schedule-time"><span class="fa fa-pencil-square-o"></span><b> Agenda: </b>' + calEvent.description + '</div>');

                    detailModal.find('#pDetails').empty().html($description);
                    var btns = "";
                    if (calEvent.isdeletedallow) {
                        btns = '<button id="btnCancel" class="btn cancelmeeting-btn pull-left"><i class="fa fa-close"></i> Cancel Meeting</button>';
                        btns += '<button id="btnEdit" class="btn btn-default"><i class="fa fa-pencil"></i> Edit</button>';
                    }
                    btns += '<button type="button" class="btn btn-default" data-dismiss="modal">Close</button>';
                    detailModal.find('#editCancelBtns').empty().html(btns);
                    detailModal.modal();
                },
                eventRender: function (event, element) {
                    element.find('.fc-title').html(event.title);/*For Month,Day and Week Views*/
                },
                selectable: true,
                select: function (start, end) {
                    var check = new Date(start).toDateString("yyyy-MM-dd");
                    var today = new Date().toDateString("yyyy-MM-dd");

                    if (start > new Date() || check == today) {
                        selectedEvent = {
                            eventID: 0,
                            title: '',
                            description: '',
                            start: start,
                            end: end,
                            allDay: false,
                            color: '',
                            attendee: '',
                            conferenceRoomId: $confRoom.val()
                        };
                        openAddEditForm();
                        $('#calendar').fullCalendar('unselect');
                    }
                },
                editable: true,
                eventDrop: function (event) {
                    var data = {
                        EventID: event.eventID,
                        Subject: event.title,
                        AttendeeName: event.attendee,
                        Start: event.start.format('DD/MM/YYYY hh:mm A'),
                        End: event.end != null ? event.end.format('DD/MM/YYYY hh:mm A') : null,
                        Description: event.description,
                        ThemeColor: event.color,
                        IsFullDay: event.allDay,
                    };
                    SaveEvent(data);
                }

            });

            //convertTo12Hour('14:00');
            function convertTo12Hour(oldFormatTime) {
                var oldFormatTimeArray = oldFormatTime.split(":");
                var HH = parseInt(oldFormatTimeArray[0]);
                var min = parseInt(oldFormatTimeArray[1]);

                var AMPM = HH >= 12 ? "PM" : "AM";
                var hours;
                if (HH == 0) {
                    hours = HH + 12;
                } else if (HH > 12) {
                    hours = HH - 12;
                } else {
                    hours = HH;
                }

                var newFormatTime = (hours > 9 ? hours : '0' + hours) + ":" + (min > 9 ? min : '0' + min) + AMPM;
                return newFormatTime;
            }

            detailModal.on('click', '#btnEdit', function () {
                detailModal.modal('hide');
                setTimeout(function () {
                    openAddEditForm();
                }, 500);

            });

            detailModal.on('click', '#btnCancel', function () {
                if (selectedEvent != null && confirm('Are you sure you want to cancel this meeting?')) {
                    $.ajax({
                        type: "POST",
                        url: domain + 'MeetingRoom/DeleteEvent',
                        data: { eventID: selectedEvent.eventID },
                        success: function (data) {
                            if (data.status) {
                                $('#calender').fullCalendar('refetchEvents');
                                detailModal.modal('hide');
                            }
                        },
                        error: function () {
                            alert('Failed');
                        }
                    })
                }
            });

            $('#dtp1,#dtp2').datetimepicker({
                format: 'DD/MM/YYYY hh:mm A'
            });

            $('#chkIsFullDay').change(function () {
                if ($(this).is(':checked')) {
                    $('#divEndDate').hide();
                }
                else {
                    $('#divEndDate').show();
                }
            });

            function openAddEditForm() {

                bookingModal.find("#divBody").empty();

                if (selectedEvent) {
                    $.ajax({
                        type: "GET",
                        url: domain + 'MeetingRoom/AddTimeSlot',
                        data: { eventID: selectedEvent.eventID },
                        success: function (result) {
                            bookingModal.find("#divBody").empty().html(result);
                            bookingModal.find('#hdEventID').val(selectedEvent.eventID);
                            bookingModal.find('#txtSelectedDate').val(selectedEvent.start.format('DD/MM/YYYY'));
                            bookingModal.find('#txtCRoom').val(selectedEvent.conferenceRoomId);
                            bookingModal.find('#txtTitle').val(selectedEvent.title2);
                            bookingModal.find('#txtDescription').val(selectedEvent.description);
                            bookingModal.find('#txtAttendee').val(selectedEvent.attendee);

                            var startTime = convertTo12Hour(selectedEvent.start.format('HH:mm'));
                            var endTime = convertTo12Hour(selectedEvent.end.format('HH:mm'));
                            var selectionStart = false, selectionEnd = false;

                            bookingModal.find('input.clsCheck[type=checkbox]').each(function () {

                                if (!selectionStart || !selectionEnd) {
                                    var slotInput = $(this);
                                    var slot = slotInput.val().split("-");
                                    var startSlot = slot[0].trim();
                                    var endSlot = slot[1].trim();

                                    if (startSlot === startTime) {
                                        slotInput.prop('checked', true).change();
                                        selectionStart = true;

                                        if (endSlot === endTime) {
                                            selectionEnd = true;
                                        }
                                    }
                                    else if (selectionStart) {
                                        slotInput.prop('checked', true).change();

                                        if (endSlot === endTime) {
                                            selectionEnd = true;
                                        }
                                    }
                                }
                            });

                            var selecteventdate = $('#txtSelectedDate').val();
                            //var selecteventdate = selectedEvent.start.format('DD/MM/YYYY');
                            console.log("selecteventdate from selectedEvent.start.format", selectedEvent.start.format('DD/MM/YYYY'));

                            //var selecteventdateNew = $('#txtSelectedDate').val();
                            console.log("selecteventdate from txtSelectedDate", selecteventdate);
                            //console.log("selecteventdateNew", selecteventdateNew)

                            if (selectedEvent.conferenceRoomId && selecteventdate) {
                                checkForFreeTimeSlotcal(selectedEvent.conferenceRoomId, selecteventdate, selectedEvent.eventID);
                            }

                            bookingModal.modal();
                        },
                        error: function (error) {
                            alert('failed');
                        }
                    });
                }
            }

            bookingModal.on('click', '#btnSave', function () {

                //-----------
                var numArray = [];
                $("input:checkbox[name=TimeSlot]:checked").each(function () {
                    var getId = this.id.toString();
                    var getIdval = getId.split("_")[1].trim();
                    numArray.push(parseInt(getIdval));
                });
                var res = checkconsecutivenumbers(numArray);
                //Validation/

                var roomId = bookingModal.find("#txtCRoom").val();
                if (roomId == "0" || roomId == "" || roomId == null) {
                    alert('Please select meeting room');
                    return;
                }
                if (bookingModal.find('#txtSelectedDate').val().trim() == "") {
                    alert('Meeting date required');
                    return;
                }
                if (bookingModal.find('#txtTitle').val().trim() == "") {
                    alert('Meeting title required');
                    return;
                }
                if (bookingModal.find('#txtAttendee').val().trim() == "") {
                    alert('Attendees name required');
                    return;
                }
                if (bookingModal.find('#txtDescription').val().trim() == "") {
                    alert('Agenda required');
                    return;
                }
                if (bookingModal.find('#hdChkCount').val().trim() == "" || $('#hdChkCount').val().trim() == "0") {
                    alert('Time slot required');
                    return;
                }
                if (res == false) {
                    alert('Please select time slot continue...');
                    return;
                }

                var starttime = bookingModal.find('input[name="TimeSlot"]:checked:first').val();
                var endtime = bookingModal.find('input[name="TimeSlot"]:checked:last').val();

                var data = {
                    EventID: bookingModal.find('#hdEventID').val(),
                    AttendeeName: bookingModal.find('#txtAttendee').val().trim(),
                    Start: bookingModal.find('#txtSelectedDate').val().trim() + " " + starttime.split("-")[0].trim(),
                    Date: bookingModal.find('#txtSelectedDate').val().trim(),
                    StartTime: starttime.split("-")[0].trim(),
                    EndTime: endtime.split("-")[1].trim(),
                    End: bookingModal.find('#txtSelectedDate').val().trim() + " " + endtime.split("-")[1].trim(),
                    Description: bookingModal.find('#txtDescription').val(),
                    Title: bookingModal.find('#txtTitle').val(),
                    ConferenceRoomId: roomId,
                }

                SaveEvent(data);
            })

            function SaveEvent(data) {
                $.ajax({
                    type: "POST",
                    url: domain + 'MeetingRoom/SaveEvent',
                    data: data,
                    success: function (data) {
                        if (data.status) {
                            $('#calender').fullCalendar('refetchEvents');
                            bookingModal.modal('hide');
                        }
                        else {
                            alert("This meeting room is already booked for this time slot");
                        }
                    },
                    error: function () {
                        alert('Failed');
                    }
                })
            }

            function checkconsecutivenumbers(data) {
                var arr = data;
                arr.sort(function (a, b) { return a - b });
                for (var i = 1; i < arr.length; i++) {
                    if (arr[i] != arr[i - 1] + 1) {
                        return false;
                    }
                }
                return true;
            }

            function checkForFreeTimeSlotcal(roomid, date, bookingId) {
                console.log("date", date)
                if (parseInt(roomid) > 0 && date && parseInt(bookingId) >= 0) {
                    $.ajax({
                        url: domain + 'MeetingRoom/GetDisabledTimeSlot',
                        data: { roomId: roomid, date: date, bookingId: bookingId },
                        dataType: "json",
                        type: "GET",
                        error: function () {
                            alert(" An error occurred.");
                        },
                        success: function (data) {
                            var arr = data.split('#');

                            $("input[name='TimeSlot']").prop("disabled", false);
                            if (arr && arr.length) {
                                $.each(arr, function (i, val) {
                                    $("#timeSlot_" + val).prop("disabled", true);
                                    $("#timeSlot_" + val).prop('checked', false);
                                    $("#timeSlot_" + val).change();
                                    $(".lbltimeslot_" + val).css('background-color', 'rgba(241, 131, 131, 0.35)');
                                });
                            }

                        }
                    });
                }
            }

            function getSelectAttendees(select) {
                var result = [];
                var options = select && select.options;
                var opt;

                for (var i = 0, iLen = options.length; i < iLen; i++) {
                    opt = options[i];

                    if (opt.selected) {
                        result.push(opt.value || opt.text);
                    }
                }
                return result;
            }
        }

        function checkForFreeTimeSlot(roomid, date, bookingId) {
            if (parseInt(roomid) > 0 && date && parseInt(bookingId) >= 0) {
                $.ajax({
                    url: domain + 'MeetingRoom/GetDisabledTimeSlot',
                    data: { roomId: roomid, date: date, bookingId: bookingId },
                    dataType: "json",
                    type: "GET",
                    error: function () {
                        alert(" An error occurred.");
                    },
                    success: function (data) {
                        var arr = data.split('#');
                        //console.log(arr);
                        $("input[name='TimeSlot']").prop("disabled", false);
                        if (data != "") {
                            jQuery.each(arr, function (i, val) {
                                $("#timeSlot_" + val).attr("disabled", true);
                                $("#timeSlot_" + val).attr('checked', false);
                                $("#timeSlot_" + val).trigger('change');
                                $(".lbltimeslot_" + val).css('background-color', 'rgba(241, 131, 131, 0.35)');
                            });
                        }

                    }
                });
            }
        }

        $this.init = function () {
            initializeForm();
        }
    }

    $(function () {
        var self = new Index();
        self.init();
    });

}(jQuery));