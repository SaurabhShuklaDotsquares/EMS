(function ($) {
    function PerformanceReport() {
        $this = this;

        function initialize() {
            $("#StartDate").datepicker({
                dateFormat: "dd/mm/yy",
                maxDate: $("#EndDate").val(),
                changeMonth: true,
                changeYear: true,
                onSelect: function (selectedDate) {
                    $("#EndDate").datepicker("option", "minDate", selectedDate);
                                    }
            });
            $("#EndDate").datepicker({
                dateFormat: "dd/mm/yy",
                maxDate: new Date(),
                minDate: $("#StartDate").val(),
                changeMonth: true,
                changeYear: true,
                onSelect: function (selectedDate) {
                    $("#StartDate").datepicker("option", "maxDate", selectedDate);
                }
            });
            $('#btnReset').off('click').on('click', function () {
                $('#UserId').val('');
                $('#StartDate').val('');
                $('#EndDate').val('')
            });

            //$('#UserId').off('change').on('change', function () {
            //    $("#btnSearch").trigger("click");
            //});

            $('#btnSearch').off('click').on('click', function () {
                if (Global.IsNullOrEmptyString($('#UserId').val())) {
                    alert("Please select a user");
                    return;
                }
                var data = {
                    startDate: $("#StartDate").val(),
                    endDate: $("#EndDate").val(),
                    uid: $('#UserId').val()
                };
                $('.divoverlay').addClass('show');
                $.ajax({
                    type:"GET",
                    url: "Performance/GetPerformance",
                    data: data,
                    success: function (result) {
                        $('#performanceDetail').empty().html(result);
                        $('.divoverlay').removeClass('show');
                        $('.divoverlay').addClass('hide');
                    }
                });
            })

            $("#modal-performance-detail").on('loaded.bs.modal', function () {
                var modal = $(this);

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-perf-extra-detail").on('loaded.bs.modal', function () {
                var modal = $(this);

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
        }

        $this.init = function () {
            initialize();
        }
    }

    $(function () {
        var self = new PerformanceReport();
        self.init();
    });
}(jQuery));