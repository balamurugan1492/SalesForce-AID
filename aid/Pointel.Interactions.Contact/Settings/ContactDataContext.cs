using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using Pointel.Interactions.IPlugins;

namespace Pointel.Interactions.Contact.Settings
{
    public delegate string ContactUpdateEvent(string operationType, string contactId, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttributesList attributeList);
    public class ContactDataContext : INotifyPropertyChanged
    {
        #region Single instance

        private static ContactDataContext _instance = new ContactDataContext();

        public static ContactDataContext GetInstance()
        {
            return _instance;
        }

        private ContactDataContext()
        {
            IsInteractionServerActive = true;
            IsTServerActive = true;
        }
        #endregion Single instance

        #region Enums
        public enum AttributeType { Single, Multiple };
        #endregion

        #region Public Members

        public event PropertyChangedEventHandler PropertyChanged;

        public static IPluginCallBack messageToClient = null;

        public Dictionary<string, string> ContactValidAttribute = new Dictionary<string, string>();

        public List<string> ContactDisplayedAttributes = new List<string>();

        public List<string> ContactMandatoryAttributes = new List<string>();

        public List<string> ContactMultipleValueAttributes = new List<string>();

        #endregion Public Members

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

        #endregion INotifyPropertyChabge

        public event ContactUpdateEvent ContactUpdateEventNotification;

        public void NotifyContactUpdate(string operationType, string contactId, Genesyslab.Platform.Contacts.Protocols.ContactServer.AttributesList attributeList)
        {
            if (ContactUpdateEventNotification != null)
                ContactUpdateEventNotification(operationType, contactId, attributeList);
        }

        public bool IsContactServerActive
        {
            get;
            set;
        }


        public bool IsContactIndexFound
        {
            get;
            set;
        }

        public bool IsInteractionIndexFound
        {
            get;
            set;
        }

        public bool IsInteractionServerActive
        {
            get;
            set;
        }

        private bool isTserverActive;
        public bool IsTServerActive
        {
            get
            {
                return isTserverActive;
            }
            set
            {
                if (isTserverActive != value)
                {
                    isTserverActive = value;
                    RaisePropertyChanged(() => IsTServerActive);
                }
            }
        }
        public bool IsEnableCaseDataFromTransaction
        {
            get;
            set;
        }

        public int IxnProxyId
        {
            get;
            set;
        }

        public List<string> OpenedMailIds = new List<string>();

        private string _preloadImage = "\\Agent.Interaction.Desktop;component\\Images\\Loading.GIF";
        public string PreloadImage
        {
            get
            {
                return _preloadImage;
            }
            set
            {
                if (_preloadImage != value)
                {
                    _preloadImage = value;
                    RaisePropertyChanged(() => PreloadImage);
                }
            }
        }

        public bool IsEmailLogon
        {
            get;
            set;
        }


    }
}
