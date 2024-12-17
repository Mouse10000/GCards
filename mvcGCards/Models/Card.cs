
namespace mvcGCards.Models
{
    public class Card
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfCreate { get; set; }
        public string Description { get; set; }
        public byte[] Image { get; set; }
    }
}
