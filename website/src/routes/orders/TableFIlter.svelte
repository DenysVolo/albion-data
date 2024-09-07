<script lang="ts">
    import { createEventDispatcher } from 'svelte'; 

    export let data: Array<Record<string, any>> = []; 

    let filters: Record<string, string> = {};
    let filteredData = data;

    let sortedColumn = ''; 
    let sortDirection: 'asc' | 'desc' | null = null;


    const dispatch = createEventDispatcher(); 

    function formatColumnName(column: string): string {
        return column.charAt(0).toUpperCase() + column.slice(1).replace(/([A-Z])/g, ' $1');
    }

    function toggleSort(column: string) {
        if (sortedColumn === column) {
        if (sortDirection === 'asc') {
            sortDirection = 'desc';
        } else if (sortDirection === 'desc') {
            sortedColumn = '';
            sortDirection = null;
        } else {
            sortDirection = 'asc';
        }
        } else {
        sortedColumn = column;
        sortDirection = 'asc';
        }

        sortData();
    }

    function sortData() {
        if (sortedColumn && sortDirection) {
        filteredData = [...filteredData].sort((a, b) => {
            const valueA = a[sortedColumn];
            const valueB = b[sortedColumn];

            if (valueA === valueB) return 0;

            if (sortDirection === 'asc') {
            return valueA > valueB ? 1 : -1;
            } else {
            return valueA < valueB ? 1 : -1;
            }
        });
        } else {
        filteredData = data;
        }
    }

    $: filteredData = data.filter((row) => {
        return Object.keys(filters).every((key) => {
            const filterValue = filters[key]?.toLowerCase() || ''; 
            const rowValue = row[key]?.toString().toLowerCase() || ''; 
            return rowValue.includes(filterValue); 
        });
    });

    $: dispatch('filteredDataChange', filteredData, );

</script>

<div class="table-container" data-simplebar>
    <table>
        <thead>
        <tr>
            {#if data.length > 0}
            {#each Object.keys(data[0]) as column}
                <th class="table-sortable" on:click={() => toggleSort(column)}>
                    {formatColumnName(column)}
                    <span class="arrow {sortedColumn === column ? sortDirection : 'neutral'}"></span>
                </th>
            {/each}
            {/if}
        </tr>
        </thead>

        <thead>
        <tr>
            {#if data.length > 0}
                {#each Object.keys(data[0]) as column}
                    <th>
                    <input
                    class="table-input"
                        type="text"
                        placeholder={`Filter ${formatColumnName(column)}`}
                        bind:value={filters[column]}
                    />
                    </th>
                {/each}
            {/if}
        </tr>
        </thead>

        <tbody>
        {#if filteredData.length > 0}
            {#each filteredData as row}
            <tr>
                {#each Object.keys(row) as column}
                <td>{row[column]}</td>
                {/each}
            </tr>
            {/each}
        {:else}
            <tr>
            <td colspan={Object.keys(data[0]).length}>No matching data found</td>
            </tr>
        {/if}
        </tbody>
    </table>
</div>
  