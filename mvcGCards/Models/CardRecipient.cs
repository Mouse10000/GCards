﻿namespace mvcGCards.Models
{
    public class CardRecipient
    {
        public int Id { get; set; }
        public required long TradeId { get; set; }
        public required long CardId { get; set; }
    }
}
