(function () {
    function ManageEstimateHour() {
        var $this = this, localeOpts;

        $(document).on("click", '#btnDeleteAll', function () {
            var strKeyIds = '';
            $(".chkDelete:checked").each(function () {
                strKeyIds += (strKeyIds == "" ? this.value : "," + this.value);
            });
            if (strKeyIds == '') {
                alert("Please select at least one library.");
                return;
            }
            if (confirm("Do you want to delete selected libraries?")) {
                $.ajax({
                    url: domain + "librarymanagement/DeleteLibrary",
                    type: 'POST',
                    datatype: 'application/json',
                    data: { strKeyIds: strKeyIds },
                    success: function (result) {
                        if (result.isSuccess) {
                            window.location.href = result.redirectUrl;
                        }
                    },
                    error: function (ex) {
                        alert("Whooaaa! Something went wrong.." + ex);
                    }
                });
            }

        });
        $(document).on("change", '#TechnologyId,#IndustryId', function () {
            var technoId = $('#TechnologyId').val();
            var industryId = $('#IndustryId').val();
            $.ajax({
                url: domain + "EstimateHour/fetchDependentFileName",
                type: 'POST',
                datatype: 'application/json',
                data: { technoId: technoId, industryId:industryId },
                success: function (data) {
                    var result = JSON.parse(data);
                    //var ddlFile = $("#FileName");
                    $("#FileName").empty();
                    $("#FileName").append("<option value='" + "" + "'>" + "-Select-" + "</option>");

                    $.each(result.data, function (index, item) {
                        $("#FileName").append("<option value='" + item.Text + "'>" + item.Text + "</option>");
                    });
                        //if (result.isSuccess) {
                    //    window.location.href = result.redirectUrl;
                    //}

                },
                error: function (ex) {
                    alert("Whooaaa! Something went wrong.." + ex);
                }
            });
        });
        function getfilter(d) {
            var dateRange = $('#EstimateDateRange').val();
            var dateFrom = '', dateTo = '';

            if (dateRange && dateRange != '') {
                dateFrom = dateRange.split(localeOpts.separator)[0];
                dateTo = dateRange.split(localeOpts.separator)[1];
            }
            d = d || {};
            d.TextSearch = $("#txtSearch").val();
            d.DateFrom = dateFrom;
            d.DateTo = dateTo;
            d.TechnologyId = $("#TechnologyId").val();
            d.IndustryId = $("#IndustryId").val();
            d.FileId = $("#FileName").val();
            //d.BA = $("#Uid_BA").val();
            //d.TL = $("#Uid_TL").val();
            //d.PMUid = pmUid || $("#PMUid").val() || null;
            //d.CRMStatusId = $("#CRMStatusId").val();
            //d.filterType = closureFilterType || null;
            //d.Country = $('#Country').val();
            return d;
        }

        function LoadEstimateHourGrid() {
            this.onChangeAllCheckbox = function () {
                $(".chkDelete").prop("checked", $('.deleteChkBoxAll').prop("checked"));
            };
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-EstimateHourlist', {
                serverSide: true,
                destroy: true,
                searchDelay: 800,
                "pageLength": 25,
                "bFilter": false,
                "bAutoWidth": false,
                "language": {
                    searchPlaceholder: "Search By Name"
                },
                ajax:
                {
                    url: domain + "EstimateHour/ManageEstimateHourList",
                    type: "POST",
                    data: function (d) {
                        getfilter(d);
                    }
                },
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "5%", "targets": 1 },
                    //{ "width": "5%", "targets": 2 },
                    { "width": "20%", "targets": 2 },
                    { "width": "25%", "targets": 3 },
                    { "width": "20%", "targets": 4 },
                    { "width": "5%", "targets": 5 },
                    { "width": "5%", "targets": 6 },
                    { "width": "3%", "targets": 7 },
                    { "width": "3%", "targets": 8 },
                    { "width": "3%", "targets": 9 }
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "Industry/ Technology", data: "industryAndTechnology", title: "Industry/</br>Technology", sortable: false, searchable: false, visible: true },
                        //{ name: "Technology", data: "technology", title: "Technology", sortable: false, searchable: false, visible: true },
                        { name: "Requirement Name", data: "requirementName", title: "Requirement Name", sortable: false, searchable: false, visible: true },
                        { name: "RequirementDescription", data: "description", title: "Requirement Description", sortable: false, searchable: false, visible: true },
                        { name: "FileName", data: "fileName", title: "File Name", sortable: false, searchable: false, visible: true },
                        { name: "CrmId", data: "crmId", title: "CrmId", sortable: false, searchable: false, visible: true },
                        { name: "EstimateHours", data: "estimateHours", title: "Estimate Hours", sortable: false, searchable: false, visible: true },
                        //{ name: "EstimateHours", data: "estimateHours", title: "Estimate Hours", sortable: false, searchable: false, visible: false },
                        { name: "IsFreeBie", data: "freebie", title: "IsFreeBie", sortable: false, searchable: false, visible: false },
                        //{ name: "CreatedDate", data: "createdDate", title: "Created Date", sortable: false, searchable: false, visible: true },
                        { name: "Conversion Date", data: "conversionDate", title: "Date Of Conversion", sortable: false, searchable: false, visible: true },
                        { name: "TL/BA", data: "tLead_BA", title: "TL/</br>BA", sortable: false, searchable: false, visible: true },
                        //{ name: "BA", data: "bA", title: "BA", sortable: false, searchable: false, visible: true },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '<center>';
                                actionButtons += $("<a/>", {
                                    id: "addEdit",
                                    class: "",
                                    href: domain + "EstimateHour/AddEdit/" + row.id,
                                    html: $("<i/>", {
                                        class: "fa fa-pencil",
                                        style: "color:black"
                                    }).get(0).outerHTML + "&nbsp;",
                                }).get(0).outerHTML + "&nbsp; ";

                                actionButtons += $("<a/>", {
                                    id: "lnkDownlaod",
                                    class: "",
                                    href: domain + "EstimateHour/Download/" + row.id,
                                    html: $("<i/>", {
                                        class: "lnkDownload",
                                        style: "color:blue;text-decoration: underline;",
                                        html:"Download"
                                    }).get(0).outerHTML + "&nbsp;",
                                }).get(0).outerHTML + "&nbsp; ";
                                    //<a class="lnkDownload" href="javascript:void(0);" data-id="@fileDto.Id">
                                    //<img src="@tempImage" alt="" class=""> @fileDto.FileShortName
                                    //    </a>
                                return actionButtons;
                            }
                        },
                        //{
                        //    name: "Deelete", data: null, title: "Delete All<input type='checkbox' class='deleteChkBoxAll' onchange='onChangeAllCheckbox();' name='deleteChkBox_All'  />", sortable: false, searchable: false,
                        //    render: function (data, type, row, meta) {
                        //        return '<center><input type="checkbox" class="chkDelete" name="deleteChkBox_' + row.keyId + '" value=' + row.keyId + ' /></center>';
                        //    }
                        //},

                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    if (aData.isSubmitted) {
                        $('td', nRow).css({ 'background-color': '#f1f1f1', 'color': 'black' });
                    }
                },
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    $('.pagination .active a').css('background-color', '#e18f00');
                    $('.pagination .active a').css('border-color', '#e18f00');
                },
                "fnInitComplete": function (oSettings, json) {
                    var html = '<div class="pull-right"><h4>Total Estimated Time: ' + json.totalEstimateHour + '</h4></div>';
                    $('.dataTables_wrapper > div.row:first > div:last').html(html);
                }
            });
        }

        function InitializeControl() {
            
            var start = moment().subtract(29, 'days');
            var end = moment();
            localeOpts = {
                format: "DD/MM/YYYY",
                separator: " to "
            };

            function rangeChangeCB(start, end) {
                $('#EstimateDateRange').val(start.format(localeOpts.format) + localeOpts.separator + end.format(localeOpts.format));
            }

            $('#EstimateDateRange').daterangepicker({
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

            rangeChangeCB(start, end);
            
            $("#btnSearch").on("click", function () {
               // pmUid = null;
                //closureFilterType = null;
                LoadEstimateHourGrid();
                
            });

            $("#btnReset").on("click", function () {
                $("#txtSearch").val('');
                //$("#chaseStatus").val('');
                $("#EstimateDateRange").val(start.format(localeOpts.format) + localeOpts.separator + end.format(localeOpts.format));
                $("#TechnologyId").val('');
                $("#IndustryId").val('');
                $("#FileName").val('')
                //$("#Uid_BA").val('');
                //$("#Uid_TL").val('');
                //$("#CRMStatusId").val('');
                //$("#ProjectStatus").val('');
                LoadEstimateHourGrid();
                
            });

            $("#txtSearch").on("keypress", function (e) {
                if (e.keyCode == 13) {
                    $("#btnSearch").click();
                    return false;
                }
            });

            $("#clrFilterDate").click(function () {
                $('#EstimateDateRange').val('');
            });

            $('.lnkDownload').off('click').on('click', function () {
                id = $(this).data("id");
                $.ajax({
                    url: "EstimateHour/Download/" + id,
                    type: "Post",
                    data: { id: id },
                    success: function (result) {
                        if (result.isSuccess) {
                            //window.location = '/LibraryManagement/Download/' + id;
                        }
                        else {
                            swal({
                                title: "Alert!",
                                text: result.message,
                                icon: "error",
                            });
                        }
                    }
                });
            });
        }

        $this.init = function () {
            InitializeControl();
            LoadEstimateHourGrid();
        }
    }
    $(function () {
        var self = new ManageEstimateHour;
        self.init();
    })
}(jQuery));