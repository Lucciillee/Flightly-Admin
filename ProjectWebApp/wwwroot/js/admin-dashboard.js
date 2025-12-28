// ===========================
// ADMIN DASHBOARD CHARTS
// ===========================
document.addEventListener("DOMContentLoaded", () => {

    if (!window.dashboardData) return;//if no data is available, exit the script.
    //extract the data, destructures the object to get labels and data for each chart.
    const {
        revenueLabels,
        revenueData,
        userGrowthLabels,
        userGrowthData,
        routeLabels,
        routeData
    } = window.dashboardData;
    //common chart options used across multiple charts to maintain consistency.
    const baseOpts = {
        responsive: true,
        maintainAspectRatio: false,
        plugins: { legend: { display: false } },
        scales: {
            x: { grid: { display: false } },
            y: { grid: { color: '#f3f4f6' } }
        }
    };

    // ===========================
    // Revenue Trend (Line)
    // ===========================
    new Chart(document.getElementById('revenueChart'), {
        type: 'line',
        data: {
            labels: revenueLabels,
            datasets: [{
                data: revenueData,
                borderColor: '#2563eb',
                backgroundColor: 'rgba(37,99,235,0.1)',
                fill: true,
                tension: 0.4
            }]
        },
        options: baseOpts
    });

    // ===========================
    // User Growth (Bar)
    // ===========================
    new Chart(document.getElementById('userChart'), {
        type: 'bar',
        data: {
            labels: userGrowthLabels,
            datasets: [{
                data: userGrowthData,
                backgroundColor: '#7c3aed',
                borderRadius: 6
            }]
        },
        options: baseOpts
    });

    // ===========================
    // Top Routes (Doughnut)
    // ===========================
    new Chart(document.getElementById('routesChart'), {
        type: 'doughnut',
        data: {
            labels: routeLabels,
            datasets: [{
                data: routeData,
                backgroundColor: [
                    '#2563eb',
                    '#7c3aed',
                    '#f97316',
                    '#22c55e',
                    '#ef4444'
                ],
                borderWidth: 0
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            cutout: '70%',
            plugins: {
                legend: { position: 'bottom' }
            }
        }
    });

});
