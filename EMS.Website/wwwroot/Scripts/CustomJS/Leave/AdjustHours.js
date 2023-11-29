(function () {
    function AdjustHours() {
        var $this = this, formAddEdit1;
        var mindateval = 0;
        function LoadData(dateText) {
            dateText ? dateText : moment(new Date()).format("DD/MM/YYYY");
            $.ajax({
                type: "GET",
                url: domain + "leave/getAdjustHours",
                data: {
                    modifiedDate: dateText
                },
                success: function (r) {
                    $("#adjustform table tbody").html(r);
                    maskinput();
                },
                error: function (jqXHR, status, error) {
                    console.log(error)
                },
                complete: function () {
                }
            });
        }

        function initializeForm() {           
            formAddEdit1 = new Global.FormHelper($("#adjustform"), {
                updateTargetId: "validation-summary", validateSettings: { ignore: '' }
            },
            function onBeforeSubmit() {
                $('.divoverlay').removeClass('hide');
            },
            function onSucccess(result) {
                $('.divoverlay').addClass('hide');
                var dateresult = result.data != null ? result.data : moment(new Date()).format("DD/MM/YYYY")
                LoadData(dateresult);

                $('html, body').animate({ scrollTop: 0 }, 0);
                var $tackCommentMessageDiv = $('#tackCommentMessageDiv');
                if (Global.IsNotNullOrEmptyString(result.message)) {
                    $tackCommentMessageDiv.addClass('alert-success').removeClass('alert-danger');
                    $tackCommentMessageDiv.empty().html(result.message);
                }
                else if (Global.IsNotNullOrEmptyString(result.errorMessage)) {
                    $tackCommentMessageDiv.addClass('alert-danger').removeClass('alert-success');
                    $tackCommentMessageDiv.empty().html(result.errorMessage);
                }
                $tackCommentMessageDiv.show();
                window.setTimeout(function () {
                    $tackCommentMessageDiv.html('');
                    $tackCommentMessageDiv.hide();
                }, 3000);
                if (result.isSuccess) {
                    return false;
                }
            });


            $("#Modified").datepicker({
                defaultDate: "+1w",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                onSelect: function (dateText) {
                    LoadData(dateText);
                }
            });

            $("#Modified").datepicker('setDate', 'today');

            $(document).on('change', ".chkLatestart", function () {
                var chk = this.checked;
                if (chk) {
                    $(this).closest('tr').find('.txtLateStarttime').removeClass('hidden');
                    $(this).closest('tr').find('.txtLateReason').removeClass('hidden');
                    $(this).closest('tr').find('.txtLateReason').addClass('required');
                }
                else {
                    $(this).closest('tr').find('.txtLateStarttime').addClass('hidden');
                    $(this).closest('tr').find('.txtLateReason').addClass('hidden');
                    $(this).closest('tr').find('.txtLateReason').removeClass('required');
                    $(this).closest('tr').find('.txtLateReason').val('');
                    $(this).closest('tr').find('.txtLateStarttime').val('');
                }
            });

            $(document).on('change', ".chkEarlyleave", function () {
                var chk = this.checked;
                if (chk) {
                    $(this).closest('tr').find('.txtEarlyLeavetime').removeClass('hidden');
                    $(this).closest('tr').find('.txtEarlyReason').removeClass('hidden');
                    $(this).closest('tr').find('.txtEarlyReason').addClass('required');
                }
                else {
                    $(this).closest('tr').find('.txtEarlyLeavetime').addClass('hidden');
                    $(this).closest('tr').find('.txtEarlyReason').removeClass('required');
                    $(this).closest('tr').find('.txtEarlyReason').addClass('hidden');
                    $(this).closest('tr').find('.txtEarlyReason').val('');
                    $(this).closest('tr').find('.txtEarlyLeavetime').val('');
                }
            });

            $(document).on('change', ".chkWorkAtHome", function () {
                var chk = this.checked;
                $(this).closest('tr').find('input:checkbox').removeAttr('disabled', false);
                $(this).closest('tr').find('.txtReasonWorkAtHome').addClass('hidden');
                $(this).closest('tr').find('.txtReasonWorkAtHome').val('');
                if (chk) {
                    $(this).closest('tr').find('input:checkbox').prop('checked', false).attr('disabled', 'disabled');
                    $(this).closest('tr').find('input:text').addClass('hidden');
                    $(this).closest('tr').find('.txtEarlyReason').addClass('hidden');
                    $(this).closest('tr').find('.txtLateReason').addClass('hidden');
                    $(this).closest('tr').find('.txtReasonWorkFromHome').addClass('hidden');
                    $(this).closest('tr').find('.txtReasonWorkFromHome').val('');
                    $(this).closest('tr').find('input:text').val('');
                    $(this).closest('tr').find('.txtEarlyReason').val('');
                    $(this).closest('tr').find('.txtLateReason').val('');
                    $(this).closest('tr').find('.txtReasonWorkAtHome').removeClass('hidden');
                    $(this).prop('checked', chk).removeAttr('disabled', false);
                }
            });

            $(document).on('change', ".chkWorkFromHome", function () {
                var chk = this.checked;
                $(this).closest('tr').find('input:checkbox').removeAttr('disabled', false);
                $(this).closest('tr').find('.txtReasonWorkFromHome').addClass('hidden');
                $(this).closest('tr').find('.txtReasonWorkFromHome').val('');
                if (chk) {
                    $(this).closest('tr').find('input:checkbox').prop('checked', false).attr('disabled', 'disabled');
                    $(this).closest('tr').find('input:text').addClass('hidden');
                    $(this).closest('tr').find('.txtEarlyReason').addClass('hidden');
                    $(this).closest('tr').find('.txtLateReason').addClass('hidden');
                    $(this).closest('tr').find('.txtReasonWorkAtHome').addClass('hidden');
                    $(this).closest('tr').find('input:text').val('');
                    $(this).closest('tr').find('.txtEarlyReason').val('');
                    $(this).closest('tr').find('.txtLateReason').val('');
                    $(this).closest('tr').find('.txtReasonWorkAtHome').val('');
                    $(this).closest('tr').find('.txtReasonWorkFromHome').removeClass('hidden');
                    $(this).prop('checked', chk).removeAttr('disabled', false);
                }
            });
        }

        function maskinput() {
            $('.clstimepicker').mask("99:99", { clearIncomplete: false });
            $('.clstimepicker').blur(function () {
                var currentMask = '';
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
                }
                $(this).val(newVal);
            });

        }

        $this.init = function () {
            initializeForm();
            LoadData(moment(new Date()).format("DD/MM/YYYY"));

        }
    }
    $(function () {
        var self = new AdjustHours;
        self.init();
    });

}(jQuery));