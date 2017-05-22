/*
* =====================================
* Pointel.Configuration.Manager.Core.ConnectionHandler
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 31-March-2015
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/
namespace Pointel.Configuration.Manager.ConnectionHandler
{
    /// <summary>
    /// Used to Handle Connections for Multiple Users
    /// </summary>
    public class UserConnectionHandler
    {
        #region Single Instance

        private static UserConnectionHandler _instance = null;

        public static UserConnectionHandler GetInstance()
        {
            if (_instance == null)
            {
                _instance = new UserConnectionHandler();
                return _instance;
            }
            return _instance;
        }
        
        #endregion
    }
}
