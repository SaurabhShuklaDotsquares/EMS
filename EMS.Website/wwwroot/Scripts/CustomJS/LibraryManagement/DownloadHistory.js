(function ($) {
    var timer;
    $(document).on({
        ajaxStart: function () {
            timer && clearTimeout(timer);
            timer = setTimeout(function () {
                $('.divoverlay').removeClass('hide');
            }, 1500);
        },
        ajaxStop: function () {
            $('.divoverlay').addClass('hide');
            clearTimeout(timer);
        }
    });

    function index() {
        var $this = this, localeOpts, pmUid, closureFilterType;
        function getfilter(d) {
            var dateRange = $('#HistoryDateRange').val();
            var dateFrom = '', dateTo = '';

            if (dateRange && dateRange != '') {
                dateFrom = dateRange.split(localeOpts.separator)[0];
                dateTo = dateRange.split(localeOpts.separator)[1];
            }
            d = d || {};
            d.LibraryTitle = $("#LibraryTitle").val();
            d.DateFrom = dateFrom;
            d.DateTo = dateTo;
            d.Users = $("#Users").val();

            return d;
        }
        function loadDwnloadHistorySummary() {
            //var param = $this.estimateRequestParam(isPageLoad, isUpdateLeadSummaryTop);
            $.ajax({
                url: domain + "librarymanagement/DownloadHistorySummary",
                async: false,
                type: 'POST',
                datatype: 'application/json',
                data: getfilter(),
                success: function (json) {
                    $this.bindLibDwnloadHistSummary(json.LibDownloadSummary);

                    //$("#loading_stats").addClass("hidden");
                    //$("#summary_stats").removeClass("hidden");

                },
                error: function (ex) {
                    alert("Whooaaa! Something went wrong.." + ex);
                }
            });

        }
        $this.bindLibDwnloadHistSummary = function (details) {
            var html = '';
            $("#tblDwnloadHistSummaryLeft tbody tr, #tblDwnloadHistSummaryRight tbody tr").remove();
            $.each(details, function (indx, data) {
                var $container = $(data.rowIndex % 2 != 0 ? "#tblDwnloadHistSummaryRight" : "#tblDwnloadHistSummaryLeft");
                $("<tr/>", {
                    "class": (data.rowIndex % 2 == 0 ? "alternate " : "") + (data.rowIndex > 9 ? "showmore hidden " : ""),
                    html: function () {
                        $("<td/>", { text: data.componentName }).appendTo(this);
                        $("<td/>", {
                            text: data.totalcount,
                            "style": "text-align:center"
                        }).appendTo(this);

                    }
                }).appendTo($container);
            });
            $("span.showmoreless").parent().css("visibility", (details.length <= 10 ? "hidden" : "visible"));
            $("span.more.showmoreless").css("display", "");
            $("span.less.showmoreless").css("display", "none");
        }
        function loadLibraryDwnloadHistory() {
            LibraryDwnloadHistoryGrid = new Global.GridHelper('#grid-LibraryDownloadHistoryList', {
                serverSide: true,
                destroy: true,
                "pageLength": 25,
                searchDelay: 800,
                "bFilter": false,
                "ordering": false,
                "bAutoWidth": false,
                "bLengthChange": true,
                ajax:
                {
                    url: domain + "librarymanagement/DownloadHistory",
                    async: false,
                    type: "Post",
                    cache: false,
                    data: function (d) {
                        getfilter(d);
                    }
                },
                "columnDefs": [
                    { "width": "2%", "targets": 0 },
                    { "width": "8%", "targets": 1 },
                    { "width": "8%", "targets": 2 },
                    { "width": "8%", "targets": 3 },
                    { "width": "8%", "targets": 4 }
                ],
                columns:
                    [
                        {
                            name: "rowIndex", data: "rowIndex", title: "S. No", sortable: false, searchable: false, visible: true
                        },
                        {
                            name: "UserName", data: "userName", title: "User Name", sortable: false, searchable: false, visible: true
                        },
                        {
                            name: "Title", data: "title", title: "Component Title", sortable: false, searchable: false, visible: true
                        },
                        {
                            name: "Name", data: "name", title: "Component Type", sortable: false, searchable: false, visible: true
                        },
                        {
                            name: "DownloadOn", data: "downloadOn", title: "Download On", sortable: false, searchable: false, visible: true
                        }
                    ],
                "fnDrawCallback": function (oSettings) {

                    $('.pagination .active a').css('background-color', '#ff8c15');
                    $('.pagination .active a').css('border-color', '#e99701');
                }
            }
            );
        }

        function InitializeControl() {

            //closureSummary = $("#div_baconversion");

            var start = moment().subtract(29, 'days');
            var end = moment();
            localeOpts = {
                format: "DD/MM/YYYY",
                separator: " to "
            };


            function rangeChangeCB(start, end) {
                $('#HistoryDateRange').val(start.format(localeOpts.format) + localeOpts.separator + end.format(localeOpts.format));
            }

            $('#HistoryDateRange').daterangepicker({
                "locale": localeOpts,
                startDate: start,
                endDate: end,
                autoUpdateInput: false,
                ranges: {
                    'Today': [moment(), moment()],
                    'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                    'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                    'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                    'This Month': [moment().startOf('month'), moment().endOf('month')],
                    'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
                }
            }, rangeChangeCB);

            rangeChangeCB(start, end);
            $("#btnSearch").on("click", function () {
                loadDwnloadHistorySummary();
                loadLibraryDwnloadHistory();
                //LoadSummary();
            });

            $("#btnReset").on("click", function () {
                $("#LibraryTitle").val('');
                $("#Users").val('').trigger('chosen:updated');
                //$('#HistoryDateRange').val('');
                $('#HistoryDateRange').val(start.format(localeOpts.format) + localeOpts.separator + end.format(localeOpts.format));
                loadDwnloadHistorySummary();
                loadLibraryDwnloadHistory();

            });

            $("#txtSearch").on("keypress", function (e) {
                if (e.keyCode == 13) {
                    $("#btnSearch").click();
                    return false;
                }
            });

            $("#clrFilterDate").click(function () {
                $('#HistoryDateRange').val('');
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
        }
        $this.init = function () {
            InitializeControl();
            loadDwnloadHistorySummary();
            loadLibraryDwnloadHistory();
        };
    }
    $(function () {
        $(".chosen").chosen();
        var self = new index();
        self.init();
    });
}(jQuery));