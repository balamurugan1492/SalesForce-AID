
using System.Data;
using System.Windows;
using Pointel.Interaction.Workbin.Controls;

namespace Pointel.Interaction.Workbin.Helpers
{
    public class MediaImageDataTemplateSelector : BaseDataTemplateSelector
    {
        #region Constructors

        /*
		 * The default constructor
		 */
        public MediaImageDataTemplateSelector()
        {
        }
        #endregion

        #region Functions
        public override DataTemplate SelectTemplate(object inItem, DependencyObject inContainer)
        {
            DataRowView row = inItem as DataRowView;

            WorkbinUserControl w = GetWindow1(inContainer);
            if (row != null)
            {
                if (row.DataView.Table.Columns.Contains("MediaType"))
                {

                    string media = string.Empty;
                    if (row["MediaType"] != null)
                        media = row["MediaType"] as string;
                    if (media == "chat")
                    {
                        return (DataTemplate)w.FindResource("MediaTemplateChat");
                    }
                    if (media == "email")
                    {
                        string type = row["InteractionType"] as string;
                        if (type.ToLower().Equals("outbound"))
                            return (DataTemplate)w.FindResource("MediaTemplateEmailOutbound");
                        if (type.ToLower().Equals("inbound"))
                            return (DataTemplate)w.FindResource("MediaTemplateEmailInbound");                        
                    }
                    if (media == "voice")
                    {
                        return (DataTemplate)w.FindResource("MediaTemplateVoice");
                    }
                }
            }
            return (DataTemplate)w.FindResource("MediaTemplateDefault");
        }
        #endregion
    }
}
