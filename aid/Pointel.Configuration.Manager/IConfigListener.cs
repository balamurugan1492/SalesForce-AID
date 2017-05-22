using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Configuration.Manager.Helpers;

namespace Pointel.Configuration.Manager
{
    public interface IConfigListener
    {
        void NotifyCMEObjectChanged(ConfigValue.CFGValueObjects objectName, KeyValuePair<ConfigValue.CFGOperation, Dictionary<string, string>> changedData);
    }
}
