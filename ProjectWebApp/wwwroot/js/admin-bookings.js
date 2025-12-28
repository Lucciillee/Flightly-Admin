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
            const searchButton = document.querySelector(".btn.primary");//The script assumes the search button has the class btn primary.
            if (searchButton) {
                e.preventDefault(); // ensures the form doesn’t submit twice.
                searchButton.click();
            }
        }

    });

});
