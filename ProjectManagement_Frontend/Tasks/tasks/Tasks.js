 function toggleSidebar() 
        {
            const sidebar = document.getElementById("mySidebar");
            const content = document.querySelector(".content-wrapper");
            const table = document.querySelector(".user-table");
    if (sidebar.style.width === "250px") {
        sidebar.style.width = "0";
        content.style.marginLeft = "80px";
        table.style.minWidth = "1100px";
        content.style.width = "calc(100% - 80px)";
    } 
    else {
        sidebar.style.width = "250px";
        content.style.marginLeft = "250px";
        table.style.minWidth = "100%";
        content.style.width = "calc(100% - 250px)";
    }
}