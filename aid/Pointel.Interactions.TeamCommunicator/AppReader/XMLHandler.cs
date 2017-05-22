/*
* =====================================
* Pointel.Interactions.TeamCommunicator.AppReader
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 05-Sep-2014
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/
#region System Namespaces
using System;
using System.Linq;
using System.IO;
using System.Data;
using System.Xml.Linq;
#endregion

#region Pointel Namespace
using Pointel.Interactions.TeamCommunicator.Settings;
#endregion

namespace Pointel.Interactions.TeamCommunicator.AppReader
{
    class XMLHandler
    {
        #region Private Read only members

        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                    "AID");

        #endregion

        #region Constructor

        public XMLHandler()
        {

        }

        #endregion

        #region Method Definitions

        /// <summary>
        /// Creates the XML file.
        /// </summary>
        public void CreateXMLFile()
        {
            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(Datacontext.GetInstance().CorporateFavoriteFile)))
                    Directory.CreateDirectory(Path.GetDirectoryName(Datacontext.GetInstance().CorporateFavoriteFile));
                if (!File.Exists(Datacontext.GetInstance().CorporateFavoriteFile))
                {
                    _logger.Warn("Corporate Favorite File Not Found");
                    XDocument config = new XDocument(
                           new XDeclaration("1.0", "utf-8", ""),
                           new XElement("UserFavorites"));

                    config.Save(@Datacontext.GetInstance().CorporateFavoriteFile);
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error in creating favorite XMLFile : " + ex.Message.ToString());
            }
        }

        /// <summary>
        /// Creates the XML data.
        /// </summary>
        /// <param name="dtRow">The dt row.</param>
        /// <returns></returns>
        public int CreateXMLData(DataRow dtRow)
        {
            try
            {

                if (File.Exists(Datacontext.GetInstance().CorporateFavoriteFile))
                {
                    System.Xml.Linq.XDocument resultsDoc = System.Xml.Linq.XDocument.Parse((System.Xml.Linq.XDocument.Load(Datacontext.GetInstance().CorporateFavoriteFile)).ToString());
                    var query = resultsDoc.Root;
                    query.Add(new System.Xml.Linq.XElement("Favorites", (new System.Xml.Linq.XElement("Category", dtRow["Category"].ToString())),
                        (new System.Xml.Linq.XElement("DisplayName", dtRow["DisplayName"])),
                        (new System.Xml.Linq.XElement("UniqueIdentity", dtRow["UniqueIdentity"])),
                        (new System.Xml.Linq.XElement("FirstName", dtRow["FirstName"])),
                        (new System.Xml.Linq.XElement("LastName", dtRow["LastName"])),
                        (new System.Xml.Linq.XElement("PhoneNumber", dtRow["PhoneNumber"])),
                        (new System.Xml.Linq.XElement("EmailAddress", dtRow["EmailAddress"])),
                        (new System.Xml.Linq.XElement("Type", dtRow["Type"]))));

                    resultsDoc.Save(Datacontext.GetInstance().CorporateFavoriteFile);
                    return 1;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error in creating favorite XML Data : " + ex.Message.ToString());
            }
            return 0;
        }

        /// <summary>
        /// Updates the XML data.
        /// </summary>
        /// <param name="dtRow">The dt row.</param>
        /// <param name="uniqueIdentity">The unique identity.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public int UpdateXmlData(DataRow dtRow, string uniqueIdentity, string type)
        {
            try
            {
                System.Xml.Linq.XDocument resultsDoc = System.Xml.Linq.XDocument.Parse((System.Xml.Linq.XDocument.Load(Datacontext.GetInstance().CorporateFavoriteFile)).ToString());
                var query = resultsDoc.Root.Descendants("Favorites");
                foreach (System.Xml.Linq.XElement e in query)
                {
                    if (e.Element("UniqueIdentity").Value.ToString() == uniqueIdentity && e.Element("Type").Value.ToString() == type)
                    {
                        e.Element("Category").Value = dtRow["Category"].ToString();
                        e.Element("DisplayName").Value = dtRow["DisplayName"].ToString();
                        e.Element("FirstName").Value = dtRow["FirstName"].ToString();
                        e.Element("LastName").Value = dtRow["LastName"].ToString();
                        e.Element("PhoneNumber").Value = dtRow["PhoneNumber"].ToString();
                        e.Element("EmailAddress").Value = dtRow["EmailAddress"].ToString();
                        e.Element("Type").Value = dtRow["Type"].ToString();
                    }
                }
                resultsDoc.Save(Datacontext.GetInstance().CorporateFavoriteFile);
            }
            catch (Exception error)
            {
                _logger.Error("Error in updating favorite XMLdata : " + error.Message.ToString());
            }
            return 0;
        }

        /// <summary>
        /// Modifies the XML data.
        /// </summary>
        /// <param name="dtRow">The dt row.</param>
        /// <param name="uniqueIdentity">The unique identity.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public int ModifyXmlData(DataRow dtRow, string uniqueIdentity, string type)
        {
            try
            {
                CreateXMLFile();
                System.Xml.Linq.XDocument resultsDoc = System.Xml.Linq.XDocument.Parse((System.Xml.Linq.XDocument.Load(Datacontext.GetInstance().CorporateFavoriteFile)).ToString());
                string query = resultsDoc.Root.Descendants("Favorites").Where(x => x.Element("UniqueIdentity").Value.ToString() == uniqueIdentity
                    && x.Element("Type").Value.ToString() == type
                    ).Select(i => i.Element("UniqueIdentity").Value.ToString()).FirstOrDefault();
                if (query == null)
                {
                    return CreateXMLData(dtRow);
                }
                else
                {
                    return UpdateXmlData(dtRow, uniqueIdentity, type);
                }
            }
            catch (Exception error)
            {
                _logger.Error("Error in modifying favorite XML data : " + error.Message.ToString());
            }
            return 0;
        }

        /// <summary>
        /// Reads the favorite fields.
        /// </summary>
        public void ReadFavoriteFields()
        {
            try
            {
                if (Datacontext.GetInstance().CorporateFavoriteFile == string.Empty)
                    return;
                if (!File.Exists(Datacontext.GetInstance().CorporateFavoriteFile))
                    return;
                System.Xml.Linq.XDocument resultsDoc = System.Xml.Linq.XDocument.Parse((System.Xml.Linq.XDocument.Load(Datacontext.GetInstance().CorporateFavoriteFile)).ToString());
                var query = resultsDoc.Root.Descendants("Favorites");
                foreach (System.Xml.Linq.XElement elements in query)
                {
                    DataRow dtRow = Datacontext.GetInstance().dtFavorites.NewRow();
                    dtRow["Category"] = elements.Element("Category").Value;
                    dtRow["DisplayName"] = elements.Element("DisplayName").Value;
                    dtRow["UniqueIdentity"] = elements.Element("UniqueIdentity").Value;
                    dtRow["FirstName"] = elements.Element("FirstName").Value;
                    dtRow["LastName"] = elements.Element("LastName").Value;
                    dtRow["PhoneNumber"] = elements.Element("PhoneNumber").Value;
                    dtRow["EmailAddress"] = elements.Element("EmailAddress").Value;
                    dtRow["Type"] = elements.Element("Type").Value;
                    Datacontext.GetInstance().dtFavorites.Rows.Add(dtRow);
                    if (dtRow["Category"] as string != null && !string.IsNullOrEmpty((dtRow["Category"] as string).Trim()) &&
                        !Datacontext.GetInstance().CategoryNamesList.Contains(dtRow["Category"].ToString()))
                        Datacontext.GetInstance().CategoryNamesList.Add(dtRow["Category"].ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error in reading favorite XMLFile : " + ex.Message.ToString());
            }
        }

        public int RemoveFavorite(string uniqueIdentity, string type)
        {
            try
            {

                System.Xml.Linq.XDocument resultsDoc = System.Xml.Linq.XDocument.Parse((System.Xml.Linq.XDocument.Load(Datacontext.GetInstance().CorporateFavoriteFile)).ToString());
                var query = resultsDoc.Root.Descendants("Favorites");
                foreach (System.Xml.Linq.XElement e in query)
                {
                    if (e.Element("UniqueIdentity").Value.ToString() == uniqueIdentity && e.Element("Type").Value.ToString() == type)
                    {
                        var collection = resultsDoc.DescendantNodes();
                        e.Remove();
                        resultsDoc.Save(Datacontext.GetInstance().CorporateFavoriteFile);
                        return 1;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error in creating removing XMLFile : " + ex.Message.ToString());
            }
            return 0;
        }

        #endregion

    }
}
