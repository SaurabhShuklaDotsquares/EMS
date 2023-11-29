(function () {
    function ProjectInfo() {
        $this = this;
        var tableCheckboxes=$(document).find('.itemCheckbox:checkbox');
        
        function initializeForm() {
            $("#lnkSubmit").on('click', function () {    
                var data = confirm("Are you sure to update records.")
                if (!data) {
                    return false;
                }
                var form = $("#frm-update-projects-info");
                var grid = new Global.FormHelper(form, {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: '' }
                }, function onBeforeSubmit() {                    
                   
                }, function onSucccess(result) {
                    if (result.isSuccess) {
                        window.location.href = result.redirectUrl;
                    }
                    else
                    {
                        Global.ShowMessage(result.errorMessage, false, 'validation-summary');
                    }
                });                
            });


            

            $("#chkAction").click(function () {
                tableCheckboxes.prop('checked', this.checked);
            });

            $(".itemCheckbox").click(function () {
                var chkElements = $('.itemCheckbox:checkbox:checked');
                console.log(chkElements.length);
                if (tableCheckboxes.length == chkElements.length) {
                    $("#chkAction").prop("checked", true);
                }
                else {
                    $("#chkAction").prop("checked", false);
                }
            });
        }

        $this.init = function () {
            initializeForm();
        }
    }
    $(function () {
        var self = new ProjectInfo();
        self.init();
    })

}(jQuery))