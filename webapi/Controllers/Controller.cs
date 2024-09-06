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
    public async Task<MarketOrderResponse> GetOrders(
        [FromQuery]MarketOrderRequest request,
        int limit = 100,
        string? sessionId = null)
    {
        return await _marketRepository.GetMarketOrdersAsync(
            request,
            limit,
            sessionId);
    }

    [HttpGet("history")]
    public async Task<IEnumerable<MarketHistory>> GetHistory()
    {
        return await _marketRepository.GetMarketHistoryAsync();
    }
}
