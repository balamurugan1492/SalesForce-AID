using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace Pointel.Interactions.Chat.CustomControls
{

    public class BindableRichTextBox : RichTextBox
    {    

        #region Document Bind
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Documents", typeof(FlowDocument), typeof(BindableRichTextBox), new PropertyMetadata(null, new PropertyChangedCallback(OnDocumentChanged)));
       
        // Methods
        private static void OnDocumentChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var richTextBox = obj as RichTextBox;
            if (args.NewValue != null)
            {
                try
                {
                    var doc = (FlowDocument)args.NewValue;

                    // Set the document
                    richTextBox.Document = doc;

                }
                catch (Exception ex)
                {
                    string error = ex.ToString();
                }
            }
        }

        // Properties
        /// <summary>
        /// Gets or sets the documents.
        /// </summary>
        /// <value>
        /// The documents.
        /// </value>
        [Description("Get's or set's the Content of the Pointel.Interactions.Chat.CustomControls.BindableRichTextBox"), Category("Common Properties")]
        public FlowDocument Documents
        {
            get
            {
                return (FlowDocument)this.GetValue(DocumentProperty);
            }
            set
            {
                this.SetValue(DocumentProperty, value);
            }
        }
        public static string GetDocumentXaml(DependencyObject obj)
        {
            return (string)obj.GetValue(DocumentProperty);
        }

        #endregion Document Bind
    }
}
