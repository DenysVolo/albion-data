using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class Controller : ControllerBase
{
    private readonly IQueryCatalogue _marketRepository;

    public Controller(IQueryCatalogue marketRepository)
    {
        _marketRepository = marketRepository;
    }

    [HttpGet("orders")]
    public async Task<IEnumerable<MarketOrderResponse>> GetOrders(
        long? albionId = null,
        int? itemNumId = null,
        string? itemTextId = null,
        int? qualityLevel = null,
        int? enchantmentLevel = null,
        long? minPrice = null,
        long? maxPrice = null,
        int? minInitialAmount = null,
        int? maxInitialAmount = null,
        int? minAmount = null,
        int? maxAmount = null,
        string? auctionType = null,
        DateTime? minExpiryDate = null,
        DateTime? maxExpiryDate = null,
        int? locationId = null,
        DateTime? minCreationDate = null,
        DateTime? maxCreationDate = null,
        DateTime? minUpdateDate = null,
        DateTime? maxUpdateDate = null,
        int defaultLimit = 100)
    {
        return await _marketRepository.GetMarketOrdersAsync(
            albionId,
            itemNumId,
            itemTextId,
            qualityLevel,
            enchantmentLevel,
            minPrice,
            maxPrice,
            minInitialAmount,
            maxInitialAmount,
            minAmount,
            maxAmount,
            auctionType,
            minExpiryDate,
            maxExpiryDate,
            locationId,
            minCreationDate,
            maxCreationDate,
            minUpdateDate,
            maxUpdateDate,
            defaultLimit);
    }

    [HttpGet("history")]
    public async Task<IEnumerable<MarketHistory>> GetHistory()
    {
        return await _marketRepository.GetMarketHistoryAsync();
    }
}
