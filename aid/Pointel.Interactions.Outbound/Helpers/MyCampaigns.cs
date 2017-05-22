using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pointel.Interactions.Outbound.Helpers
{
    public class MyCampaigns : IMyCampaigns
    {
        public MyCampaigns(System.Windows.Media.ImageSource campaignStatusImageSource, string campaignName, string deliveryMode, string date, string description)
        {
            CampaignStatusImageSource = campaignStatusImageSource;
            CampaignName = campaignName;
            DeliveryMode = deliveryMode;
            Date = date;
            Description = description;
        }

        public System.Windows.Media.ImageSource CampaignStatusImageSource { get; set; }
        public string CampaignName { get; set; }
        public string DeliveryMode { get; set; }
        public string Date { get; set; }
        public string Description { get; set; }
    }
}
