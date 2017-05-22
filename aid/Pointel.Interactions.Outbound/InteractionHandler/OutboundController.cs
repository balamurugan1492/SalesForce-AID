using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Interactions.IPlugins;
using System.Threading;
using System.Windows;

namespace Pointel.Interactions.Outbound.InteractionHandler
{
    public class OutboundController : IOutboundPlugin
    {

        #region Declaration
        Pointel.Interactions.Outbound.OutboundHandler.OutboundHandler outboundHandler = null;
        #endregion

        #region IOutboundPlugin Members

        public void InitializeOutbound(string userName, string applicationName, Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.ConfService comObject, 
            IPluginCallBack callBack)
        {
            string _userName = userName;
            string appName = applicationName;
            Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.ConfService _comObject = comObject;
            outboundHandler = new OutboundHandler.OutboundHandler();
        }

        public void NotifyInteractionProtocol(Genesyslab.Platform.OpenMedia.Protocols.InteractionServerProtocol ixnProtocol)
        {

        }

        public void NotifyTServerProtocol(Genesyslab.Platform.Voice.Protocols.TServerProtocol tServerProtocol)
        {

        }


        public System.Windows.Controls.UserControl GetMyCampaign(bool visibleGetRecord)
        {
            return new Pointel.Interactions.Outbound.UserControls.UCMyCampaigns(visibleGetRecord);
        }

        public void NotifyEventMessage(MediaTypes mediaType, Genesyslab.Platform.Commons.Protocols.IMessage eventMessage)
        {
            switch (mediaType)
            {
                case MediaTypes.Voice:
                    Genesyslab.Platform.Voice.Protocols.TServer.Events.EventUserEvent userEvent = (Genesyslab.Platform.Voice.Protocols.TServer.Events.EventUserEvent)eventMessage;
                    if (userEvent.UserData != null)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            outboundHandler.NotifyEventIMessage(userEvent.UserData);
                        }));
                    }
                    break;
                case MediaTypes.Interaction:
                    Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventUserEvent eventUserEvent = (Genesyslab.Platform.OpenMedia.Protocols.InteractionServer.Events.EventUserEvent)eventMessage;
                    if (eventUserEvent.PsEventContent != null)
                    {
                        Application.Current.Dispatcher.Invoke((System.Action)(delegate
                        {
                            outboundHandler.NotifyEventIMessage(eventUserEvent.PsEventContent);
                        }));
                    }
                    break;
            }
        }

        #endregion
    }
}
