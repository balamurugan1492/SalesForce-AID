using System;
using System.Collections.Generic;
using Pointel.Integration.Core.Manager;

namespace Pointel.Integration.Core.Helper
{
    public class PortDataParser
    {
        #region Data memeber
         private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
          "AID");
        #endregion

         #region Methods
         public RequestType GetRequestType(string data)
         {
             try
             {
                 string[] datas = data.Split('\r', '\n');
                 if (datas.Length > 0)
                 {
                     if (datas[0].StartsWith("GET") && datas[0].EndsWith("HTTP/1.1"))
                     {
                         return RequestType.HTTPGet;
                     }
                 }
             }
             catch (Exception ex)
             {
                 logger.Error( ex.Message);
             }
             return RequestType.TCPClient;
         }

         public string ParseGetMethodData(string data)
         {
             try
             {
                 string[] datas = data.Split('\r', '\n');
                 if (datas.Length > 0)
                 {
                     if (datas[0].StartsWith("GET") && datas[0].EndsWith("HTTP/1.1"))
                     {
                         string[] clientData = datas[0].Split(' ');
                         return clientData[1];
                     }
                 }
             }
             catch (Exception ex)
             {
                 logger.Error(ex.Message);
             }
             return null;
         }

         public Dictionary<string, string> GetData(string rawData,string delimeter)
         {
             try
             {
                 if (!string.IsNullOrEmpty(rawData) && !string.IsNullOrEmpty(delimeter))
                 {
                     Dictionary<string, string> dictionary = new Dictionary<string, string>();
                     string[] data = rawData.Split(delimeter.ToCharArray());
                     foreach (string str in data)
                     {
                         string[] keyValue = str.Split('=');//str.Substring(dataIndex >= 0 ? dataIndex : 0).Split('=');
                         if (keyValue.Length > 1)
                         {
                             dictionary.Add(keyValue[0].Replace(" ",""), keyValue[1]);
                         }
                     }
                     return dictionary;

                 }
             }
             catch (Exception ex)
             {
                 logger.Error( ex.Message);
             }
             return null;
         }

         #endregion


    }
}
