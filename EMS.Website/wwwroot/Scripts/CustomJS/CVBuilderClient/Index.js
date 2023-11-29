(function ($) {
    function index() {
        var $this = this, grid;
        var ckStatusType = true;
        var ckTraining = false;
        var ckPMReview = false;
        function getfilter() {
           
            var data = {                
                //DateFrom: $('#StartDate').val(),
                //DateTo: $('#EndDate').val(),
                Uid_User: $("#user").val(),
                pm: $("#pm").val(),
                department: $("#department").val(),
                ExperienceType: $("#Experience").val(),
                DomainType: $("#DomainType").val(),
                Technology: $("#Technology").val(),
                Domains: $('#industries').val(),
                Technologies: $('#technologies').val(),
                SpecType: $('#SpecType').val(),
                EmpStatusType: $('#EmpStatusType').val(),
                EmpStatusTypeCheck: ckStatusType,
                TechnologyrdAnd: isAllowed > 0 ? document.getElementById('rdAnd').checked : true,
                TrainingCheck: ckTraining,
                PMReviewCheck: ckPMReview
            };

            return data;
        }
        $('.searchfilterpm').on('change', function () {            
            GetDeparmentwiseUsers();
            
            if ($('#user') && $('#user').length > 0) {
                $('#user').val("0");
            }
            //loadGrid();
        });
        $('.searchfilterdepartment').on('change', function () {            
            GetDeparmentwiseUsers();
            if ($('#user') && $('#user').length > 0) {
                $('#user').val("0");
            }
            //loadGrid();
        });
        $('.searchfilter').on('change', function () {
            //loadGrid();
        });
        $('.select2Industries').on('change', function () {
            //loadGrid();
        });
        $('.select2Technologies').on('change', function () {
            //loadGrid();
        });
        $('.select2Experience').on('change', function () {
            //loadGrid();
        });
        $('#EmpStatusTypeCheck').on('change', function (event) {            
            if (event.currentTarget.checked) {                
                ckStatusType = true;
            } else {
                ckStatusType = false;
            }
        });
        $('#TrainingCheck').on('change', function (event) {
            if (event.currentTarget.checked) {
                ckTraining = true;
            } else {
                ckTraining = false;
            }
        });
        $('#PMReviewCheck').on('change', function (event) {
            if (event.currentTarget.checked) {
                ckPMReview = true;
            } else {
                ckPMReview = false;
            }
        });
        function GetProjectManagerUsers() {
            var pmid = $("#pm").val();
            if (pmid == '')
                pmid = 0;
            var emp = $("#user");
            $.ajax
                ({
                    url: domain + 'clientcv/GetEmployeesByPM',
                    type: 'POST',
                    data: { pmid: pmid },
                    
                    success: function (result) {
                        emp.empty().append('<option value="">All Employee</option>');
                        $.each(result, function () {
                            emp.append($("<option></option>").val(this['value']).html(this['text']));
                        });
                    },
                    error: function (ex) {
                        alert("Whooaaa! Something went wrong.." + ex);
                    },
                });
        }
        function GetDeparmentwiseUsers() {
            var pmid = $("#pm").val();
            if (pmid == '')
                pmid = 0;
            var departmentid = $("#department").val();
            if (departmentid == '')
                departmentid = 0;
            var emp = $("#user");
            $.ajax
                ({
                    url: domain + 'clientcv/GetEmployeesByDepartment',
                    type: 'POST',
                    data: { pmid: pmid, departmentId: departmentid },

                    success: function (result) {
                        emp.empty().append('<option value="">All Employee</option>');
                        $.each(result, function () {
                            emp.append($("<option></option>").val(this['value']).html(this['text']));
                        });
                    },
                    error: function (ex) {
                        alert("Whooaaa! Something went wrong.." + ex);
                    },
                });
        }
        
        function loadGrid() {
            //$('.divoverlay').removeClass('hide');
            $('.loading-common,.loading-overlay').show()
            grid = new Global.GridHelper('#grid-document-table', {
                serverSide: true,
                destroy: true,
                //"pageLength": 50,
                "ordering": false,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                "bPaginate": false,
                ajax:
                {
                    url: domain + "clientcv/index",
                    type: "Post",
                    data: getfilter()
                },
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "8%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "5%", "targets": 3 },
                    { "width": "7%", "targets": 4 },
                    { "width": "12%", "targets": 5 },
                    { "width": "12%", "targets": 6 },
                    { "width": "7%", "targets": 7, "className": "text-right" },
                    { "width": "5%", "targets": 8 },
                    { "width": "5%", "targets": 9 },
                    { "width": "11%", "targets": 10 },
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "name", data: "name", title: "Name", sortable: false, searchable: false, visible: true },                        
                        { name: "email", data: "email", title: "Email", sortable: false, searchable: false, visible: true },                        
                        { name: "phone", data: "phone", title: "Phone", sortable: false, searchable: false, visible: true },                        
                        { name: "experienceType", data: "experienceType", title: "Experience", sortable: false, searchable: false, visible: true },                        
                        { name: "industry", data: "industry", title: "Industry / Domain", sortable: false, searchable: false, visible: true },                        
                        { name: "technology", data: "technology", title: "Technologies", sortable: false, searchable: false, visible: true },
                        { name: "estimateRate", data: "estimateRate", title: "COST (Per Hour)", sortable: false, searchable: false, visible: false },                        
                        {
                            name: "isApproved", data: null, title: "Training", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '';
                                if (data.isPM) {
                                    actionButtons = '<center>';

                                    actionButtons += '<div class="chk-box dis-block clearfix">';
                                    if (data.isApproved) {
                                        actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + data.encryptId + '" checked/><span class="slider round"></span></label>';
                                        actionButtons += '<br/><br/><span class="label label-success">Done</span>';
                                    }
                                    else {
                                        actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox" id="isApproved" name="isApproved" value="' + data.encryptId + '" /><span class="slider round"></span></label>';
                                        actionButtons += '<br/><br/><span class="label label-danger">Pending</span>';
                                    }
                                    actionButtons += '<label for=isApproved"></label>'
                                    actionButtons += '</div>&nbsp;&nbsp;'
                                }
                                else {
                                    actionButtons = '<center>';

                                    actionButtons += '<div class="chk-box dis-block clearfix">';
                                    if (data.isApproved) {
                                        //actionButtons += '<label class="switch" style="pointer-events: none;"><input type="checkbox" title="Approved" class="" id="isApproved" name="isApproved" value="" checked/><span class="slider round"></span></label>';
                                        actionButtons += '<span class="label label-success">Done</span>';
                                    }
                                    else {
                                        //actionButtons += '<label class="switch" style="pointer-events: none;"><input type="checkbox" title="Approved" class="" id="isApproved" name="isApproved" value="" /><span class="slider round"></span></label>';
                                        actionButtons += '<span class="label label-danger">Pending</span>';
                                    }                                    
                                    actionButtons += '</div>&nbsp;&nbsp;'
                                }
                                return actionButtons;
                            }
                        },
                        {
                            name: "pMApproved", data: null, title: "PM Review", sortable: false, searchable: false,
                            render: function (data, type, row, meta) {
                                var actionButtons = '';
                                if (data.isPM) {
                                    actionButtons = '<center>';

                                    actionButtons += '<div class="chk-box dis-block clearfix">';
                                    if (data.pmApproved) {
                                        actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox2" id="isApproved" name="isApproved" value="' + data.encryptId + '" checked/><span class="slider round"></span></label>';
                                        actionButtons += '<br/><br/><span class="label label-success">Done</span>';
                                    }
                                    else {
                                        actionButtons += '<label class="switch"><input type="checkbox" title="Approved" class="switchBox2" id="isApproved" name="isApproved" value="' + data.encryptId + '" /><span class="slider round"></span></label>';
                                        actionButtons += '<br/><br/><span class="label label-danger">Pending</span>';
                                    }
                                    actionButtons += '<label for=isApproved"></label>'
                                    actionButtons += '</div>&nbsp;&nbsp;'
                                }
                                else {
                                    actionButtons = '<center>';

                                    actionButtons += '<div class="chk-box dis-block clearfix">';
                                    if (data.pmApproved) {
                                        //actionButtons += '<label class="switch" style="pointer-events: none;"><input type="checkbox" title="Approved" class="" id="isApproved" name="isApproved" value="" checked/><span class="slider round"></span></label>';
                                        actionButtons += '<span class="label label-success">Done</span>';
                                    }
                                    else {
                                        //actionButtons += '<label class="switch" style="pointer-events: none;"><input type="checkbox" title="Approved" class="" id="isApproved" name="isApproved" value="" /><span class="slider round"></span></label>';
                                        actionButtons += '<span class="label label-danger">Pending</span>';
                                    }
                                    
                                    actionButtons += '</div>&nbsp;&nbsp;'
                                }
                                return actionButtons;
                            }
                        },
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {                                
                                //get
                                var templateid = localStorage.getItem("templateType") == null ? 2 : localStorage.getItem("templateType");
                                var actionButtons = "";                                
                                actionButtons += $("<a/>", {
                                    id: "viewDetail",
                                    title: "View",
                                    href: domain + "clientcv/view?id=" + dataRow.encryptId + "&&tpId=" + templateid,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-view-cvdata",
                                    'data-backdrop': "static",
                                    'class': "btn btn-default btn-sm",
                                    html: "View",
                                }).get(0).outerHTML + "&nbsp; ";
                                if (data.isEdit) {
                                    actionButtons += $("<a/>", {
                                        id: "editevent",
                                        title: "Edit",
                                        'class': "btn btn-default btn-sm",
                                        href: domain + "clientcv/add?id=" + data.encryptId,
                                        html: "Edit",
                                    }).get(0).outerHTML + "&nbsp; ";
                                }
                                actionButtons += $("<a/>", {
                                    id: "Download",
                                    title: "Download",
                                    'class': "btn btn-default btn-sm btndownload",
                                    href: domain + "clientcv/Print?id=" + data.encryptId + "&&tpId=" + templateid,
                                    html: "Download",
                                }).get(0).outerHTML + "&nbsp; ";                                
                                
                                return actionButtons;
                            }
                        },
                    ],
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {

                },
                "fnDrawCallback": function (oSettings) {
                    //$('.divoverlay').addClass('hide');
                    $('.loading-common,.loading-overlay').hide()
                    $('.switchBox').on('change', function () {                        
                        var switchElement = this;
                        $.get(domain + 'clientcv/UpdateApproved', {
                            id: this.value
                        });
                        location.reload();
                    });
                    $('.switchBox2').on('change', function () {
                        var switchElement = this;
                        $.get(domain + 'clientcv/ApprovedPM', {
                            id: this.value
                        });
                        location.reload();
                    });
                    $('.btndownload').on('click', function () {
                        $('.loading-common,.loading-overlay').show();
                        setTimeout(function () {
                            $('.loading-common,.loading-overlay').hide();
                        }, 7000);
                    });
                    if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                        $('.dataTables_paginate').hide();
                    }
                    else {
                        $('.dataTables_paginate').show();
                    }
                    
                    var table = $('#grid-document-table').DataTable();                                       
                    if (isAllowed) {
                        table.column(7).visible(true);    // To show
                    }
                    else {
                        table.column(7).visible(false);   // To hide
                    }
                    $('.pagination .active a').css('background-color', '#e99701');
                    $('.pagination .active a').css('border-color', '#e99701');
                    $('#grid-document-table tr:eq(0) th:eq(7)').css('text-transform', 'none');
                }
            });
        }


        function intializeModalWithForm() {
            $("#StartDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("#EndDate").datepicker("option", "minDate", selectedDate);

                }
            });

            $("#EndDate").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                onClose: function (selectedDate) {
                    $("#StartDate").datepicker("option", "maxDate", selectedDate);
                }
            });

            $("#btnSearch").on("click", function () {
                loadGrid();
            });
            $("#btnReset").on("click", function () {
                //$("#StartDate").val('');
                //$('#EndDate').val('');                
                $("#user").val('');
                $("#pm").val('');
                $("#department").val('');
                //$("#Experience").val('');  
                $("#DomainType").val('');
                $("#Technology").val('');
                location.reload();
            });
            $("#btnDownload").on("click", async function () {
                $("#ProgressBar_Status").show();
                var templateid = localStorage.getItem("templateType") == null ? 2 : localStorage.getItem("templateType");
                try {
                    var headers = new Headers();
                    
                    headers.append('Accept', 'application/zip'); // This one is enough for GET requests
                    headers.append('Content-Type', 'application/json'); // This one sends body
                    const request = await fetch(domain + "clientcv/download", {
                        method: 'Post',
                        //type: 'application/zip',
                        headers: headers,
                        body: JSON.stringify({                            
                            TemplateId: templateid,
                            Uid_User: $("#user").val(),
                            pm: $("#pm").val(),
                            department: $("#department").val(),
                            ExperienceType: $("#Experience").val(),
                            DomainType: $("#DomainType").val(),
                            Technology: $("#Technology").val(),
                            Domains: $('#industries').val(),
                            Technologies: $('#technologies').val(),
                            SpecType: $('#SpecType').val(),
                            EmpStatusType: $('#EmpStatusType').val(),
                            EmpStatusTypeCheck: ckStatusType,
                            TechnologyrdAnd: isAllowed > 0 ? document.getElementById('rdAnd').checked : true,
                            TrainingCheck: ckTraining,
                            PMReviewCheck: ckPMReview
                        }),
                    });
                    const file = await request.blob();
                    //const data = await request.json();
                    let tempUrl = URL.createObjectURL(file);
                    const aTag = document.createElement("a");
                    aTag.href = tempUrl;
                    aTag.download = 'CV-Download.zip';
                    document.body.appendChild(aTag);
                    aTag.click();
                    URL.revokeObjectURL(tempUrl);
                    aTag.remove();
                    $("#ProgressBar_Status").hide();
                }
                catch (e) {
                    console.log(`error occurred → ${e}`);
                }
                
            });
           
        }

        $this.init = function () {
            $("#modal-view-cvdata").on('loaded.bs.modal', function (e) {
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
            intializeModalWithForm();
            $('.select2Industries').fSelect({ placeholder: "Select Industry / Domain" });
            $('.select2Technologies').fSelect({ placeholder: "Select Technology" });
            $('.select2Experience').fSelect({ placeholder: "Select Year(s) of Experience" });
            
            if (isAllowed) {
                document.getElementById('EmpStatusTypeCheck').checked = true;
                document.getElementById('rdAnd').checked = true;
            }
            if (localStorage.getItem("templateType") == null) {
                document.getElementById('rdTemplate2').checked = true;
            }
            if (localStorage.getItem("templateType") == 1) {
                document.getElementById('rdTemplate1').checked = true;
            }
            if (localStorage.getItem("templateType") == 2) {
                document.getElementById('rdTemplate2').checked = true;
            }
            
            loadGrid();
        };
    }

    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));