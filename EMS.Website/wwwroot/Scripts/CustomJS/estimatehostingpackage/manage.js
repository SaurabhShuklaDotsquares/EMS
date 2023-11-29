(function ($) {

    function initializeModalWithForm() {

        $('#EstimateTechnologyIds').select2();

        $(document).on('change', '#CountryId', function () {
            var $this = $(this);
            $.ajax({
                url: "/estimatehostingpackage/currency",
                data: { countryId: $this.val() },
                success: function (data) {
                    if (data) {
                        $('#CurrencyId').val(data.id);
                        $('.currsymb').html(data.currSign);
                    }
                }
            });
        });
    }

    $(function () {
        initializeModalWithForm();
    });
}(jQuery));