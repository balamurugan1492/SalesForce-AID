using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Genesyslab.Platform.Contacts.Protocols.ContactServer;
using Genesyslab.Platform.Contacts.Protocols.ContactServer.Events;
using Pointel.Configuration.Manager;
using Pointel.Interactions.Contact.Core.Common;
using Pointel.Interactions.IPlugins;

namespace Pointel.Interactions.Contact.Helpers
{
    public static class ResponseHelper
    {

        public static EmailSignature GetSignature(string responsePath, ref bool isHTML)
        {
            Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
    "AID");
            //string responseContent = string.Empty;
            EmailSignature emailSignature = new EmailSignature();
            try
            {
                if (responsePath.StartsWith("response"))
                {
                    responsePath = responsePath.Substring(("response:").Length);
                    List<string> path = responsePath.Split('\\').ToList<string>();

                    OutputValues outputMsg = Pointel.Interactions.Contact.Core.Request.RequestGetAllResponse.GetResponseContent(ConfigContainer.Instance().TenantDbId);
                    if (outputMsg.IContactMessage != null)
                    {
                        CategoryList categoryList = null;
                        EventGetAllCategories eventGetAllCategories = (EventGetAllCategories)outputMsg.IContactMessage;
                        _logger.Info("AllCatageories " + eventGetAllCategories.SRLContent.ToString());
                        categoryList = eventGetAllCategories.SRLContent;
                        if (categoryList != null)
                        {

                            StandardResponse response = GetResponseCategories(categoryList, path);
                            if (response == null) return null;
                            //responseContent = string.Empty;
                            //                            responseContent = !string.IsNullOrEmpty(response.StructuredBody) ? response.StructuredBody : !string.IsNullOrEmpty(response.Body) ? response.Body : string.Empty;
                            emailSignature.EmailBody = !string.IsNullOrEmpty(response.StructuredBody) ? response.StructuredBody : !string.IsNullOrEmpty(response.Body) ? response.Body : string.Empty;
                            isHTML = !string.IsNullOrEmpty(response.StructuredBody);
                            emailSignature.Subject = string.IsNullOrEmpty(response.Subject) ? null : response.Subject;
                            if (response.Attachments != null)
                                emailSignature.AttachmentList = response.Attachments;
                        }
                        //LoadResponses(categoryList);
                    }
                }
                else if (responsePath.StartsWith("file"))
                {
                    responsePath = responsePath.Substring(("file:").Length);
                    if (File.Exists(responsePath))
                    {
                        try
                        {
                            using (StreamReader sr = new StreamReader(responsePath))
                            {
                                //responseContent = sr.ReadToEnd();
                                emailSignature.EmailBody = sr.ReadToEnd();
                            }
                        }
                        catch (Exception ex)
                        {
                            //responseContent = string.Empty;
                            emailSignature.EmailBody = string.Empty;
                        }
                    }
                    else
                    {
                        //responseContent = string.Empty;
                        emailSignature.EmailBody = string.Empty;
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred as " + ex.Message);
            }
            finally
            {
                _logger = null;
            }
            //return responseContent.Trim();
            return emailSignature;
        }

        private static StandardResponse GetResponseCategories(CategoryList categoryList, List<string> responsePath)
        {
            Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
    "AID");
            StandardResponseList responselist = null;
            StandardResponse response = null;
            try
            {
                //pointel\Email\actional\actional
                //pointel\Email\emailaccount
                //response:Pointel\Sanofi\Email\Actonal\Sanofi\Email\Actonal
                //pointel\Sanofi\Email\Actonal
                int index = 0;
                foreach (string categoryName in responsePath)
                {
                    //it for find category list, Only if the Category is not last item and should has child category
                    if (responsePath.IndexOf(categoryName) != responsePath.Count - 1 && categoryList != null
                        && categoryList.Count > 0
                        && categoryList.Cast<Category>().Where(c => c.Name.Equals(categoryName)).ToList().Count > 0)
                    {
                        Category category = categoryList.Cast<Category>().Where(c => c.Name.Equals(categoryName)).Single();
                        if (category != null)
                        {
                            categoryList = category.ChildrenCategories;

                            if (index == responsePath.Count - 2)
                            {
                                responselist = category.ChildrenStdResponses;
                                break;
                            }
                        }
                    }
                    else //if the user gives wrong signature path.
                    {
                        break;
                    }
                    index++;
                }

                if (responselist != null && responselist.Count > 0 && responselist.Cast<StandardResponse>().Where(s => s.TheName.Equals(responsePath.LastOrDefault())).ToList().Count > 0)
                {
                    response = responselist.Cast<StandardResponse>().Where(s => s.TheName.Equals(responsePath.LastOrDefault())).Single();
                }


            }
            catch (Exception ex)
            {
                _logger.Error("Error occurred at : " + ((ex.InnerException == null) ? ex.Message.ToString() : ex.InnerException.ToString()));
            }
            finally
            {
                _logger = null;
            }
            return response;
        }
    }
}
