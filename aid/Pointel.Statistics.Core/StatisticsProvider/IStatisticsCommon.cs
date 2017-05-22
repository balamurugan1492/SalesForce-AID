#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
#endregion

namespace Pointel.Statistics.Core.StatisticsProvider
{
    /// <summary>
    /// IStatisticsCommon
    /// </summary>
    internal interface IStatisticsCommon
    {
        string Source
        {
            get;
            set;
        }

        int DisplayTime
        {
            get;
            set;
        }

        int Insensitivity
        {
            get;
            set;
        }

        int NotifySeconds
        {
            get;
            set;
        }
        
        string PrimaryServerName
        {
            get;
            set;
        }

        string SecondaryServerName
        {
            get;
            set;
        }

        string TenantName
        {
            get;
            set;
        }
        
        string AgentStatistics
        {
            get;
            set;
        }

        string AgentGroupStatistics
        {
            get;
            set;
        }

        string ACDStatistics
        {
            get;
            set;
        }

        string DNGroupStatistics
        {
            get;
            set;
        }

        string VQueueStatistics
        {
            get;
            set;
        }

        string AgentObjects
        {
            get;
            set;
        }

        string AgentGroupObjects
        {
            get;
            set;
        }

        string ACDObjects
        {
            get;
            set;
        }

        string DNGroupObjects
        {
            get;
            set;
        }

        string VQueueObjects
        {
            get;
            set;
        }

        string Position
        {
            get;
            set;
        }


        string AudioPath
        {
            get;
            set;
        }

        string NotifyBackground
        {
            get;
            set;
        }

        string NotifyForeground
        {
            get;
            set;
        }

        Color DBColor
        {
            get;
            set;
        }

        Color ServerColor
        {
            get;
            set;
        }
        string BusinessAttributeName
        {
            get;
            set;
        }
        string QueueObjectType
        {
            get;
            set;
        }
        string Switches
        {
            get;
            set;
        }

        int MaxObject
        {
            get;
            set;
        }
        string ServerName
        {
            get;
            set;
        }
    
    }
}
