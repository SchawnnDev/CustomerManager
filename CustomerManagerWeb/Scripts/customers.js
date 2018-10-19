$(function () {
    $.contextMenu({
        selector: ".context-menu-one", // customers

        callback: function (key, options) {

            var clicked = options.$trigger[0].innerText;

            if (clicked == null) return;

            var customer = clicked.match(/\S+/g) || [];
            var id = options.$trigger[0].dataset.customerId;

            switch (key) {

                case "display":
                    showAddressPopup(customer, parseInt(id));
                    break;

                case "edit":
                    showEditCustomerPopup(id);
                    break;

                case "new":
                    showEditCustomerPopup(0);
                    break;

                case "delete":
                    if (confirm("Do you really want to delete customer " + customer[0] + " " + customer[1])) {

                        var antiForgeryToken = $("input[name=__RequestVerificationToken]").val();

                        //Prepare parameters
                        var params = {
                            Id: id
                        };

                        $.postJSON(window.location.href + "Customers/Delete/" + id, params, function success(data) {
                            options.$trigger[0].remove();
                            //$("#shippingAddresses").parentElement.location.reload();
                        }, antiForgeryToken);

                    }
                    break;

            }

        },

        items: {
            "display": { name: "Display shipping addresses", icon: "paste" },
            "sep2": "---------",
            "new": { name: "Create new", icon: "new" },
            "sep3": "---------",
            "edit": { name: "Edit", icon: "edit" },
            "delete": { name: "Delete", icon: "delete" },
        }
    });

    $.contextMenu({
        selector: ".context-menu-two", // shipping addresses

        callback: function (key, options) {

            var clicked = options.$trigger[0].innerText;

            if (clicked == null) return;

            var id = options.$trigger[0].dataset.customerId;
            var addressId = options.$trigger[0].dataset.addressId;

            switch (key) {

                case "new":
                    showEditAddressPopup(id, 0);
                    break;

                case "edit":
                    showEditAddressPopup(id, addressId);
                    break;

                case "delete":
                    if (confirm("Do you really want to delete this shipping address ?")) {

                        var antiForgeryToken = $("input[name=__RequestVerificationToken]").val();

                        //Prepare parameters
                        var params = {
                            Id: addressId
                        };
                        
                        $.postJSON(window.location.href + "ShippingAddresses/Delete/" + id, params, function success(data) {
                           options.$trigger[0].remove();
                            //$("#shippingAddresses").parentElement.location.reload();
                        },antiForgeryToken);


                    }
                    break;

            }

        },

        items: {
            "new": { name: "Create new", icon: "new" },
            "sep2": "---------",
            "edit": { name: "Edit", icon: "edit" },
            "delete": { name: "Delete", icon: "delete" },
        }
    });

});


function showAddressPopup(customer, id) {
    $("#shippingAddresses").load(window.location.href + "ShippingAddresses/Index/" + id).dialog({
        //height: 400,
        title: "Shipping Addresses of " + customer[0] + " " + customer[1],
        width: 500,
        closeOnEscape: true,
        resizable: false,
        modal: true,

        buttons: {
            "Close": function () {
                $(this).dialog("close");
            }

        }
    });
}

function showEditAddressPopup(id, addressId) {
    $("#shippingAddresses").load(window.location.href + "ShippingAddresses/Manage/" + id + "/" + addressId).dialog({
        //height: 400,
        title: "Editing Shipping Address",
        width: 500,
        closeOnEscape: true,
        resizable: false,
        modal: true,

        buttons: {
            "Close": function () {
                $(this).dialog("close");
            }
        }
    });
}

function showEditCustomerPopup(id, addressId) {
    $("#shippingAddresses").load(window.location.href + "Customers/Manage/" + id + "/" + addressId).dialog({
        //height: 400,
        title: "Editing Customers",
        width: 500,
        closeOnEscape: true,
        resizable: false,
        modal: true,

        buttons: {
            "Close": function () {
                $(this).dialog("close");
            }
        }
    });
}

function createNewCustomer() {

}

function createNewShippingAddress(customerId) {

}

jQuery.postJSON = function (url, data, success, antiForgeryToken, dataType) {
    if (dataType === void 0) { dataType = "json"; }
    if (typeof (data) === "object") { data = JSON.stringify(data); }
    var ajax = {
        url: url,
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: dataType,
        data: data,
        success: success
    };
    if (antiForgeryToken) {
        ajax.headers = {
            "__RequestVerificationToken": antiForgeryToken
        };
    };

    return jQuery.ajax(ajax);
};