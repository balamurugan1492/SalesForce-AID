
namespace Pointel.Interactions.Contact.Helpers
{

    public enum SearchCondition
    {
        None,
        On,
        OnorAfter,
        Before,
        Contains,
        StartsWith,
        Equals, 
        Is
    }

    public enum MatchCondition
    {
        None,
        MatchAll,
        MatchAny
    }
}
