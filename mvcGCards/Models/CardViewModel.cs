using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace mvcGCards.Models
{
    public class CardViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfAdd { get; set; }
        public string ?Description { get; set; }
        public string Rank { get; set; }
        public int Number { get; set; }
        public IFormFile ?Image { get; set; }
    }
}
