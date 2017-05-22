using System.Windows.Media;

namespace Pointel.Interactions.Chat.Helpers
{
    public interface IChatPersonsStatus
    {
        string AgentID { get; set; }
        string PlaceID { get; set; }
        string ChatPersonName { get; set; }
        ImageSource ChatPersonStatusIcon { get; set; }
        string ChatPersonStatus { get; set; } 
    }
}
