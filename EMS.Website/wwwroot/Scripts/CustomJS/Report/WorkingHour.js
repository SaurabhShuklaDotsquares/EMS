(function ($) {
    function index() {
        var $this = this, grid;
        function loadWorkingHourgrid() {
            $('.divoverlay').removeClass('hide');
            grid = new Global.GridHelper('#grid-InvoiceWorkingHour', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                "ordering": false,
                info: false,
                "paging": false,
                ajax:
                {
                    url: domain + "Report/workinghour",
                    type: "Post",
                    data: { "month": $('#Month').val() },
                },


                "columnDefs": [
                    { "width": "25%", "targets": 0, "className": "rowCenterText" },
                    { "width": "25%", "targets": 1, "className": "rowCenterText" },
                    { "width": "25%", "targets": 2, "className": "rowCenterText" },
                    { "width": "25%", "targets": 3, "className": "rowCenterText" }

                ],
                columns:
                    [

                        {
                            name: "Developer", data: "developer", title: "Paid Developer Hour", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (data == "-") {
                                    return data;
                                }
                                else {
                                    //return '<a OnClick="loadTimesheetgrid('+selectedItems+')" class="ablue" style="text-decoration: underline;cursor:pointer !important;">' + data + '</a>';
                                    return '<a OnClick="loadTimesheetgrid([1,2])" class="ablue showtimesheet" style="text-decoration: underline;cursor:pointer !important;">' + data + '</a>';
                                }
                            }
                        },
                        {
                            name: "TL", data: "tl", title: "Paid Support Hour (TL)", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (data == "-") {
                                    return data;
                                }
                                else {
                                    return '<a OnClick="return loadTimesheetgrid([3])" class="ablue" style="text-decoration: underline;cursor:pointer !important;">' + data + '</a>';
                                }
                            }
                        },
                        {
                            name: "Designer", data: "designer", title: "Paid Support Hour (Designer)", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (data == "-") {
                                    return data;
                                }
                                else {
                                    return '<a OnClick="return loadTimesheetgrid([8])" class="ablue" style="text-decoration: underline;cursor:pointer !important;">' + data + '</a>';
                                }
                            }
                        },
                        {
                            name: "BA", data: "ba", title: "Paid Support Hour (BA)", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (data == "-") {
                                    return data;
                                }
                                else {
                                    return '<a OnClick="return loadTimesheetgrid([9])" class="ablue" style="text-decoration: underline;cursor:pointer !important;">' + data + '</a>';
                                }
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

            })
        }



        function initilizeControls() {
            $('#CustomSearch').off().on('click', function () {
                $('#TimesheetList').hide();
                loadWorkingHourgrid();

            });

        }

        $this.init = function () {
            //loadWorkingHourgrid();
            initilizeControls();
        };
    }



    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));

function loadTimesheetgrid(roleToFind) {

    if (roleToFind == null && roleToFind != "") {
        return false;
    }

    if (roleToFind.length == 2 && (roleToFind.includes(1) && roleToFind.includes(2))) {
        $('#TimesheetListHead').html('Paid Developer Hour');
    }
    else if (roleToFind.length == 1 && roleToFind[0] == 3) {
        $('#TimesheetListHead').html('Paid Support Hour (TL)');
    }
    else if (roleToFind.length == 1 && roleToFind[0] == 8) {
        $('#TimesheetListHead').html('Paid Support Hour (Designer)');
    }
    else if (roleToFind.length == 1 && roleToFind[0] == 9) {
        $('#TimesheetListHead').html('Paid Support Hour (BA)');
    }

    $('.divoverlay').removeClass('hide');
    grid = new Global.GridHelper('#grid-TimesheetList', {
        serverSide: true,
        destroy: true,
        "pageLength": 50,
        "bAutoWidth": false,
        "bLengthChange": false,
        "ordering": false,
        "bFilter": false,
        ajax:
        {
            url: domain + "Report/TimesheetsByRole",
            type: "Post",
            data: { "roleIds": roleToFind },

        },


        "columnDefs": [
            { "width": "15%", "targets": 0, "className": "rowCenterText" },
            { "width": "15%", "targets": 1, "className": "rowCenterText" },
            { "width": "20%", "targets": 2, "className": "rowCenterText" },
            { "width": "10%", "targets": 3, "className": "rowCenterText" },
            { "width": "15%", "targets": 4, "className": "rowCenterText" },
            { "width": "25%", "targets": 5, "className": "rowCenterText" },

        ],
        columns:
            [
                { name: "Date", data: "date", title: "Date", sortable: false, searchable: false, visible: true },
                { name: "Name", data: "name", title: "Name", sortable: false, searchable: false, visible: true },
                { name: "Project", data: "project", title: "Project", sortable: false, searchable: false, visible: true },
                { name: "VirtualDeveloper", data: "virtualDeveloper", title: "VirtualDeveloper", sortable: false, searchable: false, visible: true },
                { name: "Hours", data: "hours", title: "Hours", sortable: false, searchable: false, visible: true },
                { name: "Description", data: "description", title: "Description", sortable: false, searchable: false, visible: true },
            ],
        "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
        },
        "fnDrawCallback": function (oSettings) {
            $('#TimesheetList').show();
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
    });
}