(function ($) {
    function ManageMedical() {
        var $this = this;

        $("#chk_relative").on('change', function () {
            $this = $(this).prop("checked");
            if ($this) {
                $(".show_relative").show();
            }
            else {
                $(".show_relative").hide();
            }
        });
        $('.datepicker').datepicker({
            dateFormat: 'dd/mm/yy',
            changeMonth: true,
            changeYear: true,
            yearRange: "-100:+0"
        });

        new Global.FormValidationReset('#Form1', {});

        $('#btnaddrelatives').on('click', function () {
            if ($('.relativedata:last').hasClass('hide') == false) {

                var i = parseInt($('div.relativedata:last').attr('data-number'));
                $('.relativeparent').append($('div.relativedata:last').clone());
                $('div.relativedata:last input').val('');

                $('div.relativedata:last .lblnumber').html('Relative ' + (i + 1));
                $('div.relativedata:last').attr('data-number', i + 1);
                $('div.relativedata:last input,div.relativedata:last select').each(function () {
                    $(this).attr('name', $(this).attr('name').replace(i - 1, i));
                    $(this).attr('id', $(this).attr('id').replace(i - 1, i));
                });
                $('div.relativedata:last .datepicker').removeClass('hasDatepicker').datepicker({
                    dateFormat: 'dd/mm/yy',
                    changeMonth: true,
                    changeYear: true,
                    yearRange: "-100:+0"
                });
            }
            else {
                $('.relativedata').removeClass('hide');
                var i = parseInt($('div.relativedata:last').attr('data-number'));

                $('div.relativedata:last .lblnumber').html('Relative ' + (i + 1));
                $('div.relativedata:last').attr('data-number', i + 1);
            }
        });
    }
    $(function () {
        var self = new ManageMedical();
        $("#chk_relative").trigger('change');
        //self.init();
    });
}(jQuery))