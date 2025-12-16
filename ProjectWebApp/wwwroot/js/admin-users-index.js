// ================================
// ADMIN USERS - INDEX PAGE
// ================================

document.addEventListener("DOMContentLoaded", () => {

    /* ------------------------------
       USERS GROWTH CHART
    ------------------------------ */
    const ctx = document.getElementById("usersChart");

    if (ctx && window.usersChartData) {
        new Chart(ctx, {
            type: "line",
            data: {
                labels: window.usersChartData.labels,
                datasets: [{
                    data: window.usersChartData.counts,
                    fill: true,
                    borderColor: "#0d8bf2",
                    backgroundColor: "rgba(13,139,242,0.08)",
                    tension: 0.4
                }]
            },
            options: {
                maintainAspectRatio: false,
                responsive: true,
                plugins: { legend: { display: false } },
                scales: {
                    y: {
                        beginAtZero: true,
                        ticks: { color: "#6b7280" },
                        grid: { color: "#f1f5f9" }
                    },
                    x: {
                        ticks: { color: "#6b7280" },
                        grid: { display: false }
                    }
                }
            }
        });
    }

    /* ------------------------------
       USERS TABLE SEARCH
    ------------------------------ */
    const searchInput = document.getElementById("userSearch");
    const rows = document.querySelectorAll("#usersTable tbody tr");

    if (searchInput) {
        searchInput.addEventListener("keyup", () => {
            const value = searchInput.value.toLowerCase();

            rows.forEach(row => {
                row.style.display =
                    row.textContent.toLowerCase().includes(value) ? "" : "none";
            });
        });
    }

    const success = document.querySelector(".success-alert");

    if (success) {
        setTimeout(() => {
            success.style.transition = "0.4s";
            success.style.opacity = "0";
            success.style.transform = "translateY(-8px)";
        }, 3000);
    }


});
