$(document).ready(function () {

    $("#treeview-kendo").kendoTreeView({
        dataSource: [{
            id: 100, text: "All Permissions", expanded: true, items: [
                {
                    id: 200, text: "Admin", expanded: true, items: [
                        { id: 1, text: "AdminPanel" },
                    ]
                },
                {
                    id: 300, text: "Dashboard", expanded: true, items: [
                        { id: 2, text: "Analytics" },
                        { id: 3, text: "CRM" },
                        { id: 4, text: "Logistics" },
                        { id: 5, text: "Academy" },
                    ]
                },

                {
                    id: 300, text: "Layouts", expanded: true, items: [
                        { id: 6, text: "Withoutmenu" },
                        { id: 7, text: "Withoutnavbar" },
                        { id: 8, text: "Fluid" },
                        { id: 9, text: "Container" },
                        { id: 10, text: "Blank" },
                    ]
                },
                {
                    id: 400, text: "AccountSettings", expanded: true, items: [
                        { id: 11, text: "AccountSettings" },
                        { id: 12, text: "Account" },
                        { id: 13, text: "Notifications" },
                        { id: 14, text: "Connections" },
                    ]
                },
                {
                    id: 500, text: "Authentication", expanded: true, items: [
                        { id: 15, text: "Login" },
                        { id: 16, text: "Register" },
                        { id: 17, text: "ForgotPassword" },
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
$(document).on("click", ".kendo-icon", function (e) {
    e.preventDefault();
    var treeview = $("#treeview-kendo").data("kendoTreeView");
    treeview.remove($(this).closest(".k-treeview-item"));
});

$("#SelectedRoleId").on("change", function () {
    var roleId = $(this).val();
    console.log("Selected Role:", roleId);
});

$(document).on('click', '#btnSave', function (e) {
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
        url: '/AdminPanel/AssignPermissionsForRole',
        type: 'POST',
        data: {
            RoleName: roleName,
            SelectedPermissionIds: checkedNodes
        },
        traditional: true, // IMPORTANT for arrays
        success: function(res) {
          if (res.success) {
              alert("Permissions assigned successfully!");
              window.location.href = "/Role/Index";

          }
            console.log("Saved successfully");
        },
            error: function(err) {
        console.error(err);
    }
    });
});
