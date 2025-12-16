// ================================
// ADMIN ROLES - INDEX PAGE
// ================================

document.addEventListener("DOMContentLoaded", () => {

    const searchInput = document.getElementById('adminSearch');
    const filterSelect = document.getElementById('roleFilter');
    const table = document.getElementById('adminsTable');

    if (!searchInput || !filterSelect || !table) return;

    const rows = table.querySelectorAll('tbody tr');

    function filterTable() {
        const text = searchInput.value.toLowerCase();
        const filter = filterSelect.value;

        rows.forEach(row => {
            const rowText = row.textContent.toLowerCase();
            const role = row.children[2].textContent.trim();
            const status = row.children[3].textContent.trim();

            const matchesFilter =
                filter === "" ||
                role === filter ||
                (filter === "Active" && status === "Active") ||
                (filter === "Blocked" && status === "Blocked");

            row.style.display =
                rowText.includes(text) && matchesFilter ? "" : "none";
        });
    }

    searchInput.addEventListener('keyup', filterTable);
    filterSelect.addEventListener('change', filterTable);

    const success = document.querySelector(".success-alert");

    if (success) {
        setTimeout(() => {
            success.style.transition = "0.4s";
            success.style.opacity = "0";
            success.style.transform = "translateY(-8px)";
        }, 3000);
    }
});
