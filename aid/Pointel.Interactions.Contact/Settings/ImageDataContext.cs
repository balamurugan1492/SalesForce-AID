using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;
using Pointel.Interactions.Contact.Helpers;

namespace Pointel.Interaction.Contact.Settings
{
    public class ImageDataContext : INotifyPropertyChanged
    {
        #region Single instance
        private static ImageDataContext _instance = null;
        public static ImageDataContext GetInstance()
        {

            if (_instance == null)
            {
                _instance = new ImageDataContext();
                return _instance;
            }
            return _instance;
        }
        #endregion
        public event PropertyChangedEventHandler PropertyChanged;
        //Normal Mail
        private ImageSource _newEmailIconImageSource;   
        private ImageSource _markDoneIconImageSource;
        private ImageSource _consultIconImageSource;
        private ImageSource _resetMailIconImagSourceInfo;
        private ImageSource _saveMailIconImagSourceInfo;
        private ImageSource _collapsetMailIconImagSourceInfo;


        //Compose Mail 
      

        public string ImagepathContact = string.Empty;
        public Hashtable HshApplicationLevel = new Hashtable(StringComparer.InvariantCultureIgnoreCase);
        public static Hashtable hshLoadGroupContact = new Hashtable();
        public string transactionName = string.Empty;

    
        //Image Icon 16*16 size  declared here 

      

        private string __expandMailMsgWorkbin = string.Empty;
        private string _newMailMsg = string.Empty;
        private string _deleteMailMsg = string.Empty;
        private string _replyMailMsg = string.Empty;
        private string _replyMailAllMsg = string.Empty;
        private string _saveMailMsg = string.Empty;
        private string _transferMailMsg = string.Empty;
        private string _printMailMsg = string.Empty;
        private string _markDoneMailMsg = string.Empty;
        private string _consultMailMsg = string.Empty;
        private string _saveMailinfo = string.Empty;
        private string _resetMailinfo = string.Empty;

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
        /// Raises the property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
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
        public ImageSource NewEmailIconImageFolder
        {
            get
            {
                return _newEmailIconImageSource;
            }
            set
            {
                if (_newEmailIconImageSource != value)
                {
                    _newEmailIconImageSource = value;
                    RaisePropertyChanged(() => NewEmailIconImageFolder);
                }
            }
        }

       
    
       

      
       
      
        public ImageSource EmailIconImageSourceUP
        {

            get
            {
                return _markDoneIconImageSource;
            }
            set
            {
                if (_markDoneIconImageSource != value)
                {
                    _markDoneIconImageSource = value;
                    RaisePropertyChanged(() => EmailIconImageSourceUP);
                }
            }

        }
        public ImageSource EmailIconImageSourceDown
        {

            get
            {
                return _consultIconImageSource;
            }
            set
            {
                if (_consultIconImageSource != value)
                {
                    _consultIconImageSource = value;
                    RaisePropertyChanged(() => EmailIconImageSourceDown);
                }
            }

        }
        public ImageSource EmailIconImageSourceWhiteText
        {

            get
            {
                return _saveMailIconImagSourceInfo;
            }
            set
            {
                if (_saveMailIconImagSourceInfo != value)
                {
                    _saveMailIconImagSourceInfo = value;
                    RaisePropertyChanged(() => EmailIconImageSourceWhiteText);
                }
            }

        }
        public ImageSource EMailIconImageSourceExpand
        {

            get
            {
                return _resetMailIconImagSourceInfo;
            }
            set
            {
                if (_resetMailIconImagSourceInfo != value)
                {
                    _resetMailIconImagSourceInfo = value;
                    RaisePropertyChanged(() => EMailIconImageSourceExpand);
                }
            }

        }
        public ImageSource EMailIconImageSourceCollapse
        {

            get
            {
                return _collapsetMailIconImagSourceInfo;
            }
            set
            {
                if (_collapsetMailIconImagSourceInfo != value)
                {
                    _collapsetMailIconImagSourceInfo = value;
                    RaisePropertyChanged(() => EMailIconImageSourceCollapse);
                }
            }

        }
        public string NewEmailMsg
        {
            get
            {
                return _newMailMsg;
            }
            set
            {
                if (_newMailMsg != value)
                {
                    _newMailMsg = value;
                    RaisePropertyChanged(() => NewEmailMsg);
                }
            }
        }

