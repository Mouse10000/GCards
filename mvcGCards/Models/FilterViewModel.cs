using Microsoft.AspNetCore.Mvc.Rendering;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace mvcGCards.Models
{
    public class FilterViewModel
    {
        public FilterViewModel(List<string> ranks, string rank, string name)
        {
            // устанавливаем начальный элемент, который позволит выбрать всех
            ranks.Insert(0, "all");
            Ranks = new SelectList(ranks);
            SelectedRank = rank;
            SelectedName = name;
        }
        public SelectList Ranks { get; } // список рангов
        public string SelectedRank { get; } // выбранная ранг
        public string SelectedName { get; } // введенное имя
    }
}
