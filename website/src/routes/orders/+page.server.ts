import { fetchOrders } from '$lib/services/api';

export async function load({ url }) {
  const params = {
    albionId: url.searchParams.get('albionId'),
    itemTextId: url.searchParams.get('itemTextId'),
    qualityLevel: url.searchParams.get('qualityLevel'),
    enchantmentLevel: url.searchParams.get('enchantmentLevel'),
    minPrice: url.searchParams.get('minPrice'),
    maxPrice: url.searchParams.get('maxPrice'),
    minInitialAmount: url.searchParams.get('minInitialAmount'),
    maxInitialAmount: url.searchParams.get('maxInitialAmount'),
    minAmount: url.searchParams.get('minAmount'),
    maxAmount: url.searchParams.get('maxAmount'),
    auctionType: url.searchParams.get('auctionType'),
    minExpiryDate: url.searchParams.get('minExpiryDate'),
    maxExpiryDate: url.searchParams.get('maxExpiryDate'),
    locationId: url.searchParams.get('locationId'),
    minCreationDate: url.searchParams.get('minCreationDate'),
    maxCreationDate: url.searchParams.get('maxCreationDate'),
    minUpdateDate: url.searchParams.get('minUpdateDate'),
    maxUpdateDate: url.searchParams.get('maxUpdateDate'),
    limit: url.searchParams.get('limit'),
    sessionId: url.searchParams.get('sessionId'),
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