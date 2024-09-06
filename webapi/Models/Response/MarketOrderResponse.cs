public class MarketOrderResponse {
    public IEnumerable<MarketOrderResponseRow>? Orders { get; set; }
    public string? SessionId { get; set; }
}