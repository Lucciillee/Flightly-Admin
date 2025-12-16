// ================================
// ADMIN FLIGHTS - INDEX PAGE
// ================================

// Fade out success alert
document.addEventListener("DOMContentLoaded", () => {

    const alert = document.querySelector(".success-alert");
    if (alert) {
        setTimeout(() => {
            alert.style.transition = "0.5s";
            alert.style.opacity = "0";
            alert.style.transform = "translateY(-10px)";
        }, 3000);
    }

});

// -------------------------------
// Cancel Flight Modal
// -------------------------------
function openCancelModal(flightId) {
    const idInput = document.getElementById("cancelFlightId");
    const modal = document.getElementById("cancelModal");

    if (!idInput || !modal) return;

    idInput.value = flightId;
    modal.classList.add("show");
}

function closeCancelModal() {
    const modal = document.getElementById("cancelModal");
    if (modal) {
        modal.classList.remove("show");
    }
}
