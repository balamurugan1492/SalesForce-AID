
namespace Pointel.Interactions.Chat.Helpers
{
    public interface IPartyInfo
    {
        string UserID { get; set; }

        string UserType { get; set; }

        string UserNickName { get; set; }

        string UserState { get; set; }

        string PersonId { get; set; }

        string Visibility { get; set; }
    }
}
