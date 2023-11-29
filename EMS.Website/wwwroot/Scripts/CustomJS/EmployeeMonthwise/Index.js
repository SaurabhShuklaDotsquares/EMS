/*global jQuery, Global,secureDomain */

(function () {
    function EmployeeMonthwiseData() {
        $this = this;
        function initializeForm() {
            

            $("#btnSearch").on("click", function () {
                if ($('#MonthYear').val() != "Select Month") {
                    LoadGrid();
                }
                else {
                    swal({
                        title: "Required!",
                        text: "Please select month !",
                        icon: "error",
                    });
                }
            });
            $("#btnExport").on("click",async function () {
                if ($('#MonthYear').val() != "Select Month") {
                    //$.ajax({
                    var url= domain + 'EmployeeMonthwise/ExportReportToExcel?MonthYear=' + $('#MonthYear').val();
                    //    type: 'GET',
                    //    dataType: "jsonp",
                    //    success: function (data) {
                    //        //LoadGrid();
                    //    }
                    //});                    
                    
                        try {
                            const request = await fetch(url, {
                                method: 'GET'                                
                            });

                            const file = await request.blob();

                            let tempUrl = URL.createObjectURL(file);
                            const aTag = document.createElement("a");
                            aTag.href = tempUrl;
                            aTag.download = 'Monthly_Employees_Report_' + $('#MonthYear').val();
                            document.body.appendChild(aTag);
                            aTag.click();
                            URL.revokeObjectURL(tempUrl);
                            aTag.remove();
                        }
                        catch (e) {
                            console.log(`error occurred → ${e}`);
                        }
                }
                else {
                    swal({
                        title: "Required!",
                        text: "Please select month !",
                        icon: "error",
                    });
                }
            });

            $("#btnReset").on("click", function () {
                $("#MonthYear").val('');

                LoadGrid();

            });

            $("#txtSearch").on("keypress", function (e) {
                if (e.keyCode == 13) {
                    $("#btnSearch").click();
                    return false;
                }
            });


        }
        //function InitForm() {
        //    if ($('#MonthYear').val() != "Select") {
        //        var data = $("#btnExport").attr("href");
        //        $('#MonthYear').on('change', function () {

        //            $("#btnExport").attr("href", data + "?MonthYear=" + $('#MonthYear').val());
        //            LoadGrid();
        //        });
        //    }
        //    else {
        //        alert("");
        //    }
        //}

        function getfilter(d) {            
            d.dateFrom = $('#MonthYear').val();
            return d;
        }

        function LoadGrid() {
            $('.divoverlay').removeClass('hide');
            var manageUserGrid = new Global.GridHelper('#grid-employeedata', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bSort": false,
                "bLengthChange": false,
                "bPaginate": false,
                ajax: {
                    url: domain + 'EmployeeMonthwise/Getlist',
                    type: 'POST',
                    data: function (d) {
                        getfilter(d);
                    }
                },
                //order: [[0, 'desc']],
                "columnDefs": [
                    { "width": "0%", "targets": 0 },
                    { "width": "1%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "12%", "targets": 4 },
                    { "width": "12%", "targets": 5 },
                    { "width": "12%", "targets": 5 },
                    { "width": "18%", "targets": 6 },
                    { "width": "15%", "targets": 7 },
                    { "width": "15%", "targets": 8 },
                ],
                columns: [
                    { name: 'uid', data: 'uid', title: 'uid', visible: false, sortable: false, searchable: false },
                    { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
                    { name: 'IsActive', data: 'isActive', title: "IsActive", visible: false, sortable: false, searchable: true },
                    { name: 'JoinedDate', data: 'joinedDate', title: "Joined Date", sortable: true, searchable: true },
                    { name: 'RelievingDate', data: 'relievingDate', title: "Relieving Date", sortable: true, searchable: true },
                    { name: 'AttendenceId', data: 'attendenceId', title: "Attendence Id", sortable: true, searchable: true },
                    { name: 'Name', data: 'name', title: "Name", sortable: true, searchable: true },
                    { name: 'JobTitle', data: 'jobTitle', title: "Job Title", sortable: true, searchable: false },
                    { name: 'DepartmentName', data: 'departmentName', title: "Department", sortable: true, searchable: false },
                    { name: 'PMName', data: 'pmName', title: "PM Name", sortable: true, searchable: false },
                    
                ],
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                },
                "fnInitComplete": function (oSettings, json) {
                }
            });
            return manageUserGrid;
        }
        $this.init = function () {
            //InitForm();
            //LoadGrid();
            initializeForm();
        }
    }


    $(function () {
        var self = new EmployeeMonthwiseData;
        self.init();
    });

}(jQuery));
