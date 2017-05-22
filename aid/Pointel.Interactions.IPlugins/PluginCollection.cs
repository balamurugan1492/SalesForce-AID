using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pointel.Interactions.IPlugins
{
    public class PluginCollection
    {
        #region Single instance

        private static PluginCollection _instance = null;

        public static PluginCollection GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PluginCollection();
                return _instance;
            }
            return _instance;
        }

        #endregion Single instance

        private Dictionary<IPlugins.Plugins, object> _pluginCollection = new Dictionary<Plugins, object>();

        public Dictionary<IPlugins.Plugins, object> PluginCollections
        {
            get
            {
                return _pluginCollection;
            }
            set
            {
                if (_pluginCollection != value)
                    _pluginCollection = value;
            }
        }

    }
}
