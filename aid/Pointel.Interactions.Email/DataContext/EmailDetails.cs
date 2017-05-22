/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Moorthy
* Owner      : Pointel Solutions
* =======================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Media;
using System.Collections.ObjectModel;
using Pointel.Interactions.Email.Helper;
using System.Windows;

namespace Pointel.Interactions.Email.DataContext
{
    /// <summary>
    /// Class EmailDetails.
    /// </summary>
    public class EmailDetails : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ImageSource _casedataImageSource;
        private ImageSource _contactImageSource;
        private ImageSource _responseImageSource;
        private ObservableCollection<IContacts> _contacts = new ObservableCollection<IContacts>();
        private ObservableCollection<IContacts> _contactsFilter = new ObservableCollection<IContacts>(); 
        private string _dialedNumbers = string.Empty;
        private double _modifiedTextSize = 0;

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

        private string titleText = string.Empty;
        public string TitleText
        {
            get
            {
                return titleText;
            }
            set
            {
                if (value != null)
                {
                    titleText = value;
                    RaisePropertyChanged(() => TitleText);
                }
            }
        }
        private ObservableCollection<EmailCaseData> emailCaseData = new ObservableCollection<EmailCaseData>();
        public ObservableCollection<EmailCaseData> EmailCaseData
        {
            get
            {
                return emailCaseData;
            }
            set
            {
                if (value != null)
                {
                    emailCaseData = value;
                    RaisePropertyChanged(() => EmailCaseData);
                }

            }
        }

        public ImageSource CasedataImageSource
        {
            get
            {
                return _casedataImageSource;
            }
            set
            {
                if (_casedataImageSource != value)
                {
                    _casedataImageSource = value;
                    RaisePropertyChanged(() => CasedataImageSource);
                }
            }
        }

        public ImageSource ContactImageSource
        {
            get
            {
                return _contactImageSource;
            }
            set
            {
                if (_contactImageSource != value)
                {
                    _contactImageSource = value;
                    RaisePropertyChanged(() => ContactImageSource);
                }
            }
        }

        public ImageSource ResponseImageSource
        {
            get
            {
                return _responseImageSource;
            }
            set
            {
                if (_responseImageSource != value)
                {
                    _responseImageSource = value;
                    RaisePropertyChanged(() => ResponseImageSource);
                }
            }
        }

        public string DialedNumbers
        {
            get
            {
                return _dialedNumbers;
            }
            set
            {
                if (_dialedNumbers != value)
                {
                    _dialedNumbers = value;
                    RaisePropertyChanged(() => DialedNumbers);
                }
            }
        }

        public double ModifiedTextSize
        {
            get
            {
                return _modifiedTextSize;
            }
            set
            {
                if (_modifiedTextSize != value)
                {
                    _modifiedTextSize = value;
                    RaisePropertyChanged(() => ModifiedTextSize);
                }
            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    RaisePropertyChanged(() => UserName);
                }
            }
        }

        private string _placeID = string.Empty;
        public string PlaceID
        {
            get
            {
                return _placeID;
            }
            set
            {
                if (_placeID != value)
                {
                    _placeID = value;
                    RaisePropertyChanged(() => PlaceID);
                }
            }
        }

        public ObservableCollection<IContacts> Contacts
        {
            get
            {
                return _contacts;
            }
            set
            {
                if (_contacts != value)
                {
                    _contacts = value;
                    RaisePropertyChanged(() => Contacts);
                }
            }
        }

        public ObservableCollection<IContacts> ContactsFilter
        {
            get
            {
                return _contactsFilter;
            }
            set
            {
                if (_contactsFilter != value)
                {
                    _contactsFilter = value;
                    RaisePropertyChanged(() => ContactsFilter);
                }
            }
        }

        private ObservableCollection<IRecentInteractions> _recentInteraction = new ObservableCollection<IRecentInteractions>();
        public ObservableCollection<IRecentInteractions> RecentInteraction
        {
            get
            {
                return _recentInteraction;
            }
            set
            {
                if (_recentInteraction != value)
                {
                    _recentInteraction = value;
                    RaisePropertyChanged(() => RecentInteraction);
                }
            }
        }
        private string _inprogressInteractionCount;
        public string InprogressInteractionCount
        {
            get
            {
                return _inprogressInteractionCount;
            }
            set
            {
                if (_inprogressInteractionCount != value)
                {
                    _inprogressInteractionCount = value;
                    RaisePropertyChanged(() => InprogressInteractionCount);
                }
            }
        }

        private string _recentInteractionCount;
        public string RecentInteractionCount
        {
            get
            {
                return _recentInteractionCount;
            }
            set
            {
                if (_recentInteractionCount != value)
                {
                    _recentInteractionCount = value;
                    RaisePropertyChanged(() => RecentInteractionCount);
                }
            }
        }

        private string _recentIXNCount;
        public string RecentIXNCount
        {
            get
            {
                return _recentIXNCount;
            }
            set
            {
                if (_recentIXNCount != value)
                {
                    _recentIXNCount = value;
                    RaisePropertyChanged(() => RecentIXNCount);
                }
            }
        }

        private string _totalInprogessIXNCount;
        public string TotalInprogessIXNCount
        {
            get
            {
                return _totalInprogessIXNCount;
            }
            set
            {
                if (_totalInprogessIXNCount != value)
                {
                    _totalInprogessIXNCount = value;
                    RaisePropertyChanged(() => TotalInprogessIXNCount);
                }
            }
        }

        private string _remainingDetails;
        public string RemainingDetails
        {
            get
            {
                return _remainingDetails;
            }
            set
            {
                if (_remainingDetails != value)
                {
                    _remainingDetails = value;
                    RaisePropertyChanged(() => RemainingDetails);
                }
            }
        }

        private Visibility _inProgressIXNCountVisibility = Visibility.Collapsed;
        private Visibility _recentIXNListVisibility = Visibility.Collapsed;
        public Visibility InProgressIXNCountVisibility
        {
            get { return _inProgressIXNCountVisibility; }
            set
            {
                _inProgressIXNCountVisibility = value;
                RaisePropertyChanged(() => InProgressIXNCountVisibility);
            }
        }
        public Visibility RecentIXNListVisibility
        {
            get { return _recentIXNListVisibility; }
            set
            {
                _recentIXNListVisibility = value;
                RaisePropertyChanged(() => RecentIXNListVisibility);
            }
        }

    }
}
