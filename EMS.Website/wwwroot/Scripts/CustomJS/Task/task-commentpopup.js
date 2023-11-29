(function ($) {

    function Commentpopup() {
        var $this = this, form, modal;      
       
        function InitForm() {
            var modal = $("#modal-comment-task");          

            modal.find("#btn_task_postreply").on('click', function () {
                modal.find("#task_postreply").show();
                modal.find("#btn_task_postreply").toggle();
            });

            modal.find("#btn_cancel_postcomment").on('click', function () {
                modal.find("#task_postreply").hide();
                modal.find("#btn_task_postreply").toggle();
            });

            formTaskComment = new Global.FormHelper(modal.find("form"),
                {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: '' }
                }, null, function onSucccess(result) {
                    if (result.isSuccess) {                       
                        if ($.fn.TaskGridIndex) {
                            $.fn.TaskGridIndex.refreshTaskGrid();
                        }
                        else if ($.fn.HomeIndex) {
                            $.fn.HomeIndex.RefreshTaskGrid();
                        }
                        modal.find("#Comment").val(' ');
                        //modal.find("#TaskStatusId").val('1');
                        modal.find("#task_postreply").hide();
                        modal.find("#btn_task_postreply").show();
                        modal.find("#statuslabel").empty().html(modal.find("#TaskStatusId option:selected").text());
                        if (result.data.showPostReplyButton==false)
                        {
                            modal.find("#btn_task_postreply").hide();
                        }
                        modal.find("#task_comment_table tbody").prepend('<tr><td>' + result.data.comment + '</td><td><strong>' + result.data.commentBy + '</strong><br /> <span style="font-size: 11px; padding-top: 4px; display: block;"><strong>Posted On : </strong>' + result.data.addedDate + '</span></td></tr>');
                        Global.ShowMessage(result.message, true, 'taskCommentMessageDiv');
                    }
                    else
                    {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'taskCommentMessageDiv');
                    }   
                });

            modal.off('hidden.bs.modal')
               .on('hidden.bs.modal', function () {                   
                   modal.removeData('bs.modal');
                   modal.find('.modal-content').empty();
               });
        }
        
       function ShowHideCommentFor()
       {
           var status = $('#TaskStatusId').val();
           if (status === "3" || status === "4") { // on loading
                $('.divCommentFor').show();
            }
            else {
                $('.divCommentFor').hide();
            }

           $('#TaskStatusId').on('change', function () {
               
               if ($('#TaskStatusId').val() === "3" || $('#TaskStatusId').val() === "4") {
                    $('.divCommentFor').show();
                }
                else {
                    $('.divCommentFor').hide();
                }
            });
        }
        ShowHideCommentFor();

        $('.btn_reply').on('click', function () {
            if (($('#TaskStatusId').val() === "3" || $('#TaskStatusId').val() === "4") && ($("input[name='CommentFor']:checked").val() === "All")) {
                var status = $('#TaskStatusId').val() === "3" ? "complete" : "closed ";
                var message = 'Are you sure, you would like ' + status +' this task for all participants?';
                if (confirm(message)) {
                    return true;
                }
                else {
                    return false;
                }
            }
            

        });
        $this.init = function () {
            InitForm();
        };
    }

    $(function () {
        var self = new Commentpopup();
        self.init();
    });

}(jQuery));