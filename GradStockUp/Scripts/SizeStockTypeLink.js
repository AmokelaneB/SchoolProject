$(document).ready(function () {
    $("#StkType").change(function () {

        $.ajax({
            type: "GET",
            url: "/SupplierOrder/GetSize",
            data: "{}",
            success: function (data) {
                var s = '<option value="-1">Please Select Size</option>';
                for (var i = 0; i < data.length; i++) {
                    if (data[i].StockTypeID2 == $("#StkType").val())
                        s += '<option value="' + data[i].ID + '">' + data[i].SizeDesc + '</option>';
                }
                $("#sizesDropdown").html(s);
            }
        });
    });
});
