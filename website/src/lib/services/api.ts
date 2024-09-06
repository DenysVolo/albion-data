export async function fetchOrders(params: Record<string, string | number | undefined>) {
  const url = new URL('/orders', 'http://localhost:5183');

  Object.keys(params).forEach((key) => {
    const value = params[key];
    if (value !== undefined && value !== null) {
      url.searchParams.append(key, String(value));
    }
  });

  const response = await fetch(url.toString());
  if (!response.ok) {
    throw new Error('Failed to fetch orders');
  }
  return await response.json();
}