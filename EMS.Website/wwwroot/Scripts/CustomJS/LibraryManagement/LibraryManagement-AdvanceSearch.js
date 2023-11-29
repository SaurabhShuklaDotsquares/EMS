//(function ($) {
function AdvanceSearch() {
    var $this = this;

    $this.intializeModalWithForm = function () {
        //console.log("-0-");
        //$(".detail-link").click(function () {
        //    //console.log("-01-");
        //    $("#modal-Library-Details").modal('show');
        //    $("#modal-Library-Details .modal-content").load($(this).data("href"));
        //});
        $("#modal-Library-Details").on('loaded.bs.modal', function () {
            $('.lnkDownload').off('click').on('click', function () {
                id = $(this).data("id");
                $.ajax({
                    url: "LibraryManagement/DownloadPermission",
                    type: "Post",
                    data: { id: id },
                    success: function (result) {
                        if (result.isSuccess) {
                            window.location = '/LibraryManagement/Download/' + id;
                        }
                        else {
                            swal({
                                title: "Alert!",
                                text: result.message,
                                icon: "error",
                            });
                        }
                    }
                });
            });
        }).on('hidden.bs.modal', function () {
            $("body").removeAttr("style");
            $(this).removeData('bs.modal');
            $(this).find('.modal-content').empty();
        });
    }

    $this.intializeModalComponentDocument = function () {
        $("#modal-Library-Details-ComponentDocu").on('loaded.bs.modal', function () {
            $('.lnkDownload').off('click').on('click', function () {
                id = $(this).data("id");
                $.ajax({
                    url: "LibraryManagement/DownloadPermission",
                    type: "Post",
                    data: { id: id },
                    success: function (result) {
                        if (result.isSuccess) {
                            window.location = '/LibraryManagement/Download/' + id;
                        }
                        else {
                            swal({
                                title: "Alert!",
                                text: result.message,
                                icon: "error",
                            });
                        }
                    }
                });
            });
            $('.lnkDownloadComponent').off('click').on('click', function () {
                id = $(this).data("id");
                $.ajax({
                    url: "LibraryManagement/DownloadComponentPermission",
                    type: "Post",
                    data: { id: id },
                    success: function (result) {
                        if (result.isSuccess) {
                            window.location = '/LibraryManagement/DownloadComponent/' + id;
                        }
                        else {
                            swal({
                                title: "Alert!",
                                text: result.message,
                                icon: "error",
                            });
                        }
                    }
                });
            });
        }).on('hidden.bs.modal', function () {
            $("body").removeAttr("style");
            $(this).removeData('bs.modal');
            $(this).find('.modal-content').empty();  
        });

        
    }

    function DownLoad() {

    }

    function getFilter() {
        var data = {
            SearchText: $.trim($('#search_box').val()),
            LibraryType: $('select[name="LibraryTypeId"]').val(),
            //DesignType: $('input[name="DesignTypeId"]:checked').val()
        };
        //if ($('#chkAdvanceSearch').is(':checked')) {

        data.Domains = $('#industries').val();
        data.Technologies = $('#technologies').val();
        data.DesignType = $('#designTypes').val();
        data.IsNDA = $("input[name='IsNda']:checked").val() === undefined ? $("input[name='IsNda']:checked").val() : Boolean(!!+$("input[name='IsNda']:checked").val());
        data.Featured = $("input[name='Featured']:checked").val() === undefined ? $("input[name='Featured']:checked").val() : Boolean(!!+$("input[name='Featured']:checked").val());
        data.IsReadyToUse = $("input[name='IsReadyToUse']:checked").val() === undefined ? $("input[name='IsReadyToUse']:checked").val() : Boolean(!!+$("input[name='IsReadyToUse']:checked").val());
        data.Layouts = $('#layouts').val();
        data.Components = $('#components').val();
        //data.FileTypes = $('#filetypes').val();
        data.IsAdvanceSearch = true;
        //}
        return data;
    }
    $this.initializeEvents = function () {
        $('#CopyLink').on('click', function () {
            var guid = null;
            $.ajax({
                url: "LibraryManagement/CreateSearch",
                data: getFilter(),
                type: 'POST',
                async: false,
                success: function (result) {

                    if (result.isSuccess) {
                        guid = result.guid;
                    }
                    else {
                        Global.ShowMessage(result.message, false, 'NotificationMessage');
                    }
                },
                error: function (result) {
                    Global.ShowMessage(result.message, false, 'NotificationMessage');
                }
            });
            currentUrl = window.location.href;
            urlObject = new URL(currentUrl);

            protocol = urlObject.protocol;
            host = urlObject.host;


            const el = document.createElement('textarea');
            el.value = protocol + "//" + host + "/librarymanagement/Index/" + guid;
            el.setAttribute('readonly', '');
            el.style.position = 'absolute';
            el.style.left = '-9999px';
            document.body.appendChild(el);

            const selected =
                document.getSelection().rangeCount > 0
                    ? document.getSelection().getRangeAt(0)
                    : false;
            el.select();
            document.execCommand('copy');
            var tooltip = document.getElementById("myTooltip");
            tooltip.innerHTML = "Copied: " + el.value;
            document.body.removeChild(el);
            if (selected) {
                document.getSelection().removeAllRanges();
                document.getSelection().addRange(selected);
            }
            $('.tooltiptext').not(tooltip).text('Copy to clipboard');

            //el.select();
            //document.execCommand('copy');
            //var tooltip = document.getElementById("myTooltip");
            //tooltip.innerHTML = "Copied: " + el.value;
            //document.body.removeChild(el);

        });

        $('.CopyLinkIndividual').off('click').on('click', function () {
            var guid = null;
            data = getFilter();
            data.KeyId = $(this).data("key-id");
            $.ajax({
                url: "LibraryManagement/CreateSearch",
                data: data,
                type: 'POST',
                async: false,
                success: function (result) {

                    if (result.isSuccess) {
                        guid = result.guid;
                    }
                    else {
                        Global.ShowMessage(result.message, false, 'NotificationMessage');
                    }
                },
                error: function (result) {
                    Global.ShowMessage(result.message, false, 'NotificationMessage');
                }
            });
            currentUrl = window.location.href;
            urlObject = new URL(currentUrl);

            protocol = urlObject.protocol;
            host = urlObject.host;


            const el = document.createElement('textarea');
            //el.value = protocol + "//" + host + "/librarymanagement/Index/?id=" + guid+"/detailid=";
            el.value = protocol + "//" + host + "/librarymanagement/Index/" + guid;
            el.setAttribute('readonly', '');
            el.style.position = 'absolute';
            el.style.left = '-9999px';
            document.body.appendChild(el);

            const selected =
                document.getSelection().rangeCount > 0
                    ? document.getSelection().getRangeAt(0)
                    : false;
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



            //el.select();
            //document.execCommand('copy');
            //var tooltip = document.getElementById("myTooltip");
            //tooltip.innerHTML = "Copied: " + el.value;
            //document.body.removeChild(el);

        });

        $('.lnkDownloadComponent').off('click').on('click', function () {
            id = $(this).data("id");
            $.ajax({
                url: "LibraryManagement/DownloadComponentPermission",
                type: "Post",
                data: { id: id },
                success: function (result) {
                    if (result.isSuccess) {
                        window.location = '/LibraryManagement/DownloadComponent/' + id;
                    }
                    else {
                        swal({
                            title: "Alert!",
                            text: result.message,
                            icon: "error",
                        });
                    }
                }
            });
        });

        $('.lnkDownloadRarOrZip').off('click').on('click', function () {
            id = $(this).data("id");
            $.ajax({
                url: "LibraryManagement/DownloadPermission",
                type: "Post",
                data: { id: id },
                success: function (result) {
                    if (result.isSuccess) {
                        window.location = '/LibraryManagement/Download/' + id;
                    }
                    else {
                        swal({
                            title: "Alert!",
                            text: result.message,
                            icon: "error",
                        });
                    }
                }
            });
        });

        $('.switchBox').off('change').on('change', function () {
            let changeStatus = confirm("Would you like to update feature status?")
            if (changeStatus) {
                $.get(domain + 'LibraryManagement/UpdateFeatureStatus', {
                    id: this.value
                });
            }
            else {
                let currenttStatus = $(this).prop("checked");
                $(this).prop("checked", !currenttStatus);
            }

        });

        //$('.myTooltipIndividual').on('onmouseout', function () {
        //    //var tooltip = document.getElementById("myTooltip");
        //    $(this).innerHTML = "Copy to clipboard";
        //})

    }

    //$this.init = function () {
    //    intializeModalWithForm();
    //    initializeEvents();
    //};
}
    //$(function () {
    //    var self = new AdvenceSearch();
    //    self.init();
    //});

//}(jQuery));