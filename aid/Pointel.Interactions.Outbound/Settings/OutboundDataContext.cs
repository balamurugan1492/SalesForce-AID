using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Collections.ObjectModel;
using Pointel.Interactions.Outbound.Helpers;
using System.Windows;
using System.Windows.Media;

namespace Pointel.Interactions.Outbound.Settings
{
    public class OutboundDataContext : INotifyPropertyChanged
    {
        #region Single instance

        private static OutboundDataContext _instance = null;

        public static OutboundDataContext GetInstance()
        {
            if (_instance == null)
            {
                _instance = new OutboundDataContext();
                return _instance;
            }
            return _instance;
        }

        #endregion Single instance

        #region private members
        private ObservableCollection<IMyCampaigns> _mySkills = new ObservableCollection<IMyCampaigns>();
        private string _campaignStatus = string.Empty;
        private string _campaignName = string.Empty;
        private string _deliveryMode = string.Empty;
        private string _date = string.Empty;
        private GridLength _grCampaignModeHeight = new GridLength();
        private bool _isEnableGetRecord = false;
        private ImageSource _campaignStatusImageSource;
        private string _description = string.Empty; 
        #endregion

        #region public members
        public bool isEnableOutboundAccept = false;
        public bool isEnableOutboundReject = false;
        public event PropertyChangedEventHandler PropertyChanged;
        //public enum CampaignState { CampaignLoaded, CampaignUnLoaded, CampaignStarted, CampaignStopped };
        #endregion

        #region INotifyPropertyChange

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion INotifyPropertyChange

        #region Properties
        public ObservableCollection<IMyCampaigns> MyCampaigns
        {
            get
            {
                return _mySkills;
            }
            set
            {
                if (_mySkills != value)
                {
                    _mySkills = value;
                    RaisePropertyChanged(() => MyCampaigns);
                }
            }
        }

        public string CampaignName
        {
            get
            {
                return _campaignName;
            }
            set
            {
                if (_campaignName != value)
                {
                    _campaignName = value;
                    RaisePropertyChanged(() => CampaignName);
                }
            }
        }

        public string CampaignStatus
        {
            get
            {
                return _campaignStatus;
            }
            set
            {
                if (_campaignStatus != value)
                {
                    _campaignStatus = value;
                    RaisePropertyChanged(() => CampaignStatus);
                }
            }
        }

        public string DeliveryMode
        {
            get
            {
                return _deliveryMode;
            }
            set
            {
                if (_deliveryMode != value)
                {
                    _deliveryMode = value;
                    RaisePropertyChanged(() => DeliveryMode);
                }
            }
        }

        public string Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    RaisePropertyChanged(() => Date);
                }
            }
        }

        public ImageSource CampaignStatusImageSource
        {
            get
            {
                return _campaignStatusImageSource;
            }
            set
            {
                if (_campaignStatusImageSource != value)
                {
                    _campaignStatusImageSource = value;
                    RaisePropertyChanged(() => CampaignStatusImageSource);
                }
            }
        }

        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged(() => Description);
                }
            }
        }

        public GridLength GRCampaignModeHeight
        {
            get
            {
                return _grCampaignModeHeight;
            }
            set
            {
                if (_grCampaignModeHeight != value)
                {
                    _grCampaignModeHeight = value;
                    RaisePropertyChanged(() => GRCampaignModeHeight);
                }
            }
        }

        public bool IsEnableGetRecord
        {
            get
            {
                return _isEnableGetRecord;
            }
            set
            {
                if (_isEnableGetRecord!=value)
                {
                    _isEnableGetRecord = value;
                    RaisePropertyChanged(()=>IsEnableGetRecord);
                }
            }
        }
       
        #endregion
    }
}
