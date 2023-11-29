/*global jQuery, Global,secureDomain */

(function () {
    function ManageMedicalData() {
        $this = this;
        function initializeForm() {
            $('#search_relative_status').on('click', function () {
                $('label.error').remove();
                if ($('#relative_status').val() == '') {
                    if ($('#relative_status').val() == '') {
                        $('#relative_status').after('<label class="error">*required</label>');
                    }
                }
                else {
                    LoadGrid();
                }
            });

            $('#relative_status').on('change', function () {
                $('#relative_status + label.error').remove();
                if ($(this).val() == '') {
                    $('#relative_status').after('<label class="error">*required</label>');
                }
            });


            var start = moment().subtract(29, 'days');
            var end = moment();
            //var start = moment().subtract(29, 'days');
            //var end = moment();
            localeOpts = {
                format: "DD/MM/YYYY",
                separator: " to "
            };

            function rangeChangeCB(start, end) {
                $('#JoiningDateRange').val(start.format(localeOpts.format) + localeOpts.separator + end.format(localeOpts.format));
            }

            $('#JoiningDateRange').daterangepicker({
                "locale": localeOpts,
                startDate: start,
                endDate: end,
                autoUpdateInput: false,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                }
            }, rangeChangeCB);

           // rangeChangeCB(start, end);
            $("#JoiningDateRange").val('');

            $("#btnSearch").on("click", function () {
                LoadGrid();
            });

            $("#btnReset").on("click", function () {
                $("#txtSearch").val('');
                $("#JoiningDateRange").val('');
                $("#relative_status").val('All');
                $("#pmlist").val('');
              
                LoadGrid();
               
            });

            $("#txtSearch").on("keypress", function (e) {
                if (e.keyCode == 13) {
                    $("#btnSearch").click();
                    return false;
                }
            });

            $("#clrFilterDate").click(function () {
                $('#JoiningDateRange').val('');
            });


        }
        function InitForm() {
            var data = $(".export-btn").attr("href");
            $('#pmlist').on('change', function () {
                $(".export-btn").attr("href", data + "?pmid=" + $('#pmlist').val());
                LoadGrid();
            });
            $("#modal-view-medicaldata").on('loaded.bs.modal', function (e) {
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
        }

        function getfilter(d) {
            var dateRange = $('#JoiningDateRange').val();
            var dateFrom = '', dateTo = '';

            if (dateRange && dateRange != '') {
                dateFrom = dateRange.split(localeOpts.separator)[0];
                dateTo = dateRange.split(localeOpts.separator)[1];
            }
          
            d.txtEmployee =  $("#txtSearch").val() != null ? $("#txtSearch").val() : ''; 
            
            d.DateFrom = dateFrom;
            d.DateTo = dateTo;
            d.pmid = $('#pmlist').val();
            d.status = $('#relative_status').val();
            return d;
        }

        function LoadGrid() {
            $('.divoverlay').removeClass('hide');
            var manageUserGrid = new Global.GridHelper('#grid-medicaldata', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bSort": false,
                "bLengthChange": false,
                ajax: {
                    url: domain + 'medicaldata/Getlist',
                    type: 'POST',
                    data: function (d) {
                        getfilter(d);
                    }
                },
                //order: [[0, 'desc']],
                "columnDefs": [
                    { "width": "0%", "targets": 0 },
                    { "width": "1%", "targets": 1 },
                    { "width": "20%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    { "width": "10%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                    { "width": "10%", "targets": 7 },
                    { "width": "10%", "targets": 8 },
                ],
                columns: [
                    { name: 'userId', data: 'userId', title: 'ID', visible: false, sortable: false, searchable: false },
                    { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
                    { name: 'Name', data: 'name', title: "Name", sortable: false, searchable: true },
                    { name: 'RelativeData', data: 'relativeData', title: "Wish to Insurance?", sortable: true, searchable: true },
                    { name: 'EmployeeCode', data: 'employeeCode', title: "Emp Code", sortable: true, searchable: true },
                    { name: 'JoiningDate', data: 'joiningDate', title: "Joining Date", sortable: true, searchable: true },
                    { name: 'DOB', data: 'dob', title: "Date Of Birth", sortable: true, searchable: true },
                    { name: 'Designation', data: 'designation', title: "Designation", sortable: true, searchable: false },
                    { name: 'AddedDate', data: 'addedDate', title: "Added Date", sortable: true, searchable: false },
                    {
                        name: 'Action', data: null, title: "Action", sortable: false, searchable: false,

                        render: function (data, type, dataRow, meta) {
                            return $("<a/>", {
                                id: "viewDetail",
                                title: "View Detail",
                                href: domain + "medicaldata/view/" + dataRow.userId,
                                'data-toggle': "modal",
                                'data-target': "#modal-view-medicaldata",
                                'data-backdrop': "static",
                                html: $("<i/>", {
                                    class: "glyphicon glyphicon-eye-open",
                                    style: "color:black"
                                }),
                            }).get(0).outerHTML + "&nbsp; ";          
                            
                        }
                    }
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
            InitForm();
            LoadGrid();
            initializeForm();
        }
    }


    $(function () {
        var self = new ManageMedicalData;
        self.init();
    });

}(jQuery));
