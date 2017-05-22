
namespace Pointel.Interactions.Chat.Helpers
{
    public class PartyInfo : IPartyInfo
    {
        public PartyInfo(string userID,string userType, string userNickName, string userState, string personId, string visibility)
        {
            UserID = userID;
            UserType = userType;
            UserNickName = userNickName;
            UserState = userState;
            PersonId = personId;
            Visibility = visibility;
        }

        public string UserID { get; set; }
        public string UserType { get; set; }
        public string UserNickName { get; set; }
        public string UserState { get; set; }
        public string PersonId { get; set; }
        public string Visibility { get; set; }
    }
}
