using System;
using System.Collections;
using System.ComponentModel;
using System.Linq.Expressions;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.OpenMedia.Protocols;
using Genesyslab.Platform.WebMedia.Protocols;

namespace Pointel.Interactions.Chat.Core.Util
{
    internal class Settings
    {
        #region Single instance
      
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatAccess"/> class.
        /// </summary>
       private static Settings _settings = null;

        internal static Settings GetInstance()
        {
            if (_settings == null)
            {
                _settings = new Settings();
                return _settings;
            }
            return _settings;
        }
        #endregion

        #region Field Declaration
        public event PropertyChangedEventHandler PropertyChanged;
        public static InteractionServerProtocol IxnServerProtocol = null;
        public static IChatListener MessageToClient = null;
        static string _primaryChatServerName = string.Empty;
        static string _secondaryChatServerName = string.Empty;
        static CfgPerson _person;
        static CfgApplication _primaryApplication;
        static CfgApplication _secondaryApplication;
        private static BasicChatProtocol _basicChatProtocol = null;
        private static string _addpServerTimeout = string.Empty;
        private static string _addpClientTimeout = string.Empty;
        static string _queueName = string.Empty;
        static string _warmStandbyTimeout = string.Empty;
        static string _warmStandbyAttempts = string.Empty;
        public Hashtable lstChatColoring = new Hashtable();
        static string _personID = string.Empty;
        static string _nickName = string.Empty;
        static string _nickNameFormat = string.Empty;
        #endregion

        #region INotifyPropertyChange

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChange

        #region Get/Set Properties
        public static string PrimaryChatServerName
        {
            set { _primaryChatServerName = value; }
            get { return _primaryChatServerName; }
        }

        public static string SecondaryChatServerName
        {
            set { _secondaryChatServerName = value; }
            get { return _secondaryChatServerName; }
        }

        public static CfgPerson Person
        {
            set { _person = value; }
            get { return _person; }
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
        public static BasicChatProtocol ChatProtocol 
        {
            set { _basicChatProtocol = value; }
            get { return _basicChatProtocol; }
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
        public static string QueueName
        { 
            set { _queueName = value; }
            get { return _queueName; }
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
        public static string NickName
        {
            set { _nickName = value; }
            get { return _nickName; }
        }
        public static string PersonID
        {
            set { _personID = value; }
            get { return _personID; }
        }
        public static string NickNameFormat
        {
            set { _nickNameFormat = value; }
            get { return _nickNameFormat; }
        }
        #endregion
    }
}
