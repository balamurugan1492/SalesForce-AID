using System.Windows.Media;

namespace Pointel.Interactions.Chat.Helpers
{
    class ChatPersonsStatus : IChatPersonsStatus
    {
        public ChatPersonsStatus(string agentID, string placeID, string chatPersonName, ImageSource chatPersonStatusIcon, string chatPersonStatus) 
        {
            AgentID = agentID;
            PlaceID = placeID;
            ChatPersonName = chatPersonName;
            ChatPersonStatusIcon = chatPersonStatusIcon;
            ChatPersonStatus = chatPersonStatus;
        }

        #region IChatPersonsStatus Members

        public string AgentID { get; set; }

        public string PlaceID { get; set; }

        public string ChatPersonName { get; set; }

        public ImageSource ChatPersonStatusIcon { get; set; }

        public string ChatPersonStatus { get; set; }

        #endregion
    }
}
