(function ($) {
    function Handover() {
        var $this = this, grid, summaryVM = new SummaryViewModel();
        var isVisibleColumn = false;

        function loadGrid() {
            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-handoverList', {
                serverSide: true,
                destroy: true,
                "pageLength": 15,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "Handover/index",
                    type: "Post",
                    data: getGridFilters()
                },
                order: [[1, 'desc']],
                "columnDefs": [
                    { "width": "2%", "targets": 0, "className": "text-center", sortable: false },
                    { "width": "7%", "targets": 1, "className": "text-center", sortable: true },
                    { "width": "15%", "targets": 2, sortable: true },
                    { "width": "5%", "targets": 3, "className": "text-center", sortable: false },
                    { "width": "5%", "targets": 4, "className": "text-center", sortable: true },
                    { "width": "8%", "targets": 5, "className": "text-center", sortable: false },
                    
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#" },
                        { name: "date", data: "date", title: "Date" },
                        { name: "projectModel", data: "projectModel", title: "Project Model" },
                        { name: "ba", data: "ba", title: "BA" },                        
                        { name: "tl", data: "tl", title: "TL" }, 
                        { name: "developer", data: "developer", title: "Developer" },
                       
                    ],
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');

                    if (oSettings.json.isVisible) {
                        isVisibleColumn = true;
                    }

                    if (oSettings._iDisplayStart === 0) {
                        var showSummary = false;

                        if (oSettings.fnRecordsDisplay() > 0) {

                            if (oSettings.json.Summary) {

                                var summaries = JSON.parse(oSettings.json.Summary);

                                if (summaries && summaries.length) {

                                    summaryVM.summaries.removeAll();

                                    $.each(summaries, function () {
                                        summaryVM.summaries.push(new Summary
                                            (this.Status,
                                                this.TotalSummary.length ? this.TotalSummary.join(", ") : "-",
                                                this.CompanyCardSummary.length ? this.CompanyCardSummary.join(", ") : "-",
                                                this.CashOrPersonalCardSummary.length ? this.CashOrPersonalCardSummary.join(", ") : "-"));
                                    });

                                    showSummary = true;
                                }
                            }
                        }
                        summaryVM.showSummary(showSummary);
                    }

                    if (oSettings.fnRecordsDisplay() > 0) {
                        $('.btn-excel').show();
                    }
                    else {
                        $('.btn-excel').hide();
                    }

                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');

                }
            });
        }

        function SummaryViewModel() {
            var self = this;
            self.showSummary = ko.observable(false);
            self.summaries = ko.observableArray([]);
        }

        function Summary(status, totalSummary, companyCardSummary, cashOrPersonalCardSummary) {
            var self = this;
            self.Status = status || "";
            self.TotalSummary = totalSummary || "";
            self.CompanyCardSummary = companyCardSummary || "";
            self.CashOrPersonalCardSummary = cashOrPersonalCardSummary || "";
        }

        function initialize() {

            ko.applyBindings(summaryVM, $("#expenseSummary")[0]);

            var handoverGrid = $('#grid-handoverList');

            handoverGrid.on('change', '#select_all', function () {
                handoverGrid.find('input[type="checkbox"].expenseCheck').prop("checked", this.checked);
            });

            handoverGrid.on('change', 'input[type="checkbox"].expenseCheck', function () {
                var expenseCheckBoxes = handoverGrid.find('input[type="checkbox"].expenseCheck');
                handoverGrid.find('#select_all').prop("checked", expenseCheckBoxes.length == expenseCheckBoxes.filter(":checked").length);
            });

            $("#modal-add-handover").on('loaded.bs.modal', function () {
                var modal = $(this);

                var form = new Global.FormHelperWithFiles(modal.find("form"), {
                    updateTargetId: "NotificationMessage"
                }, function onSucccess(result) {
                    if (result.isSuccess) {
                        modal.modal('hide');
                        grid.ajax.reload(null, false);
                        Global.ShowMessage(result.message, true, "divMessage");
                    } else {
                        form.find("#NotificationMessage").html(result);
                    }
                });

                form.on("keypress keyup blur", ".decimal-number", function (event) {
                    $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
                    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                        event.preventDefault();
                    }
                });
                form.find(".multiple").select2({
                    allowClear: true,
                    width: '100%',
                    matcher: matchCustom
                });

                form.find("input[name='HasReceipt']").on("change", function () {
                    if (this.value == "true") {
                        form.find("#Receipt").removeClass("hidden").prop("disabled", false);
                        form.find(".receiptDocument").removeClass("hidden");
                    }
                    else {
                        form.find("#Receipt").addClass("hidden").prop("disabled", true).val("");
                        form.find(".receiptDocument").addClass("hidden");
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });


            $("#btnSearch").on("click", function () {
                loadGrid();
            });

            $("#btnReset").on("click", function () {
                $("#ProjectId").val('');
                $("#BAId").val('');
                $("#TLId").val('');
                $("#DeveloperId").val('');
                loadGrid();
            });

          

            function getReimburseCheckCheckboxes() {
                return handoverGrid.find('input[type="checkbox"].reimburseCheck:checked');
            }
            function matchCustom(params, data) {
                // If there are no search terms, return all of the data
                if ($.trim(params) === '') {
                    return data;
                }

                // Do not display the item if there is no 'text' property
                if (typeof data === 'undefined') {
                    return null;
                }

                var searchCritarea;
                if (data.indexOf("(") > -1) {
                    var endIndex = data.indexOf("(");
                    searchCritarea = data.substring(0, parseInt(endIndex));
                    searchCritarea = $.trim(searchCritarea).toLowerCase();
                }
                else {
                    searchCritarea = $.trim(data).toLowerCase();
                }


                if (searchCritarea.indexOf(params.toLowerCase()) > -1) {
                    return data;
                }
            }
        }

        function getGridFilters() {
            return {
                ProjectId: $("#ProjectId").val(),
                BAId: $("#BAId").val(),
                TLId: $("#TLId").val(),
                DeveloperId: $("#DeveloperId").val()
            }
        }

        $this.init = function () {
            loadGrid();
            initialize();
        };
    }
    $(function () {
        var self = new Handover();
        self.init();
    });
}(jQuery));