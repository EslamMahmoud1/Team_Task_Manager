debugger;
$(document).ready(function () {

    $("#treeview-kendo").kendoTreeView({
        dataSource: buildTreeData(permissions) ,
        loadOnDemand: false,
        dragAndDrop: false,
        checkboxes: false
    });

    console.log($("#treeview-kendo").data("kendoTreeView"), "out");

});

function buildTreeData(permissions) {

    var tree = {
        id: 100,
        text: "All Permissions",
        expanded: true,
        items: []
    };

    // 🔹 Admin
    var AdminItems = [
        { id: 1, text: "AdminPanel" },
    ].filter(x => permissions.includes(x.id));

    if (AdminItems.length > 0) {
        tree.items.push({
            id: 200,
            text: "Admin",
            expanded: true,
            items: AdminItems
        });
    }
    // 🔹 Dashboard
    var dashboardItems = [
        { id: 2, text: "Analytics" },
        { id: 3, text: "CRM" },
        { id: 4, text: "Logistics" },
        { id: 5, text: "Academy" }
    ].filter(x => permissions.includes(x.id));

    if (dashboardItems.length > 0) {
        tree.items.push({
            id: 300,
            text: "Dashboard",
            expanded: true,
            items: dashboardItems
        });
    }

    // 🔹 Layouts
    var layoutItems = [
        { id: 6, text: "Withoutmenu" },
        { id: 7, text: "Withoutnavbar" },
        { id: 8, text: "Fluid" },
        { id: 9, text: "Container" },
        { id: 10, text: "Blank" }
    ].filter(x => permissions.includes(x.id));

    if (layoutItems.length > 0) {
        tree.items.push({
            id: 400,
            text: "Layouts",
            expanded: true,
            items: layoutItems
        });
    }

    // 🔹 Account Settings
    var accountItems = [
        { id: 11, text: "AccountSettings" },
        { id: 12, text: "Account" },
        { id: 13, text: "Notifications" },
        { id: 14, text: "Connections" }
    ].filter(x => permissions.includes(x.id));

    if (accountItems.length > 0) {
        tree.items.push({
            id: 500,
            text: "AccountSettings",
            expanded: true,
            items: accountItems
        });
    }

    // 🔹 Authentication
    var authItems = [
        { id: 15, text: "Login" },
        { id: 16, text: "Register" },
        { id: 17, text: "ForgotPassword" }
    ].filter(x => permissions.includes(x.id));

    if (authItems.length > 0) {
        tree.items.push({
            id: 600,
            text: "Authentication",
            expanded: true,
            items: authItems
        });
    }

    return [tree];
}