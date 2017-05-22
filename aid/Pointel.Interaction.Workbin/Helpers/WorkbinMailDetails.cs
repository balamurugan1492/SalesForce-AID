/*
 * ======================================================
 * Pointel.Interaction.Workbin.Helpers.WorkbinMailDetails
 * ======================================================
 * Project    : Agent Interaction Desktop
 * Created on : 3-Feb-2015
 * Author     : Sakthikumar
 * Owner      : Pointel Solutions
 * ======================================================
 */

using System.Collections.Generic;
using System.Windows.Media;
using Genesyslab.Platform.Commons.Collections;

namespace Pointel.Interaction.Workbin.Helpers
{
    public class WorkbinMailDetails
    {
        #region Private Data Members
        
        #endregion

        public WorkbinMailDetails()
        {
            Attachment = new List<AttachmentDetails>();
            CaseData = new List<EmailCaseData>();
            DisplayValue = new List<string>();
        }
        public string AgentId
        {
            get;
            set;
        }

        public string InteractionId
        {
            get;
            set;
        }

        public string ParentInteractionId
        {
            get;
            set;
        }

        public string ThreadId
        {
            get;
            set;
        }

        public ImageSource MailImage
        {
            get;
            set;
        }

        public string WorkbinName
        {
            get;
            set;
        }

        public string TypeId
        {
            get;
            set;
        }

        public string SubTypeId
        {
            get;
            set;
        }


        public string State
        {
            get;
            set;
        }

        public string From
        {
            get;
            set;
        }

        public string To
        {
            get;
            set;
        }

        public string CC
        {
            get;
            set;
        }

        public string BCC
        {
            get;
            set;
        }

        public string Subject
        {
            get;
            set;
        }

        public string ReceivedDate
        {
            get;
            set;
        }

        public string StartDate
        {
            get;
            set;
        }

        public string MailState
        {
            get;
            set;
        }

        public List<AttachmentDetails> Attachment
        {
            get;
            set;
        }

        public string MailBody
        {
            get;
            set;
        }

        public string EmailNotes
        {
            get;
            set;
        }

        public List<EmailCaseData> CaseData
        {
            get;
            set;
        }

        public string DispositionKey
        {
            get;
            set;
        }

        public KeyValueCollection UserData
        {
            get;
            set;
        }

        public List<string> DisplayValue
        {
            get;
            set;
        }

        public string InterationState { get; set; }
         
    }
}
