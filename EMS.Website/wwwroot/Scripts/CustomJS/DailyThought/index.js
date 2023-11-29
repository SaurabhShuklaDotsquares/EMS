(function ($) {
    function index() {
        var $this = this;

        function intialize() {           
            var form = new Global.FormHelper($("form#daiyThought_form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                }, null, function (result) {                   
                    if (result.isSuccess) {                       
                        Global.ShowMessage(result.message, true, 'validation-summary');
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                    }
                });           
        }
      
        $this.init = function () {          
            intialize();
        };
    }

    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));