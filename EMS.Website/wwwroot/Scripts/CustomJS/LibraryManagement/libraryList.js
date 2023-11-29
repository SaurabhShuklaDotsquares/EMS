/*global jQuery, Global,secureDomain */
(function () {
    function ManageLibrary() {
        $this = this;

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
                        if (result) {
                            location.reload();
                        }
                        else {
                            alert("Sorry! Something went wrong..");
                        }
                    },
                    error: function (ex) {
                        alert("Sorry! Something went wrong.." + ex);
                    }
                });
            }

        });
        

        function LoadLibraryGrid() {
            this.onChangeAllCheckbox = function () {
                $(".chkDelete").prop("checked", $('.deleteChkBoxAll').prop("checked"));
            };
            var data = {
                DesignLayoutType: $("#DesignLayoutType").val(),
                LibraryComponentType: $("#LibraryComponentType").val()
            };

            $('.divoverlay').removeClass('hide');
            if (isPM == 1) {
                grid = new Global.GridHelper('#grid-librarylist', {
                    serverSide: true,
                    destroy: true,
                    searchDelay: 800,
                    "pageLength": 25,
                    "bFilter": true,
                    "bAutoWidth": false,
                    "language": {
                        searchPlaceholder: "Search By Name"
                    },
                    ajax:
                    {
                        url: domain + "librarymanagement/ManageLibraryList",
                        data: data,
                        type: "POST",
                    },
                    "columnDefs": [
                        { "width": "2%", "targets": 0 },
                        { "width": "27%", "targets": 1 },
                        { "width": "20%", "targets": 2 },
                        { "width": "15%", "targets": 3 },
                        { "width": "5%", "targets": 4 },
                        { "width": "5%", "targets": 5 },
                        { "width": "15%", "targets": 6 },
                        { "width": "10%", "targets": 7 },
                        { "width": "10%", "targets": 8 },
                        { "width": "3%", "targets": 9 }
                    ],
                    columns:
                        [
                            { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                            {
                                name: "Title", data: "title", title: "Title", sortable: false, searchable: false, visible: true,
                                render: function (data, type, row, meta) {
                                    var result = "<b>" + data + "</b>";
                                    if (row.downloadHistory != "" || row.downloadHistory != 0) {
                                        result += '<br /><span><b style="color: #e18f00">Download: </b><b>(' + row.downloadHistory + ')</b></span>';
                                    }
                                    if (row.technology != "") {
                                        result += '<br /><span><b style="color: #e18f00">Technology: </b>' + row.technology + '</span>';
                                    }

                                    //if (row.indutry != "") {
                                    //    result += '<br /><span><b style="color: #e18f00">Industry: </b>' + row.industry + '</span>';
                                    //}
                                    return result;
                                }
                            },
                            {
                                name: "LibraryType", data: "libraryType", title: "Library Type", sortable: false, searchable: false, visible: true,
                                render: function (data, type, row, meta) {
                                    var result = data;
                                    if (row.component != "") {
                                        result += "<br><b>" + row.component + "</b>";
                                    }
                                    if (row.libraryType == 'Component') {
                                        result += "<br>" + "Integration Hours: " + row.integrationHours + "<br>" + "Re-Development Hours: " + row.reDevelopmentHours + "<br>" + "Estimated Hours: " + row.estimatedHours;
                                    }

                                    if (data == "Design" && row.file > 1) {
                                        result += " (" + row.file + ")";
                                    }
                                    if (data == "Design") {
                                        result = "<b>" + result + "</b>";
                                    }
                                    if (row.mainImage != "" && row.libraryType == "Design") {

                                        result += '<br><img src=' + row.mainImage + ' style="width:80px" />'
                                    }
                                    return result;
                                }
                            },
                            { name: "SearchKeyword", data: "searchKeyword", title: "Search Keyword", sortable: false, searchable: false, visible: true },
                            {
                                name: "isFeatured", data: "isFeatured", title: "Featured", sortable: false, searchable: false, visible: true,
                                render: function (data, type, row, meta) {
                                    if (data == "FALSE") {
                                        return '<center><i class="fa fa-times" aria-hidden="true" style="color:#c60909;font-size: 16px;"></i></center>';
                                    } else {
                                        return '<center><i class="fa fa-check" aria-hidden="true" style="color:#068406;font-size: 16px;"></i></center>';
                                    }
                                }
                            },
                            {
                                name: "isActive", data: "isActive", title: "Active", sortable: false, searchable: false, visible: true,
                                render: function (data, type, row, meta) {
                                    if (data == "FALSE") {
                                        return '<center><i class="fa fa-times" aria-hidden="true" style="color:#c60909;font-size: 16px;"></i></center>';
                                    } else {
                                        return '<center><i class="fa fa-check" aria-hidden="true" style="color:#068406;font-size: 16px;"></i></center>';
                                    }
                                }
                            },
                            { name: "CreatedDate", data: "createdDate", title: "Created Date", sortable: false, searchable: false, visible: true },
                            { name: "CreatedBy", data: "createdBy", title: "Created By", sortable: false, searchable: false, visible: true },
                            {
                                name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                                render: function (data, type, row, meta) {
                                    var actionButtons = '<center>';
                                    actionButtons += $("<a/>", {
                                        id: "addEdit",
                                        class: "",
                                        href: domain + "LibraryManagement/AddEdit/?guid=" + row.keyId,
                                        html: $("<i/>", {
                                            class: "fa fa-pencil",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp;",
                                    }).get(0).outerHTML + "&nbsp;&nbsp; ";
                                    //if (isPM == 1) {
                                    //    actionButtons += '<div class="chk-box dis-block clearfix">';
                                    //    if (row.isApproved === "TRUE") {
                                    //        actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + row.keyId + '" checked/><span class="slider round"></span></label>';
                                    //    }
                                    //    else {
                                    //        actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + row.keyId + '" /><span class="slider round"></span></label>';
                                    //    }
                                    //    actionButtons += '<label for=isApproved"></label>'
                                    //    actionButtons += '</div>'
                                    //}
                                    return actionButtons;
                                }
                            },
                            {
                                name: "isApproved", data: "isApproved", title: "Approved", sortable: false, searchable: false,
                                render: function (data, type, row, meta) {
                                    var actionButtons = '<center>';
                                    
                                    if (isPM == 1) {
                                        actionButtons += '<div class="chk-box dis-block clearfix">';
                                        if (row.isApproved === "TRUE") {
                                            actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + row.keyId + '" checked/><span class="slider round"></span></label>';
                                        }
                                        else {
                                            actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + row.keyId + '" /><span class="slider round"></span></label>';
                                        }
                                        actionButtons += '<label for=isApproved"></label>'
                                        actionButtons += '</div>&nbsp;&nbsp;'
                                    }
                                    return actionButtons;
                                }
                            },
                            {
                                name: "Deelete", data: null, title: "Delete All<input type='checkbox' class='deleteChkBoxAll' onchange='onChangeAllCheckbox();' name='deleteChkBox_All'  />", sortable: false, searchable: false,
                                render: function (data, type, row, meta) {
                                    return '<center><input type="checkbox" class="chkDelete" name="deleteChkBox_' + row.keyId + '" value=' + row.keyId + ' /></center>';
                                }
                            },

                        ],
                    "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                        if (aData.isSubmitted) {
                            $('td', nRow).css({ 'background-color': '#f1f1f1', 'color': 'black' });
                        }
                    },
                    "fnDrawCallback": function (oSettings) {
                        $('.divoverlay').addClass('hide');
                        $('.switchBox').on('change', function () {
                            var switchElement = this;
                            $.get(domain + 'librarymanagement/ApprovedStatus', {
                                id: this.value
                            });
                        });
                        if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                            $('.dataTables_paginate').hide();
                        }
                        else {
                            $('.dataTables_paginate').show();
                        }
                        $('.pagination .active a').css('background-color', '#e18f00');
                        $('.pagination .active a').css('border-color', '#e18f00');
                    }
                });
            }
            else
            {
                grid = new Global.GridHelper('#grid-librarylist', {
                    serverSide: true,
                    destroy: true,
                    searchDelay: 800,
                    "pageLength": 25,
                    "bFilter": true,
                    "bAutoWidth": false,
                    "language": {
                        searchPlaceholder: "Search By Name"
                    },
                    ajax:
                    {
                        url: domain + "librarymanagement/ManageLibraryList",
                        data: data,
                        type: "POST",
                    },
                    "columnDefs": [
                        { "width": "2%", "targets": 0 },
                        { "width": "27%", "targets": 1 },
                        { "width": "20%", "targets": 2 },
                        { "width": "15%", "targets": 3 },
                        { "width": "5%", "targets": 4 },
                        { "width": "5%", "targets": 5 },
                        { "width": "15%", "targets": 6 },
                        { "width": "10%", "targets": 7 },
                        { "width": "3%", "targets": 8 }
                    ],
                    columns:
                        [
                            { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                            {
                                name: "Title", data: "title", title: "Title", sortable: false, searchable: false, visible: true,
                                render: function (data, type, row, meta) {
                                    var result = "<b>" + data + "</b>";
                                    if (row.downloadHistory != "" || row.downloadHistory != 0) {
                                        result += '<br /><span><b style="color: #e18f00">Download: </b><b>(' + row.downloadHistory + ')</b></span>';
                                    }
                                    if (row.technology != "") {
                                        result += '<br /><span><b style="color: #e18f00">Technology: </b>' + row.technology + '</span>';
                                    }

                                    //if (row.indutry != "") {
                                    //    result += '<br /><span><b style="color: #e18f00">Industry: </b>' + row.industry + '</span>';
                                    //}
                                    return result;
                                }
                            },
                            {
                                name: "LibraryType", data: "libraryType", title: "Library Type", sortable: false, searchable: false, visible: true,
                                render: function (data, type, row, meta) {
                                    var result = data;
                                    if (row.component != "") {
                                        result += "<br><b>" + row.component + "</b>";
                                    }
                                    if (row.libraryType == 'Component') {
                                        result += "<br>" + "Integration Hours: " + row.integrationHours + "<br>" + "Re-Development Hours: " + row.reDevelopmentHours + "<br>" + "Estimated Hours: " + row.estimatedHours;
                                    }

                                    if (data == "Design" && row.file > 1) {
                                        result += " (" + row.file + ")";
                                    }
                                    if (data == "Design") {
                                        result = "<b>" + result + "</b>";
                                    }
                                    if (row.mainImage != "" && row.libraryType == "Design") {

                                        result += '<br><img src=' + row.mainImage + ' style="width:80px" />'
                                    }
                                    return result;
                                }
                            },
                            { name: "SearchKeyword", data: "searchKeyword", title: "Search Keyword", sortable: false, searchable: false, visible: true },
                            {
                                name: "isFeatured", data: "isFeatured", title: "Featured", sortable: false, searchable: false, visible: true,
                                render: function (data, type, row, meta) {
                                    if (data == "FALSE") {
                                        return '<center><i class="fa fa-times" aria-hidden="true" style="color:#c60909;font-size: 16px;"></i></center>';
                                    } else {
                                        return '<center><i class="fa fa-check" aria-hidden="true" style="color:#068406;font-size: 16px;"></i></center>';
                                    }
                                }
                            },
                            {
                                name: "isActive", data: "isActive", title: "Active", sortable: false, searchable: false, visible: true,
                                render: function (data, type, row, meta) {
                                    if (data == "FALSE") {
                                        return '<center><i class="fa fa-times" aria-hidden="true" style="color:#c60909;font-size: 16px;"></i></center>';
                                    } else {
                                        return '<center><i class="fa fa-check" aria-hidden="true" style="color:#068406;font-size: 16px;"></i></center>';
                                    }
                                }
                            },
                            { name: "CreatedDate", data: "createdDate", title: "Created Date", sortable: false, searchable: false, visible: true },
                            { name: "CreatedBy", data: "createdBy", title: "Created By", sortable: false, searchable: false, visible: true },
                            {
                                name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                                render: function (data, type, row, meta) {
                                    var actionButtons = '<center>';
                                    actionButtons += $("<a/>", {
                                        id: "addEdit",
                                        class: "",
                                        href: domain + "LibraryManagement/AddEdit/?guid=" + row.keyId,
                                        html: $("<i/>", {
                                            class: "fa fa-pencil",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp;",
                                    }).get(0).outerHTML + "&nbsp;&nbsp; ";
                                    //if (isPM == 1) {
                                    //    actionButtons += '<div class="chk-box dis-block clearfix">';
                                    //    if (row.isApproved === "TRUE") {
                                    //        actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + row.keyId + '" checked/><span class="slider round"></span></label>';
                                    //    }
                                    //    else {
                                    //        actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + row.keyId + '" /><span class="slider round"></span></label>';
                                    //    }
                                    //    actionButtons += '<label for=isApproved"></label>'
                                    //    actionButtons += '</div>'
                                    //}
                                    return actionButtons;
                                }
                            },                            
                            {
                                name: "Deelete", data: null, title: "Delete All<input type='checkbox' class='deleteChkBoxAll' onchange='onChangeAllCheckbox();' name='deleteChkBox_All'  />", sortable: false, searchable: false,
                                render: function (data, type, row, meta) {
                                    return '<center><input type="checkbox" class="chkDelete" name="deleteChkBox_' + row.keyId + '" value=' + row.keyId + ' /></center>';
                                }
                            },

                        ],
                    "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                        if (aData.isSubmitted) {
                            $('td', nRow).css({ 'background-color': '#f1f1f1', 'color': 'black' });
                        }
                    },
                    "fnDrawCallback": function (oSettings) {
                        $('.divoverlay').addClass('hide');
                        $('.switchBox').on('change', function () {
                            var switchElement = this;
                            $.get(domain + 'librarymanagement/ApprovedStatus', {
                                id: this.value
                            });
                        });
                        if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                            $('.dataTables_paginate').hide();
                        }
                        else {
                            $('.dataTables_paginate').show();
                        }
                        $('.pagination .active a').css('background-color', '#e18f00');
                        $('.pagination .active a').css('border-color', '#e18f00');
                    }
                });
            }
        }

        $('#btnLibraryFilter').on('click', function () {
            LoadLibraryGrid();
        });
        

        $this.init = function () {
            //initializeForm();
            LoadLibraryGrid();
        }
    }

    $(function () {
        var self = new ManageLibrary;
        self.init();
        console.log("-0-");
    });

}(jQuery));
