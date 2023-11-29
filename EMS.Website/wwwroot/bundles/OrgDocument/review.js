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
    function Review() {
        var $this = this, grid, selectedDoc;

        function Intializecontrol() {
            var modal = $("#modal-orgApprovedDoc");
            modal.on('loaded.bs.modal', function () {

                var form = new Global.FormHelper(modal.find("form"), {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                }, null, function (result) {

                    var $MessageDiv = $('#MessageDiv');

                    if (Global.IsNotNullOrEmptyString(result.message)) {
                        $MessageDiv.addClass('alert-success').removeClass('alert-danger');
                        $MessageDiv.empty().html(result.message);
                    }
                    else if (Global.IsNotNullOrEmptyString(result.errorMessage)) {
                        $MessageDiv.addClass('alert-danger').removeClass('alert-success');
                        $MessageDiv.empty().html(result.errorMessage);
                    }

                    $MessageDiv.show();
                    window.setTimeout(function () {
                        $MessageDiv.html('');
                        $MessageDiv.hide();
                    }, 5000);

                    modal.modal('hide');

                    loadOrgDocGrid();

                });

                form.find('#selectedDoc').html(selectedDoc);

            })
                .on('hidden.bs.modal', function () {
                    modal.removeData('bs.modal');
                    modal.find('.modal-content').empty();
                });

            grid.on('click', 'a.btn-link', function () {
                selectedDoc = $(this).closest('tr').find('td:eq(1)').html();
            });
        }

        function loadOrgDocGrid() {
            grid = new Global.GridHelper('#grid-orgdocument', {
                serverSide: true,
                destroy: true,
                "pageLength": 20,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                    {
                        url: domain + "orgdocument/review",
                        type: "Post"
                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "1%", "targets": 0 },
                    { "width": "3%", "targets": 1 },
                    { "width": "20%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "20%", "targets": 4 },
                    { "width": "20%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                ],
                columns:
                    [
                       { name: "Id", data: "id", title: "#", sortable: false, searchable: false, visible: false },
                       { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                       {
                           name: "Document", data: "document", title: "Document", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               return '<a class="ablue" target="_blank" href="' + row.documentPath + '">' + data + " v" + row.version + '</a>'
                           }
                       },
                       {
                           name: "Status", data: "status", title: "Status", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               return row.status + (row.approvedDate ? "<br>" + row.approvedDate : "")
                           }
                       },
                       { name: "Reviewers", data: "reviewers", title: "Approvers", sortable: false, searchable: false, visible: true },
                       {
                           name: "Departments", data: "departments", title: "Access to", sortable: false, searchable: false, visible: true,
                           render: function (data, type, row, meta) {
                               var accessTo = '';
                               accessTo += row.departments ? '<b>Departments</b> : ' + row.departments : '';
                               accessTo += row.roles ? (accessTo != '' ? '<br>' : '') + '<b>Roles</b> : ' + row.roles : '';

                               return accessTo;
                           }
                       },
                       {
                           name: "Action", data: "allowApproval", title: "Action", sortable: false, searchable: false,
                           render: function (data, type, row, meta) {
                               var actions = '';
                               if (row.allowApproval && !row.selfApproved) {
                                   actions = '<a title="Approve document" class="btn btn-default btn-sm" data-backdrop="static" data-target="#modal-orgApprovedDoc" data-toggle="modal", href="' + domain + "orgdocument/OrgDocApprove/" + row.id + '"><i class="fa fa-check-square-o"></i> Approve</a>';
                               }
                               actions += ' <a class="btn btn-default btn-sm" href="' + domain + 'orgdocument/addedit/' + row.id + '"><i class="fa fa-pencil-square-o"></i> Edit</a>';

                               return actions;
                           }
                       }
                    ],
                "fnDrawCallback": function (oSettings) {
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

        $this.init = function () {
            loadOrgDocGrid();
            Intializecontrol();
        };
    }
    $(function () {
        var self = new Review();
        self.init();
    });
}(jQuery));