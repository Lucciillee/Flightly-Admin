// ===========================
// BOOKINGS INDEX KEY HANDLERS
// ===========================
document.addEventListener("DOMContentLoaded", () => {

    document.addEventListener("keydown", (e) => {

        // ESC = Reset filters
        if (e.key === "Escape") {
            window.location.href = "/AdminBookings";
        }

        // ENTER = Trigger Search
        if (e.key === "Enter") {
            const searchButton = document.querySelector(".btn.primary");
            if (searchButton) {
                e.preventDefault(); // prevent double submit
                searchButton.click();
            }
        }

    });

});
