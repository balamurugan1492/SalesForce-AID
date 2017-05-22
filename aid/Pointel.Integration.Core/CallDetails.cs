
namespace Pointel.Integration.Core
{
    //public class CallDetails
    //{
    //    private  Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
    //      "AID");
    //    public void InsertCallDetails(string connID,string firstName,string lastName,string userName,string thisDN,string ani,string eventName,string callType,string dnis)
    //    {
    //        try{
    //            using (Database dataBase = new Database(DesktopMessenger.GetCallData().PortData.DataProvider))
    //            {
    //                dataBase.ConnectionString = DesktopMessenger.GetCallData().PortData.ConnectionString;
    //                dataBase.OpenDbConnection();
    //                List<System.Data.Common.DbParameter> parameter=new List<System.Data.Common.DbParameter>();
    //                parameter.Add(dataBase.CreateParameter("@connID",connID));
    //                parameter.Add( dataBase.CreateParameter("@firstName",firstName));
    //                parameter.Add(dataBase.CreateParameter("@lastName",lastName));
    //                parameter.Add(dataBase.CreateParameter("@userName",userName));
    //                parameter.Add(dataBase.CreateParameter("@thisDN",thisDN));
    //                parameter.Add(dataBase.CreateParameter("@ani",ani));
    //                parameter.Add(dataBase.CreateParameter("@eventName",eventName));
    //                parameter.Add(dataBase.CreateParameter("@callType",callType));
    //                parameter.Add(dataBase.CreateParameter("@dnis",dnis.ToString()));
    //                if (dataBase.ExecuteNonQuery("sp_insertCallDetails", System.Data.CommandType.StoredProcedure, parameter.ToArray()) > 0)
    //                {
    //                    logger.Info("Call Details inserted to the ConnId : " + connID);
    //                }
    //                else
    //                {
    //                    logger.Info("Call Details not inserted to the ConnId : " + connID);
    //                }

    //            }
            
    //        }
    //        catch(Exception ex)
    //        {
    //            logger.Error(ex.Message);
            
    //        }
    //    }

    //    public void UpdateCallDetails(string connID, string firstName, string lastName, string userName, string thisDN, string ani, string eventName, string callType, string dnis)
    //    {
    //        try
    //        {
    //            using (Database dataBase = new Database(DesktopMessenger.GetCallData().PortData.DataProvider))
    //            {
    //                dataBase.ConnectionString = DesktopMessenger.GetCallData().PortData.ConnectionString;
    //                dataBase.OpenDbConnection();
    //                List<System.Data.Common.DbParameter> parameter = new List<System.Data.Common.DbParameter>();
    //                parameter.Add(dataBase.CreateParameter("@connID", connID));
    //                parameter.Add(dataBase.CreateParameter("@firstName", firstName));
    //                parameter.Add(dataBase.CreateParameter("@lastName", lastName));
    //                parameter.Add(dataBase.CreateParameter("@userName", userName));
    //                parameter.Add(dataBase.CreateParameter("@thisDN", thisDN));
    //                parameter.Add(dataBase.CreateParameter("@ani", ani));
    //                parameter.Add(dataBase.CreateParameter("@eventName", eventName));
    //                parameter.Add(dataBase.CreateParameter("@callType", callType));
    //                parameter.Add(dataBase.CreateParameter("@dnis", dnis.ToString()));
    //                if (dataBase.ExecuteNonQuery("sp_updateCallDetails", System.Data.CommandType.StoredProcedure, parameter.ToArray()) > 0)
    //                {
    //                    logger.Info("Call Details Updated to the ConnId : " + connID);
    //                }
    //                else
    //                {
    //                    logger.Info("Call Details not Updated to the ConnId : " + connID);
    //                }

    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error(ex.Message);
    //        }
    //    }

    //    public void InsertCallUserData(string connID, string userDataKey, string userDatavalue)
    //    {
    //        try
    //        {
    //            using (Database dataBase = new Database(DesktopMessenger.GetCallData().PortData.DataProvider))
    //            {
    //                dataBase.ConnectionString = DesktopMessenger.GetCallData().PortData.ConnectionString;
    //                dataBase.OpenDbConnection();
    //                List<System.Data.Common.DbParameter> parameter = new List<System.Data.Common.DbParameter>();
    //                parameter.Add(dataBase.CreateParameter("@connID", connID));
    //                parameter.Add(dataBase.CreateParameter("@userDataKey", userDataKey));
    //                parameter.Add(dataBase.CreateParameter("@userDataName", userDatavalue));
    //                if (dataBase.ExecuteNonQuery("sp_inserCallUserData", System.Data.CommandType.StoredProcedure, parameter.ToArray()) > 0)
    //                {
    //                    logger.Info("Call Details inserted to the ConnId : " + connID);
    //                }
    //                else
    //                {
    //                    logger.Info("Call Details not inserted to the ConnId : " + connID);
    //                }

    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error(ex.Message);
    //        }
    //    }

    //    public void UpdateCallUserData(string connID, string userDataKey, string userDatavalue)
    //    {
    //        try
    //        {
    //            using (Database dataBase = new Database(DesktopMessenger.GetCallData().PortData.DataProvider))
    //            {
    //                dataBase.ConnectionString = DesktopMessenger.GetCallData().PortData.ConnectionString;
    //                dataBase.OpenDbConnection();
    //                List<System.Data.Common.DbParameter> parameter = new List<System.Data.Common.DbParameter>();
    //                parameter.Add(dataBase.CreateParameter("@connID", connID));
    //                parameter.Add(dataBase.CreateParameter("@userDataKey", userDataKey));
    //                parameter.Add(dataBase.CreateParameter("@userDataName", userDatavalue));
    //                if (dataBase.ExecuteNonQuery("sp_updateCallUserData", System.Data.CommandType.StoredProcedure, parameter.ToArray()) > 0)
    //                {
    //                    logger.Info("Call Details inserted to the ConnId : " + connID);
    //                }
    //                else
    //                {
    //                    logger.Info("Call Details not inserted to the ConnId : " + connID);
    //                }

    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            logger.Error(ex.Message);
    //        }
    //    }
    //}
}
