// ===========================
// SUCCESS ALERT AUTO-HIDE
// ===========================
document.addEventListener("DOMContentLoaded", () => {
    setTimeout(() => {
        const alert = document.getElementById("successAlert");
        if (alert) {
            alert.style.transition = "0.5s";
            alert.style.opacity = "0";
            alert.style.transform = "translateY(-10px)";
        }
    }, 3000);
});

// ===========================
// CANCEL BOOKING MODAL
// ===========================
function openCancelBookingModal(bookingId) {
    const modal = document.getElementById("cancelBookingModal");
    const confirmBtn = document.getElementById("confirmCancelBooking");

    confirmBtn.href = "/AdminBookingDetails/CancelBooking/" + bookingId;
    modal.classList.add("show");
}

function closeCancelBookingModal() {
    document
        .getElementById("cancelBookingModal")
        .classList.remove("show");
}
