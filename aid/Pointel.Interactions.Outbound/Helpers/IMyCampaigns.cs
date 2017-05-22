using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace Pointel.Interactions.Outbound.Helpers
{
    public interface IMyCampaigns
    {

        ImageSource CampaignStatusImageSource { get; set; }

        string CampaignName { get; set; }

        string DeliveryMode { get; set; }

        string Date { get; set; }

        string Description { get; set; }
         
    }
}
