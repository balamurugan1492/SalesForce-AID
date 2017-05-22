/*
* =======================================
* Pointel.Interactions.Contact.Forms
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 08-06-2015
* Author     : Sakthikumar
* Owner      : Pointel Solutions
* =======================================
*/

using System.Windows;
using System.Windows.Input;
using Pointel.Interactions.Contact.Controls;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Pointel.Interactions.Contact.Forms
{
    /// <summary>
    /// Interaction logic for ContactDirectoryWindow.xaml
    /// </summary>
    public partial class ContactDirectoryWindow : Window
    {
        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #region Properties

        public string ContactId
        {
            get;
            private set;
        }

        public string Reason
        {
            get;
            private set;
        }

        public string Description
        {
            get;
            private set;
        }

        #endregion
        
        public ContactDirectoryWindow()
        {
            InitializeComponent();
            this.Owner = Application.Current.Windows.Cast<Window>().Where(x => "SoftphoneBar".Equals(Name)).SingleOrDefault();
                //SoftphoneBar
        }

        /// <summary>
        /// Handles the Loaded event of the ContactDirectoryWindow1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ContactDirectoryWindow1_Loaded(object sender, RoutedEventArgs e)
        {

            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);            
            dockContactDirectoryPanel.Children.Clear();
            if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
            {
                ContactDirectory contactDirectory = new ContactDirectory();
                contactDirectory.ContactSelectedEvent += new ContactDirectoryHandler(contactDirectory_ContactSelectedEvent);
                dockContactDirectoryPanel.Children.Add(contactDirectory);
            }

            this.Topmost = true;
            this.Activate();
            this.Topmost = false;
        }

        void contactDirectory_ContactSelectedEvent(ContactInfoEventArgs e)
        {
            if (e != null)
            {
                this.ContactId = e.ContactId;
                this.Description = e.Description;
                this.Reason = e.Reason;
            }
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the pTitleBar control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void pTitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        /// <summary>
        /// Handles the Activated event of the ContactDirectoryWindow1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ContactDirectoryWindow1_Activated(object sender, EventArgs e)
        {
            //ImageDataContext.GetInstance().EmailMainBorderBrush = (Brush)(new BrushConverter().ConvertFromString("#0070C5"));
        }

        /// <summary>
        /// Handles the Deactivated event of the ContactDirectoryWindow1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void ContactDirectoryWindow1_Deactivated(object sender, EventArgs e)
        {
            //ImageDataContext.GetInstance().EmailMainBorderBrush = (Brush)(new BrushConverter().ConvertFromString("#0070C5"));
        }

        /// <summary>
        /// Handles the Click event of the btnMinimize control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        /// <summary>
        /// Handles the Click event of the btnClose control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();            
        }

        /// <summary>
        /// Handles the KeyDown event of the ContactDirectoryWindow1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void ContactDirectoryWindow1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                this.Close();
            }
        }
    }
}
