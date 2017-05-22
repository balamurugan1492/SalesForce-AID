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

namespace Pointel.Interactions.Outbound.UserControls
{
    /// <summary>
    /// Interaction logic for UCMyCampaigns.xaml
    /// </summary>
    public partial class UCMyCampaigns : UserControl
    {
        public UCMyCampaigns(bool visibleGetrecord)
        {
            InitializeComponent();
            this.DataContext = OutboundDataContext.GetInstance();
            OutboundDataContext.GetInstance().IsEnableGetRecord = visibleGetrecord;
        }

        private void btnGetReord_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
