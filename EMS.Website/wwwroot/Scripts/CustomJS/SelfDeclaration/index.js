(function ($) {
    function index() {
        var $this = this, grid;

        function intializeGrid() {

            $('.divoverlay').removeClass('hide');

            $this.bindEstimateGrid = function myfunction() {
                var param = { txtSearch: $('#txtSearch').val(), drpInJpr: $('#drpInJpr').val(), drpHasDiseaseSymptoms: $('#drpHasDiseaseSymptoms').val() };
                grid = new Global.GridHelper('#grid-self-declaration-table', {
                    serverSide: true,
                    destroy: true,
                    ordering: false,
                    //searchDelay: 800,
                    "pageLength": 25,
                    "bFilter": true,
                    "bAutoWidth": false,
                    "bLengthChange": false,
                    "searching": false,
                    //"language": {
                    //    searchPlaceholder: "Search By Name"
                    //},
                    "dom": '<"pull-left"f><"pull-right"l>tip',
                    ajax:
                    {
                        url: domain + "selfdeclaration/index",
                        type: "POST",
                        data: param
                    },

                    "columnDefs": [
                        { "width": "5%", "targets": 0 },
                        { "width": "30%", "targets": 1 },
                        { "width": "10%", "targets": 2 },
                        { "width": "20%", "targets": 3 },
                        { "width": "20%", "targets": 4 },
                        { "width": "10%", "targets": 5 },
                        { "width": "5%", "targets": 6 },


                    ],
                    columns:
                        [
                            { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                            { name: "Name", data: "name", title: "Employee Name", sortable: false, searchable: false, visible: true },
                            { name: "mobileNumber", data: "mobileNumber", title: "Mobile No.", sortable: false, searchable: false, visible: true },
                            {
                                name: "recentlyInJaipur", data: "recentlyInJaipur", title: "Recently in Jaipur", sortable: false, searchable: false, visible: true,
                                render: function (data, type, full, meta) {
                                    if (data == "Yes") {
                                        //<span class="label label-success">data</span>
                                        return '<span class="label label-success">' + data + '</span>';
                                    }
                                    else {
                                        return '<span class="label label-danger">' + data + '</span>';
                                    }

                                    return '<a class="trans-btn"  href="' + domain + 'selfdeclaration/add/' + full.userId + '" "><i class="glyphicon glyphicon-eye-open" style="color:black"></i></a>';
                                    return '<a class="trans-btn"  href="' + domain + 'selfdeclaration/add/' + full.userId + '" "><i class="glyphicon glyphicon-eye-open" style="color:black"></i></a>';
                                }
                            },

                            {
                                name: "hasDiseaseSymptoms", data: "hasDiseaseSymptoms", title: "Has Disease Symptoms", sortable: false, searchable: false, visible: true,
                                render: function (data, type, full, meta) {
                                    if (data == "No") {
                                        //<span class="label label-success">data</span>
                                        return '<span class="label label-success">' + data + '</span>';
                                    }
                                    else {
                                        return '<span class="label label-danger">' + data + '</span>';
                                    }

                                    return '<a class="trans-btn"  href="' + domain + 'selfdeclaration/add/' + full.userId + '" "><i class="glyphicon glyphicon-eye-open" style="color:black"></i></a>';
                                    return '<a class="trans-btn"  href="' + domain + 'selfdeclaration/add/' + full.userId + '" "><i class="glyphicon glyphicon-eye-open" style="color:black"></i></a>';
                                }
                            },
                            { name: "AddedDate", data: "addedDate", title: "Added Date", sortable: false, searchable: false, visible: true },
                            {
                                name: 'action', data: null, title: "Action", className: "text-center", sortable: false, searchable: false, render: function (data, type, full, meta) {
                                    return '<a class="trans-btn"  href="' + domain + 'selfdeclaration/add/?uid=' + full.userId + '" "><i class="glyphicon glyphicon-eye-open" style="color:black"></i></a>';
                                }
                            },
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
                    }
                });

            };


            $(document).on("click", '#btnSearch', function () {
                $this.bindEstimateGrid();
            });

            //$(document).on("change", '#drpInJpr', function myfunction() {
            //    $this.getSelfDeclarationList();
            //});

            //$(document).on("change", '#drpHasDiseaseSymptoms', function myfunction() {
            //    $this.getSelfDeclarationList();
            //});

            //$this.getSelfDeclarationList = function () {
            //        var param = { txtSearch: $('#txtSearch').val(), drpInJpr: $('#drpInJpr').val(), drpHasDiseaseSymptoms: $('#drpHasDiseaseSymptoms').val() };

            //    alert(param);
            //    return;
            //        $.ajax({
            //            url: domain + "estimate/getleadsummary",
            //            type: 'POST',
            //            datatype: 'application/json',
            //            contentType: 'application/json',
            //            data: JSON.stringify(param),
            //            success: function (json) {
            //            },
            //            error: function (ex) {
            //                alert("Whooaaa! Something went wrong.." + ex);
            //            }
            //        });
            //};


        }

        $this.init = function () {
            intializeGrid();
            $this.bindEstimateGrid();
        };
    }

    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));