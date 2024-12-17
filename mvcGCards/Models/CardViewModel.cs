namespace mvcGCards.Models
{
    public class CardViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfCreate { get; set; }
        public string Description { get; set; }
        public IFormFile Image { get; set; }
    }
}
