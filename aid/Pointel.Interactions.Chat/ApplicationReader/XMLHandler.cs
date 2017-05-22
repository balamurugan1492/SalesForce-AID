using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Pointel.Interactions.Chat.ApplicationReader
{
    internal class XMLHandler
    {
        #region Load XML Contacts
        //Code added by Manikandan on 26/11/2013
        //For getting the XML Contacts
        public Dictionary<string, string> LoadXmlContacts(string XMLFile)
        {
            Dictionary<string, string> xmlContacts = new Dictionary<string, string>();
            StreamReader readUserDetails = null;
            XmlDocument configXml = new XmlDocument();
            try
            {
                if (readUserDetails == null)
                    readUserDetails = new StreamReader(XMLFile);
                configXml.LoadXml(readUserDetails.ReadToEnd());
                XmlNodeList nodeList = configXml.GetElementsByTagName("Users");
                if (nodeList.Count > 0)
                    foreach (XmlNode node in nodeList)
                    {
                        string[] tr = node.InnerText.ToString().Split('/');
                        xmlContacts.Add(tr[0].ToString(), tr[1].ToString());
                    }
            }
            catch //(Exception commonException)
            {
                //System.Windows.MessageBox.Show("XML File Not found");
                readUserDetails.Dispose();
            }
            readUserDetails.Dispose();
            return xmlContacts;
        }
        #endregion Load XML Contacts
    }
}
