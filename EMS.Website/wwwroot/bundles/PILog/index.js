/*! DataTables Bootstrap 3 integration
 * �2011-2014 SpryMedia Ltd - datatables.net/license
 */

/**
 * DataTables integration for Bootstrap 3. This requires Bootstrap 3 and
 * DataTables 1.10 or newer.
 *
 * This file sets the defaults and adds options to DataTables to style its
 * controls using Bootstrap. See http://datatables.net/manual/styling/bootstrap
 * for further information.
 */
(function (window, document, undefined) {

    var factory = function ($, DataTable) {
        "use strict";


        /* Set the defaults for DataTables initialisation */
        $.extend(true, DataTable.defaults, {
            dom:
                "<'row'<'col-sm-6'l><'col-sm-6'f>>" +
                "<'row'<'col-sm-12'tr>>" +
                "<'row'<'col-sm-5'i><'col-sm-7'p>>",
            renderer: 'bootstrap'
        });


        /* Default class modification */
        $.extend(DataTable.ext.classes, {
            sWrapper: "dataTables_wrapper form-inline dt-bootstrap",
            sFilterInput: "form-control input-sm",
            sLengthSelect: "form-control input-sm"
        });


        /* Bootstrap paging button renderer */
        DataTable.ext.renderer.pageButton.bootstrap = function (settings, host, idx, buttons, page, pages) {
            var api = new DataTable.Api(settings);
            var classes = settings.oClasses;
            var lang = settings.oLanguage.oPaginate;
            var btnDisplay, btnClass, counter = 0;

            var attach = function (container, buttons) {
                var i, ien, node, button;
                var clickHandler = function (e) {
                    e.preventDefault();
                    if (!$(e.currentTarget).hasClass('disabled')) {
                        api.page(e.data.action).draw(false);
                    }
                };

                for (i = 0, ien = buttons.length ; i < ien ; i++) {
                    button = buttons[i];

                    if ($.isArray(button)) {
                        attach(container, button);
                    }
                    else {
                        btnDisplay = '';
                        btnClass = '';

                        switch (button) {
                            case 'ellipsis':
                                btnDisplay = '&hellip;';
                                btnClass = 'disabled';
                                break;

                            case 'first':
                                btnDisplay = lang.sFirst;
                                btnClass = button + (page > 0 ?
                                    '' : ' disabled');
                                break;

                            case 'previous':
                                btnDisplay = lang.sPrevious;
                                btnClass = button + (page > 0 ?
                                    '' : ' disabled');
                                break;

                            case 'next':
                                btnDisplay = lang.sNext;
                                btnClass = button + (page < pages - 1 ?
                                    '' : ' disabled');
                                break;

                            case 'last':
                                btnDisplay = lang.sLast;
                                btnClass = button + (page < pages - 1 ?
                                    '' : ' disabled');
                                break;

                            default:
                                btnDisplay = button + 1;
                                btnClass = page === button ?
                                    'active' : '';
                                break;
                        }

                        if (btnDisplay) {
                            node = $('<li>', {
                                'class': classes.sPageButton + ' ' + btnClass,
                                'id': idx === 0 && typeof button === 'string' ?
                                    settings.sTableId + '_' + button :
                                    null
                            })
                                .append($('<a>', {
                                    'href': '#',
                                    'aria-controls': settings.sTableId,
                                    'data-dt-idx': counter,
                                    'tabindex': settings.iTabIndex
                                })
                                    .html(btnDisplay)
                                )
                                .appendTo(container);

                            settings.oApi._fnBindAction(
                                node, { action: button }, clickHandler
                            );

                            counter++;
                        }
                    }
                }
            };

            // IE9 throws an 'unknown error' if document.activeElement is used
            // inside an iframe or frame. 
            var activeEl;

            try {
                // Because this approach is destroying and recreating the paging
                // elements, focus is lost on the select button which is bad for
                // accessibility. So we want to restore focus once the draw has
                // completed
                activeEl = $(document.activeElement).data('dt-idx');
            }
            catch (e) { }

            attach(
                $(host).empty().html('<ul class="pagination"/>').children('ul'),
                buttons
            );

            if (activeEl) {
                $(host).find('[data-dt-idx=' + activeEl + ']').focus();
            }
        };


        /*
         * TableTools Bootstrap compatibility
         * Required TableTools 2.1+
         */
        if (DataTable.TableTools) {
            // Set the classes that TableTools uses to something suitable for Bootstrap
            $.extend(true, DataTable.TableTools.classes, {
                "container": "DTTT btn-group",
                "buttons": {
                    "normal": "btn btn-default",
                    "disabled": "disabled"
                },
                "collection": {
                    "container": "DTTT_dropdown dropdown-menu",
                    "buttons": {
                        "normal": "",
                        "disabled": "disabled"
                    }
                },
                "print": {
                    "info": "DTTT_print_info"
                },
                "select": {
                    "row": "active"
                }
            });

            // Have the collection use a bootstrap compatible drop down
            $.extend(true, DataTable.TableTools.DEFAULTS.oTags, {
                "collection": {
                    "container": "ul",
                    "button": "li",
                    "liner": "a"
                }
            });
        }

    }; // /factory


    // Define as an AMD module if possible
    if (typeof define === 'function' && define.amd) {
        define(['jquery', 'datatables'], factory);
    }
    else if (typeof exports === 'object') {
        // Node/CommonJS
        factory(require('jquery'), require('datatables'));
    }
    else if (jQuery) {
        // Otherwise simply initialise as normal, stopping multiple evaluation
        factory(jQuery, jQuery.fn.dataTable);
    }


})(window, document);


