using Genesyslab.Platform.Commons.Collections;
using Pointel.Integration.Core.iSubjects;
namespace Pointel.Integration.Core.Data
{
    public class PipeIntegrationData
    {
        #region Properties

        public string PipeName
        {
            get;
            private set;
        }
        public string PipeFormat
        {
            get;
            private set;
        }
        public string[] PipeEvent
        {
            get;
            private set;
        }
        public string ValueSeperator
        {
            get;
            private set;
        }
        public string Delimeter
        {
            get;
            private set;
        }
        public string DefaultValueToNull
        {
            get;
            set;
        }
        public string DataSection
        {
            get;
            private set;
        }
        public MediaType MediaType
        {
            get;
            private set;
        }
        public bool IsEnabled
        {
            get;
            private set;
        }
        public bool IsServer
        {
            get;
            private set;
        }
        public KeyValueCollection DataToPost
        {
            get;
            private set;
        }

        #endregion

        public PipeIntegrationData(KeyValueCollection configSection)
        {
            PipeFormat = "text";
            MediaType = MediaType.Voice;
            ParseData(configSection);
        }

        private void ParseData(KeyValueCollection section)
        {
            if (section == null)
                return;
            
            DataToPost = new KeyValueCollection();
            foreach (string keyName in section.AllKeys)
            {
                switch (keyName)
                {
                    case "pipe.name":
                        PipeName = section.GetAsString("pipe.name");
                        break;
                    case "param.value.separator":
                        ValueSeperator = section.GetAsString("param.value.separator");
                        break;
                    case "param.delimiter":
                        Delimeter = section.GetAsString("param.delimiter");
                        break;
                    case "popup.event":
                        PipeEvent = section.GetAsString("popup.event").ToLower().Split(',');
                        break;
                    case "enable.integration":
                        IsEnabled = System.Convert.ToBoolean(section.GetAsString("enable.integration").ToLower());
                        break;
                    case "pipe.isserver":
                        IsServer = System.Convert.ToBoolean(section.GetAsString("pipe.isserver").ToLower());
                        break;
                    case "pipe.format":
                        PipeFormat = section.GetAsString("pipe.format").ToLower();
                        break;
                    case "data-section":
                        DataSection = section.GetAsString("data-section");
                        break;
                    case "enable.popup.channel":
                        if (section.GetAsString("enable.popup.channel").ToLower().Equals("email"))
                            MediaType = MediaType.Email;
                        else if (section.GetAsString("enable.popup.channel").ToLower().Equals("chat"))
                            MediaType = MediaType.Chat;
                        else if (section.GetAsString("enable.popup.channel").ToLower().Equals("sms"))
                            MediaType = MediaType.SMS;
                        break;
                    default:
                        DataToPost.Add(keyName, section[keyName]);
                        break;
                }
            }
        }

    }
}
