<script lang="ts">
    export let filters: Record<string, string | number>; 
    export let applyFilters: () => void;
  
    function getInputType(key: string) {
      if (key.toLowerCase().includes("date")) return "date";
      if (key.toLowerCase().includes("price") || key.toLowerCase().includes("amount")) return "number";
      return "text";
    }
  </script>
  
  <form on:submit|preventDefault={applyFilters}>
    <div class="filter-container">
      {#each Object.keys(filters) as key (key)}
        <label class="filter-element" for={key}>{key.charAt(0).toUpperCase() + key.slice(1).replace(/([A-Z])/g, ' $1')}: </label>
  
        {#if getInputType(key) === 'text'}
          <input class="filter-element" id={key} type="text" bind:value={filters[key]} placeholder={"Filter " + key.charAt(0).toUpperCase() + key.slice(1)} />
        {:else if getInputType(key) === 'number'}
          <input class="filter-element" id={key} type="number" bind:value={filters[key]} placeholder={"Filter " + key.charAt(0).toUpperCase() + key.slice(1)} />
        {:else if getInputType(key) === 'date'}
          <input class="filter-element" id={key} type="date" bind:value={filters[key]} placeholder={"Filter " + key.charAt(0).toUpperCase() + key.slice(1)} />
        {/if}
      {/each}
    </div>
    <div class="filter-button-container">
        <button class="filter-menu-button" type="submit">Load Data With Filters</button>
    </div>
  </form>
  