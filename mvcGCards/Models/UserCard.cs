using System.ComponentModel.DataAnnotations;

namespace mvcGCards.Models
{
    public class UserCard
    {
        public long Id { get; set; }
        public string UserName {  get; set; }
        public long CardId {  get; set; }
        [Range(0, 100)]
        public int CountDublicate {  get; set; }
    }
}
