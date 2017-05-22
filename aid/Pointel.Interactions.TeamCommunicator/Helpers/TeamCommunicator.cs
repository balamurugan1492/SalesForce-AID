/*
* =====================================
* Pointel.Interactions.TeamCommunicator.Helpers
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 05-Sep-2014
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/
using System.Windows.Media;
using Pointel.Interactions.TeamCommunicator.Settings;

namespace Pointel.Interactions.TeamCommunicator.Helpers
{
    public class TeamCommunicator : ITeamCommunicator
    {

        public TeamCommunicator(string imgTooltip, ImageSource searchIconImageSource, string searchedName,
            ImageSource statusImageSource, ImageSource mediaImageSource, Pointel.Interactions.IPlugins.InteractionType mediaType, string status, ImageSource detailImageSource,
            string toolTip, Datacontext.SelectorFilters searchedType, string dn, string uniqueIdentity, ImageSource favoriteImageSource, string favoriteToolTip,string emailAddress="")
        {
            ImageToolTip = imgTooltip;
            SearchIconImageSource = searchIconImageSource;
            SearchedName = searchedName;
            StatusImageSource = statusImageSource;
            MediaImageSource = mediaImageSource;
            MediaType = mediaType;
            Status = status;
            DetailImageSource = detailImageSource;
            ToolTip = toolTip;
            SearchedType = searchedType;
            DN = dn;
            UniqueIdentity = uniqueIdentity;
            FavoriteImageSource = favoriteImageSource;
            FavoriteToolTip = favoriteToolTip;
            EmailAddress = emailAddress;
        }


        #region ITeamCommunicator Members

        public string ImageToolTip { get; set; }
        public ImageSource SearchIconImageSource { get; set; }
        public string SearchedName { get; set; }
        public ImageSource StatusImageSource { get; set; }
        public ImageSource MediaImageSource { get; set; }
        public Pointel.Interactions.IPlugins.InteractionType MediaType { get; set; }
        public string Status { get; set; }
        public ImageSource DetailImageSource { get; set; }
        public string ToolTip { get; set; }
        public Datacontext.SelectorFilters SearchedType { get; set; }
        public string DN { get; set; }
        public string UniqueIdentity { get; set; }
        public ImageSource FavoriteImageSource { get; set; }
        public string FavoriteToolTip { get; set; }
        public string EmailAddress { get; set; }
        #endregion
    }
}
