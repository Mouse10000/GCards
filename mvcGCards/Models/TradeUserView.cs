namespace mvcGCards.Models
{
    public class TradeUserView
    {
        public long Id { get; set; }
        public required string UserSender { get; set; }
        public required string UserRecipient { get; set; }
        public string? State { get; set; }
        public IEnumerable<string>? Users { get; set; }
    }
}
