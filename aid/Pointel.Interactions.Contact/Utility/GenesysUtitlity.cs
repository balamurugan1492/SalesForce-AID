using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.Queries;
using Genesyslab.Platform.Commons.Protocols;
namespace Pointel.Interactions.Contact.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class GenesysUtitlity
    {
        /// <summary>
        /// Gets the full name of the agent.
        /// </summary>
        /// <param name="agentDbId">The agent db id.</param>
        /// <param name="tenantDbId">The tenant db id.</param>
        /// <returns></returns>
        public static string GetAgentFullName(int agentDbId, int tenantDbId)
        {
            if (Pointel.Configuration.Manager.ConfigContainer.Instance().ConfServiceObject.Protocol.State == ChannelState.Opened)
            {
                CfgPersonQuery personQuery = new CfgPersonQuery();
                personQuery.TenantDbid = tenantDbId;
                personQuery.Dbid = agentDbId;
                CfgPerson person = Pointel.Configuration.Manager.ConfigContainer.Instance().ConfServiceObject.RetrieveObject<CfgPerson>(personQuery);
                if (person != null)
                {
                    string name = string.Empty;
                    if (!string.IsNullOrEmpty(person.FirstName))
                        name += person.FirstName;
                    if (!string.IsNullOrEmpty(person.LastName))
                        name += string.IsNullOrEmpty(name) ? "" : " " + person.LastName;
                    return name;
                }
            }
            return "Unidentified Agent";
        }

    }
}
