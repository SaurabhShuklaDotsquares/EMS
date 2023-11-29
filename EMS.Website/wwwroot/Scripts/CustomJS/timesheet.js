
var app;
(function () {
    app = angular.module('myapp', []);

    app.filter('trustAsHtml', ['$sce', function ($sce) {

        return function (text) {

            return $sce.trustAsHtml(text);
        };

    }]);

    app.service('myService', function ($http) {

        this.GetTimeSheetResults = function (PageSize, CurrentPage) {
            var jsonObj = [];
            return $http.post(domain + 'timesheet/GetData', { pageSize: PageSize, pageNumber: CurrentPage, DateFrom: $('#txt_dateFrom').val(), DateTo: $('#txt_dateTo').val() })
                .success(function (data, status, headers, config) {
                    return jsonObj = data;
                });
        }


        this.GetDevelopers = function (projectId) {
            return $http.post(domain + 'timesheet/GetDeveloperByProject?projectId=' + projectId)
                .success(function (data, status, headers, config) {
                    return data;
                });
        }

        this.AddTimesheet = function (timeSheetModel) {
            return $http.post(domain + 'timesheet/AddTimesheetData', timeSheetModel)
                .success(function (data, status, headers, config) {
                    return data;
                });
        }


    });
    //End Service

    //Start Controller
    app.controller('MyController', function ($scope, $filter, myService) {

        $scope.tables = new Array();
        $scope.develoers = new Array();
        $scope.selectedProjectId = '0';
        $scope.developerId = '0';
        $scope.description = '';

        $scope.TotalRecords = 0;

        $scope.PageSize = 25;
        $scope.currentPage = 1;
        $('#pageid').val(1);
        $scope.pagedItems = 0;

        var date = new Date();
        $scope.addedDate = $filter('date')(new Date(), 'dd/MM/yyyy');
        $scope.time = '';
        $scope.description = '';

        $scope.userTimeSheetId = "0";
        $scope.editDate = $filter('date')(new Date(), 'dd/MM/yyyy');
        $scope.selectedEditProjectId = '0';
        $scope.editDeveloperId = '0';
        $scope.editTime = '';
        $scope.editDescription = '';
        $scope.UKPMVirtualDeveloperID = 0;

        GetTimeSheetResults();

        function GetTimeSheetResults() {

            myService.GetTimeSheetResults($scope.PageSize, $scope.currentPage).then(function (result) {
                objData = result.data;

                $scope.TotalRecords = objData.totalRecords;
                $scope.PageSize = objData.pageSize;
                $scope.currentPage = objData.currentPage;

                $scope.pagedItems = $scope.totalRecords / objData.pageSize;

                $scope.project = objData.projectList;
                $scope.tables = objData.timeSheetList;                

                $scope.selectedProjectId = objData.defaultProjectID;
                $scope.isUKPM = objData.isUKPM;
                $scope.UKPMVirtualDeveloperID = objData.defaultVirtualDeveloperID;
                //console.log($scope.UKPMVirtualDeveloperID);
                if (objData.developerList != null) {
                    $scope.develoers = objData.developerList;
                    $scope.developerId = objData.defaultVirtualDeveloperID;
                }
                /* Paging */
                var pageId = parseInt($('#pageid').val());              
                var totalPage = Math.ceil($scope.TotalRecords / objData.pageSize);
                $("#spanCurrentPageNumber").html(pageId);
                $("#spanTotalPageNumber").html(totalPage);
                $(".clsPaggingLast a").attr("data-pageid", totalPage); 
                setPagging(totalPage, pageId);
            });
        }

        $scope.onProjectChange = function () {
            myService.GetDevelopers($scope.selectedProjectId).then(function (resultDev) {
                if (resultDev.data.length > 0) {
                    $scope.develoers = resultDev.data;
                    if ($scope.isUKPM) {
                        var projectName = $.grep($scope.project, function (proj) {
                            return proj.id == $scope.selectedProjectId;
                        })[0].Name
                        if (projectName != "Others") {
                            console.log($scope.UKPMVirtualDeveloperID);
                            $scope.developerId = $scope.UKPMVirtualDeveloperID; //656;
                        }
                        else
                            $scope.developerId = '0';
                    }
                }
            });
        }

        $scope.onProjectChangeEdit = function () {
            myService.GetDevelopers($scope.selectedEditProjectId).then(function (resultDev) {
                if (resultDev.data.length > 0) {
                    $scope.develoers = resultDev.data;
                    if ($scope.isUKPM) {
                        var projectName = $.grep($scope.project, function (proj) {
                            return proj.Id == $scope.selectedProjectId;
                        })[0].Name
                        if (projectName != "Others")
                            $scope.editDeveloperId = $scope.UKPMVirtualDeveloperID; //656;
                        else
                            $scope.editDeveloperId = '0';
                    }
                }
            });
        }

       

        $scope.AddTimeSheet = function () {
           var TimeSheetModel = {
                AddedDate: $scope.addedDate,
                ProjectId: $scope.selectedProjectId,
                DeveloperId: $scope.developerId,
                WorkHours: $("#WorkHours").val(),
                Description: $scope.description,
                Id: '0'
            };


            myService.AddTimesheet(TimeSheetModel).then(function (resultTimeSheet) {

                if (resultTimeSheet.data.success == 'yes') {
                    $scope.TotalRecords = 0;
                    $scope.currentPage = 1;
                    $('#pageid').val(1);
                    $scope.pagedItems = 0;
                    GetTimeSheetResults();
                    $scope.time = "";
                    $scope.description = "";

                }
                else {
                    alert(resultTimeSheet.data.message);
                }
            });

        }
        $scope.SearchTimeSheet = function () {
            $scope.TotalRecords = 0;
            $scope.currentPage = 1;
            $('#pageid').val(1);
            $scope.pagedItems = 0;
            GetTimeSheetResults();
        }


        $scope.UpdateTimeSheet = function () {
         var TimeSheetModel = {
                AddedDate: $scope.editDate,
                ProjectId: $scope.selectedEditProjectId,
                DeveloperId: $scope.editDeveloperId,
                //WorkHours: $scope.editTime,
                WorkHours: $("#txtUpdateTime").val(),
                Description: $scope.editDescription,
                Id: $scope.userTimeSheetId
            };

            myService.AddTimesheet(TimeSheetModel).then(function (resultTimeSheet) {
                if (resultTimeSheet.data.success == 'yes') {
                    $('#myModal').modal('hide');
                    GetTimeSheetResults();
                    $scope.editDate = "";
                    $scope.selectedEditProjectId = '';
                    $scope.editDeveloperId = '';
                    $scope.editTime = "";
                    $scope.editDescription = "";
                    $scope.userTimeSheetId = "";

                }
                else {
                    alert(resultTimeSheet.data.message);
                }
            });
        }



        $scope.editItem = function (x) {
            $scope.userTimeSheetId = x.id;
            $scope.editDate = x.addedDateEdit;
            $scope.selectedEditProjectId = x.projectId;
            $scope.editDeveloperId = x.developerId;
            $scope.editTime = x.workHoursEdit;
            $scope.editDescription = x.description.replace("<sup style = 'color:#ca1198;font-weight:bold;'><b>PMS</b></sup>", '').trim();


            myService.GetDevelopers(x.projectId).then(function (resultDev) {
                if (resultDev.data.length > 0) {
                    $scope.develoers = resultDev.data;
                }
            });
        };


        $scope.setRowColor = function (isReviewed) {
            if (!isReviewed)
                return { "background-color": "#f7efef;" }
        }

        $scope.range = function (start, end) {
            var ret = [];
            if (!end) {
                end = start;
                start = 0;
            }
            for (var i = start; i < end; i++) {
                ret.push(i);
            }
            return ret;
        };
     
        $(document).on('click', ".page-item a", function (e) {
            var pageid = parseInt($(this).attr("data-pageid"));
            if (pageid == 0) { return false; }
            else {
                $('.page-item').removeClass("active");
                if ($(this).parent("li").hasClass("clsPaggingPrevious")) {
                    $('.clsPagging1').addClass("active");
                }
                else if ($(this).parent("li").hasClass("clsPaggingNext")) {
                    $('.clsPagging4').addClass("active");
                }
                else if ($(this).parent("li").hasClass("clsPaggingFirst")) {
                    resetPagging();
                }
                else { $(this).parent("li").addClass("active"); }

                $('#pageid').val(pageid);
                $scope.currentPage = pageid;
                GetTimeSheetResults();
            }

        });
    });
    // End Controller

})();
