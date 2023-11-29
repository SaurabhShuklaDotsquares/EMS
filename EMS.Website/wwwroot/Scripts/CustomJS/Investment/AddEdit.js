(function ($) {
    'use strict';
    function Investment() {
        var $this = this, form, invProfileVM;

        var InvestmentDocViewModel = function (id, documentTypeId, documentName, documentUrl) {

            var self = this;
            self.Id = id || 0;
            self.DocumentTypeId = ko.observable(documentTypeId || '');
            self.DocumentName = documentName || '';
            self.DocumentUrl = documentUrl || '';
        };

        function InvestmentProfileViewModel(docs) {
            var self = this;
            self.InvestmentDocuments = ko.observableArray([]);

            self.addInvestmentDoc = function () {
                self.InvestmentDocuments.push(new InvestmentDocViewModel());
            };

            self.removeInvestmentDoc = function (doc) {
                self.InvestmentDocuments.remove(doc);
            }

            if (docs && docs != "" && docs.length) {
                var invDocs = [];

                $.each(docs, function () {
                    invDocs.push(new InvestmentDocViewModel(this.Id, '' + this.DocumentTypeId, this.DocumentName, this.DocumentUrl));
                });

                self.InvestmentDocuments(invDocs);

            }
            //else {
            //    self.addInvestmentDoc();
            //}
        }

        function InitializeForm() {

            invProfileVM = new InvestmentProfileViewModel(investmentDocs);

            ko.applyBindings(invProfileVM, $('#investmentDocList')[0]);

            form = new Global.FormHelperWithFiles($("form#investment_form"), {
                updateTargetId: "validation-summary", validateSettings: { ignore: '' }});

            form.on("keypress keyup blur", ".decimal-number", function (event) {
                $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
                if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                    event.preventDefault();
                }
            });

            form.on("keypress keyup blur", ".number-only", function (event) {
                $(this).val($(this).val().replace(/[^\d].+/, ""));
                if ((event.which < 48 || event.which > 57)) {
                    event.preventDefault();
                }
            });

            form.on("change", "#DocumentTypeId", function () {
                var type = $(this);
                var docName = type.closest(".row").find("input[name$='.DocumentName']");
                if (type.val() !== "" && type.val() != null) {
                    docName.val(type.find("option:selected").text());
                }
                else {
                    docName.val('');
                }
            });

            form.find("#DOB").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                yearRange: "-70:+0",
            });

            form.on('click', '#save', function (e) {
                e.preventDefault();
                if (confirm("Are you sure ?\nDetails will be submitted to account department and further changes will be locked.")) {
                    form.find("#IsDraft").val(false);
                    form.submit();
                }
            });
            
        }

        $this.init = function () {
            InitializeForm();
        }
    }
    $(function () {
        var self = new Investment();
        self.init();
    });
}(jQuery));