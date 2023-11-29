/*global jQuery, Global,secureDomain */
(function () {
    function UpdateUserInfo() {
        $this = this;


        $("#btn_Request").on('click', function myfunction() {
            if (confirm("This will update technology information for all users in CRM.\nDo you want to continue?")) {
                $(".classUid").each(function (e) {
                    UpdateAllUserTechInfoInCRMByUid($(this).text());
                });
            }
        });

        function UpdateAllUserTechInfoInCRMByUid(uid) {
            $('#ResultFromCRM_TD_' + uid).addClass('status_inprogress');
            $.post(domain + 'user/UpdateAllUserTechInfoInCRM', { Uid: uid }, function (data) {
                if (data != null) {
                    $('#ResultFromCRM_' + data.data).text(data.message);
                    $('#ResultFromCRM_TD_' + data.data).removeClass('status_inprogress');
                    if (data.success == true) {
                        $('#ResultFromCRM_TD_' + data.data).addClass('status_success');
                    } else if (data.success == false) {
                        $('#ResultFromCRM_TD_' + data.data).addClass('status_fail');
                    }
                    //alert("success");
                }
                else {
                    alert("Error");
                }
            });
        }


        $this.init = function () {
            
        };
    }

    $(function () {
        var self = new UpdateUserInfo;
        self.init();
    });

}(jQuery));
