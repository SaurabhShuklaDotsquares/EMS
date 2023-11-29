(function ($) {
    function index() {
        var $this = this, grid, formAddEdit;
      
        function IntializeForm() {
            formAddEdit = new Global.FormHelper($("form"), {
                updateTargetId: "validation-summary",
                validateSettings: { ignore: [] },
                beforeSubmit: function () {                

                        var isValidate = checkValidation($("form"));
                        return isValidate;
                                    
                }

            });
           
            CKEDITOR.replace('Suggestion', { toolbar: "Basic" });
            attachEventCKEditor('Suggestion');

            CKEDITOR.replace('Reason', { toolbar: "Basic" });
            attachEventCKEditor('Reason');

            $(".select2").select2({
                placeholder: "--Select Project--",
                allowClear: true,
                width: '100%'
            });

            $("#dateOfClosing").datepicker({
                defaultDate: "+1w",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                minDate: 0
            });

            $("#nextStartDate").datepicker({
                defaultDate: "+1w",
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                minDate: 0,
                maxDate: "+29"
            });

            formAddEdit.find("#DeadResponseDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                minDate: 0,
                maxDate: 100
            });

            formAddEdit.find("#IsPermanentDead").change(function (e) {
                $("#DeadResponseDate").toggle();
            });

            formAddEdit.find("#ChangeStatusId").change(function (e) {
                if ($("#ChangeStatusId").val() == "3") {
                    $("#DeadResponseDateRow").show();
                } else {
                    $("#DeadResponseDateRow").hide();
                }
            });


            $("input[name='LiveUrl']").on('change', function () {
                if (this.value == "true") {
                    $("#ProjectUrlAbsenseReason").addClass('hidden').prop('required', false).prop('disabled', true).val("").blur();
                    //$("#ProjectLiveUrl").removeClass('hidden').prop('required', true).prop('disabled', false);
                    $("#ProjectLiveUrl").removeClass('hidden').prop('disabled', false);
                }
                else {
                    $("#ProjectUrlAbsenseReason").removeClass('hidden').prop('disabled', false);
                    //  $("#ProjectUrlAbsenseReason").removeClass('hidden').prop('required', true).prop('disabled', false);
                    $("#ProjectLiveUrl").addClass('hidden').prop('required', false).prop('disabled', true).val("").blur();
                }
            });

            $("input[name='LiveUrl']:checked").change();

            if ($("#ChangeStatusId").val() == "3" || $("#ChangeStatusId").val() == "4") {
                $("#nextStartDate").removeClass('required');
            }
            else {
                $("#nextStartDate").addClass('required');
            }

            $("#ChangeStatusId").on('change', function () {
                if ((this.value == "3") || (this.value == "4")) {
                    $("#nextStartDate").removeClass('required');
                }
                else {
                    $("#nextStartDate").addClass('required');
                }
            });

            $(document).on("change", '.Project-closure-history', function () {
                getProjectClosureHistory($(this).val());
            });

            var projectId = $("#createEditProjectClosure #Id").val() || 0;
            if (projectId > 0) {
                getProjectClosureHistory($("#ProjectID").val());
            }


            $('#Country').on('change', function () {
                loadAbortedPM($('#Country option:selected').text());
            });

        }

        function checkValidation($formId) {
            if ($formId.validate().valid()) {
                var checkedArray = [];
                var isValid = false;
                var abordPMDiv = $('#abortedPMListDIV').html().length;
                if (abordPMDiv > 0) {
                    var selectedCountryText = $('#Country option:selected').text();
                    $("#abordPMCheckedDiv").find("input[type=checkbox]:checked").each(function () {
                        checkedArray.push(this.name);
                    });
                   
                    if (selectedCountryText === 'UK') {
                        if (checkedArray.length < 2) {
                            alert('minimum two contact should be selected');
                            isValid = false;
                        }
                        else {
                            isValid = true;
                        }
                    }
                    else if (selectedCountryText !== 'UK') {
                        if (checkedArray.length < 1) {
                            alert('minimum one contact should be selected');
                            isValid = false;
                        }
                        else {
                            isValid = true;
                        }
                    }

                    return isValid;

                }
            }
        }
        function loadAbortedPM(countryText) {           
            var projectClosureId = $("#createEditProjectClosure #Id").val() || 0;
                $.ajax({
                    url: 'projectclosure/getabortedpm?countryName=' + countryText + '&projectClosureId=' + projectClosureId,
                    type: 'GET',
                    success: function (htmlElement) {
                        $('#abortedPMListDIV').empty().html(htmlElement);
                    }
                });           
        }
        function attachEventCKEditor(instance) {
            CKEDITOR.on('instanceReady', function (e) {
                e.editor.document.on('keyup', function () {
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        }

        function getProjectClosureHistory(pid) {
            if (pid > 0) {
                $("#dvProjectClosureHistory").removeClass('hide');
            }
            else {
                $("#dvProjectClosureHistory").addClass('hide');
            }
            $.ajax({
                type: "GET",
                url: domain + 'projectclosure/history/',
                data: { id: pid || 0, view: '_ProjectHistory' },
                success: function (result) {
                    $("#dvProjectClosureHistory table tbody").html($(result).find('tbody').html());
                }
            });
        }


        $this.init = function () {
            IntializeForm();
            var countryText = $('#Country option:selected').text();
            loadAbortedPM(countryText);
        };
    }

    $(function () {
        var self = new index();
        self.init();



    });
}(jQuery));