        public string DeleteMailMsg
        {
            get
            {
                return _deleteMailMsg;
            }
            set
            {
                if (_deleteMailMsg != value)
                {
                    _deleteMailMsg = value;
                    RaisePropertyChanged(() => DeleteMailMsg);
                }
            }
        }
       
       
       
     
      
        public string MarkDoneMailMsg
        {
            get
            {
                return _markDoneMailMsg;
            }
            set
            {
                if (_markDoneMailMsg != value)
                {
                    _markDoneMailMsg = value;
                    RaisePropertyChanged(() => MarkDoneMailMsg);
                }
            }
        }
        public string ConsultMailMsg
        {
            get
            {
                return _consultMailMsg;
            }
            set
            {
                if (_consultMailMsg != value)
                {
                    _consultMailMsg = value;
                    RaisePropertyChanged(() => ConsultMailMsg);
                }
            }
        }
        public string SaveMailInfo
        {
            get
            {
                return _saveMailinfo;
            }
            set
            {
                if (_saveMailinfo != value)
                {
                    _saveMailinfo = value;
                    RaisePropertyChanged(() => SaveMailInfo);
                }
            }
        }
        public string ResetMailInfo
        {
            get
            {
                return _resetMailinfo;
            }
            set
            {
                if (_resetMailinfo != value)
                {
                    _resetMailinfo = value;
                    RaisePropertyChanged(() => ResetMailInfo);
                }
            }
        }
        private Brush _emailMainBorderBrush;
        public Brush EmailMainBorderBrush
        {
            get
            {
                return _emailMainBorderBrush;
            }
            set
            {
                if (_emailMainBorderBrush != value)
                {
                    _emailMainBorderBrush = value;
                    RaisePropertyChanged(() => EmailMainBorderBrush);
                }
            }
        }

        private Thickness _emailMainBorderThickness;
        public Thickness EmailMainBorderThickness
        {
            get
            {
                return _emailMainBorderThickness;
            }
            set
            {
                if (_emailMainBorderThickness != value)
                {
                    _emailMainBorderThickness = value;
                    RaisePropertyChanged(() => EmailMainBorderThickness);
                }
            }
        }


      
     

        //Compose Mail 

        private string _dialedNumbers = string.Empty;
        private double _modifiedTextSize = 0;
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
        private Thickness _bottomWinBorderThickness;
        public Thickness BottomWindowThickness
        {
            get
            {
                return _bottomWinBorderThickness;
            }
            set
            {
                if (_bottomWinBorderThickness != value)
                {
                    _bottomWinBorderThickness = value;
                    RaisePropertyChanged(() => BottomWindowThickness);
                }
            }
        }
        //Outbound Image







    
        
       
        public ImageSource SavemailIconImageSourceInfo
        {

            get
            {
                return _saveMailIconImagSourceInfo;
            }
            set
            {
                if (_saveMailIconImagSourceInfo != value)
                {
                    _saveMailIconImagSourceInfo = value;
                    RaisePropertyChanged(() => SavemailIconImageSourceInfo);
                }
            }

        }
        public ImageSource ResetMailIconImageSourceInfo
        {

            get
            {
                return _resetMailIconImagSourceInfo;
            }
            set
            {
                if (_resetMailIconImagSourceInfo != value)
                {
                    _resetMailIconImagSourceInfo = value;
                    RaisePropertyChanged(() => ResetMailIconImageSourceInfo);
                }
            }

        }
       


   
   

       

        public ImageSource ConsultEmailIconImageSource
        {

            get
            {
                return _consultIconImageSource;
            }
            set
            {
                if (_consultIconImageSource != value)
                {
                    _consultIconImageSource = value;
                    RaisePropertyChanged(() => ConsultEmailIconImageSource);
                }
            }

        }
        private ObservableCollection<IContacts> _contacts = new ObservableCollection<IContacts>();

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
        private ObservableCollection<IContacts> _contactsFilter = new ObservableCollection<IContacts>();

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
        private string _diallingNumber = string.Empty;
        public string DiallingNumber
        {
            get
            {
                return _diallingNumber;
            }
            set
            {
                if (_diallingNumber != value)
                {
                    _diallingNumber = value;
                    RaisePropertyChanged(() => DiallingNumber);
                }
            }
        }
        private Dictionary<string, string> _annexContacts = new Dictionary<string, string>();
        public Dictionary<string, string> AnnexContacts
        {
            get
            {
                return _annexContacts;
            }
            set
            {
                if (_annexContacts != value)
                {
                    _annexContacts = value;
                    RaisePropertyChanged(() => AnnexContacts);
                }
            }
        }

    }


}

    


