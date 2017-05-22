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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Pointel.Interactions.Outbound.Settings;

namespace Pointel.Interactions.Outbound.WinForms
{
    /// <summary>
    /// Interaction logic for CampaignNotifier.xaml
    /// </summary>
    public partial class CampaignNotifier : TaskbarNotifier.TaskbarNotifier
    {
        #region Declaration
        public OutboundWindow Mainwindow;
        #endregion

        public CampaignNotifier()
        {
            InitializeComponent();
            this.DataContext = OutboundDataContext.GetInstance();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            base.DisplayState = Pointel.TaskbarNotifier.TaskbarNotifier.DisplayStates.Hiding;
        }

        private void TaskbarNotifier_Loaded(object sender, RoutedEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(UIElement), AccessKeyManager.AccessKeyPressedEvent, new AccessKeyPressedEventHandler(OnAccessKeyPressed));
        }
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
        
    }
}
