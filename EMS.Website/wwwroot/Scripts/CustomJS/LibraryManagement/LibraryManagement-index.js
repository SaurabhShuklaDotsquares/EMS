
$(function ($) {
    function LibraryManagement() {
        var $this = this;
        var totalSearchedRecord = $("#totalSearchedRecords").val();
        function getFilter() {
            /*    console.log($("input[name='Featured']:checked").val());*/
            var data = {
                SearchText: $('#search_box').val(),
                LibraryType: $('select[name="LibraryTypeId"]').val(),
            };
            data.Domains = $('#industries').val();

            data.Technologies = $('#technologies').val();
            data.Layouts = $('#layouts').val();
            data.Components = $('#components').val();
            data.Templates = $('#templates').val();

            data.DesignType = $('#designTypes').val();
            data.SalesKitTypeId = $('#salesKitTypes').val();
            data.CvsTypeId = $('#cvsTypes').val();

            data.IsNDA = $("input[name='IsNda']:checked").val() === undefined ? $("input[name='IsNda']:checked").val() : Boolean(!!+$("input[name='IsNda']:checked").val());
            data.Featured = $("input[name='Featured']:checked").val() === undefined ? $("input[name='Featured']:checked").val() : Boolean(!!+$("input[name='Featured']:checked").val());
            data.IsReadyToUse = $("input[name='IsReadyToUse']:checked").val() === undefined ? $("input[name='IsReadyToUse']:checked").val() : Boolean(!!+$("input[name='IsReadyToUse']:checked").val());
            data.IsAdvanceSearch = true;
            data.LibraryTypeLabel = $('.libraryTypeLabel').text();
            data.LibraryTypeFilterId = "#LibraryTypeId";
            data.DesignTypeLabel = $('.designTypeLabel').text();
            data.SalesKitTypeLabel = $('.salesKitTypeLabel').text();
            data.CvsTypeLabel = $('.cvsTypeLabel').text();

            //data.DesignFilterId = "#designTypes";
            //data.SalesKitFilterId = "#salesKitType";
            //data.CvsFilterId = "#cvsType";

            data.LayoutLabel = $('.layoutLabel').text();
            data.DesignTypeFilterId = "#designTypes";
            data.SalesKitTypeFilterId = "#salesKitType";
            data.CvsTypeFilterId = "#cvsType";

            data.LayoutLabel = $('.layoutLabel').text();
            data.LayoutFilterId = ".layout-filter";
            data.ComponentLabel = $('.componentLabel').text();
            data.ComponentFilterId = ".component";
            data.TemplateLabel = $('.templateLabel').text();
            data.TemplateFilterId = ".template";
            data.IndustryLabel = $('.industryLabel').text();
            data.IndustryFilterId = ".industry";
            data.TechnologyLabel = $('.technologyLabel').text();
            data.TechnologyFilterId = ".technology";
            data.NDAStatusLabel = $('.ndaStatusLabel').text();
            data.NDAStatusFilterId = "#IsNda";
            data.FeaturedStatusLabel = $('.featuredStatusLabel').text();
            data.FeaturedStatusFilterId = "#Featured";
            data.IsReadyToUseStatusLabel = $('.isReadyToUseStatusLabel').text();
            data.IsReadyToUseStatusFilterId = "#IsReadyToUse";
            return data;
        }
        function isNullOrWhitespace(input) {
            if (typeof input === 'undefined' || input == null) return true;
            return input.replace(/\s/g, '').length < 1;
        }

        function InitializeEvents() {
            document.oncontextmenu = document.body.oncontextmenu = function () { return false; }

            var isDesigner = $("#loginUserType").val();



            $(document).keydown(function (event) {
                if (event.keyCode == 123) { // Prevent F12
                    return false;
                } else if (event.ctrlKey && event.shiftKey && event.keyCode == 73) { // Prevent Ctrl+Shift+I        
                    return false;
                }
            });

            $(".type-sec").off('click').click(function () {
                if ($('.type-sec .drop-sec').is(':visible')) {
                    $(".type-sec").data("visible", "0");
                    $('.type-sec .drop-sec').hide();
                }
                else {
                    $(".type-sec").data("visible", "1");
                    $('.type-sec .drop-sec').show();
                }
                $('.adv-srch .drop-sec').hide();
            });

            $(".type-sec .close").off('click').click(function (event) {
                event.stopPropagation();
                $(".type-sec .drop-sec").hide();
            });

            $(".adv-srch .adv-srch-title, .adv-srch.option-box::after").off('click').click(function () {
                if ($('.adv-srch .drop-sec').is(':visible')) {
                    $(".adv-srch").data("visible", "0");
                    $('.adv-srch .drop-sec').hide();
                }
                else {
                    $(".adv-srch").data("visible", "1");
                    $('.adv-srch .drop-sec').show();
                }
                $('.type-sec .drop-sec').hide();
            });

            $(".adv-srch .close").off('click').click(function (event) {
                event.stopPropagation();
                $('.adv-srch .drop-sec').hide();
            });

            //#region change and click events on filter

            $("#industries,#technologies,#IsNda,#Featured").on("change", function () {
                $("#reqSearch").hide();
                Search();
            });

            $("#search_box").on("keyup", function () {
                $("#reqSearch").hide();
                Search();
            });

            $("#LibraryTypeId").on("change", function () {
                $("#reqSearch").hide();
                $("#cvsType").val('');
                $("#salesKitType").val('');
                ResetControls();
                Search();
            });
            $("#cvsType,#salesKitType,#designType").on("change", function () {
                $("#reqSearch").hide();
                Search();
            });

            //#endregion

            $(document).off("click", "#btntext_search").on("click", "#btntext_search", function (event) {
                $("#getLoadMoreData").css("display", "none");
                //console.log("-1-"); 
                //console.log($("input[name='Featured']").val());
                //console.log($("#Featured:checked").val());
                //console.log($("#IsNda:checked").val());
                if ($("#LibraryTypeId").val() == 0 && $.trim($("#search_box").val()) == "" && $("#Featured:checked").val() === undefined && $("#IsNda:checked").val() === undefined
                    && $("#designTypes").val() === "" && $("#technologies").val() == null
                    && $("#industries").val() == null) {
                    $("#SearchResult").html("");
                    $("#reqSearch").show();
                } else {
                    $("#reqSearch").hide();
                    Search();

                }
            });
            $('#search_box').on('keypress', function (e) {

                if (e.keyCode == 13) {
                    $("#btntext_search").click();
                    return false;
                }
            });
            $('#search_box').on('click', function () {

                $('.adv-srch .drop-sec').hide();
                $('.type-sec .drop-sec').hide();
            });


            $('.industry-togle').on('click', function () {
                $(this).toggleClass('open');
                $('.industry-block').slideToggle();
            });

            //$(window).scroll(function () {
            //    //Check for user has reached bottom of Page
            //    $("#noRecordFound").hide();

            //    if ($(window).scrollTop() == ($(document).height() - window.innerHeight)) {
            //        //    alert($(window).scrollTop() + " and " + ($(document).height() - window.innerHeight));
            //        $('.lds-roller').show();
            //        $('#load-more').show();
            //        setTimeout(function () {
            //            appendContent();
            //        }, 1000);
            //    }
            //});


            $(document).off("click", "#getLoadMoreData").on("click", "#getLoadMoreData", function (event) {
                $("#noRecordFound").hide();
                $('.lds-roller').show();
                $('#load-more').show();
                appendContent();
            });


            function appendContent() {

                $.ajax({
                    url: "LibraryManagement/LoadSearchLibrary",
                    data: getFilter(),
                    type: 'POST',
                    success: function (result) {

                        if (result != "") {
                            $(".listing-sec").append(result);
                            $('.lds-roller').hide();
                            $('#load-more').hide();

                        } else {
                            $("#noRecordFound").show();
                        }
                        if ($("#totalSearchedRecords").val() == undefined || $("#totalSearchedRecords").val() == $(".listing-sec .search-library-col").length) {
                            $("#getLoadMoreData").css("display", "none");
                        }
                    }
                });
            };

            $("#search_box").on("keyup", function () {
                var value = $("#search_box").val();
                $.ajax({
                    url: "LibraryManagement/GetTags",
                    data: { "tag": value },
                    type: 'POST',
                    success: function (result) {
                        if (result != "") {
                            var data = JSON.parse(result);
                            $("#search_box").autocomplete({
                                source: data
                            });
                        }
                    }
                });
            });

            $(".library-type").on("change", function () {
                ResetControls();
                var current = $(this).val();
                if (current == 2) {
                    $("#designType").fadeIn();
                    $('div#divAdvancedFilters .layout').show();
                }
                else {
                    $("#designType").fadeOut();
                    $('#layouts option').removeAttr("selected");
                    $('div#divAdvancedFilters .layout').hide();
                }
                if (current == 4) {
                    $('.ReadyToUse').show();
                    $('div#divAdvancedFilters .component').show();
                }
                else {
                    $('.ReadyToUse').hide();
                    $('#IsReadyToUse option').removeAttr("selected");
                    $('#components option').removeAttr("selected");
                    $('div#divAdvancedFilters .component').hide();
                }
                if (current == 7) {
                    $("#salesKitType").fadeIn();
                    $("#cvsTypes option").removeAttr("selected");
                    $('div#divAdvancedFilters .layout').show();
                }
                else {
                    $("#salesKitType").fadeOut();
                    $('div#divAdvancedFilters .layout').hide();
                }
                if (current == 8) {
                    $("#cvsType").fadeIn();
                    $("#salesKitTypes option").removeAttr("selected");
                    $('div#divAdvancedFilters .layout').show();
                }
                else {
                    $("#cvsType").fadeOut();
                    $('div#divAdvancedFilters .layout').hide();
                }
                if (current == 6) {
                    $('div#divAdvancedFilters .template').show();
                }

                else {
                    $('#templates option').removeAttr("selected");
                    $('div#divAdvancedFilters .template').hide();
                }
            });

            $(document).on('click', '.filter-display .close1', function (event) {
                event.stopPropagation();
                $('.adv-srch .drop-sec').hide();
                var filterType = $(this).data('filter-type');
                var value = $(this).data('value');
                var children = $(this).closest('.filter-element').find('.cross-element');
                if (children.length < 2) {
                    $(this).closest('.filter-element').remove();
                }
                else {
                    $(this).parent().remove();
                }
                if (filterType == '.layout-filter' || filterType == '.component' ||
                    filterType == '.industry' || filterType == '.technology'
                    || filterType == '.template') {
                    RemoveItemFromSelectList(filterType, value);
                } else if (filterType == '#LibraryTypeId') {
                    if ($(filterType).val() != "0") {
                        $(filterType).val("0");
                    }
                    $('.ReadyToUse').hide();
                    $("input[name='IsReadyToUse']").removeAttr("checked");
                    $("#designTypes option").removeAttr("selected");
                    $("#salesKitTypes option").removeAttr("selected");
                    $("#cvsTypes option").removeAttr("selected");

                    $('div#divAdvancedFilters .layout').hide();
                    $('div#divAdvancedFilters .component').hide();
                    $('div#divAdvancedFilters .template').hide();
                }
                else if (filterType == '#designTypes') {
                    if ($(filterType).val() != "") {
                        $(filterType).val("");
                    }
                }
                else if (filterType == '#IsReadyToUse') {
                    $("input[name='IsReadyToUse']").removeAttr("checked");
                    //$('#IsReadyToUse:first').prop("checked", true);
                }
                else if (filterType == '#Featured') {
                    $("input[name='Featured']").removeAttr("checked");
                    //$('#Featured:first').prop("checked", true);
                }
                else if (filterType == '#IsNda') {
                    $("input[name='IsNda']").removeAttr("checked");
                    //$(filterType).removeAttr("checked");
                    //$('#IsNda:first').prop("checked", true);
                }
                else if (filterType == '#salesKitType') {
                    $("#salesKitTypes option").removeAttr("selected");
                    $("#salesKitType").fadeOut();
                }
                else if (filterType == '#cvsType') {
                    $("#cvsTypes option").removeAttr("selected");
                    $("#cvsType").fadeOut();
                }
                if ($("#LibraryTypeId").val() == 0 && $.trim($("#search_box").val()) == "" && $("#Featured:checked").val() === undefined && $("#IsNda:checked").val() === undefined
                    && $("#designTypes").val() === "" && $("#technologies").val() == null
                    && $("#industries").val() == null) {
                    $("#SearchResult").html("");
                    $("#salesKitType").fadeOut();
                    $("#cvsType").fadeOut();
                    $("#salesKitTypes option").removeAttr("selected");
                    $("#cvsTypes option").removeAttr("selected");
                    //$("#reqSearch").show();
                } else {
                    $("#reqSearch").hide();
                    Search();
                }
                //Search();
            });

            $(document).on('click', "#techSelect .fs-option", function (e) {
                setTimeout(
                    function () {
                        var techs = $('#technologies').val();
                        var arr = techs.join();
                        $.ajax({
                            url: "LibraryManagement/GetComponents",
                            data: { Ids: arr },
                            type: 'POST',
                            success: function (result) {
                                if (result != "") {
                                    $(".component").html(result);
                                    $('.select2Components').fSelect({ placeholder: "Select Component" });
                                }
                            }
                        });
                    }, 1000);
            });

            $('#btnApplyFilter').off('click').on('click', function (e) {
                $("#btntext_search").click();
                return false;
            });


            $('.industry-listing').enscroll({
                showOnHover: false,
                verticalTrackClass: 'track3',
                verticalHandleClass: 'handle3'
            });
            $('.scroll2').enscroll({
                showOnHover: false,
                verticalTrackClass: 'track3',
                verticalHandleClass: 'handle3'
            });

            if (isDesigner == "True") {
                $('#LibraryTypeId').val('2').trigger('change');
            }
        }

        function removeFilter() {
            var children = $('.close1').closest('.filter-element').find('.sub-category').html();
            if ($("#LibraryTypeId").val() == 2) {
                if (children.length > 0 && children.indexOf("#designType") == -1) {
                    $('.close1').closest('.filter-element').find('.sub-category').remove();
                }
            }
            else if ($("#LibraryTypeId").val() == 7) {
                if (children.length > 0 && children.indexOf("#salesKitType") == -1) {
                    $('.close1').closest('.filter-element').find('.sub-category').remove();
                }
            }
            else if ($("#LibraryTypeId").val() == 8) {
                if (children.length > 0 && children.indexOf("#cvsType") == -1) {
                    $('.close1').closest('.filter-element').find('.sub-category').remove();
                }
            }
        }

        function RemoveItemFromSelectList(Id, value) {
            $(Id + ' select option:selected[value="' + value + '"]').removeAttr('selected');
            $(Id + ' .fs-options .fs-option[data-value="' + value + '"]').removeClass('selected', false);
            var $wrap = $(Id).find('.fs-wrap');
            var $select = $wrap.find('select');
            $select.fSelect('reloadDropdownLabel');
        }

        function Search() {
            $("#SearchResult").html('');
            $(".lds-roller").show();
            $("#btntext_search").html("Please wait...");
            $("#btntext_search").prop("disabled", true);
            $.ajax({
                url: "LibraryManagement/SearchLibrary",
                data: getFilter(),
                type: 'POST',
                success: function (result) {
                    $(".lds-roller").hide();
                    $("#btntext_search").html("Search");
                    $("#btntext_search").prop("disabled", false);
                    $("#SearchResult").html(result);
                    var adsearch = new AdvanceSearch();
                    adsearch.intializeModalWithForm();
                    adsearch.intializeModalComponentDocument();
                    adsearch.initializeEvents();
                    removeFilter();
                    if ($("#totalSearchedRecords").val() == undefined || $("#totalSearchedRecords").val() == $(".listing-sec .search-library-col").length) {
                        $("#getLoadMoreData").css("display", "none");
                    }
                    else {
                        $("#getLoadMoreData").css("display", "block");
                    }
                }
            });
        }

        $(".clearControl").click(function () {
            var control = $(this).data('control');
            if (control == "Featured") {
                $("input[name='Featured']").removeAttr("checked");
            }
            if (control == "IsNda") {
                $("input[name='IsNda']").removeAttr("checked");
            }
            if (control == "IsReadyToUse") {
                $("input[name='IsReadyToUse']").removeAttr("checked");
            }
            Search();
        });

        function ResetControls() {
            $('.ReadyToUse').hide();
            $("#designTypes option").removeAttr("selected");
            $("#salesKitTypes option").removeAttr("selected");
            $("#cvsTypes option").removeAttr("selected");
            $("#search_box").val("");
            $("#designType").fadeOut();
            $("#salesKitType").fadeOut();
            $("#cvsType").fadeOut();

            fselectClear('technologies', 'select2Technologies', 'Select Technology', 'technology');
            fselectClear('layouts', 'select2Layouts', 'Select Layout', 'layout');
            fselectClear('components', 'select2Components', 'Select Component', 'component');
            fselectClear('templates', 'select2Cemplates', 'Select Template', 'template');
            $("input[name='IsNda']").removeAttr("checked");
            $("input[name='IsReadyToUse']").removeAttr("checked");
            $("input[name='Featured']").removeAttr("checked");
        }

        function fselectClear(id, classDropdown, label, classDiv) {
            $('#' + id + ' option:selected').removeAttr('selected');
            $('.' + classDropdown).prev(".fs-dropdown").find(".fs-options>.fs-option").removeClass('selected');
            $('.' + classDiv + ' .fs-label').html(label);
        }

        function bindMultiSelect() {
            $('.select2Industries').fSelect({ placeholder: "Select Domain" });
            $('.select2Technologies').fSelect({ placeholder: "Select Technology" });
            $('.select2Layouts').fSelect({ placeholder: "Select Layout" });
            $('.select2Components').fSelect({ placeholder: "Select Component" });
            $('.select2Templates').fSelect({ placeholder: "Select Template" });
            //$(document).on("fSelect", ".select2Components");
        }

        function OnLoadVisibility() {
            if ($('#chkAdvanceSearch').is(':checked')) {
                $("#divAdvancedFilters").css("display", "block");
            }
            if ($('select[name="LibraryTypeId"]').val() == 2) {
                $('#designType').show();
                $('.ReadyToUse').hide();
                $('div#divAdvancedFilters .component').hide();
            }
            if (isBA == 1 && $('select[name="LibraryTypeId"]').val() == 7) {
                $('#designType').hide();
                $('#cvsType').hide();
                $('#salesKitType').show();
                $('.ReadyToUse').hide();
                $('div#divAdvancedFilters .component').hide();
            }
            else if ($('select[name="LibraryTypeId"]').val() == 4) {
                //console.log("-21-");
                $('.ReadyToUse').hide();
                $('div#divAdvancedFilters .component').show();
                $('div#divAdvancedFilters .layout').hide();
                //console.log("-22-");
            }
            else {
                $('#salesKitType').hide();
                $('#designType').hide();
                $('#cvsType').hide();

                $('.ReadyToUse').hide();
                $('div#divAdvancedFilters .layout').hide();
                $('div#divAdvancedFilters .component').hide();
                $('div#divAdvancedFilters .template').hide();
            }

            if ($("#hdnDetailid").val() != "") {
                var aElement = $("a.btn-details").filter(function () {
                    return $(this).data("key-id") == $("#hdnDetailid").val();
                });
                if (aElement.length == 1) {
                    $(aElement).click();
                }
            }

            $('.btn-reset').off('click').on('click', function () {
                $('.ReadyToUse').hide();
                $('div#divAdvancedFilters .layout').hide();
                $('div#divAdvancedFilters .component').hide();
                $('div#divAdvancedFilters .template').hide();
                ResetControls();
                $('#LibraryTypeId').prop('selectedIndex', 0);
                //$('div#divAdvancedFilters .layout').show();
                $("#SearchResult").html("");
                $("#designType").fadeOut();
                $("#salesKitType").fadeOut();
                $("#cvsType").fadeOut();

                //reset industry
                fselectClear('industries', 'select2Industries', 'Select Domain', 'industry');
                $(".fs-option").removeClass("selected");
                //$(".fs-label").text('Select some options');
                // Search();
            })
        }
        $this.init = function () {
            if (isBA == 1) {
                $("#LibraryTypeId option[value='7']").prop('selected', true);
                Search();
                $('.library-type').trigger('change');
            }
            InitializeEvents();
            bindMultiSelect();
            OnLoadVisibility();
            //$('#LibraryTypeId').prop('selectedIndex', 0);
            //$("#designType").fadeIn();
            //$('div#divAdvancedFilters .layout').show();
            $("#LibraryTypeId option[value='6']").remove();

            //Search();
        };
    }
    $(function () {
        var self = new LibraryManagement();
        self.init();
        if ($(".SearchFile").length) {
            var adsearch = new AdvanceSearch();
            adsearch.intializeModalWithForm();
            adsearch.intializeModalComponentDocument();
            adsearch.initializeEvents();
        }
    });
}(jQuery));

$(document).click(function (e) {
    if (!$(e.target).parents().andSelf().is('.type-sec')) {
        if ($('.type-sec').data("visible") == "1") {
            $(".type-sec .drop-sec").hide();
        }
    }
    if (!$(e.target).parents().andSelf().is('.adv-srch')) {
        if ($('.adv-srch').data("visible") == "1") {
            $(".adv-srch .drop-sec").hide();
        }
    }
});