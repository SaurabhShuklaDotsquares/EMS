/*global jQuery, Global,secureDomain */
(function () {

    function EmployeeStatusReport() {
        $this = this;

        function initializeForm() {
            $("#pmId").on('change', function () {
                GetProjectManagerUsers();
            });
            BindDatePickers();
        }

        function BindDatePickers() {

            $("#StartDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("#EndDate").datepicker("option", "minDate", selectedDate);
                }
            });

            $("#EndDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("#StartDate").datepicker("option", "maxDate", selectedDate);
                }
            });

            $('#searchDate').on('click', function () {
                LoadLeavesGrid();
            });
        }

        function GetProjectManagerUsers() {

            var pmid = $("#pmId").val();
            if (pmid == '')
                pmid = 0;
            var emp = $("#userId");

            $.ajax
                ({
                    url: domain + 'Leave/GetEmployeesByPM',
                    type: 'POST',
                    datatype: 'application/json',
                    contentType: 'application/json',
                    data: JSON.stringify({
                        pmid: pmid
                    }),
                    success: function (result) {
                        emp.empty().append('<option value="">All Employee</option>');
                        $.each(result, function () {
                            emp.append($("<option></option>").val(this['value']).html(this['text']));
                        });
                    },
                    error: function (ex) {
                        alert("Whooaaa! Something went wrong.." + ex);
                    },
                });
        }

        function LoadLeavesGrid() {

            var data = { user: $('#userId').val(), startDate: $('#StartDate').val(), endDate: $('#EndDate').val() };
            $(".divoverlay").removeClass('hide');
            var manageUserGrid = new Global.GridHelper('#grid-employeestatusreport', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bSort": false,
                ajax: {
                    url: domain + 'report/getemployeestatusreport',
                    type: 'POST',
                    data: data
                },
                "columnDefs": [
                    { "width": "10%", "targets": 0 },
                    { "width": "20%", "targets": 1 },
                    { "width": "20%", "targets": 2 },
                    { "width": "50%", "targets": 3 },
                ],
                columns: [
                    { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
                    { name: 'date', data: 'date', title: "Date", sortable: true, searchable: false, visible: true },
                    { name: 'loginTime', data: 'loginTime', title: "Login Time", sortable: false, searchable: false, visible: true },
                    { name: 'status', data: 'status', title: "Status", sortable: false, searchable: false },
                ],
                "fnDrawCallback": function (oSettings) {
                },
                "fnInitComplete": function (oSettings, json) {
                    $(".divoverlay").addClass('hide');
                    var html = '<div class="col-md-6 pull-right text-right">';
                    if (json.totalpaiddays != undefined) {
                        html = html + '<label style="padding: 10px 0; margin-bottom:0; font-weight:bold;">Total Paid Days: ' + json.totalpaiddays + '</label>'
                    }
                    if (json.totalfreedays != undefined) {
                        html = html + '<label style="padding: 10px 0; margin-bottom:0; font-weight:bold;">, Total Free Days: ' + json.totalfreedays + '</label>'
                    }
                    html = html + '</div>';
                    $('.dataTables_wrapper > div.row:first > div:last').html(html);
                }
            });
        }

        $this.init = function () {
            LoadLeavesGrid();
            initializeForm();
        }
    }


    $(function () {
        var self = new EmployeeStatusReport();
        self.init();
    });

}(jQuery));
