(function () {
    function ManageEstiamtePrice() {
        $this = this;
        $(document).on("change", '#EstimateRoleId', function () {
            var EstimateRoleId = $('#EstimateRoleId').val();
            window.location = `/EstimatePrice/ManageEstimatePrice/${EstimateRoleId}`;
            
            //var TechnologyParentId = $('#TechnologyParentId').val();

            //if ($('#EstimateRoleId').find(":selected").text() == "Developer") {
            //    $('.TechDiv').show();
            //} else {
            //    $('.TechDiv').hide();

            //}
            //$.ajax({
            //    url: domain + "EstimatePrice/fetchRoleDependentExperience",
            //    type: 'POST',
            //    datatype: 'application/json',
            //    data: { EstimateRoleId: EstimateRoleId, TechnologyParentId: TechnologyParentId },
            //    success: function (data) {
            //        $('#EstimateExpPricePartial').html(data);

            //    },
            //    error: function (ex) {
            //        alert("Whooaaa! Something went wrong.." + ex);
            //    }
            //});
        });

        $(document).on("change", '#EstimateTechnologyId', function () {
            var EstimateRoleId = $('#EstimateRoleId').val();
            var TechnologyParentId = $('#EstimateTechnologyId').val();
            window.location = `/EstimatePrice/ManageEstimatePrice/${EstimateRoleId}?technologyid=${TechnologyParentId}`;
        });
        function initializeForm() {

            new Global.FormValidationReset('#Form1', { ignore: '.validated' });
            $('#Form1').on('submit', function (e) {

                //if (isRoleUKPM == 1 && ($('#LeaveId').val() == '' || $('#LeaveId').val() == '0')) {
                //    if (!$('#Form1').hasClass('verified')) {
                //        e.preventDefault();
                //        //var uid = currentuserid;
                //        //if ($('#Uid').length > 0) {
                //        //    if ($('#IsSelfLeave').length == 0 || !$('#IsSelfLeave').prop('checked')) {
                //        //        uid = $('#Uid').val();
                //        //    }
                //        //}
                //        $('#Form1').validate({ ignore: '.validated' });
                //        //if ($('#Form1').valid()) {
                //        //    $.post(domain + "leave/checkforotherleave", { uid: uid, StartDate: $('#StartDate').val(), EndDate: $('#EndDate').val() }, function (data) {
                //        //        if (data.needToShowMessage && !confirm(data.message)) {

                //        //        }
                //        //        else {
                //        //            $('#Form1').addClass('verified');
                //        //            $('#Form1').submit();
                //        //        }
                //        //    });
                //        //}
                //    }
                //}

            });
        }
        $this.init = function () {
            initializeForm();
        }
    }
    $(function () {
        var self = new ManageEstiamtePrice;
        self.init();
    });
}(jQuery));