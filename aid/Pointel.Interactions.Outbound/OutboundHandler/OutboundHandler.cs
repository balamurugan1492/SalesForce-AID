using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Genesyslab.Platform.Commons.Collections;
using Pointel.Interactions.Outbound.Settings;
using Pointel.Interactions.Outbound.WinForms;
using System.Windows;
using Pointel.Interactions.Outbound.Helpers;
using System.Windows.Media.Imaging;

namespace Pointel.Interactions.Outbound.OutboundHandler
{
    public class OutboundHandler
    {
        #region Declaration
        private CampaignNotifier _campaignNotifier;

        private CampaignNotifier CampaignsNotifier
        {
            get { return _campaignNotifier; }
        }
        private int _notifierStayOpenTime = 1000;
        private List<string> _campaignStatus = new List<string> { "CampaignLoaded", "CampaignUnloaded", "CampaignStarted", "CampaignStopped" };
        #endregion

        public OutboundHandler()
        {
            _campaignNotifier = new CampaignNotifier();
            _campaignNotifier.DataContext = OutboundDataContext.GetInstance();
            _campaignNotifier.StayOpenMilliseconds = _notifierStayOpenTime;
            _campaignNotifier.OpeningMilliseconds = 1000;
            _campaignNotifier.HidingMilliseconds = 500;
        }

        public void NotifyEventIMessage(KeyValueCollection userData)
        {
            if (userData.ContainsKey("GSW_USER_EVENT") && _campaignStatus.Contains(userData["GSW_USER_EVENT"]))
            {
                OutboundDataContext.GetInstance().Date = DateTime.Now.ToString();
                OutboundDataContext.GetInstance().CampaignStatus = userData["GSW_USER_EVENT"].ToString();
                if (userData.ContainsKey("GSW_CAMPAIGN_NAME"))
                    OutboundDataContext.GetInstance().CampaignName = userData["GSW_CAMPAIGN_NAME"].ToString();
                if (userData.ContainsKey("GSW_CAMPAIGN_DESCRIPTION"))
                    OutboundDataContext.GetInstance().Description = userData["GSW_CAMPAIGN_DESCRIPTION"].ToString();
                if (userData.ContainsKey("GSW_CAMPAIGN_MODE"))
                {
                    OutboundDataContext.GetInstance().DeliveryMode = userData["GSW_CAMPAIGN_MODE"].ToString();
                    OutboundDataContext.GetInstance().GRCampaignModeHeight = GridLength.Auto;
                }
                else
                {
                    OutboundDataContext.GetInstance().DeliveryMode = "-";
                    OutboundDataContext.GetInstance().GRCampaignModeHeight = new GridLength(0);
                }
                _campaignNotifier.Show();
                _campaignNotifier.Notify(100, false);
                if (OutboundDataContext.GetInstance().MyCampaigns.Any(p => p.CampaignName == OutboundDataContext.GetInstance().CampaignName))
                {
                    int i = OutboundDataContext.GetInstance().MyCampaigns.IndexOf(OutboundDataContext.GetInstance().MyCampaigns.Where(p => p.CampaignName == OutboundDataContext.GetInstance().CampaignName).FirstOrDefault());
                    if (OutboundDataContext.GetInstance().CampaignStatus.ToLower().Trim() == "campaignstarted")
                    {
                        OutboundDataContext.GetInstance().MyCampaigns.RemoveAt(i);
                        OutboundDataContext.GetInstance().CampaignStatusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.Outbound;component/Images/Start.png", UriKind.Relative));
                        OutboundDataContext.GetInstance().MyCampaigns.Insert(0, (new MyCampaigns(OutboundDataContext.GetInstance().CampaignStatusImageSource, OutboundDataContext.GetInstance().CampaignName, OutboundDataContext.GetInstance().DeliveryMode, OutboundDataContext.GetInstance().Date, "")));
                    }
                    else if (OutboundDataContext.GetInstance().CampaignStatus.ToLower().Trim() == "campaignunloaded")
                    {
                        var getCampaign = OutboundDataContext.GetInstance().MyCampaigns.Where(x => x.CampaignName == OutboundDataContext.GetInstance().CampaignName).ToList();
                        if (getCampaign.Count > 0)
                        {
                            foreach (var data in getCampaign)
                            {
                                OutboundDataContext.GetInstance().MyCampaigns.Remove(data);
                            }
                        }
                    }
                    else
                    {
                        OutboundDataContext.GetInstance().MyCampaigns.RemoveAt(i);
                        OutboundDataContext.GetInstance().CampaignStatusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.Outbound;component/Images/Stop.png", UriKind.Relative));
                        OutboundDataContext.GetInstance().MyCampaigns.Insert(0, (new MyCampaigns(OutboundDataContext.GetInstance().CampaignStatusImageSource, OutboundDataContext.GetInstance().CampaignName, OutboundDataContext.GetInstance().DeliveryMode, OutboundDataContext.GetInstance().Date, "")));
                    }
                }
                else
                {
                    OutboundDataContext.GetInstance().CampaignStatusImageSource = new BitmapImage(new Uri("/Pointel.Interactions.Outbound;component/Images/Stop.png", UriKind.Relative));
                    OutboundDataContext.GetInstance().MyCampaigns.Add(new MyCampaigns(OutboundDataContext.GetInstance().CampaignStatusImageSource, OutboundDataContext.GetInstance().CampaignName, OutboundDataContext.GetInstance().DeliveryMode, OutboundDataContext.GetInstance().Date, ""));
                }
            }
        }
    }
}
