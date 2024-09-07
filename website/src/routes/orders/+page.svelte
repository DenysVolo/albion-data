<script lang="ts">
  import { fetchOrders } from '$lib/services/api'; 
	import FilterForm from './FilterForm.svelte';
  import TableFilter from './TableFIlter.svelte';
  import Graph from './Graph.svelte';

  export let data; 
  export let error; 

  let orders = data.orders.orders
  let isLoading = true;

  let isDropdownOpen = false;

  let filteredOrders = orders;

  function handleFilteredDataChange(event: { detail: any[]; }) {
    filteredOrders = event.detail;
  }

  function toggleDropdown() {
    isDropdownOpen = !isDropdownOpen;
  }

  let filters = {
    albionId : '',
    itemTextId : '',
    qualityLevel : '',
    enchantmentLevel : '',
    minPrice : '',
    maxPrice : '',
    minInitialAmount : '',
    maxInitialAmount : '',
    minAmount : '',
    maxAmount : '',
    auctionType : '',
    minExpiryDate : '',
    maxExpiryDate : '',
    locationId : '',
    minCreationDate : '',
    maxCreationDate : '',
    minUpdateDate : '',
    maxUpdateDate : '',
    sessionId : '',
  };

  $: isLoading = data && orders && orders.length === 0;

  async function applyFilters() {
    isLoading = true;
    const response = await fetchOrders(filters);
    orders = response.orders;
    isLoading = false;
  }
</script>

<div class="main-content">
  <section>
    {#if error}
      <p class="error">{error}</p>
    {:else if isLoading}
      <p>Loading data...</p>
    {:else if orders && orders.length > 0}
      <h1>Orders Data</h1>

      <button class="dropdown-button" on:click={toggleDropdown} aria-expanded={isDropdownOpen}>
        Load Data Filter Options
      </button>

      <div class={`dropdown-content ${isDropdownOpen ? 'open' : ''}`}>
        <FilterForm {filters} {applyFilters} />
      </div>

      <TableFilter data={orders} on:filteredDataChange={handleFilteredDataChange} />

      <Graph filteredData={filteredOrders} />
    {:else}
      <p>No data available to display.</p>
    {/if}
  </section>
</div>