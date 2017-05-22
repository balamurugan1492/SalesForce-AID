using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;
using Genesyslab.Platform.Commons.Protocols;

using Pointel.Interactions.Chat.Core.Util;

namespace Pointel.Interactions.Chat.Core.Listener
{
    internal class BasicChatListener
    {
        # region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private static BasicChatListener basicChatListener = null;
        # endregion

        #region Single Instance
        public static BasicChatListener GetInstance()
        {

            if (basicChatListener == null)
            {
                basicChatListener = new BasicChatListener();
                return basicChatListener;
            }
            else
            {
                return basicChatListener;
            }

        }
        #endregion

        #region BasicChatListener

        /// <summary>
        /// Basics the extractor.
        /// </summary>
        /// <param name="message">The message.</param>
        public void BasicExtractor(object sender, EventArgs e)
        {                
            try
            {        
                IMessage message = ((MessageEventArgs)e).Message;
                Settings.MessageToClient.NotifyChatMediaEvents(message);
            }
            catch (Exception ex)
            {
                logger.Error("BasicExtractor() :" + ex.Message.ToString());
            }
        }
        #endregion

        #region Subscribe
        /// <summary>
        /// Subscribes the agent media status.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void SubscribeChatMediaEvents(IChatListener listener)
        {
            Settings.MessageToClient = listener;
        }
        #endregion
    }
}
