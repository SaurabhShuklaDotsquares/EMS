/*global jQuery, Global,secureDomain */

(function () {
    function ManageForecasting() {
        $this = this;
        var startDate = '', endDate = '';
        $('select[name=status] option:eq(1)').attr('selected', 'selected');
        function initializeForm() {
        }

        function LoadForecastingGrid() {
            $('.divoverlay').addClass('hide');
            startDate = $('#StartDate').val();
            endDate = $('#EndDate').val();
            var e = document.getElementById("status");
            var status = e.options[e.selectedIndex].value;
            var data = { status: status, startDate: startDate, endDate: endDate };

            var manageForecastingGrid = new Global.GridHelper('#grid-Forecasting', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bSort": false,
                ajax: {
                    url: domain + 'Forecasting/GetForecastingList',
                    type: 'POST',
                    data: data
                },
                order: [[0, 'desc']],
                "columnDefs": [
                 //{ "width": "0%", "targets": 0 },
                 //{ "width": "5%", "targets": 1 },
                ],
                columns: [
                    { name: 'rowId', data: 'rowId', title: "#", sortable: false, searchable: false },
                     //{ name: 'id', data: 'id', title: "id", sortable: false, searchable: false, visible: false }, // commented on 23-11-2021
                    { name: 'action', data: null, id: "colorbox", title: "Project / Client", className: "", sortable: false, searchable: false, render: function (data, type, full, meta) {
                        var html = "";
                        
                        if (full.isUserRolePM) {
                            html =  '<a class="inline clicklead cboxElement cls_' + full.chasingStatus + '" id="' + full.chasingStatus + "_" + full.id + '"  href="javascript:void(0)">' + full.title + '</a>';
                        }
                        else {
                            html = '<a class="inline clicklead cls_' + full.chasingStatus + '" id="' + full.chasingStatus + "_" + full.id + '"  href="javascript:void(0)">' + full.title + '</a>';
                        }

                        return html;
                      }
                    },
                    {
                        //console.log(full.infoColor);
                        name: 'crmOrLeadId', data: 'crmOrLeadId', title: "CRM ID / LEAD ID", sortable: false, searchable: false, render: function (data, type, full, meta) {
                            return full.crmOrLeadId + '<a href="javascript:void(0);" data-toggle="tooltip" data-placement="top" title="' + full.projectDescription + '" style="margin-left: 10px;"><i class="glyphicon glyphicon-info-sign" style="'+ full.infoColor +'"></i></a>';
                        }
                    },
                    { name: 'country', data: 'country', title: "Country", sortable: false, searchable: false },
                    { name: 'addedPerson', data: 'addedPerson', title: "Owner", sortable: false, searchable: false },
                    { name: 'reviewed', data: 'reviewed', title: "Reviewer", sortable: false, searchable: false },
                    { name: 'noOfDeveloper', data: 'noOfDeveloper', title: "Dev.", sortable: false, searchable: false },
                    { name: 'addedDate', data: 'addedDate', title: "Added On", sortable: false, searchable: false },//new column added 23-11-2021
                    { name: 'tentiveDate', data: 'tentiveDate', title: "Tentive Date", sortable: false, searchable: false },
                    { name: 'chasingType', data: 'chasingType', title: "Forecasting Type", sortable: false, searchable: false },
                    { name: 'chasingStatus', data: 'chasingStatus', title: "Status", sortable: false, searchable: false },
                    {
                        name: 'action', data: null, title: "Action", className: "text-center", sortable: false, searchable: false, render: function (data, type, full, meta) {
                            if (full.chasingStatus == "Pending") {
                                return '<a class="trans-btn btnEdit" id="' + full.id + '"  href="javascript:void(0)"><i class="fa fa-edit"></i></a><a class="trans-btn btnDelete" id="' + full.id + '" href="javascript:void(0)" "><i class="fa fa-trash"></i></a>';
                            }
                            else {
                                return '';
                            }
                        }
                    }
                ],

                "fnInitComplete": function (oSettings, json) {
                    if (startDate == undefined) {
                        startDate = '';
                    }
                    if (endDate == undefined) {
                        endDate = '';
                    }
                    var html = '<input type="button" id="searchDate" class="btn btn-custom pull-right btnlistview" value="Search" />';
                    html = html + '<div class="col-md-3 pull-right"><input type="text" name="EndDate" autocomplete="off" pattern="/^([0-9]{2})\/([0-9]{2})\/([0-9]{4})$/" id="EndDate" class="form-control searchfilter" placeholder="End Date" value="' + endDate + '" /></div>';
                    html = html + '<div class="col-md-1 pull-right text-right"><label style="padding: 0px 0; margin-bottom:0; font-weight:bold; margin-top: 10px;">To:</label></div>'
                    html = html + '<div class="col-md-3 pull-right"><input type="text" name="StartDate" pattern="/^([0-9]{2})\/([0-9]{2})\/([0-9]{4})$/" id="StartDate" autocomplete="off" class="form-control searchfilter" placeholder="Start Date" value="' + startDate + '" /></div>';
                    html = html + '<div class="pull-right"><label style="padding: 0px 0; margin-bottom:0; font-weight:bold; margin-top: 10px;">Tentive date from: ' + '</label></div>'
                    $('.dataTables_wrapper > div.row:first > div:last').html(html);
                    BindDatePickers();
                    BindRowColor();
                    //console.log(json);
                    $('[data-toggle="tooltip"]').tooltip()
                }
            });
            return manageForecastingGrid;
        }

        function BindRowColor() {
            $('a.cls_Pending').closest('tr').css({ "background": "#ffff80" });
            $('a.cls_Converted').closest('tr').css({ "background": "#9dff9d" });
        }

        function BindDatePickers() {
            $("#StartDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
            });
            $("#EndDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
            });

            $('#searchDate').on('click', function () {
                LoadForecastingGrid();
            });

            $("#status").change(function () {
                LoadForecastingGrid();
            });

            $('.btnDelete').click(function () {
                var val = this.id;
                if (confirm('Are you sure want to delete?')) {
                    $.ajax({
                        type: "POST",
                        url: domain + 'Forecasting/DeleteRecord',
                        data: { 'id': val },
                        success: function (data) {
                            if (data.status) {
                                //Refresh
                                LoadForecastingGrid();
                            }
                        },
                        error: function () {
                            alert('Failed');
                        }
                    })
                }
            })

            $('.btnEdit').click(function () {
                $('.divoverlay').addClass('show');
                var val = this.id;
                $.ajax({
                    type: "GET",
                    url: domain + 'Forecasting/_EditManageForecasting',
                    data: { 'id': val },
                    success: function (result) {
                        var divBody = $("#divBody");
                        divBody.html("");
                        divBody.append(result);
                        $('.divoverlay').removeClass('show');
                        $('.divoverlay').addClass('hide');
                        $('#myModalSave').modal();
                    },
                    error: function (error) {
                        alert('failed');
                    }
                })
            })


            $('.cboxElement').click(function () {
                var val = this.id;
                $('#seletedId').val(val.split("_")[1]);
                if (val.split("_")[0] == "Converted") {
                    $("#radio_converted").prop("checked", true);
                }
                else {
                    $("#radio_pending").prop("checked", true);
                }
                $('#myModalUpdateStatus').modal();
            })
        }

        $('#btnaddnew').click(function () {
            $('.divoverlay').addClass('show');
            $.ajax({
                type: "GET",
                url: domain + 'Forecasting/_ManageForecasting',
                success: function (result) {
                    var divBody = $("#divBody");
                    divBody.html("");
                    divBody.append(result);
                    $('.divoverlay').removeClass('show');
                    $('.divoverlay').addClass('hide');
                    $('#myModalSave').modal();
                },
                error: function (error) {
                    alert('failed');
                }
            })
        })

        $('#btnSave').click(function () {
            var status = $('input[name=status]:checked', '#myForm').val();
            var id = $('#seletedId').val();
            var data = { id: id, status: status };
            $.ajax({
                type: "POST",
                url: domain + 'Forecasting/ChangeStatus',
                data: data,
                success: function (data) {
                    if (data.status) {
                        //Refresh 
                        LoadForecastingGrid();
                        $('#myModalUpdateStatus').modal('hide');
                    }
                },
                error: function () {
                    alert('Failed');
                }
            })
            // call function for submit data to the server
        })


        $this.init = function () {
            initializeForm();
            LoadForecastingGrid();
        }
    }

    $(function () {
        var self = new ManageForecasting;
        self.init();
    });
}(jQuery));
