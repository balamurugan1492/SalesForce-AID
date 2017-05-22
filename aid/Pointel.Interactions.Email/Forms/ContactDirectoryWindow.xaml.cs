/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Sakthikumar
* Owner      : Pointel Solutions
* =======================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using log4net;
using System.Windows.Controls;
using Pointel.Interactions.IPlugins;
using Pointel.Interactions.Email.DataContext;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Pointel.Interactions.Email.Forms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ContactDirectoryWindow : Window
    {
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
           "AID");
        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);


        public Func<string, string, string, bool> NotifySelectedEmailId
        {
            get;
            set;
        }
        public ContactDirectoryWindow(string toAddress, string ccAddress, string bccAddress)
        {
            ToAddress = toAddress;
            CcAddress = ccAddress;
            BccAddress = bccAddress;
            InitializeComponent();
        }

        public string ToAddress
        {
            get;
            private set;
        }
        public string CcAddress
        {
            get;
            private set;
        }
        public string BccAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// Handles the Loaded event of the ContactDirectoryWindow1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void ContactDirectoryWindow1_Loaded(object sender, RoutedEventArgs e)
        {
            dockContactDirectoryPanel.Children.Clear();
            try
            {
                var hwnd = new WindowInteropHelper(this).Handle;
                SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
                dockContactDirectoryPanel.Children.Clear();
                if (Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections.ContainsKey(Pointel.Interactions.IPlugins.Plugins.Contact))
                {
                    UserControl contactDirectory = ((IContactPlugin)Pointel.Interactions.IPlugins.PluginCollection.GetInstance().PluginCollections[Pointel.Interactions.IPlugins.Plugins.Contact]).GetContactDirectoryUserControl(true, null, NotifySelectedEmailId, ToAddress, CcAddress, BccAddress);
                    dockContactDirectoryPanel.Children.Add(contactDirectory);
                }
            }
            catch (Exception exception)
            {
                logger.Error("btnResponses_Click" + exception.ToString());
            }
            this.Topmost = true;
            this.Topmost = false;
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