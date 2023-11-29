(function ($) {
    function Conclusion() {
        var $this = this, form, modal;

        function InitForm() {

            var modal = $("#modal-conclusion");

            form = new Global.FormHelperWithFiles(modal.find("form"), {
                updateTargetId: "error-ModalMessage",
                validateSettings: { ignore: '' }
            }, function (result) {
                if (result.isSuccess) {
                    if ($.fn.HomeIndex) {
                        $.fn.HomeIndex.RefreshPreSales();
                    }
                    else if ($.fn.EstimateIndex) {
                        $.fn.EstimateIndex.bindEstimateGrid(false, false);
                    }
                    modal.modal('hide');
                    Global.ShowMessage(result.message, true, 'MessageDiv');
                }
                else {
                    Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageNotification');
                }
            });

            modal.find('#StatusUpdateDate').datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0"
            });

            modal.off('hidden.bs.modal').on('hidden.bs.modal', function () {
                modal.removeData('bs.modal');
                modal.find('.modal-content').empty();
            });


            $("#Status").on("change", function (e) {

                var statusVal = $('#Status').val();
                $.ajax({
                    type: "GET",
                    url: domain + 'estimate/GetChildStatus',
                    data: { statusVal: statusVal },
                    success: function (data) {
                        $('#ChildStatus').empty();
                        $('input:hidden[name=Conclusion]').val("");
                        if (data.length === 0) {
                            $('#ReasonChildStatus').hide();
                        } else {
                            $('#ReasonChildStatus').show();
                        }
                        for (var value of data) {
                            $('#ChildStatus')
                                .append(`<input type="checkbox" id="${value}" name="chkChildStatus" value="${value}">`)
                                .append(` <label for="${value}">${value}</label>`)
                                .append(`<br>`);
                        }
                    },
                    error: function () {
                        alert('Failed');
                    }
                });
            });


            $(document).on('change', 'input:checkbox[name="chkChildStatus"]', function (e) {
                var selectedStatus = "";
                $('input[name="chkChildStatus"]:checked').each(function () {
                    var id = $(this).attr("id");
                    if (selectedStatus === "") {
                        selectedStatus += id;
                    } else {
                        selectedStatus += ", " + id;
                    }
                });
                $('input:hidden[name=Conclusion]').val(selectedStatus);
            });
        }

        $this.init = function () {
            InitForm();
        };
    }

    $(function () {
        var self = new Conclusion();
        self.init();
    });
}(jQuery));
