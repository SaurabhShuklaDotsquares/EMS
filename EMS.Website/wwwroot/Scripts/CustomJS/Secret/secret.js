$("#Mypassword").on("click", function () {
    var UserId = document.getElementById('UserId').value;
    $.ajax(
        {
            type: 'POST',
            dataType: 'JSON',
            url: 'Secret/ShowSecretUserPassword',
            data: {
                UserId: UserId
            },
            success: function (data) {
                $('#inPassword').val(data.data.originalPassword);
                $("#divOrignalPassword").show();
                $("#divOrignalPassword").css('display', 'block');
            },
            error: function ( err) {
                alert('Error:' + err);
            }
        });

    });

