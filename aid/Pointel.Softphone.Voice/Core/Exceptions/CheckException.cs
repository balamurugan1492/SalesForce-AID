#region System Namespace

using System;

#endregion System Namespace

#region Genesys Namespace

using Genesyslab.Platform.Commons.Collections;

#endregion Genesys Namespace

#region AID Namespaces

using Pointel.Softphone.Voice.Core.Util;
using Pointel.Softphone.Voice.Common;

#endregion AID Namespaces

namespace Pointel.Softphone.Voice.Core.Exceptions
{
    /// <summary>
    /// This Class provide relevant exception's for all improper input values given by an agent
    /// </summary>
    internal class CheckException
    {
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        /// <summary>
        /// This method used to validate location,primaryhost,application name,username,password
        /// entered by agent
        /// </summary>
        /// <param name="pLocation">The location.</param>
        /// <param name="pprimaryHost">The primary host.</param>
        /// <param name="pPrimaryPort">The primary port.</param>
        /// <param name="pApplicationName">Name of the application.</param>
        /// <param name="pUserName">Name of the  user.</param>
        /// <param name="pPassword">The password.</param>
        /// <exception cref="System.Exception">location is Invalid or Null</exception>

        #region CheckServerValues

        public static void CheckServerValues(string pLocation, string pprimaryHost, string pPrimaryPort, string pApplicationName, string pUserName,
            string pPassword)
        {
            if (string.IsNullOrEmpty(pLocation))
            {
                throw new Exception("location is Invalid or Null");
            }
            if (string.IsNullOrEmpty(pprimaryHost))
            {
                throw new Exception("Primary configServerHost is Invalid or Null");
            }
            if (string.IsNullOrEmpty(pApplicationName))
            {
                throw new Exception("ApplicationName is Invalid or Null");
            }
            if (string.IsNullOrEmpty(pPrimaryPort))
            {
                throw new Exception("Primary configServerPort is Invalid or Null");
            }
            if (string.IsNullOrEmpty(pUserName))
            {
                throw new Exception("UserName is Invalid or Null");
            }
            if (string.IsNullOrEmpty(pPassword))
            {
                throw new Exception("Password is Invalid or Null");
            }
        }

        #endregion CheckServerValues

        /// <summary>
        /// This method used to validate place and username entered by agent.
        /// </summary>
        /// <param name="pPlace">The place.</param>
        /// <param name="pUserName">Name of the  user.</param>
        /// <param name="pPassword">The password.</param>
        /// <exception cref="System.Exception">Place is Invalid or Null</exception>

        # region CheckLoginValues

        public static void CheckLoginValues(string pPlace, string pUserName)
        {
            if (string.IsNullOrEmpty(pPlace))
            {
                throw new Exception("Place is Invalid or Null");
            }
            if (string.IsNullOrEmpty(pUserName))
            {
                throw new Exception("UserName is Invalid or Null");
            }
        }

        # endregion

        /// <summary>
        /// This method used to validate the number entered by agent.
        /// </summary>
        /// <param name="pOtherDN">The other DN.</param>
        /// <exception cref="System.Exception">OtherDN is Invalid or Null</exception>

        # region CheckDialValues

        public static void CheckDialValues(string pOtherDN)
        {
            if (string.IsNullOrEmpty(pOtherDN))
            {
                throw new Exception("DN is Invalid or Null");
            }
        }

        # endregion

        /// <summary>
        /// This method used to validate thisdn,connectionid,userdata entered by agent
        /// </summary>
        /// <param name="pThisDN">The this DN.</param>
        /// <param name="pConnID">The ConnID</param>
        /// <param name="pUserData">The UserData</param>
        /// <exception cref="System.Exception">DN is Invalid or Null</exception>
        /// <exception cref="System.Exception">ConnectionID is Null</exception>
        /// <exception cref="System.Exception">Userdata is Invalid or Null</exception>

        # region CheckUserDataValues

        public static void CheckUserDataValues(string pThisDN, string pConnID, KeyValueCollection pUserData)
        {
            if (string.IsNullOrEmpty(pThisDN))
            {
                throw new Exception("DN is Invalid or Null");
            }
            if (string.IsNullOrEmpty(pConnID))
            {
                throw new Exception("ConnectionID is Null");
            }
            if (pUserData == null || pUserData.Count <= 0)
            {
                throw new Exception("UserData is Invalid or Null");
            }
        }

        # endregion

        /// <summary>
        /// This method is used to validate username and password entered by agent.
        /// </summary>
        /// <param name="PUsername">The username.</param>
        /// <param name="pPassword">The password.</param>
        /// <exception cref="System.Exception">User name is Invalid or Null</exception>

        # region CheckAuthenticateUser

        public static void CheckAuthenticateUser(string PUsername, string pPassword)
        {
            if (string.IsNullOrEmpty(PUsername))
            {
                throw new Exception("User name is Invalid or Null");
            }
        }

        # endregion

        /// <summary>
        /// This method used to validate whether ACD position and Extension DN is configured in given Place
        /// </summary>
        /// <param name="acdPosition">ACD Position</param>
        /// <param name="extensionDN">Extension DN</param>
        /// <param name="placeName">Place Name</param>

        # region CheckDN

        public static OutputValues CheckDN(string acdPosition, string extensionDN, string placeName)
        {
            OutputValues output = OutputValues.GetInstance();
            if (string.IsNullOrEmpty(extensionDN) && string.IsNullOrEmpty(acdPosition))
            {
                if (Settings.GetInstance().ErrorMessages.ContainsKey("dn.collection"))
                {
                    output.MessageCode = "2001";
                    output.Message = Settings.GetInstance().ErrorMessages["dn.collection"] + "  in " + placeName;
                }
            }
            else if (string.IsNullOrEmpty(acdPosition))
            {
                if (Settings.GetInstance().ErrorMessages.ContainsKey("acd.collection"))
                {
                    output.MessageCode = "2001";
                    output.Message = Settings.GetInstance().ErrorMessages["acd.collection"] + "  in " + placeName;
                }
            }
            else if (string.IsNullOrEmpty(extensionDN))
            {
                if (Settings.GetInstance().ErrorMessages.ContainsKey("extension.collection"))
                {
                    output.MessageCode = "2001";
                    output.Message = Settings.GetInstance().ErrorMessages["extension.collection"] + "  in " + placeName;
                }
            }
            return output;
        }

        # endregion
    }
}