(function ($) {
    function Invoice() {

        var invoiceGrid, $this = this;

        function GetFilter() {
            var data = {
                FromDate: $('#txt_From').val(),
                ToDate: $('#txt_To').val(),
                BAId: parseInt($('#ddl_BaList').val()),
                TLId: parseInt($('#ddl_TlList').val()),
                Name: $('#txt_search').val()
            };
            return data;
        }

        function loadInvoiceGrid() {

            $('.divoverlay').removeClass('hide');
            invoiceGrid = new Global.GridHelper('#grid-invoice', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bLengthChange": false,
                "ordering": false,
                "bFilter": false,
                "bAutoWidth": false,
                "dom": 'Bfrtip',
                buttons: [
                  'excel'
                ],
                ajax:
                    {
                        url: domain + "Invoice/Index",
                        type: "Post",
                        data: GetFilter()
                    },
                order: [[0, "desc"]],
                "columnDefs": [

                    { "width": "12%", "targets": 0 },
                    { "width": "22%", "targets": 1 },
                    { "width": "8%", "targets": 2 },
                    { "width": "10%", "targets": 3, "className": "text-center" },
                    { "width": "15%", "targets": 4 },
                    { "width": "7%", "targets": 5, "className": "text-right" },
                    { "width": "10%", "targets": 6, "className": "text-center" },
                    { "width": "5%", "targets": 7, "className": "text-center" },
                ],
                columns:
                    [
                        {
                            name: "CRMProjectId", data: "crmProjectId", title: "Client", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                return "CRM ID:[" + row.crmProjectId + "]<br>" + row.clientName;
                            }
                        },
                        {
                            name: "ProjectName", data: "projectName", title: "Project Name", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                return row.projectName + "<br><span style='font-size:11px'>Last Activity</span>: " + row.lastActivity;
                            }
                        },
                        { name: "InvoiceNumber", data: "invoiceNumber", title: "Invoice Number", sortable: false, searchable: false, visible: true },
                        {
                            name: "InvoiceDate", data: "invoiceDate", title: "Date", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                return row.invoiceStartDate + " /<br>" + row.invoiceEndDate;
                            }
                        },
                        { name: "BAName", data: "baName", title: "BA Name", sortable: false, searchable: false, visible: true },
                        { name: "TLName", data: "tlName", title: "TL Name", sortable: false, searchable: false, visible: true },
                        {
                            name: "InvoiceStatus", title: "Status", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                return ' <a class="ablue" href="' + domain + 'Invoice/InvoiceStatus?InvoiceId=' + dataRow.id + '" data-toggle="modal" data-target="#modal-invoiceStatus"> '
                                    + dataRow.invoiceStatus +
                                    ' </a>';
                            }
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = "";
                                if (dataRow.showChaseIcon) {
                                    actionButtons += '<a data-toggle="modal" title="chase" data-target="#modal-chaseInvoice" href="' + domain + 'Invoice/ChaseInvoice?Id=' + dataRow.id + '"><i class="fa fa-share blue" style="color:black"></i></a>&nbsp;&nbsp;';
                                }
                                actionButtons += '<a data-toggle="modal" title="view detail" data-target="#modal-ViewInvoice" href="' + domain + 'Invoice/ViewInvoice?Id=' + dataRow.id + '" ><i class="fa fa-eye blue"  style="color:black"></i></a>&nbsp;&nbsp;';
                                if (dataRow.showDelete) {
                                    actionButtons += '<a data-toggle="modal"  title="add/edit" data-target="#modal-AddEditInvoice" href="' + domain + 'Invoice/AddEditInvoice?Id=' + dataRow.id + '" ><i class="fa fa-pencil blue"  style="color:black"></i></a>&nbsp;&nbsp;';
                                    actionButtons += '<a data-toggle="modal" title="delete" data-target="#modal-deleteInvoice" href="' + domain + 'Invoice/CancelInvoice?Id=' + dataRow.id + '"><i class="fa fa-trash blue"  style="color:black"></i></a>';
                                }
                                return actionButtons;
                            }
                        }
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    $(nRow).addClass(aData.cssClass);
                },

                "fnDrawCallback": function (oSettings) {

                    $('.divoverlay').addClass('hide');
                    if (oSettings.fnRecordsDisplay() > 0) {
                        $("#btnExcel").show();
                    }
                    else {
                        $("#btnExcel").hide();
                    }
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                },

            });
        }

        function initilizeControls() {

            $('#CustomSearch').click(function () {
                loadInvoiceGrid();
            });

            $('#btn_reset').click(function () {
                $('#txt_search').val('');
                $('#txt_From').val('');
                $('#txt_To').val('');
                $('#ddl_BaList').val('');
                $('#ddl_TlList').val('');
                loadInvoiceGrid();
            });

            $("#txt_From").datepicker({
                dateFormat: 'dd/mm/yy',
                showOtherMonths: true,
                selectOtherMonths: true
            });

            $("#txt_To").datepicker({
                dateFormat: 'dd/mm/yy',
                showOtherMonths: true,
                selectOtherMonths: true
            });

        }

        function initializeModal() {

            $("#modal-AddEditInvoice").on('loaded.bs.modal', function () {
                var modal = $(this);

                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: '' }
                }, null, function (result) {

                    if (result.isSuccess) {
                        invoiceGrid.ajax.reload();
                        modal.modal('hide');
                        Global.ShowMessage(result.message, true, 'validation-summary');
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageDiv');
                    }

                });

                form.on("keypress keyup blur", ".decimal-number", function (event) {
                    $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
                    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                        event.preventDefault();
                    }
                });

                form.find("#StartDate").datepicker({
                    dateFormat: "dd/mm/yy",
                    onSelect: function (selectedDate) {
                        form.find("#EndDate").datepicker("option", "minDate", selectedDate);
                    }
                });

                form.find("#EndDate").datepicker({
                    dateFormat: "dd/mm/yy",
                    onSelect: function (selectedDate) {
                        form.find("#StartDate").datepicker("option", "maxDate", selectedDate);
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-invoiceStatus").on('loaded.bs.modal', function () {
                var modal = $(this);

                var form = new Global.FormHelper(modal.find("form"),
                    {
                        updateTargetId: "validation-summary",
                        validateSettings: { ignore: 'disabled' }
                    }, null, function (result) {

                        if (result.isSuccess) {
                            invoiceGrid.ajax.reload();
                            modal.modal('hide');
                            Global.ShowMessage(result.message, true, 'validation-summary');
                        }
                        else {
                            Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageDiv');
                        }

                    });

                form.on("change", "#InvoiceStatusId", function (event) {
                    if ($("#InvoiceStatusId").val() == "2") {
                        $('.remainamnt').removeClass('hide');
                        $(".remainamnt #RemainingAmount").attr('disabled', false);
                    }
                    else {
                        $('.remainamnt').addClass('hide');
                        $(".remainamnt #RemainingAmount").attr('disabled', true);
                    }
                });

                form.on("keypress keyup blur", ".decimal-number", function (event) {
                    $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
                    if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                        event.preventDefault();
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-deleteInvoice").on('loaded.bs.modal', function () {
                var modal = $("#modal-deleteInvoice");
                var form = new Global.FormHelper($(this).find("form"),
                    {
                        updateTargetId: "validation-summary",
                        validateSettings: { ignore: '' }
                    }, null, function (result) {

                        if (result.isSuccess) {
                            invoiceGrid.ajax.reload();
                            modal.modal('hide');
                            Global.ShowMessage(result.message, true, 'validation-summary');
                        }
                        else {
                            Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageDiv');
                        }

                    });
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
        }

        $this.init = function () {
            loadInvoiceGrid();
            initilizeControls();
            initializeModal();
        }
    }

    $(function () {
        var self = new Invoice();
        self.init();
    });
})(jQuery);