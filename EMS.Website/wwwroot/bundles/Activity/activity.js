/*global jQuery, Global,secureDomain */

(function () {
    function ActivityUser() {
        $this = this;

        function initialize() {

            var tblTeamActivityList = $("#team_activity tbody");
            var tblTeamSummary = $("#team_summary");

            $(".datepicker").datepicker({
                dateFormat: "dd/mm/yy",
                changeMonth: true,
                changeYear: true,
                yearRange: "-100:+0"
            });

            $("#chboxNoticePeriod").on("click", function () {
                //var noticeperiod = (this).checked;
                //$('.divoverlay').removeClass('hide');
                //if ($("#chboxNoticePeriod").is(":checked")) {
                //    $.post(domain + 'activity/GetFilter',
                //        {
                //            noticePeriod: noticeperiod
                //        },
                //        function (response) {
                //            $("#tbl_grid").html(response);
                //            $('.divoverlay').addClass('hide');

                //            initTooltip();
                //        });
                //}
                $('#btn_search').trigger("click");
            });

            $("#clrActivityDate").click(function () {
                $('#txtActivityDate').val('');
            });

            $("a[data-url]").click(function (e) {
                e.preventDefault();
                window.location.href = domain + $(this).data("url");
            });

            $(".moreTech").click(function () {
                var id = $(this).data('id');
                $("." + id).show();
                $(this).parent().hide();
                $("#" + id + "_less").parent().show();
            });

            $(".lessTech").click(function () {
                var id = $(this).data('id');
                var more = $(this).data('more');
                $("." + id).hide();
                $(this).parent().hide();
                $("#" + more + "_more").parent().show();
            });

            $("#ddl_pm").on("change", function () {
                var pmId = $(this).val();
                $("#ddl_tl").empty();
                $("#ddl_tl").append("<option value='" + "" + "'>" + "-Select-" + "</option>");
                $.get(domain + 'activity/bindtl', { pmId: pmId }, function (response) {
                    if (response.isSucess) {
                        $.each(response.data, function (i, tl) {
                            $("#ddl_tl").append("<option value='" + tl.value + "'>" + tl.text + "</option>");
                        });

                    }
                });

            });

            $(".filter-btn").click(function () {
                $(".filter-outer").slideToggle('300');
            });

            $("#btn_search").on("click", function () {                
                var pmId = 0;
                $('.divoverlay').removeClass('hide');
                var technology = new Array();
                var specialist = new Array();
                var department = new Array();
                var status = new Array();
                var availability = new Array();
                var noticeperiod = false;
                var domainexpert = new Array();
                pmId = $("#ddl_pm").val();
                var activityDate = $("#txtActivityDate").val();

                $('.checkboxtechnology:checked').each(function () {
                    technology.push($(this).val());
                });

                //$(".checkboxspecialist:checked").each(function () {
                //    specialist.push($(this).val());
                //});

                $(".checkboxdepartment:checked").each(function () {
                    department.push($(this).val());
                });

                if ($("#chboxNoticePeriod").is(":checked")) {
                    noticeperiod = true;
                }

                //$(".checkboxstatus").each(function () {
                //    if (this.checked) {
                //        status.push($(this).val());
                //    }
                //});

                $(".checkboxavailability:checked").each(function () {
                    availability.push($(this).val());
                });
                $('.checkboxdomainexpert:checked').each(function () {
                    domainexpert.push($(this).val());
                });
                var tlId = $("#ddl_tl").val();
                var search = $("#txtsearch").val();

                $.post(domain + 'activity/GetFilter',
                    {
                        department: department,
                        //status: status,
                        //specialist: specialist,
                        technologies: technology,
                        Avail: availability,
                        search: search,
                        leadId: tlId,
                        pmId: pmId,
                        activityDate: activityDate,
                        domainexpert: domainexpert
                    },
                    function (response) {
                        $("#tbl_grid").html(response);
                        $('.divoverlay').addClass('hide');

                        initTooltip();
                    });
            });

            $("#MainContent_btnrefresh").click(function () {
                location.reload();
            });

            $("#tbl_grid").on("click", "#team_summary td.filter", function () {
                var td = $(this);
                var pmuId = td.closest("tr").data("pmuid");
                var visRecords = null;
                tblTeamActivityList = $("#team_activity tbody");
                if (pmuId) {
                    if (td.hasClass("active")) {
                        td.removeClass("active");
                        visRecords = tblTeamActivityList.children();
                    }
                    else {
                        tblTeamSummary.find("td.filter.active").removeClass("active");
                        td.addClass("active");
                        tblTeamActivityList.children().fadeOut();

                        if (td.data("status") && td.data("role")) {
                            visRecords = tblTeamActivityList.children("tr." + td.data("status") + "[data-role='" + td.data("role") + "'][data-pmuid='" + pmuId + "']");
                        }
                        else if (td.data("department")) {
                            visRecords = tblTeamActivityList.children("tr[data-department='" + td.data("department") + "'][data-pmuid='" + pmuId + "']");
                        }
                        else if (td.data("status")) {
                            visRecords = tblTeamActivityList.children("tr." + td.data("status") + "[data-pmuid='" + pmuId + "']");
                        }
                    }

                    if (visRecords && visRecords.length) {
                        visRecords.each(function (i, tr) {
                            $(tr).find("td:first").text(i + 1);
                        }).fadeIn();
                    }
                }
            });

            $("#tbl_grid").on("click", "#team_summary td.filter2", function () {
                var td = $(this);
                var pmuId = td.closest("tr").data("pmuid");

                if (pmuId) {

                    switch (td.data("other")) {
                        case "unassigned":
                            var unassignedRunning = td.data("unassignedrunning");

                            if (unassignedRunning) {
                                unassignedRunning = unassignedRunning.toString();

                                $.post(domain + 'activity/unassignedprojects?pmUid=' + pmuId, { runningProjects: unassignedRunning.split(',') }, function (data) {
                                    $("#modal-project-detail").find(".modal-content").html(data).end().modal('show');
                                });
                            }

                            break;

                        case "bucket":
                            var bucketRunning = td.data("bucketrunning");

                            if (bucketRunning) {
                                bucketRunning = bucketRunning.toString();

                                $.post(domain + 'activity/bucketprojects?pmUid=' + pmuId, { runningProjects: bucketRunning.split(',') }, function (data) {
                                    $("#modal-project-detail").find(".modal-content").html(data).end().modal('show');
                                });
                            }

                            break;

                        case "bonus":

                            var runningDevelopers = td.data("bonusrunning");

                            if (runningDevelopers && runningDevelopers.length) {

                                $.post(domain + 'activity/bonusprojects?pmUid=' + pmuId, { runningDevelopers: runningDevelopers }, function (data) {
                                    $("#modal-project-detail").find(".modal-content").html(data).end().modal('show');
                                });
                            }
                            break;

                        case "seo":
                            var seoDevelopers = td.data("seorunning");

                            if (seoDevelopers) {
                                seoDevelopers = seoDevelopers.toString();
                                $.post(domain + 'activity/seoprojects?pmUid=' + pmuId, { runningDevelopers: seoDevelopers.split(',') }, function (data) {
                                    $("#modal-project-detail").find(".modal-content").html(data).end().modal('show');
                                });
                            }

                            break;
                    }
                }
            });

            $("#MainContent_btnExcel").on("click", function () {
                $('.divoverlay').removeClass('hide');
                var technology = new Array();
                var specialist = new Array();
                var department = new Array();
                var status = new Array();
                var availability = new Array();

                $('.checkboxtechnology').each(function () {
                    if (this.checked) {
                        technology.push($(this).val());
                    }
                });

                $(".checkboxspecialist").each(function () {
                    if (this.checked) {
                        specialist.push($(this).val());
                    }
                });

                $(".checkboxdepartment").each(function () {
                    if (this.checked) {
                        department.push($(this).val());
                    }
                });


                $(".checkboxstatus").each(function () {
                    if (this.checked) {
                        status.push($(this).val());
                    }
                });

                $(".checkboxavailability").each(function () {
                    if (this.checked) {
                        availability.push($(this).val());
                    }
                });

                var tlId = $("#ddl_tl").val();
                var search = $("#txtsearch").val();
                var url = domain + 'activity/ExcelExport';
                var form = $('<form></form>').attr('action', url).attr('method', 'post');
                for (var i = 0; i < technology.length; i++) {
                    form.append($("<input></input>").attr('type', 'hidden').attr('name', 'department').attr('value', technology[i]));
                }
                for (var i = 0; i < status.length; i++) {
                    form.append($("<input></input>").attr('type', 'hidden').attr('name', 'status').attr('value', status[i]));
                }
                for (var i = 0; i < specialist.length; i++) {
                    form.append($("<input></input>").attr('type', 'hidden').attr('name', 'specialist').attr('value', specialist[i]));
                }
                for (var i = 0; i < technology.length; i++) {
                    form.append($("<input></input>").attr('type', 'hidden').attr('name', 'technologies').attr('value', technology[i]));
                }
                for (var i = 0; i < availability.length; i++) {
                    form.append($("<input></input>").attr('type', 'hidden').attr('name', 'Avail').attr('value', availability[i]));
                }
                form.append($("<input></input>").attr('type', 'hidden').attr('name', 'search').attr('value', search));
                form.append($("<input></input>").attr('type', 'hidden').attr('name', 'leadId').attr('value', tlId));
                form.appendTo('body').submit().remove();
                $('.divoverlay').addClass('hide');
            });

            $(".modal").on('hidden.bs.modal', function () {
                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
        }

        function initTooltip() {
            $("#tbl_grid [data-toggle='tooltip']").tooltip({ html: true });
        }

        $this.init = function () {
            initialize();
            initTooltip();
        }
    }

    $(function () {
        var self = new ActivityUser();
        self.init();
    });

}(jQuery));
