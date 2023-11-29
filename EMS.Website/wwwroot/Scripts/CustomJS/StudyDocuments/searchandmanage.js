(function ($) {
    "use strict"

    function SearchAndManage() {
        var $this = this, postData = {}, pageNo = 0;

        // init all controls
        function Intializecontrol() {

            // multiselect list
            $('#TechnologyId').multipleSelect({

                width: '100%',
                placeholder: "Select Technology",
                maxHeight: 150,
            });

            $('#TechnologyId').on("change", function () {

                $("#btntext_search").click();
                return false;
            });

            // request to approve
            $(document).on("click", ".clsreqtoapprove", function () {

                $.ajax({
                    url: domain + "studydocuments/RequestToViewDocuments/" + $(this).data("keyid"),
                    type: "get",
                    success: function (r) {
                        //debugger
                        if (r.isSuccess) {
                            swal({
                                text: r.message,
                                icon: "success",
                            });
                        }
                        else {
                            swal({
                                text: r.errorMessage,
                                icon: "error",
                            });
                        }
                    }
                });
            });
            // search button
            $(document).on("click", '#btntext_search', function () {

                Search();
            });
            // press enter key on search textbox
            $('#search_box').on('keyup', function (e) {

                if (e.keyCode == 13 || $(this).val() == "") {
                    $("#btntext_search").click();
                    return false;
                }
            });
            // reset 
            $('.btn-reset').off('click').on('click', function () {

                window.location.replace(domain + "studydocuments/SearchAndManage");
            })
            // copy link for single result
            $(document).on('click', ".CopyLinkIndividual", function () {

                var keyId = $(this).data("key-id");
                var singleResultUrl = domain + "studydocuments/SearchAndManage/" + keyId;

                const el = document.createElement('textarea');

                el.value = singleResultUrl;
                el.setAttribute('readonly', '');
                el.style.position = 'absolute';
                el.style.left = '-9999px';
                document.body.appendChild(el);

                const selected = document.getSelection().rangeCount > 0 ? document.getSelection().getRangeAt(0) : false;
                el.select();
                document.execCommand('copy');
                var tooltip = $(this).find(".myTooltipIndividual");
                $(tooltip).html("Copied: " + el.value);

                document.body.removeChild(el);
                if (selected) {
                    document.getSelection().removeAllRanges();
                    document.getSelection().addRange(selected);
                }
                $('.tooltiptext').not(tooltip).text('Copy to clipboard');

            });
            // model for get details
            $(document).on('loaded.bs.modal', "#modal-SD-Details", function () {

                // custom model show
            }).on('hidden.bs.modal', function () {

                $(this).removeData('bs.modal');
                $(this).find('.modal-content').empty();
            });
            // custom model popup open
            $(document).on("click", "[data-toggle='sdmodal']", function (e) {
                //debugger
                var targetId = $(this).data("target");
                var url = $(this).data("remote");
                $(targetId).modal('show');
                $(targetId).find(".modal-content").load(url);
            });
            // load more 
            $("#getLoadMoreData").on("click", function () {

                Search(true);
            });
        }

        // search
        function Search(isLoadMore) {
            //debugger
            $(".lds-roller").show();
            $("#btntext_search").html("Please wait...");
            $("#btntext_search").prop("disabled", true);

            if (isLoadMore == true) {
                pageNo = pageNo + 1;
                postData.PageNo = pageNo;
                postData.IsLoadMore = true;
                // load more takes prev. search data
            }
            else {
                pageNo = 0;
                $("#SearchResult").html('');
                var technologyId = (($("#TechnologyId").val() == null || $("#TechnologyId").val() == undefined) ? "" : $("#TechnologyId").val());
                postData = { SearchText: $("#search_box").val(), TechnologyId: technologyId };
            }

            $.ajax({
                url: domain + "studydocuments/SearchAndManage",
                data: postData,
                type: 'POST',
                success: function (result) {
                    //debugger
                    $(".lds-roller").hide();
                    $("#btntext_search").html("Search");
                    $("#btntext_search").prop("disabled", false);

                    // set result
                    $("#SearchResult").append(result);

                    // load more
                    if ($("#totalSearchedRecords").val() == undefined || $("#totalSearchedRecords").val() == $(".listing-sec .search-sd-col").length) {
                        $("#getLoadMoreData").hide();
                    }
                    else {
                        $("#getLoadMoreData").show();
                    }
                }
            });
        }


        // initlize
        $this.init = function () {
            Intializecontrol();
        };
    }
    $(function () {
        var self = new SearchAndManage();
        self.init();
    });
}(jQuery));
