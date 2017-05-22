/*
* =====================================
* Pointel.Configuration.Manager.Core.Helpers
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 31-March-2015
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/

namespace Pointel.Configuration.Manager.Helpers
{
    public class ConfigValue
    {
        public string _key = string.Empty;
        public string _value = string.Empty;
        public enum CFGValueObjects { None, Application, AgentGroup, Agent, PersonSkill };
        public enum CFGOperation { Add, Update, Delete };
        public CFGValueObjects _valueObject;
        public static IConfigListener SendToClient = null;


        public string Key
        {
            get
            {
                return _key;
            }
            set
            {
                _key = value;
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }

        public CFGValueObjects ValueObject
        {
            get
            {
                return _valueObject;
            }
            set
            {
                _valueObject = value;
            }
        }

        public void SubscribeConfigObjectNotification(IConfigListener configListener)
        {
            SendToClient = configListener;
        }
    }
}
