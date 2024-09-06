import { fetchOrders } from '$lib/services/api';

export async function load({ url }) {
  
  const params = {
    albionId: url.searchParams.get('albionId'),
    itemNumId: url.searchParams.get('itemNumId'),
    // Add more query parameters 
  };

  try {
    const filteredParams = Object.fromEntries(
      Object.entries(params).filter(([_, value]) => value !== null)
    ) as Record<string, string | number | undefined>;
    
    const orders = await fetchOrders(filteredParams);

    return { orders };
  } catch (error) {
    return { error: (error as Error).message };
  }
}

async function test() {
  return new Promise((fulfil) => {
		setTimeout(fulfil, 1000);
	});
}
