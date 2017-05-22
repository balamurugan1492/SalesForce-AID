
using Genesyslab.Platform.Commons.Collections;
using Pointel.Integration.Core.iSubjects;
namespace Pointel.Integration.Core.Data
{
    public class FileIntegrationData
    {
        #region Properties

        public string FileName
        {
            get;
            private set;
        }

        public string Directory
        {
            get;
            private set;
        }

        public bool IsEnabled
        {
            get;
            private set;
        }

        public string FileEvent
        {
            get;
            private set;
        }

        public string DataSection
        {
            get;
            private set;
        }

        public string FileFormat
        {
            get;
            private set;
        }

        public string Delimiter
        {
            get;
            private set;
        }

        public string ValueSeperator
        {
            get;
            private set;
        }

        public string KeyValueLeftSymbol
        {
            get;
            private set;
        }

        public string KeyValueRightSymbol
        {
            get;
            private set;
        }

        public string DefaultNullValue
        {
            get;
            private set;
        }

        public KeyValueCollection DataToPost
        {
            get;
            private set;
        }

        public MediaType MediaType
        {
            get;
            private set;
        }
        
        #endregion

        public FileIntegrationData(KeyValueCollection section)
        {
            ValueSeperator = "=";
            Delimiter = "&";
            MediaType = MediaType.Voice;
            KeyValueLeftSymbol = KeyValueRightSymbol = "";
            ParseData(section);
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
                    case "file-name":
                        FileName = section.GetAsString("file-name");
                        break;
                    case "param.value.separator":
                        ValueSeperator = section.GetAsString("param.value.separator");
                        break;
                    case "param.delimiter":
                        Delimiter = section.GetAsString("param.delimiter");
                        break;
                    case "event.type":
                        FileEvent = section.GetAsString("event.type").ToLower();
                        break;
                    case "enable.integration":
                        IsEnabled = System.Convert.ToBoolean(section.GetAsString("enable.integration").ToLower());
                        break;
                    case "directory":
                        Directory = section.GetAsString("directory");
                        break;
                    case "file-format":
                        FileFormat = section.GetAsString("file-format").ToLower();
                        break;
                    case "data-section":
                        DataSection = section.GetAsString("data-section");
                        break;
                    case "default.null-value":
                        DefaultNullValue = section.GetAsString("default.null-value");
                        break;
                    case "enable.popup.channel":
                        if (section.GetAsString("enable.popup.channel").ToLower().Equals("email"))
                            MediaType = MediaType.Email;
                        else if (section.GetAsString("enable.popup.channel").ToLower().Equals("chat"))
                            MediaType = MediaType.Chat;
                        else if (section.GetAsString("enable.popup.channel").ToLower().Equals("sms"))
                            MediaType = MediaType.SMS;
                        break;
                    case "param.keyvalue.cover-symbol":
                        switch (section.GetAsString("param.keyvalue.cover-symbol").ToLower())
                        {
                            case "angle-bracket":
                                KeyValueLeftSymbol = "<";
                                KeyValueRightSymbol = ">";
                                break;
                            case "bracket":
                                KeyValueLeftSymbol = "[";
                                KeyValueRightSymbol = "[";
                                break;
                            case "braces":
                                KeyValueLeftSymbol = "<";
                                KeyValueRightSymbol = ">";
                                break;
                        }
                        break;
                    default:
                        DataToPost.Add(keyName, section[keyName]);
                        break;
                }
            }
        }
    }
}
