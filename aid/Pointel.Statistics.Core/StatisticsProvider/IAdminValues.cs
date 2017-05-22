using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Pointel.Statistics.Core.StatisticsProvider
{
    internal interface IAdminValues
    {
        int MaxTabs
        {
            get;
        }

        int NRReasonCode
        {
            get;
        }

        List<string> AdminUsers
        {
            get;
            set;
        }

        string NRReasonName
        {
            get;
            set;
        }

        Color ExistingColor
        {
            get;
            set;
        }

        Color NewColor
        {
            get;
            set;
        }

        bool AgentNR
        {
            get;
            set;
        }

        Color LogoutColor
        {
            get;
            set;
        }

        Color NRColor
        {
            get;
            set;
        }

        Color ReadyColor
        {
            get;
            set;
        }

        bool AutoAgents
        {
            get;
            set;
        }
    }
}
