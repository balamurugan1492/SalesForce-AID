#region System Namespaces

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

#endregion System Namespaces

#region log4Net Namespace



#endregion log4Net Namespace

#region AID Namespace

using Pointel.Integration.Core.iSubjects;
using System.Collections;

#endregion AID Namespace

namespace Pointel.Integration.Core.XML_Storage
{
    /// <summary>
    ///
    /// </summary>
    internal class XMLFile
    {
        #region Member Declarations

        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private XmlTextWriter writeUserDetails = null;

        #endregion Member Declarations

        /// <summary>
        /// Saves the initialize parameters.
        /// </summary>
        /// <param name="Filepath">The file path.</param>
        /// <param name="FileName">Name of the file.</param>
        /// <param name="dicHoldingDetails">The dictionary holding details.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>

        #region SaveInitializeParameters
        public bool SaveInitializeParameters( Dictionary<string, string> dicHoldingDetails, iCallData value)
        {
            bool isSaved = false;
            try
            {
                string folder = "";
                if (value.FileData.DirectoryPath != string.Empty)
                    folder = Path.Combine(value.FileData.DirectoryPath, "");
                else
                {
                    logger.Warn("SaveInitializeParameters File path is not specified");
                    folder = Path.Combine(Environment.CurrentDirectory.ToString(), "");
                }
                try
                {
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);
                    string file = "";
                    if (value.FileData.FileName != string.Empty)
                        file = Path.Combine(folder, value.FileData.FileName);
                    else
                    {
                        logger.Warn("SaveInitializeParameters File Name is not specified");
                        file = Path.Combine(folder, "calldata_vd");
                    }
                    writeUserDetails = new XmlTextWriter(file + ".txt", Encoding.Default);
                }
                catch (Exception fileException)
                {
                    logger.Error("Error occurred while writing data in Text file" + fileException.ToString());
                }

                IDictionaryEnumerator Enumerator = dicHoldingDetails.GetEnumerator();
                while (Enumerator.MoveNext())
                    writeUserDetails.WriteElementString(Enumerator.Key.ToString(), Enumerator.Value.ToString());

                writeUserDetails.Close();
                writeUserDetails = null;
            }
            catch (Exception generalException)
            {
                logger.Error("Error occurred " + generalException.ToString());
            }
            return isSaved;
        }
        #endregion SaveInitializeParameters
    }
}