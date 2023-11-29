/*global jQuery, Global,secureDomain */
(function () {
    function ManageAttendance() {
        $this = this;       
        function initializeForm() {            

            $('#user').on('change', function () {
                LoadGrid();
            });
            
            $('.searchfilterpm').on('change', function () {
                GetProjectManagerUsers();
            });
        }

        function GetProjectManagerUsers() {

            var pmid = $("#pm").val();
            if (pmid == '')
                pmid = 0;
            var emp = $("#user");
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

        function LoadGrid() {
            var showInoutTime = displayinoutTime == 1;         
            $('.divoverlay').removeClass('hide');
            var columns = [
                    { name: 'date', data: 'date', title: "Date", sortable: false, searchable: false, visible: true },                    
                    {
                        name: 'status', data: 'status', title: "Status", sortable: false, searchable: false, "render": function (data, type, full, meta) {
                            if (data != null) {
                                if (data.toLowerCase() == "present")
                                    return "<span style='color:green; font-weight:bold;'>" + data + "</span>";
                                if (data.toLowerCase() == "half day")
                                    return "<span style='color:red; font-weight:bold;'>" + data + "</span>";
                                return "<span style='font-weight:bold;'>" + data + "</span>"

                            }
                            return "";
                        }
                    }
            ];
            if (displayinoutTime == 1)
            {
                columns = [
                    { name: 'date', data: 'date', title: "Date", sortable: false, searchable: false, visible: true },
                    {
                        name: 'intime', data: 'intime', title: "In Time", sortable: false, searchable: false, visible: true, "render": function (data) {
                            if (data != null && data != '') {
                                return "<span class='watch'>" + data + "</span>";
                            }
                            return "";
                        }
                    },
                    {
                        name: 'outtime', data: 'outtime', title: "Out Time", sortable: false, searchable: false, visible: true, "render": function (data) {
                            if (data != null && data != '') {
                                return "<span class='watch'>" + data + "</span>";
                            }
                            return "";
                        }
                    },
                    {
                        name: 'status', data: 'status', title: "Status", sortable: false, searchable: false, "render": function (data, type, full, meta) {
                            if (data != null) {
                                if (data.toLowerCase() == "present")
                                    return "<span style='color:green; font-weight:bold;'>" + data + "</span>";
                                if (data.toLowerCase() == "half day")
                                    return "<span style='color:red; font-weight:bold;'>" + data + "</span>";
                                return "<span style='font-weight:bold;'>" + data + "</span>"

                            }
                            return "";
                        }
                    }
                ];
            }

            var manageUserGrid = new Global.GridHelper('#grid-attendance', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bSort": false,
                "bInfo": false,
                ajax: {
                    url: domain + 'attendance/GetAttendance',
                    type: 'POST',
                    data: { "userId" : $('#user').val()}
                },                
                "columnDefs": [
               
                ],
                "bPaginate": false,
                columns: columns,
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                },
                "fnInitComplete": function (oSettings, json) {                
                    $('#grid-attendance_wrapper > div.row:first > div:last').remove();
                },
                "createdRow": function (row, data, dataIndex) {
                    $(row).addClass(data.color);
                }
            });
            return manageUserGrid;
        }

        $this.init = function () {
            initializeForm();
            LoadGrid();
        }
    }

    $(function () {
        var self = new ManageAttendance;
        self.init();
    });

}(jQuery));
