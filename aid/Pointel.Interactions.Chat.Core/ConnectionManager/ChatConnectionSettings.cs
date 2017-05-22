using System;

namespace Pointel.Interactions.Chat.Core.ConnectionManager
{
    internal class ChatConnectionSettings
    {

        #region Single Instance
        private static ChatConnectionSettings chatConnectionSettings = null;
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static ChatConnectionSettings GetInstance() 
        {
            if (chatConnectionSettings == null)
            {
                chatConnectionSettings = new ChatConnectionSettings();
                return chatConnectionSettings;

            }
            else
            {
                return chatConnectionSettings;
            }
        }
        #endregion

        #region Field Declaration
        public Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.ConfService ComObject;
        public int TenantDBID = Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.WellKnownDbids.EnterpriseModeTenantDbid;
        #endregion
    }
}
