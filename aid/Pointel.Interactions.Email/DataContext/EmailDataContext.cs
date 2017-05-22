/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Rajkumar
* Owner      : Pointel Solutions
* =======================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Pointel.Interactions.IPlugins;
using Pointel.Interactions.Email.Helper;


namespace Pointel.Interactions.Email.DataContext
{
    /// <summary>
    /// Class EmailDataContext.
    /// </summary>
    public class EmailDataContext
    {
        #region Data Members and properties
        //Static Data
        private static EmailDataContext objEmailDataContext = new EmailDataContext();
        #endregion

        private EmailDataContext()
        {
            SpeedDialContact = new Hashtable();
            NonEditibleKeys = new List<string>();
            NonEditibleKeys.Add("ContactId");
            NonEditibleKeys.Add("Subject");
            NonEditibleKeys.Add("To");
            NonEditibleKeys.Add("Cc");
            NonEditibleKeys.Add("Bcc");
            NonEditibleKeys.Add("Mailbox");
            NonEditibleKeys.Add("FromPersonal");
            NonEditibleKeys.Add("Email");
            NonEditibleKeys.Add("StartDate");
        }

        public static EmailDataContext GetInstance()
        {
            return objEmailDataContext;
        }

        public System.Windows.Controls.ContextMenu cmshow = new System.Windows.Controls.ContextMenu();

        public Hashtable HshApplicationLevel = new Hashtable(StringComparer.InvariantCultureIgnoreCase);

        public Hashtable hshLoadGroupContact = new Hashtable();

        public string transactionName = string.Empty;

        public IPluginCallBack MessageToClientEmail = null;

        public Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.ConfService ConfigurationServer { get; set; }

        public Hashtable SpeedDialContact { get; set; }

        public string Username { get; set; }

        public string PlaceName { get; set; }

        public string AgentID { get; set; }

        public string ApplicationName { get; set; }

        public int TenantDbId { get; set; }

        public int ProxyClientID { get; set; }

        public List<FromAddresses> MailBox { get; set; }

        public string SpeedDialXMLFile = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\SpeedDial.xml";

        private Dictionary<string, string> _annexContacts = new Dictionary<string, string>();

        public Dictionary<string, string> AnnexContacts
        {
            get
            {
                return _annexContacts;
            }
            set
            {
                if (_annexContacts != value)
                {
                    _annexContacts = value;
                }
            }
        }

        public List<string> NonEditibleKeys
        {
            get;
            set;
        }

        public bool IsContactServerActive
        {
            get;
            set;
        }
        public bool IsVoiceMediaEnabled
        {
            get;
            set;
        }
        public bool isNotifyShowing
        {
            get;
            set;
        }
        public bool isFromAddressPopulated = false;
      
    }
}
