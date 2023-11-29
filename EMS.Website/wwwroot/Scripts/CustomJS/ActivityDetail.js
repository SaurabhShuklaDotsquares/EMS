/*global jQuery, Global,secureDomain */

(function () {
    function ActivityDetail() {
        $this = this;

        function initializeForm() {
            var teamFilters = $('.team-filter');

            $('input[type="radio"][name="PMId"]').on('change', function (e) {
                GetDetailActivityByPM(parseInt(this.value));
            });

            var filterPMId = $("#filterPMId").val();
            filterPMId = filterPMId && filterPMId !== "" ? parseInt(filterPMId) : 0;
            if (filterPMId) {
                $('input[name="PMId"][value="' + filterPMId + '"]').prop('checked', true);
            }
        }

        function GetDetailActivityByPM(pmId) {

            pmId = pmId ? pmId : 0;
            $('.divoverlay').removeClass('hide');

            $.get(domain + 'activity/GetDetailByPM/' + pmId,
                    function (response) {
                        $("#team_summary").html(response);
                        $('.divoverlay').addClass('hide');
                    });
        }

        $this.init = function () {
            initializeForm();
        }
    }


    $(function () {
        var self = new ActivityDetail();
        self.init();
    });

}(jQuery));
