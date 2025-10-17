document.addEventListener("DOMContentLoaded", function () {
    const toggle = document.getElementById("menu-toggle");
    const sidebar = document.getElementById("sidebar");

    // Toggle show/hide on button click
    if (toggle && sidebar) {
        toggle.addEventListener("click", function () {
            sidebar.classList.toggle("show");
        });
    }

   // this helps collapsing the side menu bar when not required, simply clicking outside the menu UI
    document.addEventListener("click", function (e) {
        if (
            sidebar.classList.contains("show") &&
            !sidebar.contains(e.target) &&
            e.target.id !== "menu-toggle"
        ) {
            sidebar.classList.remove("show");
        }
    });
});
