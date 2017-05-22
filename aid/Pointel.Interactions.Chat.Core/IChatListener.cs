using Genesyslab.Platform.Commons.Protocols;
using Genesyslab.Platform.WebMedia.Protocols;

namespace Pointel.Interactions.Chat.Core
{
    public interface IChatListener
    {
        void NotifyChatMediaEvents(IMessage message);
        void NotifyChatProtocol(BasicChatProtocol chatprotocol, string nickName);
        void NotifyChatState(string chatProtocolState); 
    }
}
