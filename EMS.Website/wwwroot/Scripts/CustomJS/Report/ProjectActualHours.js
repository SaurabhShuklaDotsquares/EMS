(function ($) {
    function index() {
        var $this = this, grid;

        function InitializeEvents() {
            $(".chosen").chosen();
            $('#DateFrom').datepicker({
                dateFormat: "dd/mm/yy",
                //maxDate: $("#DateTo").val(),
                changeMonth: true,
                changeYear: true
                //onSelect: function (selectedDate) {
                //    $("#DateTo").datepicker("option", "minDate", selectedDate);
                //}
            });
            $('#DateTo').datepicker({
                dateFormat: "dd/mm/yy",
                //maxDate: new Date(),
                //minDate: $("#DateFrom").val(),
                changeMonth: true,
                changeYear: true
                //onSelect: function (selectedDate) {
                //    $("#DateFrom").datepicker("option", "maxDate", selectedDate);
                //}
            });
            $('#btnGo').off('click').on('click', function () {
                //grid.ajax.reload();
                //loadtaskgrid();
                $this.loadtaskgrid();
            });
        }
        //function InitialVisibility() {
        //    if ($('#WorkingHourTypeId').val() == "") {
        //        $('.div-Department').hide();
        //        $('.div-Employee').hide();
        //    }
        //}

        $this.loadtaskgrid = function () {
            $('.divoverlay').removeClass('hide');
            var data = {
                DateFrom: $('#DateFrom').val(),
                DateTo: $('#DateTo').val(),
                Uid: $('#Uid').val(),
                ProjectId: $('#ProjectId').val(),
                ProjectStatus: $('#ProjectStatus').val()
            };
            grid = new Global.GridHelper('#grid-ProjectActualHour-table', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                //"bLengthChange": false,
                //"ordering": false,
                "bFilter": false,
                //"bAutoWidth": false,
                //"dom": 'Bfrtip',

                "bSort": true,

                buttons: [
                    'excel'
                ],
                "bPaginate": true,
                ajax:
                {
                    url: domain + "report/GetProjectActualHourReport",
                    type: "POST",
                    data: data
                },


                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "8%", "targets": 1 },
                    { "width": "8%", "targets": 2 },
                    { "width": "8%", "targets": 3 },
                    { "width": "8%", "targets": 4 }

                ],
                columns:
                    [
                        {
                            name: "rowIndex", data: "rowIndex", sortable: false, searchable: false, visible: false,
                            render: function (data, type, row, meta) {
                                return row.rowIndex;
                            }
                        },
                        {
                            name: "ProjectName", data: "projectName", title: "Project Name", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var p = "";
                                if (row.projectName) {
                                    //p += "<strong>" + row.projectName + "</strong>";
                                    p += "<strong><a class='ablue' target='_blank' style='text-decoration:underline;cursor:pointer;' href='" + domain + "report/ProjectActualHourDetails/" + row.projectId + "'>" + row.projectName + "</a></strong>";
                                }
                                return p;
                            }
                        },
                        //{ name: "MemberName", data: "memberName", title: "User Name", sortable: false, searchable: false, visible: true },
                        { name: "PlanHour", data: "planHour", title: "Invoice Hours", sortable: false, searchable: false, visible: true },
                        { name: "ActualHours", data: "actualHours", title: "Actual Hours", sortable: false, searchable: false, visible: true },
                        {
                            name: "Variance", data: "variance", title: "Variance Hours", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                var p = '<div>' + row.variance;
                                if (row.varianceP == "N/A") {
                                    return '<label class="label converted" style="color:black!important;">' + row.varianceP + '</label>';
                                }
                                else if (row.varianceP > 20 && row.variance > 0) {
                                    p += '  <label class="label label-danger converted">(' + row.varianceP + ' %)</label>';
                                } else {
                                    p += '  <label class="label converted" style="color:black!important;">(' + row.varianceP + ' %)</label>';
                                }
                                p += '</div>';
                                return p;
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
                },
                "fnInitComplete": function (oSettings, json) {
                    $(".divoverlay").addClass('hide');
                }
            });
        };



        $this.init = function () {
            InitializeEvents();
            //loadtaskgrid();
            //InitialVisibility();
        };
    }
    $(function () {
        var self = new index();
        self.init();
        self.loadtaskgrid();
    });
}(jQuery));