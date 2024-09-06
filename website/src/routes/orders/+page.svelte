<script lang="ts">
    import { onDestroy } from 'svelte';
    import { Chart, type ChartData, type ChartOptions } from 'chart.js/auto';

  
    export let data; 
    export let error;  
  
    let isLoading = true; 
    let chart: Chart | null = null;
    let chartContainer: HTMLCanvasElement | null = null;
  
    // Initialize chartData with empty values
    const chartData: ChartData<'line'> = {
      labels: [],
      datasets: [
        {
          label: 'Order Prices',
          data: [],
          borderColor: 'rgba(75, 192, 192, 1)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          fill: true,
        }
      ]
    };
  
    // Initialize chartOptions
    const chartOptions: ChartOptions<'line'> = {
      responsive: true,
      scales: {
        x: {
          type: 'category'
        },
        y: {
          beginAtZero: true
        }
      }
    };
  
    $: if (data.orders && data.orders.length > 0 && chartContainer) {
        isLoading = false;
        chartData.labels = data.orders.map((order: { createdAt: string | number | Date; }) => new Date(order.createdAt).toLocaleDateString());
        chartData.datasets[0].data = data.orders.map((order: { price: any; }) => order.price);

        chart?.destroy();
        chart = new Chart(chartContainer, {
            type: 'line',
            data: chartData,
            options: chartOptions
        });
        
    }

    onDestroy(() => {
        chart?.destroy();
    });
    
</script>
  
<section>
    {#if error}
        <p class="error">{error}</p>
    {:else if data.orders && data.orders.length > 0}
        <h1>table</h1>
        <canvas bind:this={chartContainer} id="myChart"></canvas>
    {:else if isLoading}
        <p>Loading data...</p>
    {:else}
        <p>No data available to display.</p>
    {/if}
</section>
  