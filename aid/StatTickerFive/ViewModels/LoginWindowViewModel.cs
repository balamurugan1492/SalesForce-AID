using System;
using System.Configuration;
using System.Collections.Generic;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Input;
using System.Windows;
using System.Security.Principal;
using Pointel.Statistics.Core.Utility;
using StatTickerFive.Views;
using StatTickerFive.Helpers;
using Pointel.Logger;
using Pointel.Statistics.Core;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.IO;
using System.Security.AccessControl;
using System.Xml;
using System.Windows.Media.Imaging;

namespace StatTickerFive.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class LoginWindowViewModel : NotificationObject
    {
        #region Public Declarations

        private static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        StatisticsSupport objStatSupport = new StatisticsSupport();
        OutputValues Output = new OutputValues();
        XMLStorage objXmlReader = new XMLStorage();
        StatisticsBase stat = new StatisticsBase();


        public bool IsAuthenticated;
        public bool IsAdmin;
        public bool isExpanded;
        public bool IsPassword;
        public bool IsHost;
        public bool IsPort;
        public bool IsApplicationName;
        public string CMEError = string.Empty;
        public string DBError = string.Empty;
        public Dictionary<string, bool> DictFolderPermissions = new Dictionary<string, bool>();

        //SystemMenuHooking
        private const int MfDisabled = 0x0002;
        private const int MfByposition = 0x0400;

        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        private static extern IntPtr GetSystemMenu(IntPtr hwnd, int revert);

        [DllImport("user32.dll", EntryPoint = "GetMenuItemCount")]
        private static extern int GetMenuItemCount(IntPtr hmenu);

        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        private static extern int RemoveMenu(IntPtr hmenu, int npos, int wflags);


        #endregion

        #region Properties

        private string _userName;
        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>The name of the user.</value>
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (value != null)
                {
                    _userName = value;
                    RaisePropertyChanged(() => UserName);
                }
            }
        }

        private string _password;
        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        /// <value>The password.</value>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (value != null)
                {
                    _password = value;
                    RaisePropertyChanged(() => Password);
                }
            }
        }

        private string _applicationName;
        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        /// <value>The name of the application.</value>
        public string ApplicationName
        {
            get
            {
                return _applicationName;
            }
            set
            {
                if (value != null)
                {
                    _applicationName = value;
                    RaisePropertyChanged(() => ApplicationName);
                }
            }
        }

        private string _host;
        /// <summary>
        /// Gets or sets the host.
        /// </summary>
        /// <value>The host.</value>
        public string Host
        {
            get
            {
                return _host;
            }
            set
            {
                if (value != null)
                {
                    _host = value;
                    RaisePropertyChanged(() => Host);
                }
            }
        }

        private string _port;
        /// <summary>
        /// Gets or sets the port.
        /// </summary>
        /// <value>The port.</value>
        public string Port
        {
            get
            {
                return _port;
            }
            set
            {
                if (value != null)
                {
                    _port = value;
                    RaisePropertyChanged(() => Port);
                }
            }
        }

        private string _place;
        /// <summary>
        /// Gets or sets the place.
        /// </summary>
        /// <value>The place.</value>
        public string Place
        {
            get { return _place; }
            set
            {
                if (value != null)
                {
                    _place = value;
                    RaisePropertyChanged(() => Place);
                }
            }
        }

        private int _rowHeight;
        /// <summary>
        /// Gets or sets the height of the row.
        /// </summary>
        /// <value>The height of the row.</value>
        public int RowHeight
        {
            get
            {
                return _rowHeight;
            }
            set
            {
                _rowHeight = value;
                RaisePropertyChanged(() => RowHeight);
            }
        }

        private int _passwordHeight;
        /// <summary>
        /// Gets or sets the height of the password.
        /// </summary>
        /// <value>The height of the password.</value>
        public int PasswordHeight
        {
            get
            {
                return _passwordHeight;
            }
            set
            {
                _passwordHeight = value;
                RaisePropertyChanged(() => PasswordHeight);
            }
        }

        private int _appTypeHeight;
        /// <summary>
        /// Gets or sets the height of the password.
        /// </summary>
        /// <value>The height of the password.</value>
        public int AppTypeHeight
        {
            get
            {
                return _appTypeHeight;
            }
            set
            {
                _appTypeHeight = value;
                RaisePropertyChanged(() => AppTypeHeight);
            }
        }


        private GridLength _errorRowHeight;
        /// <summary>
        /// Gets or sets the height of the error row.
        /// </summary>
        /// <value>The height of the error row.</value>
        public GridLength ErrorRowHeight
        {
            get
            {
                return _errorRowHeight;
            }
            set
            {
                _errorRowHeight = value;
                RaisePropertyChanged(() => ErrorRowHeight);
            }
        }

        private string _errorMessage;
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        /// <value>The error message.</value>
        public string ErrorMessage
        {
            get
            {
                return _errorMessage;
            }
            set
            {
                if (value != null)
                {
                    _errorMessage = value;
                    RaisePropertyChanged(() => ErrorMessage);
                }
            }
        }

        private DropShadowBitmapEffect _shadowEffect;
        /// <summary>
        /// Gets or sets the shadow effect.
        /// </summary>
        /// <value>The shadow effect.</value>
        public DropShadowBitmapEffect ShadowEffect
        {
            get
            {
                return _shadowEffect;
            }
            set
            {
                if (value != null)
                {
                    _shadowEffect = value;
                    RaisePropertyChanged(() => ShadowEffect);
                }
            }
        }

        private SolidColorBrush _borderBrush;
        /// <summary>
        /// Gets or sets the border brush.
        /// </summary>
        /// <value>The border brush.</value>
        public SolidColorBrush BorderBrush
        {
            get
            {
                return _borderBrush;
            }
            set
            {
                if (value != null)
                {
                    _borderBrush = value;
                    RaisePropertyChanged(() => BorderBrush);
                }
            }
        }

        private bool _isTopMost;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is top most.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is top most; otherwise, <c>false</c>.
        /// </value>
        public bool IsTopMost
        {
            get
            {
                return _isTopMost;
            }
            set
            {
                _isTopMost = value;
                RaisePropertyChanged(() => IsTopMost);
            }
        }



        private bool _isPlaceEnabled;
        public bool isPlaceEnabled
        {
            get
            {
                return _isPlaceEnabled;
            }
            set
            {
                _isPlaceEnabled = value;
                RaisePropertyChanged(() => isPlaceEnabled);
            }
        }

        private bool _isHostEnabled;
        public bool IsHostEnabled
        {
            get { return _isHostEnabled; }
            set
            {
                _isHostEnabled = value;
                RaisePropertyChanged(() => IsHostEnabled);
            }
        }

        private bool _isPortEnabled;
        public bool IsPortEnabled
        {
            get { return _isPortEnabled; }
            set
            {
                _isPortEnabled = value;
                RaisePropertyChanged(() => IsPortEnabled);
            }
        }

        private bool _isAppNameEnabled;
        public bool IsAppNameEnabled
        {
            get { return _isAppNameEnabled; }
            set
            {
                _isAppNameEnabled = value;
                RaisePropertyChanged(() => IsAppNameEnabled);
            }
        }
        private int _selectedType;
        public int SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                _selectedType = value;
                if (SelectedType < 0)
                    SelectedType = 0;
                RaisePropertyChanged(() => SelectedType);
            }
        }

        private string _expanderHeader;
        public string ExpanderHeader
        {
            get
            {
                return _expanderHeader;
            }
            set
            {
                if (value != null)
                {
                    _expanderHeader = value;
                    RaisePropertyChanged(() => ExpanderHeader);
                }
            }
        }


        private int _UnameTabIndex;
        public int UnameTabIndex
        {
            get
            {
                return _UnameTabIndex;
            }
            set
            {
                _UnameTabIndex = value;
                RaisePropertyChanged(() => UnameTabIndex);
            }
        }

        private int _PwdTabIndex;
        public int PwdTabIndex
        {
            get
            {
                return _PwdTabIndex;
            }
            set
            {
                _PwdTabIndex = value;
                RaisePropertyChanged(() => PwdTabIndex);
            }
        }

        private int _ApplicationTabIndex;
        public int ApplicationTabIndex
        {
            get
            {
                return _ApplicationTabIndex;
            }
            set
            {
                _ApplicationTabIndex = value;
                RaisePropertyChanged(() => ApplicationTabIndex);
            }
        }

        private int _HostTabIndex;
        public int HostTabIndex
        {
            get
            {
                return _HostTabIndex;
            }
            set
            {
                _HostTabIndex = value;
                RaisePropertyChanged(() => HostTabIndex);
            }
        }

        private int _PortTabIndex;
        public int PortTabIndex
        {
            get
            {
                return _PortTabIndex;
            }
            set
            {
                _PortTabIndex = value;
                RaisePropertyChanged(() => PortTabIndex);
            }
        }

        private int _PlaceTabIndex;
        public int PlaceTabIndex
        {
            get
            {
                return _PlaceTabIndex;
            }
            set
            {
                _PlaceTabIndex = value;
                RaisePropertyChanged(() => PlaceTabIndex);
            }
        }

        private int _ExpanderTabIndex;
        public int ExpanderTabIndex
        {
            get
            {
                return _ExpanderTabIndex;
            }
            set
            {
                _ExpanderTabIndex = value;
                RaisePropertyChanged(() => ExpanderTabIndex);
            }
        }

        private int _LoginTabIndex;
        public int LoginTabIndex
        {
            get
            {
                return _LoginTabIndex;
            }
            set
            {
                _LoginTabIndex = value;
                RaisePropertyChanged(() => LoginTabIndex);
            }
        }

        private int _CancelTabIndex;
        public int CancelTabIndex
        {
            get
            {
                return _CancelTabIndex;
            }
            set
            {
                _CancelTabIndex = value;
                RaisePropertyChanged(() => CancelTabIndex);
            }
        }

        private SolidColorBrush _titleBackground;
        public SolidColorBrush TitleBackground
        {
            get
            {
                return _titleBackground;
            }
            set
            {
                if (value != null)
                {
                    _titleBackground = value;
                    RaisePropertyChanged(() => TitleBackground);
                }
            }
        }

        private SolidColorBrush _titleForeground;
        public SolidColorBrush TitleForeground
        {
            get
            {
                return _titleForeground;
            }
            set
            {
                if (value != null)
                {
                    _titleForeground = value;
                    RaisePropertyChanged(() => TitleForeground);
                }
            }
        }

        private SolidColorBrush _backgroundColor;
        public SolidColorBrush BackgroundColor
        {
            get
            {
                return _backgroundColor;
            }
            set
            {
                if (value != null)
                {
                    _backgroundColor = value;
                    RaisePropertyChanged(() => BackgroundColor);
                }
            }
        }

        private ImageSource _iconSource;
        public ImageSource IconSource
        {
            get
            {
                return _iconSource;
            }
            set
            {
                _iconSource = value;
                RaisePropertyChanged(() => IconSource);
            }
        }

        #endregion

        #region Commands

        public ICommand LoginClick { get { return new DelegateCommand(Login); } }
        public ICommand CancelClick { get { return new DelegateCommand(Cancel); } }
        public ICommand DragCmd { get { return new DelegateCommand(DragMove); } }
        public ICommand ActivatedCommand { get { return new DelegateCommand(WinActivated); } }
        public ICommand DeactivateCommand { get { return new DelegateCommand(WinDeActivated); } }
        public ICommand ExpanderExpanded { get { return new DelegateCommand(ExpExpanded); } }
        public ICommand ExpanderCollapsed { get { return new DelegateCommand(ExpCollapsed); } }
        public ICommand TextChanged { get { return new DelegateCommand(TxtChanged); } }
        public ICommand WinClosing { get { return new DelegateCommand(WinClose); } }
        public ICommand AppTypeChanged { get { return new DelegateCommand(ApplicationType); } }

        #endregion

        #region Methods


        /// <summary>
        /// Wins the close.
        /// </summary>
        private void WinClose()
        {
            Application.Current.Shutdown();
            //Environment.Exit(0);
        }

        /// <summary>
        /// TXTs the changed.
        /// </summary>
        private void TxtChanged()
        {
            if (UserName != null && Password != null)
            {
                ErrorRowHeight = new GridLength(0);
            }
        }

        /// <summary>
        /// Applications the type.
        /// </summary>
        private void ApplicationType()
        {
            try
            {
                switch (SelectedType)
                {
                    case 0:
                        Settings.GetInstance().ApplicationType = StatisticsEnum.StatSource.StatServer.ToString();
                        //Place = Settings.GetInstance().Place;
                        isPlaceEnabled = true;
                        if (Settings.GetInstance().IsHostEnabled)
                            IsHostEnabled = true;
                        if (Settings.GetInstance().IsAppNameEnabled)
                            IsAppNameEnabled = true;
                        if (Settings.GetInstance().IsPortEnabled)
                            IsPortEnabled = true;
                        break;
                    case 1:
                        Settings.GetInstance().ApplicationType = StatisticsEnum.StatSource.DB.ToString();
                        isPlaceEnabled = false;
                        IsHostEnabled = false;
                        IsAppNameEnabled = false;
                        IsPortEnabled = false;
                        break;
                    case 2:

                        Settings.GetInstance().ApplicationType = StatisticsEnum.StatSource.All.ToString();
                        Place = Settings.GetInstance().Place == string.Empty ? Place : Settings.GetInstance().Place;
                        isPlaceEnabled = true;
                        if (Settings.GetInstance().IsHostEnabled)
                            IsHostEnabled = true;
                        if (Settings.GetInstance().IsAppNameEnabled)
                            IsAppNameEnabled = true;
                        if (Settings.GetInstance().IsPortEnabled)
                            IsPortEnabled = true;
                        #region Web service

                        //Settings.GetInstance().ApplicationType = StatisticsEnum.StatSource.WS.ToString();
                        //isPlaceEnabled = false;
                        //Place = "";
                        //IsHostEnabled = false;
                        //IsAppNameEnabled = false;
                        //IsPortEnabled = false;

                        #endregion
                        break;
                    case 3:
                        Settings.GetInstance().ApplicationType = StatisticsEnum.StatSource.All.ToString();
                        Place = Settings.GetInstance().Place == string.Empty ? Place : Settings.GetInstance().Place;
                        isPlaceEnabled = true;
                        if (Settings.GetInstance().IsHostEnabled)
                            IsHostEnabled = true;
                        if (Settings.GetInstance().IsAppNameEnabled)
                            IsAppNameEnabled = true;
                        if (Settings.GetInstance().IsPortEnabled)
                            IsPortEnabled = true;
                        break;
                    default:
                        break;
                }

                stat.ClearTaggedStats();
                StatisticsBase.GetInstance().IsMandatoryFieldsMissing = false;
                StatisticsBase.GetInstance().ListMissedValues.Clear();

                ErrorMessage = string.Empty;
                ErrorRowHeight = new GridLength(0);

            }
            catch (Exception GeneralException)
            {
                logger.Error("LoginWindowModel : ApplicationType : " + GeneralException.Message);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoginWindowViewModel" /> class.
        /// </summary>
        public LoginWindowViewModel()
        {
            try
            {
                logger.Debug("LoginWindowViewModel : Constructor : Method Entry");
                IsTopMost = false;
                ErrorRowHeight = new GridLength(0);
                ExpCollapsed();

                isPlaceEnabled = true;

                foreach (var placekey in ConfigurationManager.AppSettings)
                {
                    logger.Warn("LoginWindowViewModel : Read AppSettings ");
                    if (placekey.ToString() == "Host")
                        Host = ConfigurationManager.AppSettings.Get("Host");
                    else if (placekey.ToString() == "Port")
                        Port = ConfigurationManager.AppSettings.Get("Port");
                    else if (placekey.ToString() == "ApplicationName")
                        ApplicationName = ConfigurationManager.AppSettings.Get("ApplicationName");
                    else if (placekey.ToString() == "Place")
                        Place = ConfigurationManager.AppSettings.Get("Place");
                }

                IsPassword = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("EnablePassword"));
                PasswordHeight = IsPassword ? 28 : 0;

                IsHost = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("EnableHost"));
                IsHostEnabled = IsHost;
                IsPort = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("EnablePort"));
                IsPortEnabled = IsPort;
                IsApplicationName = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("EnableApplicationName"));
                IsAppNameEnabled = IsApplicationName;

                Host = Settings.GetInstance().Host;
                Port = Settings.GetInstance().Port;
                ApplicationName = Settings.GetInstance().ApplicationName;
                Place = Settings.GetInstance().Place;

                IsHostEnabled = Settings.GetInstance().IsHostEnabled;
                IsPortEnabled = Settings.GetInstance().IsPortEnabled;
                IsAppNameEnabled = Settings.GetInstance().IsAppNameEnabled;

                PasswordHeight = Settings.GetInstance().IsPasswordEnabled ? 28 : 0;
                AppTypeHeight = Settings.GetInstance().IsAppTypeEnabled && isExpanded ? 28 : 0;

                Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> dictTheme = new Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush>();
                dictTheme = objStatSupport.ThemeSelector(Settings.GetInstance().Theme);

                TitleBackground = dictTheme[StatisticsEnum.ThemeColors.TitleBackground];
                BackgroundColor = dictTheme[StatisticsEnum.ThemeColors.BackgroundColor];
                TitleForeground = dictTheme[StatisticsEnum.ThemeColors.TitleForeground];
                BorderBrush = dictTheme[StatisticsEnum.ThemeColors.BorderBrush];
                Settings.Instance.Backcolor = dictTheme[StatisticsEnum.ThemeColors.BackgroundColor].ToString();
                Settings.Instance.Forecolor = dictTheme[StatisticsEnum.ThemeColors.TitleBackground].ToString();
                Settings.Instance.TitleBackcolor = dictTheme[StatisticsEnum.ThemeColors.BackgroundColor].ToString();
                ShadowEffect = new DropShadowBitmapEffect();
                ShadowEffect.Color = (Color)BorderBrush.Color;

                Settings.Instance.MouseOver = dictTheme[StatisticsEnum.ThemeColors.MouseOver].ToString();
                Settings.Instance.MousePressed = dictTheme[StatisticsEnum.ThemeColors.MousePressed].ToString();

                if ((string.Compare(Settings.GetInstance().Theme, "outlook8", true) == 0) || (string.Compare(Settings.GetInstance().Theme, "Grey", true) == 0))
                {
                    IconSource = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/StatTickerFive-32x32-01.png"));
                }
                else if (string.Compare(Settings.GetInstance().Theme, "yahoo", true) == 0)
                {
                    IconSource = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/StatTickerFive-32x32-01_Yahoo.png"));
                }
                else if (string.Compare(Settings.GetInstance().Theme, "BB_Theme1", true) == 0)
                {
                    IconSource = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/StatTickerFive-32x32-01_BB.png"));
                }

                WinActivated();


                if (Settings.GetInstance().ApplicationType == null || Settings.GetInstance().ApplicationType == "" || Settings.GetInstance().ApplicationType == string.Empty)
                {
                    Settings.GetInstance().ApplicationType = StatisticsEnum.StatSource.StatServer.ToString();
                }

                if (Settings.GetInstance().ApplicationType != StatisticsEnum.StatSource.StatServer.ToString() && Settings.GetInstance().ApplicationType != StatisticsEnum.StatSource.All.ToString())
                {
                    isPlaceEnabled = false;
                    IsHostEnabled = false;
                    IsAppNameEnabled = false;
                    IsPortEnabled = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error("LoginWindowModel : Constructor : " + ex.Message);
            }
            finally
            {
                logger.Debug("LoginWindowViewModel : Constructor : Method Exit");
                GC.Collect();
            }
        }



        /// <summary>
        /// Systems the menu hook.
        /// </summary>
        /// <param name="thiswindow">The thiswindow.</param>
        public void SystemMenuHook(Window thiswindow)
        {

            #region SystemMenuDisable
            try
            {
                WindowInteropHelper helper = new WindowInteropHelper(thiswindow);
                IntPtr windowHandle = helper.Handle; //Get the handle of this window
                IntPtr hmenu = GetSystemMenu(windowHandle, 0);
                int cnt = GetMenuItemCount(hmenu);

                //remove the button and remove the extra menu items
                // Menu items  
                RemoveMenu(hmenu, cnt - 1, MfDisabled | MfByposition); // Close 
                RemoveMenu(hmenu, cnt - 2, MfDisabled | MfByposition); // Separator
                RemoveMenu(hmenu, cnt - 3, MfDisabled | MfByposition); // Maximize
                RemoveMenu(hmenu, cnt - 4, MfDisabled | MfByposition); // Minimize
                RemoveMenu(hmenu, cnt - 5, MfDisabled | MfByposition); // Size
                RemoveMenu(hmenu, cnt - 6, MfDisabled | MfByposition); // Move
                RemoveMenu(hmenu, cnt - 7, MfDisabled | MfByposition); // Restore                
            }
            catch (Exception ex)
            {
                logger.Error("LoginWindowModel : SystemMenuHook : " + ex.Message);
            }
            finally
            {
                GC.Collect();
            }

            #endregion
        }


        /// <summary>
        /// Exp_s the expanded.
        /// </summary>
        private void ExpExpanded()
        {
            RowHeight = 28;
            UnameTabIndex = 0;
            PwdTabIndex = 1;
            PlaceTabIndex = 2;
            ApplicationTabIndex = 3;
            HostTabIndex = 4;
            PortTabIndex = 5;
            ExpanderTabIndex = 6;
            LoginTabIndex = 7;
            CancelTabIndex = 8;
            AppTypeHeight = Settings.GetInstance().IsAppTypeEnabled ? 28 : 0;
            ExpanderHeader = "Less";
        }

        /// <summary>
        /// Exp_s the collapsed.
        /// </summary>
        private void ExpCollapsed()
        {
            RowHeight = 0;
            AppTypeHeight = 0;
            ExpanderHeader = "More";

            UnameTabIndex = 0;
            PwdTabIndex = 0;
            ExpanderTabIndex = 2;
            LoginTabIndex = 3;
            CancelTabIndex = 4;

            PlaceTabIndex = 0;
            ApplicationTabIndex = 0;
            HostTabIndex = 0;
            PortTabIndex = 0;
        }

        /// <summary>
        /// Win_s the deactivated.
        /// </summary>
        private void WinDeActivated()
        {
            ShadowEffect.Opacity = 0;
        }

        /// <summary>
        /// Win_s the activated.
        /// </summary>
        private void WinActivated()
        {
            //object obj = (ColorConverter.ConvertFromString("#0070C5"));
            try
            {
                logger.Debug("LoginWindowViewModel : WinActivated Method : Method Entry");
                //if (obj != null)
                //    BorderBrush = new SolidColorBrush((Color)obj);

                ShadowEffect = new DropShadowBitmapEffect { ShadowDepth = 0, Opacity = 0.5, Softness = 0.5 };
                //object convertFromString = ColorConverter.ConvertFromString("#003660");
                //if (convertFromString != null)
                //    ShadowEffect.Color = (Color)convertFromString;
            }
            catch (Exception ex)
            {
                logger.Error("LoginWindowModel : WinActivated : " + ex.Message);
            }
            finally
            {
                ShadowEffect = null;
                GC.Collect();
                logger.Debug("LoginWindowViewModel : WinActivated Method : Method Exit");
            }

        }

        /// <summary>
        /// Cancels this instance.
        /// </summary>
        private void Cancel()
        {
            Environment.Exit(0);
            //Application.Current.Shutdown();
        }

        /// <summary>
        /// Drag_s the move.
        /// </summary>
        private void DragMove()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Title == "Login")
                {
                    window.DragMove();
                }
            }
        }

        /// <summary>
        /// Checks the permission.
        /// </summary>
        /// <param name="folderPath">The folder path.</param>
        public void CheckPermission(string folderPath)
        {
            logger.Debug("LoginWindowViewModel : CheckPermission Method : Entry");
            try
            {                
                string user = Environment.UserDomainName + "\\" + Environment.UserName;
                var accessControlList = Directory.GetAccessControl(folderPath);
                if (accessControlList != null)
                {
                    var tempaccess = accessControlList.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                    if (tempaccess != null)
                    {
                        foreach (FileSystemAccessRule rule in tempaccess)
                        {
                            NTAccount ntAccount = rule.IdentityReference as NTAccount;
                            if (ntAccount.Value.Equals(user))
                            {
                                if (rule.AccessControlType == AccessControlType.Allow)
                                {
                                    if ((FileSystemRights.Write & rule.FileSystemRights) == FileSystemRights.Write)
                                    {
                                        DictFolderPermissions.Add(FileSystemRights.Write.ToString(), true);
                                    }
                                    if ((FileSystemRights.Read & rule.FileSystemRights) == FileSystemRights.Read)
                                    {
                                        DictFolderPermissions.Add(FileSystemRights.Read.ToString(), true);
                                    }
                                }
                            }
                        }

                        if (!DictFolderPermissions.ContainsKey(FileSystemRights.Write.ToString()))
                        {
                            DictFolderPermissions.Add(FileSystemRights.Write.ToString(), false);
                        }

                        if (!DictFolderPermissions.ContainsKey(FileSystemRights.Read.ToString()))
                        {
                            DictFolderPermissions.Add(FileSystemRights.Read.ToString(), false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("LoginWindowViewModel : CheckPermission Method : " + ex.Message);
            }
            finally
            {
                logger.Debug("LoginWindowViewModel : CheckPermission Method : Exit");
                GC.Collect();
            }
        }

        #region Genesys Server Connection

        /// <summary>
        /// Genesyses the server asynchronous onnection.
        /// </summary>
        public bool GenesysServerConnection()
        {
            bool Result = false;
            Output = new OutputValues();

            try
            {
                logger.Debug("LoginWindowViewModel : GenesysServerConnection Method : Entry");

                #region Genesys Server Connection

                if (Settings.GetInstance().DefaultAuthentication.Contains(StatisticsEnum.StatSource.StatServer.ToString()) || Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                {
                    Output = stat.ConfigConnectionEstablish(Host, Port, ApplicationName, UserName, Password, UserName, Settings.GetInstance().ApplicationType, String.Empty, true);
                }
                else
                {
                    Output = stat.ConfigConnectionEstablish(Host, Port, ApplicationName, Settings.GetInstance().DefaultUsername, Settings.GetInstance().DefaultPassword, Settings.GetInstance().logUserName, Settings.GetInstance().ApplicationType, UserName, false);
                }

                if (Output.MessageCode == "2000")
                {
                    //Output = stat.CheckPlace(Place);

                    //if (Output.MessageCode == "2001")
                    //{

                    logger.Info("LoginWindowViewModel : GenesysServerConnection Method : Username : " + UserName);

                    Settings.GetInstance().UserName = UserName;
                    Settings.GetInstance().Password = Password;
                    Settings.GetInstance().ApplicationName = ApplicationName;
                    Settings.GetInstance().Host = Host;
                    Settings.GetInstance().Port = Port;
                    Settings.GetInstance().Place = Place;

                    bool IsAuthenticated;
                    string op = stat.CheckUserPrevileges(Settings.GetInstance().UserName);
                    string[] Authenticated = op.Split(',');
                    IsAdmin = Convert.ToBoolean(Authenticated[1].ToString());
                    IsAuthenticated = Convert.ToBoolean(Authenticated[0]);

                    Settings.GetInstance().DictEnableDisableChannels = stat.GetEnableDisableChannels();
                    Settings.GetInstance().DictErrorValues = stat.GetErrorValues();

                    if (IsAuthenticated)
                    {
                        //if added condition to check access right on application data folder  -03/09/2014
                        logger.Warn("LoginWindowViewModel : Login Method : IsAuthenticated True");
                        //System.Windows.MessageBox.Show("IS Plugin : " + StatisticsBase.GetInstance().isPlugin.ToString());

                        if (!StatisticsBase.GetInstance().isPlugin)
                        {
                            string passwordEnc = objXmlReader.EncodeTo64UTF8(Password);
                            objXmlReader.SaveInitializeParameters(ApplicationName, UserName, passwordEnc, Place, Host,
                                Port, Settings.GetInstance().ApplicationType, Settings.GetInstance().DefaultAuthentication);
                        }
                        //CheckPermission(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

                        //if (DictFolderPermissions.Count != 0)
                        //{
                        //    if (DictFolderPermissions.ContainsKey(FileSystemRights.Write.ToString()))
                        //    {
                        //        if (DictFolderPermissions[FileSystemRights.Write.ToString()])
                        //        {
                        //            string passwordEnc = objXmlReader.EncodeTo64UTF8(Password);
                        //            objXmlReader.SaveInitializeParameters(ApplicationName, UserName, passwordEnc, Place, Host,
                        //                Port, Settings.GetInstance().ApplicationType, Settings.GetInstance().DefaultAuthentication);
                        //        }
                        //        else
                        //        {
                        //            logger.Debug("Inner");
                        //        }
                        //    }
                        //    else
                        //    {
                        //        logger.Debug("middle");
                        //    }
                        //}
                        //else
                        //{
                        //    logger.Debug("Outer");
                        //}

                        //else
                        //{
                        //XmlDataDocument xmldoc = new XmlDataDocument();
                        //string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Pointel\\AgentInteractionDesktopV5.0.3.11\\app_config.xml";
                        //xmldoc.Load(path);
                        //XmlElement node1 = xmldoc.SelectSingleNode("/Settings/AppSetting/Place") as XmlElement;

                        //if (node1 != null && Place != node1.InnerText)
                        //{
                        //    node1.InnerText = Place;

                        //    xmldoc.Save(path);
                        //}
                        //}
                        Settings.GetInstance().IsCMEAuthenticated = true;
                        Result = true;
                    }
                    else
                    {

                        Settings.GetInstance().IsCMEAuthenticated = false;

                        //ErrorRowHeight = GridLength.Auto;
                        CMEError = Settings.GetInstance().DictErrorValues["user.authorization"].ToString();
                        ErrorMessage = Settings.GetInstance().DictErrorValues["user.authorization"].ToString();
                        Output.Message = Settings.GetInstance().DictErrorValues["user.authorization"].ToString();
                        // ErrorMessage = Output.Message; 
                        logger.Error("LoginWindowViewModel : GenesysServerConnection Method : " + ErrorMessage);
                    }
                    //}
                    //else
                    //{

                    //    Settings.GetInstance().IsCMEAuthenticated = false;
                    //    CMEError = Output.Message;
                    //    ErrorMessage = Output.Message;
                    //    //ErrorRowHeight = GridLength.Auto;
                    //    //Logger.Info("LoginWindowViewModel : GenesysServerConnection Method : " + ErrorMessage);

                    //}
                }
                else if (Output.MessageCode == "2002")
                {
                    ErrorRowHeight = GridLength.Auto;

                }
                else
                {
                    Settings.GetInstance().IsCMEAuthenticated = false;
                    string[] errorList = Output.Message.Split(new[] { ':' });
                    if (Settings.GetInstance().ApplicationType != StatisticsEnum.StatSource.All.ToString())
                    {
                        ErrorRowHeight = GridLength.Auto;
                    }
                    CMEError = errorList.Length > 2 ? errorList[2] : errorList[0];

                    logger.Info("LoginWindowViewModel : GenesysServerConnection Method : " + CMEError);
                }

                #endregion
            }
            catch (Exception GeneralException)
            {
                logger.Error("LoginWindowViewModel : GenesysServerCOnnection Method : Exception caught :  " + GeneralException.Message);

            }
            finally
            {
                logger.Debug("LoginWindowViewModel : GenesysServerCOnnection Method : Exit");
            }
            return Result;
        }

        #endregion


        #region Database Connection

        /// <summary>
        /// Databases the connection.
        /// </summary>
        public bool DatabaseConnection()
        {
            bool Result = false;
            try
            {
                logger.Debug("LoginWindowViewModel : DatabaseConnection Method : Entry");

                //System.Windows.MessageBox.Show("LoginWindowViewModel : DatabaseConnection Method : Entry");

                #region DB Connection

                objXmlReader.ReadDBSettings();

                //Output = stat.DBLoginCheck(Settings.GetInstance().dbHost, Settings.GetInstance().dbPort, Settings.GetInstance().dbLoginQuery, Settings.GetInstance().dbType, Settings.GetInstance().dbName, Settings.GetInstance().dbUsername, Settings.GetInstance().dbPassword, Settings.GetInstance().dbSid, Settings.GetInstance().dbSname, Settings.GetInstance().dbSource, Settings.GetInstance().logUserName, StatisticsEnum.StatSource.All.ToString(), UserName, objXmlReader.EncodeTo64UTF8(Password));


                if (Output.MessageCode == "2000")
                {

                    logger.Info("LoginWindowViewModel : DatabaseConnection Method : Username : " + UserName);

                    Settings.GetInstance().UserName = UserName;
                    Settings.GetInstance().Password = Password;
                    Settings.GetInstance().ApplicationName = ApplicationName;
                    Settings.GetInstance().Host = Host;
                    Settings.GetInstance().Port = Port;

                    Settings.GetInstance().DictEnableDisableChannels = stat.GetEnableDisableChannels();
                    Settings.GetInstance().DictErrorValues = stat.GetErrorValues();


                    string passwordEnc = objXmlReader.EncodeTo64UTF8(Password);
                    objXmlReader.SaveInitializeParameters(ApplicationName, UserName, passwordEnc, Place, Host,
                        Port, Settings.GetInstance().ApplicationType, Settings.GetInstance().DefaultAuthentication);

                    Settings.GetInstance().IsDBAuthenticated = true;
                    Result = true;
                }
                else
                {
                    Settings.GetInstance().IsDBAuthenticated = false;
                    //DBError = Output.Message;
                    if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                    {
                        string Message = string.Empty;
                        int CurrentErrrorCount = 1;
                        foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                        {
                            if (CurrentErrrorCount <= Settings.GetInstance().ErrorDisplayCount)
                            {
                                if (Message == string.Empty)
                                    Message = "'" + MissingKeys + "' value is Missing";
                                else
                                    Message = Message + "\n '" + MissingKeys + "' value is Missing";
                            }
                            CurrentErrrorCount++;
                        }
                        DBError = Message;
                    }
                    else
                    {
                        string[] errorList = Output.Message.Split(new[] { ':' });
                        //ErrorRowHeight = GridLength.Auto;
                        DBError = errorList.Length > 2 ? errorList[2] : errorList[0];
                    }

                    logger.Info("LoginWindowViewModel : DatabaseConnection Method : " + Output.Message);
                }

                #endregion

            }
            catch (Exception GeneralException)
            {
                //System.Windows.MessageBox.Show("LoginWindowViewModel : DatabaseConnection Method : Exception : " + GeneralException.Message);
                logger.Error("LoginWindowViewModel : DatabaseConnection Method : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("LoginWindowViewModel : DatabaseConnection Method : Exit");
            }
            return Result;
        }

        #endregion

        /// <summary>
        /// Logins this instance.
        /// </summary>
        public void Login()
        {
            try
            {
                logger.Debug("LoginWindowViewModel : Login Method : Entry");
                //System.Windows.Forms.MessageBox.Show("Login Started");
                StatisticsBase.GetInstance().IsMandatoryFieldsMissing = false;
                ValidationClass objValidate = new ValidationClass();
                CMEError = string.Empty;
                DBError = string.Empty;
                string error;
                bool authenticated = false;

                if ((Host != null || Settings.GetInstance().IsHostEnabled) && (Port != null || Settings.GetInstance().IsPortEnabled) && (UserName != null && UserName != string.Empty) &&
                  (((ApplicationName != null || Settings.GetInstance().IsAppNameEnabled) && (ApplicationName != string.Empty)) || Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString()) &&
                   ((Place != null & Place != "") || Settings.GetInstance().ApplicationType != StatisticsEnum.StatSource.StatServer.ToString()) &&
                    (Settings.GetInstance().ApplicationType != string.Empty && Settings.GetInstance().ApplicationType != ""))
                {
                    if (Password == null)
                    {
                        Password = string.Empty;
                    }

                    WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
                    if (windowsIdentity != null)
                        Settings.GetInstance().logUserName = windowsIdentity.Name.Split('\\')[1];

                    if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                    {
                        authenticated = GenesysServerConnection();
                        //System.Windows.Forms.MessageBox.Show("authenticated : "+authenticated.ToString());
                    }
                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
                    {
                        StatisticsBase.GetInstance().IsAllType = false;
                        authenticated = DatabaseConnection();
                    }
                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                    {
                        StatisticsBase.GetInstance().ApplicationType = Settings.GetInstance().ApplicationType;

                        if (Settings.GetInstance().DefaultAuthentication == string.Empty || Settings.GetInstance().DefaultAuthentication == "")
                        {
                            StatisticsBase.GetInstance().DefaultAuthentication = Settings.GetInstance().DefaultAuthentication = StatisticsEnum.StatSource.StatServer.ToString() + "," + StatisticsEnum.StatSource.DB.ToString();
                        }
                        else
                        {
                            StatisticsBase.GetInstance().DefaultAuthentication = Settings.GetInstance().DefaultAuthentication;
                        }

                        string[] authenticateType = Settings.GetInstance().DefaultAuthentication.Split(',');

                        GenesysServerConnection();

                        if (!StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                        {
                            foreach (string Authentication in authenticateType)
                            {
                                //if (Authentication == StatisticsEnum.StatSource.StatServer.ToString())
                                //{
                                //    GenesysServerConnection();
                                //    //if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                //    //{
                                //    //    StatisticsBase.GetInstance().IsAllType = true;
                                //    //    DatabaseConnection();
                                //    //}
                                //}

                                if (Authentication == StatisticsEnum.StatSource.DB.ToString())
                                {
                                    StatisticsBase.GetInstance().IsAllType = true;
                                    DatabaseConnection();
                                    //if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                    //{
                                    //    GenesysServerConnection();
                                    //}
                                }
                            }
                        }

                        if (!Settings.GetInstance().DefaultAuthentication.Contains(StatisticsEnum.StatSource.DB.ToString()))
                        {
                            Settings.GetInstance().IsDBAuthenticated = true;
                        }

                        Views.MessageBox msgbox = new Views.MessageBox();

                        if (Settings.GetInstance().IsCMEAuthenticated || Settings.GetInstance().IsDBAuthenticated)
                        {
                            authenticated = true;

                            if (Settings.GetInstance().DefaultAuthentication.Contains("StatServer"))
                            {
                                if (!Settings.GetInstance().IsCMEAuthenticated)
                                {
                                    if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                                    {
                                        string Message = string.Empty;
                                        int CurrentErrrorCount = 1;
                                        foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                                        {
                                            if (CurrentErrrorCount <= Settings.GetInstance().ErrorDisplayCount)
                                            {
                                                if (Message == string.Empty)
                                                    Message = "'" + MissingKeys + "' value is Missing";
                                                else
                                                    Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                            }
                                            CurrentErrrorCount++;
                                        }
                                        ErrorMessage = Message;
                                        MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in CME Authentication", ErrorMessage, msgbox, "MainWindow", Settings.GetInstance().Theme);
                                        msgbox.DataContext = mboxviewmodel;
                                        msgbox.ShowDialog();
                                    }
                                    else
                                    {
                                        MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in CME Authentication", CMEError, msgbox, "MainWindow", Settings.GetInstance().Theme);
                                        msgbox.DataContext = mboxviewmodel;
                                        msgbox.ShowDialog();
                                    }
                                }
                            }

                            if (Settings.GetInstance().DefaultAuthentication.Contains("DB"))
                            {
                                if (!Settings.GetInstance().IsDBAuthenticated)
                                {
                                    if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                                    {
                                        string Message = string.Empty;
                                        int CurrentErrrorCount = 1;
                                        foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                                        {
                                            if (CurrentErrrorCount <= Settings.GetInstance().ErrorDisplayCount)
                                            {
                                                if (Message == string.Empty)
                                                    Message = "'" + MissingKeys + "' value is Missing";
                                                else
                                                    Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                            }
                                            CurrentErrrorCount++;
                                        }
                                        ErrorMessage = Message;
                                        MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in DB Authentication", ErrorMessage, msgbox, "MainWindow", Settings.GetInstance().Theme);
                                        msgbox.DataContext = mboxviewmodel;
                                        msgbox.ShowDialog();
                                    }
                                    else
                                    {
                                        MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in DB Authentication", Settings.GetInstance().DictErrorValues["db.authentication"].ToString(), msgbox, "MainWindow", Settings.GetInstance().Theme);
                                        msgbox.DataContext = mboxviewmodel;
                                        msgbox.ShowDialog();
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString() && (Settings.GetInstance().DefaultAuthentication.Contains(StatisticsEnum.StatSource.StatServer.ToString()) && Settings.GetInstance().DefaultAuthentication.Contains(StatisticsEnum.StatSource.DB.ToString())))
                            {
                                if (Settings.GetInstance().DefaultAuthentication.Contains("StatServer"))
                                {
                                    if (!Settings.GetInstance().IsCMEAuthenticated)
                                    {
                                        if (Settings.GetInstance().isFirstTime)
                                        {
                                            if (CMEError != string.Empty || CMEError != "")
                                            {
                                                ErrorRowHeight = GridLength.Auto;
                                                ErrorMessage = CMEError;
                                            }
                                            else
                                            {
                                                if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                                                {
                                                    string Message = string.Empty;
                                                    int CurrentErrrorCount = 1;
                                                    foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                                                    {
                                                        if (CurrentErrrorCount <= StatisticsBase.GetInstance().ErrorCount)
                                                        {
                                                            if (Message == string.Empty)
                                                                Message = "'" + MissingKeys + "' value is Missing";
                                                            else
                                                                Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                                        }
                                                        CurrentErrrorCount++;
                                                    }
                                                    ErrorMessage = Message;
                                                }
                                                else
                                                {
                                                    ErrorRowHeight = GridLength.Auto;
                                                    ErrorMessage = CMEError;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                                            {
                                                string Message = string.Empty;
                                                int CurrentErrrorCount = 1;
                                                foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                                                {
                                                    if (CurrentErrrorCount <= Settings.GetInstance().ErrorDisplayCount)
                                                    {
                                                        if (Message == string.Empty)
                                                            Message = "'" + MissingKeys + "' value is Missing";
                                                        else
                                                            Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                                    }
                                                    CurrentErrrorCount++;
                                                }
                                                ErrorMessage = Message;
                                                MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in CME Authentication", ErrorMessage, msgbox, "MainWindow", Settings.GetInstance().Theme);
                                                msgbox.DataContext = mboxviewmodel;
                                                msgbox.ShowDialog();
                                            }
                                            else
                                            {
                                                MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in CME Authentication", CMEError, msgbox, "MainWindow", Settings.GetInstance().Theme);
                                                msgbox.DataContext = mboxviewmodel;
                                                msgbox.ShowDialog();
                                            }
                                        }
                                    }
                                }

                                if (!Settings.GetInstance().DefaultAuthentication.Contains("StatServer") || !StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                                {
                                    if (Settings.GetInstance().DefaultAuthentication.Contains("DB"))
                                    {
                                        if (!Settings.GetInstance().IsDBAuthenticated)
                                        {
                                            if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                                            {
                                                string Message = string.Empty;
                                                int CurrentErrrorCount = 1;
                                                foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                                                {
                                                    if (CurrentErrrorCount <= Settings.GetInstance().ErrorDisplayCount)
                                                    {
                                                        if (Message == string.Empty)
                                                            Message = "'" + MissingKeys + "' value is Missing";
                                                        else
                                                            Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                                    }
                                                    CurrentErrrorCount++;
                                                }
                                                ErrorMessage = Message;

                                                msgbox = new Views.MessageBox();
                                                MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in DB Authentication", ErrorMessage, msgbox, "MainWindow", Settings.GetInstance().Theme);
                                                msgbox.DataContext = mboxviewmodel;
                                                msgbox.ShowDialog();
                                            }
                                            else
                                            {
                                                msgbox = new Views.MessageBox();
                                                MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered in DB Authentication", DBError, msgbox, "MainWindow", Settings.GetInstance().Theme);
                                                msgbox.DataContext = mboxviewmodel;
                                                msgbox.ShowDialog();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (authenticated)
                    {
                        if (IsAdmin)
                        {
                            #region If User is Admin

                            //Microsoft.Windows.Controls.MessageBox.Show("Admin");
                            foreach (Window win in Application.Current.Windows)
                            {
                                if (win.Title == "Login")
                                {
                                    win.Hide();
                                }
                            }

                            Views.MessageBox msgbox = new Views.MessageBox();
                            ViewModels.MessageBoxViewModel mboxvmodel = new MessageBoxViewModel("Information", "Do you want to configure Statistics ?", msgbox, "ObjectChanged", Settings.GetInstance().Theme);
                            msgbox.DataContext = mboxvmodel;
                            msgbox.ShowDialog();

                            //Settings.GetInstance().DictServerFilters = stat.ReadFilters();

                            Settings.GetInstance().DictStatFormats = new Dictionary<string, string>();
                            Settings.GetInstance().DictStatFormats.Add("T", "(Time Format)");
                            Settings.GetInstance().DictStatFormats.Add("D", "(Decimal Format)");
                            Settings.GetInstance().DictStatFormats.Add("S", "(String Format)");

                            if (Settings.GetInstance().IsConfigureStats)
                            {
                                AdminConfigWindow adminConfigView = new AdminConfigWindow();
                                Settings.GetInstance().adminConfigVM = new AdminConfigWindowViewModel();
                                adminConfigView.DataContext = Settings.GetInstance().adminConfigVM;
                                adminConfigView.Show();

                                //View for configurting new statistics in 8.5 framework
                                //StatisticsWindow statisticsView = new StatisticsWindow();
                                //StatisticsWindowViewModel statistcisVM = new StatisticsWindowViewModel();
                                //statisticsView.DataContext = statistcisVM;
                                //statisticsView.Show();
                            }
                            else
                            {
                                AdminMainWindow adminMainView = new AdminMainWindow();
                                adminMainView.ShowDialog();
                            }

                            #endregion
                        }
                        else
                        {
                            //Microsoft.Windows.Controls.MessageBox.Show("User");
                            logger.Debug("WindowName :" + windowsIdentity.Name);

                            #region If User is User

                            if (Settings.GetInstance().IsGadgetShow)
                            {
                                if (Settings.GetInstance().mainview == null)
                                    Settings.GetInstance().mainview = new MainWindow();
                                if (Settings.GetInstance().mainVm == null)
                                    Settings.GetInstance().mainVm = new MainWindowViewModel();
                                Settings.GetInstance().mainview.DataContext = Settings.GetInstance().mainVm;

                                foreach (Window win in Application.Current.Windows)
                                {
                                    if (win.Title == "Login")
                                    {
                                        win.Hide();
                                    }
                                }

                                Settings.GetInstance().mainview.Show();
                                Settings.GetInstance().mainVm.dragwindowInitialize(Settings.GetInstance().mainview);
                            }
                            else
                            {
//                                Window win = new Window();
                                //                                win.Show();

                                if (Settings.GetInstance().mainview == null)
                                    Settings.GetInstance().mainview = new MainWindow();
                                if (Settings.GetInstance().mainVm == null)
                                    Settings.GetInstance().mainVm = new MainWindowViewModel();
                                Settings.GetInstance().mainview.DataContext = Settings.GetInstance().mainVm;

                                foreach (Window win in Application.Current.Windows)
                                {
                                    if (win.Title == "Login")
                                    {
                                        win.Hide();
                                    }
                                }

                                Settings.GetInstance().mainview.Show();
                                Settings.GetInstance().mainview.Hide();

                                if (Settings.GetInstance().QueueConfigWin == null)
                                    Settings.GetInstance().QueueConfigWin = new ObjectConfigWindow();
                                if (Settings.GetInstance().QueueConfigVM == null)
                                    Settings.GetInstance().QueueConfigVM = new ObjectConfigWindowViewModel();
                                Settings.GetInstance().QueueConfigWin.DataContext = Settings.GetInstance().QueueConfigVM;

                                foreach (Window win in Application.Current.Windows)
                                {
                                    if (win.Title == "Login")
                                    {
                                        win.Hide();
                                    }
                                }

                                Settings.GetInstance().QueueConfigWin.Show();

                                ////ObjectConfigWindow objView = new ObjectConfigWindow();
                                ////Settings.GetInstance().QueueConfigVM = new ObjectConfigWindowViewModel();
                                ////Settings.GetInstance().QueueConfigVM.LoadObjectConfiguration();
                                ////objView.DataContext = Settings.GetInstance().QueueConfigVM;
                                ////objView.Show();
                            }
                            //Commented by Elango.T, on 06/07/2015 for running the Application as seperate applicatio in plugin
                            //MainWindowViewModel mainVm = new MainWindowViewModel();
                            //MainWindow mainview = new MainWindow();
                            //MainWindowViewModel mainVm = new MainWindowViewModel();
                            //mainview.DataContext = mainVm;

                            //foreach (Window win in Application.Current.Windows)
                            //{
                            //    if (win.Title == "Login")
                            //    {
                            //        win.Hide();
                            //    }
                            //}

                            //mainview.Show();
                            //mainVm.dragwindowInitialize(mainview);

                            #endregion
                        }
                    }
                    else
                    {
                        if (Settings.GetInstance().isFirstTime)
                        {
                            if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                            {
                                if (!Settings.GetInstance().IsCMEAuthenticated && Settings.GetInstance().DefaultAuthentication.Contains(StatisticsEnum.StatSource.StatServer.ToString()))
                                {
                                    ErrorRowHeight = GridLength.Auto;

                                    if (CMEError != string.Empty || CMEError != "")
                                    {
                                        ErrorRowHeight = GridLength.Auto;
                                        ErrorMessage = CMEError;
                                    }
                                    else
                                    {
                                        if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                                        {
                                            string Message = string.Empty;
                                            int CurrentErrrorCount = 1;
                                            foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                                            {
                                                if (CurrentErrrorCount <= StatisticsBase.GetInstance().ErrorCount)
                                                {
                                                    if (Message == string.Empty)
                                                        Message = "'" + MissingKeys + "' value is Missing";
                                                    else
                                                        Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                                }
                                                CurrentErrrorCount++;
                                            }
                                            ErrorMessage = Message;
                                        }
                                        else
                                        {
                                            ErrorMessage = DBError;
                                        }
                                    }
                                }
                                else if (!Settings.GetInstance().IsDBAuthenticated && Settings.GetInstance().DefaultAuthentication.Contains(StatisticsEnum.StatSource.DB.ToString()) || Settings.GetInstance().IsCMEAuthenticated)
                                {
                                    ErrorRowHeight = GridLength.Auto;

                                    if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                                    {
                                        string Message = string.Empty;
                                        int CurrentErrrorCount = 1;
                                        foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                                        {
                                            if (CurrentErrrorCount <= Settings.GetInstance().ErrorDisplayCount)
                                            {
                                                if (Message == string.Empty)
                                                    Message = "'" + MissingKeys + "' value is Missing";
                                                else
                                                    Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                            }
                                            CurrentErrrorCount++;
                                        }
                                        ErrorMessage = Message;
                                    }
                                    else
                                    {
                                        ErrorMessage = DBError;
                                    }
                                }
                            }
                            else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())// || !Settings.GetInstance().IsDBAuthenticated)
                            {
                                string[] errorList = Output.Message.Split(new[] { ':' });

                                ErrorRowHeight = GridLength.Auto;

                                if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                                {
                                    string Message = string.Empty;
                                    int CurrentErrrorCount = 1;
                                    foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                                    {
                                        if (CurrentErrrorCount <= StatisticsBase.GetInstance().ErrorCount)
                                        {
                                            if (Message == string.Empty)
                                                Message = "'" + MissingKeys + "' value is Missing";
                                            else
                                                Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                        }
                                        CurrentErrrorCount++;
                                    }
                                    ErrorMessage = Message;
                                }
                                else
                                {
                                    ErrorMessage = errorList.Length > 2 ? errorList[2] : errorList[0];
                                }
                            }
                            else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
                            {
                                string[] errorList = Output.Message.Split(new[] { ':' });

                                ErrorRowHeight = GridLength.Auto;

                                if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                                {
                                    string Message = string.Empty;
                                    int CurrentErrrorCount = 1;
                                    foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                                    {
                                        if (CurrentErrrorCount <= Settings.GetInstance().ErrorDisplayCount)
                                        {
                                            if (Message == string.Empty)
                                                Message = "'" + MissingKeys + "' value is Missing";
                                            else
                                                Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                        }
                                        CurrentErrrorCount++;
                                    }
                                    ErrorMessage = Message;
                                }
                                else
                                {
                                    ErrorMessage = errorList.Length > 2 ? errorList[2] : errorList[0];
                                }
                            }
                            Password = string.Empty;
                        }
                        else
                        {
                            Views.MessageBox msgbox = new Views.MessageBox();

                            if (StatisticsBase.GetInstance().IsMandatoryFieldsMissing)
                            {
                                string Message = string.Empty;
                                int CurrentErrrorCount = 1;
                                foreach (string MissingKeys in StatisticsBase.GetInstance().ListMissedValues)
                                {
                                    if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString() || (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString() && StatisticsBase.GetInstance().isCMEAuthentication))
                                    {
                                        if (CurrentErrrorCount <= StatisticsBase.GetInstance().ErrorCount)
                                        {
                                            if (Message == string.Empty)
                                                Message = "'" + MissingKeys + "' value is Missing";
                                            else
                                                Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                        }
                                    }
                                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString() || (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString() && StatisticsBase.GetInstance().isDBAuthentication))
                                    {
                                        if (CurrentErrrorCount <= Settings.GetInstance().ErrorDisplayCount)
                                        {
                                            if (Message == string.Empty)
                                                Message = "'" + MissingKeys + "' value is Missing";
                                            else
                                                Message = Message + "\n '" + MissingKeys + "' value is Missing";
                                        }
                                    }
                                    CurrentErrrorCount++;
                                }
                                ErrorMessage = Message;

                                MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered", ErrorMessage, msgbox, "LoginWindow", Settings.GetInstance().Theme);
                                msgbox.DataContext = mboxviewmodel;
                                msgbox.ShowDialog();
                            }
                            else
                            {
                                string[] errorList = Output.Message.Split(new[] { ':' });

                                if (errorList.Length > 2)
                                {
                                    MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered", errorList[2], msgbox, "LoginWindow", Settings.GetInstance().Theme);
                                    msgbox.DataContext = mboxviewmodel;
                                    msgbox.ShowDialog();
                                }

                                else
                                {
                                    MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered", errorList[0], msgbox, "LoginWindow", Settings.GetInstance().Theme);
                                    msgbox.DataContext = mboxviewmodel;
                                    msgbox.ShowDialog();
                                }
                            }
                        }
                    }
                }
                else
                {
                    error = "Provide valid information";
                    if (Settings.GetInstance().isFirstTime)
                    {
                        //Microsoft.Windows.Controls.MessageBox.Show("Error");
                        ErrorRowHeight = GridLength.Auto;
                        ErrorMessage = error;
                        logger.Info("LoginWindowViewModel : Login Method : " + error);
                    }
                    else
                    {
                        Views.MessageBox msgbox = new Views.MessageBox();
                        MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered", error, msgbox, "LoginWindow", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxviewmodel;
                        msgbox.ShowDialog();
                    }
                }

                #region old code
                //if (Host != null && Port != null && UserName != null && UserName != string.Empty &&
                //    ApplicationName != null && Place != null & Place!="")
                //{
                //    Output = objValidate.LoginValidation(Port,Place);
                //    if (Output.MessageCode!= "2000")
                //    {
                //        Logger.Warn("LoginWindowViewModel : Login Method : Login Validation Successfull");
                //        if (Password == null)
                //        {
                //            Password = string.Empty;
                //        }
                //        WindowsIdentity windowsIdentity = WindowsIdentity.GetCurrent();
                //        if (windowsIdentity != null)
                //            Settings.GetInstance().logUserName = windowsIdentity.Name.Split('\\')[1];

                //        Output = stat.ConfigConnectionEstablish(Host,Port, ApplicationName, UserName, Password,Settings.GetInstance().logUserName);

                //        if (Output.MessageCode == "2000")
                //        {
                //            Logger.Warn("LoginWindowViewModel : Login Method : Authentication success");              

                //            Output = stat.CheckPlace(Place);

                //            if (Output.MessageCode == "2001")
                //            {
                //                Logger.Warn("LoginWindowViewModel : Login Method : Check Place success");              

                //                Logger.Info("LoginWindowViewModel : Login Method : Username : " + UserName);

                //                Settings.GetInstance().UserName = UserName;
                //                Settings.GetInstance().Password = Password;
                //                Settings.GetInstance().ApplicationName = ApplicationName;
                //                Settings.GetInstance().Host = Host;
                //                Settings.GetInstance().Port = Port;
                //                Settings.GetInstance().Place = Place;

                //                string op = stat.CheckUserPrevileges(Settings.GetInstance().UserName);
                //                string[] Authenticated = op.Split(',');
                //                IsAdmin = Convert.ToBoolean(Authenticated[1].ToString());
                //                IsAuthenticated =Convert.ToBoolean(Authenticated[0]);

                //                if (IsAuthenticated)
                //                {
                //                    //if added condition to check access right on application data folder  -03/09/2014
                //                    Logger.Warn("LoginWindowViewModel : Login Method : IsAuthenticated True");  

                //                    CheckPermission(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

                //                    if (DictFolderPermissions.Count != 0)
                //                    {
                //                        if (DictFolderPermissions.ContainsKey(FileSystemRights.Write.ToString()))
                //                        {
                //                            if (DictFolderPermissions[FileSystemRights.Write.ToString()])
                //                            {
                //                                string passwordEnc = objXmlReader.EncodeTo64UTF8(Password);
                //                                objXmlReader.SaveInitializeParameters(ApplicationName, UserName, passwordEnc, Place, Host,
                //                                    Port);
                //                            }
                //                        }
                //                    }
                //                       // if commented for dont write statistics on stattickerfive.exe.config file -03/09/2014

                //                    #region To Write Place in the Application Installed location

                //                        //string appPath = string.Empty;
                //                        //appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                //                        //string configFile = System.IO.Path.Combine(appPath, Settings.execonfig);
                //                        //ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                //                        //configFileMap.ExeConfigFilename = configFile;

                //                        //var statkey = ConfigurationManager.AppSettings["Place"];
                //                        //if (string.IsNullOrEmpty(statkey))
                //                        //{
                //                        //    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                //                        //    config.AppSettings.Settings.Remove("Place");
                //                        //    config.AppSettings.Settings.Add("Place", Place);
                //                        //    config.Save(ConfigurationSaveMode.Minimal);
                //                        //}
                //                        //else
                //                        //{
                //                        //    System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                //                        //    config.AppSettings.Settings["Place"].Value = Place;
                //                        //    config.Save();
                //                    //}

                //                    #endregion

                //                    Settings.GetInstance().DictEnableDisableChannels = stat.GetEnableDisableChannels();
                //                    Settings.GetInstance().DictErrorValues = stat.GetErrorValues();                                                         

                //                    if (IsAdmin)
                //                    {

                //                        MainWindow mainview = new MainWindow();
                //                        MainWindowViewModel mainVm = new MainWindowViewModel();
                //                        mainview.DataContext = mainVm;

                //                        foreach (Window win in Application.Current.Windows)
                //                        {
                //                            if (win.Title == "Login")
                //                            {
                //                                win.Hide();
                //                            }
                //                        }

                //                        mainview.Show();
                //                           mainVm.dragwindowInitialize(mainview);


                //                    }
                //                    else
                //                    {
                //                        MainWindow mainview = new MainWindow();
                //                        MainWindowViewModel mainVm = new MainWindowViewModel();
                //                        mainview.DataContext = mainVm;

                //                        foreach (Window win in Application.Current.Windows)
                //                        {
                //                            if (win.Title == "Login")
                //                            {
                //                                win.Hide();
                //                            }
                //                        }

                //                        mainview.Show();
                //                         mainVm.dragwindowInitialize(mainview);
                //                    }
                //                }
                //                else
                //                {
                //                    if (Settings.GetInstance().isFirstTime)
                //                    {
                //                        ErrorRowHeight = GridLength.Auto;
                //                        error = Settings.GetInstance().DictErrorValues["user.authorization"].ToString();
                //                        ErrorMessage = error;
                //                        Logger.Warn("LoginWindowViewModel : Login Method : " + error);
                //                    }
                //                    else
                //                    {
                //                        Views.MessageBox msgbox = new Views.MessageBox();

                //                        string errorList = Settings.GetInstance().DictErrorValues["user.authorization"].ToString();

                //                        MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered", errorList, msgbox, "LoginWindow");
                //                        msgbox.DataContext = mboxviewmodel;
                //                        msgbox.ShowDialog();

                //                    }
                //                }

                //            }
                //            else
                //            {
                //                if (Settings.GetInstance().isFirstTime)
                //                {
                //                    ErrorMessage = Output.Message;
                //                    ErrorRowHeight = GridLength.Auto;
                //                    Logger.Warn("LoginWindowViewModel : Login Method : " + ErrorMessage);
                //                }
                //                else
                //                {
                //                    Views.MessageBox msgbox = new Views.MessageBox();
                //                    string errorList = Output.Message;
                //                    MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered", errorList, msgbox, "LoginWindow");
                //                    msgbox.DataContext = mboxviewmodel;
                //                    msgbox.ShowDialog();
                //                }
                //            }
                //        }
                //        else
                //        {
                //            if (Settings.GetInstance().isFirstTime)
                //            {
                //                string[] errorList = Output.Message.Split(new[] { ':' });

                //                ErrorRowHeight = GridLength.Auto;

                //                ErrorMessage = errorList.Length > 2 ? errorList[2] : errorList[0];
                //            }
                //            else
                //            {

                //                Views.MessageBox msgbox = new Views.MessageBox();

                //                string[] errorList = Output.Message.Split(new[] { ':' });

                //                if (errorList.Length > 2)
                //                {
                //                    MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered", errorList[2], msgbox, "LoginWindow");
                //                    msgbox.DataContext = mboxviewmodel;
                //                    msgbox.ShowDialog();
                //                }

                //                else
                //                {
                //                    MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered", errorList[0], msgbox, "LoginWindow");
                //                    msgbox.DataContext = mboxviewmodel;
                //                    msgbox.ShowDialog();
                //                }

                //            }
                //            Logger.Warn("LoginWindowViewModel : Login Method : " + ErrorMessage);
                //        }
                //    }
                //    else
                //    {
                //        if (Settings.GetInstance().isFirstTime)
                //        {
                //            ErrorMessage = Output.Message;
                //            ErrorRowHeight = GridLength.Auto;
                //            Logger.Warn("LoginWindowViewModel : Login Method : " + ErrorMessage);
                //        }
                //        else
                //        {
                //            Views.MessageBox msgbox = new Views.MessageBox();
                //            string errorList = Output.Message;
                //            MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered", errorList, msgbox, "LoginWindow");
                //            msgbox.DataContext = mboxviewmodel;
                //            msgbox.ShowDialog();   
                //        }

                //    }
                //}
                //else
                //{   
                //    error = "Provide valid information";
                //    if (Settings.GetInstance().isFirstTime)
                //    {
                //        ErrorRowHeight = GridLength.Auto;
                //        ErrorMessage = error;
                //        Logger.Warn("LoginWindowViewModel : Login Method : " + error);
                //    }
                //    else
                //    {
                //        Views.MessageBox msgbox = new Views.MessageBox();
                //        MessageBoxViewModel mboxviewmodel = new MessageBoxViewModel("Problem Encountered", error, msgbox, "LoginWindow");
                //        msgbox.DataContext = mboxviewmodel;
                //        msgbox.ShowDialog();   
                //    }
                //}

                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("LoginWindowViewModel : Login Method : " + ex.Message);
            }
            finally
            {
                logger.Debug("LoginWindowViewModel : Login Method : Exit");
                GC.Collect();
            }
        }

        #endregion
    }
}
