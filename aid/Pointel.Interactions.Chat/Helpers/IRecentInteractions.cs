using System.Windows.Media;

namespace Pointel.Interactions.Chat.Helpers
{
    public interface IRecentInteractions
    {
        string MediaType { get; set; }

        string InteractionStarted { get; set; }

        string InteractionSubject { get; set; }
    }
}
