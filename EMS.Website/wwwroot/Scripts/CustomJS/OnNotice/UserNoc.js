(function ($) {
    function index() {
        var $this = this;

        function initializeEvent() {

            // hide and show Voluntary comment box on true/fasle

            if ($('input[type=radio].IsVoluntaryExit:checked').val() == 'true') {
                $('.VoluntaryExitstoggle').addClass('hide')
            }
            // hide and show Rehire comment box on true/fasle
            if ($('input[type=radio].IsEligibleForRehire:checked').val() == 'true') {
                $('.RehireToggle').addClass('hide')
            }

            $('.IsVoluntaryExit').change(function () {
                var data = $(event.target).val();
                $('.VoluntaryExitstoggle').toggleClass('hide')
                $('#Model_VoluntaryComment').val('')
                $('#setVoluntaryComment').text('')

            });
            $('.IsEligibleForRehire').change(function () {
                $('.RehireToggle').toggleClass('hide')
                $('#Model_RehireComment').val('')
                $('#setRehireComment').text('')
            });

            $('#Model_VoluntaryComment').val($('#setVoluntaryComment').text())
            $('#Model_RehireComment').val($('#setRehireComment').text())

            $(".allowdisabled").attr('disabled', 'disabled');

            //$('input[type=radio][name=IsVoluntaryExit]').change(function () {
            //    alert()
            //    var radioValue = $(this).val();
            //    if (radioValue == 'false') {
            //        $('.VoluntaryExitstoggle').removeClass('hide')
            //    }
            //    else {
            //        $('.VoluntaryExitstoggle').addClass('hide')
            //    }
            //});

            //$('input[type=radio][name=IsEligibleForRehire]').change(function () {
            //    var radioValue = $(this).val();
            //    if (radioValue == 'false') {
            //        $('.RehireToggle').removeClass('hide')
            //    }
            //    else {
            //        $('.RehireToggle').addClass('hide')
            //    }
            //});
        }

        $this.init = function () {
            if ($('input[class=IsVoluntaryExit]:checked').val() === 'False') {
                $('.setvoluntaryrequired').prop('required', 'required');
            };

            initializeEvent();
            //$('.checkcompleted').change(function () {
            //    var chk = $(this).is(':checked');
            //    if (chk == true) {
            //        if (confirm("Are you sure to Complete all exit Formalities?")) {
            //            $('.ResignationDate').attr("required", "required");
            //            $('.RelieveDate').attr("required", "required");
            //            return true;
            //        }
            //        else {
            //            return false;
            //        }
            //    }
            //});

            $('#btnsubmit').click(function () {

                var chk = $('.checkcompleted').is(':checked');
                if (chk == true) {
                    var valid = checkNocClear();
                    if (valid == false) {
                        $("#btnsubmit").blur();
                        return false;
                    }
                    if (confirm("Are you sure to Complete all exit Formalities?")) {
                        return true; 
                    }
                    else {
                        $("#btnsubmit").blur();
                        return false;
                    }
                }
            });

            $('input[type=radio].NocListEle').change(function () {
                $(this).parent().parent().find('label').removeClass("red");
            });


            //use to check all required NOC's are clear of not
            function checkNocClear() {
                var condition = true;
                $.each($(".usernoclists"), function () {
                    var isClear = $(this).data("isclear");
                    var elementName = $(this).children(".chkBox").children(":nth-child(2)").attr("name");
                    var checked = $("input[name='" + elementName + "']:checked").val();
                    if (isClear == "True") {
                        if (isClear != checked) {
                            condition = false;
                            //$("#reasoncheckbox").find(".red").html("<b>Please clear Required<small> (*) </small> NOC's</b>").css("float", "right");
                            $(this).find('label').addClass("red");
                        }
                    }
                });
                return condition;
            }

            // used to set the value for ResignationDate
            //$('#Model_ResignationDate').val($('.setResignationDate').text())
            //$('#Model_RelieveDate').val($('.setRelieveDate').text())

            // used to disable datepiker on is all formalities completed
            //var chkAllFormalities = $('.checkcompletedhidden').val();

            //if (chkAllFormalities == true) {
            //    $('.ResignationDate').attr("readonly", "readonly");
            //    $('.RelieveDate').attr("readonly", "readonly");
            //}
            //else {
            //}           
        }
    }
    $(function () {
        var self = new index();
        self.init();
    });
}(jQuery));
