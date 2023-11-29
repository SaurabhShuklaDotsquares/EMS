(function () {
    function MyKRA() {
        var $this = this;

        $(document).ready(function () {
            var userDesignation = $("#userDes").val();
            $("span#" + userDesignation).addClass("txt").parent().parent().addClass("selected");
        });

        $this.init = function () {

        }
    }
    $('#modal-kra-detail').on('hide.bs.modal', function () {
        $(this).removeData('bs.modal');
        $(this).find('.modal-content').empty();
    });
    $(function () {
        var self = new MyKRA();
        self.init();
    })

}(jQuery))


//$(document).on('click', '#kra-detail', function () {
//    //alert('Hello');
//})

