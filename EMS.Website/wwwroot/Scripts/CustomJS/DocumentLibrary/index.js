(function ($) {
    function Expenses() {
        var $this = this, grid;
        var isVisibleColumn = false;
        $(document).on('click', '#btn-Acticedeactive', function () {

            var status = $('#idStatus').val();

            $.ajax({
                url: domain + 'DocumentLibrary/ApprovedStatus?id=' + status,
                type: 'GET',
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
        //$('#IsApprover').on("change", function () {
        //    loadGrid();
        //});
        $("#btnSearch").on("click", function () {
            loadGrid();
        })

        $("#btnReset").on("click", function () {
            location.reload();
        })

        function loadGrid() {
            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-expenseList', {
                serverSide: true,
                destroy: true,
                "pageLength": 15,
                "bFilter": true,
                "bAutoWidth": false,
                "bLengthChange": false,
                "language": {
                    searchPlaceholder: "Search"
                },
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                {
                    url: domain + "DocumentLibrary/index",
                    type: "Post",
                    data: getGridFilters()
                },
                order: [[4, 'desc']],
                "columnDefs": [
                    { "width": "2%", "targets": 0, "className": "text-center", sortable: false },
                    { "width": "10%", "targets": 1, "className": "text-center", sortable: true },
                    { "width": "10%", "targets": 2, "className": "text-center", sortable: true },
                    { "width": "5%", "targets": 3, "className": "text-center", sortable: true },
                    { "width": "10%", "targets": 4, "className": "text-center", sortable: true },
                    { "width": "8%", "targets": 5, "className": "text-center", sortable: false },
                    { "width": "10%", "targets": 6, "className": "text-center", sortable: false },
                    { "width": "10%", "targets": 7, "className": "text-center", sortable: false },                  
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#" },
                        { name: "documentTitle", data: "documentTitle", title: "Document Title" },
                        { name: "documentType", data: "documentType", title: "Document Type" },
                        { name: "version", data: "version", title: "Version" },                        
                        { name: "CreateDate", data: "createDate", title: "Date" },
                        {
                            name: "Status", data: "status", title: "Status",
                            render: function (data, type, row, meta) {
                                var actionButtons = '<center>';
                                actionButtons += '<div class="chk-box dis-block clearfix">';
                                
                                if (row.isActive == true) {
                                    actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + row.id + '" checked/><span class="slider round"></span></label>';
                                }
                                else {
                                    actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + row.id + '" /><span class="slider round"></span></label>';
                                }
                                actionButtons += '<label for=isApproved"></label>'
                                actionButtons += '</div>&nbsp;';
                                return actionButtons;
                            }
                        },                                              
                        {
                            name: "Attachment", data: "receipt", title: "Download",
                            render: function (data, type, row, meta) {                                
                                return "<a class='ablue' href='" + domain + "DocumentLibrary/Download?id=" + row.id + "' target='_blank'><i class='fa fa-download'></i> Download</a>";
                            }
                        },                     
                        {
                            name: "Action", data: null, title: "Action",
                            render: function (data, type, row, meta) {
                                var actionButtons = '';
                                actionButtons += $("<a/>", {
                                    id: "addEdit",
                                    class: "btn btn-default btn-sm",
                                    href: domain + "DocumentLibrary/addedit?guid=" + row.id,                                    
                                    html: $("<i/>", {
                                        class: "fa fa-pencil",
                                        style: "color:black"
                                    }).get(0).outerHTML + "&nbsp; Edit",
                                }).get(0).outerHTML + "&nbsp; ";

                                actionButtons += $("<a/>", {
                                    id: "delete",
                                    class: "btn btn-default btn-sm",
                                    href: domain + "DocumentLibrary/DeleteRecord?id=" + row.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-add-expense",
                                    'data-backdrop': "static",
                                    html: $("<i/>", {
                                        class: "fa fa-trash",
                                        style: "color:black"
                                    }).get(0).outerHTML + "&nbsp; Delete",
                                }).get(0).outerHTML + "&nbsp; ";
                                return actionButtons;
                            }
                        },
                        
                    ],
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                    //$('.switchBox').on('change', function () {
                    //    var switchElement = this;
                    //    $.get(domain + 'DocumentLibrary/ApprovedStatus', {
                    //        id: this.value
                    //    });
                    //});
                    $('.switchBox').on("change", function () {
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

        

        function initialize() {


            var expenseGrid = $('#grid-expenseList');

        }

        function getGridFilters() {
            return {
                Status: $("#IsApprover").val()
            }
        }

        $this.init = function () {
            loadGrid();
            initialize();
        };
    }
    $(function () {
        var self = new Expenses();
        self.init();
    });
}(jQuery));