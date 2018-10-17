$(function () {
    $.contextMenu({
        selector: ".context-menu-one",
        
        callback: function (key, options) {

            var m = "clicked: " + key;
            window.console && console.log(m) || alert(m);

            var clicked = options.$trigger[0].innerText;

            if (clicked == null) return;

            var customer = clicked.match(/\S+/g) || [];
            
            switch (key) {

                case "display":
                    console.log("it displays: " + customer);
                    break;

                case "edit":
                    console.log("it edits: " + customer);
                    break;

                case "delete":
                    console.log("it deletes: " + customer);
                    break;

            }

        },

        items: {
            "display": { name: "Display shipping addresses", icon: "paste" },
            "sep2": "---------",
            "edit": { name: "Edit", icon: "edit" },
            "delete": { name: "Delete", icon: "delete" },
            "sep2": "---------",
            "quit": {
                name: "Quit", icon: function () {
                    return 'context-menu-icon context-menu-icon-quit';
                }
            }
        }
    });

    $('.context-menu-one').mousedown(function(e) {
        if (e.which !== 3) return; 
        console.log("clicked", this);
    });
});