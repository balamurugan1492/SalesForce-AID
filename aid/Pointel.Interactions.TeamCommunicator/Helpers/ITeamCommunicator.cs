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
    public interface ITeamCommunicator
    {
        string ImageToolTip { get; set; }
        ImageSource SearchIconImageSource { get; set; }
        string SearchedName { get; set; }
        ImageSource StatusImageSource { get; set; }
        Pointel.Interactions.IPlugins.InteractionType MediaType { get; set; }
        ImageSource MediaImageSource { get; set; }
        string Status { get; set; }
        ImageSource DetailImageSource { get; set; }
        string ToolTip { get; set; }
        Datacontext.SelectorFilters SearchedType { get; set; }
        string DN { get; set; }
        string UniqueIdentity { get; set; }
        ImageSource FavoriteImageSource { get; set; }
        string FavoriteToolTip { get; set; }
        string EmailAddress { get; set; }
    }
}
