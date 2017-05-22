using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Pointel.HTML.Text.Editor.Settings;
using System.Windows.Input;

namespace Pointel.Windows.Views.Common.Editor.Controls
{
    /// <summary>
    /// Interaction logic for HyperLinkPicker.xaml
    /// </summary>
    public partial class HyperLinkPicker : UserControl
    {
        private EditorDataContext editorDataContext;
        public delegate void PassLinkToRTB(string displayName, Uri uri);
        public event PassLinkToRTB LinkSelected;
        private static readonly Regex UrlRegex = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&amp;(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");
        private static readonly Regex urlRegex = new Regex(@"^(((ht|f)tp(s?))\://)?((([a-zA-Z0-9_\-]{2,}\.)+[a-zA-Z]{2,})|((?:(?:25[0-5]|2[0-4]\d|[01]\d\d|\d?\d)(?(\.?\d)\.)){4}))(:[a-zA-Z0-9]+)?(/[a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~]*)?$");
        public HyperLinkPicker(EditorDataContext _editorDataContext)
        {
            InitializeComponent();
            editorDataContext = _editorDataContext;
            txtURL.Focus();
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Paragraph urlMessageParagraph = new Paragraph();
            try
            {
                Uri tempUri = null;
                Run temprun = null;
                string urlString = string.Empty;
                urlString = txtURL.Text.ToString().Trim();
                if (urlString != string.Empty || urlString != "")
                {
                    if (IsHyperlink(urlString))
                    {
                        tempUri = new Uri(urlString, UriKind.RelativeOrAbsolute);

                        if (!tempUri.IsAbsoluteUri)
                        {
                            tempUri = new Uri(@"http://" + urlString, UriKind.Absolute);
                        }
                        if (tempUri != null && tempUri.ToString() != string.Empty)
                        {
                            if (string.IsNullOrEmpty(txtText.Text))
                                txtText.Text = "Click Here";
                            //temprun = new Run(txtText.Text);
                            //Hyperlink hl = new Hyperlink(temprun);
                            //hl.Foreground = Brushes.Blue;
                            //hl.NavigateUri = new Uri(tempUri.ToString());
                            //urlMessageParagraph.Inlines.Add(hl);
                            LinkSelected.Invoke(txtText.Text, tempUri);
                            editorDataContext.contextMenuUC.StaysOpen = false;
                            editorDataContext.contextMenuUC.IsOpen = false;
                        }
                        else
                        {
                            lblError.Content = "It is not a Valid URL  '" + txtURL.Text.ToString() + "'";
                            txtURL.Focus();
                        }
                    }
                    else
                    {
                        lblError.Content = "It is not a Valid URL  '" + txtURL.Text.ToString() + "'";
                        txtURL.Focus();
                    }
                }
                else
                {
                    lblError.Content = "Enter the URL";
                    txtURL.Focus();
                }
            }
            catch //(Exception generalException)
            {

            }
        }
        public static bool IsHyperlink(string word)
        {
            try
            {
                if (word.IndexOfAny(@":.\/".ToCharArray()) != -1)
                {
                    if (UrlRegex.IsMatch(word))
                    {
                        if (!word.StartsWith("http:"))
                            word = "http://" + word;
                        Uri uri;
                        if (Uri.TryCreate(word, UriKind.Absolute, out uri))
                        {
                            return true;
                        }
                    }
                    else if (urlRegex.IsMatch(word))
                    {
                        if (!word.StartsWith("http:"))
                            word = "http://" + word;
                        Uri uri;
                        if (Uri.TryCreate(word, UriKind.Absolute, out uri))
                        {
                            return true;
                        }
                    }
                }
            }
            catch //(Exception generalException)
            {
                // _logger.Error("Error occurred while IsHyperlink() :" + generalException.ToString());
            }
            return false;
        }
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            editorDataContext.contextMenuUC.StaysOpen = false;
            editorDataContext.contextMenuUC.IsOpen = false;
        }

        /// <summary>
        /// Previews the key up.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.Input.KeyEventArgs"/> instance containing the event data.</param>
        private void PreviewKeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {

            //if (e.Key == Key.Enter)
            //    btnLogin_Click(null, null);
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if (key == Key.Enter)
            {
                btnOK_Click(null, null);
                btnOK.Focus();
                e.Handled = true;
            }

        }

        private void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);
            if (key == Key.Enter)
                btnOK.Focus();
        }

    }
}
