(function ($) {

    function Lead() {
        $this = this;

        $('#btnSearch').click(function () {
            LoadEstimateDocGrid();
        })

        $('#wireframe-Image-gallery').on('click', '#myPager a', function (e) {
            e.preventDefault();
            $.ajax({
                type: "GET",
                url: this.href,
                chase: false,
                success: function (result) {
                    $('#wireframe-Image-gallery').html(result);
                    $('.colorboxGallery').colorbox({ width: "60%", height: "80%" });
                    var maxHeight = Math.max.apply(null, $('.wireframe-item').map(function () {
                        return $(this).innerHeight() + 10;
                    }).get());
                    $('.wireframe-item').css('height', maxHeight);
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');

                }
            })
        })

        $('#DocType').change(function () {
            if ($(this).val() == "WM") {
                $('#div-grid-EstimateDoc').css("display", "none");
                $('#wireframe-Image-gallery').css("display", "");
                $.ajax({
                    type: "GET",
                    url: domain + "Lead/GetWireframeMockupImages",
                    success: function (result) {
                        $('#wireframe-Image-gallery').html(result);
                        $('.colorboxGallery').colorbox({ width: "60%", height: "80%" });
                        var maxHeight = Math.max.apply(null, $('.wireframe-item').map(function () {
                            return $(this).innerHeight() + 10;
                        }).get());
                        $('.wireframe-item').css('height', maxHeight);
                        $('.pagination .active a').css('background-color', '#e99701');
                        $('.pagination .active a').css('border-color', '#e99701');
                    }
                })


            }
            else {
                $('#div-grid-EstimateDoc').css("display", "");
                $('#wireframe-Image-gallery').css("display", "none");
                LoadEstimateDocGrid();
            }
        })
        $('#drpSpam').change(function () {
            LoadEstimateDocGrid();
        })
        //Method to Load EstimateDocument Grid
        function LoadEstimateDocGrid() {
            var dsPhoto = false;
            if ($('#chkIsDSPhoto').is(":checked")) {
                dsPhoto = true;
            }
            var data1 = { txtSearch: $('#txtSearch').val(), doctype: $('#DocType').val(), spam: $('#drpSpam').val(), leadType: $('#LeadType').val(), isDSPhoto: dsPhoto }
            var EstimateDocGrid = new Global.GridHelper('#grid-EstimateDoc', {
                serverSide: true,
                destroy: true,
                "bAutoWidth": false,
                "pageLength": 50,
                "bFilter": false,
                ajax:
                {
                    url: domain + "Lead/EstimateDocuments",
                    type: "POST",
                    data: data1,
                },
                order: [1, "desc"],
                "columnDefs": [
                    { "width": "0%", "targets": 0 },
                    { "width": "3%", "targets": 1 },

                    { "width": "13%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "10%", "targets": 4 },

                    { "width": "13%", "targets": 5 },
                    { "width": "14%", "targets": 6 },
                    { "width": "13%", "targets": 7 },
                    { "width": "13%", "targets": 8 },

                    { "width": "8%", "targets": 9 },
                    { "width": "3%", "targets": 10 },
                ],
                columns:
                    [
                        { name: "id", data: "id", title: "id", sortable: false, searchable: false, visible: false },

                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },

                        {
                            name: "title", data: "title", title: "title", sortable: true, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {

                                if (dataRow.leadId != "") {
                                    return dataRow.title + "</br><b>LeadID:<a target='_blank' style='text-decoration:underline;' href=" + domain + "User/leads/edit.aspx?leadid=" + dataRow.leadId + ">" + dataRow.leadId + "</a></b>";
                                }
                                else {
                                    return dataRow.title;
                                }

                            }
                        },
                        { name: "tags", data: "tags", title: "tags", sortable: true, searchable: false, visible: true },

                        {
                            name: "industry", data: "industry", title: "industry/ technology", sortable: true, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var TechNIndustry = "";
                                if (dataRow.industry != "") {
                                    TechNIndustry += dataRow.industry;
                                }
                                if (dataRow.technology != null) {
                                    if (TechNIndustry != "") {
                                        TechNIndustry += " / " + dataRow.technology;
                                    }
                                    else {
                                        TechNIndustry += dataRow.technology;
                                    }
                                }
                                return TechNIndustry;
                            }
                        },


                        {
                            name: "documentPath", data: null, title: "Proposal Document", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var Link = "";

                                if (dataRow.documentPath != null) {
                                    Link += '<a  style="color:#101ee5;text-decoration:underline;" href="Upload/EstimateDocument/' + data.documentPath + '" target="_blank">' + data.documentPath + '</a></br>';
                                }
                                if (dataRow.estimateTimeInDays != "") {
                                    Link += "</br> <b>Estimated Time: </b>" + dataRow.estimateTimeInDays;
                                }


                                return Link

                            }
                        },
                        {
                            name: "wireFrame/Mockups", data: null, title: "Wireframes/ Mockups All", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var mockups = "";
                                if (dataRow.wireframe_MockupsDoc != null && dataRow.wireframe_MockupsDoc != "") {
                                    mockups += '<a class="colorbox" title="Wireframe/Mockups(Images & Designs)"  href="Upload/EstimateDocument/' + data.wireframe_MockupsDoc + '"><img  src="' + domain + 'Upload/EstimateDocument/' + dataRow.wireframe_MockupsDoc + '" width="120" height="100"></a></br>';
                                }
                                if (dataRow.mockupDocument != null) {
                                    mockups += '<a  style="color:#101ee5;text-decoration:underline;" href="Upload/EstimateDocument/' + data.mockupDocument + '" target="_blank">' + data.mockupDocument + '</a></br>';
                                }
                                return mockups;
                            }

                        },
                        {
                            name: "otherDocument", data: null, title: "Other Document", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var other = "";
                                if (dataRow.otherDocument != null) {
                                    other += '<a  style="color:#101ee5;text-decoration:underline;" href="Upload/EstimateDocument/' + data.otherDocument + '" target="_blank">' + data.otherDocument + '</a></br>'
                                }
                                return other;
                            }

                        },
                        {
                            name: "flowcharts", data: null, title: "Flowchart Document", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {

                                var flowchart = "";
                                if (dataRow.flowcharts != null) {
                                    flowchart += '<a  style="color:#101ee5;text-decoration:underline;" href="Upload/EstimateDocument/' + data.flowcharts + '" target="_blank">' + data.flowcharts + '</a></br>'
                                }

                                return flowchart;
                            }

                        },
                        {
                            name: "modified", data: null, title: "modified On", sortable: true, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                return dataRow.modified + "</br> <b>uploaded By</b>: " + dataRow.uploadedBy;
                            }
                        },
                        {
                            name: "action", data: null, title: "action", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = "";
                                actionButtons += '<a data-toggle="modal" data-target="#modal-AddEditEstimateDocument" class="fa fa-edit" href="' + domain + 'lead/UploadDocument/' + dataRow.id + '"></a>&nbsp;&nbsp;';
                                if (data.isPM) {
                                    //actionButtons += '<a class="fa fa-trash" href="' + domain + 'lead/DeleteDoc/' + dataRow.id + '"></a>';
                                    actionButtons += $("<a/>", {
                                        href: domain + "lead/Delete/" + dataRow.id ,
                                        id: "deleteEstimateDocument",
                                        title: "Delete",
                                        'data-toggle': "modal",
                                        'data-target': "#modal-EstimateDocumentDelete",
                                        html: $("<i/>", {
                                            class: "fa fa-trash",
                                            style: "color:black"
                                        }),
                                    }).get(0).outerHTML;
                                }
                                return actionButtons;
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
                    $('.colorbox').colorbox({ width: "60%", height: "80%" });
                }
            })
            return EstimateDocGrid;
        }

        //Method to delete document using ajax
        function DeleteDocument(item) {
            var elementId = item;
            if (confirm("Are you sure to delete this document ?")) {
                var data1 = { estimateId: $('#EstimateDocumentId').val(), documentId: elementId.attr('id') }

                $.ajax({
                    type: "POST",
                    url: domain + "Lead/DeleteDocument/",
                    data: data1,
                    success: function (result) {
                        if (result.isSuccess) {

                            var messageBox = result.updateTargetId;
                            var message = result.message;
                            elementId.closest('.divUploadedFile').html('');
                            $('#' + messageBox).attr('class', 'alert alert-success');
                            $('#' + messageBox).html(message);
                            $('#' + messageBox).show()
                        }
                        else {
                            var messageBox = result.updateTargetId;
                            var message = result.message;
                            $('#' + messageBox).attr('class', 'alert alert-danger');
                            $('#' + messageBox).html(message);
                            $('#' + messageBox).show()
                        }
                    }

                })
            }
            return false;
        }
        function Delete(item) {
            var elementId = item;
            if (confirm("Are you sure to delete this document ?")) {
                var data1 = { id: elementId }

                $.ajax({
                    type: "POST",
                    url: domain + "Lead/Delete/",
                    data: data1,
                    success: function (result) {
                        if (result.isSuccess) {
                            window.location.href = result.redirectUrl;
                            //var messageBox = result.updateTargetId;
                            //var message = result.message;
                            //elementId.closest('.estimateDocument-page').html('');
                            //$('#' + messageBox).attr('class', 'alert alert-success');
                            //$('#' + messageBox).html(message);
                            //$('#' + messageBox).show()
                            
                        }
                        else {
                            var messageBox = result.updateTargetId;
                            var message = result.message;
                            $('#' + messageBox).attr('class', 'alert alert-danger');
                            $('#' + messageBox).html(message);
                            $('#' + messageBox).show()
                        }
                    }

                })
            }
            return false;
        }
        $('.DeleteWMDoc').on('click', function () {
            //var data = $(this);
            var delId = $(this).attr("data-attr");
            Delete(delId);
        });

        //$('.DownloadWMDoc').on('click', function () {
        //    //debugger;
        //    var file = $(this).attr("data-attr");
        //    return file;

        //});

        function InitializeModal() {
            $('#modal-AddEditEstimateDocument').on('loaded.bs.modal', function (e) {
                new Global.FormHelperWithFiles($('#uploadDocumentForm'), { updateTargetId: "error-ModalMessage" });
                $('#EstimateTimeinDays').keypress(function (evt) {
                    evt = (evt) ? evt : window.event;
                    var charCode = (evt.which) ? evt.which : evt.keyCode;
                    if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                        return false;
                    }

                    return true;
                })
                $('#lnkDelProposalDoc').click(function () {
                    var lnkDel = $(this);
                    DeleteDocument(lnkDel);
                })
                $('#lnkDelWireframeMockupDoc').click(function () {
                    var lnkDel = $(this);
                    DeleteDocument(lnkDel);
                })
                $('#lnkDelOtherDoc').click(function () {
                    var lnkDel = $(this);
                    DeleteDocument(lnkDel);
                })
                $('#lnkDelMockupDoc').click(function () {
                    var lnkDel = $(this);
                    DeleteDocument(lnkDel);
                })
                $('#lnkDelFlowcharts').click(function () {
                    var lnkDel = $(this);
                    DeleteDocument(lnkDel);
                })
            }).on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                LoadEstimateDocGrid();
            })
            $("#modal-EstimateDocumentDelete").on("loaded.bs.modal", function (e) {
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
        }

        $this.init = function () {

            InitializeModal()
            LoadEstimateDocGrid();

        }
    }
    $(function () {
        var self = new Lead();
        self.init();
    })

})(jQuery);