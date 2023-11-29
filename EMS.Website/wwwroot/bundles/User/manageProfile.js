(function ($) {
    function Manageprofile() {
        var $this = this, form;

        function initializeForm() {
            form = new Global.FormValidationReset('#form1');

            form.find("#DOB").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0"
            });

            form.find("#MarraigeDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0"
            });

            form.find('.checkList input[name="Technology"]').each(
                function () {
                    var specializations = $(this).closest('.checkList').find('input.checkspl');

                    if (this.checked) {
                        if (specializations.filter(':checked').length === 0) {
                            specializations.next('label').css('color', 'red');
                        }
                        else {
                            specializations.next('label').css('color', 'green');
                            $(this).next('label').css('color', 'green');
                        }
                    }
                    else {
                        specializations.prop('disabled', true).prop('checked', false);
                    }
                }).on('change', function () {
                    var specializations = $(this).closest('.checkList').find('input.checkspl');

                    if (this.checked) {
                        specializations.prop('disabled', false);
                        if (specializations.filter(':checked').length === 0) {
                            specializations.next('label').css('color', 'red');
                        }
                        else {
                            specializations.next('label').css('color', 'green');
                            $(this).next('label').css('color', 'green');
                        }
                    }
                    else {
                        specializations.prop('disabled', true).prop('checked', false);
                        specializations.next('label').css('color', '');
                        $(this).next('label').css('color', '');
                    }
                });

            form.on('change', '.chkSecialization input[type="radio"]', function () {
                $(this).closest('.chkSecialization').find('label').css('color', 'green');
                $(this).closest('.checkList').find('input[name="Technology"]+label').css('color', 'green');
            });

            form.on('submit').on('submit', function (e) {
                e.preventDefault();
                if (form.valid()) {
                    
                    var userTechs = [];
                    var selectedTechs = $('.checkList input[name="Technology"]:checked');
                    if (selectedTechs.length) {
                        var errorMsg = false;
                        selectedTechs.each(function () {
                            var specType = $(this).closest('.checkList').find('input.checkspl:checked').val();

                            if (!specType) {
                                errorMsg = ('Please choose specialization level for technology : "' + $(this).next('label').text().trim() + '"');
                                return false;
                            }
                            else {
                                userTechs.push({
                                    TechId: this.value,
                                    SpecTypeId: specType
                                });
                            }
                        });

                        if (errorMsg) {
                            CustomAlerts.error("Error !!!", errorMsg);
                            return false;
                        }
                    }

                    var formdata = form.serializeArray();
                    if (userTechs.length) {

                        $.each(userTechs, function (i, item) {
                            formdata.push({ name: 'TechnologyList[' + i + '].TechId', value: item.TechId });
                            formdata.push({ name: 'TechnologyList[' + i + '].SpecTypeId', value: item.SpecTypeId });
                        });
                    }

                    $.post(form[0].action, formdata, function (data) {
                        if (data.Success) {
                            window.location.reload();
                        } else {
                            CustomAlerts.error("Error !!!", data.Message);
                        }
                    });
                }
            });
        }

        $this.init = function () {
            initializeForm();
        };
    }

    $(function () {
        var self = new Manageprofile();
        self.init();
    });
}(jQuery));
