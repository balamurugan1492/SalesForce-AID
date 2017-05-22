namespace Pointel.Interactions.Contact.Core.Util
{
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
    using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Contacts.Protocols;

    internal class Settings
    {
        #region Fields

        public static ConfService comObject;
        public static NullableInt tenantDBID;

        static string _addpClientTimeout = string.Empty;
        static string _addpServerTimeout = string.Empty;
        static CfgApplication _primaryApplication;
        static string _primaryContactServerName = null;
        static CfgApplication _secondaryApplication;
        static string _secondaryContactServerName = null;
        static UniversalContactServerProtocol _ucsProtocol;
        static string _warmStandbyAttempts = string.Empty;
        static string _warmStandbyTimeout = string.Empty;

        #endregion Fields

        #region Constructors

        public Settings()
        {
        }

        #endregion Constructors

        #region Properties

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

        public static bool IsConnectionOpened
        {
            get;
            set;
        }

        public static CfgApplication PrimaryApplication
        {
            set { _primaryApplication = value; }
            get { return _primaryApplication; }
        }

        public static string PrimaryContactServerName
        {
            set { _primaryContactServerName = value; }
            get { return _primaryContactServerName; }
        }

        public static CfgApplication SecondaryApplication
        {
            set { _secondaryApplication = value; }
            get { return _secondaryApplication; }
        }

        public static string SecondaryContactServerName
        {
            set { _secondaryContactServerName = value; }
            get { return _secondaryContactServerName; }
        }

        public static UniversalContactServerProtocol UCSProtocol
        {
            set { _ucsProtocol = value; }
            get { return _ucsProtocol; }
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

        #endregion Properties
    }
}