
using System.Collections.Generic;
namespace Pointel.Interactions.IPlugins
{
    public interface ITeamCommunicatorPlugin
    {
        //void Subscribe(ITeamCommunicator teamComm);

        void NotifyStatistics(string statHolderID, string statisticsType, Dictionary<string, object> agentStatistics);

        void NotifyStatistics(object statisticsInfo);

        void NotifyCMEObjects(object CMEObjects);

        void NotifyStatServerStatus(string serverStatus);

        //void NoftifyAgentStatus(MediaTypes mediaTypes, AgentMediaStatus agentStatus);

        void Disconnect();
    }
}
