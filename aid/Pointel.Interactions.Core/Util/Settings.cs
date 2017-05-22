using System.Collections.Generic;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.OpenMedia.Protocols;


namespace Pointel.Interactions.Core.Util
{
    internal class Settings
    {

        #region Declaration

        private static Settings _instance = null;
        static string _addpServerTimeout = string.Empty;
        static string _addpClientTimeout = string.Empty;
        static string _warmStandbyTimeout = string.Empty;
        static string _warmStandbyAttempts = string.Empty;
        static CfgApplication _primaryApplication;
        static CfgApplication _secondaryApplication;
        static string _primaryInteractionServerName = null;
        static string _secondaryInteractionServerName = null;
        static InteractionServerProtocol _interactionProtocol = null; 
        public static Dictionary<InteractionTypes, IInteractionServices> subscriberObject = new Dictionary<InteractionTypes, IInteractionServices>();
        #endregion

        #region Single Instance
        public static Settings GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Settings();
                return _instance;
            }
            return _instance;
        }
        #endregion
        
        # region Properties
        public static InteractionServerProtocol InteractionProtocol 
        {
            set { _interactionProtocol = value; }
            get { return _interactionProtocol; }
        } 

        public static string AddpClientTimeout
        {
            set { _addpClientTimeout = value; }
            get { return _addpClientTimeout; }
        }

        public static string AddpServerTimeout
        {
            set { _addpServerTimeout = value; }
            get { return _addpServerTimeout; }
        }

        public static string WarmStandbyAttempts
        {
            set { _warmStandbyAttempts = value; }
            get { return _warmStandbyAttempts; }
        }

        public static string WarmStandbyTimeout
        {
            set { _warmStandbyTimeout = value; }
            get { return _warmStandbyTimeout; }
        }

        public static CfgApplication PrimaryApplication
        {
            set { _primaryApplication = value; }
            get { return _primaryApplication; }
        }

        public static CfgApplication SecondaryApplication
        {
            set { _secondaryApplication = value; }
            get { return _secondaryApplication; }
        }
        public static string PrimaryInteractionServerName 
        {
            set { _primaryInteractionServerName = value; }
            get { return _primaryInteractionServerName; }
        } 

        public static string SecondaryInteractionServerName  
        {
            set { _secondaryInteractionServerName = value; }
            get { return _secondaryInteractionServerName; }
        }
        # endregion
    }
}
