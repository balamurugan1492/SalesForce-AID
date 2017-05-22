using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using System.Collections;
using Genesyslab.Platform.OpenMedia.Protocols;
using Genesyslab.Platform.Voice.Protocols;
using System.Windows.Controls;

namespace Pointel.Interactions.IPlugins
{
    public interface IOutboundPlugin
    {
        void InitializeOutbound(string userName, string applicationName, ConfService comObject,IPluginCallBack callBack);

        UserControl GetMyCampaign(bool visibleGetRecord);

        void NotifyInteractionProtocol(InteractionServerProtocol ixnProtocol);

        void NotifyTServerProtocol(TServerProtocol tServerProtocol);

        void NotifyEventMessage(MediaTypes mediaType, IMessage eventMessage);
    }
}
