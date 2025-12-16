// ================================
// ADMIN PROMO CODES - INDEX PAGE
// ================================

document.addEventListener("DOMContentLoaded", () => {

    const search = document.getElementById("promoSearch");
    const table = document.getElementById("promoTable");

    if (!search || !table) return;

    const rows = table.querySelectorAll("tbody tr");

    // -------------------------------
    // Filter Promo Codes
    // -------------------------------
    function filterPromo() {
        const value = search.value.toLowerCase();

        rows.forEach(row => {
            const text = row.textContent.toLowerCase();
            row.style.display = text.includes(value) ? "" : "none";
        });
    }

    // Expose to inline HTML (onclick)
    window.filterPromo = filterPromo;

    search.addEventListener("keyup", filterPromo);

});


// ================================
// DELETE PROMO MODAL
// ================================
function openDeletePromoModal(promoId) {
    const modal = document.getElementById("deletePromoModal");
    const confirmBtn = document.getElementById("confirmDeletePromo");

    confirmBtn.href = "/AdminPromoCodes/Delete/" + promoId;
    modal.classList.add("show");
}

function closeDeletePromoModal() {
    document.getElementById("deletePromoModal").classList.remove("show");
}