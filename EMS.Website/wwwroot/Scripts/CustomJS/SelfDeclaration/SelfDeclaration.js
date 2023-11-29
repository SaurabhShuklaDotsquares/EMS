
(function ($) {
    function index() {
        var $this = this, form;
        function InitializeEvents() {
            form = new Global.FormValidationReset('#form1');
            form.find("#SymptomsStartDate").datepicker({
                dateFormat: "dd/mm/yy",
                maxDate: new Date()
            });
            form.find("#SymptomsEndDate").datepicker({
                dateFormat: "dd/mm/yy",
                maxDate: new Date()
            });
            //form.find("#JoiningDate").datepicker({
            //    dateFormat: "dd/mm/yy",
            //    maxDate: new Date(),
            //    changeMonth: true,
            //    changeYear: true,
            //    yearRange: "-100:+0"
            //});
            form.find("#Dob").datepicker({
                dateFormat: "dd/mm/yy",
                maxDate: new Date(),
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0"
            });
            //form.find("input:radio[name=RecentlyInJaipur]").off('change').on('change', function () {
            $("input:radio[name=RecentlyInJaipur]").off('change').on('change', function () {
                let val = $(this).val();
                if (val=="true") {
                    $('.RecentlyInJaipurYes').show();
                    $('.RecentlyInJaipurNo').hide();
                    $('.purpose').val('');
                }
                else {
                    $('.RecentlyInJaipurNo').show();
                    $('.RecentlyInJaipurYes').hide();

                    //$('.RecentlyInJaipurNo').removeClass('hidden');
                    //$('.RecentlyInJaipurYes').addClass('hidden');
                }
            });
            $("input:radio[name=HasDiseaseSymptoms]").off('change').on('change', function () {
                let val = $(this).val();
                if (val == "true") {
                    $('.rowSymptoms').show();
                    $('#SymptomsStartDate').attr('required','true')
                    $('#SymptomsEndDate').attr('required','true')
                }
                else {
                    $('.rowSymptoms').hide();
                    $('#SymptomsStartDate').attr('required', 'false')
                    $('#SymptomsEndDate').attr('required', 'false')
                    $('#SymptomsStartDate').val('');
                    $('#SymptomsEndDate').val('');
                    //$('.RecentlyInJaipurNo').removeClass('hidden');
                    //$('.RecentlyInJaipurYes').addClass('hidden');
                }
            });

            form.on('submit').on('submit', function (e) {
                e.preventDefault();
                if (form.valid()) {

                    var formdata = form.serializeArray();

                    var submitBtn = $(form).find(':submit');
                    var submitHtml = submitBtn.filter(':focus').addClass('submitting').html();

                    submitBtn.filter('.submitting').html('<i class="fa fa-refresh fa-spin"></i> Submitting...');
                    submitBtn.prop('disabled', true);

                    $.post(form[0].action, formdata, function (data) {
                        submitBtn.filter('.submitting').html(submitHtml).removeClass('submitting');
                        submitBtn.prop('disabled', false);
                        if (data.isSuccess) {
                            Global.ShowMessage(data.message, true, 'NotificationDiv');
                        } else {
                            Global.ShowMessage(data.message, data.success, 'NotificationDiv');
                        }
                    });
                }
            });
        }

        $this.init = function () {
            InitializeEvents();
            $("input:radio[name=RecentlyInJaipur]:checked").trigger('change');
            $("input:radio[name=HasDiseaseSymptoms]:checked").trigger('change');
        };
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));