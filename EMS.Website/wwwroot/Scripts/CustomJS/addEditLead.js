(function ($) {

    function AddEditLead() {
        $this = this;

        function InitializeForm() {
            new Global.FormHelperWithFiles($('#frm-addEditLead'), { updateTargetId: "validation-summary" });


            CKEDITOR.replace('Notes');
            attachEventCKEditor('Notes');

            $('#AssignedDate').datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0"
            });

            $('#ConversionDate').datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0"
            });

            $("#LeadType").change(function (e) {
                if ($(this).find("option:selected").text() == "Almost Converted") {
                    $("#ConversionDateDiv").show();
                } else {
                    $("#ConversionDateDiv").hide();
                }
            });		

            $('#btn-submit').click(function (e) {
                var totalTechnician = $('input[name=Technician]:checked').length;
                var totalTechnology = $('input[name=Technology]:checked').length;
                if (totalTechnician <= 0) {
                    alert("alteast one technician must be selected");
                    return false;
                }
                if (totalTechnology <= 0) {
                    alert("atleast one technology must be selected");
                    return false;
                }
            });

            $('input[name=Technology][value=other]').change(function () {
                if (this.value == "other") {
                    if (this.checked) {
                        $('#TechnologyOther').removeAttr('readonly');
                    }
                    else {
                        $('#TechnologyOther').attr('readonly', 'readonly');
                        $('#TechnologyOther').val('');
                    }
                }
            }).change();

            $('#EstimateTimeInDays').keypress(function (evt) {
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                    return false;
                }

                return true;
            });

            $('#LeadCRMId').keypress(function (evt) {
                evt = (evt) ? evt : window.event;
                var charCode = (evt.which) ? evt.which : evt.keyCode;
                if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                    return false;
                }
                return true;
            });

            $('.delete-doc').click(function (e) {
                e.preventDefault();
                var lnk = $(this);
                if (confirm("Are you sure to delete this document ?")) {
                    var data1 = { id: $('#LeadId').val(), documentId: lnk.attr('id') }

                    $.ajax({
                        type: "POST",
                        url: domain + "estimate/deletedocument",
                        data: data1,
                        success: function (result) {
                            if (result.isSuccess) {
                                lnk.closest('.divUploadedFile').empty();
                            }
                            alert(result.message || result.errorMessage);
                        }
                    });
                }
            });
        }

        function attachEventCKEditor(instance) {
            CKEDITOR.instances[instance].on("instanceReady", function (e) {
                e.editor.document.on("keyup", function () {
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        }

        $this.init = function () {
            InitializeForm();
        };
    }

    $(function () {
        var self = new AddEditLead();
        self.init();
    })
})(jQuery);