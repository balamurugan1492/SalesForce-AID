/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Rajkumar
* Owner      : Pointel Solutions
* =======================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Effects;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace Pointel.Interactions.Email.Forms
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        public string btnleft_Content = string.Empty;
        public string btnright_Content = string.Empty;
        public bool IsUsedForForward = false;
        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();
        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBox"/> class.
        /// </summary>
        /// <param name="Msgbox_typename">The msgbox_typename.</param>
        /// <param name="content">The content.</param>
        /// <param name="btnleft_content">The btnleft_content.</param>
        /// <param name="btnright_content">The btnright_content.</param>
        /// <param name="isUsedForForward">if set to <c>true</c> [is used for forward].</param>
        public MessageBox(string Msgbox_typename, string content, string btnleft_content, string btnright_content, bool isUsedForForward)
        {
            InitializeComponent();
            btnleft_Content = btnleft_content;
            btnright_Content = btnright_content;
            IsUsedForForward = isUsedForForward;           
         
            ForwardError.Visibility = System.Windows.Visibility.Hidden;
            ForwardError.Content = "";
            if (Msgbox_typename == "close")
            {
                this.DialogResult = true;
                this.Close();
            }
            if (IsUsedForForward)
            {
                growFwd.Height = GridLength.Auto;
                btn_left.IsEnabled = false;
            }
            else
                growFwd.Height = new GridLength(0);

            if (IsUsedForForward)
            {
                if (Msgbox_typename.Contains("Cancel"))
                {
                    lblTitle.Content = Msgbox_typename;
                    btn_left.IsEnabled = true;
                }
                else
                {
                    lblTitle.Content = Msgbox_typename;
                }
            }
            else
                lblTitle.Content = Msgbox_typename + " - Agent Interaction Desktop";

            txtblockContent.Text = content;
            if (!string.IsNullOrEmpty(btnleft_content))
                btn_left.Content = btnleft_content;
            else
                btn_left.Visibility = System.Windows.Visibility.Hidden;
            btn_right.Content = btnright_content;
            ShadowEffect.ShadowDepth = 0;
            ShadowEffect.Opacity = 0.5;
            ShadowEffect.Softness = 0.5;
            ShadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
        }

        /// <summary>
        /// Handles the Activated event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Activated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#0070C5")));
            MainBorder.BitmapEffect = ShadowEffect;
        }

        /// <summary>
        /// Handles the Deactivated event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void Window_Deactivated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#0070C5")));
            MainBorder.BitmapEffect = ShadowEffect;
        }

        /// <summary>
        /// Handles the Click event of the btn_right control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btn_right_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(btnleft_Content))
                    this.DialogResult = false;
                else
                    this.DialogResult = true;
                this.Close();
            }
            catch { this.Close(); }
        }

        /// <summary>
        /// Handles the Click event of the btn_left control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void btn_left_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the lblTitle control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void lblTitle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
                //if (this.Left < 0)
                //    this.Left = 0;
                //if (this.Top < 0)
                //    this.Top = 0;
                //if (this.Left > System.Windows.SystemParameters.WorkArea.Right - this.Width)
                //    this.Left = System.Windows.SystemParameters.WorkArea.Right - this.Width;
                //if (this.Top > System.Windows.SystemParameters.WorkArea.Bottom - this.Height)
                //    this.Top = System.Windows.SystemParameters.WorkArea.Bottom - this.Height;
            }
            catch { }
        }
        #region Dispose

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            btnleft_Content = null;
            btnright_Content = null;
            ShadowEffect = null;
            //GC.Collect();
            GC.SuppressFinalize(this);
        }

        #endregion Dispose

        /// <summary>
        /// Handles the KeyDown event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                DialogResult = false;
                this.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
        }
    }
}
