namespace mvcGCards.Models
{
    public class IndexUserViewModel
    {
        public IEnumerable<Card> Cards { get; }
        public PageViewModel PageViewModel { get; }
        public FilterViewModel FilterViewModel { get; }
        public SortViewModel SortViewModel { get; }
        public string UserName { get; }
        public IndexUserViewModel(IEnumerable<Card> cards, PageViewModel pageViewModel,
            FilterViewModel filterViewModel, SortViewModel sortViewModel, string userName)
        {
            Cards = cards;
            PageViewModel = pageViewModel;
            FilterViewModel = filterViewModel;
            SortViewModel = sortViewModel;
            UserName = userName;
        }
    }
}
