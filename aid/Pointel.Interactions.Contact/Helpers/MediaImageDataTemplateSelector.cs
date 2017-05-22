namespace Pointel.Interactions.Contact.Helpers
{
    using System.Data;
    using System.Windows;

    using Pointel.Interactions.Contact.Controls;

    public class MediaImageDataTemplateSelector : BaseDataTemplateSelector
    {
        #region Constructors

        /*
         * The default constructor
         */
        public MediaImageDataTemplateSelector()
        {
        }

        #endregion Constructors

        #region Methods

        public override DataTemplate SelectTemplate(object inItem, DependencyObject inContainer)
        {
            DataRowView row = inItem as DataRowView;

            ContactHistory w = GetWindow1(inContainer);
            if (row != null)
            {
                if (row.DataView.Table.Columns.Contains("media"))
                {

                    string media = string.Empty;
                    if (row["media"] != null)
                        media = row["media"] as string;
                    if (media == "chat")
                    {
                        return (DataTemplate)w.FindResource("MediaTemplateChat");
                    }
                    if (media == "email")
                    {
                        string type = row["SubtypeId"] as string;
                        if (string.IsNullOrEmpty(type))
                            return (DataTemplate)w.FindResource("MediaTemplateEmailOutbound");
                        if (type.ToLower().Contains("inbound"))
                            return (DataTemplate)w.FindResource("MediaTemplateEmailInbound");
                        else
                            return (DataTemplate)w.FindResource("MediaTemplateEmailOutbound");
                    }
                    if (media == "voice")
                    {
                        return (DataTemplate)w.FindResource("MediaTemplateVoice");
                    }
                }
            }
            return (DataTemplate)w.FindResource("MediaTemplateDefault");
        }

        #endregion Methods
    }
}