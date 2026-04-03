/**
 * Render a line chart of efficiency over time using Chart.js.
 *
 * @param {string} canvasId - ID of canvas element.
 * @param {string[]} labels - X-axis labels (timestamps).
 * @param {number[]} values - Y-axis values (efficiency percentages).
 */
function renderEfficiencyChart(canvasId, labels, values) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    new Chart(canvas, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Efficiency (%)',
                data: values,
                borderColor: '#198754',
                backgroundColor: 'rgba(25, 135, 84, 0.15)',
                tension: 0.3,
                fill: true,
                pointRadius: 3
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y: {
                    beginAtZero: true,
                    title: {
                        display: true,
                        text: 'Efficiency (%)'
                    }
                },
                x: {
                    title: {
                        display: true,
                        text: 'Timestamp'
                    }
                }
            }
        }
    });
}
