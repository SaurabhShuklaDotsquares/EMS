(function ($) {
    function index() {
        var $this = this, grid;

        function intializeGrid() {
            var isAllow = isAllowed;
            var columns = [
                { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                { name: "departmentId", data: "departmentId", title: "Department", sortable: false, searchable: false, visible: true },
                { name: "documentName", data: "documentName", title: "Document Name", sortable: false, searchable: false, visible: false },

                {
                    name: "documentPath", data: "documentPath", title: "Document", sortable: false, searchable: false, visible: true,
                    render: function (data, type, row, meta) {
                        return '<a class="ablue" target="_blank" href="' + row.documentPath + '">' + row.documentName + '</a>'
                    }
                },
                { name: "createdDate", data: "createdDate", title: "Date", sortable: false, searchable: false, visible: true },
                
            ];
            if (isAllow == 1) {
                columns = [
                    { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                    { name: "departmentId", data: "departmentId", title: "Department", sortable: false, searchable: false, visible: true },
                    { name: "documentName", data: "documentName", title: "Document Name", sortable: false, searchable: false, visible: false },

                    {
                        name: "documentPath", data: "documentPath", title: "Document", sortable: false, searchable: false, visible: true,
                        render: function (data, type, row, meta) {
                            return '<a class="ablue" target="_blank" href="' + row.documentPath + '">' + row.documentName + '</a>'
                        }
                    },
                    { name: "createdDate", data: "createdDate", title: "Date", sortable: false, searchable: false, visible: true },
                    {
                        name: "isActive", data: "isActive", title: "Is Active", sortable: false, searchable: false,
                        render: function (data, type, row, meta) {                            
                                var actionButtons = '<center>';

                                actionButtons += '<div class="chk-box dis-block clearfix">';
                                if (row.isActive == true) {
                                    actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isActive" name="isActive" value="' + row.id + '" checked/><span class="slider round"></span></label>';
                                }
                                else {
                                    actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isActive" name="isActive" value="' + row.id + '" /><span class="slider round"></span></label>';
                                }
                                actionButtons += '<label for=isActive"></label>'
                                actionButtons += '</div>'
                                return actionButtons;                           
                        }
                    },
                    {
                        name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                        render: function (data, type, dataRow, meta) {                            
                                var actionButtons = $("<a/>", {
                                    id: "editevent",
                                    title: "edit",
                                    href: domain + "Document/add/" + data.id,
                                    html: $("<i/>", {
                                        class: "glyphicon glyphicon-edit",
                                        style: "color:black"
                                    }),
                                }).get(0).outerHTML + "&nbsp; ";

                                return actionButtons;                            
                        }
                    },
                ];
            }
            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-document-table', {
                serverSide: true,
                destroy: true,
                searchDelay: 800,
                ordering: false,
                "pageLength": 50,
                "bFilter": true,
                "bAutoWidth": false,
                "bLengthChange": false,
                "language": {
                    searchPlaceholder: "Search By Document"
                },
                "dom": '<"pull-left"f><"pull-right"l>tip',
                ajax:
                {
                    url: domain + "Document/index",
                    type: "Post"
                },
                "columnDefs": [

                ],
                columns: columns,
                //"columnDefs": [
                //    { "width": "5%", "targets": 0 },
                //    { "width": "20%", "targets": 1 },
                //    { "width": "10%", "targets": 2 },
                //    { "width": "10%", "targets": 3 },
                //    { "width": "10%", "targets": 4 },
                //    { "width": "10%", "targets": 5 },
                //    { "width": "10%", "targets": 6 },
                //],
                //columns:
                //    [
                //        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                //        { name: "departmentId", data: "departmentId", title: "Department", sortable: false, searchable: false, visible: true },
                //        { name: "documentName", data: "documentName", title: "Document Name", sortable: false, searchable: false, visible: true },
                        
                //        {
                //            name: "documentPath", data: "documentPath", title: "Document", sortable: false, searchable: false, visible: true,
                //            render: function (data, type, row, meta) {
                //                return '<a class="ablue" target="_blank" href="' + row.documentPath + '">' + row.documentName + '</a>'
                //            }
                //        },
                //        { name: "createdDate", data: "createdDate", title: "Date", sortable: false, searchable: false, visible: true },
                //        {
                //            name: "isActive", data: "isActive", title: "Is Active", sortable: false, searchable: false,
                //            render: function (data, type, row, meta) {
                //                if (row.isAllow) {
                //                    var actionButtons = '<center>';

                //                    actionButtons += '<div class="chk-box dis-block clearfix">';
                //                    if (row.isActive == true) {
                //                        actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isActive" name="isActive" value="' + row.id + '" checked/><span class="slider round"></span></label>';
                //                    }
                //                    else {
                //                        actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isActive" name="isActive" value="' + row.id + '" /><span class="slider round"></span></label>';
                //                    }
                //                    actionButtons += '<label for=isActive"></label>'
                //                    actionButtons += '</div>&nbsp;&nbsp;'
                //                    return actionButtons;
                //                }
                //            }
                //        },
                //        {
                //            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                //            render: function (data, type, dataRow, meta) {
                //                if (dataRow.isAllow) {
                //                    var actionButtons = $("<a/>", {
                //                        id: "editevent",
                //                        title: "edit",
                //                        href: domain + "Document/add/" + data.id,
                //                        html: $("<i/>", {
                //                            class: "glyphicon glyphicon-edit",
                //                            style: "color:black"
                //                        }),
                //                    }).get(0).outerHTML + "&nbsp; ";

                //                    return actionButtons;
                //                }
                //            }
                //        },

                //    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                },
                "fnDrawCallback": function (oSettings) {
                    $('.divoverlay').addClass('hide');
                    $('.switchBox').on('change', function () {
                        var switchElement = this;
                        $.get(domain + 'Document/ApprovedStatus', {
                            id: this.value
                        });
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

        function intializeModalWithForm() {

            
        }

        $this.init = function () {
            intializeGrid();
            intializeModalWithForm();
        };
    }

    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));