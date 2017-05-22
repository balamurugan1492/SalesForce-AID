/*
* =====================================
* Pointel.Interactions.TeamCommunicator.WinForms
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 05-Sep-2014
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using Pointel.Interactions.TeamCommunicator.Settings;

namespace Pointel.Interactions.TeamCommunicator.WinForms
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Window
    {
        #region Public Members

        public string btnleft_Content = string.Empty;
        public string btnright_Content = string.Empty;
        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();

        #endregion Public Members

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBox"/> class.
        /// </summary>
        /// <param name="Msgbox_typename">The msgbox_typename.</param>
        /// <param name="content">The content.</param>
        /// <param name="btnleft_content">The btnleft_content.</param>
        /// <param name="btnright_content">The btnright_content.</param>
        /// <param name="isUsedForForward">if set to <c>true</c> [is used for forward].</param>
        public MessageBox(string Msgbox_typename, string content, string btnleft_content, string btnright_content)
        {
            btnleft_Content = btnleft_content;
            btnright_Content = btnright_content;
            InitializeComponent();
            this.DataContext = Datacontext.GetInstance();
            if (Msgbox_typename == "close")
            {
                this.DialogResult = true;
                this.Close();
            }
           

           
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

        #endregion Constructor

        #region Window Events

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
            MainBorder.BorderBrush = Brushes.Black;
            MainBorder.BitmapEffect = null;
        }

        /// <summary>
        /// Handles the MouseLeftButtonDown event of the Label control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="MouseButtonEventArgs"/> instance containing the event data.</param>
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try { this.DragMove(); }
            catch { }
        }

        #region Controls Events


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

        #endregion Controls Events

        #endregion Window Events
    }
}