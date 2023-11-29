(function () {
    function AddEdit() {
        var $this = this, form;

        function InitializeForm() {

            form = new Global.FormHelper($("form#lessonlearnedForm"), {
                updateTargetId: "validation-summary", validateSettings: { ignore: '' }
            }, function (xhr, data) {
                if (!form.find('textarea').filter(function () { return this.value.trim() != "" }).length) {
                    xhr.abort();
                    alert('Please add text for any of below\n\n- What Went Good?\n- What Went Bad?\n- What Learned?');
                }
            });
        }

        $this.init = function () {
            InitializeForm();
        }
    }

    $(function () {
        var self = new AddEdit();
        self.init();
    })

}(jQuery));