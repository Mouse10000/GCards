namespace mvcGCards.Models
{
    public class TradeSenderView
    {
        public required int TradeId { get; set; }
        public required List<TradeCard> TradeCards { get; set; }
    }
}
