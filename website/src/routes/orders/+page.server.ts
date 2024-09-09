import { fetchOrders } from '$lib/services/api';

export async function load() {
  try {
    const orders = await fetchOrders({});

    return { orders };
  } catch (error) {
    return { error: (error as Error).message };
  }
}