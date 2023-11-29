(function ($) {
    function ProjectInvoicelist() {
        var $this = this, grid, baList, tlList;

        function projectInvoiceGrid() {

            baList = $("#BAList").addClass("ba");
            tlList = $("#TLList").addClass("teamLeader");

            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-projectInvoiceList', {
                serverSide: true,
                destroy: true,
                paging: false,
                "bFilter": false,
                "ordering": false,
                "bLengthChange": false,
                "bInfo": false,
                "bSortable": false,
                "bAutoWidth": false,
                ajax:
                    {
                        url: domain + "Invoice/ProjectInvoiceList",
                        type: "POST",
                    },
                "columnDefs": [
                    { "width": "3%", "targets": 0 },
                    { "width": "15%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "15%", "targets": 5 },
                    { "width": "8%", "targets": 6 },
                   { "width": "5%", "targets": 7 },
                   { "width": "3%", "targets": 8 },
                   { targets: 'no-sort', orderable: false }
                ],
                columns:
                    [
                        {
                            name: "srNo", data: null, title: "#", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                return '<span>' + data.rowIndex + ''
                                + ' <input  type="hidden" name="hdProjectInvoiceID" class="hdProjectInvoiceID" value="' + data.projectInvoiceId + '"/>'
                                + ' <input  type="hidden"  name="hdProjectID" class="hdProjectID" value="' + data.projectId + '"/>'
                                + ' <input  type="hidden" name="hdClientName"  class="hdClientName" value="' + data.clientName + '"/>'
                                + ' <input  type="hidden"  name="hdinvoice_id" class="hdinvoice_id" value="' + data.id + '"/>'
                                + ' <input  type="hidden"  name="hdstart_date" class="hdstart_date" value="' + data.startDate + '"/>'
                                + ' <input  type="hidden" name="hdend_date" class="hdend_date" value="' + data.endDate + '"/>'
                                + ' <input  type="hidden" name="hdtotal_amount" class="hdtotal_amount" value="' + data.invoiceAmount + '"/>'
                                + ' <input  type="hidden" name="hdpayment_status_id" class="hdpayment_status_id" value="' + data.invoiceStatus + '"/>'
                                + ' <input  type="hidden" name="hdCurrencyID" class="hdCurrencyID" value="' + data.currencyID + '"/>';
                            }
                        },
                        { name: "Project Name", data: "projectName", title: "Project Name", sortable: false, searchable: false, visible: true },
                        { name: "Invoice Number", data: "invoiceNumber", title: "Invoice Number", sortable: false, searchable: false, visible: true },
                        { name: "Invoice Date", data: "invoiceDate", title: "Invoice Date", sortable: false, searchable: false, visible: true },
                        { name: "Invoice Status", data: "invoiceStatus", title: "Invoice Status", sortable: false, searchable: false, visible: true },
                        {
                            name: "baname", data: null, title: "BA Name", sortable: false, searchable: false, render: function (data, type, dataRow, meta) {
                                return dataRow.baId ? baList.clone().find("option[value='" + dataRow.baId + "']").attr("selected", true).end().get(0).outerHTML : baList.clone().get(0).outerHTML;
                            }
                        },
                          {
                              name: "Team Leader", data: null, title: "Team Leader", sortable: false, searchable: false, render: function (data, type, dataRow, meta) {
                                  return dataRow.tlId ? tlList.clone().find("option[value='" + dataRow.tlId + "']").attr("selected", true).end().get(0).outerHTML : tlList.clone().get(0).outerHTML;
                              }
                          },
                        { name: "Action", data: "action", title: "Action", sortable: false, searchable: false, visible: true },
                        {
                            name: "", data: null, title: "<input type='checkbox' class='editor-active' id='chkSelectAll'>", sortable: false, searchable: false, render: function (data, type, dataRow, meta) {

                                return "<input type='checkbox' class='editor-active chkrow clickrow'>";
                            }
                        }
                    ],
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                    $("#chkSelectAll").on("change", function () {
                        if ($(this).is(":checked")) {
                            $(".chkrow").prop("checked", true);
                        }
                        else {
                            $(".chkrow").prop("checked", false);
                        }
                    });
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

        function checkTablerow() {
            $("#updateCrminvoices").off('click').on('click', function () {

                var objList = [];
                var hasError = false;
                $.each($(".clickrow:checked"), function () {
                    var row = $(this).closest("tr");
                    var $td = row.find("td:eq(0)");
                    var $td5 = row.find("td:eq(5)");
                    var $td6 = row.find("td:eq(6)");

                    if ($td5.find(".ba").val() == "" || $td6.find(".teamLeader").val() == "") {
                        hasError = true;
                        return false;
                    }

                    var obj1 = {
                        ProjectInvoiceID: $td.find(".hdProjectInvoiceID").val(),
                        ProjectID: $td.find(".hdProjectID").val(),
                        ClientName: $td.find(".hdClientName").val(),
                        invoice_id: $td.find(".hdinvoice_id").val(),
                        start_date: $td.find(".hdstart_date").val(),
                        end_date: $td.find(".hdend_date").val(),
                        invoice_amount: $td.find(".hdtotal_amount").val(),
                        invoice_payment_status_id: $td.find(".hdpayment_status_id").val(),
                        CurrencyID: $td.find(".hdCurrencyID").val(),
                        BA_ID: $td5.find(".ba").val(),
                        TL_ID: $td6.find(".teamLeader").val()
                    }
                    objList.push(obj1);
                });

                if (!hasError) {
                    if (objList.length) {
                        $('.divoverlay').removeClass('hide');

                        $.ajax({
                            url: "Invoice/UpdateCrmInvoice",
                            type: 'post',
                            data: { model: objList },
                            success: function (result) {

                                if (result.isSuccess) {
                                    grid.ajax.reload();
                                    Global.ShowMessage(result.message, true, 'NotificationMessage');
                                }
                                else {
                                    Global.ShowMessage(result.message, false, 'NotificationMessage');
                                }
                            },
                            error: function (result) {
                                Global.ShowMessage(result.message, false, 'NotificationMessage');
                            },
                            complete: function (result) {
                                $('.divoverlay').addClass('hide');
                            }
                        });
                    }
                    else {
                        $('.divoverlay').addClass('hide');
                        Global.ShowMessage("Please check atleast one row to update!", false, 'NotificationMessage');
                    }
                }
                else {
                    $('.divoverlay').addClass('hide');
                    Global.ShowMessage("Please select BA and Team Leader in all selected rows to update!", false, 'NotificationMessage');
                }
            })
        }

        $this.init = function () {
            projectInvoiceGrid();
            checkTablerow();
        }
    }

    $(function () {
        var self = new ProjectInvoicelist();
        self.init();
    })
})(jQuery);