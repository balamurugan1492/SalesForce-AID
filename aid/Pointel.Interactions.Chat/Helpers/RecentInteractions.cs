using System.Windows.Media;

namespace Pointel.Interactions.Chat.Helpers
{
    public class RecentInteractions : IRecentInteractions
    {
        public RecentInteractions(string mediaType, string interactionStarted, string interactionSubject) 
        {
            MediaType = mediaType;
            InteractionStarted = interactionStarted;
            InteractionSubject = interactionSubject;
        }

        public string MediaType { get; set; }

        public string InteractionStarted { get; set; }

        public string InteractionSubject { get; set; }
    }
}
