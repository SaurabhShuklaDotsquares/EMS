/*global jQuery, Global,secureDomain */
(function () {
    function ManageSME() {
        $this = this;

        //when user is of type HR/PM/PMO
        $('#smeInput').on("change", function () {
            debugger;
            var selectedSme = $(this).val();
            if (selectedSme !== "") {
                $.ajax({
                    url: "/SME/GetSmeData",
                    type: "GET",
                    data: { sme: selectedSme },
                    success: function (data) {                      
                        if (data.success) {                           
                            // Populate form fields with data received from the server
                            $("#subjectmatterid").val(data.data.id);
                            $("#Level1").val(data.data.level1);
                            $("#Level2").val(data.data.level2);
                            $("#Level3").val(data.data.level3);
                            $("#Level4").val(data.data.level4);
                            $("#Level5").val(data.data.level5);
                            if (data.data.isActive === true) {
                                $("#IsActive").prop("checked", true);
                            } else {
                                $("#IsActive").prop("checked", false);
                            }
                           
                            $("#saveBtn").text("Edit");
                        } else {
                            // Clear form fields
                            //$("#subjectmatterid").val("");
                            $("#Level1").val("");
                            $("#Level2").val("");
                            $("#Level3").val("");
                            $("#Level4").val("");
                            $("#Level5").val("");
                            // Change button event name to "Save"
                            $("#saveBtn").text("Save");
                        }
                    },
                    error: function (data) {                       
                        console.log("Error fetching SME data.");
                    }
                });
            }
        });


        function initializeForm() {    
            new Global.FormValidationReset('#Form1', { ignore: '.validated' });
            
        };
   
        $this.init = function () {           
            initializeForm();           
        }
    }


    $(function () {
        var self = new ManageSME;
        self.init();
    });


}(jQuery));