
namespace Pointel.Interactions.IPlugins
{
    public interface ITeamCommunicator
    {

        void DialEvent(InteractionType interactionType, OperationType operationType, string searchedType, string searchedValue, string DN, string place);

        //void RequestDN(string place);

        //void RequestCMEObject(string objectType, string objectId);

        void TeamCommunicatorErrorMessage(string errorMessage);
    }

   
}
