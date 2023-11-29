(function ($) {
    function Calculation() {
        var $this = this, form, modal;
        var percentArray = []; var isTriggerSearch = false;
        function InitForm() {

            //$('#btnSearch').attr("disabled", true);
            //$('#btnSearch').css('pointer-events', 'none');
            $(document).on('keyup', '#SearchCRMLeadId', function () {
                if ($(this).val()) {
                    $('#btnSearch').attr('disabled', false);
                    $('#btnSearch').css('pointer-events', '');
                }
                else {
                    $('#btnSearch').attr("disabled", true);
                    $('#btnSearch').css('pointer-events', 'none');
                }
            });

            $(document).on("click", "#btnadd", function (e) {
                $.ajax({
                    type: "GET",
                    url: domain + 'estimate/addestimateform',
                    success: function (data) {

                        $("#estimateform").append(data);
                        $("form").data('validator', null);
                        $.validator.unobtrusive.parse('form');
                        $(".addnew").addClass("hide");
                        $("#estimateform .row").last().find(".addnew").removeClass("hide");
                        $(".remove").removeClass('hide');
                    },
                    error: function () {
                    }
                });
            });

            $(document).on('click', '.btnSDLC', function () {
                $('.SDLCTime').toggleClass('hidden');
            });
            $(document).on('click', '#btnSearch', function () {
                var crmid = $('#SearchCRMLeadId').val();
                SearchData(crmid, 0, "");
            });
            $(document).on('click', '[data-emodelid]', function () {
                var crmid = $('#SearchCRMLeadId').val();
                var emodelid = $(this).data().emodelid;
                var estimatename = $(this).data().estimatename;
                SearchData(crmid, emodelid, estimatename);
            });

            $(document).on('click', '.btncopy', function () {
                var $this = $(this);
                var packageboxbody = $this.closest('.package-box-body');
                var packagedetailsection = packageboxbody.find('.packagedetail-section');
                var id = packagedetailsection.attr('id');
                selectText(id);
                packageboxbody.find('.copy-msg').html(' Copied');
                setTimeout(function () {
                    packageboxbody.find('.copy-msg').html('');
                }, 2000)
            });

            $(document).on("change", ".roleid", function () {
                var $this = $(this);
                var row = $this.closest('.row');
                var experience = row.find('.experienceid');
                var value = $this.val();

                if (!value) {

                    row.find('.technologyid').prop("selectedIndex", 0);
                    row.find('.experienceid').prop("selectedIndex", 0);
                    row.find('.noOfResources').prop("selectedIndex", 0);
                    return true;
                }
                var technology = row.find('.technology');
                if (value != 1) {
                    technology.addClass('hide');
                    row.find('.technologyname').val('');
                }
                else {
                    technology.removeClass('hide');
                }

                row.find('.rolename').val($this.find('option:selected').text());

                $.ajax({
                    type: "GET",
                    url: domain + 'estimate/estimateroleexp',
                    data: { estimateRoleId: value },
                    success: function (data) {
                        experience.html('');
                        experience.append($('<option></option>').val('').html('Select Experience'));
                        $.each(data, function (val, dr) {
                            experience.append(
                                $('<option></option>').val(dr.value).html(dr.text)
                            );
                        });

                        if ($this.find('option:selected').val() == 1) {
                            row.find(".technologyid").prop("selectedIndex", 1);
                            experience.prop("selectedIndex", 2);

                            row.find('.technologyname').val(row.find('.technologyid').find('option:selected').text())
                        }

                        else {
                            row.find(".technologyid").prop("selectedIndex", 0);
                            experience.prop("selectedIndex", 1);;
                        }

                        row.find(".noOfResources").prop("selectedIndex", 1);
                        row.find('.technologyid').click();
                        row.find('.experienceid').click();
                        row.find('.noOfResources').click();

                        price($this);
                    },
                    error: function () {
                    }
                });


                //calculatePercentage();
            });

            $(document).on("click", "#spnSDLCTime", function () {
                if ($('.SDLCTime').hasClass('hidden') && $(this).text() == " + SDLC Time ") {
                    $(this).text(' - SDLC Time ')
                }
                else if ($('.SDLCTime').not('hidden') && $(this).text() == " - SDLC Time ") {
                    $(this).text(' + SDLC Time ')
                }
            });
            $(document).on("change", ".technologyid", function () {
                var $this = $(this);
                price($this);

                var row = $this.closest('.row');

                row.find('.technologyname').val($this.find('option:selected').text());

            });

            $(document).on("change", ".experienceid", function () {
                var $this = $(this);
                price($this);
                var row = $this.closest('.row');
                row.find('.experiencename').val($this.find('option:selected').text());
            });

            if ($('.remove').length == 1) {
                $('.remove').addClass('hide');
            }
            $(document).on('click', '.remove', function () {
                var $this = $(this);
                var estimateformRow = $("#estimateform .row");
                if (estimateformRow.length > 1) {
                    $this.closest('.row').remove();
                }
                if (estimateformRow.length == 1) {
                    $this.addClass('hide');
                }
                else {
                    $(".remove").removeClass('hide');
                }

                $("#estimateform .row").last().find(".addnew").removeClass("hide");
                if ($('.remove').length == 1) {
                    $('.remove').addClass('hide');
                }
                calculatePercentage();
            });

            var length = ($(".addnew").length - 1);
            $(`.addnew:eq(${length})`).removeClass('hide');

            // server hosting list 
            $(document).on('click', '.btn-servercountry', function () {
                var $this = $(this);
                var countryid = $this.data().countryid;
                var technologyids = $('.technologyid').map(function (i, el) {
                    var value = $(el).val();
                    if (value != '') {
                        return parseInt($(el).val());
                    }
                }).get();

                var serversection = $('#serverhosting-section');
                serversection.removeClass('hide');
                //serversection.find('[data-countryid]').addClass('hide');
                //serversection.find(`[data-countryid="${countryid}"]`).removeClass('hide');
                $('#ProgressBar_Status').show();
                $.ajax({
                    type: "POST",
                    url: domain + 'estimate/getserverhostingpackage',
                    data: { countryId: countryid, technologies: technologyids },
                    success: function (data) {
                        $('#ProgressBar_Status').hide();
                        serversection.html(data);
                    },
                    error: function (xhr) {
                        $('#ProgressBar_Status').hide();
                    }
                });

            });
        }

        $(document).on('click', '#btnEstimateFormSubmit', function (evt) {
            $("#CRMLeadId").css("display", "none");
            $("#HiddenCRMLeadId").css("display", "block");
            if (isTriggerSearch == true || $("#CRMLeadId").val()) {
                $('#IsSearchCRM').val(true);
            }
            else {
                $('#IsSearchCRM').val(false);
            }
            $('#EstimateModelId').attr('required', false);
            $('#EstimateName').attr('required', false);
            //$('#ProgressBar_Status').show();

        });
        if (!$.trim($('#divGraph').html()).length == false) {
            $('#divSave').css('display', 'block');
        }
        else {
            $('#divSave').css('display', 'none');
        }
        $(document).on('click', '#btnEstimateFormSave', function (evt) {
            $('#EstimateModelId').attr('required', true);
            $('#EstimateName').attr('required', true);
            $("#HiddenCRMLeadId").css("display", "none");
            $("#CRMLeadId").css("display", "block");

            if ($('#estimationForm').valid()) {
                var crmid = $("#CRMLeadId").val();
                var emodelid = $("#EstimateModelId").val();
                var estimateName = $("#EstimateName").val();
                $.ajax({
                    url: `estimate/checkcrmleadexist?crmLeadId=${crmid}&estimatemodelid=${emodelid}&estimateName=${estimateName}`,
                    type: 'post',
                    success: function (result) {
                        if (result.isCrmExist) {

                            if (confirm("Already added the data for this lead, Are you sure to update the existing data?")) {
                                //debugger;
                                $('#IsOverWrite').val(true);
                                //$('input[name="Command"]').val("Save");

                                //return true;
                                $('#estimationForm').submit();
                            }
                            else {
                                return false;
                            }
                        }
                        else {
                            $('#estimationForm').submit();
                        }
                    }
                });
                return false;
            }
        });

        $(document).on('change', '#EstimateModelId', function () {
            var $this = $(this);
            if ($this.val() != '') {
                var $estimateModelId = $('#EstimateModelId option:selected');
                var valname = $estimateModelId.text();
                //var hestimatename = $('#hEstimateName');
                //if (hestimatename.val() == '') {
                //    $('#EstimateName').val(valname);
                //}

                var estimatename = $('#EstimateName');
                if (estimatename.val() == '' && $estimateModelId.val() != '') {
                    estimatename.val(valname);
                }
                else {
                    $('#EstimateModelId option').each(function (i, v) {
                        if (estimatename.val() == v.text) {
                            estimatename.val(valname);
                        }
                    })
                }
            }
        });

        $.fn.inputFilter = function (inputFilter) {
            return this.on("input keydown keyup mousedown mouseup select contextmenu drop", function () {
                if (inputFilter(this.value)) {
                    this.oldValue = this.value;
                    this.oldSelectionStart = this.selectionStart;
                    this.oldSelectionEnd = this.selectionEnd;
                } else if (this.hasOwnProperty("oldValue")) {
                    this.value = this.oldValue;
                    this.setSelectionRange(this.oldSelectionStart, this.oldSelectionEnd);
                } else {
                    this.value = "";
                }
            });
        };
        $("#CRMLeadId,#SearchCRMLeadId").inputFilter(function (value) {
            return /^\d*$/.test(value);    // Allow digits only, using a RegExp
        });

        function calculatePercentage() {
            percentArray = [];
            var roleTypes = $('#estimateform').find('select.roleid');
            if (roleTypes.length > 1) {
                $.each(roleTypes, function (i, item) {
                    var roleType = $(item).find('option:selected').text();
                    if (roleType == 'Developer' || roleType == 'Designer') {
                        var hourTime = $(item).parents('.row').find('.estimateHour').val();
                        percentArray.push(hourTime);
                    }
                });
                calculatePercentageHour();
            }
        }
        function SearchData(crmid, estimatemodelid, estimatename) {
            $('#serverhosting-section').html('');
            $('#ProgressBar_Status').show();
            $.ajax({
                type: "GET",
                url: domain + `estimate/SearchPriceCalculation?crmleadid=${crmid}&estimatemodelid=${estimatemodelid}&estimateName=${estimatename}`,
                success: function (data) {
                    $('#ProgressBar_Status').hide();
                    //$("#estimate-main").html('');
                    $(".alert-success").alert('close');
                    if (data.status == undefined) {
                        $("#estimate-main").html(data);

                        $("form").data('validator', null);
                        $.validator.unobtrusive.parse('form');
                        $(".addnew").addClass("hide");
                        $("#estimateform .row").last().find(".addnew").removeClass("hide");
                        $(".remove").removeClass('hide');

                        $('#headername').html(`Estimate Calculator (CRM Lead Id : ${crmid} )`);

                        isTriggerSearch = true;
                        //$('#btnEstimateFormSubmit').click();
                        $("#CRMLeadId").css("display", "block");
                        $("#HiddenCRMLeadId").css("display", "none");
                        if (isTriggerSearch == true || $("#CRMLeadId").val()) {
                            $('#IsSearchCRM').val(true);
                        }
                        else {
                            $('#IsSearchCRM').val(false);
                        }

                        Graph();
                    }
                    else {
                        $('#divMessage').css("display", "block").addClass('alert alert-danger alert-dismissable');
                        $('#lblMessaggedescription').html(data.message);

                        $('#headername').html('Estimate Calculator');
                    }

                },
                error: function () {
                    $('#ProgressBar_Status').hide();
                }
            });
        }

        function calculatePercentageHour() {
            var sum = 0;
            $.each(percentArray, function (i, item) {
                sum += Number(item);
            });
            var percentHour = Math.round(sum * 10 / 100);

            updateHourTime(percentHour);
        }
        function updateHourTime(hour) {
            var roleTypes = $('#estimateform').find('select.roleid');
            if (roleTypes.length > 0) {
                $.each(roleTypes, function (i, item) {
                    var roleType = $(item).find('option:selected').text();
                    if (roleType == 'BA' || roleType == 'Business Analyst' || roleType == 'QA' || roleType == 'Quality Analyst') {
                        if (!$(item).parents('.row').find('.estimateHour').hasClass('custom-change')) {
                            $(item).parents('.row').find('.estimateHour').addClass('custom-change');
                            $(item).parents('.row').find('.estimateHour').val(hour);
                        }
                    }
                });
            }
        }
        function selectText(node) {
            node = document.getElementById(node);

            var textArea = document.createElement("textarea");
            textArea.value = node.textContent;
            document.body.appendChild(textArea);
            textArea.select();
            document.execCommand("Copy");
            textArea.remove();

            // this code for show selected text
            //if (document.body.createTextRange) {
            //    const range = document.body.createTextRange();
            //    range.moveToElementText(node);
            //    range.select();
            //} else if (window.getSelection) {
            //    const selection = window.getSelection();
            //    const range = document.createRange();
            //    range.selectNodeContents(node);
            //    selection.removeAllRanges();
            //    selection.addRange(range);
            //}
        }
        function price($this) {
            var row = $this.closest('.row');
            var experience = row.find('.experienceid').val();
            var technology = row.find('.technologyid').val();
            var roleid = row.find('.roleid').val();
            $.ajax({
                type: "GET",
                url: domain + 'estimate/estimateprice',
                data: { roleid: roleid, estimateRoleExpId: experience, technologyParentId: technology },
                success: function (data) {
                    row.find('.price').val(data.price);
                    row.find('.minprice').val(data.minPrice);
                },
                error: function () {
                }
            });
        }
        function Graph() {
            var itemArray = [];
            data = graphJson;
            var totalHours = 0;
            $.each(data, function (i, item) {
                if (item.y > 0) {
                    totalHours += item.y;
                }
            });
            if (totalHours > 0) {
                $.each(data, function (i, item) {
                    if (item.y > 0) {
                        itemArray.push({
                            name: item.name,
                            y: (item.y / totalHours * 100),
                            //color: item.color,
                            value: (item.y),
                            //totalWork: item.totalWork
                        });
                    }
                });

                var chart = Highcharts.chart('container-chart', {
                    chart: {
                        plotBackgroundColor: null,
                        plotBorderWidth: null,
                        plotShadow: false,
                        type: 'pie'
                    },
                    title: {
                        text: "Total Estimated Time" + " (" + totalHours + " Hrs)",
                        style: { "font-weight": "bold" }
                    },
                    tooltip: {
                        formatter: function () {
                            return '<b>' + this.point.name + '</b>: ' + this.y.toFixed(2) + '%' + " (" + this.point.value + " hrs)";
                        }
                    },
                    plotOptions: {
                        pie: {
                            size: '100%',
                            allowPointSelect: true,
                            cursor: 'pointer',
                            dataLabels: {
                                enabled: true,
                                style: {
                                    textOverflow: 'clip'
                                },
                                formatter: function () {
                                    if (this.y != 0) {
                                        return '<b>' + this.point.name + '</b>: ' + this.y.toFixed(2) + '%';
                                    } else {
                                        return null;
                                    }
                                }
                            },
                            point: {
                                events: {
                                    click: function () {
                                        console.log(this);
                                        LoadTimeSheetList(this.name);
                                    }
                                }
                            }
                        }
                    },
                    series: [{
                        data: itemArray
                    }]
                },
                    function (chart) { // on complete
                        if (itemArray.length == 0) {
                            chart.renderer.text('No Data Available, try Another Search.', 140, 395)
                                .css({
                                    color: 'Red',
                                    fontSize: '15px'
                                })
                                .add();
                        }
                    });
                $('.highcharts-credits').css("display", "none");
            }
        }

        $this.init = function () {
            InitForm();
            Graph();
        };
    }
    $(function () {
        var self = new Calculation();
        self.init();
    });
}(jQuery));
