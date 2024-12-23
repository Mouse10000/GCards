using System.ComponentModel.DataAnnotations;

namespace mvcGCards.Models
{
    public class UserCardModel
    {
        public string UserName { get; set; }
        public Card Card {  get; set; }
        [Range(0, 100)]
        public int Count { get; set; }
    }
}
