namespace mvcGCards.Models
{
    public class SortViewModel
    {
        public SortState NameSort { get; } // значение для сортировки по имени
        public SortState NumberSort { get; }    // значение для сортировки по возрасту
        public SortState RankSort { get; }   // значение для сортировки по компании
        public SortState Current { get; }     // текущее значение сортировки

        public SortViewModel(SortState sortOrder)
        {
            NameSort = sortOrder == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
            NumberSort = sortOrder == SortState.NumberAsc ? SortState.NumberDesc : SortState.NumberAsc;
            RankSort = sortOrder == SortState.RankAsc ? SortState.RankDesc : SortState.RankAsc;
            Current = sortOrder;
        }
    }
}
