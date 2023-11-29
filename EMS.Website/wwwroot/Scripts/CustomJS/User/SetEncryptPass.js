(function ($) {

    function index() {
        var $this = this;
  

        function setencryptiondata() {  
            $.ajax({
                type: "POST",
                url: domain + 'User/SetEncryptPassword',
                data: '',
                success: function (result) {
                     //console.log('result', result)
                    if (result === true) {
                        $('#EncryptSuccess').removeClass('hide');
                        $('#EncryptError').addClass('hide');
                        $('#EncryptInProgress').addClass('hide');
                    }
                    else {
                        $('#EncryptError').removeClass('hide');
                        $('#EncryptSuccess').addClass('hide');
                        $('#EncryptInProgress').addClass('hide');
                    }                   
                }
            });
        }

        function initilizeControls() {
            $('#startencryption').click(function () {
                if (confirm("This will create encrypted password in PasswordKey. Do you want to continue?")) {
                    $('#EncryptInProgress').removeClass('hide');
                    setencryptiondata();
                }
            }); 
        }

        $this.init = function () {
            initilizeControls();
        };
    }

    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));