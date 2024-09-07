<script lang="ts">
import { onDestroy, onMount } from 'svelte';
import { Chart, type ChartData, type ChartOptions } from 'chart.js/auto';

export let filteredData: any[] = [];

let availableColumns = filteredData && filteredData.length > 0 ? Object.keys(filteredData[0]) : [];

let columnX = 'createdAt';
let columnY = 'price';
let columnPoint = 'initialAmount'; 
let chartContainer: HTMLCanvasElement;
let chart: Chart | null = null;

let chartData: ChartData<'line'> = {
    labels: [],
    datasets: [
    {
        label: 'Order Prices',
        data: [],
        borderColor: 'rgba(255, 165, 0, 1)',
        backgroundColor: 'rgba(255, 165, 0, 0.2)',
        fill: true,
        pointRadius: 5,
        pointHoverRadius: 7,
    }
    ]
};

const chartOptions: ChartOptions<'line'> = {
    responsive: true,
    scales: {
        x: { type: 'category' },
        y: { beginAtZero: true },
    },
    plugins: {
        tooltip: {
            callbacks: {
                label: (context) => {
                    const order = filteredData[context.dataIndex];
                    return `${columnPoint}: ${order[columnPoint]}`;
                }
            }
        }
    }
};

function updateChart() {
    if (filteredData && filteredData.length > 0 && chartContainer) {
    chartData.labels = filteredData.map(order => order[columnX]); 
    chartData.datasets[0].data = filteredData.map(order => order[columnY]); 

    if (chart) {
        chart.destroy(); 
    }
    
    chart = new Chart(chartContainer, {
        type: 'line',
        data: chartData,
        options: chartOptions
    });
    }
}

$: if (filteredData && chartContainer) {
    availableColumns = filteredData && filteredData.length > 0 ? Object.keys(filteredData[0]) : [];
    updateChart();
}

onDestroy(() => {
    chart?.destroy(); 
});
</script>

<div>
<label for="columnX">Select X-Axis:</label>
<select id="columnX" bind:value={columnX}>
    {#each availableColumns as column}
    <option value={column}>{column.charAt(0).toUpperCase() + column.slice(1)}</option>
    {/each}
</select>

<label for="columnY">Select Y-Axis:</label>
<select id="columnY" bind:value={columnY}>
    {#each availableColumns as column}
    <option value={column}>{column.charAt(0).toUpperCase() + column.slice(1)}</option>
    {/each}
</select>

<label for="columnPoint">Select Tooltip Point Value:</label>
<select id="columnPoint" bind:value={columnPoint}>
    {#each availableColumns as column}
    <option value={column}>{column.charAt(0).toUpperCase() + column.slice(1)}</option>
    {/each}
</select>
</div>

<canvas bind:this={chartContainer} id="myChart"></canvas>