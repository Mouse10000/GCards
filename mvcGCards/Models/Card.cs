using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace mvcGCards.Models
{
    public class Card
    {
        public long Id { get; set; }
        public string Name { get; set; }
        [DataType(DataType.Date)]
        [Remote(action: "CheckDate", controller: "Cards", ErrorMessage = "Enter date of today")]
        public DateTime DateOfAdd { get; set; }
        public string ?Description { get; set; }
        public string Rank { get; set; }
        [Range(0, 1000)]
        public int Number { get; set; }
        public byte[] Image { get; set; }
    }
}
