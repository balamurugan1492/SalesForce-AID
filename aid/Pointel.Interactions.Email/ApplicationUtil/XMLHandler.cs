/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Manikandan
* Owner      : Pointel Solutions
* =======================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;

namespace Pointel.Interactions.Email.ApplicationUtil
{
    /// <summary>
    /// Class XMLHandler.
    /// </summary>
    internal class XMLHandler
    {

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");


        /// <summary>
        /// Loads the XML contacts.
        /// </summary>
        /// <param name="XMLFile">The XML file.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
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
            catch (Exception commonException)
            {
                logger.Error("XML File Not found: " + commonException.Message);
                readUserDetails.Dispose();
            }
            readUserDetails.Dispose();
            return xmlContacts;
        }
    }
}
