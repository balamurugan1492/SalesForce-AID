using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using Pointel.Interactions.Outbound.Settings;

namespace Pointel.Interactions.Outbound.WinForms
{
    /// <summary>
    /// Interaction logic for Notifier.xaml
    /// </summary>
    public partial class Notifier : TaskbarNotifier.TaskbarNotifier
    {
        #region Field Declarations
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        public DropShadowBitmapEffect ShadowEffect = new DropShadowBitmapEffect();
        #endregion Field Declarations

        #region Constructor
        public Notifier()
        {
            logger.Info("outbound notify constructor");
            InitializeComponent();
            this.DataContext = OutboundDataContext.GetInstance();
            logger.Info("outbound notify constructor -- Initialize Component");
            EventManager.RegisterClassHandler(typeof(UIElement), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
            if (!OutboundDataContext.GetInstance().isEnableOutboundReject && !OutboundDataContext.GetInstance().isEnableOutboundAccept)
            {
                OutboundDataContext.GetInstance().isEnableOutboundAccept = true;
                OutboundDataContext.GetInstance().isEnableOutboundReject = false;
            }
            if (!OutboundDataContext.GetInstance().isEnableOutboundReject && OutboundDataContext.GetInstance().isEnableOutboundAccept)
            {
                btnLeft.Visibility = System.Windows.Visibility.Hidden;
                btnRight.Content = "_Accept";
                btnRight.Style = (Style)FindResource("CallButton");
            }
            if (!OutboundDataContext.GetInstance().isEnableOutboundAccept && OutboundDataContext.GetInstance().isEnableOutboundReject)
            {
                btnLeft.Visibility = System.Windows.Visibility.Hidden;
                btnRight.Content = "_Reject";
                btnRight.Style = (Style)FindResource("RejectButton");
            }
            if (OutboundDataContext.GetInstance().isEnableOutboundReject && OutboundDataContext.GetInstance().isEnableOutboundAccept)
            {
                btnLeft.Visibility = System.Windows.Visibility.Visible;
                btnLeft.Content = "_Accept";
                btnRight.Content = "_Reject";
                btnRight.Style = (Style)FindResource("RejectButton");
            }
        }
        #endregion Constructor

        #region Window Events

        /// <summary>
        /// Handles the Click event of the Right control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Right_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// Handles the Click event of the Left control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Left_Click(object sender, RoutedEventArgs e)
        {
          
        }

        #endregion Window Events

        #region Controls Events
        /// <summary>
        /// Called when [access key pressed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="AccessKeyPressedEventArgs"/> instance containing the event data.</param>
        private static void OnAccessKeyPressed(object sender, AccessKeyPressedEventArgs e)
        {
            if (!e.Handled && e.Scope == null && (e.Target == null))
            {
                //if alt key is not in use handle event to prevent behavior without alt key
                if ((Keyboard.Modifiers & ModifierKeys.Alt) != ModifierKeys.Alt)
                {
                    e.Target = null;
                    e.Handled = true;
                }
            }
        }
        /// <summary>
        /// Handles the Expanded event of the Expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            base.SetInitialLocations(true);
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - this.Width;

            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight - this.Height;
            this.Top = this.Top - 52;
        }

        /// <summary>
        /// Handles the Collapsed event of the Expander control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs"/> instance containing the event data.</param>
        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            base.SetInitialLocations(true);
            this.Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - this.Width;

            this.Top = System.Windows.SystemParameters.FullPrimaryScreenHeight - this.Height;
            this.Top = this.Top + 98;
        }

        #endregion Controls Events

        #region Callable Function
        public void ReloadUI(bool isrejectCallDisplayed)
        {
            if (!isrejectCallDisplayed)
            {
                btnLeft.Visibility = System.Windows.Visibility.Hidden;
                btnRight.Content = "_Accept";
                btnRight.Style = (Style)FindResource("CallButton");
            }
            else
            {
                if (!OutboundDataContext.GetInstance().isEnableOutboundReject && !OutboundDataContext.GetInstance().isEnableOutboundAccept)
                {
                    OutboundDataContext.GetInstance().isEnableOutboundAccept = true;
                    OutboundDataContext.GetInstance().isEnableOutboundReject = false;
                }
                if (!OutboundDataContext.GetInstance().isEnableOutboundReject && OutboundDataContext.GetInstance().isEnableOutboundAccept)
                {
                    btnLeft.Visibility = System.Windows.Visibility.Hidden;
                    btnRight.Content = "_Accept";
                    btnRight.Style = (Style)FindResource("CallButton");
                }
                if (!OutboundDataContext.GetInstance().isEnableOutboundAccept && OutboundDataContext.GetInstance().isEnableOutboundReject)
                {
                    btnLeft.Visibility = System.Windows.Visibility.Hidden;
                    btnRight.Content = "_Reject";
                    btnRight.Style = (Style)FindResource("RejectButton");
                }
                if (OutboundDataContext.GetInstance().isEnableOutboundReject && OutboundDataContext.GetInstance().isEnableOutboundAccept)
                {
                    btnLeft.Visibility = System.Windows.Visibility.Visible;
                    btnLeft.Content = "_Accept";
                    btnRight.Content = "_Reject";
                    btnRight.Style = (Style)FindResource("RejectButton");
                }
                else
                {
                    btnLeft.Visibility = System.Windows.Visibility.Visible;
                    btnLeft.Content = "_Accept";
                    btnRight.Content = "_Reject";
                    btnRight.Style = (Style)FindResource("RejectButton");
                }
            }
        }

        #endregion Callable Function
    }
}
