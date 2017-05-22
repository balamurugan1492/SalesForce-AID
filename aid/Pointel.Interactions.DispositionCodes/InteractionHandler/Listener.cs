using System.Collections.Generic;
using System.Windows;
using Pointel.Interactions.DispositionCodes.Settings;
using Pointel.Interactions.DispositionCodes.UserControls;


namespace Pointel.Interactions.DispositionCodes.InteractionHandler
{
    public class Listener
    {

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private Datacontext _datacontext;

        public void NotifyCMEObjects(Dictionary<string, object> CMEObjects)
        {
            _datacontext = new Datacontext();
            if (CMEObjects != null && CMEObjects.Count > 0)
            {
                if (CMEObjects.ContainsKey("voice.disposition.codes"))
                {
                    _datacontext.VoiceDispositionCodes = (Dictionary<string, string>)CMEObjects["voice.disposition.codes"];
                }
                if (CMEObjects.ContainsKey("email.disposition.codes"))
                {
                    _datacontext.EmailDispositionCodes = (Dictionary<string, string>)CMEObjects["email.disposition.codes"];
                }
                if (CMEObjects.ContainsKey("chat.disposition.codes"))
                {
                    _datacontext.ChatDispositionCodes = (Dictionary<string, string>)CMEObjects["chat.disposition.codes"];
                }
                if (CMEObjects.ContainsKey("voice.subdisposition.codes"))
                {
                    _datacontext.VoiceSubDispositionCodes = (Dictionary<string, Dictionary<string, string>>)CMEObjects["voice.subdisposition.codes"];
                }
                if (CMEObjects.ContainsKey("email.subdisposition.codes"))
                {
                    _datacontext.EmailSubDispositionCodes = (Dictionary<string, Dictionary<string, string>>)CMEObjects["email.subdisposition.codes"];
                }
                if (CMEObjects.ContainsKey("chat.subdisposition.codes"))
                {
                    _datacontext.ChatSubDispositionCodes = (Dictionary<string, Dictionary<string, string>>)CMEObjects["chat.subdisposition.codes"];
                }
                if (CMEObjects.ContainsKey("enable.multidisposition.enabled"))
                {
                    _datacontext.IsMultiDispositionEnabled = (bool)CMEObjects["enable.multidisposition.enabled"];
                    //if(_datacontext.IsMultiDispositionEnabled)
                    {
                        _datacontext.SingleDispositionPanelVisibility = Visibility.Collapsed;
                        _datacontext.MultiDispositionPanelVisibility = Visibility.Visible;
                    }
                    //else
                    //{
                    //    _datacontext.SingleDispositionPanelVisibility = Visibility.Visible;
                    //    _datacontext.MultiDispositionPanelVisibility = Visibility.Collapsed;
                    //}
                }
                if (CMEObjects.ContainsKey("DispositionCodeKey"))//DispositionCode Name
                {
                    _datacontext.DispositionCodeKey = CMEObjects["DispositionCodeKey"].ToString();
                }
                if (CMEObjects.ContainsKey("DispositionName"))//First Transaction Object Name
                {
                    _datacontext.DispositionName = CMEObjects["DispositionName"].ToString();
                }
            }
        }

        public Disposition CreateUserControl()
        {
            return new Disposition(_datacontext);
        }
    }
}
