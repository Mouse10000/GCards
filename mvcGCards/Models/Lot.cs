namespace mvcGCards.Models
{
    public class Lot
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public long Cost { get; set; }
        public string Status { get; set; }
        public DateTime DateOfStartAuction { get; set; }
        public DateTime DateOfEndAuction { get; set; }
        public long UserId { get; set; }

    }
}
