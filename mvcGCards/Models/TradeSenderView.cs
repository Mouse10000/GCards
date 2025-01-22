namespace mvcGCards.Models
{
    public class TradeSenderView
    {
        public required long TradeId { get; set; }
        public required List<TradeCard> TradeCards { get; set; }
    }
}
