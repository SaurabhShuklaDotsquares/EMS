(function ($) {
    function Expenses() {
        var $this = this, grid, summaryVM = new SummaryViewModel();
        var isVisibleColumn = false;

        function loadGrid() {
            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-expenseList', {
                serverSide: true,
                destroy: true,
                "pageLength": 15,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "expenses/index",
                        type: "Post",
                        data: getGridFilters()
                    },
                order: [[1, 'desc']],
                "columnDefs": [
                    { "width": "2%", "targets": 0, "className": "text-center", sortable: false },
                    { "width": "7%", "targets": 1, "className": "text-center", sortable: true },
                    { "width": "15%", "targets": 2, sortable: false },
                    { "width": "5%", "targets": 3, "className": "text-center", sortable: false },
                    { "width": "5%", "targets": 4, "className": "text-right", sortable: false },
                    { "width": "8%", "targets": 5, "className": "text-right", sortable: false },
                    { "width": "10%", "targets": 6, "className": "text-right", sortable: false },
                    { "width": "10%", "targets": 7, "className": "text-center", sortable: true },
                    { "width": "10%", "targets": 8, visible: isApprover, sortable: false },
                    { "width": "7%", "targets": 9, "className": "text-center", sortable: true },
                    { "width": "7%", "targets": 10, "className": "text-center", sortable: false },
                    { "width": "7%", "targets": 11, "className": "text-center", sortable: false },
                    { "width": "6%", "targets": 12, "className": "text-center", visible: isApprover, sortable: false, 'title': '<input id="select_all" type="checkbox" /> <label for="select_all">Select All<label>' },
                    { "width": "5%", "targets": 13, "className": "text-center", visible: !isApprover, sortable: false },
                    { "width": "5%", "targets": 14, "className": "text-center", sortable: false },
                ],
                columns:
                    [
                       { name: "rowIndex", data: "rowIndex", title: "#" },
                       { name: "ExpenseDate", data: "expenseDate", title: "Date" },
                       { name: "Description", data: "description", title: "Description" },
                       {
                           name: "Receipt", data: "receipt", title: "Receipt",
                           render: function (data, type, row, meta) {
                               return data ? "Yes" : "No";
                           }
                       },
                       { name: "Amount", data: "amount", title: "Amount" },
                       {
                           name: "CompanyCard", data: "paidThrough", title: "Company Card",
                           render: function (data, type, row, meta) {
                               var amount = "-";
                               if (data.indexOf('Company') > -1) {
                                   amount = row.amount;
                               }
                               return amount;
                           }
                       },
                       {
                           name: "CompanyCard", data: "paidThrough", title: "Cash/ Personal Card",
                           render: function (data, type, row, meta) {
                               var amount = "-";
                               if (data.indexOf('Cash') > -1) {
                                   amount = row.amount;
                               }
                               return amount;
                           }
                       },
                       { name: "CreateDate", data: "createDate", title: "Added Date" },
                       { name: "CreatedBy", data: "createdBy", title: "Requested By" },
                       {
                           name: "Status", data: "status", title: "Status",
                           render: function (data, type, row, meta) {
                               var text = "";
                               switch (data) {
                                   case "Approved":
                                       text = '<span style="color:green;">' + data + '</span><br>' + row.approvedBy;
                                       break;
                                   case "Rejected":
                                       text = '<span style="color:red;">' + data + '</span>';
                                       break;
                                   default:
                                       text = '<span style="color:#e99701;">' + data + '</span>';
                                       break;
                               }
                               return text;
                           }
                       },
                       //{
                       //    name: "IsReimbursed", data: "isReimbursed", title: "Paid?",
                       //    render: function (data, type, row, meta) {
                       //        var text = "-";

                       //        if (!row.isReimbursed) {
                       //            if (row.isReimbursable) {
                       //                if (row.reimburseAllowed) {
                       //                    text = "<a class='ablue' data-toggle='modal' data-target='#modal-mark-reimbursed' href='" + domain + "expenses/markasreimbursed/" + row.id + "'>Mark as Paid</a>";
                       //                }
                       //                else {
                       //                    text = "Not Paid";
                       //                }
                       //            }
                       //        }
                       //        else {
                       //            text = "Paid<br>" + row.reimburseDate;
                       //        }

                       //        return text;
                       //    }
                       //},
                        {
                            name: "IsReimbursed", data: "isReimbursed", title: "Paid?",
                            render: function (data, type, row, meta) {
                                var text = "-";

                                if (!row.isReimbursed) {
                                    if (row.isReimbursable) {
                                        if (row.reimburseAllowed) {
                                            text = '<input type="checkbox" class="reimburseCheck" value="' + row.id + '">';
                                        }
                                        else {
                                            text = "Not Paid";
                                        }
                                    }
                                }
                                else {
                                    text = "Paid<br>" + row.reimburseDate;
                                }

                                return text;
                            }
                        },
                       {
                           name: "Attachment", data: "receipt", title: "Attachment",
                           render: function (data, type, row, meta) {
                               if (data) {
                                   return "<a class='ablue' href='" + (domain + data) + "' target='_blank'><i class='fa fa-download'></i> Download</a>";
                               }
                               return "";
                           }
                       },
                       {
                           name: "ApprovalAllowed", data: "approvalAllowed",
                           render: function (data, type, row, meta) {                            
                               if (data) {                                 
                                   return '<input type="checkbox" class="expenseCheck" value="' + row.id + '">';
                               }
                               else {
                                   return "";
                               }
                           }
                       },
                       {
                           name: "Action", data: null, title: "Action",
                           render: function (data, type, row, meta) {
                               var actionButtons = '';
                               if (row.editAllowed) {
                                   actionButtons += $("<a/>", {
                                       id: "addEdit",
                                       class: "btn btn-default btn-sm",
                                       href: domain + "expenses/addedit/" + row.id,
                                       'data-toggle': "modal",
                                       'data-target': "#modal-add-expense",
                                       'data-backdrop': "static",
                                       html: $("<i/>", {
                                           class: "fa fa-pencil",
                                           style: "color:black"
                                       }).get(0).outerHTML + "&nbsp; Edit",
                                   }).get(0).outerHTML + "&nbsp; ";
                               }
                               return actionButtons;
                           }
                        },
                        {
                            name: "Delete", data: null, title: "Delete",
                            render: function (data, type, row, meta) {
                                var actionButtons = '';
                                if (row.deleteAllowed) {
                                    actionButtons += $("<a/>", {
                                        id: "delete",
                                        class: "btn btn-default btn-sm",
                                        href: domain + "expenses/deleteexpenses/" + row.id,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-add-expense",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "fa fa-trash",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp; Delete",
                                    }).get(0).outerHTML + "&nbsp; ";
                                }
                                return actionButtons;
                            }
                        },

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

            var expenseGrid = $('#grid-expenseList');

            expenseGrid.on('change', '#select_all', function () {
                expenseGrid.find('input[type="checkbox"].expenseCheck').prop("checked", this.checked);
            });

            expenseGrid.on('change', 'input[type="checkbox"].expenseCheck', function () {
                var expenseCheckBoxes = expenseGrid.find('input[type="checkbox"].expenseCheck');
                expenseGrid.find('#select_all').prop("checked", expenseCheckBoxes.length == expenseCheckBoxes.filter(":checked").length);
            });

            $("#modal-add-expense").on('loaded.bs.modal', function () {
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

                form.find("#ExpenseDate").datepicker({
                    dateFormat: "dd/mm/yy",
                    maxDate: new Date()
                });

                form.on("keypress keyup blur", ".decimal-number", function (event) {
                    $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
                    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                        event.preventDefault();
                    }
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

            //$("#modal-mark-reimbursed").on('loaded.bs.modal', function () {
            //    var modal = $(this);

            //    var form = new Global.FormHelperWithFiles(modal.find("form"), {
            //        updateTargetId: "NotificationMessage"
            //    }, function onSucccess(result) {
            //        if (result.isSuccess) {
            //            modal.modal('hide');
            //            grid.ajax.reload(null, false);
            //            Global.ShowMessage(result.message, true, "divMessage");
            //        } else {
            //            form.find("#NotificationMessage").html(result);
            //        }
            //    });

            //    form.find("#ReimburseDate").datepicker({
            //        dateFormat: "dd/mm/yy",
            //        maxDate: new Date()
            //    });

            //}).on('hidden.bs.modal', function () {
            //    $(this).removeData('bs.modal');
            //    $(this).find('.modal-content').empty();
            //});

            $("#modal-mark-reimbursed").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = modal.find("#reimbursedForm");
                var selectedCheckboxesLength = getReimburseCheckCheckboxes().length;
                if (selectedCheckboxesLength > 1) {
                    modal.find(".form-group.required.clearfix").prepend('<p>You have selected ' + getReimburseCheckCheckboxes().length + ' approved record(s) and  fill the payment date to mark them paid</p>');
                }
                else if (selectedCheckboxesLength == 1) {
                    modal.find(".form-group.required.clearfix").prepend('<p>You have selected ' + getReimburseCheckCheckboxes().length + ' approved record(s) and fill the payment date to mark this paid</p>');
                }

                form.find("#ReimburseDate").datepicker({
                    dateFormat: "dd/mm/yy",
                    maxDate: new Date()
                });
                var submit = modal.find("#markAsPaid");
                $("#markAsPaid").on("click", function (e) {
                    e.preventDefault();
                    var data = {
                        ExpenseIds: [],
                        ReimburseDate: $("#ReimburseDate").val()
                    };
                    var getSelectedCheckboxes = getReimburseCheckCheckboxes();
                    getSelectedCheckboxes.each(function () {
                        data.ExpenseIds.push(this.value);
                    });

                    if (!data.ExpenseIds.length) {

                        alert("Select at least one record from the list");
                        return;
                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: domain + "expenses/MarkAsReimbursed",
                            data: data,
                            success: function (result) {
                                if (result.isSuccess) {
                                    modal.modal('hide');
                                    grid.ajax.reload(null, false);
                                    Global.ShowMessage(result.message, true, "divMessage");
                                } else {
                                    form.find("#NotificationMessage").html(result);
                                }
                            }
                        });
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $(".updateStatus").on("click", function (e) {
                e.preventDefault();

                if ($(this).data('val') != "") {
                    var data = {
                        ExpenseIds: [],
                        IsApproved: $(this).data('val') == "1"
                    };

                    expenseGrid.find('input[type="checkbox"].expenseCheck:checked').each(function () {
                        data.ExpenseIds.push(this.value);
                    });

                    if (!data.ExpenseIds.length) {

                        alert("Select at least one record from the list");
                        return;
                    }

                    if (confirm('Are you confirm to mark selected records as ' + (data.IsApproved ? "Approved" : "Rejected") + "?")) {
                        $.ajax({
                            type: "POST",
                            url: domain + "expenses/updatestatus",
                            data: data,
                            success: function (result) {
                                if (result.isSuccess) {
                                    grid.ajax.reload(null, false);
                                    Global.ShowMessage(result.message, true, "divMessage");
                                } else {
                                    form.find("#NotificationMessage").html(result);
                                }
                            }
                        });
                    }

                }
            });
         
            $("#btnSearch").on("click", function () {
                loadGrid();
            });

            $("#btnReset").on("click", function () {
                $("#UserId").val('');
                $("#Status").val('');
                $("#DateFrom").val('');
                $("#DateTo").val('');
                loadGrid();
            });

            $("#DateFrom").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $("#DateTo").datepicker("option", "minDate", selectedDate);
                }
            });

            $("#DateTo").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                numberOfMonths: 1,
                onSelect: function (selectedDate) {
                    $("#DateFrom").datepicker("option", "maxDate", selectedDate);
                }
            });

            function getReimburseCheckCheckboxes() {
                return expenseGrid.find('input[type="checkbox"].reimburseCheck:checked');
            }

        }

        function getGridFilters() {
            return {
                UserId: $("#UserId").val(),
                Status: $("#Status").val(),
                DateFrom: $("#DateFrom").val(),
                DateTo: $("#DateTo").val()
            }
        }

        $this.init = function () {
            loadGrid();
            initialize();
        };
    }
    $(function () {
        var self = new Expenses();
        self.init();
    });
}(jQuery));