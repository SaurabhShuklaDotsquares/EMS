/*global jQuery, Global,secureDomain */
(function () {

    function EmployeeLoginReport() {
        $this = this;

        function initializeForm() {
            SearchLogin();
            $("#ddl_pm").val(0);

            $("#AttendanceDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                maxDate:0
            });

            $("#clrFilterDate").click(function () {
                $('#AttendanceDate').val('');
            });
        }

        function SearchLogin() {
            $('#searchLogin').on('click', function () {
                LoadLoginGrid();
            });
        }



        function LoadLoginGrid() {
            var PMId = 0;
            if ($("#ddl_pm") != undefined) {
                PMId = $("#ddl_pm").val();
            }

            var data = { UserId: $('#userId').val(), MonthYear: $("#MonthYear").val(), PMId: PMId };
            $('.divoverlay').removeClass('hide');
            $.ajax({
                url: domain + "report/EmployeeLoginReport",
                type: 'POST',
                data: data,
                success: function (result) {
                    $('.divoverlay').addClass('hide');
                    var tableString = "<table class='table-bordered text-align-center' style='width:100%;table-layout:fixed' >";
                    tableString += "<tr>";
                    tableString += "<th class='headerclass headerwidth'>Name</th>";
                    result[0].loginDetails.forEach(function (item) {
                        tableString += "<th class='headerclass'>" + item.day + "</th>";
                    });
                    tableString += "</tr>";
                    result.forEach(function (item) {
                        tableString += "<tr style='text-align:center'><td><b>" + item.name + "</b></br>" + item.email + "</br><i>(" + item.department + ")</i></td>";
                        item.loginDetails.forEach(function (logintime) {
                            tableString += "<td>" + logintime.logintime + "</td>";
                        });
                        tableString += "</tr>";
                    });

                    tableString += "</table>";
                    $('#divLoginReport').html(tableString);

                },
                error: function (ex) {

                }
            });
        }

        $this.init = function () {
            //LoadLoginGrid();
            initializeForm();
        }
    }


    $(function () {
        var self = new EmployeeLoginReport();
        self.init();
    });



}(jQuery));


function FillEmployee() {
    var PMid = $('#ddl_pm').val();
    $.ajax({
        url: '/Report/FillEmployee',
        type: "GET",
        dataType: "JSON",
        data: { PMid: PMid },
        success: function (Employee) {
            $("#userId").html("");
            $("#userId").append(
                $('<option></option>').val('').html("All Employee"));
            $.each(Employee, function (i, Employee) {
                $("#userId").append(
                    $('<option></option>').val(Employee.value).html(Employee.text));
            });
        }
    });
}

function ExportAttendanceReport() {
    var AttendanceDate = $("#AttendanceDate").val();
    
    if (AttendanceDate == "") {
        alert("Please select Attendance Date");
        return false;
    }

    var PMid = $('#ddl_pm').val();
    if (PMid == undefined) {
        PMid = 0;
    }

    var userId = $("#userId").val();
    window.location = '/Report/DownloadAttendanceReport?AttendanceDate=' + AttendanceDate + "&PMId=" + PMid + "&UserId=" + userId;

}