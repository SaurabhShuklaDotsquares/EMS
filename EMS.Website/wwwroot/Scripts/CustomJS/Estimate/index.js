(function ($) {
    function ProjectLead() {

        var $this = this;
        $this.newClient = false;
        $this.exsistingClient = false;
        $this.escalatedClient = false;
        $this.awaitingResponse = false;
        $this.newConvertedClient = false;
        $this.existingConvertedClient = false;
        $SelectedValueForNote = '';
        $this.getLeadSummary = function (isPageLoad, isUpdateLeadSummaryTop) {
           
            if (isUpdateLeadSummaryTop) {
                $("#summary_stats").addClass("hidden");
                $("#loading_stats").removeClass("hidden");
                var param = $this.estimateRequestParam(isPageLoad, isUpdateLeadSummaryTop);
               
                $.ajax({
                    url: domain + "estimate/getleadsummary",
                    type: 'POST',
                    datatype: 'application/json',
                    contentType: 'application/json',
                    data: JSON.stringify(param),
                    success: function (json) {
                       
                        if (json.leadSummary != null) {                            
                            $this.bindLeadSummary(json.leadSummary);
                        }                       
                        $this.bindUserLeadSummary(json.userLeadSummary);

                        $("#loading_stats").addClass("hidden");
                        $("#summary_stats").removeClass("hidden");

                    },
                    error: function (ex) {
                        alert("Whooaaa! Something went wrong.." + ex);
                    }
                });
            }
        };

        $this.bindLeadSummary = function (json) {            
            $('#totalLead').html(json.totalClients);
            $('#spanNewClient').html(json.newClients);
            $('#spanExistingClient').html(json.existingClient);
            $('#convertedLead').html(json.totalConvertedClients);
            $('#spanConvertedNewClient').html(json.convertedNewClients);
            $('#spanConvExistingClient').html(json.convertedExistingClients);
            $('#spanEscalatedClient').html(json.escalatedLeads);
            $('#spanAwaitingResponse').html(json.awaitingResponseLeads);
            $('#totalConversion').html(json.totalConversion);
            $('#totalNewConversion').html(json.totalNewLeadConversion);
            $(".remarkTitle").tooltip();
        };

        $this.bindUserLeadSummary = function (details) {
            
            var html = '';
            $("#tblEstimateOwnerRight tbody tr, #tblEstimateOwnerLeft tbody tr").remove();
            $.each(details, function (indx, data) {
                var $container = $(data.index % 2 != 0 ? "#tblEstimateOwnerRight" : "#tblEstimateOwnerLeft");
                $("<tr/>", {
                    "class": (data.index % 2 == 0 ? "alternate " : "") + (data.index > 9 ? "showmore hidden " : ""),
                    html: function () {
                        $("<td/>", { text: data.ownerName }).appendTo(this);
                        $("<td/>", {
                            text: data.newConversion + " (" + data.totalNew + ")",
                            "style": "text-align:center"
                        }).appendTo(this);
                        $("<td/>", {
                            text: data.existingConversion == 0 && data.totalExisting == 0 ? " - " : data.existingConversion + " (" + data.totalExisting + ")",
                            "style": "text-align:center"
                        }).appendTo(this);
                    }
                }).appendTo($container);
            });
            $("span.showmoreless").parent().css("visibility", (details.length <= 10 ? "hidden" : "visible"));
            $("span.more.showmoreless").css("display", "");
            $("span.less.showmoreless").css("display", "none");
        }

        $this.getSelectedCountries = function () {
            var chkCountryArray = [];
            $('input[name="Country"]:checked').each(function () {
                chkCountryArray.push($(this).val());
            });
            return chkCountryArray;
        }

        $this.loader = function (isShow) {
            if (isShow) {
                //start loading
                $(".divoverlay").removeClass('hide');
            }
            else {
                //end loading
                $(".divoverlay").addClass('hide');
            }
        };

        $this.estimateRequestParam = function (isPageLoad, isUpdateLeadSummaryTop) {
            var chkCountryArray = $this.getSelectedCountries();
            var chkStatusArray = [];
            $('input[name="leadStatus"]:checked').each(function () {
                chkStatusArray.push($(this).val());
            });

            var data = {
                txtSearch: $('#txtSearch').val(), txtAssignedFrom: $('#txtAssignedFrom').val(), txtAssignedTo: $('#txtAssignedTo').val(),
                drpOwner: $('#drpOwner').val(), drpStatus: $('#drpStatus').val(), drpType: $('#drpType').val(), drpClient: $('#drpClient').val(),
                chkCountry: chkCountryArray, chkStatus: chkStatusArray, existClient: $this.exsistingClient, newClient: $this.newClient, awaitResp: $this.awaitingResponse,
                escalatedClient: $this.escalatedClient, newConverted: $this.newConvertedClient, existingConvert: $this.existingConvertedClient, isPageLoad: isPageLoad, isUpdateLeadSummaryTop: isUpdateLeadSummaryTop, isCovid19: $('#covid19').is(":checked")
            };

            return data;
        };

        $this.bindEstimateGrid = function (isPageLoad, isUpdateLeadSummaryTop) {
            $this.loader(true);
            var param = $this.estimateRequestParam(isPageLoad, isUpdateLeadSummaryTop);
            var EstimateGrid = new Global.GridHelper('#grid-Estimates', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                "ordering": false,
                ajax: {
                    url: domain + "Estimate/Index",
                    type: "Post",
                    data: param
                },
                "columnDefs": [
                    { "width": "8%", "targets": 0 },
                    { "width": "25%", "targets": 1 },
                    { "width": "5%", "targets": 2 },
                    { "width": "15%", "targets": 3 },
                    { "width": "10%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                    { "width": "10%", "targets": 7 },
                    { "width": "13%", "targets": 8 }
                ],
                columns:
                    [
                       {
                           name: "client", data: "client", title: "Lead/Client", sortable: false, searchable: false, visible: true,
                           render: function (data, type, dataRow, meta) {
                               var client = "CRM Lead Id : " + (dataRow.crmid != "" ? "<b>" + dataRow.crmid + "</b>" : "N/A") + '</br>';
                               client += 'EMS Lead Id: ' + dataRow.leadId + '</br>';

                               client += dataRow.clientId != "" ? "<b>" + dataRow.client + "</b></br>" :
                                   (dataRow.allowClientAndDelete ? "<a data-toggle='modal' style='text-decoration:underline;cursor:pointer;' data-target='#modal-createSelectClient' href='" + domain + "estimate/AddClient/" + dataRow.leadId + "'>Client:N/A</a></br>" : "");

                               //client += dataRow.clientId != "" ? "ClientId:<b>" + dataRow.clientId + "</b></br><b>" + dataRow.client + "</b>" : (dataRow.allowClientAndDelete ? "<a data-toggle='modal' style='text-decoration:underline;cursor:pointer;' data-target='#modal-createSelectClient' href='" + domain + "estimate/AddClient/" + dataRow.leadId + "'>ClientID:N/A</a>" : "ClientID:N/A");

                               return client += dataRow.newClient;
                           }
                       }, {
                           name: "title", data: "title", title: "Title", sortable: false, searchable: false, visible: true,
                           render: function (data, type, dataRow, meta) {

                               var title = '<img title="' + dataRow.abroadPM + '" src="' + domain + 'images/icons/' + dataRow.abroadPM + '.jpg" alt=""/> ';
                               title += dataRow.allowConclusionAndEdit ? '<a data-toggle="modal" data-target="#modal-conclusion" style="text-decoration:underline;text-transform:uppercase;font-weight: bold;" href="' + domain + 'estimate/Conclusion/' + dataRow.leadId + '">' + dataRow.leadTitle + '</a>' : '<span style="text-transform:uppercase;font-weight: bold;">' + dataRow.leadTitle + '</span>';

                               return title += ' ' + (dataRow.remark != "" ? "<a href='javascript:void(0)' class='remarkTitle' title='" + dataRow.remark + "'><img src='" + domain + "images/icons/Comments-icon.png' alt='R' /></a>" : "")
                                   //+ ' [<b>LeadId:' + dataRow.leadId + '</b>]'
                                   + '</br><div style="font-size: 11px;">Generated on: ' + dataRow.generatedDate + '</br>Last Activity: ' + dataRow.modifiedDate + ' </br> <a target="_black" style="text-decoration:underline;" href="' + domain + 'upload/estimatedocument/' + dataRow.proposalDocument + '">' + dataRow.proposalDocument + '</a></div>';
                           }
                       },
                        { name: "estimateTime", data: "estimateTimeinDay", title: "ESTIMATED TIME", sortable: false, searchable: false, visible: true },
                        {
                            name: "owners", data: "owners", title: "Estimate/Comm. Owners", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var rData = dataRow.ownersName + '</br>' + (dataRow.nextChaseDate != "" ? "<b>Next Chase On :</b> " + dataRow.nextChaseDate : "");
                                if (dataRow.showConversionDate && dataRow.conversionDate != "") {
                                    rData += "<div style='margin-bottom:5px;margin-top:5px;'><label class='label future-occupancy-label-color converted'><b>Conversion Date : </b>" + dataRow.conversionDate + "</label></div>";
                                }

                                return rData;
                            }
                        },
                        { name: "technologies", data: "technologies", title: "technologies", sortable: false, searchable: false, visible: true },
                        {
                            name: "source", data: "source", title: "Source", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                return (dataRow.source != "" ? "<b>" + dataRow.source + "</b><br/>" + dataRow.assignedDate : "");
                            }
                        },
                        {
                            name: "status", data: "status", title: "Status", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                if (dataRow.allowAction) {
                                    return '<a style="text-decoration: underline" data-toggle="modal" data-target="#modal-leadStatus" href="' + domain + 'estimate/LeadStatus/' + dataRow.leadId + '">'
                                        + dataRow.status.replace("Team", dataRow.source).replace("Out of India PM", dataRow.source + ' - ' + dataRow.abroadPM) + '</a>';
                                }
                                else {
                                    return dataRow.status.replace("Team", dataRow.source).replace("Out of India PM", dataRow.source + ' - ' + dataRow.abroadPM)
                                }
                            }
                        },
                        {
                            name: "lastConversation", data: "lastConversation", title: "Last Conv.", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                if (dataRow.lastConversation == "" || !(dataRow.showLastConversationInDiv && dataRow.lastConversation.substring(dataRow.lastConversation.length - 3) == "...")) {
                                    return dataRow.lastConversation;
                                } else {
                                    var lastConversation = '<lable>' + $(dataRow.lastConversationFull).text().substring(0, 100) + '</lable>';

                                    lastConversation += '<div class="tooltipfortable">...more<span class="tooltiptextfortable">' + dataRow.lastConversationFull + '</span></div>';
                                    return lastConversation;
                                }
                            }
                        },
                        {
                            name: "action", data: null, title: "Action", sortable: false, searchable: false, visible: true,
                            render: function (data, item, row, meta) {
                                var actionButtons = "";

                                if (data.allowAction) {
                                    actionButtons += $("<a/>", {
                                        id: "chasetask",
                                        title: "Chase",
                                        href: domain + "estimate/LeadStatus/" + data.leadId,
                                        'data-toggle': "modal",
                                        'data-target': "#modal-leadStatus",
                                        'data-backdrop': "static",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-share",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML + "&nbsp; ";
                                }


                                actionButtons += $("<a/>", {
                                    id: "viewDetail",
                                    title: "View History",
                                    href: domain + "estimate/EstimateHistory/" + row.leadId,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-lead-estimateHistory",
                                    html: $("<i/>", {
                                        class: "glyphicon glyphicon-eye-open",
                                        style: "color:black"
                                    })
                                }).get(0).outerHTML + "&nbsp; ";

                                if (row.crmid != "" && row.crmid != "0") {
                                    actionButtons += $("<a/>", {
                                        id: "viewCRMNotes",
                                        class:"clsViewNote",
                                        title: "CRM Notes",
                                        'data-id': row.crmid,                                        
                                        'data-toggle': "modal",
                                        'data-target': "#modal-lead-crmnotes",
                                        html: $("<i/>", {
                                            class: "glyphicon glyphicon-th-list",
                                            style: "color:black"
                                        })
                                    }).get(0).outerHTML + "&nbsp; ";
                                }

                                if (data.allowConclusionAndEdit) {
                                    actionButtons += $("<a/>", {
                                        id: "editLead",
                                        title: "Edit",
                                        href: domain + "estimate/AddEditLead/" + row.leadId,
                                        html: $("<i/>", {
                                            class: "fa fa-edit",
                                            style: "color:black"
                                        })
                                    }).get(0).outerHTML + "&nbsp; ";
                                }

                                if (data.allowClientAndDelete) {
                                    actionButtons += $("<a/>", {
                                        href: domain + "estimate/DeleteLead/" + row.leadId,
                                        id: "deletetask",
                                        title: "Delete",
                                        'data-toggle': "modal",
                                        'data-target': "#modal-lead-delete",
                                        html: $("<i/>", {
                                            class: "fa fa-trash",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML;
                                }

                                return actionButtons;

                            }
                        },
                    ],

                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                    switch (aData["status"]) {
                        case "Chase Request":
                            $(nRow).addClass('freeuser_border awaiting FillCellBasicStyleForGrid');
                            break;
                        case "Action Required From (Team)":
                        case "Action Required From (Out of India PM)":
                            $(nRow).addClass('awaiting FillCellBasicStyleForGrid');
                            break;
                        case "Converted":
                        case "Almost Converted":
                            $(nRow).addClass('darkgreen_border converted FillCellBasicStyleForGrid');
                            break;
                        case "Do Not Chase":
                            $(nRow).addClass('donotchase_border FillCellBasicStyleForGrid');
                            break;
                        case "Escalated":
                            $(nRow).addClass('escalated_border FillCellBasicStyleForGrid');
                            break;
                    }

                },
                "fnDrawCallback": function (oSettings) {
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $(".dataTables_paginate").hide();
                    }
                    else {
                        $(".dataTables_paginate").show();
                        $(".pagination .active a").css('background-color', '#e99701');
                        $('.pagination .active a').css('border-color', '#e99701');
                    }
                    $(".remarkTitle").tooltip();
                },
                "fnInitComplete": function (oSettings, json) {
                    //bind top Project lead summary details
                    $this.loader(false);
                }
            })
        };

        $this.attachEventCKEditor = function (instance) {
            CKEDITOR.instances[instance].on("instanceReady", function (e) {
                e.editor.document.on("keyup", function () {
                    CKEDITOR.instances[instance].updateElement();
                });
            });
        };

        $this.bindInitialEvents = function () {
            $(document).on("click", '#newClient', function () {
                $this.newClient = true;
                $this.exsistingClient = false;
                $this.escalatedClient = false;
                $this.awaitingResponse = false;
                $this.newConvertedClient = false;
                $this.existingConvertedClient = false;

                $this.bindEstimateGrid(false, false);

            });

            $(document).on("click", '#existingClient', function () {
                $this.newClient = false;
                $this.exsistingClient = true;
                $this.escalatedClient = false;
                $this.awaitingResponse = false;
                $this.newConvertedClient = false;
                $this.existingConvertedClient = false;
                $this.bindEstimateGrid(false, false);
            });

            $(document).on("click", '#escalatedClient', function () {
                $this.newClient = false;
                $this.exsistingClient = false;
                $this.escalatedClient = true;
                $this.awaitingResponse = false;
                $this.newConvertedClient = false;
                $this.existingConvertedClient = false;

                $this.bindEstimateGrid(false, false);
            });

            $(document).on("click", '#awaitingResponse', function () {
                $this.newClient = false;
                $this.exsistingClient = false;
                $this.escalatedClient = false;
                $this.awaitingResponse = true;
                $this.newConvertedClient = false;
                $this.existingConvertedClient = false;

                $this.bindEstimateGrid(false, false);
            });

            $(document).on("click", '#newConvertedClient', function () {
                $this.newClient = false;
                $this.exsistingClient = false;
                $this.escalatedClient = false;
                $this.awaitingResponse = false;
                $this.newConvertedClient = true;
                $this.existingConvertedClient = false;

                $this.bindEstimateGrid(false, false);
            });

            $(document).on("click", '#existingConvertedClient', function () {
                $this.newClient = false;
                $this.exsistingClient = false;
                $this.escalatedClient = false;
                $this.awaitingResponse = false;
                $this.newConvertedClient = false;
                $this.existingConvertedClient = true;

                $this.bindEstimateGrid(false, false);
            });

            $('#txtAssignedFrom').datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                onSelect: function (selectedDate) {
                    $("#txtAssignedTo").datepicker("option", "minDate", selectedDate);
                }
            });

            $('#txtAssignedTo').datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0",
                onSelect: function (selectedDate) {
                    $("#txtAssignedFrom").datepicker("option", "maxDate", selectedDate);
                }
            });

            $(document).on("click", '#btnSearch', function () {
                $this.newClient = false;
                $this.exsistingClient = false;
                $this.escalatedClient = false;
                $this.awaitingResponse = false;
                $this.newConvertedClient = false;
                $this.existingConvertedClient = false;
                $this.bindEstimateGrid(false, true);
                $this.getLeadSummary(false, true);
            });

            $('.showmoreless').click(function () {
                if ($(this).hasClass('more')) {
                    $("#btnMore").hide();
                    $(".showmore").removeClass("hidden");
                    $("#btnLess").show();
                } else {
                    $("#btnLess").hide();
                    $(".showmore").addClass("hidden");
                    $("#btnMore").show();
                }
            });

            $("#clrFilterDate").click(function () {
                $('#txtAssignedFrom').val('');
                $('#txtAssignedTo').val('');
            });

            $('#btnReset').click(function () {
                $('#txtAssignedTo').val('');
                $('#txtAssignedFrom').val('');
                $('#txtSearch').val('');
                $('#drpOwner').val('');
                $('#drpStatus').val('');
                $('#drpType').val('');
                $('#drpClient').val('');
                $('input[name="Country"]:checked').each(function () {
                    $(this).prop('checked', false);
                })
                $('input[name="leadStatus"]:checked').each(function () {
                    $(this).prop('checked', false);
                })
                $(".topbox").removeClass('checed-box');
                $('#covid19').prop('checked', false);
                $this.bindEstimateGrid(true, true);
                $this.getLeadSummary(true, true);
            });

            $(document).on('change', '.chk_box input[type="checkbox"]', function () {
                $this.newClient = false;
                $this.exsistingClient = false;
                $this.escalatedClient = false;
                $this.awaitingResponse = false;
                $this.newConvertedClient = false;
                $this.existingConvertedClient = false;

                if (this.name == 'Country') {
                    $this.bindEstimateGrid(false, true);
                    $this.getLeadSummary(false, true);
                }
                else {
                    $this.bindEstimateGrid(false, false);
                }
            });

            $("#modal-lead-estimateHistory").on("hidden.bs.modal", function () {
                $(this).removeData("bs.modal");
                $(this).find('.modal-content').empty();
            });

            $("#modal-lead-crmnotes").on("hidden.bs.modal", function () {
                $(this).removeData("bs.modal");
                $(this).find('.modal-content').empty();
            });

            
            $("#modal-lead-delete").on("loaded.bs.modal", function (e) {
                var modal = $(this);
                new Global.FormHelper(modal.find("form"), { updateTargetId: "error-ModalMessage" }, null, function (result) {
                    if (result.isSuccess) {
                        modal.modal('hide');
                        window.location.href = result.redirectUrl;
                    }
                    else {
                        modal.find('#error-ModalMessage').attr('class', 'alert alert-danger').html(result.message || result.errorMessage).show();
                    }
                });
            }).on("hidden.bs.modal", function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });

            //$('#modal-createSelectClient').on('loaded.bs.modal', function (e) {
            //    var modal = $(this);
            //    new Global.FormHelper(modal.find('#createClient'), { updateTargetId: "error-ModalMessage" }, null, function (result) {
            //        if (result.isSuccess) {
            //            modal.modal('hide');
            //            window.location.href = result.redirectUrl;
            //        }
            //        else {
            //            modal.modal('hide');
            //            modal.find('#error-ModalMessage').attr('class', 'alert alert-danger').html(result.message || result.errorMessage).show();
            //        }
            //    });
            //    new Global.FormHelper($(this).find('#selectClient'), { updateTargetId: "error-ModalMessage" }, null, function (result) {
            //        if (result.isSuccess) {
            //            window.location.href = result.redirectUrl;
            //        }
            //        else {
            //            modal.find('#error-ModalMessage').attr('class', 'alert alert-danger').html(result.message || result.errorMessage).show();
            //        }
            //    });
            //}).on('hidden.bs.modal', function () {
            //    $(this).removeData('bs.modal');
            //    $(this).find('.modal-content').empty();
            //});


            $(document).off('click', '.clsViewNote').on('click', '.clsViewNote', function () {
                $('#hdnSelectedNote').val($(this).attr('data-id'));
                /*   setTimeout(function () {*/


                var recipient = domain + "estimate/LeadNotes/" + parseInt($('#hdnSelectedNote').val()); // Extract info from data-* attributes

                $('#modal-lead-crmnotes .modal-content').load(recipient, function () {

                });
                /*  }, 500)*/

            });

          



            $("#exportToExcel").click(function (event) {
                event.preventDefault();
                var url = $(this).prop("href");
                var params = $this.estimateRequestParam(false, false);
                console.log(params)
                Global.DynamicFormPost(url, params, true);
                return false;
            });
        };

        $this.init = function () {
            //bind events at page load
            $this.bindInitialEvents();

            //bind Estimate Grid details
            $this.bindEstimateGrid(true, true);

            $this.getLeadSummary(true, true);
        };
    }
    $(function () {
        var self = new ProjectLead();
        self.init();
        $.fn.EstimateIndex = self;
    });
})(jQuery);