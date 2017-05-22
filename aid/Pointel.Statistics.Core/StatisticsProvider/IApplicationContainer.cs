#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

namespace Pointel.Statistics.Core.StatisticsProvider
{
    /// <summary>
    /// IApplicationContainer
    /// </summary>
    internal interface IApplicationContainer
    {
        #region _errors_

        string ConfigConnection
        {
            get;
            set;
        }

        string PlaceNotFound
        {
            get;
            set;
        }

        string PrimaryServer
        {
            get;
            set;
        }

        string SecondaryServer
        {
            get;
            set;
        }

        string ServerDown
        {
            get;
            set;
        }

        string UserAuthorization
        {
            get;
            set;
        }

        string UserPermission
        {
            get;
            set;
        }

        string NoStats
        {
            get;
            set;
        }

        string dbAuthentication
        {
            get;
            set;
        }

        string dbConnection
        {
            get;
            set;
        }

        #endregion

        #region _system_

        string AdminGroupName
        {
            get;
            set;
        }

        bool SystemDraggable
        {
            get;
            set;
        }

        string UserGroupName
        {
            get;
            set;
        }

        string DisplayTime
        {
            get;
            set;
        }

        bool StatBold
        {
            get;
            set;
        }


        #endregion

        #region enable-disable-channels

        bool EnableAlwaysOnTop
        {
            get;
            set;
        }

        bool EnableCCStatAID
        {
            get;
            set;
        }

        bool EnableLog
        {
            get;
            set;
        }

        bool EnableMainGadget
        {
            get;
            set;
        }

        bool EnableMenuButton
        {
            get;
            set;
        }

        bool EnableMyStatAID
        {
            get;
            set;
        }

        bool EnableTaskbarClose
        {
            get;
            set;
        }

        bool EnableNotificationBalloon
        {
            get;
            set;
        }

        bool EnableMenuShowCCStat
        {
            get;
            set;
        }

        bool EnableMenuClose
        {
            get;
            set;
        }

        bool EnableMenuShowMyStat
        {
            get;
            set;
        }

        bool EnableMenuOnTop
        {
            get;
            set;
        }

        bool EnableTagButton
        {
            get;
            set;
        }

        bool EnableUntagButton
        {
            get;
            set;
        }

        bool EnableTagVertical
        {
            get;
            set;
        }

        bool EnableHHMMSS
        {
            get;
            set;
        }

        bool EnableThresholdNotification
        {
            get;
            set;
        }

        bool EnableSystemDraggable
        {
            get;
            set;
        }

        bool EnableMyQueueStatistics
        {
            get;
            set;
        }

        bool EnableMyQueueConfig
        {
            get;
            set;
        }
        #endregion
    }
}
