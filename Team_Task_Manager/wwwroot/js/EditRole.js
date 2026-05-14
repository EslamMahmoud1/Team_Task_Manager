$(document).ready(function() {
    function isChecked(id) {
        return selectedPermissions.includes(id);
    }
    $("#treeview-kendo").kendoTreeView({
        dataSource: [{
            id: 100, text: "All Permissions", expanded: true, items: [
                {
                    id: 200, text: "Admin", expanded: true, items: [
                        { id: 1, text: "AdminPanel", checked: isChecked(1) },
                    ]
                },
                {
                    id: 300, text: "Dashboard", expanded: true, items: [
                        { id: 2, text: "Analytics", checked: isChecked(2) },
                        { id: 3, text: "CRM", checked: isChecked(3) },
                        { id: 4, text: "Logistics", checked: isChecked(4) },
                        { id: 5, text: "Academy", checked: isChecked(5) },
                    ]
                },

                {
                    id: 300, text: "Layouts", expanded: true, items: [
                        { id: 6, text: "Withoutmenu", checked: isChecked(6) },
                        { id: 7, text: "Withoutnavbar", checked: isChecked(7) },
                        { id: 8, text: "Fluid", checked: isChecked(8) },
                        { id: 9, text: "Container", checked: isChecked(9) },
                        { id: 10, text: "Blank", checked: isChecked(10) },
                    ]
                },
                {
                    id: 400, text: "AccountSettings", expanded: true, items: [
                        { id: 11, text: "AccountSettings", checked: isChecked(11) },
                        { id: 12, text: "Account", checked: isChecked(12) },
                        { id: 13, text: "Notifications", checked: isChecked(13) },
                        { id: 14, text: "Connections", checked: isChecked(14) },
                    ]
                },
                {
                    id: 500, text: "Authentication", expanded: true, items: [
                        { id: 15, text: "Login", checked: isChecked(15) },
                        { id: 16, text: "Register", checked: isChecked(16) },
                        { id: 17, text: "ForgotPassword", checked: isChecked(17) },
                    ]

                }
            ]
        }],
        checkboxes: {
            checkChildren: true
        },
        loadOnDemand: false
    });

    console.log($("#treeview-kendo").data("kendoTreeView"), "out");

});


// Delete button behavior
$(document).on("click", ".kendo-icon", function(e) {
    e.preventDefault();
    var treeview = $("#treeview-kendo").data("kendoTreeView");
    treeview.remove($(this).closest(".k-treeview-item"));
});

$("#SelectedRoleId").on("change", function() {
    var roleId = $(this).val();
    console.log("Selected Role:", roleId);
});

$(document).on('click', '#btnSave', function(e) {
    e.preventDefault();
    var treeview = $("#treeview-kendo").data("kendoTreeView");

    if (!treeview) {
        console.error("TreeView not initialized");
        return;
    }

    function getCheckedNodes(nodes) {
        for (var i = 0; i < nodes.length; i++) {

            if (nodes[i].checked) {

                // Only include leaf nodes (no children)
                if (!nodes[i].hasChildren) {
                    checkedNodes.push(nodes[i].id);
                }
            }

            if (nodes[i].hasChildren) {
                getCheckedNodes(nodes[i].children.view());
            }
        }
    }
    var checkedNodes = [];
    var treeview = $("#treeview-kendo").data("kendoTreeView");

    getCheckedNodes(treeview.dataSource.view());

    console.log(checkedNodes);

    $("#PermissionsIds").val(checkedNodes.join(","));

    var roleName = $('#newRoleName').val().trim();
    $.ajax({
        url: '/Role/Edit',
        type: 'POST',
        data: {
            RoleName: roleName,
            SelectedPermissionIds: checkedNodes
        },
        traditional: true, 
        success: function(res) {
            if (res.success) {
                alert("Role edited successfully!");
                window.location.href = "/Role/Index";

            }
            console.log("Saved successfully");
        }
    });
});
