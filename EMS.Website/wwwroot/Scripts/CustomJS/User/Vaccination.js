(function ($) {
    function VaccinationIndex() {
        var $this = this;
        function initilizeForm() {
            var _vaccForm = new Global.FormHelperWithFiles($('#form-vaccination'),
                {
                    updateTargetId: "validation-summary",
                    validateSettings: { ignore: [] }
                }, function (result) {
                    if (result.isSuccess) {
                        loadVaccinationFileGrid();
                       
                        $('#form-vaccination')[0].reset();
                        $('#divDose2').css('display',"none");
                        Global.ShowMessage(result.message, true, 'MessageDiv');
                    }
                    else {
                        Global.ShowMessage(result.message || result.errorMessage || result, false, 'MessageDiv');
                    }
                    window.setTimeout(function () {
                        $('#MessageDiv').html('');
                        $('#MessageDiv').hide();
                    }, 5000);
                });

           

            $('input[name=file]').change(function (ev) {
                readURL(this);
            });
            $("#Dose1Date").datepicker({
                dateFormat: "dd/mm/yy",
                onSelect: function () {
                    $("#Dose1Date").valid();
                }
            });
            $("#Dose2Date").datepicker({
                dateFormat: "dd/mm/yy",
                onSelect: function () {
                    $("#Dose2Date").valid();
                }
            });
            $('.dose-group').on('click', function () {
                if ($(this).val() == "1") {
                    $('#divDose1').show();
                    $('#divDose2').hide();
                }
                else if ($(this).val() == "2") {
                    $('#divDose1').show();
                    $('#divDose2').show();
                }
            });
        }

        function readURL(input) {
            if (input.files && input.files[0]) {
                var ext = input.value.split('.').pop().toLowerCase();
                if ($.inArray(ext, ['pdf']) == -1) {
                    input.value = '';
                    Global.ShowMessage("Invalid extensions, please upload vaccination certificate in pdf format only.", false, 'MessageDiv');
                    window.setTimeout(function () {
                        $('#MessageDiv').html('');
                        $('#MessageDiv').hide();
                    }, 5000);
                    return false;
                }

                if (Math.round(input.files[0].size / (1024 * 1024)) > 8) {
                    input.value = '';
                    Global.ShowMessage('Max Upload image size is 8MB only', false, 'MessageDiv');
                    window.setTimeout(function () {
                        $('#MessageDiv').html('');
                        $('#MessageDiv').hide();
                    }, 5000);
                    return false;
                }
                else {
                    $("#file").valid();
                    var reader = new FileReader();
                    reader.readAsDataURL(input.files[0]);
                }

            }
        }
        //$('#btnSearch').click(function () {
        //    loadVaccinationFileGrid();
        //})
        function loadVaccinationFileGrid() {

            // var data1 = { txtSearch: $('#txtSearch').val() }
            var vaccinationDocGrid = new Global.GridHelper('#grid-Vaccination', {
                serverSide: true,
                destroy: true,
                "bAutoWidth": false,
                "pageLength": 50,
                "bFilter": false,
                "bPaginate": false,
                "bFilter": false, //hide Search bar
                "bInfo": false, // hide showing entries
                ajax:
                {
                    url: domain + "Vaccination/VaccinationFiles",
                    type: "POST",
                    //data: data1,
                },
                order: [1, "desc"],
                "columnDefs": [
                    { "width": "0%", "targets": 0 },
                    { "width": "3%", "targets": 1 },
                    { "width": "15%", "targets": 2 },
                ],
                columns:
                    [
                        { name: "id", data: "id", title: "id", sortable: false, searchable: false, visible: false },
                        { name: "rowIndex", data: "rowIndex", title: "#", sortable: false, searchable: false, visible: true },

                        {
                            name: "vaccineName", data: "vaccineName", title: "Vaccine", sortable: true, searchable: false, visible: true,
                        },
                        {
                            name: "doseType", data: "doseType", title: "Vaccination Status", sortable: false, searchable: false, visible: true,
                        },
                        {
                            name: "addedDate", data: "addedDate", title: "Added Date", sortable: true, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {

                                return moment(dataRow.addedDate).format('DD-MMM-YYYY');

                            }
                        },
                        {
                            name: "updatedCertificate", data: null, title: "Vaccination Certificate", sortable: false, searchable: false, visible: true,
                            render: function (data, type, dataRow, meta) {
                                var Link = "";
                                if (dataRow.updatedCertificate != null) {
                                    Link += '<a download style="color:#101ee5;text-decoration:underline;" href="Upload/Vaccination_Files/' + data.updatedCertificate + '" target="_blank">' + data.updatedCertificate + '</a></br>';
                                }

                                return Link

                            }
                        },

                    ],


                "fnDrawCallback": function (oSettings) {
                    //if (oSettings._iDisplayLength > oSettings.fnRecordsDisplay()) {
                    //    $('.dataTables_paginate').hide();
                    //}
                    //else {
                    //    $('.dataTables_paginate').show();
                    //}
                    //$('.pagination .active a').css('background-color', '#e99701');
                    //$('.pagination .active a').css('border-color', '#e99701');

                }
            })
            return vaccinationDocGrid;
        }

        $this.init = function () {
            initilizeForm();
            loadVaccinationFileGrid();
        };
    }

    $(function () {
        var self = new VaccinationIndex();
        self.init();
    });
}(jQuery));