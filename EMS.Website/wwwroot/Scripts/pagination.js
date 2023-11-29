
function resetPagging() {
    $('.page-item').removeClass("current");
    $('.clsPagging1').addClass("current");
    $('.clsPaggingNext a').attr("data-pageid", 0);
    $('.clsPagging4 a').attr("data-pageid", 4);
    $('.clsPagging3 a').attr("data-pageid", 3);
    $('.clsPagging2 a').attr("data-pageid", 2);
    $('.clsPagging1 a').attr("data-pageid", 1);
    $('.clsPaggingPrevious a').attr("data-pageid", 0);
    $('.clsPagging4').show();
    $('.clsPagging3').show();
    $('.clsPagging2').show();
    $('.clsPagging1').show();
    $('.clsPagging4 a').html(4);
    $('.clsPagging3 a').html(3);
    $('.clsPagging2 a').html(2);
    $('.clsPagging1 a').html(1);
}

function setPagging(totalPagging, pageId) {

    totalPagging = parseInt(totalPagging);
    pageId = parseInt(pageId);
    //var prvbuttonpageid = parseInt($('.clsPaggingPrevious a').attr("data-pageid"));
    //var nextbuttonpageid = parseInt($('.clsPaggingNext a').attr("data-pageid"));

    $('.clsPagging').show();
    if (totalPagging > 4) {

        //if (nextbuttonpageid == pageId) {
        //    tempid = nextbuttonpageid > 0 ? nextbuttonpageid : prvbuttonpageid - 1;
        //}
        //else if (prvbuttonpageid == pageId) {
        //    tempid = (prvbuttonpageid + 3 > totalPagging ? totalPagging : prvbuttonpageid + 3);
        //}
        //else { tempid = nextbuttonpageid > 0 ? nextbuttonpageid - 1 : prvbuttonpageid + 4 > totalPagging ? totalPagging : prvbuttonpageid + 4; }

        var MaxId = parseInt($('.clsPagging4 a').attr("data-pageid"));
        //alert(MaxId);
        tempid = MaxId > pageId ? MaxId > (pageId + 3) ? (MaxId - 1) : MaxId : pageId;
        // alert(tempid);
        $('.clsPaggingNext a').attr("data-pageid", (pageId == totalPagging ? 0 : pageId + 1));
        $('.clsPagging4 a').attr("data-pageid", tempid);
        $('.clsPagging3 a').attr("data-pageid", tempid - 1);
        $('.clsPagging2 a').attr("data-pageid", tempid - 2);
        $('.clsPagging1 a').attr("data-pageid", tempid - 3);
        $('.clsPaggingPrevious a').attr("data-pageid", (1 < pageId ? pageId - 1 : 0));
        $('.clsPagging3').show();
        $('.clsPagging2').show();
        $('.clsPagging1').show();
        $('.clsPagging4 a').html(tempid);
        $('.clsPagging3 a').html(tempid - 1);
        $('.clsPagging2 a').html(tempid - 2);
        $('.clsPagging1 a').html(tempid - 3);

    }
    else if (totalPagging > 3) {
        $('.clsPaggingNext a').attr("data-pageid", (totalPagging > pageId ? pageId + 1 : 0));
        $('.clsPagging4 a').attr("data-pageid", totalPagging);
        $('.clsPagging3 a').attr("data-pageid", totalPagging - 1);
        $('.clsPagging2 a').attr("data-pageid", totalPagging - 2);
        $('.clsPagging1 a').attr("data-pageid", totalPagging - 3);
        $('.clsPaggingPrevious a').attr("data-pageid", (1 < pageId ? pageId - 1 : 0));
        $('.clsPagging3').show();
        $('.clsPagging2').show();
        $('.clsPagging1').show();
        $('.clsPagging4 a').html(totalPagging);
        $('.clsPagging3 a').html(totalPagging - 1);
        $('.clsPagging2 a').html(totalPagging - 2);
        $('.clsPagging1 a').html(totalPagging - 3);
    }
    else if (totalPagging > 2) {
        $('.clsPaggingNext a').attr("data-pageid", (totalPagging > pageId ? pageId + 1 : 0));
        $('.clsPagging3 a').attr("data-pageid", totalPagging)
        $('.clsPagging2 a').attr("data-pageid", totalPagging - 1);
        $('.clsPagging1 a').attr("data-pageid", totalPagging - 2);
        $('.clsPaggingPrevious a').attr("data-pageid", (1 < pageId ? pageId - 1 : 0));
        $('.clsPagging3').show();
        $('.clsPagging2').show();
        $('.clsPagging4').hide();
        $('.clsPagging3 a').html(totalPagging);
        $('.clsPagging2 a').html(totalPagging - 1);
        $('.clsPagging1 a').html(totalPagging - 2);
    }
    else if (totalPagging > 1) {
        $('.clsPaggingNext a').attr("data-pageid", (totalPagging > pageId ? pageId + 1 : 0));
        $('.clsPagging2 a').attr("data-pageid", totalPagging);
        $('.clsPagging1 a').attr("data-pageid", totalPagging - 1);
        $('.clsPaggingPrevious a').attr("data-pageid", (1 < pageId ? pageId - 1 : 0));
        $('.clsPagging2').show();
        $('.clsPagging3').hide();
        $('.clsPagging4').hide();
        $('.clsPagging2 a').html(totalPagging);
        $('.clsPagging1 a').html(totalPagging - 1);
    }
    else if (totalPagging > 0) {
        $('.clsPaggingNext a').attr("data-pageid", 0);
        $('.clsPagging1 a').attr("data-pageid", totalPagging);
        $('.clsPaggingPrevious').attr("data-pageid", 0);
        $('.clsPagging4').hide();
        $('.clsPagging3').hide();
        $('.clsPagging2').hide();
        $('.clsPagging1 a').html(totalPagging);
    }
    else {
        $('.clsPagging').hide();
    }

    if (pageId > totalPagging) {
        $('#pageid').val(totalPagging);
    }
    $('.page-item').removeClass("current");
    $(".page-item a[data-pageid='" + pageId + "']").parent("li").addClass("current");
}
