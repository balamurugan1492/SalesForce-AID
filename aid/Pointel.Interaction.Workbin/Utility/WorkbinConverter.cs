/*
 * ======================================================
 * Pointel.Interaction.Workbin.Utility.WorkbinConverter
 * ======================================================
 * Project    : Agent Interaction Desktop
 * Created on : 3-Feb-2015
 * Author     : Sakthikumar
 * Owner      : Pointel Solutions
 * ======================================================
 */

using System.Collections.Generic;
using Genesyslab.Platform.Commons.Collections;
using Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Core;

namespace Pointel.Interaction.Workbin.Utility
{
    public class WorkbinConverter
    {
        public static List<string> GetWorkbinList()
        {
            //try
            //{
                InteractionService service = new InteractionService();
                Pointel.Interactions.Core.Common.OutputValues result = service.GetWorkbin(ConfigContainer.Instance().TenantDbId, WorkbinUtility.Instance().IxnProxyID);
                List<string> list = new List<string>();
                if (result.MessageCode.Equals("200"))
                {
                    EventWorkbinTypesInfo workbinType = result.IMessage as EventWorkbinTypesInfo;
                    if (workbinType != null)
                    {
                        foreach (string workbinName in workbinType.WorkbinTypes.AllKeys)
                        {
                            KeyValueCollection collection = workbinType.WorkbinTypes[workbinName] as KeyValueCollection;
                            if (collection != null && (int)collection["Active"] == 1)
                            {
                                list.Add(workbinName);
                            }
                        }
                    }
                }
                return list;

            //}
            //catch (Exception ex)
            //{
            //    throw ex;
            //}
        }
    }
}
