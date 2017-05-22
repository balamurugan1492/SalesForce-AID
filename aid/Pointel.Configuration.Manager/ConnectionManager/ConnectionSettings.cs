/*
* =====================================
* Pointel.Configuration.Manager.Core.ConnectionManager
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 31-March-2015
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/

using System.Collections.Generic;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;

namespace Pointel.Configuration.Manager.ConnectionManager
{
    class ConnectionSettings
    {
        # region Declaration
        //public static ConfService comObject;
        public const string ConfServer = "config";

        private static Dictionary<string, dynamic> _cmeValues = new Dictionary<string, dynamic>();

        public static Dictionary<string, dynamic> CMEValues
        {
            get
            {
                return _cmeValues;
            }
            set
            {
                _cmeValues = value;
            }
        }

        # endregion
    }
}
