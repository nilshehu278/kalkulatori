/**
 * Render a polar area chart of efficiency readings using Chart.js.
 *
 * @param {string} canvasId - ID of canvas element.
 * @param {string[]} labels - Labels (timestamps).
 * @param {number[]} values - Efficiency percentages.
 */
function renderEfficiencyPolarChart(canvasId, labels, values) {
    const canvas = document.getElementById(canvasId);
    if (!canvas) return;

    new Chart(canvas, {
        type: 'polarArea',
        data: {
            labels: labels,
            datasets: [{
                label: 'Efficiency (%)',
                data: values,
                backgroundColor: [
                    'rgba(25, 135, 84, 0.25)',
                    'rgba(13, 110, 253, 0.25)',
                    'rgba(255, 193, 7, 0.25)',
                    'rgba(220, 53, 69, 0.25)',
                    'rgba(111, 66, 193, 0.25)',
                    'rgba(32, 201, 151, 0.25)'
                ],
                borderColor: [
                    'rgba(25, 135, 84, 0.85)',
                    'rgba(13, 110, 253, 0.85)',
                    'rgba(255, 193, 7, 0.85)',
                    'rgba(220, 53, 69, 0.85)',
                    'rgba(111, 66, 193, 0.85)',
                    'rgba(32, 201, 151, 0.85)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
                legend: {
                    position: 'right'
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return `Efficiency: ${context.raw}%`;
                        }
                    }
                }
            },
            scales: {
                r: {
                    beginAtZero: true
                }
            }
        }
    });
}
