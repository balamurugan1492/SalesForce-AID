using System.Collections.Generic;
namespace Pointel.Integration.PlugIn
{
    public struct ApplicationDataDetails
    {
        #region Fields

        public string ApplicationName;
        public Dictionary<string, string> DataToSent { get; set; }

        #endregion Fields
    }
}