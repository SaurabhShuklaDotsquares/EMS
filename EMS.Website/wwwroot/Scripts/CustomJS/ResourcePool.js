(function () {
    function Project() {
        $this = this;

        $('#ddl_Pm').change(function () {
            LoadProjectGrid();
        })

        $('#ddl_department').change(function () {
            LoadProjectGrid();
        })

        $('#btn_export').on('click', function () {
            var deptID = 0;
            var pmID = 0;
            if ($('#ddl_Pm').val() != undefined)
                pmID = $('#ddl_Pm').val();

            if ($('#ddl_department').val() != undefined && $('#ddl_department').val() != '')
                deptID = $('#ddl_department').val();


            var data1 = { pmId: pmID, deptId: deptID };
            $.get(domain + "/ResourcePool/ExportToExcel", data1, function (data) {
            });
          
        })

        

        function LoadProjectGrid() {
            var deptID = 0;
            var pmID = 0;
            if ($('#ddl_Pm').val() != undefined)
                pmID = $('#ddl_Pm').val();

            if ($('#ddl_department').val() != undefined && $('#ddl_department').val() != '')
                deptID = $('#ddl_department').val();


            var data1 = { pmId: pmID, deptId: deptID };
            var ProjectGrid = new Global.GridHelper('#grid-manageProject', {
                serverSide: true,
                destroy: true,
                "paging": false,
                "info": false,
                "bFilter": false,
                ajax:
                    {
                        url: domain + "ResourcePool/Index",
                        type: "Post",
                        data: data1

                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "0%", "targets": 0 },
                    { "width": "2%", "targets": 1 },
                    { "width": "25%", "targets": 2 },
                     { "width": "5%", "targets": 3 },
                    { "width": "28%", "targets": 4 },
                    { "width": "15%", "targets": 5 },
                    { "width": "15%", "targets": 6 },
                    { "width": "10%", "targets": 7 }
                ],
                columns:
                    [
                        { name: "projectId", data: "projectId", title: "ProjectID", sortable: false, searchable: false, visible: false },
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                         { name: "developerName", data: "developerName", title: "Name", sortable: false, searchable: false, visible: true },
                          {
                              name: 'action', data: null, title: "", sortable: false, searchable: false, render: function (data, type, row, meta) {
                                  if (row.usersListSameProject != undefined && row.usersListSameProject != null && row.usersListSameProject != '')
                                  return "<a href='javascript:void(0)' class='tooltip help' title='" + row.usersListSameProject + "'><img src=" + domain + "images/helpicon.png widht='15px' height ='15px' /></a>"
                                  else
                                      return '';
                              }
                          },
                        { name: "projectName", data: "projectName", title: "Project Name", sortable: false, searchable: false, visible: true },
                        { name: "modelName", data: "modelName", title: "Bucket Model", sortable: false, searchable: false, visible: true },
                        { name: "departmentName", data: "departmentName", title: "Department Name", sortable: false, searchable: false, visible: true },
                        { name: "status", data: "status", title: "Status", sortable: false, searchable: false, visible: true }
                    ],
                "fnDrawCallback": function (oSettings) {
                    if (oSettings.fnRecordsDisplay() > 0) {
                        $("#btn_export").show();
                        var _href = $(".export-btn").data("href");
                        $(".export-btn").attr("href", _href + "?pmId=" + pmID + "&deptId=" + deptID);
                    }
                    else
                    {
                        $("#btn_export").hide();
                    }
                },
                "createdRow": function (row, data, index) {
                    if(data.status=="Free")
                        $(row).addClass('freeuser');
                    else if(data.status=="Away")
                        $(row).addClass('away');
                    else if(data.status=="Not Logged-In")
                        $(row).addClass('nologin');
                    else if(data.status=="Working")
                        $(row).addClass('working');
                }
            }) 
            return ProjectGrid;
        }

        $this.init = function () {
            LoadProjectGrid();
        }
    }
    $(function () {
        var self = new Project();
        self.init();
    })

}(jQuery))