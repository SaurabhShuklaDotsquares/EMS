(function () {
    function Project() {
        $this = this;
        var viewModel;
        var isSuperAdmin = $('#hdnIsSuperAdmin').val() == "True" ? true : false;
        var isHr = $('#hdnIsHr').val() == "True" ? true : false;

        function addViewModel() {
            var self = this;
            self.VirtualDevelopers = ko.observableArray([]);
            self.ActualDevelopers = ko.observableArray([]);
            self.ProjectDevelopers = ko.observableArray([]);
            self.StatusList = ko.observableArray([]);
            self.addProjectDevelopers = function (count) {
                if (count != "") {
                    self.ProjectDevelopers.push(new PackageProjectDevelopers(0, 0));
                }
                else {
                    self.ProjectDevelopers.removeAll();
                }
            };
            self.removeProjectDeveloper = function (data) {
                if (self.ProjectDevelopers().length > 1) {
                    self.ProjectDevelopers.remove(data);
                }
            }

            self.setProjectDevelopers = function (data) {                
                for (var p = 0; p < data.length; ++p) {                    
                    var projectDeveloper = new PackageProjectDevelopers(data[p].uid, data[p].virtualDeveloperID, data[p].remark, data[p].status);
                    self.ProjectDevelopers.push(projectDeveloper);
                }
            }
            self.saveProjectDeveloper = function () {
                var form = $('#addEditDeveloperForm');
                form.validate();
                if (form.valid()) {
                    var ProjectId = $('#hdnProjectId').val();
                    var projectDeveloperMappingList1 = ko.mapping.toJS(self.ProjectDevelopers); 
                    console.log(projectDeveloperMappingList1);
                    Model= { projectDeveloperMappingList: projectDeveloperMappingList1 };                   
                    $.ajax({
                        url: domain + "Project/AddEditProjectDeveloper?ProjectId=" + ProjectId,
                        type: 'POST',
                        contentType: 'application/json',
                        data: ko.toJSON(projectDeveloperMappingList1),
                        success: function (result) {
                            if (result.isSuccess) {
                                window.location.href = result.redirectUrl;
                            } else {
                                if (settings.updateTargetId) {
                                    $("#" + settings.updateTargetId).html(result);
                                }
                            }
                        }
                    });
                }
                else {
                    return false;
                }
            }
        }
        function PackageProjectDevelopers(Uid, VirtualDeveloperID, Remark, Status) {           
            var self = this;
            self.Uid = ko.observable(Uid);
            self.VirtualDeveloperID = ko.observable(VirtualDeveloperID);
            self.Remark = ko.observable(Remark);
            self.Status = ko.observable(Status);
        };

        $('#btn_search').click(function () {
            LoadProjectGrid();
        })

        $("#StartDate").datepicker({
            dateFormat: "dd/mm/yy",
            changeMonth: true,
            changeYear: true,
            yearRange: "-100:+0"
        });

        $('#btn-Submit').click(function () {
            var total = $('.deptChkBox').find('input[name=Department]:checked').length;
            if (total <= 0) {
                alert("Atleast 1 department must be selected");
                return false;
            }
            var startDate = $('#StartDate').val();
            if (!ValidateDate(startDate)) {
                alert("Invalid start date");
                $('#StartDate').focus();
                return false;
            }
        })

        $('#CRMId').change(function () {
            //console.log($('#IsInHouseOld').val());
            var inHouse = $('#IsInHouse');
            //console.log(inHouse.val());
            if ($("#ProjectId").val() == '0') {
                $(this).val('0');
                $(this).attr("disabled", true);
            }
            if ($('#IsInHouseOld').val() == 'False') {
                if ($(this).val() > 0) {
                    inHouse.attr("disabled", false);
                } else {
                    inHouse.attr("disabled", true);
                    inHouse.prop("checked", true);
                }
            } else {
                inHouse.attr("disabled", true);
            }            
        })

        function ValidateDate(input) {
            var regexes = [
                /^(\d{1,2})\/(\d{1,2})\/(\d{4})$/,
                /^(\d{1,2})\-(\d{1,2})\-(\d{4})$/
            ];
            for (var i = 0; i < regexes.length; i++) {
                var r = regexes[i];
                if (!r.test(input)) {
                    continue;
                }
                var a = inputmatch(r), d = new Date(a[3], a[1] - 1, a[2]);
                if (d.getFullYear() != a[3] || d.getMonth() + 1 != a[1] || d.getDate() != a[2]) {
                    continue;
                }
                return true;


            }
            return false;
        }

        $('#ActualDevelopers').keypress(function (evt) {

            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;

        })

        $('#EstimatedDays').keypress(function (evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }

            return true;
        })
        $('#CRMId').keypress(function (evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }

            return true;
        })
        $('#ClientId').keypress(function (evt) {
            evt = (evt) ? evt : window.event;
            var charCode = (evt.which) ? evt.which : evt.keyCode;
            if (charCode > 31 && (charCode < 48 || charCode > 57)) {
                return false;
            }
            return true;
        })
        $('#txt_search').keypress(function (e) {
            if (e.keyCode == 13) {
                e.preventDefault();
                return false;
            }
        })
        $('#ddlPMList').change(function () {
            LoadProjectGrid();
        })

        $('#ddl_status').change(function () {
            LoadProjectGrid();
        })
        //$('#btnCRMProject').click(function () {

        //    var ddlPMList = $('#ddlPMList').val();
        //    if (ddlPMList != "") {
        //        window.location.href = domain + "Project/GetCRMProjectList?pmId=" + ddlPMList;
        //    }
        //    else {
        //        alert("PM must be selected to view pending CRM list");
        //        $('#ddlPMList').focus();
        //        return false;
        //    }

        //})
        function LoadProjectGrid() {
            var data1 = { ddl_status: $('#ddl_status').val(), txt_search: $('#txt_search').val(), ddlPMList: $('#ddlPMList').val() };
            var ProjectGrid = new Global.GridHelper('#grid-manageProject', {
                serverSide: true,
                destroy: true,
                "pageLength": 50,
                "bFilter": false,
                "bAutoWidth": false,
                ajax:
                    {
                        url: domain + "Project/Index",
                        type: "Post",
                        data: data1

                    },
                order: [[0, "desc"]],
                "columnDefs": [
                    { "width": "0%", "targets": 0 },
                    { "width": "5%", "targets": 1 },
                    { "width": "25%", "targets": 2 },
                    { "width": "10%", "targets": 3 },
                    { "width": "20%", "targets": 4 },
                    { "width": "10%", "targets": 5 },
                    { "width": "10%", "targets": 6 },
                    { "width": "7%", "targets": 7 }
                ],
                columns:
                    [
                        { name: "projectId", data: "projectId", title: "ProjectID", sortable: false, searchable: false, visible: false },
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },
                         { name: "name", data: "name", title: "name", sortable: false, searchable: false, visible: true },
                        { name: "clientId", data: "clientId", title: "CLient", sortable: false, searchable: false, visible: true },

                        { name: "model", data: "model", title: "model", sortable: false, searchable: false, visible: true },
                        { name: "actualDevelopers", data: "actualDevelopers", title: "Actual Developers", sortable: false, searchable: false, visible: true },
                        {
                            name: "status", data: "status", title: "status", sortable: false, searchable: false, visible: true
                        },

                        {
                            name: "Action", data: null, title: "Action", sortable: false, searchable: false,
                            render: function (data, type, dataRow, meta) {

                                return '<a title="edit project" class="fa fa-edit" style="font-size: 15px" href="' + domain + 'project/addeditproject/' + dataRow.projectId + '"></a> | <a data-toggle="modal" data-target="#modal-addEditDevforProject" href="' + domain + 'Project/AddEditProjectDeveloper?projectId=' + dataRow.projectId + '" ><img title="add/edit project developer" width="22" height="22" src="' + domain + 'Content/images/admin_add_user.png"/></a>';

                                //if (isSuperAdmin || isHr) {
                                //    return '<a title="edit project" class="fa fa-edit" style="font-size: 15px" href="' + domain + 'project/addeditproject/' + dataRow.projectId + '"></a> | <a data-toggle="modal" data-target="#modal-addEditDevforProject" href="' + domain + 'Project/AddEditProjectDeveloper?projectId=' + dataRow.projectId + '" ><img title="add/edit project developer" width="22" height="22" src="' + domain + 'Content/images/admin_add_user.png"/></a>';
                                //}
                                //else {
                                //    return '<a title="edit project" class="fa fa-edit" style="font-size: 15px" href="' + domain + 'project/addeditproject/' + dataRow.projectId + '"></a>';
                                //}
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
            })
            return ProjectGrid;
        }

        function InitializeForm() {
            new Global.FormValidationReset('#addEditProjectForm');


        }

        function InitializeModel() {

            $("#modal-addEditDevforProject").on('loaded.bs.modal', function (e) {
                viewModel = new addViewModel();
                ko.applyBindings(viewModel, $('#koProjectDeveloper')[0]);
                var projectId = $('#hdnProjectId').val();
                var PMid = $('#ddlPMList').val();
                $('#devModal-Process').css("display", "");
                var jqxhr = $.getJSON(domain + "project/GetVirtualDevelopers?projectId=" + projectId + "&PMUid=" + PMid, function (data) {
                    if (data != null) {

                        console.log(data);
                        viewModel.VirtualDevelopers = data.virtualDeveloperList;
                        viewModel.ActualDevelopers = data.developerList;
                        viewModel.StatusList = data.status;

                        viewModel.setProjectDevelopers(data.projectDeveloperMappingList);
                    }

                })
                jqxhr.complete(function () {
                    $('#devModal-Process').css("display", "none");
                })
                $('.AddNewProjectDeveloper').click(function () {
                    viewModel.setProjectDevelopers(1);
                });

            }).on("hidden.bs.modal", function (e) {
                $(this).removeData("bs.modal");
            });
        }

        $this.init = function () {
            InitializeForm();
            LoadProjectGrid();
            InitializeModel();
            $('#CRMId').trigger('change');

        }
    }
    $(function () {
        var self = new Project();
        self.init();
    })

}(jQuery))