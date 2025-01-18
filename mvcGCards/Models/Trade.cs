namespace mvcGCards.Models
{
    public class Trade
    {
        public long Id { get; set; }
        public required string UserSender { get; set; }
        public required string UserRecipient { get; set; }
        public required string State { get; set; }
    }
}
