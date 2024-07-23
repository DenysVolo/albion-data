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
    public async Task<IEnumerable<MarketOrder>> GetOrders()
    {
        return await _marketRepository.GetMarketOrdersAsync();
    }

    [HttpGet("history")]
    public async Task<IEnumerable<MarketHistory>> GetHistory()
    {
        return await _marketRepository.GetMarketHistoryAsync();
    }
}
