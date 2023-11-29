(function ($) {
    "use strict"

    function SDList() {

        var $this = this, grid, form;

        function Intializecontrol() {

            // multiselect list
            $('#TechnologyId').multipleSelect({

                width: '100%',
                placeholder: "Select Technology",
                maxHeight: 150,
            });

            $('#TechnologyId').on("change", function () {

                LoadSDGrid();
            });

            // select all
            $(document).on("change", ".selectChkBoxAll", function () {

                $(".chkSelect").prop("checked", $(this).prop("checked"));
            });

            // delete
            $(document).on("click", '#btnSDDeleteAll', function () {

                var strKeyIds = getSelectedCheckbox();
                if (strKeyIds == '') {
                    swal({
                        text: "Please select at least one record from List.",
                        icon: "info",
                    });
                    return;
                }

                // confirm
                swal({
                    text: "Would you like to delete selected record(s)?",
                    icon: "info",
                    buttons: {
                        yes: {
                            text: "Ok",
                            value: "yes"
                        },
                        no: {
                            text: "Cancel",
                            value: "no"
                        }
                    }
                }).then((value) => {
                    if (value === "yes") {

                        $.ajax({
                            url: domain + "studydocuments/deletestudydocuments",
                            type: 'POST',
                            datatype: 'application/json',
                            data: { id: strKeyIds },
                            success: function (result) {
                                if (result) {
                                    location.reload();
                                }
                                else {
                                    swal({
                                        text: "Sorry! Something went wrong.",
                                        icon: "error",
                                    });
                                }
                            },
                            error: function (ex) {
                                swal({
                                    text: "Sorry! Something went wrong.",
                                    icon: "error",
                                });
                            }
                        });
                    }
                    return false;
                });

            });

            // approve
            $(document).on("click", '#btnSDApproveAll', function () {

                approveStatus(true);
            });

            // disapprove
            $(document).on("click", '#btnSDDisapproveAll', function () {

                approveStatus(false);
            });

            // model for un-approved-reason
            $(document).on('shown.bs.modal', "#modal-SD-unapprovedreason", function () {

                initUnapprovedReasonPartialForm();
            }).on('hidden.bs.modal', function () {

                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            // add/delete user permission
            $(document).on("click", '#btnSDAddDelPermissionAll', function () {

                addDelPermission();
            });

            // model for get user and than save
            $(document).on('shown.bs.modal', "#modal-SD-adddelpermission", function () {

                initAddDelPermisionPartialForm();
            }).on('hidden.bs.modal', function () {

                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            // quick view
            $(document).on("click", "#quickView", function () {
                //debugger

                var keyid = $(this).data("keyid");
                $.ajax({
                    url: domain + "studydocuments/quickview",
                    type: 'get',
                    data: { id: keyid },
                    success: function (result) {
                        //debugger
                        if (result) {

                            $("#modal-SD-quickview").find(".modal-content").html(result);
                            $("#modal-SD-quickview").modal({ backdrop: 'static', keyboard: false }, 'show');
                        }
                        else {
                            swal({
                                text: "Sorry! Something went wrong.",
                                icon: "error",
                            });
                        }
                    },
                    error: function (ex) {
                        swal({
                            text: "Sorry! Something went wrong.",
                            icon: "error",
                        });
                    }
                });
            });

            // create duplicate entry
            $(document).on("click", "#cloneEntry", function () {
                //debugger

                // confirm
                swal({
                    text: "Do you want to create duplicate entry?",
                    icon: "info",
                    buttons: {
                        yes: {
                            text: "Ok",
                            value: "yes"
                        },
                        no: {
                            text: "Cancel",
                            value: "no"
                        }
                    }
                }).then((value) => {
                    if (value === "yes") {

                        var keyid = $(this).data("keyid");
                        $.ajax({
                            url: domain + "studydocuments/createDuplicateEntry",
                            type: 'post',
                            data: { id: keyid },
                            success: function (result) {
                                //debugger
                                if (result) {
                                    window.location.reload();
                                }
                                else {
                                    swal({
                                        text: "Sorry! Something went wrong.",
                                        icon: "error",
                                    });
                                }
                            },
                            error: function (ex) {
                                swal({
                                    text: "Sorry! Something went wrong.",
                                    icon: "error",
                                });
                            }
                        });

                    }
                    return false;
                });
               
            });

            // model for quick view
            $(document).on('shown.bs.modal', "#modal-SD-quickview", function () {

                initQuickViewPartialForm();
            }).on('hidden.bs.modal', function () {

                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            // view reason
            $(document).on("click", ".clsviewreason", function () {
                var reason = $(this).data("reason");
                $("#modal-SD-unapprovedreasonmessage").modal('show');
                $("#modal-SD-unapprovedreasonmessage").find(".modal-content-message").html(reason);
            });

        }

        // approve status
        function approveStatus(status) {
            //debugger

            var strKeyIds = getSelectedCheckbox();
            if (strKeyIds == '') {
                swal({
                    text: "Please select at least one  record from List.",
                    icon: "info",
                });
                return;
            }

            var _type = null;
            var _data = null;
            var msg = null;
            if (status) {
                _type = "post";
                _data = { StudyDocumentIds: strKeyIds, IsApproved: status, UnapprovedReason: "na" };
                msg = "Would you like to 'approve' selected record(s)?";
            } else {
                _type = "get";
                _data = {};
                msg = "Do you want to 'un-approve' selected records?\n\nPlease fill the reason in further screen and take action accordingly.";
            }

            // confirm
            swal({
                text: msg,
                icon: "info",
                buttons: {
                    yes: {
                        text: "Ok",
                        value: "yes"
                    },
                    no: {
                        text: "Cancel",
                        value: "no"
                    }
                }
            }).then((value) => {
                if (value === "yes") {

                    $.ajax({
                        url: domain + "studydocuments/ApproveStudyDocuments",
                        type: _type,
                        datatype: 'application/json',
                        data: _data,
                        success: function (result) {
                            //debugger

                            if (status) {

                                if (result) {
                                    window.location.reload();
                                } else {
                                    swal({
                                        text: "Sorry! Something went wrong.",
                                        icon: "error",
                                    });
                                }
                            }
                            else {

                                $("#modal-SD-unapprovedreason").find(".modal-content").html(result);
                                $("#modal-SD-unapprovedreason").modal({ backdrop: 'static', keyboard: false }, 'show');

                                // set selected ids in hidden
                                $("#StudyDocumentIds").val(strKeyIds);
                            }
                        },
                        error: function (ex) {
                            swal({
                                text: "Sorry! Something went wrong.",
                                icon: "error",
                            });
                        }
                    });
                }
                return false;
            });

        }

        // initilize un-approved-reason
        function initUnapprovedReasonPartialForm() {

            // form 
            form = new FormHelperWithCustomFileCollection($("form#frmunapprovedreason"), {

                updateTargetId: "validation-summary-unapprovedreason",
                validateSettings: {
                    ignore: ''
                }
            });
        }

        // add/delete user permission
        function addDelPermission() {
            //debugger
            var strKeyIds = getSelectedCheckbox();
            if (strKeyIds == '') {
                swal({
                    text: "Please select at least one from List.",
                    icon: "info",
                });
                return;
            }

            // confirm
            swal({
                text: "Do you want to 'allow/restrict' user permission to selected study document(s)?\n\Please take action accordingly on next screen.",
                icon: "info",
                buttons: {
                    yes: {
                        text: "Ok",
                        value: "yes"
                    },
                    no: {
                        text: "Cancel",
                        value: "no"
                    }
                }
            }).then((value) => {
                if (value === "yes") {

                    $.ajax({
                        url: domain + "studydocuments/AddDelUsersPermission",
                        type: 'get',
                        success: function (result) {
                            //debugger
                            if (result) {

                                $("#modal-SD-adddelpermission").find(".modal-content").html(result);
                                $("#modal-SD-adddelpermission").modal({ backdrop: 'static', keyboard: false }, 'show');

                                // set selected ids in hidden
                                $("#StudyDocumentIds").val(strKeyIds);
                            }
                            else {
                                swal({
                                    text: "Sorry! Something went wrong.",
                                    icon: "error",
                                });
                            }
                        },
                        error: function (ex) {
                            swal({
                                text: "Sorry! Something went wrong.",
                                icon: "error",
                            });
                        }
                    });
                }
                return false;
            });

        }

        // initilize add/del permission components
        function initAddDelPermisionPartialForm() {

            // form 
            form = new FormHelperWithCustomFileCollection($("form#frmadddelpermission"), {

                updateTargetId: "validation-summary-adddelpermission",
                validateSettings: {
                    ignore: ''
                },
                beforeSubmit: function () {

                    //debugger
                    var users = $("#UserId").val();
                    if (users == null || users == undefined || users.length <= 0) {
                        swal({
                            text: "please select user(s).",
                            icon: "info",
                        });
                        return false;
                    }
                    return true;
                }
            });
            // multiselect list
            $('.multiselectcheckbox').multipleSelect({

                width: '100%',
                placeholder: "Select User",
                maxHeight: 150,
            });
            // date from
            $("#StartDate").datepicker({

                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                minDate: 0,
                //maxDate: "+" + mindateval + "D", 
                onClose: function (selectedDate) {
                    //debugger
                    $("#EndDate").datepicker("option", "minDate", selectedDate);
                }
            }).datepicker("setDate", new Date());
            // date to
            $("#EndDate").datepicker({

                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                minDate: 0,
                //maxDate: "+" + mindateval + "D"
            }).datepicker("setDate", new Date());

            // hide date option if delete
            $("#AddDelPermission").on("change", function () {

                $(".clsdateoption").toggle("slow");
            });

        }

        // initilize quick view components
        function initQuickViewPartialForm() {

            $(".clsdelSdfile").on("click", function () {

                //debugger
                if ($(".clsdelSdfile").length == 1) {
                    swal({
                        text: "you cannot remove all files, at least one file whould be require.",
                        icon: "info",
                    });
                    return false;
                }

                // confirm
                swal({
                    text: "Do you want to remove selected record?",
                    icon: "info",
                    buttons: {
                        yes: {
                            text: "Ok",
                            value: "yes"
                        },
                        no: {
                            text: "Cancel",
                            value: "no"
                        }
                    }
                }).then((value) => {
                    if (value === "yes") {

                        var $this = $(this);
                        var keyid = $this.data("keyid");
                        var filekeyid = $this.data("filekeyid");

                        $.ajax({
                            url: domain + "studydocuments/DeleteStudyDocumentFile",
                            type: 'post',
                            data: { id: keyid, fileKeyId: filekeyid },
                            success: function (result) {
                                //debugger

                                $("#validation-summary-quickview").html(result);
                                $this.closest("tr.clssdf").remove();
                                $(".clssdf").each(function (index, val) {
                                    //debugger

                                    var $val = $(val);

                                    // all reset for assigned class
                                    var $clsresetindex = $val.find(".clsresetindex");
                                    $clsresetindex.each(function ($clsresetindex_i, $clsresetindex_val) {

                                        var $ele = $($clsresetindex_val);
                                        var data_rowno = $ele.attr("data-rowno");

                                        if (data_rowno != null && data_rowno != undefined) {
                                            $ele.text(index + 1);
                                        }
                                    });
                                });
                            },
                            error: function (ex) {
                                swal({
                                    text: "Sorry! Something went wrong.",
                                    icon: "error",
                                });
                            }
                        });
                    }
                    return false;
                });

            });
        }

        // selected checkbox with ids
        function getSelectedCheckbox() {
            var strKeyIds = "";
            $(".chkSelect:checked").each(function (i, v) {
                strKeyIds += (strKeyIds == "" ? v.value : "," + v.value);
            });
            return strKeyIds;
        }


        // grid
        function LoadSDGrid() {

            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#gridSDlist', {
                serverSide: true,
                destroy: true,
                searchDelay: 800,
                "pageLength": 50,
                "bFilter": true,
                "bAutoWidth": false,
                "language": {
                    searchPlaceholder: "Search By Title",
                },
                ajax:
                {
                    url: domain + "studydocuments/index?TechnologyId=" + (($("#TechnologyId").val() == null || $("#TechnologyId").val() == undefined) ? "" : $("#TechnologyId").val()),
                    //data: data,
                    type: "POST",
                },
                "columnDefs": [
                    { "width": "3%", "targets": 0 },
                    { "width": "15%", "targets": 1 },
                    { "width": "25%", "targets": 2 },
                    { "width": "5%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    { "width": "3%", "targets": 6 },
                    { "width": "3%", "targets": 7 },
                    { "width": "3%", "targets": 8 },
                    { "width": "1%", "targets": 9 }
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "title", data: "title", title: "Title", sortable: false, searchable: false, visible: true },
                        { name: "description", data: "description", title: "Description", sortable: false, searchable: false, visible: true },
                        { name: "technology", data: "technology", title: "Technology", sortable: false, searchable: false, visible: true },
                        { name: "addedDate", data: "addedDate", title: "Created Date", sortable: false, searchable: false, visible: true },
                        { name: "addedBy", data: "addedBy", title: "Created By", sortable: false, searchable: false, visible: true },
                        {
                            name: "isActive", data: "isActive", title: "Active", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (data == "false") {
                                    return '<center><i class="fa fa-times" aria-hidden="true" style="color:#c60909;font-size: 16px;"></i></center>';
                                } else {
                                    return '<center><i class="fa fa-check" aria-hidden="true" style="color:#068406;font-size: 16px;"></i></center>';
                                }
                            }
                        },
                        {
                            name: "isApproved", data: "isApproved", title: "Approved", sortable: false, searchable: false, visible: true,
                            render: function (data, type, row, meta) {
                                if (data == "false") {

                                    if (row.unApprovedReason.toLowerCase() == "na") {
                                        return '<center><i class="fa fa-times" aria-hidden="true" style="color:#c60909;font-size: 16px;"></i></center>';
                                    } else {
                                        return '<center><a class="clsviewreason" title="View reason" style="cursor:pointer;" data-reason="' + row.unApprovedReason + '"><i class="fa fa-times" aria-hidden="true" style="color:#c60909;font-size: 16px;"></a></i></center>';
                                    }
                                } else {
                                    return '<center><i class="fa fa-check" aria-hidden="true" style="color:#068406;font-size: 16px;"></i></center>';
                                }
                            }
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '<center/>';

                                // edit
                                if ($("#hasPermission").val() == "true" || row.isApproved == "false") {
                                    actionButtons += $("<a/>", {
                                        id: "addEdit",
                                        class: "",
                                        title: "Edit Study Center",
                                        href: domain + "studydocuments/AddEdit/" + row.keyId,
                                        html: $("<i/>", {
                                            class: "fa fa-pencil",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp;",
                                    }).get(0).outerHTML + "&nbsp;&nbsp; ";
                                }
                                // quick view
                                if ($("#hasPermission").val() == "true") {
                                    actionButtons += $("<a/>", {
                                        id: "quickView",
                                        class: "",
                                        title: "Quick View Study Center",
                                        href: "javascript:void(0)",
                                        "data-keyid": row.keyId,
                                        html: $("<i/>", {
                                            class: "fa fa-eye",
                                            style: "color:black"
                                        }).get(0).outerHTML + "&nbsp;",
                                    }).get(0).outerHTML + "&nbsp;&nbsp; ";
                                }
                                // create duplicate entry
                                actionButtons += $("<a/>", {
                                    id: "cloneEntry",
                                    class: "",
                                    title: "Create Duplicate Record",
                                    href: "javascript:void(0)",
                                    "data-keyid": row.keyId,
                                    html: $("<i/>", {
                                        class: "fa fa-clone",
                                        style: "color:black"
                                    }).get(0).outerHTML + "&nbsp;",
                                }).get(0).outerHTML + "&nbsp;&nbsp; ";

                                return actionButtons;
                            }
                        },
                        {
                            name: "Select", data: null, title: "All<input type='checkbox' class='selectChkBoxAll' />", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                return '<center><input type="checkbox" class="chkSelect" value=' + row.id + ' /></center>';
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


        // init
        $this.init = function () {
            LoadSDGrid();
            Intializecontrol();
        };
    }
    $(function () {
        var self = new SDList();
        self.init();
    });
}(jQuery));