(function ($) {
    function DeviceMaster() {
        var $this = this, grid;

        function loadGrid() {
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-device-table', {
                serverSide: true,
                destroy: true,
                ordering: false,
                "pageLength": 15,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "device/devicemaster",
                    type: "POST"
                },
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "8%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "5%", "targets": 3 },
                    { "width": "8%", "targets": 4 },
                    { "width": "8%", "targets": 5 },
                    { "width": "4%", "targets": 6 },
                ],
                columns:
                [
                    { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                    {
                        name: "DeviceName", data: "deviceName", title: "Device", sortable: false, searchable: false, visible: true,
                        render: function (data, type, row, meta) {
                            return row.simDetails == "" ? data : data + "<br>" + row.simDetails;
                        }
                    },
                    { name: "Condition", data: "condition", title: "Description", sortable: false, searchable: false, visible: true },
                    {
                        name: "Quantity", data: "quantity", title: "Quantity", sortable: false, searchable: false, visible: true,
                        render: function (data, type, row, meta) {
                            return "<b style='font-weight:600'>Quantity : " + data + "</b><br>Assg. Qty : " + row.assignedQuantity + "<br>Stock : " + (data - row.assignedQuantity);
                        }
                    },
                    {
                        name: "AddedBy", data: "addedBy", title: "Added By", sortable: false, searchable: false, visible: true,
                        render: function (data, type, row, meta) {
                            return data + "<br>" + row.addedDate;
                        }
                    },
                    {
                        name: "ModifiedBy", data: "modifiedBy", title: "Modified By", sortable: false, searchable: false, visible: true,
                        render: function (data, type, row, meta) {
                            return data + "<br>" + row.modifiedDate;
                        }
                    },
                    {
                        name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                        render: function (data, type, row, meta) {
                            var actionButtons = '';
                            actionButtons += $("<a/>", {
                                id: "addEdit",
                                class: "btn btn-default btn-sm",
                                href: domain + "device/AddEditDevice/" + row.id,
                                'data-toggle': "modal",
                                'data-target': "#modal-add-edit-device",
                                'data-backdrop': "static",
                                html: $("<i/>", {
                                    class: "fa fa-pencil",
                                    style: "color:black"
                                }).get(0).outerHTML + "&nbsp; Edit",
                            }).get(0).outerHTML + "&nbsp; ";

                            return actionButtons;
                        }
                    }
                ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                },
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
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

        function initialize() {

            $("#modal-add-edit-device").on('loaded.bs.modal', function () {

                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "NotificationMessage"
                }, null,
                    function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload();
                            Global.ShowMessage(result.message, true, "NotificationMessage");
                        } else {
                            form.find("#NotificationMessage").html(result);
                        }
                    });

                var deviceId = parseInt(form.find("#Id").val());

                hideShowSimControls(form);

                form.on("keypress keyup blur", ".number", function (event) {
                    $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
                    if ((event.which < 48 || event.which > 57)) {
                        event.preventDefault();
                    }
                });

                form.on("change", "#Quantity", function () {
                    if (deviceId > 0) {
                        var spnQuanityError = form.find('#spnQuanityError');
                        spnQuanityError.html('');
                        var inputQty = parseInt(this.value);
                        var assignedQty = parseInt(form.find("#AssignedQuantity").val());

                        if ((inputQty == 0 || inputQty) && assignedQty > inputQty) {
                            spnQuanityError.html("Device already assigned to " + assignedQty + " users, so quantity must be greater than or equal to " + assignedQty);
                            form.find(":submit").prop("disabled", true);
                        }
                        else {
                            form.find(":submit").prop("disabled", false);
                        }
                    }
                });

                form.on("change", "#DeviceType", function () {
                    hideShowSimControls(form);
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            var queryString = Global.GetUrlVars();
            if (queryString && queryString.option && queryString.option.trim() == "add") {
                $("#AddDevice").click();
            }
        }

        function hideShowSimControls(parent) {

            var deviceType = parent.find("#DeviceType").val();
            var simFields = parent.find('.sim');
           
            if (deviceType == deviceTypeSIM) {
                simFields.show().find(":input").data('rule-required', true).data('msg-required', '*required');
                parent.find("#Quantity").val('1').prop("disabled",true);
            }
            else {
                simFields.hide().find(":input").data('rule-required', true).data('msg-required', '*required');
                parent.find("#Quantity").val('').prop("disabled", false);
            }
        }

        $this.init = function () {
            loadGrid();
            initialize();
        };
    }
    $(function () {
        var self = new DeviceMaster();
        self.init();
    });
}(jQuery));