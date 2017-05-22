
namespace Pointel.Interactions.Contact.Helpers
{
  public   class SearchContactList:ISeachContactList 
    {
         public SearchContactList(string searchContactType, string filterContactType, string searchContactText)
        {
            SearchContactType = searchContactType;
            FilterContactType = filterContactType;
            SearchContactText = searchContactText;
        }

        public string SearchContactType { get; set; }

        public string FilterContactType { get; set; }

        public string SearchContactText { get; set; }
    
    }
}