(function ($) {
    function Index() {
        var $this = this, grid;

        function loadGrid() {
            $('.divoverlay').removeClass('hide');            
            grid = new Global.GridHelper('#grid-logList', {
                serverSide: true,
                destroy: true,
                "pageLength": 15,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "pilog/index",
                        type: "POST",
                        data: getGridFilters()
                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "1%", "targets": 0 },
                    { "width": "2%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    { "width": "7%", "targets": 6 }
                ],
                columns:
                    [
                       { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       { name: "ProcessName", data: "processName", title: "Process Name", sortable: false, searchable: false, visible: true },
                       { name: "CreateDate", data: "createDate", title: "Date Of Request", sortable: false, searchable: false, visible: true },
                       { name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true },
                       { name: "SuggestedBy", data: "suggestedBy", title: "Suggested By", sortable: false, searchable: false, visible: true },
                       {
                           name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                           render: function (data, type, row, meta) {                             
                               var actionButtons = '';
                               if (row.editAllowed) {
                                   actionButtons += $("<a/>", {
                                       id: "addEdit",
                                       class: "btn btn-default btn-sm",
                                       href: domain + "pilog/addedit/" + row.id,
                                       'data-toggle': "modal",
                                       'data-target': "#modal-add-pilog",
                                       'data-backdrop': "static",
                                       html: $("<i/>", {
                                           class: "fa fa-pencil",
                                           style: "color:black"
                                       }).get(0).outerHTML + "&nbsp; Edit",
                                   }).get(0).outerHTML + "&nbsp; ";
                               }

                               actionButtons += $("<a/>", {
                                   id: "reqApproval",
                                   class: "btn btn-default btn-sm",
                                   title: "Edit",
                                   href: domain + "pilog/requestapproval/" + row.id,
                                   'data-toggle': "modal",
                                   'data-target': "#modal-approve-pilog",
                                   'data-backdrop': "static",
                                   html: $("<i/>", {
                                       class: "fa fa-" + (row.approvalAllowed || row.rollOutAllowed ? "check" : "info-circle"),
                                       style: "color:black"
                                   }).get(0).outerHTML + "&nbsp; " + (row.approvalAllowed ? "Process" : row.rollOutAllowed ? "Roll-Out" : "Details"),
                               }).get(0).outerHTML + "&nbsp; ";
                               
                               return actionButtons;
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
        }

        function getGridFilters() {
            return {
                pmId: $("#PMId").val(),
                status: $("#Status").val(),
            }
        }

        function initialize() {

            $("#modal-add-pilog").on('loaded.bs.modal', function () {
                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "NotificationMessage"
                }, null,
                    function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload();
                            Global.ShowMessage(result.message, true, "divMessage");
                        } else {
                            form.find("#NotificationMessage").html(result);
                        }
                    });
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#modal-approve-pilog").on('loaded.bs.modal', function () {

                var modal = $(this);
                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "NotificationMessage",
                    validateSettings: { ignore: ':disabled' }
                }, null, function (result) {
                        if (result.isSuccess) {
                            modal.modal('hide');
                            grid.ajax.reload();
                            Global.ShowMessage(result.message, true, "divMessage");
                        } else {
                            form.find("#NotificationMessage").html(result);
                        }
                    });

                form.find("#EstimatedSchedule").datepicker({
                    defaultDate: "+1w",
                    dateFormat: "dd/mm/yy",
                    minDate: 0,
                    changeMonth: true,
                    changeYear: true,
                    numberOfMonths: 1
                });

                form.on("change", "#Status", function () {
                    switch (parseInt(this.value)) {
                        case 2:
                            form.find("#divCancelReason").addClass('hidden').find(":input").prop("disabled", true);
                            form.find("#divRemarks").removeClass('hidden').find(":input").prop("disabled", false);
                            form.find("#divEstimatedSchedule").addClass('hidden').find(":input").prop("disabled", true);
                            break;
                        case 3:
                            form.find("#divCancelReason").addClass('hidden').find(":input").prop("disabled", true);
                            form.find("#divRemarks").removeClass('hidden').find(":input").prop("disabled", false);
                            form.find("#divEstimatedSchedule").removeClass('hidden').find(":input").prop("disabled", false);
                            break;
                        case 10:
                            form.find("#divCancelReason").removeClass('hidden').find(":input").prop("disabled", false);
                            form.find("#divRemarks").addClass('hidden').find(":input").prop("disabled", true);
                            form.find("#divEstimatedSchedule").addClass('hidden').find(":input").prop("disabled", true);
                            break;
                        default:
                            form.find("#divCancelReason").addClass('hidden').find(":input").prop("disabled", true);
                            form.find("#divRemarks").addClass('hidden').find(":input").prop("disabled", true);
                            form.find("#divEstimatedSchedule").addClass('hidden').find(":input").prop("disabled", true);
                            break;
                    }
                });

                form.find("#Status").change();

                form.on('click', '#btn-rollOut', function (e) {

                    e.preventDefault();
                    var id = parseInt(form.find('#Id').val());

                    if (id) {
                        if (confirm('Are you sure?\nProcess Improvement suggestion will be Roll Out for all users.')) {
                            form.attr('action', domain + 'pilog/rollout/' + id).submit();
                        }
                    }
                });

            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            $("#btnSearch").on("click", function () {
                loadGrid();
            });
        }

        $this.init = function () {
            loadGrid();
            initialize();
        };
    }
    $(function () {
        var self = new Index();
        self.init();
    });
}(jQuery));