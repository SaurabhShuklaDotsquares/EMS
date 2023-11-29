/*global jQuery, Global,secureDomain */
(function () {
    function ManageLeave() {
        $this = this;
        var mindateval = 0;

        String.prototype.replaceAll = function (search, replacement) {
            var target = this;
            return target.replace(new RegExp(search, 'g'), replacement);
        };

        function ToJavaScriptDate(strdate, format, separator) {
            var arr = strdate.split(separator);
            var formararr = format.split(separator);
            var year, month, date;
            for (var i = 0; i < formararr.length; i++) {
                if (formararr[i].toLowerCase() == 'yyyy') {
                    year = arr[i];
                }
                if (formararr[i].toLowerCase() == 'dd') {
                    date = arr[i];
                }
                if (formararr[i].toLowerCase() == 'mm') {
                    month = arr[i];
                }
            }
            var dt = new Date(year, month - 1, date);
            return dt;
        }

        function setFormat(sdate, format) {
            var FullmonthNames = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
            var monthNames = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
            var FulldayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
            var dayNames = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];
            format = format.replaceAll('DDDD', FulldayNames[sdate.getDay()]);
            format = format.replaceAll('DDD', dayNames[sdate.getDay()]);
            format = format.replaceAll('dd', sdate.getDate());
            format = format.replaceAll('MMMM', FullmonthNames[sdate.getMonth()]);
            format = format.replaceAll('MMM', monthNames[sdate.getMonth()]);
            format = format.replaceAll('MM', sdate.getMonth() + 1);
            format = format.replaceAll('yyyy', sdate.getFullYear());
            format = format.replaceAll('yy', sdate.getFullYear().toString().substring(2));
            return format;
        }

        function CheckLeaveType(selectedDate) {
            var hdnDaysval = parseInt($("#hdnDays").val());
            var mindateval = 0;
            if (hdnDaysval != "" && hdnDaysval > 0) {
                mindateval = hdnDaysval - 1;
            }
            var one_day = 1000 * 60 * 60 * 24;
            var todaydate = new Date();
            todaydate = new Date(todaydate.getFullYear(), todaydate.getMonth(), todaydate.getDate());
            var daysdiff = (ToJavaScriptDate(selectedDate, 'dd/mm/yyyy', '/') - todaydate) / one_day;
            if (daysdiff < hdnDaysval) {
                var normaldate = new Date(todaydate.getFullYear(), todaydate.getMonth(), todaydate.getDate() + hdnDaysval);
                $('#lblleaveType').html('Leave will be marked as urgent because you are applying leave for the day before ' + setFormat(normaldate, 'DDD, MMM dd, yyyy'));
                $('.dvleaveType').removeClass('hide');
            }
            else {
                $('.dvleaveType').addClass('hide');
                $('#lblleaveType').html('');
                //$('#lblleaveType').html('This is a Normal Leave');
            }
        }

        function ManageLeaveDate() {
            var hdnDaysval = $("#hdnDays").val();
            var mindateval = 0;
            if (hdnDaysval != "" && hdnDaysval > 0) {
                mindateval = hdnDaysval - 1;
            }

            if (isAllowedBackDate == 1) {
                $("#StartDate").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    changeMonth: true,
                    changeYear: true,
                    onClose: function (selectedDate) {
                        CheckLeaveType(selectedDate);
                        $("#EndDate").datepicker("option", "minDate", selectedDate);
                        $('#Form1').removeClass('verified');

                    }
                });
                $("#EndDate").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    changeMonth: true,
                    changeYear: true,
                    onClose: function (selectedDate) {
                        $("#StartDate").datepicker("option", "maxDate", selectedDate);
                        $('#Form1').removeClass('verified');
                    }
                });
            }
            else {
                $("#StartDate").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    changeMonth: true,
                    changeYear: true,
                    minDate: 0,
                    onClose: function (selectedDate) {
                        CheckLeaveType(selectedDate);
                        $("#EndDate").datepicker("option", "minDate", selectedDate);
                        //$("#EndDate").datepicker("option", "maxDate", selectedDate.setDate(selectedDate.getDate()+5));
                        $('#Form1').removeClass('verified');

                    }
                });
                $("#EndDate").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    changeMonth: true,
                    changeYear: true,
                    minDate: 0,
                    onClose: function (selectedDate) {
                        $("#StartDate").datepicker("option", "maxDate", selectedDate);
                        $('#Form1').removeClass('verified');
                    }
                });
            }




        }

        function GetProjectManagerUsers() {

            var pmid = $("#PMid").val();
            if (pmid == '')
                pmid = 0;
            var emp = $("#Uid");
            var workemp = $("#WorkAlterID");
            $.post(domain + 'leave/getemployeesbypm', { pmid: pmid }, function (result) {
                emp.empty().append('<option value="">-Select-</option>');
                workemp.empty().append('<option value="">-Select-</option>');
                $.each(result, function (index, data) {
                    emp.append($("<option></option>").val(data.value).html(data.text));
                    workemp.append($("<option></option>").val(data.value).html(data.text));
                });
            }).fail(function (ex) {
                alert("Whooaaa! Something went wrong.." + ex);
            });

            $.post(domain + 'leave/getpriorleavebypm', { pmid: pmid }, function (result) {
                $("#hdnDays").val(result.leavepriordays);
                if ($("#StartDate").val() != '') {
                    CheckLeaveType($("#StartDate").val());
                }
            });
        }




        /*
        if ($('#LeaveId').val() > 0) {
            $('input[name="LeaveType"]:radio').attr("disabled", "disabled");
        }
        if ($('input[name="LeaveType"]:checked').val() == "Urgent") {
            //Urgent
            var hdnDaysval = $("#hdnDays").val();
            var mindateval = 0;
            if (hdnDaysval != "" && hdnDaysval > 0) {
                mindateval = hdnDaysval - 1;
            }

            $("#StartDate").datepicker({
                defaultDate: "+1w",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                minDate: 0,
                maxDate: "+" + mindateval + "D"
                , onClose: function (selectedDate) {
                    $("#EndDate").datepicker("option", "minDate", selectedDate);
                    $('#Form1').removeClass('verified');
                }
            });

            $("#EndDate").datepicker({
                defaultDate: "+1w",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                minDate: 0
                // ,maxDate: "+" + mindateval + "D"
               , onClose: function (selectedDate) {
                   $("#StartDate").datepicker("option", "maxDate", selectedDate);
                   $('#Form1').removeClass('verified');
               }
            });

        }
        else {

            //Normal

            var hdnDaysval = $("#hdnDays").val();
            var mindateval = 0;

            if (hdnDaysval != "" && hdnDaysval > 0) {
                mindateval = hdnDaysval;
            }

            $("#StartDate").datepicker({
                defaultDate: "+1w",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                minDate: "+" + mindateval + "D",
                onClose: function (selectedDate) {
                    $("#EndDate").datepicker("option", "minDate", selectedDate);
                    $('#Form1').removeClass('verified');
                }
            });

            $("#EndDate").datepicker({
                defaultDate: "+1w",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                minDate: "+" + mindateval + "D",
                onClose: function (selectedDate) {
                    $("#StartDate").datepicker("option", "maxDate", selectedDate);
                    $('#Form1').removeClass('verified');
                }
            });


        }
        */
        $('#IsHalf').on('click', function () {
            if ($('#IsHalf').is(":checked")) {
                $('#workingHalf').removeClass("hide");
            }
            else {
                $('#workingHalf').addClass("hide");
            }
        });

        $('#IsCancel').on('click', function () {
            if ($('#IsCancel').is(":checked")) {
                const radioButtons = document.querySelectorAll('input[name="Status"]');
                for (const radioButton of radioButtons) {
                    if (radioButton.value == 8) {
                        radioButton.checked = true;
                    }
                }
            }
        });

        $('input[name="Status"]:radio').on("change", function () {
            var selectedValue = $("input[type='radio'][name='Status']:checked").val();
            if (selectedValue == 8) {
                $("#IsCancel").prop("checked", true);
            }
            else {
                $("#IsCancel").prop("checked", false);
            }
        });

        $('input[name="LeaveType"]:radio').on("change", function () {

            $('#StartDate').datepicker('destroy');
            $('#EndDate').datepicker('destroy');

            $("#StartDate").val("");
            $("#EndDate").val("");

            var radioValue = $("input[name='LeaveType']:checked").next().text();

            if (radioValue == 'Urgent') {


                //urgent

                var hdnDaysval = $("#hdnDays").val();
                var mindateval = 0;
                if (hdnDaysval != "" && hdnDaysval > 0) {
                    mindateval = hdnDaysval - 1;
                }

                $("#StartDate").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    changeMonth: true,
                    changeYear: true,
                    minDate: 0,
                    maxDate: "+" + mindateval + "D"
                    , onClose: function (selectedDate) {
                        $("#EndDate").datepicker("option", "minDate", selectedDate);
                        $('#Form1').removeClass('verified');
                    }
                });

                $("#EndDate").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    changeMonth: true,
                    changeYear: true,
                    minDate: 0
                    //,maxDate: "+" + mindateval + "D"
                    , onClose: function (selectedDate) {
                        $("#StartDate").datepicker("option", "maxDate", selectedDate);
                        $('#Form1').removeClass('verified');
                    }
                });


            }

            else {
                //Normal 
                var hdnDaysval = $("#hdnDays").val();
                var mindateval = 0;

                if (hdnDaysval != "" && hdnDaysval > 0) {
                    mindateval = hdnDaysval;
                }

                $("#StartDate").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    changeMonth: true,
                    changeYear: true,
                    minDate: "+" + mindateval + "D"
                    , onClose: function (selectedDate) {

                        $("#EndDate").datepicker("option", "minDate", selectedDate);
                        $('#Form1').removeClass('verified');
                    }
                });

                $("#EndDate").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    changeMonth: true,
                    changeYear: true,
                    minDate: "+" + mindateval + "D"
                    , onClose: function (selectedDate) {
                        $("#StartDate").datepicker("option", "maxDate", selectedDate);
                        $('#Form1').removeClass('verified');
                    }
                });

            }

        });

        //get work alternator's name
        $('#WorkAlterID').on("change", function () {

            $('#WorkAlternatorName').val($('#WorkAlterID option:selected').text());
        });


        //remove option from work alternator 
        //which is selected in Employee
        //when user is of type HR/PM/PMO
        $('#Uid').on("change", function () {
            $("#WorkAlterID option").each(function () {
                if (($(this).attr('value') == $("#Uid  option:selected").attr('value'))) {
                    $(this).hide();
                    $(this).siblings().show();
                }
            });

        });

        $('#LeaveCategory').on("change", function () {
            hiddenValues();
        });




        function initializeForm() {
            if (isEdit) {
                $('#LeaveCategory').prop('disabled', true);
            }

            // FormManageLogin = new Global.FormValidationReset('#Form1');
            $('#PMid').on('change', function () {
                GetProjectManagerUsers();
            });

            ValidateUID();

            // $('#IsSelfLeave').on('click', ValidateUID);
            $('#IsSelfLeave').on('click', function () {
                if ($('#IsSelfLeave').is(":checked")) {
                    //var IsHRUserRole = $("#IsHRUserRole").val();
                    //if (IsHRUserRole) {
                    GetProjectUsers();
                    GetUserLeave();
                    $('.divLeaveBalance').removeClass('hide');
                    //}
                }
                else {
                    var pmid = $("#PMid").val();
                    $("#Uid").val('');
                    if (pmid)
                        GetProjectManagerUsers();
                }
                daysCount();
                ValidateUID();
            });

            if ($('#IsHalf').is(":checked")) {
                var firstHalf = $('#FirstHalf').val();
                var secondHalf = $('#SecondHalf').val();
                var value = firstHalf == "True" ? "1" : (secondHalf == "True") ? "2" : "";
                /* $("#HalfType option").each(function () {*/
                if (($('#HalfType').attr('value') == value)) {
                    $(this).hide();
                    $(this).siblings().show();
                }
                /* });*/
                //$('#HalfType').val(value);
                $('#workingHalf').removeClass("hide");
            };
            $('#IsHalf').on("click", function () {
                hiddenValues();
            });
            $('#StartDate,#EndDate').on("change", function () {
                hiddenValues();
                daysCount();
            });

            $("#Uid").change(function () {
                var selected = $("#Uid option:selected").val();
                if (selected != '') {

                    $('.divLeaveBalance').removeClass('hide');
                }
                var currentUserId = $(".dvuser").attr("data-userId");
                GetUserLeave();
                hiddenValues();
                $('.divLeaveBalance').removeClass('hide');
                if (selected == currentUserId) {
                    //$('.dvuser').addClass('hide');
                    //$('#Uid').addClass('validated');
                    //$('.dvleavestatus').addClass('hide');
                    //$('.dvleavestatus input,.dvleavestatus textarea').addClass('validated');
                    $("#Uid").val("");
                    $("#IsSelfLeave").prop("checked", "true");
                    ValidateUID();
                }
                //else {
                //    $('#Uid').removeClass('validated');
                //    $('.dvuser').removeClass('hide');
                //    $('.dvleavestatus').removeClass('hide');
                //    $('.dvleavestatus input,.dvleavestatus textarea').removeClass('validated');
                //}
            });

            new Global.FormValidationReset('#Form1', { ignore: '.validated' });

            $('#Form1').on('submit', function (e) {
                if (isRoleUKPM == 1 && ($('#LeaveId').val() == '' || $('#LeaveId').val() == '0')) {
                    if (!$('#Form1').hasClass('verified')) {
                        e.preventDefault();
                        var uid = currentuserid;
                        if ($('#Uid').length > 0) {
                            if ($('#IsSelfLeave').length == 0 || !$('#IsSelfLeave').prop('checked')) {
                                uid = $('#Uid').val();
                            }
                        }
                        $('#Form1').validate({ ignore: '.validated' });
                        if ($('#Form1').valid()) {
                            $.post(domain + "leave/checkforotherleave", { uid: uid, StartDate: $('#StartDate').val(), EndDate: $('#EndDate').val() }, function (data) {
                                if (data.needToShowMessage && !confirm(data.message)) {

                                }
                                else {

                                    $('#Form1').addClass('verified');
                                    $('#Form1').submit();
                                    //$("#btnSave").prop('disabled', true);
                                }
                            });
                        }
                    }
                }
                if ($('#Form1').valid()) {
                    $("#btnSave").prop('disabled', true);
                }

            });
        };

        //$("#btnLeaveBalance").on("click", function () {
        //    var selected = $("#Uid option:selected").val();
        //    var currentUserId = $(".dvuser").attr("data-userId");
        //    var id = 0;
        //    if (selected != '') {
        //        id = selected;
        //    }
        //    else if (currentUserId != '') {
        //        id = currentUserId;
        //    }
        //    if (id > 0) {
        //        loadUserLeaveBalanace();
        //    }
        //});

        function loadUserLeaveBalanace() {

            $('#modal-view-leave-balance').on('shown.bs.modal', function () {
                //var e = document.getElementById("Uid");
                //var strUser = e.value;
                var selected = $("#Uid option:selected").val();
                var currentUserId = $(".dvuser").attr("data-userId");
                var id = 0;
                if (selected != '') {
                    id = selected;
                }
                if ($('#IsSelfLeave').is(":checked")) {
                    if (currentUserId != '') {
                        id = currentUserId;
                    }
                }
                var recipient = domain + "leavebalance/ViewUserLeaveBalance/" + id; // Extract info from data-* attributes
                $('#modal-view-leave-balance .modal-content').load(recipient, function () {

                });
            }).on('hide.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });



            //$.ajax({
            //    type: "GET",
            //    url: domain + "leavebalance/ViewUserLeaveBalance",
            //    data: { Id: id },
            //    success: function (result) {
            //        $('#userid').val(id);
            //        alert($('#userid').val());
            //    }

            //});
            /*return '<a style="text-decoration: underline" data-toggle="modal" data-target="#modal-view-leave-balance" href="' + domain + 'leavebalance/ViewUserLeaveBalance/' + {uid:id} + '</a>';*/
            //$("#modal-view-leave-balance").on('loaded.bs.modal', function () {

            //    /*var userid = currentUserId;*/
            //    //if ($("#IsSelfLeave").prop("checked", "true")) {
            //    //    userid = $('#selfleave').val();
            //    //}
            //    var modal = $(this);
            //    $(".btnLeaveBalance").on('click', domain + "leavebalance/ViewUserLeaveBalance", { uid: id }, function () {

            //    });

            //}).on('hidden.bs.modal', function () {
            //    $(this).removeData('bs.modal');
            //    //$(this).find('.modal-content').empty();
            //});
        };


        function GetProjectUsers() {
            var workemp = $("#WorkAlterID");
            $.post(domain + 'leave/GetEmployeesByHR', function (result) {
                workemp.empty().append('<option value="">-Select-</option>');
                $.each(result, function (index, data) {
                    workemp.append($("<option></option>").val(data.value).html(data.text));
                });
            }).fail(function (ex) {
                alert("Whooaaa! Something went wrong1.." + ex);
            });
        }

        function GetUserLeave() {
            var uId = 0;
            if ($('#IsSelfLeave').is(":checked")) {
                uId = $(".dvuser").attr("data-userId");
            }
            if ($('#IsSelfLeave').is(":unchecked")) {
                uId = $('#Uid').val();
            }

            $.post(domain + 'leave/GetUserLeaves', { uid: uId, edit: isEdit,id:leaveId }, function (result) {
                var data = result;
                $("#hdnLeavesCL").val(data.casualLeave);
                $("#hdnLeavesEL").val(data.earnedLeave);
                $("#hdnLeavesAL").val(data.lossPayLeave);
                $("#hdnLeavesSL").val(data.sickLeave);
                $("#hdnLeavesBL").val(data.bereavementLeave);
                $("#hdnLeavesWL").val(data.weddingLeave);
                $("#hdnLeavesLL").val(data.loyaltyLeave);
                $("#hdnLeavesPL").val(data.paternityLeave);
                $("#hdnLeavesML").val(data.maternityLeave);
                $("#hdnLeavesCO").val(data.compensatoryOff);
                hiddenValues();
            });
        }
        function daysCount() {
            var startDate = $("#StartDate").val();
            var endDate = $("#EndDate").val();
            var one_day = 1000 * 60 * 60 * 24;
            if (startDate != '' && endDate != '') {
                var days = (ToJavaScriptDate(endDate, 'dd/mm/yyyy', '/') - ToJavaScriptDate(startDate, 'dd/mm/yyyy', '/')) / one_day;
                days = days + 1;
                if (days > 1) {
                    $('#IsHalf').prop('checked', false);
                    $('#IsHalf').prop('disabled', true);
                    $('#workingHalf').addClass("hide");
                }
                if (days == 1) {
                    $('#IsHalf').prop('checked', false);
                    $('#IsHalf').removeProp('disabled');
                }
                if ($('#IsSelfLeave').is(":checked") && isRoleUKAUPM == 0) {
                    if (days > 5) {
                        $('#lbldayCount').html('You can apply for a maximum of 5 days leave.');
                        $('.dvDayCount').removeClass('hide');
                    }

                    else {
                        $('.dvDayCount').addClass('hide');
                        $('#lbldayCount').html('');
                    }
                }
                else if ($('#uid').val() != '' && isRoleUKAUPM == 0) {
                    if (days > 5 && isPmHrTl == 0) {
                        $('#lbldayCount').html('You can apply for a maximum of 5 days leave.');
                        $('.dvDayCount').removeClass('hide');
                    }

                    else {
                        $('.dvDayCount').addClass('hide');
                        $('#lbldayCount').html('');
                    }
                }

            }
        }
        function hiddenValues() {
            var leaveType = $('#LeaveCategory').val();
            var startDate = $("#StartDate").val();
            var endDate = $("#EndDate").val();
            var one_day = 1000 * 60 * 60 * 24;
            var isHalf = $('#IsHalf').is(":checked");
            $('.divleaveCategory').addClass('hide');
            $('#lblLeaveCategory').removeAttr("style");
            var message = 'Insufficient leave balance in selected leave category. try again with a different leave category.';
            if (startDate != "" && endDate != "") {
                var days = (ToJavaScriptDate(endDate, 'dd/mm/yyyy', '/') - ToJavaScriptDate(startDate, 'dd/mm/yyyy', '/')) / one_day;
                days = days + 1;
                if (leaveType != "" && leaveType > 0) {
                    var leaveCL = parseFloat($("#hdnLeavesCL").val());
                    if (leaveType == "5") {
                        if (isHalf) {
                            if ((leaveCL - (days / 2)) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                        else {
                            if ((leaveCL - days) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                    }

                    var leaveEL = parseFloat($("#hdnLeavesEL").val());
                    if (leaveType == "9") {
                        if (isHalf) {
                            if ((leaveEL - (days / 2)) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                        else {
                            if ((leaveEL - days) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                    }

                    //var leaveAL = parseFloat($("#hdnLeavesAL").val());
                    //if (leaveType == "1") {
                    //    if (isHalf) {
                    //        if ((leaveAL - (days / 2)) < 0) {
                    //            $('.divleaveCategory').removeClass('hide');
                    //            $('#lblLeaveCategory').html(message);
                    //            $('#LeaveCategory').val('');
                    //        }
                    //    }
                    //    else {
                    //        if ((leaveAL - days) < 0) {
                    //            $('.divleaveCategory').removeClass('hide');
                    //            $('#lblLeaveCategory').html(message);
                    //            $('#LeaveCategory').val('');
                    //        }
                    //    }
                    //}

                    var leaveSL = parseFloat($("#hdnLeavesSL").val());
                    if (leaveType == "10") {
                        if (isHalf) {
                            if ((leaveSL - (days / 2)) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                        else {
                            if ((leaveSL - days) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                    }

                    var leaveBL = parseFloat($("#hdnLeavesBL").val());
                    if (leaveType == "12") {
                        if (isHalf) {
                            if ((leaveBL - (days / 2)) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                        else {
                            if ((leaveBL - days) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                    }

                    var leaveWL = parseFloat($("#hdnLeavesWL").val());
                    if (leaveType == "13") {
                        if (isHalf) {
                            if ((leaveWL - (days / 2)) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                        else {
                            if ((leaveWL - days) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                    }

                    //var leaveLL = parseFloat($("#hdnLeavesLL").val());
                    //if (leaveType == "14") {
                    //    if (isHalf) {
                    //        if ((leaveLL - (days / 2)) < 0) {
                    //            $('.divleaveCategory').removeClass('hide');
                    //            $('#lblLeaveCategory').html('Insufficient leave balance with selected leaves category but you may apply.');
                    //        }
                    //    }
                    //    else {
                    //        if ((leaveLL - days) < 0) {
                    //            $('.divleaveCategory').removeClass('hide');
                    //            $('#lblLeaveCategory').html('Insufficient leave balance with selected leaves category but you may apply.');
                    //        }
                    //    }
                    //}

                    var leavePL = parseFloat($("#hdnLeavesPL").val());
                    if (leaveType == "8") {
                        if (isHalf) {
                            if ((leavePL - (days / 2)) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                        else {
                            if ((leavePL - days) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                    }

                    var leaveML = parseFloat($("#hdnLeavesML").val());
                    if (leaveType == "11") {
                        if (isHalf) {
                            if ((leaveML - (days / 2)) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                        else {
                            if ((leaveML - days) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                    }

                    var leaveCO = parseFloat($("#hdnLeavesCO").val());
                    if (leaveType == "2") {
                        if (isHalf) {
                            if ((leaveCO - (days / 2)) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                        else {
                            if ((leaveCO - days) < 0) {
                                $('.divleaveCategory').removeClass('hide');
                                $('#lblLeaveCategory').html(message);
                                $('#LeaveCategory').val('');
                            }
                        }
                    }
                }
            }
        }

        function ValidateUID() {
            if ($('#IsSelfLeave').length > 0) {
                if ($('#IsSelfLeave').prop('checked')) {
                    $('.dvuser').addClass('hide');
                    $('#Uid').addClass('validated');
                    $('.dvleavestatus').addClass('hide');
                    $('.dvleavestatus input,.dvleavestatus textarea').addClass('validated');
                }
                else {
                    $('#Uid').removeClass('validated');
                    $('.dvuser').removeClass('hide');
                    $('.dvleavestatus').removeClass('hide');
                    $('.dvleavestatus input,.dvleavestatus textarea').removeClass('validated');
                }
            }
        }

        $this.init = function () {
            loadUserLeaveBalanace()
            initializeForm();
            ManageLeaveDate();
            GetUserLeave();
            hiddenValues();
            //daysCount();
        }
    }


    $(function () {
        var self = new ManageLeave;
        self.init();
    });


}(jQuery));