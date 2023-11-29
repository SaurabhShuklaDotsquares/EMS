(function ($) {
    function index() {
        var $this = this, grid;
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
                Technologies : $('#technologies').val()
            };

            return data;
        }
        $('.searchfilterpm').on('change', function () {            
            GetDeparmentwiseUsers();
            
            if ($('#user') && $('#user').length > 0) {
                $('#user').val("0");
            }
            loadGrid();
        });
        $('.searchfilterdepartment').on('change', function () {            
            GetDeparmentwiseUsers();
            if ($('#user') && $('#user').length > 0) {
                $('#user').val("0");
            }
            loadGrid();
        });
        $('.searchfilter').on('change', function () {
            loadGrid();
        });
        $('.select2Industries').on('change', function () {
            loadGrid();
        });
        $('.select2Technologies').on('change', function () {
            loadGrid();
        });
        $('.select2Experience').on('change', function () {
            loadGrid();
        });
        function GetProjectManagerUsers() {
            var pmid = $("#pm").val();
            if (pmid == '')
                pmid = 0;
            var emp = $("#user");
            $.ajax
                ({
                    url: domain + 'CVBuilder/GetEmployeesByPM',
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
                    url: domain + 'CVBuilder/GetEmployeesByDepartment',
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
            $('.divoverlay').removeClass('hide');

            grid = new Global.GridHelper('#grid-document-table', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "ordering": false,
                "bFilter": false,
                "bAutoWidth": false,
                "bLengthChange": false,
                ajax:
                {
                    url: domain + "CVBuilder/index",
                    type: "Post",
                    data: getfilter()
                },
                "columnDefs": [
                    { "width": "5%", "targets": 0 },
                    { "width": "8%", "targets": 1 },
                    { "width": "10%", "targets": 2 },
                    { "width": "8%", "targets": 3 },
                    { "width": "8%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                    { "width": "9%", "targets": 7 },
                ],
                columns:
                    [
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                        { name: "name", data: "name", title: "Name", sortable: false, searchable: false, visible: true },                        
                        { name: "email", data: "email", title: "Email", sortable: false, searchable: false, visible: true },                        
                        { name: "phone", data: "phone", title: "Phone", sortable: false, searchable: false, visible: true },                        
                        { name: "experienceType", data: "experienceType", title: "Experience", sortable: false, searchable: false, visible: true },                        
                        { name: "industry", data: "industry", title: "Industry", sortable: false, searchable: false, visible: true },                        
                        { name: "technology", data: "technology", title: "Technology", sortable: false, searchable: false, visible: true },                        
                        //{
                        //    name: "Title", data: null, title: "About me", sortable: false, searchable: false,
                        //    render: function (data, type, dataRow, meta) {
                        //        var count = data.title.replace(/<[^>]*>|\s/g, '').length;
                        //        var returnTable = "";
                        //        if (count > 99) {
                        //            returnTable = '<table class="table-responsive"><tr>';
                        //            returnTable += '<td width="10%;"> <div style="max-height: 100px;overflow-x: hidden; overflow-y: auto;">' + data.title + '</div>';
                        //            returnTable += '<div class="pull-right divmoreinner"></div>';
                        //            //returnTable += '<div class="pull-right divmoreinner"><a  title="Read More" href="ProjectReview/ReviewerCommentReadMore/' + data.id + '" data-toggle="modal" data-target="#modal-commentreadmore" data-backdrop="static" style="color: red;margin-right:24px;margin-top:3px;"><span>Read More</span></a></div>';
                        //            returnTable += '</td>';
                        //            returnTable += '</tr></table>';
                        //        }
                        //        else {
                        //            returnTable = "<span>" + data.title + "</span>";
                        //        }
                        //        return returnTable;
                        //    }
                        //},                         
                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {
                                var actionButtons = "";
                                if (data.isEdit) {
                                    actionButtons += $("<a/>", {
                                        id: "editevent",
                                        title: "Edit",
                                        'class': "btn btn-default btn-sm",
                                        href: domain + "cvbuilder/add/" + data.id,
                                        html: "Edit", 
                                    }).get(0).outerHTML + "&nbsp; ";
                                }
                                actionButtons += $("<a/>", {
                                    id: "viewDetail",
                                    title: "Preview",
                                    href: domain + "cvbuilder/view/" + dataRow.id,
                                    'data-toggle': "modal",
                                    'data-target': "#modal-view-cvdata",
                                    'data-backdrop': "static",
                                    'class': "btn btn-default btn-sm",
                                    html: "Preview",
                                }).get(0).outerHTML + "&nbsp; ";
                                actionButtons += $("<a/>", {
                                    id: "comment",
                                    title: "Download",
                                    'class': "btn btn-default btn-sm",
                                    //'data-toggle': "modal",
                                    //'data-target': "#modal-reviewer-addcomment",
                                    href: domain + "cvbuilder/Print/" + data.id,
                                    html: "Download",
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
                
                try {
                    var headers = new Headers();
                    
                    headers.append('Accept', 'application/zip'); // This one is enough for GET requests
                    headers.append('Content-Type', 'application/json'); // This one sends body
                    const request = await fetch(domain + "CVBuilder/download", {
                        method: 'Post',
                        //type: 'application/zip',
                        headers: headers,
                        body: JSON.stringify({
                            Uid_User: $("#user").val(),
                            pm: $("#pm").val(),
                            department: $("#department").val(),
                            ExperienceType: $("#Experience").val(),
                            DomainType: $("#DomainType").val(),
                            Technology: $("#Technology").val(),
                            Domains: $('#industries').val(),
                            Technologies: $('#technologies').val() }),
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
                }
                catch (e) {
                    console.log(`error occurred → ${e}`);
                }
                //$.ajax({                    
                //    url: domain + "CVBuilder/download",
                //    type: "Post",
                //    data: getfilter(),
                //    success: function (data) {
                        
                //    }
                //});
            });
        }

        $this.init = function () {
            $("#modal-view-cvdata").on('loaded.bs.modal', function (e) {
            }).on('hidden.bs.modal', function (e) {
                $(this).removeData('bs.modal');
            });
            intializeModalWithForm();
            $('.select2Industries').fSelect({ placeholder: "Select Industry" });
            $('.select2Technologies').fSelect({ placeholder: "Select Technology" });
            $('.select2Experience').fSelect({ placeholder: "Select Experience" });
            loadGrid();
        };
    }

    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));