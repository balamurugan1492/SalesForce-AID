#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#endregion

#region Pointel Namespace
using Pointel.Statistics.Core.StatisticsProvider;
#endregion

namespace Pointel.Statistics.Core.Utility
{
    internal class ApplicationContainer : IApplicationContainer
    {

        #region Field Declaration

        #region _errors_

        private string _configConnection;
        private string _placeNotFound;
        private string _priServer;
        private string _secServer;
        private string _serverDown;
        private string _userAuthorization;
        private string _userPermission;
        private string _noStats;
        // private string _validInformation;
        private string _dbAuthentication;
        private string _dbConnection;

        #endregion

        #region _system_

        private string _adminAccessGroup;
        private bool _systemDraggable;
        private string _userAccessGroup;
        private string _displayTime;
        private bool _statBold;

        #endregion

        #region enable-disable-channels

        private bool _enableAlwaysOnTop;
        private bool _enableCCStatAid;
        private bool _enableLog;
        private bool _enableMainGadget;
        private bool _enableMenuButton;
        private bool _enableMyStatAid;
        private bool _enableTaskbarClose;
        private bool _enableNotificationBalloon;
        private bool _enableMenuShowCCStat;
        private bool _enableMenuClose;
        private bool _enableMenuShowMyStat;
        private bool _enableMenuOnTop;
        private bool _enableTagButton;
        private bool _enableUntagButton;
        private bool _enableTagVertical;
        private bool _enableHHMMSS;
        private bool _enableThresholdNotification;
        private bool _enableSystemDraggable;
        private bool _enableMyQueueStatistics;
        private bool _enableMyQueueConfig=false;

        #endregion

        #endregion

        #region Property

        #region _errors_

        public string ConfigConnection
        {
            get
            {
                return _configConnection;
            }
            set
            {
                _configConnection = value;
            }
        }

        public string PlaceNotFound
        {
            get
            {
                return _placeNotFound;
            }
            set
            {
                _placeNotFound = value;
            }
        }

        public string PrimaryServer
        {
            get
            {
                return _priServer;
            }
            set
            {
                _priServer = value;
            }
        }

        public string SecondaryServer
        {
            get
            {
                return _secServer;
            }
            set
            {
                _secServer = value;
            }
        }

        public string ServerDown
        {
            get
            {
                return _serverDown;
            }
            set
            {
                _serverDown = value;
            }
        }

        public string UserAuthorization
        {
            get
            {
                return _userAuthorization;
            }
            set
            {
                _userAuthorization = value;
            }
        }

        public string UserPermission
        {
            get
            {
                return _userPermission;
            }
            set
            {
                _userPermission = value;
            }
        }

        public string NoStats
        {
            get
            {
                return _noStats;
            }
            set
            {
                _noStats = value;
            }
        }

        public string dbAuthentication
        {
            get
            {
                return _dbAuthentication;
            }
            set
            {
                _dbAuthentication = value;
            }
        }

        public string dbConnection
        {
            get
            {
                return _dbConnection;
            }
            set
            {
                _dbConnection = value;
            }
        }

        #endregion

        #region _system_

        public string AdminGroupName
        {
            get
            {
                return _adminAccessGroup;
            }
            set
            {
                _adminAccessGroup = value;
            }
        }


        public bool SystemDraggable
        {
            get
            {
                return _systemDraggable;
            }
            set
            {
                _systemDraggable = value;
            }
        }

        public string UserGroupName
        {
            get
            {
                return _userAccessGroup;
            }
            set
            {
                _userAccessGroup = value;
            }
        }

        public string DisplayTime
        {
            get
            {
                return _displayTime;
            }
            set
            {
                _displayTime = value;
            }
        }

        public bool StatBold
        {
            get
            {
                return _statBold;
            }
            set
            {
                _statBold = value;
            }
        }


        #endregion

        #region enable-disable-channels

        public bool EnableAlwaysOnTop
        {
            get
            {
                return _enableAlwaysOnTop;
            }
            set
            {
                _enableAlwaysOnTop = value;
            }
        }

        public bool EnableSystemDraggable
        {
            get
            {
                return _enableSystemDraggable;
            }
            set
            {
                _enableSystemDraggable = value;
            }
        }

        public bool EnableCCStatAID
        {
            get
            {
                return _enableCCStatAid;
            }
            set
            {
                _enableCCStatAid = value;
            }
        }

        public bool EnableLog
        {
            get
            {
                return _enableLog;
            }
            set
            {
                _enableLog = value;
            }
        }

        public bool EnableMainGadget
        {
            get
            {
                return _enableMainGadget;
            }
            set
            {
                _enableMainGadget = value;
            }
        }

        public bool EnableMenuButton
        {
            get
            {
                return _enableMenuButton;
            }
            set
            {
                _enableMenuButton = value;
            }
        }

        public bool EnableMyStatAID
        {
            get
            {
                return _enableMyStatAid;
            }
            set
            {
                _enableMyStatAid = value;
            }
        }

        public bool EnableTaskbarClose
        {
            get
            {
                return _enableTaskbarClose;
            }
            set
            {
                _enableTaskbarClose = value;
            }
        }

        public bool EnableNotificationBalloon
        {
            get
            {
                return _enableNotificationBalloon;
            }
            set
            {
                _enableNotificationBalloon = value;
            }
        }

        public bool EnableMenuShowCCStat
        {
            get
            {
                return _enableMenuShowCCStat;
            }
            set
            {
                _enableMenuShowCCStat = value;
            }
        }

        public bool EnableMenuClose
        {
            get
            {
                return _enableMenuClose;
            }
            set
            {
                _enableMenuClose = value;
            }
        }

        public bool EnableMenuShowMyStat
        {
            get
            {
                return _enableMenuShowMyStat;
            }
            set
            {
                _enableMenuShowMyStat = value;
            }
        }

        public bool EnableMenuOnTop
        {
            get
            {
                return _enableMenuOnTop;
            }
            set
            {
                _enableMenuOnTop = value;
            }
        }

        public bool EnableTagButton
        {
            get
            {
                return _enableTagButton;
            }
            set
            {
                _enableTagButton = value;
            }
        }

        public bool EnableUntagButton
        {
            get
            {
                return _enableUntagButton;
            }
            set
            {
                _enableUntagButton = value;
            }
        }

        public bool EnableTagVertical
        {
            get
            {
                return _enableTagVertical;
            }
            set
            {
                _enableTagVertical = value;
            }
        }

        public bool EnableHHMMSS
        {
            get
            {
                return _enableHHMMSS;
            }
            set
            {
                _enableHHMMSS = value;
            }
        }

        public bool EnableThresholdNotification
        {
            get
            {
                return _enableThresholdNotification;
            }
            set
            {
                _enableThresholdNotification = value;
            }
        }

        public bool EnableMyQueueStatistics
        {
            get 
            {
                return _enableMyQueueStatistics;
            }
            set
            {
                _enableMyQueueStatistics = value;
            }
        }

        public bool EnableMyQueueConfig
        {
            get
            {
                return _enableMyQueueConfig;
            }
            set
            {
                _enableMyQueueConfig = value;
            }
        }

        #endregion

        #endregion
    }
}
