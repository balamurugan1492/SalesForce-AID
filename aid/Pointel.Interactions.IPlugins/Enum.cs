
namespace Pointel.Interactions.IPlugins
{
    public enum MediaTypes {None, Voice, Email, Chat , OutboundPreview, Interaction};
    public enum Plugins { None, Voice, Email, Chat, Statistic, Workbin, Contact, TeamCommunicator, DispositionCode, OutboundPreview,Salesforce }; 
    public enum ContactDetails { ContactId, Title, FirstName, LastName, PhoneNumber, EmailAddress  };

    public enum InteractionType { None, Voice, Email, Chat, WorkBin, Contact };

    public enum OperationType { None, Transfer, Conference, Call, Consult, Queue, Workbin, Forward, Supervisor };

    public enum AgentMediaStatus
    {
        All,
        Ready,
        ConditionallyReady,
        NotReady,
        LoggedOut,
        Busy,
        LoggedIn,
        None 
    };
}
