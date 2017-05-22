#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Pointel.Statistics.Core.Utility
{
    public class StatisticsEnum
    {
        public enum StatSource
        {
            StatServer,
            DB,
            WS,
            All
        }

        public enum StatisticsType
        {
            /// <summary>
            /// 
            /// </summary>
            Agent,
            /// <summary>
            /// 
            /// </summary>
            AgentGroup,
            /// <summary>
            /// 
            /// </summary>
            Global,
            /// <summary>
            /// 
            /// </summary>
            AgentStatus
        }

        public enum GadgetState
        {
            None,
            Opened,
            Ended,
            Closed
        }

        public enum ACDDisplayName
        {
            Queue,
            RoutingPoint,
            Skill,
            VirtualQueue
        }

        public enum DBType
        {
            SQLServer,
            SQLite,
            ORACLE
        }

        public enum StatProperties
        {
            SectionName,
            DisplayName,
            StatName,
            Filter,
            Format,
            TooltipName,
            Color1,
            ThresLevel1,
            ThresLevel2,
            Color2,
            Color3,
        }

        public enum StatisticsObject
        {
            Agent,
            GroupAgents,
            Queue,
            GroupQueues,
            RoutePoint,
        }

        public enum ObjectType
        {
            Agent,
            AgentGroup,
            ACDQueue,
            DNGroup,
            RoutingPoint,
            VirtualQueue
        }

        public enum ThemeColors
        {
            TitleBackground,
            BackgroundColor,
            TitleForeground,
            BorderBrush,
            MouseOver,
            MousePressed,
        }

        public enum SectionName
        {
            acd,
            agent,
            group,
            dn,
            vq,
        }

    }
}
