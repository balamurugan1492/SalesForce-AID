namespace Pointel.Desktop.Access.Control
{
    using System.Collections.Generic;

    using Genesyslab.Platform.Commons.Collections;
    using Genesyslab.Platform.Commons.Protocols;

    #region Enumerations

    public enum EventType
    {
        UserDefined, EventRelease, AgentLogin
    }

    #endregion Enumerations

    /// <summary>
    /// This interface notifies desktop with success/error message
    /// </summary>
    public interface IDesktopCommunicator
    {
        #region Methods

        /// <summary>
        /// Notify AID toolbar to be activate, which meand AID become in front of all other applications
        /// </summary>
        void NotifyActivateSoftphone();

        /// <summary>
        /// Notify AID toolbar to be de-activate, which meand AID goes behind of all other applications
        /// </summary>
        void NotifyDeactivateSoftphone();

        /// <summary>
        /// This method notify desktop with error message while sending integrating data
        /// </summary>
        /// <param name="output">Integrating Data</param>
        void NotifyDesktopErrorMessage(string output);

        /// <summary>
        /// This method notify desktop with success message whether the integrating data posted successfully
        /// </summary>
        /// <param name="output">Integrating Data</param>
        void NotifyDesktopSuccessMessage(string output);

        /// <summary>
        /// Notifies the file data.
        /// </summary>
        /// <param name="userData">The user data.</param>
        /// <param name="annexdFacet">The annexd facet.</param>
        /// <param name="isEnableView">if set to <c>true</c> [is enable view].</param>
        void NotifyFileData(KeyValueCollection userData, Dictionary<string, string> annexdFacet, bool isEnableView);

        /// <summary>
        /// Notifies the web URL.
        /// </summary>
        /// <param name="urlString">The URL string.</param>
        /// <param name="typeOfEvent">The type of event.</param>
        /// <param name="webPageName">Name of the web page.</param>
        /// <param name="enableWebPage">if set to <c>true</c> [enable web page].</param>
        /// <param name="enableSingleBrowser">if set to <c>true</c> [enable single browser].</param>
        //void NotifyWebUrl(string urlString, EventType typeOfEvent, string webPageName, bool enableWebPage, bool enableSingleBrowser, bool addressBar, bool statusBar, IMessage objEvent = null);
        void NotifyWebUrl(string urlString, string applicationName, byte count, bool allowNewWindowHook = false, bool overrideSameWindow = false, bool isSurpressScript = false, IMessage objEvent = null);

        /// <summary>
        /// This is used to post the data to particular method.
        /// </summary>
        /// <param name="appName"></param>
        /// <param name="urlToPost"></param>
        /// <param name="dataToPost"></param>
        void PostFormData(string appName, string urlToPost, Dictionary<string, string> dataToPost);

        void PostDataInGVAS(KeyValueCollection userData, string applicationName, string urlString);

        void PostDataToEvas(KeyValueCollection userData, string applicationName, string urlString);

        void PostDataToNvas(KeyValueCollection userData, string applicationName, string urlString);

        #endregion Methods


        #region 

        void NotifyMIDState(bool isVisible, string connectionId);

        //void ShowMIDCollector(string mid, string midurl);

        //void HideMIDCollector();

        //void NotifyHIMMSButtonState(bool isVisible);

        //bool IsAgentInCall();

        #endregion
    }
}