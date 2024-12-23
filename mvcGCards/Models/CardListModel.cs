using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace mvcGCards.Models
{
    public class CardListModel
    {
        public IEnumerable<Card> Cards { get; set; } = new List<Card>();
        public SelectList Ranks { get; set; }
        public string? Name { get; set; }
    }
}
