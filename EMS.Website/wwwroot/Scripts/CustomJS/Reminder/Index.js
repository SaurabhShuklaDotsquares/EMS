(function ($) {
    function index() {
        var $this = this, grid;

        $("#btnSearch").on("click", function () {

            intializeGrid();
        })

        $("#btnReset").on("click", function () {
            $('#ActiveStatusId').val(0)
            $('#EmployeeId').val('0')
            $("#DateFrom").val(null)
            $("#DateTo").val(null)
            intializeGrid();
        })

        $(document).on('click', '#btn-Acticedeactive', function () {

            var status = $('#idStatus').val();

            $.ajax({
                url: domain + 'Reminder/isComplete?id=' + status,
                type: 'POST',
                dataType: 'json',
                //data: {id: "dada" },
                contentType: false,
                processData: false,
                success: function (result) {

                    if (result.isSuccess) {
                        swal({
                            title: "Alert!",
                            text: result.message,
                            icon: "success",
                        }).then(function () {
                            location.reload();
                        });
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'validation-summary');
                    }
                }
            });
        });
        $(document).on('click', '#btn-ActicedeactiveNo', function () {
            location.reload();
        });

        function intializeGrid() {
            $('.loading-common,.loading-overlay').show()

            grid = new Global.GridHelper('#grid-reminder-table', {
                serverSide: true,
                destroy: true,
                ordering: false,
                searchDelay: 800,
                "pageLength": 25,
                "bFilter": true,
                "bAutoWidth": false,
                "bLengthChange": false,
                "language": {
                    searchPlaceholder: "Search By Title"
                },
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                {
                    url: domain + "reminder/reminderindex",
                    type: "POST",
                    data: getGridFilters()



                },

                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "27%", "targets": 1 },
                    { "width": "8%", "targets": 2 },
                    { "width": "30%", "targets": 3 },
                    { "width": "10%", "className": "text-center", "targets": 4 },
                    { "width": "16%", "className": "text-center", "targets": 5 },
                    { "width": "4%", "targets": 6 },
                ],
                columns:
                    [
                        {
                            name: "rowIndex",
                            data: "rowIndex",
                            title: "#",
                            sortable: false,
                            searchable: false,
                            visible: true
                        },
                        {
                            name: "Title",
                            data: "title",
                            title: "Title",
                            sortable: false,
                            searchable: false,
                            visible: true
                        },
                        {
                            name: "ReminderDate",
                            data: "reminderDate",
                            title: "Reminder Date",
                            sortable: false,
                            searchable: false,
                            visible: true
                        },
                        {
                            name: "ReminderWith",
                            data: "reminderWith",
                            title: "Reminder With",
                            sortable: false,
                            searchable: false,
                            visible: true
                        },
                        {
                            name: "IsActive",
                            data: "runningStatus",
                            title: "Running Status",
                            sortable: false,
                            searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '<center>';

                                actionButtons += '<div class="chk-box dis-block clearfix">';
                                if (row.runningStatus == "Running") {
                                    actionButtons += '<span  class="label label-success"> Running</span>';
                                }
                                else {
                                    actionButtons += '<span class="label label-danger"> Completed</span>';
                                }                                
                                actionButtons += '</div>&nbsp;&nbsp;'

                                return actionButtons;
                            },

                        },
                        //{
                        //    name: "IsActive",
                        //    data: "runningStatus",
                        //    title: "Running Status",
                        //    sortable: false,
                        //    searchable: false,
                        //    visible: true
                        //},
                        {
                            name: "CompleteStatus",
                            data: "runningStatus",
                            title: "Mark as complete",
                            sortable: false,
                            searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '<center>';

                                actionButtons += '<div class="chk-box dis-block clearfix">';
                                if (row.runningStatus == "Running") {
                                    actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + row.id + '" checked/><span class="slider round"></span></label>';
                                }
                                else {
                                    actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" disabled value="' + row.id + '" /><span class="slider round"></span></label>';
                                }
                                actionButtons += '<label for=isApproved"></label>'
                                actionButtons += '</div>&nbsp;&nbsp;'

                                return actionButtons;
                            },

                        },
                        {
                            name: "Action",
                            data: null,
                            title: "Action",
                            sortable: false,
                            searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = "";

                                actionButtons = $("<a/>", {
                                    id: "editreminder",
                                    title: "edit",
                                    href: domain + "reminder/add/" + data.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-edit-reminder",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "glyphicon glyphicon-edit",
                                        style: "color:black;align: center;"
                                    }),
                                }).get(0).outerHTML + "&nbsp; " + $("<a/>", {
                                    href: domain + "reminder/delete/" + data.id,
                                    id: "deletereminder",
                                    title: "delete",
                                    'data-toggle': "modal",
                                    'data-target': "#modal-delete-reminder",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "glyphicon glyphicon-trash",
                                        style: "color:black;align: center;"
                                    }),
                                }).get(0).outerHTML;

                                return actionButtons;
                            }
                        },
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                },
                "fnDrawCallback": function (oSettings) {
                    $('.loading-common,.loading-overlay').hide();
                    $('.switchBox').on("change", function () {
                        debugger;
                        $("#modalTurnOff").modal();
                        $("#idStatus").val(this.value);
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

        function intializeModalWithForm() {

            $("#modal-add-edit-reminder").on('loaded.bs.modal', function () {

                var modal = $(this);

                $("#modal-add-edit-reminder #reminderDate").datepicker({
                    dateFormat: "dd/mm/yyyy"
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-delete-reminder").on('loaded.bs.modal', function () {

                var modal = $(this);

                var form = new Global.FormHelper($(this).find("form"),
                    {
                        updateTargetId: "validation-summary",
                        validateSettings: { ignore: '' }
                    }, null, function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            location.href = domain + result.redirectUrl;
                            //grid.ajax.reload(null, false);
                            //Global.ShowMessage(result.message, true, 'MessageDiv');
                        }
                        else {
                            //Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageDiv');
                        }
                    });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-isComplete-reminder").on('loaded.bs.modal', function () {

                var modal = $(this);

                var form = new Global.FormHelper($(this).find("form"),
                    {
                        updateTargetId: "validation-summary",
                        validateSettings: { ignore: '' }
                    }, null, function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload(null, false);
                            Global.ShowMessage(result.message, true, 'MessageDiv');
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

        function getGridFilters() {
            return {
                EmployeeId: $('#EmployeeId').val(),
                ActiveStatusId: $('#ActiveStatusId').val(),
                DateFrom: $("#DateFrom").val(),
                DateTo: $("#DateTo").val()
            }
        }

        $this.init = function () {
            intializeGrid();
            intializeModalWithForm();
        };
    }

    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));