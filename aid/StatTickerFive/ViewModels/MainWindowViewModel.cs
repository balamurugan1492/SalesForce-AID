namespace StatTickerFive.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.IO.Pipes;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Security.AccessControl;
    using System.Security.Policy;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Forms;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Media.Imaging;
    using System.Windows.Resources;
    using System.Windows.Threading;

    using Genesyslab.Platform.ApplicationBlocks.Commons;
    using Genesyslab.Platform.Reporting.Protocols.StatServer;

    using Pointel.Logger;
    using Pointel.Statistics.Core;
    using Pointel.Statistics.Core.Utility;

    using StatTickerFive.Helpers;
    using StatTickerFive.UserControls;
    using StatTickerFive.UserControls.View;
    using StatTickerFive.Views;

    using WPFGrowlNotification;

    using Application = System.Windows.Application;

    using Color = System.Drawing.Color;

    using ContextMenu = System.Windows.Controls.ContextMenu;

    using MenuItem = System.Windows.Forms.MenuItem;

    using Orientation = System.Windows.Controls.Orientation;

    using Point = System.Windows.Point;

    using UserControl = System.Windows.Controls.UserControl;
    using System.Xml;
    using System.Text;

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WIN32Rectangle
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    /// <summary>
    /// 
    /// </summary>
    public class MainWindowViewModel : NotificationObject, IStatTicker
    {
        #region Fields

        public static Dictionary<string, string> DicTemp = new Dictionary<string, string>();
        public static Hashtable hashTaggedStats = new Hashtable();
        public static bool IsClosed = false;
        public static int TagNo = 0;

        public Hashtable hashTagTooltips = new Hashtable();

        private const double leftOffset = 300;
        private const int MF_BYPOSITION = 0x0400;
        private const int MF_DISABLED = 0x0002;
        private const Int32 SIZE_MAXHIDE = 0x0004;
        private const Int32 SIZE_MAXIMIZED = 0x0002;
        private const Int32 SIZE_MAXSHOW = 0x0003;
        private const Int32 SIZE_MINIMIZED = 0x0001;
        private const Int32 SIZE_RESTORED = 0x0000;
        private const double topOffset = 220;
        private const Int32 WM_EXITSIZEMOVE = 0x0232;
        const int WM_MOVING = 0x0216;
        private const Int32 WM_SIZE = 0x0005;
        const int WM_SIZING = 0x0214;
        private const uint WM_SYSTEMMENU = 0xa4;
        private const uint WP_SYSTEMMENU = 0x02;

        readonly GrowlNotifications growlNotifications = new GrowlNotifications();

        static bool DBResult;
        static string DBTaggedStats = string.Empty;
        private static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        static bool Result;
        static bool ServerResult;
        static string taggedStats = string.Empty;
        static double tagHeight = 0;
        static double tempTagheight = 0;
        static Control1 userControl;

        bool beginInvoke = true;
        double bottomMargin;
        Color DBColor;
        bool defaultHeader = false;
        string[] DeviceName;
        SortedDictionary<int, string> dict = new SortedDictionary<int, string>();
        SortedDictionary<int, string> dictAddedStatistics = new SortedDictionary<int, string>();
        Dictionary<string, int> dictAudioAlertAttempts = new Dictionary<string, int>();
        Dictionary<string, int> dictPopupAlertAttempts = new Dictionary<string, int>();
        string ErrorCode = string.Empty;
        string ErrorMessage = string.Empty;
        ObservableCollection<UserControl> ExceededTagCollection = new ObservableCollection<UserControl>();
        Hashtable hashTagDetails = new Hashtable();
        int i = 1;
        Point InitialWindowLocation;
        bool isEnableMessageBox = true;
        bool isHeaderClicked = true;
        bool isHeaderHide = false;
        bool isHeaderHideShortLength = false;
        bool isMenuClicked = false;
        bool isMinimized;
        bool isNext;
        bool isNotiferPopupSelected = false;
        bool isNotifierAudioSelected = false;
        bool isPrevious;
        bool IsSecondaryOrientaion = false;
        bool IsSecondScreen = false;
        private bool isShowCCStatistics = true;
        bool isShowHeader = true;
        private bool isShowMyStatistics = true;
        bool isThirdRowActive = false;
        private List<string> listTaggedStatObjects = new List<string>();
        List<string> LstNotifierIds = new List<string>();
        Window Main_Window;
        private MediaPlayer mediaPlayer = new System.Windows.Media.MediaPlayer();
        DispatcherTimer Mediatimer = new DispatcherTimer();
        private CustomAlertNotifier notifierPopup;
        System.Windows.Forms.ContextMenu notifyContext = new System.Windows.Forms.ContextMenu();
        int NumberOfTags_Horizontally = 1;
        int NumberOfTags_Vertically = 1;
        private LowLevelKeyboardProc objKeyboardProcess;
        private NotifyIcon objNotify;
        StatisticsBase objStatTicker = new StatisticsBase();
        XMLStorage objXmlReader = new XMLStorage();
        OutputValues output;
        DispatcherTimer PausedStatTimer1 = new DispatcherTimer();
        string PausedTag;
        private Thread pipeThread = null;
        private Keys previousKey = 0;
        string PreviousScreen;
        double priheight;
        string[] PrimaryScreen;
        int PrimaryScreenHeight;
        int PrimaryScreenWidth;
        double primwidh;

        //Declaring Global objects
        private IntPtr ptrHook;
        string ReferenceId = string.Empty;
        int RemainTags;
        Queue<string> removedNo = new Queue<string>();
        double rightMargin;
        int ScreenWidth;
        int SecondaryScreenHeight;
        int SecondaryScreenWidth;
        Color ServerColor;
        DispatcherTimer StatDisplayTimer = new DispatcherTimer();
        string strNewStatName = string.Empty;
        string TagStatisticsObject = string.Empty;
        int Tag_Width;
        int taskbarHeight;
        private CustomNotifier taskbarNotifier;
        int taskbarWidth;
        int tbarHeight;
        ObservableCollection<UserControl> TempTagCollection = new ObservableCollection<UserControl>();
        int TotalTagsExceeded;
        DispatcherTimer UnPausedStatTimer = new DispatcherTimer();
        TagGadgetControl userTagControl;
        int windowheight;
        int WindowHorizontalHeight;
        int WindowHorizontalWidth;
        int WindowVerticalHeight;
        int WindowVerticalWidth;
        int windowwidth;
        ObservableCollection<UserControl> Wrap2Collection = new ObservableCollection<UserControl>();
        int Wrap2Rows = 0;
        int WrapHorizontalWidth;
        int WrapVerticalHeight;
        private double _appLeft = 0;
        private double _appTop = 0;
        private string _appType;
        private SolidColorBrush _BackgroundColor;
        private ContextMenu _BtnContextMenu;
        private double _gadgetHeight;
        private double _gadgetRowHeight;
        private string _GadgetTagValue;
        private double _gadgetWidth;
        private SolidColorBrush _gridBackgroundColor;
        private Thickness _gridColumnMargin;
        private int _gridColumnSpan;
        private double _gridStatHolderHeight;
        private double _gridStatisticsHeight;
        private int _gridStatObjColspan;
        private double _gridStatObjectsHeight;
        private double _gridTagButtonHeight;
        private VerticalAlignment _gridValeAlign;
        private int _gridValueHeight;
        private VerticalAlignment _gridVerticalAlign;
        private bool _IsTopMost;
        private bool _isTransparency;
        private int _LblMainStatName;
        private int _LblStatName;
        private Visibility _MainCtrlVisibility;
        private int _MainGridHeight;
        private SolidColorBrush _MainStatNameColor;
        private Thickness _mainStatNameMargin;
        private string _mainStatObject;
        private int _mainStatValueColSpan;
        private Thickness _MainStatValueMargin;
        private int _MainStatWidth;
        private int _mainTagButtonRowSpan;
        private Thickness _MenuButtonMargin;
        private ImageSource _MenuImg;
        private Visibility _MenuVisibility;
        private ObservableCollection<UserControl> _myControlCollection;
        private ObservableCollection<UserControl> _myMainControlCollection;
        private ObservableCollection<UserControl> _MyTagControlCollection2;
        private ObservableCollection<UserControl> _myTempControlCollection;
        private ImageSource _NextImg;
        private string _playpause;
        private ImageSource _PlayPauseImg;
        private string _PlayPauseTooltip;
        private ImageSource _PreviousImg;
        private Visibility _StackVisibility;
        private Thickness _statisticsMargin;
        private string _statName;
        private string _StatNameToolTip;
        private string _StatRefId;
        private string _StatToolTip;
        private string _StatType;
        private string _StatTypeTooltip;
        private double _StatTypeWidth;
        private Thickness _statValMargin;
        private string _statValue;
        private SolidColorBrush _StatValueColor;
        private FontWeight _StatWeight;
        private Thickness _TagButtonMargin;
        private double _tagGadgetHeight;
        private Thickness _tagMargin;
        private string _TagStatName;
        private Thickness _tagStatObject;
        private Thickness _tagStatObjMargin;
        private Visibility _tagStatvaleVisible;
        private string _TagStatValue;
        private Thickness _TagStatValueMargin;
        private int _TagStatWidth;
        private string _TagValue;
        private Visibility _TagVisibility;
        private int _TagWidth;
        private SolidColorBrush _ThemeColor;
        private int _TotalGridHeight;
        private Thickness _TraverseButtonMargin;
        private ICommand _UntagButtonCommand;
        private VerticalAlignment _untagGridAlign;
        private Thickness _unTagGridMargin;
        private Visibility _UnTagVisibility;
        private int _VBMainStatName;
        private int _VBStatName;
        private double _VBStatTypeWidth;
        private Thickness _VBStatValMargin;
        private int _Window_Height;
        private int _Window_Width;
        private WindowState _WinState;
        private int _Wrap2Height;
        private Thickness _Wrap2Margin;
        private Orientation _Wrap2Orientation;
        private Visibility _Wrap2Visibility;
        private int _Wrap2Width;
        private int _WrapHeight;
        private Thickness _WrapMargin;
        private int _WrapMaxHeight;
        private int _WrapMaxWidth;
        private Orientation _wrapOrientation;
        private int _WrapWidth;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindowViewModel" /> class.
        /// </summary>
        public MainWindowViewModel()
        {
            StreamResourceInfo sri;
            ProcessModule objCurrentModule;

            try
            {
                //System.Windows.Forms.MessageBox.Show("enter");
                logger.Debug("MainWindowViewModel : Constructor : Method Entry");

                //if (!Settings.GetInstance().IsPluginReaded)
                //{

                MyTagControlCollection = new ObservableCollection<UserControl>();
                MyTagControlTempCollection = new ObservableCollection<UserControl>();
                MyMainControlCollection = new ObservableCollection<UserControl>();
                MyTagControlCollection2 = new ObservableCollection<UserControl>();

                if (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString())
                    _StatDisplayMessage += StatTickerGadget__StatDisplayMessage;
                else if (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.DB.ToString())
                    _DBStatDisplayMessage += StatTickerGadget__StatDisplayMessage;
                else if (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString())
                {
                    _StatDisplayMessage += StatTickerGadget__StatDisplayMessage;
                    _DBStatDisplayMessage += StatTickerGadget__StatDisplayMessage;
                }

                Settings.GetInstance().GadgetContextMenu = new ContextMenu();

                //ResourceDictionary styleresource = new ResourceDictionary();
                //styleresource.Source = new Uri("pack://application:,,,/StatTickerFive;component/Resources/StyleResource.xaml");
                //Style mymenustyle = styleresource["CustomContextmenu"] as Style;

                //Settings.GetInstance().GadgetContextMenu.Style = mymenustyle;
                BtnContextMenu = Settings.GetInstance().GadgetContextMenu;

                Mediatimer.Interval = new TimeSpan(1);
                Mediatimer.Tick += new EventHandler(Mediatimer_Tick);

                PlayPauseTooltip = "Stop";
                StatDisplayTimer.Tag = "True";
                //if (objStatTicker.isAgentConfigFromLocalFileSystem())
                //{
                //    objStatTicker.SaveAgentConfigFromLocalFileSystem(objXmlReader.ReadAgentConfiguration());
                //}
                if (!objStatTicker.isGadgetClose)
                {
                    bool StatBold = false;
                    if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                    {
                        StatBold = objStatTicker.IsStatisticsBold();
                    }
                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
                    {
                        StatBold = Settings.GetInstance().IsStatBold;
                    }
                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                    {
                        if (StatisticsBase.GetInstance().isCMEAuthentication)
                        {
                            StatBold = objStatTicker.IsStatisticsBold();
                        }
                        else if (StatisticsBase.GetInstance().isDBAuthentication)
                        {
                            StatBold = Settings.GetInstance().IsStatBold;
                        }
                    }

                    IsTransparency = Settings.GetInstance().AllowTrans;

                    if (StatBold)
                        StatWeight = FontWeights.Bold;
                    else
                        StatWeight = FontWeights.Normal;

                    ScreenWidth = (int)SystemParameters.PrimaryScreenWidth;
                    WrapMaxWidth = (int)SystemParameters.PrimaryScreenWidth;
                    WrapMaxHeight = (int)SystemParameters.PrimaryScreenHeight;

                    IStatTicker statListener = this;
                    objStatTicker.Subscribe("StatTickerFive", statListener);

                    taskbarNotifier = new Views.CustomNotifier();
                    notifierPopup = new CustomAlertNotifier();

                    if (Settings.GetInstance().Theme == null || Settings.GetInstance().Theme == string.Empty || Settings.GetInstance().Theme == "")
                        Settings.GetInstance().Theme = ConfigurationManager.AppSettings.Get("Theme");

                    ThemeSelector();

                    if (StatisticsBase.GetInstance().isPlugin)
                    {
                        logger.Warn("MainWindowViewModel : Constructor : Is Plugin : True");
                        Result = objStatTicker.StartStatistics(Settings.GetInstance().UserName + "_STF", Settings.GetInstance().ApplicationName, Settings.GetInstance().AddpServerTimeOut, Settings.GetInstance().AddpClientTimeOut, true, false);
                    }
                    else
                    {
                        if (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString())
                            Result = objStatTicker.StartStatistics(Settings.GetInstance().UserName + "_STF", Settings.GetInstance().ApplicationName, Settings.GetInstance().AddpServerTimeOut, Settings.GetInstance().AddpClientTimeOut, true, false);
                        else if (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.DB.ToString())
                        {
                            //Result = objStatTicker.StartStatistics();

                            Settings.GetInstance().DictEnableDisableChannels = objStatTicker.GetEnableDisableChannels();
                            Settings.GetInstance().DictErrorValues = objStatTicker.GetErrorValues();
                        }
                        else if (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString())
                        {
                            if (Settings.GetInstance().IsCMEAuthenticated || !Settings.GetInstance().DefaultAuthentication.Contains(StatisticsEnum.StatSource.StatServer.ToString()))
                            {
                                ServerResult = objStatTicker.StartStatistics(Settings.GetInstance().UserName, Settings.GetInstance().ApplicationName, Settings.GetInstance().AddpServerTimeOut, Settings.GetInstance().AddpClientTimeOut, true, false);
                            }
                            if (Settings.GetInstance().IsDBAuthenticated || !Settings.GetInstance().DefaultAuthentication.Contains(StatisticsEnum.StatSource.DB.ToString()))
                            {
                                if (!Settings.GetInstance().IsDBAuthenticated)
                                {
                                    objXmlReader.ReadDBSettings();
                                }

                                objStatTicker.StoreDBValues(Settings.GetInstance().dbHost, Settings.GetInstance().dbPort, Settings.GetInstance().dbLoginQuery, Settings.GetInstance().dbType, Settings.GetInstance().dbName, Settings.GetInstance().dbUsername, Settings.GetInstance().dbPassword, Settings.GetInstance().dbSid, Settings.GetInstance().dbSname, Settings.GetInstance().dbSource);
                                //DBResult = objStatTicker.StartStatistics();
                            }

                            if (ServerResult || DBResult)
                                Result = true;

                            if (ServerResult && DBResult)
                            {
                                ServerColor = objStatTicker.GetServerColor();
                                DBColor = objStatTicker.GetDBColor();
                            }
                            else if (ServerResult && !DBResult)
                            {

                                if (Settings.GetInstance().Theme != null && Settings.GetInstance().Theme != string.Empty && Settings.GetInstance().Theme != "")
                                {
                                    switch (Settings.GetInstance().Theme)
                                    {
                                        case "Outlook8":
                                            ServerColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                        case "Yahoo":
                                            ServerColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                        case "Grey":
                                            ServerColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                        case "BB_Theme1":
                                            ServerColor = System.Drawing.ColorTranslator.FromHtml("#6082B6");
                                            break;
                                        default:
                                            ServerColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                    }
                                }
                            }
                            else if (!ServerResult && DBResult)
                            {
                                if (Settings.GetInstance().Theme != null && Settings.GetInstance().Theme != string.Empty && Settings.GetInstance().Theme != "")
                                {
                                    switch (Settings.GetInstance().Theme)
                                    {
                                        case "Outlook8":
                                            DBColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                        case "Yahoo":
                                            DBColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                        case "Grey":
                                            DBColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                        case "BB_Theme1":
                                            DBColor = System.Drawing.ColorTranslator.FromHtml("#6082B6");
                                            break;
                                        default:
                                            DBColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                if (Settings.GetInstance().Theme != null && Settings.GetInstance().Theme != string.Empty && Settings.GetInstance().Theme != "")
                                {
                                    switch (Settings.GetInstance().Theme)
                                    {
                                        case "Outlook8":
                                            ServerColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            DBColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                        case "Yahoo":
                                            ServerColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            DBColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                        case "Grey":
                                            ServerColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            DBColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                        case "BB_Theme1":
                                            ServerColor = System.Drawing.ColorTranslator.FromHtml("#6082B6");
                                            DBColor = System.Drawing.ColorTranslator.FromHtml("#6082B6");
                                            break;
                                        default:
                                            ServerColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            DBColor = System.Drawing.ColorTranslator.FromHtml("#FFFFFF");
                                            break;
                                    }
                                }
                            }
                        }
                    }

                    if (Result)
                    {

                        logger.Warn("MainWindowViewModel : Constructor : Result : True");

                        string AppPosition = string.Empty;

                        if (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString() || ((Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString() && Settings.GetInstance().IsCMEAuthenticated)))
                        {
                            AppPosition = objStatTicker.GetAppPosition();

                            if (StatisticsBase.GetInstance().GadgetWidth <= Screen.PrimaryScreen.WorkingArea.Width)
                                GadgetWidth = StatisticsBase.GetInstance().GadgetWidth;
                            else
                                GadgetWidth = Screen.PrimaryScreen.WorkingArea.Width;
                        }
                        else if ((Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.DB.ToString()) || (!Settings.GetInstance().IsCMEAuthenticated))
                        {
                            AppPosition = Settings.GetInstance().Position;

                            if (Settings.GetInstance().Width == 0)
                                Settings.GetInstance().Width = 200;

                            if (Settings.GetInstance().Width <= Screen.PrimaryScreen.WorkingArea.Width)
                                GadgetWidth = Settings.GetInstance().Width;
                            else
                                GadgetWidth = Screen.PrimaryScreen.WorkingArea.Width;
                        }

                        if (AppPosition != string.Empty)
                        {
                            //string[] appPos = AppPosition.Split(',');
                            //if (appPos.Length > 1)
                            //{
                            //    if (appPos[0] != "")
                            //        AppLeft = Convert.ToDouble(appPos[0]);
                            //    if (appPos[1] != "")
                            //        AppTop = Convert.ToDouble(appPos[1]);
                            //}
                            //else
                            //{
                            //    if (appPos[0] != "")
                            //        AppLeft = Convert.ToDouble(appPos[0]);
                            //}

                            //code changed by arun on 16-08-2016 for gadget position exceeds screen size
                            string[] appPos = AppPosition.Split(',');
                            if (appPos.Length > 1)
                            {
                                if (appPos[0] != "")
                                {
                                    if (Convert.ToDouble(appPos[0]) < Screen.PrimaryScreen.WorkingArea.Width)
                                    {
                                        AppLeft = Convert.ToDouble(appPos[0]);
                                    }
                                    else
                                    {
                                        AppLeft = (Screen.PrimaryScreen.WorkingArea.Width - GadgetWidth);
                                    }
                                }

                                if (appPos[1] != "")
                                {
                                    if (Convert.ToDouble(appPos[1]) < Screen.PrimaryScreen.WorkingArea.Height)
                                    {
                                        AppTop = Convert.ToDouble(appPos[1]);
                                    }
                                    else
                                    {
                                        AppTop = (Screen.PrimaryScreen.WorkingArea.Height - 70);
                                    }
                                }

                            }
                            else
                            {
                                if (appPos[0] != "")
                                {
                                    if (Convert.ToDouble(appPos[0]) < Screen.PrimaryScreen.WorkingArea.Width)
                                    {
                                        AppLeft = Convert.ToDouble(appPos[0]);
                                    }
                                    else
                                    {
                                        AppLeft = (Screen.PrimaryScreen.WorkingArea.Width - GadgetWidth);
                                    }
                                }
                            }
                        }
                        else
                        {
                            AppLeft = 0;
                            AppTop = 0;
                        }

                        foreach (Window mainWin in Application.Current.Windows)
                        {
                            if (mainWin.Title == "StatGadget")
                            {
                                if (AppLeft != 0)
                                    mainWin.Left = AppLeft;
                                else
                                    mainWin.Left = 0;

                                if (AppTop != 0)
                                    mainWin.Top = AppTop;
                                else
                                    mainWin.Top = 0;
                            }
                        }

                        AddMenuItems();

                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("notificationclose"))
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels["notificationclose"])
                            {

                                logger.Warn("MainWindowViewModel : Constructor : NotificationClose : True");
                                MenuItem menu = new MenuItem();
                                menu.Text = "Close Gadget";
                                menu.Name = "cmenuClose";
                                menu.Click += NotifyMenuSelect;
                                notifyContext.MenuItems.Add(menu);
                            }
                            else
                            {
                                logger.Warn("MainWindowViewModel : Constructor : NotificationClose : False");
                            }
                        }
                        else
                        {
                            logger.Warn("MainWindowViewModel : Constructor : NotificationClose : False");
                        }

                        userControl = new Control1();

                        userControl.Tag = "tag_" + TagNo;
                        userControl.DataContext = this;
                        GridBackgroundColor = BackgroundColor;

                        StackVisibility = Visibility.Hidden;

                        logger.Warn("MainWindowViewModel : Constructor : isStatConfigured : True");

                        sri = Application.GetResourceStream(new Uri("pack://application:,,,/StatTickerFive;component/Images/StatTickerFive-32x32-01.ico"));

                        //if (!StatisticsBase.GetInstance().isPlugin)
                        //{
                        //    if (sri != null)
                        //    {
                        //        objNotify = new NotifyIcon()
                        //        {
                        //            Icon = new Icon(sri.Stream),
                        //            ContextMenu = notifyContext,
                        //            Visible = true
                        //        };
                        //    }
                        //}
                        //else
                        //{
                        //    if (sri != null)
                        //    {
                        //        objNotify = new NotifyIcon()
                        //        {
                        //            Icon = new Icon(sri.Stream),
                        //            ContextMenu = notifyContext,
                        //            Visible = false
                        //        };
                        //    }
                        //}

                        if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                            Settings.GetInstance().DisplayTime = objStatTicker.GetDisplayTime();

                        //objNotify.ShowBalloonTip(500, Settings.GetInstance().ApplicationName, "Statistics", ToolTipIcon.Info);
                        //objNotify.DoubleClick += objNotifyDoubleClick;

                        MyMainControlCollection.Add(userControl);

                        objStatTicker.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Opened);// Added to notification gadget window activated state to AID

                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                            {

                                logger.Warn("MainWindowViewModel : Constructor : MainGadget : True");
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton"))
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels["menubutton"])
                                    {
                                        logger.Warn("MainWindowViewModel : Constructor : Menubutton : True");
                                        MenuVisibility = Visibility.Visible;
                                        MainCtrlVisibility = Visibility.Visible;
                                        MainGridHeight = (int)GadgetHeight;
                                        StatTypeWidth = GadgetWidth - (16 * 4);
                                    }
                                    else
                                    {
                                        logger.Warn("MainWindowViewModel : Constructor : Menubutton : False");
                                        MenuVisibility = Visibility.Hidden;
                                        MainCtrlVisibility = Visibility.Visible;
                                        TraverseButtonMargin = new Thickness(16, 0, 0, 0);
                                        StatTypeWidth = GadgetWidth - (16 * 3);
                                    }
                                }
                            }
                            else
                            {
                                logger.Warn("MainWindowViewModel : Constructor : MainGadget : False");
                                MenuVisibility = Visibility.Hidden;
                                MainCtrlVisibility = Visibility.Hidden;
                            }
                        }

                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels["tagbutton"])
                            {
                                logger.Warn("MainWindowViewModel : Constructor : TagButton : True");
                                TagVisibility = Visibility.Visible;
                            }
                            else
                            {
                                logger.Warn("MainWindowViewModel : Constructor : TagButton : False");
                                TagVisibility = Visibility.Hidden;
                            }
                        }

                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("AlwaysOnTop"))
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels["AlwaysOnTop"])
                            {
                                IsTopMost = true;
                                logger.Warn("MainWindowViewModel : Constructor : AlwaysOnTop : True");
                            }
                            else
                            {
                                IsTopMost = false;
                                logger.Warn("MainWindowViewModel : Constructor : AlwaysOnTop : False");
                            }
                        }

                        StatTickerAddTaggedStats();
                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                            {
                                NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth) - 1;
                                NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
                            }
                            else
                            {
                                NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth);
                                NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
                            }
                        }

                        tbarHeight = (int)SystemParameters.PrimaryScreenHeight - (int)SystemParameters.WorkArea.Height;
                        growlNotifications.Height = SystemParameters.PrimaryScreenHeight;
                        growlNotifications.Top = SystemParameters.WorkArea.Top - tbarHeight;
                        growlNotifications.Left = 1800;
                        growlNotifications.NotifierClosed += new GrowlNotifications.NotificationClose(growlNotifications_NotifierClosed);
                        growlNotifications.MaximumHeight = (int)SystemParameters.WorkArea.Height;
                        growlNotifications.MAX_NOTIFICATIONS = (int)SystemParameters.WorkArea.Height;

                        growlNotifications.SetDisplayTime(StatisticsBase.GetInstance().AlertNotifyTime);

                        PlayPause = "Play";

                        Settings.GetInstance().DictAllStats = objStatTicker.GetAllStats();

                        foreach (Window window in Application.Current.Windows)
                        {
                            if (window.Title == "StatGadget")
                            {
                                var screen = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(window).Handle);
                                string Dilimitor = "\\";
                                PrimaryScreen = screen.DeviceName.Split(new[] { Dilimitor }, StringSplitOptions.None);
                                PrimaryScreenWidth = screen.Bounds.Width;
                                PrimaryScreenHeight = screen.Bounds.Height;
                            }
                        }

                        foreach (var screen in Screen.AllScreens)
                        {
                            string Dilimitor = "\\";
                            string[] tempArray = screen.DeviceName.Split(new[] { Dilimitor }, StringSplitOptions.None);

                            if (tempArray[3].ToString() == PrimaryScreen[3].ToString())
                            {
                                SecondaryScreenHeight = screen.Bounds.Size.Height;
                                SecondaryScreenWidth = screen.Bounds.Size.Width;
                                DeviceName = screen.DeviceName.Split(new[] { Dilimitor }, StringSplitOptions.None);
                            }
                        }

                        SetNotifierProperties();

                        #region Hooking key

                        objCurrentModule = Process.GetCurrentProcess().MainModule; //Get Current Module
                        objKeyboardProcess = new LowLevelKeyboardProc(captureKey); //Assign callback function each time keyboard process
                        ptrHook = SetWindowsHookEx(13, objKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0); //Setting Hook of Keyboard Process for current module

                        #endregion

                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                }
                //}
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : Constructor : " + ex.Message);
            }
            finally
            {
                sri = null;
                objCurrentModule = null;
                GC.Collect();
                logger.Debug("MainWindowViewModel : Constructor : Method Exit");
            }
        }

        #endregion Constructors

        #region Delegates

        //System level functions to be used for hook and unhook keyboard input
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        delegate void DBStatisticDisplayMessage(string refId, string statName, string statValue, string toolTip, Color statColor, bool isThresholdBreach, string dbStatName, bool isLevelTwo);

        delegate void StatisticDisplayMessage(string refId, string statName, string statValue, string toolTip, Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo);

        #endregion Delegates

        #region Events

        static event DBStatisticDisplayMessage _DBStatDisplayMessage;

        /// <summary>
        /// Occurs when [_ stat display message].
        /// </summary>
        static event StatisticDisplayMessage _StatDisplayMessage;

        #endregion Events

        #region Properties

        public double AppLeft
        {
            get
            {
                return _appLeft;
            }
            set
            {
                _appLeft = value;
                RaisePropertyChanged(() => AppLeft);
            }
        }

        public double AppTop
        {
            get
            {
                return _appTop;
            }
            set
            {
                _appTop = value;
                RaisePropertyChanged(() => AppTop);
            }
        }

        /// <summary>
        /// Gets or sets the tag value.
        /// </summary>
        /// <value>The tag value.</value>
        public string AppType
        {
            get
            {
                return _appType;
            }
            set
            {
                if (value != null)
                {
                    _appType = value;
                    RaisePropertyChanged(() => AppType);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public SolidColorBrush BackgroundColor
        {
            get
            {
                return _BackgroundColor;
            }
            set
            {
                _BackgroundColor = value;
                RaisePropertyChanged(() => BackgroundColor);
            }
        }

        /// <summary>
        /// Gets or sets the BTN context menu.
        /// </summary>
        /// <value>The BTN context menu.</value>
        public ContextMenu BtnContextMenu
        {
            get
            {
                return _BtnContextMenu;
            }
            set
            {
                if (value != null)
                {
                    _BtnContextMenu = value;
                    RaisePropertyChanged(() => BtnContextMenu);
                }
            }
        }

        /// <summary>
        /// Gets the BTN next.
        /// </summary>
        /// <value>The BTN next.</value>
        public ICommand btnNext
        {
            get { return new DelegateCommand(NextClick); }
        }

        /// <summary>
        /// Gets the BTN pause.
        /// </summary>
        /// <value>The BTN pause.</value>
        public ICommand btnPause
        {
            get { return new DelegateCommand(PlayPauseClick); }
        }

        /// <summary>
        /// Gets the BTN previous.
        /// </summary>
        /// <value>The BTN previous.</value>
        public ICommand btnPrevious
        {
            get { return new DelegateCommand(PreviousClick); }
        }

        public ICommand DragCommand
        {
            get { return new DelegateCommand(DragMove); }
        }

        public double GadgetHeight
        {
            get
            {
                return _gadgetHeight;
            }
            set
            {
                _gadgetHeight = value;
                RaisePropertyChanged(() => GadgetHeight);
            }
        }

        /// <summary>
        /// Gets or sets the gadget tag value.
        /// </summary>
        /// <value>The gadget tag value.</value>
        public string GadgetTagValue
        {
            get
            {
                return _GadgetTagValue;
            }
            set
            {
                if (value != null)
                {
                    _GadgetTagValue = value;
                    RaisePropertyChanged(() => GadgetTagValue);
                    UntagButtonCommand = new DelegateCommand(UnTagGadget);
                }
            }
        }

        public double GadgetWidth
        {
            get
            {
                return _gadgetWidth;
            }
            set
            {
                _gadgetWidth = value;
                RaisePropertyChanged(() => GadgetWidth);
            }
        }

        /// <summary>
        /// Gets or sets the color of the background.
        /// </summary>
        /// <value>The color of the background.</value>
        public SolidColorBrush GridBackgroundColor
        {
            get
            {
                return _gridBackgroundColor;
            }
            set
            {
                _gridBackgroundColor = value;
                RaisePropertyChanged(() => GridBackgroundColor);
            }
        }

        public Thickness GridColumnMargin
        {
            get
            {
                return _gridColumnMargin;
            }
            set
            {
                _gridColumnMargin = value;
                RaisePropertyChanged(() => GridColumnMargin);
            }
        }

        public int GridColumnSpan
        {
            get
            {
                return _gridColumnSpan;
            }
            set
            {
                _gridColumnSpan = value;
                RaisePropertyChanged(() => GridColumnSpan);
            }
        }

        public double GridRowHeight
        {
            get
            {
                return _gadgetRowHeight;
            }
            set
            {
                _gadgetRowHeight = value;
                RaisePropertyChanged(() => GridRowHeight);
            }
        }

        public double GridStatHolderHeight
        {
            get
            {
                return _gridStatHolderHeight;
            }
            set
            {
                _gridStatHolderHeight = value;
                RaisePropertyChanged(() => GridStatHolderHeight);
            }
        }

        public double GridStatisticsHeight
        {
            get
            {
                return _gridStatisticsHeight;
            }
            set
            {
                _gridStatisticsHeight = value;
                RaisePropertyChanged(() => GridStatisticsHeight);
            }
        }

        public int GridStatObjColspan
        {
            get
            {
                return _gridStatObjColspan;
            }
            set
            {
                _gridStatObjColspan = value;
                RaisePropertyChanged(() => GridStatObjColspan);
            }
        }

        public double GridStatObjectsHeight
        {
            get
            {
                return _gridStatObjectsHeight;
            }
            set
            {
                _gridStatObjectsHeight = value;
                RaisePropertyChanged(() => GridStatObjectsHeight);
            }
        }

        public double GridTagButtonHeight
        {
            get
            {
                return _gridTagButtonHeight;
            }
            set
            {
                _gridTagButtonHeight = value;
                RaisePropertyChanged(() => GridTagButtonHeight);
            }
        }

        public VerticalAlignment GridValeAlign
        {
            get
            {
                return _gridValeAlign;
            }
            set
            {
                _gridValeAlign = value;
                RaisePropertyChanged(() => GridValeAlign);
            }
        }

        public int GridValueHeight
        {
            get
            {
                return _gridValueHeight;
            }
            set
            {
                _gridValueHeight = value;
                RaisePropertyChanged(() => GridValueHeight);
            }
        }

        public VerticalAlignment GridVerticalAlign
        {
            get
            {
                return _gridVerticalAlign;
            }
            set
            {
                _gridVerticalAlign = value;
                RaisePropertyChanged(() => GridVerticalAlign);
            }
        }

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
                return _IsTopMost;
            }
            set
            {
                _IsTopMost = value;
                RaisePropertyChanged(() => IsTopMost);
            }
        }

        public bool IsTransparency
        {
            get
            {
                return _isTransparency;
            }
            set
            {
                _isTransparency = value;
                RaisePropertyChanged(() => IsTransparency);
            }
        }

        /// <summary>
        /// Gets or sets the name of the LBL main stat.
        /// </summary>
        /// <value>The name of the LBL main stat.</value>
        public int LblMainStatName
        {
            get
            {
                return _LblMainStatName;
            }
            set
            {
                _LblMainStatName = value;
                RaisePropertyChanged(() => LblMainStatName);
            }
        }

        /// <summary>
        /// Gets or sets the name of the LBL stat.
        /// </summary>
        /// <value>The name of the LBL stat.</value>
        public int LblStatName
        {
            get
            {
                return _LblStatName;
            }
            set
            {
                _LblStatName = value;
                RaisePropertyChanged(() => LblStatName);
            }
        }

        /// <summary>
        /// Gets or sets the main CTRL visibility.
        /// </summary>
        /// <value>The main CTRL visibility.</value>
        public Visibility MainCtrlVisibility
        {
            get
            {
                return _MainCtrlVisibility;
            }
            set
            {
                _MainCtrlVisibility = value;
                RaisePropertyChanged(() => MainCtrlVisibility);
            }
        }

        /// <summary>
        /// Gets or sets the width of the wrap.
        /// </summary>
        /// <value>The width of the wrap.</value>
        public int MainGridHeight
        {
            get
            {
                return _MainGridHeight;
            }
            set
            {
                _MainGridHeight = value;
                RaisePropertyChanged(() => MainGridHeight);
            }
        }

        /// <summary>
        /// Gets or sets the color of the main stat name.
        /// </summary>
        /// <value>The color of the main stat name.</value>
        public SolidColorBrush MainStatNameColor
        {
            get
            {
                return _MainStatNameColor;
            }
            set
            {
                _MainStatNameColor = value;
                RaisePropertyChanged(() => MainStatNameColor);
            }
        }

        public Thickness MainStatNameMargin
        {
            get
            {
                return _mainStatNameMargin;
            }
            set
            {
                _mainStatNameMargin = value;
                RaisePropertyChanged(() => MainStatNameMargin);
            }
        }

        /// <summary>
        /// Gets or sets the name of the stat.
        /// </summary>
        /// <value>The name of the stat.</value>
        public string MainStatObject
        {
            get
            {
                return _mainStatObject;
            }
            set
            {
                if (value != null)
                {
                    _mainStatObject = value;
                    RaisePropertyChanged(() => MainStatObject);
                }
            }
        }

        public int MainStatValueColSpan
        {
            get
            {
                return _mainStatValueColSpan;
            }
            set
            {
                _mainStatValueColSpan = value;
                RaisePropertyChanged(() => MainStatValueColSpan);
            }
        }

        /// <summary>
        /// Gets or sets the VB stat value.
        /// </summary>
        /// <value>The VB stat value.</value>
        public Thickness MainStatValueMargin
        {
            get
            {
                return _MainStatValueMargin;
            }
            set
            {
                _MainStatValueMargin = value;
                RaisePropertyChanged(() => MainStatValueMargin);
            }
        }

        /// <summary>
        /// Gets or sets the width of the main stat.
        /// </summary>
        /// <value>The width of the main stat.</value>
        public int MainStatWidth
        {
            get
            {
                return _MainStatWidth;
            }
            set
            {
                _MainStatWidth = value;
                RaisePropertyChanged(() => MainStatWidth);
            }
        }

        public int MainTagButtonRowSpan
        {
            get
            {
                return _mainTagButtonRowSpan;
            }
            set
            {
                _mainTagButtonRowSpan = value;
                RaisePropertyChanged(() => MainTagButtonRowSpan);
            }
        }

        /// <summary>
        /// Gets or sets the menu button margin.
        /// </summary>
        /// <value>The menu button margin.</value>
        public Thickness MenuButtonMargin
        {
            get
            {
                return _MenuButtonMargin;
            }
            set
            {
                if (value != null)
                {
                    _MenuButtonMargin = value;
                    RaisePropertyChanged(() => MenuButtonMargin);
                }
            }
        }

        /// <summary>
        /// Gets the menu click.
        /// </summary>
        /// <value>The menu click.</value>
        public ICommand MenuClick
        {
            get { return new DelegateCommand(MenuBtnClick); }
        }

        /// <summary>
        /// Gets or sets the menu img.
        /// </summary>
        /// <value>The menu img.</value>
        public ImageSource MenuImg
        {
            get
            {
                return _MenuImg;
            }
            set
            {
                if (value != null)
                {
                    _MenuImg = value;
                    RaisePropertyChanged(() => MenuImg);
                }
            }
        }

        /// <summary>
        /// Gets or sets the menu visibility.
        /// </summary>
        /// <value>The menu visibility.</value>
        public Visibility MenuVisibility
        {
            get
            {
                return _MenuVisibility;
            }
            set
            {
                _MenuVisibility = value;
                RaisePropertyChanged(() => MenuVisibility);
            }
        }

        /// <summary>
        /// Gets or sets my main control collection.
        /// </summary>
        /// <value>My main control collection.</value>
        public ObservableCollection<UserControl> MyMainControlCollection
        {
            get
            {
                return _myMainControlCollection;
            }
            set
            {
                _myMainControlCollection = value;
                RaisePropertyChanged(() => MyMainControlCollection);
            }
        }

        /// <summary>
        /// Gets or sets my tag control collection.
        /// </summary>
        /// <value>My tag control collection.</value>
        public ObservableCollection<UserControl> MyTagControlCollection
        {
            get
            {
                return _myControlCollection;
            }
            set
            {
                _myControlCollection = value;
                RaisePropertyChanged(() => MyTagControlCollection);
            }
        }

        /// <summary>
        /// Gets or sets my tag control collection.
        /// </summary>
        /// <value>My tag control collection.</value>
        public ObservableCollection<UserControl> MyTagControlCollection2
        {
            get
            {
                return _MyTagControlCollection2;
            }
            set
            {
                _MyTagControlCollection2 = value;
                RaisePropertyChanged(() => MyTagControlCollection2);
            }
        }

        /// <summary>
        /// Gets or sets my tag control temp collection.
        /// </summary>
        /// <value>My tag control temp collection.</value>
        public ObservableCollection<UserControl> MyTagControlTempCollection
        {
            get
            {
                return _myTempControlCollection;
            }
            set
            {
                _myTempControlCollection = value;
                RaisePropertyChanged(() => MyTagControlTempCollection);
            }
        }

        /// <summary>
        /// Gets or sets the next img.
        /// </summary>
        /// <value>The next img.</value>
        public ImageSource NextImg
        {
            get
            {
                return _NextImg;
            }
            set
            {
                if (value != null)
                {
                    _NextImg = value;
                    RaisePropertyChanged(() => NextImg);
                }
            }
        }

        public CustomAlertNotifier NotifierPopup
        {
            get { return notifierPopup; }
        }

        /// <summary>
        /// Gets or sets the play pause.
        /// </summary>
        /// <value>The play pause.</value>
        public string PlayPause
        {
            get
            {
                return _playpause;
            }
            set
            {
                if (value != null)
                {
                    _playpause = value;
                    RaisePropertyChanged(() => PlayPause);
                }
            }
        }

        /// <summary>
        /// Gets or sets the play pause img.
        /// </summary>
        /// <value>The play pause img.</value>
        public ImageSource PlayPauseImg
        {
            get
            {
                return _PlayPauseImg;
            }
            set
            {
                if (value != null)
                {
                    _PlayPauseImg = value;
                    RaisePropertyChanged(() => PlayPauseImg);
                }
            }
        }

        /// <summary>
        /// Gets or sets the play pause tooltip.
        /// </summary>
        /// <value>The play pause tooltip.</value>
        public string PlayPauseTooltip
        {
            get
            {
                return _PlayPauseTooltip;
            }
            set
            {
                if (value != null)
                {
                    _PlayPauseTooltip = value;
                    RaisePropertyChanged(() => PlayPauseTooltip);
                }
            }
        }

        /// <summary>
        /// Gets or sets the previous img.
        /// </summary>
        /// <value>The previous img.</value>
        public ImageSource PreviousImg
        {
            get
            {
                return _PreviousImg;
            }
            set
            {
                if (value != null)
                {
                    _PreviousImg = value;
                    RaisePropertyChanged(() => PreviousImg);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stack visibility.
        /// </summary>
        /// <value>The stack visibility.</value>
        public Visibility StackVisibility
        {
            get
            {
                return _StackVisibility;
            }
            set
            {
                _StackVisibility = value;
                RaisePropertyChanged(() => StackVisibility);
            }
        }

        public Thickness StatisticsMargin
        {
            get
            {
                return _statisticsMargin;
            }
            set
            {
                _statisticsMargin = value;
                RaisePropertyChanged(() => StatisticsMargin);
            }
        }

        /// <summary>
        /// Gets or sets the name of the stat.
        /// </summary>
        /// <value>The name of the stat.</value>
        public string StatName
        {
            get
            {
                return _statName;
            }
            set
            {
                if (value != null)
                {
                    _statName = value;
                    RaisePropertyChanged(() => StatName);
                }
            }
        }

        /// <summary>
        /// Gets or sets the stat name tool tip.
        /// </summary>
        /// <value>The stat name tool tip.</value>
        public string StatNameToolTip
        {
            get
            {
                return _StatNameToolTip;
            }
            set
            {
                if (value != null)
                {
                    _StatNameToolTip = value;
                    RaisePropertyChanged(() => StatNameToolTip);
                }
            }
        }

        /// <summary>
        /// Gets or sets the gadget tag value.
        /// </summary>
        /// <value>The gadget tag value.</value>
        public string StatRefId
        {
            get
            {
                return _StatRefId;
            }
            set
            {
                if (value != null)
                {
                    _StatRefId = value;
                    RaisePropertyChanged(() => StatRefId);
                    UntagButtonCommand = new DelegateCommand(UnTagGadget);
                }
            }
        }

        /// <summary>
        /// Gets or sets the gadget tag value.
        /// </summary>
        /// <value>The gadget tag value.</value>
        public string StatToolTip
        {
            get
            {
                return _StatToolTip;
            }
            set
            {
                if (value != null)
                {
                    _StatToolTip = value;
                    RaisePropertyChanged(() => StatToolTip);
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the stat.
        /// </summary>
        /// <value>The type of the stat.</value>
        public string StatType
        {
            get
            {
                return _StatType;
            }
            set
            {
                if (value != null)
                {
                    _StatType = value;
                    RaisePropertyChanged(() => StatType);
                }

            }
        }

        /// <summary>
        /// Gets or sets the stat type tooltip.
        /// </summary>
        /// <value>The stat type tooltip.</value>
        public string StatTypeTooltip
        {
            get
            {
                return _StatTypeTooltip;
            }
            set
            {
                if (value != null)
                {
                    _StatTypeTooltip = value;
                    RaisePropertyChanged(() => StatTypeTooltip);
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the stat type.
        /// </summary>
        /// <value>The width of the stat type.</value>
        public double StatTypeWidth
        {
            get
            {
                return _StatTypeWidth;
            }
            set
            {
                _StatTypeWidth = value;
                RaisePropertyChanged(() => StatTypeWidth);
            }
        }

        public Thickness StatValMargin
        {
            get
            {
                return _statValMargin;
            }
            set
            {
                _statValMargin = value;
                RaisePropertyChanged(() => StatValMargin);
            }
        }

        /// <summary>
        /// Gets or sets the stat value.
        /// </summary>
        /// <value>The stat value.</value>
        public string StatValue
        {
            get
            {
                return _statValue;
            }
            set
            {
                if (value != null)
                {
                    _statValue = value;
                    RaisePropertyChanged(() => StatValue);
                }
            }
        }

        /// <summary>
        /// Gets or sets the color of the stat value.
        /// </summary>
        /// <value>The color of the stat value.</value>
        public SolidColorBrush StatValueColor
        {
            get
            {
                return _StatValueColor;
            }
            set
            {
                if (value != null)
                {
                    _StatValueColor = value;
                    RaisePropertyChanged(() => StatValueColor);
                }
            }
        }

        public FontWeight StatWeight
        {
            get
            {
                return _StatWeight;
            }
            set
            {
                if (value != null)
                {
                    _StatWeight = value;
                    RaisePropertyChanged(() => StatWeight);
                }
            }
        }

        /// <summary>
        /// Gets or sets the tag button margin.
        /// </summary>
        /// <value>The tag button margin.</value>
        public Thickness TagButtonMargin
        {
            get
            {
                return _TagButtonMargin;
            }
            set
            {
                _TagButtonMargin = value;
                RaisePropertyChanged(() => TagButtonMargin);
            }
        }

        /// <summary>
        /// Gets the tag click.
        /// </summary>
        /// <value>The tag click.</value>
        public ICommand TagClick
        {
            get { return new DelegateCommand(AddTag); }
        }

        public double TagGadgetHeight
        {
            get
            {
                return _tagGadgetHeight;
            }
            set
            {
                _tagGadgetHeight = value;
                RaisePropertyChanged(() => TagGadgetHeight);
            }
        }

        /// <summary>
        /// Gets or sets the wrap margin.
        /// </summary>
        /// <value>The wrap margin.</value>
        public Thickness TagMargin
        {
            get
            {
                return _tagMargin;
            }
            set
            {
                if (value != null)
                {
                    _tagMargin = value;
                    RaisePropertyChanged(() => TagMargin);
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the tag stat.
        /// </summary>
        /// <value>The name of the tag stat.</value>
        public string TagStatName
        {
            get
            {
                return _TagStatName;
            }
            set
            {
                _TagStatName = value;
                RaisePropertyChanged(() => TagStatName);
            }
        }

        public Thickness TagStatObject
        {
            get
            {
                return _tagStatObject;
            }
            set
            {
                _tagStatObject = value;
                RaisePropertyChanged(() => TagStatObject);
            }
        }

        public Thickness TagStatObjMargin
        {
            get
            {
                return _tagStatObjMargin;
            }
            set
            {
                _tagStatObjMargin = value;
                RaisePropertyChanged(() => TagStatObjMargin);
            }
        }

        public Visibility TagStatvaleVisible
        {
            get
            {
                return _tagStatvaleVisible;
            }
            set
            {
                _tagStatvaleVisible = value;
                RaisePropertyChanged(() => TagStatvaleVisible);
            }
        }

        /// <summary>
        /// Gets or sets the tag stat value.
        /// </summary>
        /// <value>The tag stat value.</value>
        public string TagStatValue
        {
            get
            {
                return _TagStatValue;
            }
            set
            {
                _TagStatValue = value;
                RaisePropertyChanged(() => TagStatValue);
            }
        }

        /// <summary>
        /// Gets or sets the wrap margin.
        /// </summary>
        /// <value>The wrap margin.</value>
        public Thickness TagStatValueMargin
        {
            get
            {
                return _TagStatValueMargin;
            }
            set
            {
                if (value != null)
                {
                    _TagStatValueMargin = value;
                    RaisePropertyChanged(() => TagStatValueMargin);
                }
            }
        }

        /// <summary>
        /// Gets or sets the width of the tag stat.
        /// </summary>
        /// <value>The width of the tag stat.</value>
        public int TagStatWidth
        {
            get
            {
                return _TagStatWidth;
            }
            set
            {
                _TagStatWidth = value;
                RaisePropertyChanged(() => TagStatWidth);
            }
        }

        /// <summary>
        /// Gets or sets the tag value.
        /// </summary>
        /// <value>The tag value.</value>
        public string TagValue
        {
            get
            {
                return _TagValue;
            }
            set
            {
                if (value != null)
                {
                    _TagValue = value;
                    RaisePropertyChanged(() => TagValue);
                }
            }
        }

        /// <summary>
        /// Gets or sets the tag visibility.
        /// </summary>
        /// <value>The tag visibility.</value>
        public Visibility TagVisibility
        {
            get
            {
                return _TagVisibility;
            }
            set
            {
                _TagVisibility = value;
                RaisePropertyChanged(() => TagVisibility);
            }
        }

        /// <summary>
        /// Gets or sets the width of the tag.
        /// </summary>
        /// <value>The width of the tag.</value>
        public int TagWidth
        {
            get
            {
                return _TagWidth;
            }
            set
            {
                _TagWidth = value;
                RaisePropertyChanged(() => TagWidth);
            }
        }

        /// <summary>
        /// Gets the taskbar notifier.
        /// </summary>
        /// <value>The taskbar notifier.</value>
        public CustomNotifier TaskbarNotifier
        {
            get { return taskbarNotifier; }
        }

        /// <summary>
        /// Gets or sets the color of the theme.
        /// </summary>
        /// <value>The color of the theme.</value>
        public SolidColorBrush ThemeColor
        {
            get
            {
                return _ThemeColor;
            }
            set
            {
                _ThemeColor = value;
                RaisePropertyChanged(() => ThemeColor);
            }
        }

        public int TotalGridHeight
        {
            get
            {
                return _TotalGridHeight;
            }
            set
            {
                _TotalGridHeight = value;
                RaisePropertyChanged(() => TotalGridHeight);
            }
        }

        /// <summary>
        /// Gets or sets the traverse button margin.
        /// </summary>
        /// <value>The traverse button margin.</value>
        public Thickness TraverseButtonMargin
        {
            get
            {
                return _TraverseButtonMargin;
            }
            set
            {
                if (value != null)
                {
                    _TraverseButtonMargin = value;
                    RaisePropertyChanged(() => TraverseButtonMargin);
                }
            }
        }

        /// <summary>
        /// Gets or sets the untag button command.
        /// </summary>
        /// <value>The untag button command.</value>
        public ICommand UntagButtonCommand
        {
            get
            {
                return _UntagButtonCommand;
            }
            set
            {
                _UntagButtonCommand = value;
                RaisePropertyChanged(() => UntagButtonCommand);
            }
        }

        public VerticalAlignment UntagGridAlign
        {
            get
            {
                return _untagGridAlign;
            }
            set
            {
                _untagGridAlign = value;
                RaisePropertyChanged(() => UntagGridAlign);
            }
        }

        public Thickness UnTagGridMargin
        {
            get
            {
                return _unTagGridMargin;
            }
            set
            {
                _unTagGridMargin = value;
                RaisePropertyChanged(() => UnTagGridMargin);
            }
        }

        /// <summary>
        /// Gets or sets the un tag visibility.
        /// </summary>
        /// <value>The un tag visibility.</value>
        public Visibility UnTagVisibility
        {
            get
            {
                return _UnTagVisibility;
            }
            set
            {
                _UnTagVisibility = value;
                RaisePropertyChanged(() => UnTagVisibility);
            }
        }

        /// <summary>
        /// Gets or sets the name of the VB main stat.
        /// </summary>
        /// <value>The name of the VB main stat.</value>
        public int VBMainStatName
        {
            get
            {
                return _VBMainStatName;
            }
            set
            {
                _VBMainStatName = value;
                RaisePropertyChanged(() => VBMainStatName);
            }
        }

        /// <summary>
        /// Gets or sets the name of the VB stat.
        /// </summary>
        /// <value>The name of the VB stat.</value>
        public int VBStatName
        {
            get
            {
                return _VBStatName;
            }
            set
            {
                _VBStatName = value;
                RaisePropertyChanged(() => VBStatName);
            }
        }

        /// <summary>
        /// Gets or sets the width of the VB stat type.
        /// </summary>
        /// <value>The width of the VB stat type.</value>
        public double VBStatTypeWidth
        {
            get
            {
                return _VBStatTypeWidth;
            }
            set
            {
                _VBStatTypeWidth = value;
                RaisePropertyChanged(() => VBStatTypeWidth);
            }
        }

        /// <summary>
        /// Gets or sets the VB stat val margin.
        /// </summary>
        /// <value>The VB stat val margin.</value>
        public Thickness VBStatValMargin
        {
            get
            {
                return _VBStatValMargin;
            }
            set
            {
                _VBStatValMargin = value;
                RaisePropertyChanged(() => VBStatValMargin);
            }
        }

        public ICommand WinClosed
        {
            get { return new DelegateCommand(WindowClosed); }
        }

        public ICommand WinClosing
        {
            get { return new DelegateCommand(WindowClosing); }
        }

        public ICommand WinDoubleClicked
        {
            get { return new DelegateCommand(WinClicked); }
        }

        /// <summary>
        /// Gets or sets the height of the window_.
        /// </summary>
        /// <value>The height of the window_.</value>
        public int Window_Height
        {
            get
            {
                return _Window_Height;
            }
            set
            {
                _Window_Height = value;
                RaisePropertyChanged(() => Window_Height);
            }
        }

        /// <summary>
        /// Gets or sets the width of the window_.
        /// </summary>
        /// <value>The width of the window_.</value>
        public int Window_Width
        {
            get
            {
                return _Window_Width;
            }
            set
            {
                _Window_Width = value;
                RaisePropertyChanged(() => Window_Width);
            }
        }

        public ICommand WinLocationChanged
        {
            get { return new DelegateCommand(WinLocChange); }
        }

        public ICommand WinStatChanged
        {
            get { return new DelegateCommand(StateChanged); }
        }

        /// <summary>
        /// Gets or sets the state of the W in.
        /// </summary>
        /// <value>The state of the W in.</value>
        public WindowState WInState
        {
            get
            {
                return _WinState;
            }
            set
            {
                _WinState = value;
                RaisePropertyChanged(() => WInState);
            }
        }

        public int Wrap2Height
        {
            get
            {
                return _Wrap2Height;
            }
            set
            {
                _Wrap2Height = value;
                RaisePropertyChanged(() => Wrap2Height);
            }
        }

        public Thickness Wrap2Margin
        {
            get
            {
                return _Wrap2Margin;
            }
            set
            {
                _Wrap2Margin = value;
                RaisePropertyChanged(() => Wrap2Margin);
            }
        }

        /// <summary>
        /// Gets or sets the wrap orientation.
        /// </summary>
        /// <value>The wrap orientation.</value>
        public Orientation Wrap2Orientation
        {
            get
            {
                return _Wrap2Orientation;
            }
            set
            {
                _Wrap2Orientation = value;
                RaisePropertyChanged(() => Wrap2Orientation);
            }
        }

        public Visibility Wrap2Visibility
        {
            get
            {
                return _Wrap2Visibility;
            }
            set
            {
                _Wrap2Visibility = value;
                RaisePropertyChanged(() => Wrap2Visibility);
            }
        }

        public int Wrap2Width
        {
            get
            {
                return _Wrap2Width;
            }
            set
            {
                _Wrap2Width = value;
                RaisePropertyChanged(() => Wrap2Width);
            }
        }

        /// <summary>
        /// Gets or sets the height of the wrap.
        /// </summary>
        /// <value>The height of the wrap.</value>
        public int WrapHeight
        {
            get
            {
                return _WrapHeight;
            }
            set
            {
                _WrapHeight = value;
                RaisePropertyChanged(() => WrapHeight);
            }
        }

        /// <summary>
        /// Gets or sets the wrap margin.
        /// </summary>
        /// <value>The wrap margin.</value>
        public Thickness WrapMargin
        {
            get
            {
                return _WrapMargin;
            }
            set
            {
                if (value != null)
                {
                    _WrapMargin = value;
                    RaisePropertyChanged(() => WrapMargin);
                }
            }
        }

        /// <summary>
        /// Gets or sets the height of the wrap max.
        /// </summary>
        /// <value>The height of the wrap max.</value>
        public int WrapMaxHeight
        {
            get
            {
                return _WrapMaxHeight;
            }
            set
            {
                _WrapMaxHeight = value;
                RaisePropertyChanged(() => WrapMaxHeight);
            }
        }

        /// <summary>
        /// Gets or sets the width of the wrap max.
        /// </summary>
        /// <value>The width of the wrap max.</value>
        public int WrapMaxWidth
        {
            get
            {
                return _WrapMaxWidth;
            }
            set
            {
                _WrapMaxWidth = value;
                RaisePropertyChanged(() => WrapMaxWidth);
            }
        }

        /// <summary>
        /// Gets or sets the wrap orientation.
        /// </summary>
        /// <value>The wrap orientation.</value>
        public Orientation WrapOrientation
        {
            get
            {
                return _wrapOrientation;
            }
            set
            {
                _wrapOrientation = value;
                RaisePropertyChanged(() => WrapOrientation);
            }
        }

        /// <summary>
        /// Gets or sets the width of the wrap.
        /// </summary>
        /// <value>The width of the wrap.</value>
        public int WrapWidth
        {
            get
            {
                return _WrapWidth;
            }
            set
            {
                _WrapWidth = value;
                RaisePropertyChanged(() => WrapWidth);
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Alerts the notifier.
        /// </summary>
        /// <param name="statname">The statname.</param>
        /// <param name="notifysec">The notifysec.</param>
        /// <param name="RefId">The preference unique identifier.</param>
        /// <param name="Attempt">The attempt.</param>
        public void AlertNotifier(string statname, int notifysec, string RefId, int Attempt)
        {
            try
            {
                logger.Debug("MainWindowViewModel : Notifer : Method Entry");

                ThresholdNotiferViewModel tNotifyViewModel = new ThresholdNotiferViewModel("Custom", StatisticsBase.GetInstance().ThresholdTitleBackground, StatisticsBase.GetInstance().ThresholdContentForeground, StatisticsBase.GetInstance().ThresholdContentBackground, StatisticsBase.GetInstance().IsThresholdNotifierBold);

                notifierPopup.DataContext = tNotifyViewModel;

                tNotifyViewModel.NotificationContent = "Statistics value exceeded threshold value for " + statname;
                tNotifyViewModel.NotificationTitle = "Threshold Breach";

                notifierPopup.StayOpenMilliseconds = notifysec;
                notifierPopup.Notify(true, RefId, Attempt);
                notifierPopup.Show();
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : Notifier : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : Notifier : Method Exit");
            }
        }

        /// <summary>
        /// Displays the error message.
        /// </summary>
        /// <param name="error">The error.</param>
        public void DisplayErrorMessage(OutputValues output)
        {
            try
            {
                logger.Debug("MainWindowViewModel : DisplayErrorMessage : Method Entry");
                if (output.MessageCode == "2002")
                {
                    logger.Debug("MainWindowViewModel : DisplayErrorMessage : Output Messagecode :" + output.MessageCode);
                    logger.Debug("MainWindowViewModel : DisplayErrorMessage : Output Message :" + output.Message);
                    StatDisplayTimer.Stop();
                    isEnableMessageBox = false;
                    Views.MessageBox msgbox = new Views.MessageBox();
                    ViewModels.MessageBoxViewModel mboxvmodel = new MessageBoxViewModel("Information", output.Message.ToString(), msgbox, "LoginWindow", Settings.GetInstance().Theme);
                    msgbox.DataContext = mboxvmodel;
                    msgbox.ShowDialog();
                }
                else if (output.MessageCode == "2001" && isEnableMessageBox)
                {
                    logger.Debug("MainWindowViewModel : DisplayErrorMessage : Output Messagecode :" + output.MessageCode);
                    logger.Debug("MainWindowViewModel : DisplayErrorMessage : Output Message :" + output.Message);
                    StatDisplayTimer.Stop();
                    isEnableMessageBox = false;
                    Views.MessageBox msgbox = new Views.MessageBox();
                    ViewModels.MessageBoxViewModel mboxvmodel = new MessageBoxViewModel("Information", output.Message.ToString(), msgbox, "MainWindow", Settings.GetInstance().Theme);
                    msgbox.DataContext = mboxvmodel;
                    msgbox.ShowDialog();
                }
                else if (output.MessageCode == "2000")
                {
                    logger.Debug("MainWindowViewModel : DisplayErrorMessage : Output Messagecode :" + output.MessageCode);
                    logger.Debug("MainWindowViewModel : DisplayErrorMessage : Output Message :" + output.Message);
                    if (PlayPause == "Pause")
                    {
                        PausedStatTimer1.Start();
                    }
                    else
                    {
                        PausedStatTimer1.Stop();
                        StatDisplayTimer.Start();
                    }
                    isEnableMessageBox = true;
                }
            }
            catch (Exception ex)
            {
                //logger.Error("MainWindowViewModel : DisplayErrorMessage Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : DisplayErrorMessage : Method Exit");
            }
        }

        /// <summary>
        /// Dragwindows the initialize.
        /// </summary>
        /// <param name="MainWin">The main win.</param>
        public void dragwindowInitialize(Window MainWin)
        {
            WindowInteropHelper helper;
            HwndSource hwndSource;
            IntPtr windowHandle;
            IntPtr hmenu;

            try
            {
                #region Draggable

                helper = new WindowInteropHelper(MainWin);
                hwndSource = HwndSource.FromHwnd(helper.Handle);
                if (hwndSource != null) hwndSource.AddHook(HwndMessageHook);

                taskbarHeight = (int)SystemParameters.PrimaryScreenHeight - Screen.PrimaryScreen.WorkingArea.Height;
                taskbarWidth = (int)SystemParameters.PrimaryScreenWidth - Screen.PrimaryScreen.WorkingArea.Width;

                Main_Window = MainWin;

                primwidh = Screen.PrimaryScreen.WorkingArea.Width;

                priheight = Screen.PrimaryScreen.WorkingArea.Height;

                rightMargin = primwidh - MainWin.Width;
                bottomMargin = priheight - MainWin.Height;

                InitialWindowLocation = new Point(MainWin.Left, MainWin.Top);

                windowHandle = helper.Handle; //Get the handle of this window

                hmenu = GetSystemMenu(windowHandle, 0);

                int cnt = GetMenuItemCount(hmenu);
                //remove the button

                RemoveMenu(hmenu, cnt - 1, MF_DISABLED | MF_BYPOSITION);

                //remove the extra menu line

                RemoveMenu(hmenu, cnt - 2, MF_DISABLED | MF_BYPOSITION);
                RemoveMenu(hmenu, cnt - 3, MF_DISABLED | MF_BYPOSITION);
                RemoveMenu(hmenu, cnt - 4, MF_DISABLED | MF_BYPOSITION);
                RemoveMenu(hmenu, cnt - 5, MF_DISABLED | MF_BYPOSITION);
                RemoveMenu(hmenu, cnt - 6, MF_DISABLED | MF_BYPOSITION);
                RemoveMenu(hmenu, cnt - 7, MF_DISABLED | MF_BYPOSITION);

                #endregion
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : dragwindowInitialize : " + ex.Message);
            }
            finally
            {
                helper = null;
                hwndSource = null;
                GC.Collect();
            }
        }

        /// <summary>
        /// Gadgetcloses this instance.
        /// </summary>
        public void gadgetclose(bool isExitApp = true)
        {
            try
            {
                logger.Debug("MainWindowViewModel : GadgetClose : Method Entry");

                #region Close Gadget

                //if (isExitApp)
                //{

                logger.Debug("MainWindowViewModel : GadgetClose : Saving Statistics");
                isMinimized = false;
                int tagno = 1;
                //string statDisplayName = string.Empty;
                taggedStats = string.Empty;
                Dictionary<string, string> DictTaggedStats = new Dictionary<string, string>();
                string ServerTaggedStats = string.Empty;
                string DBTaggedStats = string.Empty;

                try
                {

                    output = new OutputValues();
                    if (hashTaggedStats.Count > 0)
                    {
                        SortedDictionary<int, string> tempdic = new SortedDictionary<int, string>();

                        if (MyTagControlCollection != null)
                        {
                            foreach (TagGadgetControl temptagcontrol in MyTagControlCollection)
                            {
                                foreach (DictionaryEntry entry in hashTaggedStats)
                                {
                                    if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
                                    {
                                        string[] keyvalues = entry.Key.ToString().Split('_');
                                        int tagn = Convert.ToInt32(keyvalues[1]);
                                        tempdic.Add(tagn, entry.Value.ToString());
                                    }
                                }
                            }
                        }

                        if (MyTagControlCollection2 != null)
                        {
                            foreach (TagGadgetControl temptagcontrol in MyTagControlCollection2)
                            {
                                foreach (DictionaryEntry entry in hashTaggedStats)
                                {
                                    if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
                                    {
                                        string[] keyvalues = entry.Key.ToString().Split('_');
                                        int tagn = Convert.ToInt32(keyvalues[1]);

                                        if (!tempdic.ContainsKey(tagn))
                                            tempdic.Add(tagn, entry.Value.ToString());
                                    }
                                }
                            }
                        }

                        foreach (string statName in tempdic.Values)
                        {
                            string Dilimitor = "_@";
                            string[] stat = statName.Split(new[] { Dilimitor }, StringSplitOptions.RemoveEmptyEntries);
                            string statobj = "";
                            if (stat.Length == 2)
                                statobj = (stat[1].Split('\n'))[0];
                            else
                            {
                                statobj = (stat[0].Split('@'))[0];
                                stat[0] = (stat[0].Split('@'))[1];
                            }


                            if (ServerTaggedStats.Equals(string.Empty))//&& statDisplayName.Equals(string.Empty))
                            {
                                if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                                {
                                    //statDisplayName = tagno + "_&" + statobj[1] + "@" + statobj[0];
                                    ServerTaggedStats = statobj + "@" + stat[0];
                                }
                                else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                {
                                    //statDisplayName = tagno + "_&" + statobj[1] + "@" + statobj[0];
                                    ServerTaggedStats = statobj + "@" + stat[0];
                                }
                            }
                            else
                            {
                                if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                                {
                                    //statDisplayName = statDisplayName + "," + tagno + "_&" + statobj[1] + "@" + statobj[0];
                                    ServerTaggedStats = ServerTaggedStats + "," + statobj + "@" + stat[0];
                                }
                                else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                {
                                    //statDisplayName = statDisplayName + "," + tagno + "_&" + statobj[1] + "@" + statobj[0];
                                    ServerTaggedStats = ServerTaggedStats + "," + statobj + "@" + stat[0];
                                }
                            }
                            tagno++;
                        }
                        if (!DictTaggedStats.ContainsKey(StatisticsEnum.StatSource.StatServer.ToString()))
                            DictTaggedStats.Add(StatisticsEnum.StatSource.StatServer.ToString(), ServerTaggedStats);

                        if (!DictTaggedStats.ContainsKey(StatisticsEnum.StatSource.DB.ToString()))
                            DictTaggedStats.Add(StatisticsEnum.StatSource.DB.ToString(), ServerTaggedStats);

                        logger.Debug("MainWindowViewModel : GadgetClose : collection : " + DictTaggedStats.Count.ToString());
                        output = objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);

                        // if commented for dont write statistics on stattickerfive.exe.config file -02/09/2014
                        // if (output.MessageCode == "2007")
                        //   SaveTaggedStatistics(statDisplayName);
                    }
                    else
                    {

                        logger.Debug("MainWindowViewModel : GadgetClose : collection is Empty");
                        objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);
                        // if commented for dont write statistics on stattickerfive.exe.config file -02/09/2014
                        // if (output.MessageCode == "2007")
                        //   SaveTaggedStatistics(string.Empty);
                    }
                    if (isExitApp)
                    {
                        //StatisticsBase.GetInstance().CloseStatistics();
                        StatisticsBase.GetInstance().CloseConfigServer();
                        Environment.Exit(0);
                    }
                    else
                        foreach (Window window in Application.Current.Windows)
                            if (window.Title == "StatGadget")
                                //window.Close();
                                window.Hide();
                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while saving the tagged stats as : " + ex.Message);
                    foreach (Window window in Application.Current.Windows)
                        if (window.Title == "StatGadget")
                            window.Hide();
                    //window.Close();
                }
                //}
                //else
                //{
                //    foreach (Window window in Application.Current.Windows)
                //        if (window.Title == "StatGadget")
                //            window.Hide();
                //}

                #endregion

                
                
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show("MainWindowViewModel : gadgetclose : " + ex.Message);
                logger.Error("MainWindowViewModel : gadgetclose : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : GadgetClose : Method Exit");
            }
        }

        /// <summary>
        /// Headers the values.
        /// </summary>
        public void HeaderValues()
        {
            try
            {

                if (isShowHeader)
                {
                    #region Show Header True

                    if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                    {
                        GadgetHeight = 70;
                    }
                    else
                    {
                        GadgetHeight = 0;
                    }

                    GridRowHeight = 43;
                    GridStatisticsHeight = 44;
                    TagGadgetHeight = 35;
                    GridStatObjectsHeight = 13;

                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                    {
                        if (Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
                        {
                            if (MyTagControlCollection2.Count != 0)
                            {
                                Wrap2Height = (int)TagGadgetHeight * (MyTagControlCollection2.Count);

                                if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                {
                                    MainGridHeight = (int)GadgetHeight;
                                    TotalGridHeight = MainGridHeight + Wrap2Height;
                                }
                                else
                                {
                                    MainGridHeight = 0;
                                    TotalGridHeight = Wrap2Height;
                                }

                            }
                            else
                            {
                                MainGridHeight = (int)GadgetHeight;
                                TotalGridHeight = MainGridHeight;
                            }

                        }
                        else
                        {

                            MainGridHeight = (int)GadgetHeight;

                            if (MyTagControlCollection.Count != 0)
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                {
                                    if (MyTagControlCollection.Count > NumberOfTags_Horizontally)
                                    {
                                        WrapHeight = (int)TagGadgetHeight * 2;
                                    }
                                    else
                                    {
                                        WrapHeight = (int)TagGadgetHeight;
                                    }
                                }
                                else
                                {
                                    if ((MyTagControlCollection.Count) % NumberOfTags_Horizontally == 0)
                                        WrapHeight = (int)TagGadgetHeight * (MyTagControlCollection.Count / NumberOfTags_Horizontally);
                                    else
                                        WrapHeight = (int)TagGadgetHeight * ((MyTagControlCollection.Count / NumberOfTags_Horizontally) + 1);
                                }
                                TotalGridHeight = MainGridHeight;
                            }
                            else
                            {
                                TotalGridHeight = MainGridHeight;
                            }

                            if (MyTagControlCollection2.Count != 0)
                            {
                                if ((MyTagControlCollection2.Count) % (NumberOfTags_Horizontally + 1) == 0)
                                    Wrap2Height = (int)TagGadgetHeight * (MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1));
                                else
                                    Wrap2Height = (int)TagGadgetHeight * ((MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1)) + 1);

                                TotalGridHeight = MainGridHeight + Wrap2Height;
                            }
                            else
                            {
                                TotalGridHeight = MainGridHeight;
                            }

                            if (!Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                            {
                                TotalGridHeight = MainGridHeight = WrapHeight;
                            }

                        }
                    }

                    if (MainStatObject != null)
                    {
                        if (MainStatObject.Length > 20)
                        {
                            StatValMargin = new Thickness(2, -5, 0, 0);
                            GridColumnMargin = new Thickness(2, -5, 0, 0);
                            StatisticsMargin = new Thickness(2, 5, 2, 0);
                            GridVerticalAlign = VerticalAlignment.Top;
                            GridValeAlign = VerticalAlignment.Top;
                            TagButtonMargin = new Thickness(0, -15, 0, 0);
                            MainTagButtonRowSpan = 1;
                            GridColumnSpan = 3;
                            GridValueHeight = 30;
                            GridTagButtonHeight = 22;
                        }
                        else
                        {
                            StatValMargin = new Thickness(2, 0, 0, 0);
                            GridColumnMargin = new Thickness(2, -20, 0, 0);
                            StatisticsMargin = new Thickness(2, -10, 2, 0);
                            GridVerticalAlign = VerticalAlignment.Bottom;
                            GridValeAlign = VerticalAlignment.Bottom;
                            TagButtonMargin = new Thickness(0, 15, 0, 0);
                            MainTagButtonRowSpan = 2;
                            GridColumnSpan = 0;
                            GridValueHeight = 45;
                            GridTagButtonHeight = 40;
                        }
                    }

                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                    {
                        if (Settings.GetInstance().DictEnableDisableChannels["tagbutton"])
                        {
                            MainStatValueColSpan = 0;
                            MainStatValueMargin = new Thickness(0, 0, 0, 0);
                        }
                        else
                        {
                            MainStatValueColSpan = 2;
                            MainStatValueMargin = new Thickness(0, 0, 2, 0);
                        }
                    }

                    foreach (TagGadgetControl tagCtrl in MyTagControlCollection)
                    {
                        if (tagCtrl.lblStatObj.Text.Length > 20)
                        {
                            tagCtrl.GridUntagButton.Margin = new Thickness(0, 0, 0, 0);
                            tagCtrl.lblStatName.Margin = new Thickness(2, 0, 0, 0);
                            tagCtrl.GridUntagButton.VerticalAlignment = VerticalAlignment.Top;
                            if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
                                {
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -18, 0, 0);
                                    tagCtrl.GridStatValue.Margin = new Thickness(-2, 0, 2, 0);
                                }
                                else
                                {
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -15, 0, 0);
                                    tagCtrl.GridStatValue.Margin = new Thickness(-2, 0, 4, 0);
                                }
                            }
                        }
                        else
                        {
                            tagCtrl.GridUntagButton.Margin = new Thickness(0, 15, 0, 0);
                            tagCtrl.lblStatName.Margin = new Thickness(2, 0, 0, 0);
                            tagCtrl.GridUntagButton.VerticalAlignment = VerticalAlignment.Bottom;
                            if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
                                {
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -40, 0, 0);
                                    tagCtrl.GridStatValue.Margin = new Thickness(-1, 0, 2, 0);
                                }
                                else
                                {
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -15, 0, 0);
                                    tagCtrl.GridStatValue.Margin = new Thickness(0, 0, 4, 0);
                                }
                            }
                        }
                    }

                    foreach (TagGadgetControl tagCtrl in MyTagControlCollection2)
                    {
                        if (tagCtrl.lblStatObj.Text.Length > 30)
                        {
                            tagCtrl.GridUntagButton.Margin = new Thickness(0, 0, 0, 0);
                            tagCtrl.lblStatName.Margin = new Thickness(2, 0, 0, 0);
                            tagCtrl.GridUntagButton.VerticalAlignment = VerticalAlignment.Top;

                            if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
                                {
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -15, 0, 0);
                                    tagCtrl.GridStatValue.Margin = new Thickness(-2, 0, 2, 0);
                                }
                                else
                                {
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -15, 0, 0);
                                }
                            }
                        }
                        else
                        {
                            tagCtrl.GridUntagButton.Margin = new Thickness(0, 15, 0, 0);
                            tagCtrl.lblStatName.Margin = new Thickness(2, 0, 0, 0);
                            tagCtrl.GridUntagButton.VerticalAlignment = VerticalAlignment.Bottom;

                            if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
                                {
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -40, 0, 0);
                                }
                                else
                                {
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -15, 0, 0);
                                }
                            }
                        }
                    }
                    TagStatvaleVisible = Visibility.Visible;
                    GridVerticalAlign = VerticalAlignment.Bottom;
                    GridValeAlign = VerticalAlignment.Bottom;
                    isHeaderClicked = false;

                    #endregion
                }
                else
                {
                    #region Show Header False

                    if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                    {
                        GadgetHeight = 52;
                    }
                    else
                    {
                        GadgetHeight = 0;
                    }

                    GridRowHeight = 27;
                    GridStatisticsHeight = 23;
                    TagGadgetHeight = 26;
                    GridStatObjectsHeight = 0;
                    GridValueHeight = 45;
                    GridTagButtonHeight = 40;

                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                    {
                        if (Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
                        {
                            if (MyTagControlCollection2.Count != 0)
                            {
                                Wrap2Height = (int)TagGadgetHeight * (MyTagControlCollection2.Count);

                                MainGridHeight = (int)GadgetHeight;

                                if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                {
                                    TotalGridHeight = MainGridHeight + Wrap2Height;
                                }
                                else
                                {
                                    TotalGridHeight = Wrap2Height;
                                }
                            }
                            else
                            {
                                MainGridHeight = (int)GadgetHeight;
                                TotalGridHeight = MainGridHeight;
                            }
                        }
                        else
                        {

                            MainGridHeight = (int)GadgetHeight;

                            if (MyTagControlCollection.Count != 0)
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                {
                                    if (MyTagControlCollection.Count > NumberOfTags_Horizontally)
                                    {
                                        WrapHeight = (int)TagGadgetHeight * 2;
                                    }
                                    else
                                    {
                                        WrapHeight = (int)TagGadgetHeight;
                                    }
                                }
                                else
                                {
                                    if ((MyTagControlCollection.Count) % NumberOfTags_Horizontally == 0)
                                        WrapHeight = (int)TagGadgetHeight * (MyTagControlCollection.Count / NumberOfTags_Horizontally);
                                    else
                                        WrapHeight = (int)TagGadgetHeight * ((MyTagControlCollection.Count / NumberOfTags_Horizontally) + 1);
                                }
                                TotalGridHeight = MainGridHeight;
                            }
                            else
                            {
                                TotalGridHeight = MainGridHeight = (int)GadgetHeight;
                            }

                            if (MyTagControlCollection2.Count != 0)
                            {
                                if ((MyTagControlCollection2.Count) % (NumberOfTags_Horizontally + 1) == 0)
                                    Wrap2Height = (int)TagGadgetHeight * (MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1));
                                else
                                    Wrap2Height = (int)TagGadgetHeight * ((MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1)) + 1);

                                TotalGridHeight = MainGridHeight + Wrap2Height;
                            }
                            else
                            {
                                TotalGridHeight = MainGridHeight;
                            }

                            if (!Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                            {
                                TotalGridHeight = MainGridHeight = WrapHeight;
                            }
                        }
                    }

                    TagStatvaleVisible = Visibility.Hidden;
                    GridVerticalAlign = VerticalAlignment.Top;
                    GridValeAlign = VerticalAlignment.Top;
                    TagStatObjMargin = new Thickness(2, 0, 0, 0);

                    GridColumnMargin = new Thickness(2, 0, 0, 0);
                    UnTagGridMargin = new Thickness(0, 0, 0, 0);

                    if (MainStatObject != null)
                    {
                        if (MainStatObject.Length > 20)
                        {
                            if (isHeaderClicked)
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels["tagbutton"])
                                    {
                                        StatisticsMargin = new Thickness(2, -2, 0, 0);
                                        MainStatValueMargin = new Thickness(0, -2, 0, 0);
                                        TagButtonMargin = new Thickness(0, 0, 0, 0);
                                    }
                                    else
                                    {
                                        StatisticsMargin = new Thickness(2, -2, 0, 0);
                                        MainStatValueMargin = new Thickness(0, 2, 0, 0);
                                        TagButtonMargin = new Thickness(0, 0, 0, 0);
                                        isHeaderHide = true;
                                    }
                                }
                            }
                            else
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels["tagbutton"])
                                    {
                                        StatisticsMargin = new Thickness(2, -12, 0, 0);
                                        MainStatValueMargin = new Thickness(0, -10, 0, 0);
                                        TagButtonMargin = new Thickness(0, -15, 0, 0);
                                    }
                                    else
                                    {
                                        if (isHeaderHide)
                                        {
                                            StatisticsMargin = new Thickness(2, -2, 2, 0);
                                            MainStatValueMargin = new Thickness(0, 2, 0, 0);
                                            TagButtonMargin = new Thickness(0, -15, 0, 0);
                                        }
                                        else
                                        {
                                            StatisticsMargin = new Thickness(2, -8, 2, 0);
                                            MainStatValueMargin = new Thickness(0, -10, 0, 0);
                                            TagButtonMargin = new Thickness(0, -15, 0, 0);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (isHeaderClicked)
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels["tagbutton"])
                                    {
                                        MainStatValueColSpan = 0;
                                        MainStatValueMargin = new Thickness(0, -8, 0, 0);
                                        StatisticsMargin = new Thickness(2, -10, 0, 0);
                                        TagButtonMargin = new Thickness(0, -15, 0, 0);
                                    }
                                    else
                                    {
                                        MainStatValueColSpan = 2;
                                        MainStatValueMargin = new Thickness(8, -10, 0, 0);
                                        StatisticsMargin = new Thickness(2, -7, 0, 0);
                                        TagButtonMargin = new Thickness(0, -5, 0, 0);
                                        isHeaderHideShortLength = true;
                                    }
                                }
                            }
                            else
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels["tagbutton"])
                                    {
                                        MainStatValueMargin = new Thickness(0, -10, 0, 0);
                                        StatisticsMargin = new Thickness(2, -8, 0, 0);
                                        TagButtonMargin = new Thickness(0, -15, 0, 0);
                                    }
                                    else
                                    {
                                        if (isHeaderHide && !isHeaderHideShortLength)
                                        {
                                            MainStatValueMargin = new Thickness(0, 2, 0, 0);
                                            StatisticsMargin = new Thickness(2, -1, 0, 0);
                                            TagButtonMargin = new Thickness(0, -15, 0, 0);
                                        }

                                        if (isHeaderHideShortLength && !isHeaderHide)
                                        {
                                            MainStatValueMargin = new Thickness(0, -10, 0, 0);
                                            StatisticsMargin = new Thickness(2, -7, 0, 0);
                                            TagButtonMargin = new Thickness(0, -15, 0, 0);
                                        }
                                    }
                                }
                            }
                        }
                        foreach (TagGadgetControl tagCtrl in MyTagControlCollection)
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
                                {
                                    tagCtrl.GridUntagButton.Margin = new Thickness(0, 2, 0, 0);
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -35, 0, 0);
                                    tagCtrl.lblStatName.Margin = new Thickness(2, 4, 0, 0);
                                    tagCtrl.GridStatValue.Margin = new Thickness(0, 0, 2, 0);
                                    tagCtrl.GridUntagButton.VerticalAlignment = VerticalAlignment.Top;
                                }
                                else
                                {
                                    tagCtrl.GridUntagButton.Margin = new Thickness(0, 2, 0, 0);
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -20, 0, 0);
                                    tagCtrl.lblStatName.Margin = new Thickness(2, 4, 0, 0);
                                    tagCtrl.GridStatValue.Margin = new Thickness(0, 0, 5, 0);
                                    tagCtrl.GridUntagButton.VerticalAlignment = VerticalAlignment.Top;
                                }
                            }
                        }

                        foreach (TagGadgetControl tagCtrl in MyTagControlCollection2)
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
                                {
                                    tagCtrl.GridUntagButton.Margin = new Thickness(0, 2, 0, 0);
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -35, 0, 0);
                                    tagCtrl.lblStatName.Margin = new Thickness(2, 4, 0, 0);
                                    tagCtrl.GridStatValue.Margin = new Thickness(0, 0, 2, 0);
                                    tagCtrl.GridUntagButton.VerticalAlignment = VerticalAlignment.Top;
                                }
                                else
                                {
                                    tagCtrl.GridUntagButton.Margin = new Thickness(0, 2, 0, 0);
                                    tagCtrl.lblStatObj.Margin = new Thickness(2, -35, 0, 0);
                                    tagCtrl.lblStatName.Margin = new Thickness(2, 4, 0, 0);
                                    tagCtrl.GridStatValue.Margin = new Thickness(0, 0, 5, 0);
                                    tagCtrl.GridUntagButton.VerticalAlignment = VerticalAlignment.Top;
                                }
                            }
                        }

                        isHeaderClicked = false;
                    }
                    else
                    {
                        GridTagButtonHeight = 0;
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                string sdf = ex.Message;
            }
        }

        /// <summary>
        /// Notifiers the specified statname.
        /// </summary>
        /// <param name="statname">The statname.</param>
        /// <param name="notifysec">The notifysec.</param>
        /// <param name="RefId">The preference unique identifier.</param>
        /// <param name="Attempt">The attempt.</param>
        public void Notifier(string statname, int notifysec, string RefId, int Attempt)
        {
            try
            {
                logger.Debug("MainWindowViewModel : Notifer : Method Entry");

                CustomNotifierViewModel cnotifyviewmodel = new CustomNotifierViewModel(Settings.GetInstance().Theme, null, null, null, false);
                taskbarNotifier.DataContext = cnotifyviewmodel;

                cnotifyviewmodel.NotificationContent = "Statistics value exceeded threshold value for " + statname;
                cnotifyviewmodel.NotificationTitle = "Threshold Breach";
                taskbarNotifier.StayOpenMilliseconds = notifysec;

                taskbarNotifier.Show();
                taskbarNotifier.Notify(false, RefId, Attempt);
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : Notifier : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : Notifier : Method Exit");
            }
        }

        /// <summary>
        /// Notifies the agent group statistics.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <param name="statisticValue">The statistic value.</param>
        /// <param name="statType">Type of the stat.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="color">The color.</param>
        public void NotifyAgentGroupStatistics(string refId, string statName, string statValue, string toolTip, Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo)
        {
            //Application.Current.Dispatcher.Invoke((Action)(delegate
            //{
            _StatDisplayMessage.Invoke(refId, statName, statValue, toolTip, statColor, statType, isThresholdBreach, isLevelTwo);
            //}));
        }

        /// <summary>
        /// Notifies the agent statistics.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <param name="statisticValue">The statistic value.</param>
        /// <param name="statType">Type of the stat.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="color">The color.</param>
        public void NotifyAgentStatistics(string refId, string statName, string statValue, string toolTip, Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo)
        {
            _StatDisplayMessage.Invoke(refId, statName, statValue, toolTip, statColor, statType, isThresholdBreach, isLevelTwo);
        }

        public void NotifyAgentStatus(string agentid, string status)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Notifies the aid statistics.
        /// </summary>
        /// <param name="StatisticsEvents">The statistics events.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void NotifyAIDStatistics(Genesyslab.Platform.Reporting.Protocols.StatServer.Events.EventInfo StatisticsEvents)
        {
            //
        }

        /// <summary>
        /// Notifies the database statistics.
        /// </summary>
        /// <param name="refid">The refid.</param>
        /// <param name="statisticsName">Name of the statistics.</param>
        /// <param name="statisticsValue">The statistics value.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="statColor">Color of the stat.</param>
        /// <param name="isThresholdBreach">if set to <c>true</c> [is threshold breach].</param>
        /// <param name="dbStatName">Name of the database stat.</param>
        public void NotifyDBStatistics(string refid, string statisticsName, string statisticsValue, string toolTip, Color statColor, bool isThresholdBreach, string dbStatName, bool isLevelTwo)
        {
            _DBStatDisplayMessage.Invoke(refid, statisticsName, statisticsValue, toolTip, statColor, isThresholdBreach, dbStatName, isLevelTwo);
        }

        /// <summary>
        /// Notifies the gadget status.
        /// </summary>
        /// <param name="gadgetstate">The gadgetstate.</param>
        public void NotifyGadgetStatus(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState gadgetstate)
        {
            //
        }

        /// <summary>
        /// Notifies the queue statistics.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <param name="statisticValue">The statistic value.</param>
        /// <param name="statType">Type of the stat.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="color">The color.</param>
        public void NotifyQueueStatistics(string refId, string statName, string statValue, string toolTip, Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo)
        {
            _StatDisplayMessage.Invoke(refId, statName, statValue, toolTip, statColor, statType, isThresholdBreach, isLevelTwo);
        }

        /// <summary>
        /// Notifies the show CC statistics.
        /// </summary>
        /// <param name="isCCStatistics">if set to <c>true</c> [is CC statistics].</param>
        public void NotifyShowCCStatistics(bool isCCStatistics)
        {
            //Nothing to implement
        }

        /// <summary>
        /// Notifies the show my statistics.
        /// </summary>
        /// <param name="iMyStatistics">if set to <c>true</c> [i my statistics].</param>
        public void NotifyShowMyStatistics(bool iMyStatistics)
        {
            //Nothing to implement
        }

        /// <summary>
        /// Notifies the stat error message.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void NotifyStatErrorMessage(OutputValues error)
        {
            DisplayErrorMessage(error);
        }

        public void NotifyStatServerStatustoTC(bool status, string serverName)
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Orientations the changed.
        /// </summary>
        public void OrientationChanged()
        {
            try
            {
                logger.Debug("MainWindowViewModel : OrientationChanged : Method Entry");
                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                {
                    if (!Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
                    {
                        int tagno = 1;

                        MyTagControlCollection = new ObservableCollection<UserControl>();
                        MyTagControlCollection2 = new ObservableCollection<UserControl>();
                        Wrap2Collection = new ObservableCollection<UserControl>();

                        foreach (UserControl tag in MyTagControlTempCollection)
                        {
                            if (tagno <= (NumberOfTags_Horizontally * 2) || (!Settings.GetInstance().DictEnableDisableChannels["maingadget"]))
                            {
                                MyTagControlCollection.Add(tag);
                                Wrap2Visibility = Visibility.Hidden;
                                isThirdRowActive = false;
                            }
                            else
                            {
                                MyTagControlCollection2.Add(tag);
                                Wrap2Collection.Add(tag);
                                Wrap2Visibility = Visibility.Visible;
                                isThirdRowActive = true;
                            }

                            tagno++;
                        }

                    }
                    else
                    {
                        int tagno = 1;

                        MyTagControlCollection2 = new ObservableCollection<UserControl>();

                        foreach (UserControl tag in MyTagControlTempCollection)
                        {
                            if (tagno <= NumberOfTags_Vertically)
                            {
                                MyTagControlCollection2.Add(tag);
                                isThirdRowActive = false;
                                WrapHeight = 0;
                                WrapWidth = 0;
                                Wrap2Visibility = Visibility.Visible;
                            }
                            else
                                break;

                            tagno++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : OrientationChanged Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : OrientationChanged : Method Exit");
            }
        }

        /// <summary>
        /// savetaggedstats this instance.
        /// </summary>
        public void savetaggedstats()
        {
            try
            {
                logger.Debug("MainWindowViewModel : savetaggedstats : Method Entry");

                #region Close Gadget

                //if (isExitApp)
                //{

                logger.Debug("MainWindowViewModel : savetaggedstats : Saving Statistics");
                isMinimized = false;
                int tagno = 1;
                //string statDisplayName = string.Empty;
                taggedStats = string.Empty;
                Dictionary<string, string> DictTaggedStats = new Dictionary<string, string>();
                string ServerTaggedStats = string.Empty;
                string DBTaggedStats = string.Empty;

                try
                {

                    output = new OutputValues();
                    if (hashTaggedStats.Count > 0)
                    {
                        SortedDictionary<int, string> tempdic = new SortedDictionary<int, string>();

                        if (MyTagControlCollection != null)
                        {
                            foreach (TagGadgetControl temptagcontrol in MyTagControlCollection)
                            {
                                foreach (DictionaryEntry entry in hashTaggedStats)
                                {
                                    if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
                                    {
                                        string[] keyvalues = entry.Key.ToString().Split('_');
                                        int tagn = Convert.ToInt32(keyvalues[1]);
                                        tempdic.Add(tagn, entry.Value.ToString());
                                    }
                                }
                            }
                        }

                        if (MyTagControlCollection2 != null)
                        {
                            foreach (TagGadgetControl temptagcontrol in MyTagControlCollection2)
                            {
                                foreach (DictionaryEntry entry in hashTaggedStats)
                                {
                                    if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
                                    {
                                        string[] keyvalues = entry.Key.ToString().Split('_');
                                        int tagn = Convert.ToInt32(keyvalues[1]);

                                        if (!tempdic.ContainsKey(tagn))
                                            tempdic.Add(tagn, entry.Value.ToString());
                                    }
                                }
                            }
                        }

                        foreach (string statName in tempdic.Values)
                        {
                            string Dilimitor = "_@";
                            string[] stat = statName.Split(new[] { Dilimitor }, StringSplitOptions.RemoveEmptyEntries);
                            string statobj = "";
                            if (stat.Length == 2)
                                statobj = (stat[1].Split('\n'))[0];
                            else
                            {
                                statobj = (stat[0].Split('@'))[0];
                                stat[0] = (stat[0].Split('@'))[1];
                            }
                                

                            if (ServerTaggedStats.Equals(string.Empty))//&& statDisplayName.Equals(string.Empty))
                            {
                                 if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                                {
                                    //statDisplayName = tagno + "_&" + statobj[1] + "@" + statobj[0];
                                    ServerTaggedStats = statobj + "@" + stat[0];
                                }
                                else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                {
                                    //statDisplayName = tagno + "_&" + statobj[1] + "@" + statobj[0];
                                    ServerTaggedStats = statobj + "@" + stat[0];
                                }
                            }
                            else
                            {
                                if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                                {
                                    //statDisplayName = statDisplayName + "," + tagno + "_&" + statobj[1] + "@" + statobj[0];
                                    ServerTaggedStats = ServerTaggedStats + "," + statobj + "@" + stat[0];
                                }
                                else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                {
                                        //statDisplayName = statDisplayName + "," + tagno + "_&" + statobj[1] + "@" + statobj[0];
                                        ServerTaggedStats = ServerTaggedStats + "," + statobj + "@" + stat[0];
                                }
                            }
                            tagno++;
                        }
                        if (!DictTaggedStats.ContainsKey(StatisticsEnum.StatSource.StatServer.ToString()))
                            DictTaggedStats.Add(StatisticsEnum.StatSource.StatServer.ToString(), ServerTaggedStats);

                        if (!DictTaggedStats.ContainsKey(StatisticsEnum.StatSource.DB.ToString()))
                            DictTaggedStats.Add(StatisticsEnum.StatSource.DB.ToString(), ServerTaggedStats);

                        logger.Debug("MainWindowViewModel : savetaggedstats : collection : " + DictTaggedStats.Count.ToString());
                        output = objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);

                        // if commented for dont write statistics on stattickerfive.exe.config file -02/09/2014
                        // if (output.MessageCode == "2007")
                        //   SaveTaggedStatistics(statDisplayName);
                    }
                    else
                    {

                        logger.Debug("MainWindowViewModel : savetaggedstats : collection is Empty");
                        objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);
                        // if commented for dont write statistics on stattickerfive.exe.config file -02/09/2014
                        // if (output.MessageCode == "2007")
                        //   SaveTaggedStatistics(string.Empty);
                    }

                }
                catch (Exception ex)
                {
                    logger.Error("Error occurred while saving the tagged stats as : " + ex.Message);
                    foreach (Window window in Application.Current.Windows)
                        if (window.Title == "StatGadget")
                            window.Hide();
                    //window.Close();
                }
                //}
                //else
                //{
                //    foreach (Window window in Application.Current.Windows)
                //        if (window.Title == "StatGadget")
                //            window.Hide();
                //}

                #endregion
            }
            catch (Exception ex)
            {
                //System.Windows.MessageBox.Show("MainWindowViewModel : gadgetclose : " + ex.Message);
                logger.Error("MainWindowViewModel : gadgetclose : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : GadgetClose : Method Exit");
            }
        }

        /// <summary>
        /// Shows the threshold breach.
        /// </summary>
        /// <param name="RefId">The ref id.</param>
        /// <param name="StatName">Name of the stat.</param>
        public void ShowThresholdBreach(string RefId, String StatName, bool isLeveltwo)
        {
            bool isThresholdBreach;
            bool isLevelTwo = isLeveltwo;
            int Attempt = 1;
            try
            {
                logger.Error("MainWindowViewModel : ShowThresholdBreach : Entry");
                if (isMinimized)
                {
                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("notificationballoon"))
                    {
                        if (Settings.GetInstance().DictEnableDisableChannels["notificationballoon"])
                        {
                            if (!isLevelTwo)
                            {
                                isThresholdBreach = Settings.GetInstance().DictThresholdBreach[RefId];
                                if (isThresholdBreach)
                                {
                                    Notifier(StatName, (StatisticsBase.GetInstance().statsIntervalTime * 1000), RefId, Attempt);
                                }
                            }
                        }
                    }
                }
                else
                {
                    taskbarNotifier.Hide();
                }

                if (isNotiferPopupSelected || isNotifierAudioSelected)
                {
                    if (isLevelTwo)
                    {
                        if (taskbarNotifier.IsThresholdNotifierClosed || Settings.GetInstance().isThresholdNotifierEnded)
                        {

                            if (Settings.GetInstance().isThresholdNotifierEnded && isNotifierAudioSelected)
                            {
                                Settings.GetInstance().isThresholdNotifierEnded = false;
                                mediaPlayer.Open(new Uri(StatisticsBase.GetInstance().ThresholdBreachAlertPath, UriKind.Relative));

                                if (!dictAudioAlertAttempts.ContainsKey(RefId))
                                {
                                    dictAudioAlertAttempts.Add(RefId, Attempt);
                                }
                                else
                                {
                                    dictAudioAlertAttempts[RefId] = Convert.ToInt32(dictAudioAlertAttempts[RefId]) + 1;
                                }

                                if (Convert.ToInt32(dictAudioAlertAttempts[RefId]) <= StatisticsBase.GetInstance().AudioPlayAttempt)
                                {
                                    Mediatimer.Start();
                                    mediaPlayer.Play();
                                }
                            }

                            if (taskbarNotifier.IsThresholdNotifierClosed && isNotiferPopupSelected)
                            {

                                if (!dictPopupAlertAttempts.ContainsKey(RefId))
                                {
                                    dictPopupAlertAttempts.Add(RefId, Attempt);
                                }
                                else
                                {
                                    dictPopupAlertAttempts[RefId] = Convert.ToInt32(dictPopupAlertAttempts[RefId] + 1);
                                }

                                if (Convert.ToInt32(dictPopupAlertAttempts[RefId]) <= StatisticsBase.GetInstance().AlertPopupAttempt)
                                {

                                    if (!LstNotifierIds.Contains(RefId))
                                    {
                                        //New Alert
                                        LstNotifierIds.Add(RefId);
                                        growlNotifications.AddNotification(new WPFGrowlNotification.Notification { RefId = RefId, Title = "Threshold Breach", ImageUrl = "pack://application:,,,/Resources/notification-icon.png", Message = "Statistics value exceeded threshold level 2 for " + StatName, TitleBackground = new BrushConverter().ConvertFromString(StatisticsBase.GetInstance().ThresholdTitleBackground) as SolidColorBrush, ContentBackground = new BrushConverter().ConvertFromString(StatisticsBase.GetInstance().ThresholdContentBackground) as SolidColorBrush, ContentForeground = new BrushConverter().ConvertFromString(StatisticsBase.GetInstance().ThresholdContentForeground) as SolidColorBrush, FontBold = StatisticsBase.GetInstance().IsThresholdNotifierBold ? FontWeights.Bold : FontWeights.Regular });
                                        i++;
                                    }
                                    else
                                    {
                                        dictPopupAlertAttempts[RefId] = --dictPopupAlertAttempts[RefId];
                                    }
                                    //AlertNotifier("Threshold Breach", "Statistics value exceeded threshold level 2 for " + StatName, StatisticsBase.GetInstance().ThresholdTitleBackground, StatisticsBase.GetInstance().ThresholdContentBackground, StatisticsBase.GetInstance().ThresholdContentForeground, StatisticsBase.GetInstance().IsThresholdNotifierBold, StatisticsBase.GetInstance().statsIntervalTime * 1000);
                                    //-Old Alert
                                    //AlertNotifier(StatName, (StatisticsBase.GetInstance().AlertNotifyTime * 1000), RefId, Attempt);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dictAudioAlertAttempts.ContainsKey(RefId))
                        {
                            Settings.GetInstance().isThresholdNotifierEnded = true;
                            taskbarNotifier.IsThresholdNotifierClosed = true;
                            dictAudioAlertAttempts[RefId] = 0;
                        }

                    }
                }

                logger.Error("MainWindowViewModel : ShowThresholdBreach : Exit");
            }
            catch (Exception GeneralException)
            {
                logger.Error("MainWindowViewModel : ShowThresholdBreach : " + GeneralException.Message);
            }
        }

        /// <summary>
        /// Stats the alignment.
        /// </summary>
        /// <param name="statname">The statname.</param>
        public void StatAlignment(string statname)
        {
            try
            {
                logger.Debug("MainWindowViewModel : StatAlignment : Method Entry");
                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                {
                    if (Settings.GetInstance().DictEnableDisableChannels["tagbutton"])
                    {
                        TagVisibility = Visibility.Visible;
                        VBStatValMargin = new Thickness(-15, -1, 0, 0);
                        MainStatWidth = 70;
                    }
                    else
                    {
                        TagVisibility = Visibility.Hidden;
                        VBStatValMargin = new Thickness(-25, -1, 0, 0);
                        MainStatWidth = 70;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : StatAlignment : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : StatAlignment : Method Exit");
            }
        }

        /// <summary>
        /// Stats the ticker gadget__ stat display message.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <param name="statisticValue">The statistic value.</param>
        /// <param name="statType">Type of the stat.</param>
        /// <param name="toolTip">The tool tip.</param>
        /// <param name="color">The color.</param>
        public void StatTickerGadget__StatDisplayMessage(string refId, string statName, string statValue, string toolTip, Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo)
        {
            StatisticsInfo objStatInfo;
            string statname;

            try
            {
                Application.Current.Dispatcher.Invoke((Action)(delegate
                {
                    logger.Debug("MainWindowViewModel : StatTickerGadget_StatDisplayMessage : Method Entry");
                    ReferenceId = refId;
                    strNewStatName = statName;

                    statname = formattedStatName(statName);

                    objStatInfo = new StatisticsInfo();

                    objStatInfo.StatName = statname;
                    objStatInfo.StatValue = statValue;
                    objStatInfo.StatColor = statColor;

                    objStatInfo.StatType = statType;
                    objStatInfo.StatToolTip = toolTip;
                    objStatInfo.IsLevelTwo = isLevelTwo;
                    objStatInfo.StatTag = strNewStatName;
                    objStatInfo.RefId = refId.ToString();
                    objStatInfo.BackgroundColors = ServerColor;
                    objStatInfo.ApplicationType = StatisticsEnum.StatSource.StatServer.ToString();

                    foreach (KeyValuePair<string, string> item in DicTemp)
                    {
                        if (item.Value == strNewStatName)
                        {
                            string[] statobj = item.Key.ToString().Split('@');
                            string[] statTooltip = toolTip.Split('\n');
                            if (statobj[1] == statTooltip[0])
                            {
                                DicTemp[item.Key] = refId;
                                string value = DicTemp[item.Key];
                                //System.Windows.MessageBox.Show("Remove" + item.Key);
                                DicTemp.Remove(item.Key);
                                //System.Windows.MessageBox.Show("Add" + item.Key);
                                DicTemp.Add(item.Key, value);
                                break;
                            }
                        }
                    }

                    if (Settings.GetInstance().DictAllStats.ContainsKey(refId))
                    {
                        string statval = Settings.GetInstance().DictAllStats[refId].ToString();

                        string[] statobject = toolTip.Split('\n');

                        if (listTaggedStatObjects.Contains(statobject[0]))
                        {
                            foreach (DictionaryEntry items in hashTaggedStats)
                            {
                                //if (items.Value.ToString() == statval)
                                //{
                                //    string statTool = items.Value + "_@" + toolTip;
                                //    hashTaggedStats[items.Key] = statTool;

                                //    break;
                                //}
                                //code changed by arun on 11-01-2016
                                if (items.Value.ToString() == statobject[0] + "@" + statval)
                                {
                                    string statTool = statval + "_@" + toolTip;
                                    hashTaggedStats[items.Key] = statTool;

                                    break;
                                }
                            }
                        }
                    }

                    if (!Settings.GetInstance().DicStatInfo.ContainsKey(refId))
                    {
                        Settings.GetInstance().DicStatInfo.Add(refId, objStatInfo);
                    }
                    else
                    {
                        Settings.GetInstance().DicStatInfo[refId] = objStatInfo;
                    }

                    if (StatDisplayTimer.Tag.ToString() == "True")
                    {
                        Settings.GetInstance().StatCount = 0;
                        StatDisplayTimer.Tag = "False";
                        StatDisplayTimer.Interval = new TimeSpan(0, 0, 0);
                        StatDisplayTimer.Tick += DispTimerTick;
                        StatDisplayTimer.Start();

                        if (StatisticsBase.GetInstance().StatSource == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString() || StatisticsBase.GetInstance().StatSource == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString())
                        {
                            StatDisplayTimer.Interval = new TimeSpan(0, 0, StatisticsBase.GetInstance().statsIntervalTime);
                        }
                    }

                    if (!Settings.GetInstance().DictThresholdBreach.ContainsKey(refId))
                        Settings.GetInstance().DictThresholdBreach.Add(refId, isThresholdBreach);
                    else
                        Settings.GetInstance().DictThresholdBreach[refId] = isThresholdBreach;

                    ShowThresholdBreach(refId, statName, isLevelTwo);
                    UpdateTagValues(refId, statValue, toolTip, statColor, ServerColor, StatisticsEnum.StatSource.StatServer);

                    foreach (TagGadgetControl TempTag in MyTagControlTempCollection)
                    {
                        if (TempTag.lblStatName.ToolTip != null)
                        {
                            if (TempTag.lblStatName.ToolTip.ToString() == toolTip)
                            {
                                TempTag.lblStatName.Tag = refId;
                                break;
                            }
                        }
                    }

                }), DispatcherPriority.ContextIdle, new object[0]);
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : StatTickerGadget__StatDisplayMessage Method : " + ex.Message);
            }
            finally
            {
                objStatInfo = null;
                GC.Collect();
                logger.Debug("MainWindowViewModel : StatTickerGadget_StatDisplayMessage : Method Exit");
            }
        }

        public void StatTickerGadget__StatDisplayMessage(string refId, string statName, string statValue, string toolTip, Color statColor, bool isThresholdBreach, string dbStatName, bool isLevelTwo)
        {
            StatisticsInfo objStatInfo;
            string statname;

            try
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(delegate
            {
                logger.Debug("MainWindowViewModel : StatTickerGadget_StatDisplayMessage : Method Entry");
                ReferenceId = refId;
                strNewStatName = statName;

                statname = formattedStatName(statName);

                objStatInfo = new StatisticsInfo();

                objStatInfo.StatName = statname;

                if (statValue != null && statValue != string.Empty && statValue != "")
                    objStatInfo.StatValue = statValue;
                else
                    objStatInfo.StatValue = "-";

                objStatInfo.StatColor = statColor;
                objStatInfo.StatToolTip = toolTip;
                objStatInfo.IsLevelTwo = isLevelTwo;
                objStatInfo.StatTag = strNewStatName;
                objStatInfo.DBStatName = dbStatName;
                objStatInfo.RefId = refId.ToString();

                if (!DBColor.IsEmpty)
                    objStatInfo.BackgroundColors = DBColor;
                else if (ServerResult)
                    objStatInfo.BackgroundColors = objStatTicker.GetDBColor();
                else
                    objStatInfo.BackgroundColors = Color.White;

                objStatInfo.ApplicationType = StatisticsEnum.StatSource.DB.ToString();

                foreach (KeyValuePair<string, string> item in DicTemp)
                {
                    string[] itemval = item.Key.Split('@');

                    if (itemval[1].ToString() == dbStatName)
                    {
                        string TempKey = itemval[0] + "@" + dbStatName;
                        //System.Windows.MessageBox.Show("Remove" + item.Key);
                        DicTemp.Remove(item.Key);
                        //System.Windows.MessageBox.Show("Add" + TempKey);
                        DicTemp.Add(TempKey, refId);
                        break;
                    }
                }

                if (Settings.GetInstance().DictAllStats.ContainsKey(refId))
                {
                    string statval = Settings.GetInstance().DictAllStats[refId].ToString();

                    if (listTaggedStatObjects.Contains(dbStatName))
                    {
                        foreach (DictionaryEntry items in hashTaggedStats)
                        {
                            if (items.Value.ToString() == statval)
                            {
                                string statTool = items.Value + "_@" + toolTip;
                                hashTaggedStats[items.Key] = statTool;

                                break;
                            }
                        }
                    }
                }

                foreach (TagGadgetControl TempTag in MyTagControlTempCollection)
                {
                    if (TempTag.lblStatName.Tag.ToString() == dbStatName)
                    {
                        TempTag.lblStatName.Tag = refId;
                        break;
                    }
                }

                if (!Settings.GetInstance().DicStatInfo.ContainsKey(refId))
                {
                    Settings.GetInstance().DicStatInfo.Add(refId, objStatInfo);
                }
                else
                {
                    Settings.GetInstance().DicStatInfo[refId] = objStatInfo;
                }

                if (StatDisplayTimer.Tag.ToString() == "True")
                {
                    Settings.GetInstance().StatCount = 0;
                    StatDisplayTimer.Tag = "False";
                    StatDisplayTimer.Interval = new TimeSpan(0, 0, 0);
                    StatDisplayTimer.Tick += DispTimerTick;
                    StatDisplayTimer.Start();

                    if (StatisticsBase.GetInstance().StatSource == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.DB.ToString())
                    {
                        StatDisplayTimer.Interval = new TimeSpan(0, 0, Convert.ToInt32(Settings.GetInstance().DisplayTime));
                    }
                    else if (StatisticsBase.GetInstance().StatSource == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString())
                    {
                        StatDisplayTimer.Interval = new TimeSpan(0, 0, Settings.GetInstance().DisplayTime);
                    }
                }

                if (!Settings.GetInstance().DictThresholdBreach.ContainsKey(refId))
                    Settings.GetInstance().DictThresholdBreach.Add(refId, isThresholdBreach);
                else
                    Settings.GetInstance().DictThresholdBreach[refId] = isThresholdBreach;

                ShowThresholdBreach(refId, statName, objStatInfo.IsLevelTwo);

                UpdateTagValues(refId, statValue, dbStatName + "\n" + toolTip, statColor, DBColor, StatisticsEnum.StatSource.DB);
            }), DispatcherPriority.Background);
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : StatTickerGadget__StatDisplayMessage Method : " + ex.Message);
            }
            finally
            {
                objStatInfo = null;
                GC.Collect();
                logger.Debug("MainWindowViewModel : StatTickerGadget_StatDisplayMessage : Method Exit");
            }
        }

        /// <summary>
        /// Handles the Tick event of the UnPausedStatTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        public void UnPausedStatTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                logger.Debug("MainWindowViewModel : UnPausedStatTimer_Tick : Method Entry");
                KeyValuePair<string, StatisticsInfo> kvpTemp;

                kvpTemp = Settings.GetInstance().DicStatInfo.ElementAt(Settings.GetInstance().StatCount);
                StatisticsInfo statInfo = kvpTemp.Value;

                StatValue = statInfo.StatValue;
            }
            catch (Exception GeneralException)
            {
                logger.Error("MainWindowViewModel : UnPausedStatTimer_Tick : Error : " + GeneralException.Message);
            }
            finally
            {
                logger.Debug("MainWindowViewModel : UnPausedStatTimer_Tick : Method Exit");
            }
        }

        /// <summary>
        /// Untag the gadget.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void UnTagGadget(object obj)
        {
            string remTag = string.Empty;
            try
            {
                logger.Debug("MainWindowViewModel : UnTagGadget : Method Entry");

                #region First Wrap object

                foreach (TagGadgetControl TagItems in MyTagControlCollection.Cast<TagGadgetControl>().Where(TagItems => TagItems.Tag.ToString() == obj.ToString()))
                {
                    foreach (DictionaryEntry items in hashTaggedStats.Cast<DictionaryEntry>().Where(items => items.Key.ToString() == obj.ToString()))
                    {
                        string Dilimitor = "_@";
                        string[] item = items.Value.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);
                        Dilimitor = "\n";
                        string[] tagnumber = items.Key.ToString().Split('_');

                        if (item.Length > 1)
                        {
                            Dilimitor = "\n";
                            string[] objname = item[1].Split(new[] { Dilimitor }, StringSplitOptions.None);
                            dictAddedStatistics.Remove(Convert.ToInt32(tagnumber[1]));

                            string stat = items.Key.ToString() + "@" + objname[0].ToString();
                            //System.Windows.MessageBox.Show("Remove" + stat);
                            DicTemp.Remove(stat);
                            stat = items.Key.ToString() + "@" + item[0].ToString();
                            //System.Windows.MessageBox.Show("Remove" + stat);
                            DicTemp.Remove(stat);

                            Settings.GetInstance().DictTaggedStats.Remove(tagnumber[1] + "," + objname[0] + "@" + item[0]);
                            listTaggedStatObjects.Remove(objname[0]);
                            removedNo.Enqueue(items.Key.ToString());
                            remTag = items.Key.ToString();
                            Settings.GetInstance().HshAllTagList.Remove(items.Key);

                            hashTaggedStats.Remove(items.Key);

                            if (TotalTagsExceeded != 0)
                            {
                                TotalTagsExceeded--;
                            }
                            TagNo--;
                            break;
                        }
                        else
                        {
                            dictAddedStatistics.Remove(Convert.ToInt32(tagnumber[1]));

                            foreach (string DeleteKey in DicTemp.Keys)
                            {
                                if (DeleteKey.Contains(items.Key.ToString()))
                                {
                                    //System.Windows.MessageBox.Show("Remove" + DeleteKey);
                                    DicTemp.Remove(DeleteKey);
                                    break;
                                }
                            }

                            foreach (string DeleteKey in Settings.GetInstance().DictTaggedStats.Keys)
                            {
                                if (DeleteKey.Contains(tagnumber[1].ToString()))
                                {
                                    Settings.GetInstance().DictTaggedStats.Remove(DeleteKey);
                                    break;
                                }
                            }
                            removedNo.Enqueue(items.Key.ToString());
                            remTag = items.Key.ToString();
                            Settings.GetInstance().HshAllTagList.Remove(items.Key);
                            hashTaggedStats.Remove(items.Key);

                            if (TotalTagsExceeded != 0)
                            {
                                TotalTagsExceeded--;
                            }
                            TagNo--;
                            break;
                        }

                    }
                    WrapHeight = WrapHeight - (int)GadgetHeight;

                    MyTagControlCollection.Remove(TagItems);
                    MyTagControlTempCollection.Remove(TagItems);

                    TempTagCollection.Remove(TagItems);
                    ExceededTagCollection.Remove(TagItems);
                    Wrap2Collection.Remove(TagItems);

                    foreach (DictionaryEntry dicEntry in hashTagDetails.Cast<DictionaryEntry>().Where(dicEntry => (string)dicEntry.Value == obj.ToString()))
                    {
                        hashTagDetails.Remove(dicEntry.Key);
                        string Dilimitor = "_@";
                        string[] tagstatname = dicEntry.Key.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);
                        Settings.GetInstance().HshAllTagNames.Remove(tagstatname[0]);

                        break;
                    }

                    if (TagNo % NumberOfTags_Horizontally == 0 || TagNo < NumberOfTags_Horizontally)
                    {
                        if (TagNo % NumberOfTags_Horizontally != 0 || TagNo < NumberOfTags_Horizontally)
                        {

                            WrapHorizontalWidth = WrapHorizontalWidth - (int)GadgetWidth;

                            WindowHorizontalWidth = WindowHorizontalWidth - (int)GadgetWidth;

                            WindowVerticalHeight = WindowVerticalHeight - (int)TagGadgetHeight;
                        }
                    }

                    break;

                }

                #endregion

                #region Second Wrap object

                foreach (TagGadgetControl TagItems in MyTagControlCollection2.Cast<TagGadgetControl>().Where(TagItems => TagItems.Tag.ToString() == obj.ToString()))
                {
                    foreach (DictionaryEntry items in hashTaggedStats.Cast<DictionaryEntry>().Where(items => items.Key.ToString() == obj.ToString()))
                    {
                        string Dilimitor = "_@";
                        string[] item = items.Value.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);
                        string[] tagnumber = items.Key.ToString().Split('_');

                        if (item.Length > 1)
                        {
                            Dilimitor = "\n";
                            string[] objname = item[1].Split(new[] { Dilimitor }, StringSplitOptions.None);
                            dictAddedStatistics.Remove(Convert.ToInt32(tagnumber[1]));

                            string stat = items.Key.ToString() + "@" + objname[0].ToString();
                            //System.Windows.MessageBox.Show("Remove" + stat);
                            DicTemp.Remove(stat);
                            stat = items.Key.ToString() + "@" + item[0].ToString();
                            //System.Windows.MessageBox.Show("Remove" + stat);
                            DicTemp.Remove(stat);

                            Settings.GetInstance().DictTaggedStats.Remove(tagnumber[1] + "," + objname[0] + "@" + item[0]);
                            listTaggedStatObjects.Remove(objname[0]);
                            removedNo.Enqueue(items.Key.ToString());
                            remTag = items.Key.ToString();
                            Settings.GetInstance().HshAllTagList.Remove(items.Key);

                            hashTaggedStats.Remove(items.Key);

                            if (TotalTagsExceeded != 0)
                            {
                                TotalTagsExceeded--;
                            }
                            TagNo--;
                            break;
                        }
                        else
                        {
                            dictAddedStatistics.Remove(Convert.ToInt32(tagnumber[1]));

                            foreach (string DeleteKey in DicTemp.Keys)
                            {
                                if (DeleteKey.Contains(items.Key.ToString()))
                                {
                                    //System.Windows.MessageBox.Show("Remove"+DeleteKey);
                                    DicTemp.Remove(DeleteKey);
                                    break;
                                }
                            }

                            foreach (string DeleteKey in Settings.GetInstance().DictTaggedStats.Keys)
                            {
                                if (DeleteKey.Contains(tagnumber[1].ToString()))
                                {
                                    Settings.GetInstance().DictTaggedStats.Remove(DeleteKey);
                                    break;
                                }
                            }
                            removedNo.Enqueue(items.Key.ToString());
                            remTag = items.Key.ToString();
                            Settings.GetInstance().HshAllTagList.Remove(items.Key);
                            hashTaggedStats.Remove(items.Key);

                            if (TotalTagsExceeded != 0)
                            {
                                TotalTagsExceeded--;
                            }
                            TagNo--;
                            break;
                        }
                    }

                    MyTagControlCollection2.Remove(TagItems);
                    Wrap2Collection.Remove(TagItems);
                    TempTagCollection.Remove(TagItems);
                    MyTagControlTempCollection.Remove(TagItems);

                    ExceededTagCollection.Remove(TagItems);

                    foreach (DictionaryEntry dicEntry in hashTagDetails.Cast<DictionaryEntry>().Where(dicEntry => (string)dicEntry.Value == obj.ToString()))
                    {
                        hashTagDetails.Remove(dicEntry.Key);
                        string Dilimitor = "_@";
                        string[] tagstatname = dicEntry.Key.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);
                        Settings.GetInstance().HshAllTagNames.Remove(tagstatname[0]);

                        break;
                    }

                    WrapHorizontalWidth = WrapHorizontalWidth - (int)GadgetWidth;

                    WindowHorizontalWidth = WindowHorizontalWidth - (int)GadgetWidth;

                    WindowVerticalHeight = WindowVerticalHeight - (int)TagGadgetHeight;

                    break;

                }
                #endregion

                if (Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
                {
                    int tagn = 1;

                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                    {
                        if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                        {
                            #region First Wrap Object

                            foreach (TagGadgetControl tagCtrl in MyTagControlCollection)
                            {
                                string[] tags = tagCtrl.Tag.ToString().Split('_');
                                int tagno = Convert.ToInt16(tags[1].ToString());

                                string txt = tagCtrl.lblStatName.Text;

                                if (tagn == 1)
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 2, 2, 2);
                                }
                                else
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                }
                                tagn++;
                            }

                            #endregion

                            #region Second Wrap Object

                            foreach (TagGadgetControl tagCtrl in MyTagControlCollection2)
                            {
                                string[] tags = tagCtrl.Tag.ToString().Split('_');
                                int tagno = Convert.ToInt16(tags[1].ToString());

                                string txt = tagCtrl.lblStatName.Text;

                                if (tagn == 1)
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 2, 2, 2);
                                }
                                else
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                }
                                tagn++;
                            }

                            #endregion

                        }
                        else
                        {

                            tagn = 0;

                            #region First Wrap Object

                            foreach (TagGadgetControl tagCtrl in MyTagControlCollection)
                            {
                                tagn++;

                                string[] tags = tagCtrl.Tag.ToString().Split('_');
                                int tagno = Convert.ToInt16(tags[1].ToString());

                                if (tagn == 1)
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 2, 2, 2);
                                }
                                else
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                }
                            }

                            #endregion

                            #region Second Wrap Object

                            foreach (TagGadgetControl tagCtrl in MyTagControlCollection2)
                            {
                                tagn++;

                                string[] tags = tagCtrl.Tag.ToString().Split('_');
                                int tagno = Convert.ToInt16(tags[1].ToString());

                                if (tagn == 1)
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 2, 2, 2);
                                }
                                else
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                }
                            }

                            #endregion

                            if (tagn == 0)
                                if (!StatisticsBase.GetInstance().isPlugin)
                                {
                                    Environment.Exit(0);
                                }
                                else
                                {
                                    foreach (Window window in Application.Current.Windows)
                                    {
                                        if (window.Title == "StatGadget")
                                        {
                                            window.Close();
                                            objStatTicker.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Ended);
                                        }
                                    }
                                }
                        }
                    }

                    if (TagNo > NumberOfTags_Horizontally)
                    {
                        WrapHeight = (TagNo * (int)TagGadgetHeight) - (TotalTagsExceeded * 2);// WrapVerticalHeight - (2 * (TagNo - NumberOfTags_Horizontally));
                    }
                    else
                    {
                        WrapHeight = WrapVerticalHeight;
                    }

                    bool isFirst = true;

                    #region First Wrap Object

                    foreach (TagGadgetControl tagCtrl in MyTagControlCollection)
                    {
                        if (isFirst)
                        {
                            WrapVerticalHeight = WrapVerticalHeight - (int)TagGadgetHeight;
                            isFirst = false;
                        }
                    }

                    #endregion

                    #region Second Wrap Object

                    foreach (TagGadgetControl tagCtrl in MyTagControlCollection2)
                    {
                        if (isFirst)
                        {
                            WrapVerticalHeight = WrapVerticalHeight - (int)TagGadgetHeight;
                            isFirst = false;
                        }
                    }

                    #endregion
                }
                else
                {
                    int tagn = 1;
                    bool isFirst = true;

                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                    {
                        if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                        {
                            #region First Wrap Object

                            foreach (TagGadgetControl tagCtrl in MyTagControlCollection)
                            {
                                string[] tags = tagCtrl.Tag.ToString().Split('_');
                                int tagno = Convert.ToInt16(tags[1].ToString());

                                string txt = tagCtrl.lblStatName.Text;
                                if (tagno <= NumberOfTags_Horizontally && isFirst)
                                {
                                    isFirst = false;
                                }

                                if (tagn > NumberOfTags_Horizontally)
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(0, 0, 2, 2);
                                }
                                else
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(0, 2, 2, 2);
                                }
                                tagn++;
                            }

                            #endregion

                            #region Second Wrap Object

                            foreach (TagGadgetControl tagCtrl in MyTagControlCollection2)
                            {
                                string[] tags = tagCtrl.Tag.ToString().Split('_');
                                int tagno = Convert.ToInt16(tags[1].ToString());

                                if (tagno <= NumberOfTags_Horizontally && isFirst)
                                {
                                    isFirst = false;
                                }

                                if (tagn > NumberOfTags_Horizontally)
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(0, 0, 2, 2);
                                }
                                else
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(0, 2, 2, 2);
                                }
                                tagn++;
                            }

                            #endregion
                        }
                        else
                        {
                            tagn = 0;

                            WrapMargin = new Thickness(-GadgetWidth, 0, 0, 0);

                            #region First Wrap Object

                            foreach (TagGadgetControl tagCtrl1 in MyTagControlCollection)
                            {
                                tagn++;

                                if (tagn < NumberOfTags_Horizontally)
                                {
                                    WrapHeight = (int)TagGadgetHeight;
                                }
                                else
                                {
                                    WrapHeight = (int)TagGadgetHeight * 2;
                                }

                                if (tagn <= NumberOfTags_Horizontally && isFirst)
                                {
                                    tagHeight = TagGadgetHeight;
                                    isFirst = false;
                                }

                                if (tagn == 1)
                                {
                                    tagCtrl1.MainGrid.Margin = new Thickness(2, 2, 2, 2);
                                }
                                else if (tagn == NumberOfTags_Horizontally)
                                {
                                    tagCtrl1.MainGrid.Margin = new Thickness(0, 2, 2, 2);
                                    WrapHeight = (int)TagGadgetHeight;
                                }
                                else if (tagn == NumberOfTags_Horizontally + 1)
                                {
                                    tagCtrl1.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                }
                            }

                            #endregion

                            #region Second Wrap Object

                            foreach (TagGadgetControl tagCtrl1 in MyTagControlCollection2)
                            {
                                tagn++;

                                if (tagn < NumberOfTags_Horizontally)
                                {
                                    WrapHeight = (int)TagGadgetHeight;
                                }
                                else
                                {
                                    WrapHeight = (int)TagGadgetHeight * 2;
                                }

                                if (tagn <= NumberOfTags_Horizontally && isFirst)
                                {
                                    isFirst = false;
                                }

                                if (tagn == 1)
                                {
                                    tagCtrl1.MainGrid.Margin = new Thickness(2, 2, 2, 2);
                                }
                                else if (tagn == NumberOfTags_Horizontally)
                                {
                                    tagCtrl1.MainGrid.Margin = new Thickness(0, 2, 2, 2);
                                    WrapHeight = (int)TagGadgetHeight;
                                }
                                else if (tagn == NumberOfTags_Horizontally + 1)
                                {
                                    tagCtrl1.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                }
                            }

                            #endregion

                            if (TagNo == 0)
                                if (!StatisticsBase.GetInstance().isPlugin)
                                {
                                    Environment.Exit(0);
                                }
                                else
                                {
                                    foreach (Window window in Application.Current.Windows)
                                    {
                                        if (window.Title == "StatGadget")
                                        {
                                            window.Close();
                                            objStatTicker.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Ended);
                                        }
                                    }
                                }

                            else if (TagNo <= NumberOfTags_Horizontally)
                                MainGridHeight = (int)GadgetHeight;
                            else
                                MainGridHeight = (int)GadgetHeight;
                        }
                    }
                }

                if (MyTagControlCollection2.Count == 0)
                {
                    isThirdRowActive = false;
                    Wrap2Visibility = Visibility.Hidden;
                    Wrap2Width = 0;
                }

                OrientationChanged();

                TagOrientation();
                HeaderValues();

                if (MyTagControlCollection2.Count % (NumberOfTags_Horizontally + 1) == 0)
                    Wrap2Rows = MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1);
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : UnTagGadget Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : UnTagGadget : Method Exit");
            }
        }

        /// <summary>
        /// Updates the tag values.
        /// </summary>
        public void UpdateTagValues(string refId, string statisticsValue, string statTooltip, Color statValuecolor, Color backgroundColor, StatisticsEnum.StatSource statisticsSource)
        {
            try
            {
                logger.Debug("MainWindowViewModel : UpdateTagValues : Method Entry");
                //System.Windows.MessageBox.Show("Dic ref ids "+ DicAsString(DicTemp) + "\n ------" + refId);
                if (DicTemp.ContainsValue(refId))
                {
                    //System.Windows.MessageBox.Show("contains rfid");

                    string[] taggedObjects = statTooltip.Split('\n');
                    if (listTaggedStatObjects.Contains(taggedObjects[0]))
                    {
                        //System.Windows.MessageBox.Show("contains taggedObject");
                        var keysWithMatchingValues = DicTemp.Where(p => p.Value == refId).Select(p => p.Key);

                        foreach (var key in keysWithMatchingValues)
                        {

                            //System.Windows.MessageBox.Show("has items in keywith matchingvalues");
                            string[] statObject = key.ToString().Split('@');
                            string[] stats = statObject[1].Split('\n');
                            string[] _statTooltip = statTooltip.Split('\n');
                            //System.Windows.MessageBox.Show(statObject + " : " + statTooltip);
                            if (stats[0] == _statTooltip[0])
                            {

                                //System.Windows.MessageBox.Show("Tagcontrol has items. " + MyTagControlCollection.Cast<TagGadgetControl>().Count(TagControl => TagControl.Tag.ToString() == statObject[0]));
                                foreach (TagGadgetControl TagControl in MyTagControlCollection.Cast<TagGadgetControl>().Where(TagControl => TagControl.Tag.ToString() == statObject[0]))
                                {
                                    //System.Windows.MessageBox.Show("inside foreach.");
                                    if (TagControl.lblStatName.ToolTip != null)
                                    {
                                        //System.Windows.MessageBox.Show("inside tooltip.");
                                        if (statisticsSource == StatisticsEnum.StatSource.StatServer)
                                            TagControl.lblStatName.ToolTip = statTooltip;
                                        else if (statisticsSource == StatisticsEnum.StatSource.DB)
                                            TagControl.lblStatName.ToolTip = _statTooltip[1].ToString();
                                        //System.Windows.MessageBox.Show("statisticsValue : " + statisticsValue);
                                        if (statisticsValue != string.Empty)
                                            TagControl.lblStatValue.Text = statisticsValue;
                                        //System.Windows.MessageBox.Show("lblStatValue : " + TagControl.lblStatValue.Text);

                                        TagControl.lblStatName.Text = UnTagButton(TagControl.lblStatName.Text, statisticsValue);

                                        string Dilimitor = "\n";
                                        string[] StatObj = TagControl.lblStatName.ToolTip.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);

                                        TagControl.lblStatObj.Text = StatObj[0].ToString();

                                        TagStatisticsObject = StatObj[0].ToString();

                                        SolidColorBrush Brush = new SolidColorBrush();
                                        if (statValuecolor.Name.ToString().Contains('#'))
                                        {
                                            Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(statValuecolor.Name.ToString()));
                                        }
                                        else
                                        {
                                            Brush.Color = System.Windows.Media.Color.FromArgb(statValuecolor.A, statValuecolor.R, statValuecolor.G,
                                                statValuecolor.B);
                                        }

                                        if (statisticsValue != string.Empty)
                                            TagControl.lblStatValue.Foreground = Brush;

                                        if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                        {
                                            Brush = new SolidColorBrush();
                                            if (backgroundColor.ToString().Contains('#'))
                                            {
                                                Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(backgroundColor.Name.ToString()));
                                            }
                                            else
                                            {
                                                Brush.Color = System.Windows.Media.Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B);
                                            }
                                            TagControl.MainGrid.Background = Brush;
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        string Dilimitor = "_@";

                                        if (statisticsSource == StatisticsEnum.StatSource.StatServer)
                                            TagControl.lblStatName.ToolTip = statTooltip;
                                        else if (statisticsSource == StatisticsEnum.StatSource.DB)
                                            TagControl.lblStatName.ToolTip = _statTooltip[1].ToString();

                                        if (statisticsValue != string.Empty)
                                            TagControl.lblStatName.Text = UnTagButton(TagControl.lblStatName.Text, statisticsValue);

                                        Dilimitor = "\n";
                                        string[] StatObj = TagControl.lblStatName.ToolTip.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);
                                        TagControl.lblStatObj.Text = StatObj[0].ToString();

                                        Dilimitor = "_@";

                                        string taggedStatValue = hashTaggedStats[TagControl.Tag.ToString()].ToString();

                                        string[] splitTagValues = taggedStatValue.Split(new[] { Dilimitor }, StringSplitOptions.None);

                                        if (statisticsValue != string.Empty)
                                            TagControl.lblStatValue.Text = statisticsValue;

                                        SolidColorBrush Brush = new SolidColorBrush();
                                        if (statValuecolor.Name.ToString().Contains('#'))
                                        {
                                            Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(statValuecolor.Name.ToString()));
                                        }
                                        else
                                        {
                                            Brush.Color = System.Windows.Media.Color.FromArgb(statValuecolor.A, statValuecolor.R, statValuecolor.G,
                                                statValuecolor.B);
                                        }

                                        if (statisticsValue != string.Empty)
                                            TagControl.lblStatValue.Foreground = Brush;

                                        if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                        {
                                            Brush = new SolidColorBrush();
                                            if (backgroundColor.ToString().Contains('#'))
                                            {
                                                Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(backgroundColor.Name.ToString()));
                                            }
                                            else
                                            {
                                                Brush.Color = System.Windows.Media.Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B);
                                            }
                                            TagControl.MainGrid.Background = Brush;
                                        }

                                        break;
                                    }
                                }

                                foreach (TagGadgetControl TagControl in MyTagControlCollection2.Cast<TagGadgetControl>().Where(TagControl => TagControl.Tag.ToString() == statObject[0]))
                                {
                                    if (TagControl.lblStatName.ToolTip != null)
                                    {
                                        {
                                            if (statisticsSource == StatisticsEnum.StatSource.StatServer)
                                                TagControl.lblStatName.ToolTip = statTooltip;
                                            else if (statisticsSource == StatisticsEnum.StatSource.DB)
                                                TagControl.lblStatName.ToolTip = _statTooltip[1].ToString();

                                            if (statisticsValue != string.Empty)
                                                TagControl.lblStatValue.Text = statisticsValue;

                                            TagControl.lblStatName.Text = UnTagButton(TagControl.lblStatName.Text, statisticsValue);

                                            string Dilimitor = "\n";
                                            string[] StatObj = TagControl.lblStatName.ToolTip.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);
                                            TagControl.lblStatObj.Text = StatObj[0].ToString();

                                            TagStatisticsObject = StatObj[0].ToString();

                                            SolidColorBrush Brush = new SolidColorBrush();
                                            if (statValuecolor.Name.ToString().Contains('#'))
                                            {
                                                Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(statValuecolor.Name.ToString()));
                                            }
                                            else
                                            {
                                                Brush.Color = System.Windows.Media.Color.FromArgb(statValuecolor.A, statValuecolor.R, statValuecolor.G,
                                                    statValuecolor.B);
                                            }

                                            if (statisticsValue != string.Empty)
                                                TagControl.lblStatValue.Foreground = Brush;

                                            if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                            {
                                                Brush = new SolidColorBrush();
                                                if (backgroundColor.ToString().Contains('#'))
                                                {
                                                    Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(backgroundColor.Name.ToString()));
                                                }
                                                else
                                                {
                                                    Brush.Color = System.Windows.Media.Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B);
                                                }

                                                TagControl.MainGrid.Background = Brush;
                                            }

                                            break;
                                        }
                                    }
                                    else
                                    {
                                        string Dilimitor = "_@";

                                        if (statisticsSource == StatisticsEnum.StatSource.StatServer)
                                            TagControl.lblStatName.ToolTip = statTooltip;
                                        else if (statisticsSource == StatisticsEnum.StatSource.DB)
                                            TagControl.lblStatName.ToolTip = _statTooltip[1].ToString();

                                        TagControl.lblStatName.Text = UnTagButton(TagControl.lblStatName.Text, statisticsValue);
                                        Dilimitor = "\n";
                                        string[] StatObj = TagControl.lblStatName.ToolTip.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);

                                        TagControl.lblStatObj.Text = StatObj[0].ToString();

                                        Dilimitor = "_@";

                                        string taggedStatValue = hashTaggedStats[TagControl.Tag.ToString()].ToString();

                                        string[] splitTagValues = taggedStatValue.Split(new[] { Dilimitor }, StringSplitOptions.None);

                                        if (statisticsValue != string.Empty)
                                            TagControl.lblStatValue.Text = statisticsValue;

                                        SolidColorBrush Brush = new SolidColorBrush();
                                        if (statValuecolor.Name.ToString().Contains('#'))
                                        {
                                            Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(statValuecolor.Name.ToString()));
                                        }
                                        else
                                        {
                                            Brush.Color = System.Windows.Media.Color.FromArgb(statValuecolor.A, statValuecolor.R, statValuecolor.G,
                                                statValuecolor.B);
                                        }

                                        if (statisticsValue != string.Empty)
                                            TagControl.lblStatValue.Foreground = Brush;

                                        if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                        {
                                            Brush = new SolidColorBrush();
                                            if (backgroundColor.ToString().Contains('#'))
                                            {
                                                Brush = (SolidColorBrush)(new BrushConverter().ConvertFrom(backgroundColor.Name.ToString()));
                                            }
                                            else
                                            {
                                                Brush.Color = System.Windows.Media.Color.FromArgb(backgroundColor.A, backgroundColor.R, backgroundColor.G, backgroundColor.B);
                                            }

                                            TagControl.MainGrid.Background = Brush;
                                        }

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                //System.Windows.MessageBox.Show("MainWindowViewModel : UpdateTagValues Method : " + ex.Message);
                logger.Error("MainWindowViewModel : UpdateTagValues Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : UpdateTagValues : Method Exit");
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern short GetAsyncKeyState(Keys key);

        [DllImport("user32.dll", EntryPoint = "GetMenuItemCount")]
        private static extern int GetMenuItemCount(IntPtr hmenu);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string name);

        //Remove Control box items
        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        private static extern IntPtr GetSystemMenu(IntPtr hwnd, int revert);

        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        private static extern int RemoveMenu(IntPtr hmenu, int npos, int wflags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);

        /// <summary>
        /// Adds the menu items.
        /// </summary>
        private void AddMenuItems()
        {
            try
            {
                logger.Debug("MainWindowViewModel : AddMenuItems : Method Entry");
                Settings.GetInstance().GadgetContextMenu = new ContextMenu();

                if ((isShowMyStatistics && isShowCCStatistics) && ((Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString()) || (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString())))
                {

                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("mystatistics"))
                    {
                        if (Settings.GetInstance().DictEnableDisableChannels["mystatistics"] && StatisticsBase.GetInstance().IsMyStats && StatisticsBase.GetInstance().IsCCStats)
                        {
                            System.Windows.Controls.MenuItem item1 = new System.Windows.Controls.MenuItem();
                            item1.Margin = new Thickness(2);
                            TextBlock header1 = new TextBlock();
                            header1.Inlines.Add(new Run("Hide My Statistics"));
                            item1.Header = header1;
                            header1.Tag = "Hide My Statistics";
                            item1.Click += menuItem_Click;
                            Settings.GetInstance().GadgetContextMenu.Items.Add(item1);
                            logger.Info("MainWindowViewModel : AddMenuItems : Menu Item number : " + Settings.GetInstance().GadgetContextMenu.Items.Count.ToString());
                            logger.Info("MainWindowViewModel : AddMenuItems : Menu Item : " + header1.Tag.ToString());

                        }
                    }
                }

                if ((isShowCCStatistics && isShowMyStatistics) && ((Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString()) || (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString())))
                {

                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("ccstatistics"))
                    {
                        if (Settings.GetInstance().DictEnableDisableChannels["ccstatistics"] && StatisticsBase.GetInstance().IsCCStats && StatisticsBase.GetInstance().IsMyStats)
                        {
                            System.Windows.Controls.MenuItem item2 = new System.Windows.Controls.MenuItem();
                            item2.Margin = new Thickness(2);
                            TextBlock header2 = new TextBlock();
                            header2.Inlines.Add(new Run("Hide Contact Center Statistics"));
                            header2.Tag = "Hide Contact Center Statistics";
                            item2.Header = header2;
                            item2.Click += menuItem_Click;
                            Settings.GetInstance().GadgetContextMenu.Items.Add(item2);
                            logger.Info("MainWindowViewModel : AddMenuItems : Menu Item number : " + Settings.GetInstance().GadgetContextMenu.Items.Count.ToString());
                            logger.Info("MainWindowViewModel : AddMenuItems : Menu Item : " + header2.Tag.ToString());
                        }
                    }
                }

                if ((!isShowMyStatistics && isShowCCStatistics) && ((Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString()) || (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString())))
                {
                    System.Windows.Controls.MenuItem item3 = new System.Windows.Controls.MenuItem();
                    item3.Margin = new Thickness(2);
                    TextBlock header3 = new TextBlock();
                    header3.Inlines.Add(new Run("Show My Statistics"));
                    header3.Tag = "Show My Statistics";
                    item3.Header = header3;
                    item3.Click += menuItem_Click;
                    Settings.GetInstance().GadgetContextMenu.Items.Add(item3);
                }
                if ((isShowMyStatistics && !isShowCCStatistics) && ((Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString()) || (Settings.GetInstance().ApplicationType == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString())))
                {
                    System.Windows.Controls.MenuItem item4 = new System.Windows.Controls.MenuItem();
                    item4.Margin = new Thickness(2);
                    TextBlock header4 = new TextBlock();
                    header4.Inlines.Add(new Run("Show Contact Center Statistics"));
                    header4.Tag = "Show Contact Center Statistics";
                    item4.Header = header4;
                    item4.Click += menuItem_Click;
                    Settings.GetInstance().GadgetContextMenu.Items.Add(item4);
                }

                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("ontop"))
                {
                    if (Settings.GetInstance().DictEnableDisableChannels["ontop"])
                    {
                        System.Windows.Controls.MenuItem item5 = new System.Windows.Controls.MenuItem();
                        item5.Margin = new Thickness(2);
                        item5.IsCheckable = true;
                        TextBlock header5 = new TextBlock();
                        header5.Inlines.Add(new Run("Always on Top"));
                        header5.Tag = "Always on Top";
                        item5.Header = header5;
                        item5.Click += menuItem_Click;

                        if (Settings.GetInstance().DictEnableDisableChannels["AlwaysOnTop"])
                        {
                            if (item5 != null)
                                item5.IsChecked = true;

                        }
                        else
                        {
                            if (item5 != null)
                                item5.IsChecked = false;
                        }

                        Settings.GetInstance().GadgetContextMenu.Items.Add(item5);
                        logger.Info("MainWindowViewModel : AddMenuItems : Menu Item number : " + Settings.GetInstance().GadgetContextMenu.Items.Count.ToString());
                        logger.Info("MainWindowViewModel : AddMenuItems : Menu Item : " + header5.Tag.ToString());
                    }
                }

                #region MyStatistics & ContactCenter

                //Commented for AID Named pipe communication, on 12/09/15
                //System.Windows.Controls.MenuItem item6 = new System.Windows.Controls.MenuItem();
                //item6.Margin = new Thickness(2);
                //TextBlock header6 = new TextBlock();
                //header6.Inlines.Add(new Run("Show "));
                //header6.Inlines.Add(new Run("My Statistics on Main Window"));
                //item6.Header = header6;
                //header6.Tag = "My Statistics";
                //item6.Click += menuItem_Click;
                //if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("mystataid"))
                //{
                //    if (Settings.GetInstance().DictEnableDisableChannels["mystataid"])
                //    {
                //        Settings.GetInstance().GadgetContextMenu.Items.Add(item6);
                //        logger.Info("MainWindowViewModel : AddMenuItems : Menu Item number : " + Settings.GetInstance().GadgetContextMenu.Items.Count.ToString());
                //        logger.Info("MainWindowViewModel : AddMenuItems : Menu Item : " + header6.Tag.ToString());
                //    }
                //}

                //System.Windows.Controls.MenuItem item7 = new System.Windows.Controls.MenuItem();
                //item7.Margin = new Thickness(2);
                //TextBlock header7 = new TextBlock();
                //header7.Inlines.Add(new Run("Show "));
                //header7.Inlines.Add(new Run("Contact Center Statistics on Main Window"));
                //item7.Header = header7;
                //header7.Tag = "CC Statistics";
                //item7.Click += menuItem_Click;
                //if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("ccstataid"))
                //{
                //    if (Settings.GetInstance().DictEnableDisableChannels["ccstataid"])
                //    {
                //        Settings.GetInstance().GadgetContextMenu.Items.Add(item7);
                //        logger.Info("MainWindowViewModel : AddMenuItems : Menu Item number : " + Settings.GetInstance().GadgetContextMenu.Items.Count.ToString());
                //        logger.Info("MainWindowViewModel : AddMenuItems : Menu Item : " + header7.Tag.ToString());
                //    }
                //}

                #endregion

                System.Windows.Controls.MenuItem item8 = new System.Windows.Controls.MenuItem();
                item8.Margin = new Thickness(2);
                TextBlock header8 = new TextBlock();
                header8.Inlines.Clear();
                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                {
                    if (Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
                    {
                        header8.Inlines.Add(new Run("Horizontal View"));
                        header8.Tag = "Horizontal";
                    }
                    else
                    {
                        header8.Inlines.Add(new Run("Vertical View"));
                        header8.Tag = "Vertical";
                    }
                }
                item8.Header = header8;
                item8.Click += menuItem_Click;
                Settings.GetInstance().GadgetContextMenu.Items.Add(item8);
                logger.Info("MainWindowViewModel : AddMenuItems : Menu Item number : " + Settings.GetInstance().GadgetContextMenu.Items.Count.ToString());
                logger.Info("MainWindowViewModel : AddMenuItems : Menu Item : " + header8.Tag.ToString());

                System.Windows.Controls.MenuItem item10 = null;
                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("showheader"))
                {
                    if (Settings.GetInstance().DictEnableDisableChannels["showheader"])
                    {
                        item10 = new System.Windows.Controls.MenuItem();
                        item10.IsCheckable = true;
                        item10.Margin = new Thickness(2);
                        TextBlock header10 = new TextBlock();
                        header10.Inlines.Clear();
                        header10.Inlines.Add(new Run("Show Header"));
                        header10.Tag = "Show Header";
                        item10.Header = header10;
                        item10.Click += menuItem_Click;

                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("isheaderenabled"))
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels["isheaderenabled"])
                            {
                                if (item10 != null)
                                    item10.IsChecked = true;
                                isShowHeader = true;
                                defaultHeader = true;
                            }
                            else
                            {
                                if (item10 != null)
                                    item10.IsChecked = false;
                                isShowHeader = false;
                                isHeaderClicked = true;
                                defaultHeader = true;
                                HeaderValues();
                            }
                        }
                        else
                        {
                            if (item10 != null)
                            {
                                item10.IsChecked = true;
                                logger.Info("Show header is true");
                            }
                            isShowHeader = true;
                            defaultHeader = true;
                        }
                        Settings.GetInstance().GadgetContextMenu.Items.Add(item10);
                        logger.Info("MainWindowViewModel : AddMenuItems : Menu Item number : " + Settings.GetInstance().GadgetContextMenu.Items.Count.ToString());
                        logger.Info("MainWindowViewModel : AddMenuItems : Menu Item : " + header10.Tag.ToString());
                    }
                }

                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("closegadget"))
                {
                    if (Settings.GetInstance().DictEnableDisableChannels["closegadget"])
                    {
                        System.Windows.Controls.MenuItem item11 = new System.Windows.Controls.MenuItem();
                        item11.Margin = new Thickness(2);
                        TextBlock header11 = new TextBlock();
                        header11.Inlines.Clear();

                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("notificationclose"))
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels["notificationclose"])
                            {
                                header11.Inlines.Add(new Run("Minimize StatTicker Gadget"));
                            }
                            else
                            {
                                header11.Inlines.Add(new Run("Close Gadget"));
                            }
                        }
                        else
                        {
                            header11.Inlines.Add(new Run("Close Gadget"));
                        }

                        header11.Tag = "Close";
                        item11.Header = header11;
                        item11.Click += menuItem_Click;

                        Settings.GetInstance().GadgetContextMenu.Items.Add(item11);
                    }
                }

                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("thresholdnotification"))
                {
                    if (Settings.GetInstance().DictEnableDisableChannels["thresholdnotification"])
                    {
                        System.Windows.Controls.MenuItem item9 = new System.Windows.Controls.MenuItem();
                        TextBlock header9 = new TextBlock();
                        header9.Inlines.Add(new Run(" Alert"));
                        header9.Tag = "Alert";
                        item9.Header = header9;
                        System.Windows.Controls.MenuItem NotificationSubitem = new System.Windows.Controls.MenuItem();
                        if (isNotifierAudioSelected)
                            NotificationSubitem.IsChecked = true;
                        NotificationSubitem.IsCheckable = true;
                        NotificationSubitem.Header = "Music";
                        NotificationSubitem.Click += new RoutedEventHandler(NotificationSubitem_Click);
                        item9.Items.Add(NotificationSubitem);
                        NotificationSubitem = new System.Windows.Controls.MenuItem();
                        if (isNotiferPopupSelected)
                            NotificationSubitem.IsChecked = true;
                        NotificationSubitem.IsCheckable = true;
                        NotificationSubitem.Header = "Popup";
                        NotificationSubitem.Click += new RoutedEventHandler(NotificationSubitem_Click);
                        item9.Items.Add(NotificationSubitem);

                        Settings.GetInstance().GadgetContextMenu.Items.Add(item9);
                    }
                }

                try
                {
                    System.Windows.Controls.MenuItem item12 = new System.Windows.Controls.MenuItem();
                    item12.Margin = new Thickness(2);
                    TextBlock header12 = new TextBlock();
                    header12.Inlines.Add(new Run("Save Tags"));
                    header12.Tag = "Save Tags";
                    item12.Header = header12;
                    item12.Click += SaveTag_Click;

                    Settings.GetInstance().GadgetContextMenu.Items.Add(item12);
                }
                catch (Exception ex)
                {

                    logger.Warn("Error occurred while adding save tag as : " + ex.Message);
                }

                //System.Windows.Controls.MenuItem item12 = new System.Windows.Controls.MenuItem();
                //item12.Margin = new Thickness(2);
                //TextBlock header12 = new TextBlock();
                //header12.Inlines.Add(new Run("Queue Configuration"));
                //item12.Header = header12;
                //header12.Tag = "Queue Configuration";
                //item12.Click += menuItem_Click;
                //Settings.GetInstance().GadgetContextMenu.Items.Add(item12);
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : AddMenuItems Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : AddMenuItems : Method Exit");
            }
        }

        /// <summary>   
        /// Adds the new tags.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void AddNewTags(object obj)
        {
            string statName = string.Empty;
            string splittagno = string.Empty;
            bool istagged = false;
            string tagName = string.Empty;
            string taggingStat = string.Empty;
            string statisticName = string.Empty;
            string[] statDetails = new string[] { };
            string[] splitedtagno;
            string[] keys;
            string tags = string.Empty;

            try
            {
                logger.Debug("MainWindowViewModel : AddNewTags : Method Entry");

                if (obj != null)
                {
                    statName = obj.ToString();
                    tagName = "tag_" + TagNo;

                    taggingStat = Settings.GetInstance().DictAllStats[statName].ToString();

                    foreach (string stats in hashTaggedStats.Values)
                    {
                        string Dilimitor = "_@";
                        statDetails = stats.Split(new[] { Dilimitor }, StringSplitOptions.None);

                        if (statDetails[0].ToString() == taggingStat)
                        {
                            if (statDetails[1].ToString() == StatNameToolTip)
                                istagged = true;
                        }
                    }

                    if (!istagged)
                    {

                        if (removedNo.Count != 0)
                        {
                            tagName = removedNo.Dequeue();
                            splitedtagno = tagName.Split('_');
                            splittagno = splitedtagno[1];
                        }

                        #region AddTags

                        userTagControl = new TagGadgetControl();

                        userTagControl.Tag = tagName;

                        userTagControl.Name = tagName;

                        userTagControl.lblStatName.Uid = statName;

                        userTagControl.lblStatName.ToolTip = StatNameToolTip;

                        userTagControl.lblStatValue.Text = userControl.lblMainValue.Text;

                        userTagControl.lblStatName.Text = UnTagButton(userControl.lblMainStatName.Text.ToString(), userControl.lblMainValue.Text);

                        string Dilimitor = "\n";
                        string[] StatObj = userTagControl.lblStatName.ToolTip.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);

                        userTagControl.lblStatObj.Text = StatObj[0].ToString();
                        TagStatisticsObject = StatObj[0].ToString();

                        userTagControl.lblStatName.Tag = userControl.lblMainStatName.Tag;

                        userTagControl.MainGrid.Background = userControl.GridStatistics.Background;

                        userTagControl.lblStatValue.Foreground = userControl.lblMainValue.Foreground;

                        hashTaggedStats.Add(tagName, taggingStat + "_@" + userTagControl.lblStatName.ToolTip.ToString());
                        tempTagheight = TagGadgetHeight;

                        if (Settings.GetInstance().HshAllTagList != null)
                        {
                            if (!Settings.GetInstance().HshAllTagList.ContainsKey(userTagControl.Name))
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
                                    {
                                        logger.Warn("MainWindowViewModel : AddNewTags : TagVertical : True");
                                        NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth) - 1;

                                        NumberOfTags_Vertically =
                                            (int)
                                                (Screen.PrimaryScreen.WorkingArea.Height /
                                                 TagGadgetHeight);

                                        Tag_Width = (int)GadgetWidth;

                                        WindowHorizontalWidth = (Tag_Width * TagNo) + (int)GadgetWidth;
                                        WindowVerticalWidth = Tag_Width;

                                        WindowHorizontalHeight = (int)userControl.MainBorder.ActualHeight;

                                        WindowVerticalHeight = (int)userControl.MainBorder.Height + (((int)userTagControl.MainBorder.Height) * TagNo);

                                        if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                        {
                                            WrapHorizontalWidth = (Tag_Width * TagNo);
                                        }
                                        else
                                        {
                                            WrapHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
                                        }

                                        WrapVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;

                                        if (TagNo > NumberOfTags_Horizontally)
                                            TotalTagsExceeded++;

                                        if (TagNo == 1)
                                        {
                                            userTagControl.MainGrid.Margin = new Thickness(2, 2, 2, 2);
                                        }
                                        else
                                        {
                                            userTagControl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                        }

                                    }
                                    else
                                    {
                                        logger.Warn("MainWindowViewModel : AddNewTags : TagVertical : False");
                                        NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth) - 1;

                                        NumberOfTags_Vertically =
                                            (int)
                                                (Screen.PrimaryScreen.WorkingArea.Height /
                                                 TagGadgetHeight);

                                        Tag_Width = (int)GadgetWidth;

                                        WindowVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;
                                        WindowHorizontalHeight = (int)userControl.MainBorder.Height;

                                        WindowVerticalWidth = Tag_Width;
                                        WindowHorizontalWidth = (Tag_Width * TagNo) + (int)GadgetWidth;

                                        if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                        {
                                            WrapWidth = Tag_Width * TagNo;
                                            if ((WrapHorizontalWidth + GadgetWidth) < (Screen.PrimaryScreen.WorkingArea.Width))
                                                WrapHorizontalWidth = (Tag_Width * TagNo);
                                        }
                                        else
                                        {
                                            WrapWidth = Tag_Width * NumberOfTags_Horizontally;
                                            WrapHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
                                        }
                                        WrapVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;

                                        if (TagNo > NumberOfTags_Horizontally)
                                        {
                                            TotalTagsExceeded++;
                                            userTagControl.MainGrid.Margin = new Thickness(0, 0, 2, 2);
                                        }
                                        else
                                        {
                                            userTagControl.MainGrid.Margin = new Thickness(0, 2, 2, 2);
                                        }
                                    }
                                }

                                if (TagNo <= (NumberOfTags_Horizontally * 2) && !Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
                                {
                                    MyTagControlCollection.Add(userTagControl);
                                    Wrap2Visibility = Visibility.Collapsed;
                                    isThirdRowActive = false;
                                }
                                else
                                {
                                    if (!isThirdRowActive)
                                    {
                                        Wrap2Visibility = Visibility.Visible;
                                        Wrap2Margin = new Thickness(0, 0, 0, 0);
                                        Wrap2Rows = ++Wrap2Rows;
                                    }

                                    MyTagControlCollection2.Add(userTagControl);
                                    Wrap2Collection.Add(userTagControl);
                                    isThirdRowActive = true;

                                    if (((float)MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1) >= 1) && ((float)MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1) > Wrap2Rows))
                                        Wrap2Rows = ++Wrap2Rows;

                                }

                                if (MyTagControlTempCollection.Count != 0)
                                {
                                    bool isadded = false;

                                    foreach (TagGadgetControl temptags in MyTagControlTempCollection)
                                    {
                                        if (temptags.lblStatName.ToolTip != null)
                                        {
                                            if (temptags.lblStatName.Tag.ToString() == userTagControl.lblStatName.Tag.ToString())
                                            {
                                                isadded = true;
                                            }
                                        }
                                    }

                                    if (!isadded)
                                    {
                                        MyTagControlTempCollection.Add(userTagControl);
                                    }
                                }
                                else
                                {
                                    MyTagControlTempCollection.Add(userTagControl);
                                }

                                HeaderValues();
                                TagOrientation();

                                ((MainWindowViewModel)userTagControl.DataContext).GadgetTagValue = tagName;

                                string Dilimitor1 = "\n";
                                string[] objname = userTagControl.lblStatName.ToolTip.ToString().Split(new[] { Dilimitor1 }, StringSplitOptions.None);

                                if (!Settings.GetInstance().HshAllTagNames.ContainsKey(objname[0] + "@" + taggingStat))
                                {
                                    Settings.GetInstance().HshAllTagNames.Add(objname[0] + "@" + taggingStat, userTagControl);
                                }
                                if (!hashTagDetails.ContainsKey(statName))
                                {
                                    if (splittagno == string.Empty)
                                        hashTagDetails.Add(statName + "_@" + TagNo, tagName);
                                    else
                                        hashTagDetails.Add(statName + "_@" + splittagno, tagName);
                                }
                                Settings.GetInstance().HshAllTagList.Add(userTagControl.Name, userTagControl);

                            }

                        }

                        #endregion
                    }
                    else
                    {
                        TagNo--;
                    }

                    foreach (string key in hashTagDetails.Keys)
                    {
                        string Dilimitor = "_@";
                        keys = key.Split(new[] { Dilimitor }, StringSplitOptions.None);

                        if (splittagno == string.Empty)
                            tags = TagNo.ToString();
                        else
                            tags = splittagno;

                        if (keys[1] == tags)
                        {
                            string[] statobj = null;
                            if (userTagControl.lblStatName.ToolTip != null)
                            {
                                if ((Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString()) || (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString()))
                                {
                                    statobj = userTagControl.lblStatName.ToolTip.ToString().Split('\n');
                                    listTaggedStatObjects.Add(statobj[0]);

                                    if (!DicTemp.ContainsKey(hashTagDetails[key] + "@" + statobj[0]))
                                    {
                                        //System.Windows.MessageBox.Show("Add" + hashTagDetails[key] + "@" + statobj[0]);
                                        DicTemp.Add(hashTagDetails[key] + "@" + statobj[0], keys[0]);
                                    }

                                }

                                if ((Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString()) || (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString()))
                                {
                                    statobj = userTagControl.lblStatName.ToolTip.ToString().Split('\n');
                                    listTaggedStatObjects.Add(statobj[0]);

                                    if (!DicTemp.ContainsKey(hashTagDetails[key] + "@" + statobj[0]))
                                    {
                                        //System.Windows.MessageBox.Show("Add" + hashTagDetails[key] + "@" + statobj[0]);
                                        DicTemp.Add(hashTagDetails[key] + "@" + statobj[0], keys[0]);
                                    }

                                }
                            }
                            statobj = null;
                            break;
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : AddNewTags Method  : " + ex.Message);
            }
            finally
            {
                statName = null;
                splittagno = null;
                tagName = null;
                taggingStat = null;
                statisticName = null;
                GC.Collect();
                logger.Debug("MainWindowViewModel : AddNewTags : Method Exit");
            }
        }

        /// <summary>
        /// Adds the tag.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void AddTag(object obj)
        {
            try
            {
                logger.Debug("MainWindowViewModel : AddTag : Method Entry");

                if (obj != null)
                {
                    TagNo++;

                    if (!Settings.GetInstance().DictEnableDisableChannels["tagvertical"] || (TagNo < NumberOfTags_Vertically))
                    {
                        AddNewTags(obj);
                    }
                    else
                    {
                        TagNo--;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : AddTag Method  : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : AddTag : Method Exit");
            }
        }

        /// <summary>
        /// Captures the key.
        /// </summary>
        /// <param name="nCode">The n code.</param>
        /// <param name="wp">The wp.</param>
        /// <param name="lp">The lp.</param>
        /// <returns></returns>
        private IntPtr captureKey(int nCode, IntPtr wp, IntPtr lp)
        {
            KBDLLHOOKSTRUCT objKeyInfo;
            try
            {

                if (nCode >= 0)
                {
                    objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));
                    if (objKeyInfo.key == Keys.D)
                    {
                        if (previousKey.Equals(Keys.RWin) || previousKey.Equals(Keys.LWin))
                        {
                            // code to minimize the application in the notification area
                            previousKey = Keys.RWin;
                            if (IsTopMost)
                            {
                                isMinimized = false;
                                WInState = WindowState.Normal;
                            }
                            else
                            {
                                isMinimized = true;
                                WInState = WindowState.Minimized;
                            }
                            previousKey = Keys.A;
                        }
                    }
                    else
                    {
                        previousKey = objKeyInfo.key;
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : captureKey : " + ex.Message);
            }
            finally
            {
                GC.Collect();
            }
            return CallNextHookEx(ptrHook, nCode, wp, lp);
        }

        private string DicAsString(Dictionary<string, string> DicTemp)
        {
            string temp = string.Empty;
            foreach (var item in DicTemp)
            {
                temp += item.Key + " , " + item.Value + "\n";
            }
            return temp;
        }

        /// <summary>
        /// Handles the Tick event of the DispTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void DispTimerTick(object sender, EventArgs e)
        {
            try
            {
                logger.Debug("MainWindowViewModel : DispTimerTick : Method Entry");
                StackVisibility = Visibility.Visible;
                string statname = string.Empty;
                UnPausedStatTimer.Stop();

                if (StatisticsBase.GetInstance().StatSource == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.DB.ToString() || ((StatisticsBase.GetInstance().StatSource == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString()) && !StatisticsBase.GetInstance().isCMEAuthentication))
                {
                    StatDisplayTimer.Interval = new TimeSpan(0, 0, Convert.ToInt32(Settings.GetInstance().DisplayTime));
                }
                else if (StatisticsBase.GetInstance().StatSource == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString() || StatisticsBase.GetInstance().StatSource == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.All.ToString())
                {
                    StatDisplayTimer.Interval = new TimeSpan(0, 0, StatisticsBase.GetInstance().statsIntervalTime);
                }

                logger.Info("MainWindowViewModel : DispTimerTick : Interval time : " + StatDisplayTimer.Interval.ToString());

                if (!isEnableMessageBox)
                    isEnableMessageBox = true;

                if (beginInvoke)
                {
                    StatDisplayTimer.Tag = "False";
                    beginInvoke = false;
                }

                if (!isPrevious && !isNext)
                {
                    Settings.GetInstance().StatCount++;
                    PausedStatTimer1.Stop();
                }
                else if (isNext)
                {
                    if (Settings.GetInstance().StatCount == Settings.GetInstance().DicStatInfo.Count - 1)
                        Settings.GetInstance().StatCount = 0;
                    else
                        Settings.GetInstance().StatCount++;
                }
                else if (isPrevious)
                {
                    if (Settings.GetInstance().StatCount == 0)
                        Settings.GetInstance().StatCount = Settings.GetInstance().DicStatInfo.Count - 1;
                    else
                        Settings.GetInstance().StatCount--;
                }

                if (Settings.GetInstance().StatCount == Settings.GetInstance().DicStatInfo.Count)
                    Settings.GetInstance().StatCount = 0;

                if (Settings.GetInstance().DicStatInfo.Count != 0)
                {
                    KeyValuePair<string, StatisticsInfo> kvpTemp;

                    kvpTemp = Settings.GetInstance().DicStatInfo.ElementAt(Settings.GetInstance().StatCount);
                    StatisticsInfo statInfo = kvpTemp.Value;

                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                    {
                        if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
                        {
                            UnTagVisibility = Visibility.Visible;
                            TagStatValueMargin = new Thickness(0, 0, 2, 0);
                            VBStatName = 125;
                            LblStatName = 119;
                            TagStatWidth = 88;
                        }
                        else
                        {
                            UnTagVisibility = Visibility.Collapsed;
                            TagStatValueMargin = new Thickness(15, 0, 2, 0);
                            VBStatName = 125;
                            LblStatName = 123;
                            TagStatWidth = 92;
                        }
                    }

                    if (!statInfo.BackgroundColors.IsEmpty || Settings.GetInstance().ApplicationType != StatisticsEnum.StatSource.All.ToString())
                    {

                        #region Show MyStatistics only

                        if (isShowMyStatistics && !isShowCCStatistics)
                        {

                            logger.Debug("MainWindowViewModel : DispTimerTick : Show MyStatistics Only");

                            if (statInfo.StatType.ToString() != "Queue" && statInfo.StatType.ToString() != "GroupPlaces" || statInfo.StatType.ToString() == "")
                            {
                                logger.Info("DispTimerTick Method : Statistics Display name : " + statInfo.StatTag + "," + "Value :" + statInfo.StatValue);

                                StatValue = statInfo.StatValue;
                                StatName = statInfo.StatName;
                                TagValue = statInfo.RefId.ToString();
                                StatNameToolTip = statInfo.StatToolTip;

                                if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                {
                                    GridBackgroundColor = new SolidColorBrush();
                                    if (statInfo.BackgroundColors.ToString().Contains('#'))
                                    {
                                        GridBackgroundColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(statInfo.BackgroundColors.Name.ToString()));
                                    }
                                    else
                                    {
                                        GridBackgroundColor.Color = System.Windows.Media.Color.FromArgb(statInfo.BackgroundColors.A, statInfo.BackgroundColors.R, statInfo.BackgroundColors.G, statInfo.BackgroundColors.B);
                                    }
                                }
                                else
                                {
                                    GridBackgroundColor = BackgroundColor;
                                }

                                string Dilimitor = "\n";
                                string[] StatObj = statInfo.StatToolTip.Split(new[] { Dilimitor }, StringSplitOptions.None);

                                MainStatObject = StatObj[0].ToString();

                                StatAlignment(StatName);

                                isNext = false;
                                isPrevious = false;
                                Settings.GetInstance().CurrStatRefId = PausedTag = statInfo.RefId;

                                StatValueColor = new SolidColorBrush();
                                if (statInfo.StatColor.ToString().Contains('#'))
                                {
                                    StatValueColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(statInfo.StatColor.Name.ToString()));
                                }
                                else
                                {
                                    StatValueColor.Color = System.Windows.Media.Color.FromArgb(statInfo.StatColor.A, statInfo.StatColor.R, statInfo.StatColor.G, statInfo.StatColor.B);
                                }

                                if (statInfo.StatType == StatisticObjectType.Queue.ToString() || statInfo.StatType == StatisticObjectType.GroupPlaces.ToString())
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton"))
                                    {
                                        if (Settings.GetInstance().DictEnableDisableChannels["menubutton"])
                                        {
                                            StatType = "Contact Center Statistics";
                                        }
                                        else
                                        {
                                            StatType = "Contact Center Statistics";
                                        }
                                        StatTypeTooltip = "Contact Center Statistics";
                                    }

                                }
                                else
                                {
                                    StatType = "My Statistics";
                                    StatTypeTooltip = "My Statistics";
                                }
                            }
                            else
                            {
                                StatDisplayTimer.Stop();
                                StatDisplayTimer.Interval = new TimeSpan(0, 0, 0);
                                StatDisplayTimer.Start();
                            }

                            ShowThresholdBreach(TagValue, StatName, statInfo.IsLevelTwo);
                        }

                        #endregion

                        #region Show CCStatistics only

                        else if (isShowCCStatistics && !isShowMyStatistics)
                        {
                            logger.Debug("MainWindowViewModel : DispTimerTick : Show Contact Center Statistics Only");

                            if (statInfo.StatType.ToString() == "Queue" || statInfo.StatType.ToString() == "GroupPlaces" || statInfo.StatType.ToString() == "")
                            {
                                logger.Info("DispTimerTick Method : Statistics Display name : " + statInfo.StatTag + "," + "Value :" + statInfo.StatValue);

                                StatValue = statInfo.StatValue;
                                StatName = statInfo.StatName;
                                TagValue = statInfo.RefId.ToString();
                                StatNameToolTip = statInfo.StatToolTip;

                                if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                {
                                    GridBackgroundColor = new SolidColorBrush();
                                    if (statInfo.BackgroundColors.ToString().Contains('#'))
                                    {
                                        GridBackgroundColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(statInfo.BackgroundColors.Name.ToString()));
                                    }
                                    else
                                    {
                                        GridBackgroundColor.Color = System.Windows.Media.Color.FromArgb(statInfo.BackgroundColors.A, statInfo.BackgroundColors.R, statInfo.BackgroundColors.G, statInfo.BackgroundColors.B);
                                    }
                                }
                                else
                                {
                                    GridBackgroundColor = BackgroundColor;
                                }

                                string Dilimitor = "\n";
                                string[] StatObj = statInfo.StatToolTip.Split(new[] { Dilimitor }, StringSplitOptions.None);

                                MainStatObject = StatObj[0].ToString();

                                StatAlignment(StatName);

                                isNext = false;
                                isPrevious = false;
                                Settings.GetInstance().CurrStatRefId = PausedTag = statInfo.RefId;

                                StatValueColor = new SolidColorBrush();
                                if (statInfo.StatColor.ToString().Contains('#'))
                                {
                                    StatValueColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(statInfo.StatColor.Name.ToString()));
                                }
                                else
                                {
                                    StatValueColor.Color = System.Windows.Media.Color.FromArgb(statInfo.StatColor.A, statInfo.StatColor.R, statInfo.StatColor.G, statInfo.StatColor.B);
                                }

                                if (statInfo.StatType == StatisticObjectType.Queue.ToString() || statInfo.StatType == StatisticObjectType.GroupPlaces.ToString())
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton"))
                                    {
                                        if (Settings.GetInstance().DictEnableDisableChannels["menubutton"])
                                        {
                                            StatType = "Contact Center Statistics";
                                        }
                                        else
                                        {
                                            StatType = "Contact Center Statistics";
                                        }
                                        StatTypeTooltip = "Contact Center Statistics";
                                    }
                                }
                                else
                                {

                                    StatType = "My Statistics";
                                    StatTypeTooltip = "My Statistics";
                                }

                            }
                            else
                            {
                                StatDisplayTimer.Stop();
                                StatDisplayTimer.Interval = new TimeSpan(0, 0, 0);
                                StatDisplayTimer.Start();
                            }

                            ShowThresholdBreach(TagValue, StatName, statInfo.IsLevelTwo);

                        }

                        #endregion

                        #region Show MyStatistics & CCStatistics

                        else if (isShowCCStatistics && isShowMyStatistics)
                        {
                            logger.Debug("MainWindowViewModel : DispTimerTick : Show Both MyStatistics & Contact Center Statistics");

                            logger.Info("DispTimerTick Method : Statistics Display name : " + statInfo.StatTag + "," + "Value :" + statInfo.StatValue);

                            StatValue = statInfo.StatValue;
                            StatName = statInfo.StatName;
                            TagValue = statInfo.RefId.ToString();
                            StatNameToolTip = statInfo.StatToolTip;

                            if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                            {
                                GridBackgroundColor = new SolidColorBrush();
                                if (statInfo.BackgroundColors.ToString().Contains('#'))
                                {
                                    GridBackgroundColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(statInfo.BackgroundColors.Name.ToString()));
                                }
                                else
                                {
                                    GridBackgroundColor.Color = System.Windows.Media.Color.FromArgb(statInfo.BackgroundColors.A, statInfo.BackgroundColors.R, statInfo.BackgroundColors.G, statInfo.BackgroundColors.B);
                                }
                            }
                            else
                            {
                                GridBackgroundColor = BackgroundColor;
                            }

                            string Dilimitor = "\n";
                            string[] StatObj = statInfo.StatToolTip.Split(new[] { Dilimitor }, StringSplitOptions.None);

                            MainStatObject = StatObj[0].ToString();

                            StatAlignment(StatName);

                            isNext = false;
                            isPrevious = false;
                            Settings.GetInstance().CurrStatRefId = PausedTag = statInfo.RefId;

                            StatValueColor = new SolidColorBrush();
                            if (statInfo.StatColor.ToString().Contains('#'))
                            {
                                StatValueColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(statInfo.StatColor.Name.ToString()));
                            }
                            else
                            {
                                StatValueColor.Color = System.Windows.Media.Color.FromArgb(statInfo.StatColor.A, statInfo.StatColor.R, statInfo.StatColor.G, statInfo.StatColor.B);
                            }

                            if (statInfo.StatType == StatisticObjectType.Queue.ToString() || statInfo.StatType == StatisticObjectType.GroupPlaces.ToString())
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("menubutton"))
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels["menubutton"])
                                    {
                                        StatType = "Contact Center Statistics";
                                    }
                                    else
                                    {
                                        StatType = "Contact Center Statistics";
                                    }
                                    StatTypeTooltip = "Contact Center Statistics";
                                }
                            }
                            else
                            {
                                StatType = "My Statistics";
                                StatTypeTooltip = "My Statistics";
                            }

                            ShowThresholdBreach(TagValue, StatName, statInfo.IsLevelTwo);
                        }

                        #endregion

                        #region PlayPause Statistics

                        if (PlayPause == "Pause")
                        {
                            if (isNext || isPrevious)
                            {
                                if (StatisticsBase.GetInstance().StatSource == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.DB.ToString())
                                {
                                    StatDisplayTimer.Interval = new TimeSpan(0, 0, StatisticsBase.GetInstance().statsIntervalTime);
                                }
                                else if (StatisticsBase.GetInstance().StatSource == Pointel.Statistics.Core.Utility.StatisticsEnum.StatSource.StatServer.ToString())
                                {
                                    StatDisplayTimer.Interval = new TimeSpan(0, 0, Settings.GetInstance().DisplayTime);
                                }
                                StatDisplayTimer.Start();
                            }
                            else
                            {
                                StatDisplayTimer.Stop();
                                PausedStatTimer1.Start();
                            }
                        }
                        else
                        {
                            UnPausedStatTimer.Interval = new TimeSpan(0, 0, 1);
                            UnPausedStatTimer.Tick += new EventHandler(UnPausedStatTimer_Tick);
                            UnPausedStatTimer.Start();
                        }

                        #endregion

                        SizeF size;
                        int TotalLength = 0;

                        #region StatName size changed dynamically

                        logger.Info("DispTimerTick Method : Statistics Display name : StatType size changed dynamically ");

                        SizeF valueSize;
                        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1)))
                        {
                            size = graphics.MeasureString(StatName, new Font("Segoe UI", 9, System.Drawing.FontStyle.Regular, GraphicsUnit.Point));
                            valueSize = graphics.MeasureString(StatValue, new Font("Segoe UI", 18, System.Drawing.FontStyle.Bold, GraphicsUnit.Point));
                        }

                        TotalLength = 0;

                        if (((int)GadgetWidth) < (int)(size.Width + valueSize.Width + userControl.GridStatistics.ColumnDefinitions[2].ActualWidth))
                        {
                            TotalLength = (int)(size.Width + valueSize.Width + userControl.GridStatistics.ColumnDefinitions[2].ActualWidth) - (int)(GadgetWidth);
                        }

                        if (TotalLength != 0)
                        {
                            string tempStatType = string.Empty;
                            tempStatType = StatName.Substring(0, StatName.Length - (TotalLength / 8));
                            if (tempStatType != StatName)
                                StatName = tempStatType + "..";
                        }

                        #endregion

                        #region StatType size changed dynamically

                        logger.Info("DispTimerTick Method : Statistics Display name : StatType size changed dynamically ");

                        using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(new Bitmap(1, 1)))
                        {
                            size = graphics.MeasureString(StatType, new Font("Segoe UI", 11, System.Drawing.FontStyle.Regular, GraphicsUnit.Point));
                        }

                        if (size.Width > userControl.GridTitle.ColumnDefinitions[0].ActualWidth)
                            TotalLength = Convert.ToInt32(size.Width) - Convert.ToInt32(userControl.GridTitle.ColumnDefinitions[0].ActualWidth);

                        if (TotalLength != 0)
                        {
                            string tempStatType = string.Empty;
                            if (StatType.Length >= (Convert.ToInt32(((userControl.GridTitle.ColumnDefinitions[0].ActualWidth + 30) / 8))))
                            {
                                tempStatType = StatType.Substring(0, Convert.ToInt32(((userControl.GridTitle.ColumnDefinitions[0].ActualWidth + 30) / 8)));
                                if (tempStatType != StatType)
                                    StatType = tempStatType + "..";
                            }
                        }

                        #endregion

                        HeaderValues();

                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : DispTimerTick Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : DispTimerTick : Method Exit");
            }
        }

        /// <summary>
        /// Drag_s the move.
        /// </summary>
        private void DragMove()
        {
            try
            {
                logger.Debug("MainWindowViewModel : DragMove : Method Entry");

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.Title == "StatGadget")
                    {
                        window.DragMove();

                        if (window.WindowState == WindowState.Maximized)
                        {
                            window.WindowState = WindowState.Normal;
                        }
                        AppLeft = window.Left;
                        AppTop = window.Top;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : DragMove : " + ex.Message);
            }
            finally
            {
                logger.Debug("MainWindowViewModel : DragMove : Method Exit");
                GC.Collect();
            }
        }

        /// <summary>
        /// Formats the name of the tagged stat.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <returns></returns>
        private string formatStatName(string statisticName, int maxLength, int subStr)
        {
            try
            {
                logger.Debug("MainWindowViewModel : FormattedTaggedStatName : Method Entry");
                if (statisticName.Length > maxLength)
                {
                    statisticName = statisticName.Substring(0, subStr);
                    statisticName = statisticName + "..";
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : FormatTaggedStatName Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : FormattedTaggedStatName : Method Exit");
            }
            return statisticName;
        }

        /// <summary>
        /// Formats the name of the stat.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <returns></returns>
        private string formattedStatName(string statisticName)
        {
            try
            {
                logger.Debug("MainWindowViewModel : FormattedStatName : Method Entry");

                if (statisticName.Length > 26)
                {
                    statisticName = statisticName.Substring(0, 24);
                    statisticName = statisticName + "..";
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : FormattedStatName Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : FormattedStatName : Method Exit");
            }

            return statisticName;
        }

        /// <summary>
        /// Growls the notifications_ notifier closed.
        /// </summary>
        /// <param name="notiferId">The notifer unique identifier.</param>
        void growlNotifications_NotifierClosed(string notiferId)
        {
            try
            {
                if (LstNotifierIds.Contains(notiferId))
                    LstNotifierIds.Remove(notiferId);
            }
            catch (Exception generalException)
            {

            }
        }

        /// <summary>
        /// HWNDs the message hook.
        /// </summary>
        /// <param name="hWnd">The h WND.</param>
        /// <param name="msg">The MSG.</param>
        /// <param name="wParam">The w param.</param>
        /// <param name="lParam">The l param.</param>
        /// <param name="bHandled">if set to <c>true</c> [b handled].</param>
        /// <returns></returns>
        private IntPtr HwndMessageHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool bHandled)
        {
            WIN32Rectangle rectangle;
            try
            {

                #region Taskbar Height

                if (taskbarHeight == (int)SystemParameters.PrimaryScreenHeight - Screen.PrimaryScreen.WorkingArea.Height)
                {
                    taskbarHeight = (int)SystemParameters.PrimaryScreenHeight - Screen.PrimaryScreen.WorkingArea.Height;
                    taskbarWidth = (int)SystemParameters.PrimaryScreenWidth - Screen.PrimaryScreen.WorkingArea.Width;

                    primwidh = Screen.PrimaryScreen.WorkingArea.Width;
                    priheight = Screen.PrimaryScreen.WorkingArea.Height;
                    rightMargin = primwidh - Main_Window.Width;
                    bottomMargin = priheight - Main_Window.Height;
                }

                #endregion

                switch (msg)
                {
                    case WM_SIZING:
                    case WM_MOVING:
                        {

                            if (!StatisticsBase.GetInstance().IsSystemDraggable)
                            {
                                rectangle = (WIN32Rectangle)Marshal.PtrToStructure(lParam, typeof(WIN32Rectangle));
                                if (rectangle.Left < 0)
                                {
                                    rectangle.Left = 0;
                                    rectangle.Right = windowwidth;

                                    bHandled = true;
                                }
                                if (rectangle.Top < 0)
                                {
                                    rectangle.Top = 0;
                                    rectangle.Bottom = windowheight;

                                    bHandled = true;
                                }
                                if (rectangle.Right > primwidh)
                                {
                                    rectangle.Left = (int)rightMargin;
                                    rectangle.Right = (int)primwidh;

                                    bHandled = true;
                                }
                                if (rectangle.Bottom > priheight)
                                {
                                    rectangle.Top = (int)bottomMargin;
                                    rectangle.Bottom = (int)priheight;

                                    bHandled = true;
                                }

                                if (bHandled)
                                {
                                    Marshal.StructureToPtr(rectangle, lParam, true);
                                }
                            }
                        }
                        break;
                }

            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : HwndMessageHook : " + ex.Message);
            }
            finally
            {
                GC.Collect();
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Handles the MediaEnded event of the mediaPlayer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void mediaPlayer_MediaEnded(object sender, EventArgs e)
        {
            Settings.GetInstance().isThresholdNotifierEnded = true;
            Mediatimer.Stop();
        }

        /// <summary>
        /// Handles the Tick event of the Mediatimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        void Mediatimer_Tick(object sender, EventArgs e)
        {
            try
            {
                string[] duration = mediaPlayer.Position.ToString(@"mm\:ss").Split(':');
                if (Convert.ToInt32(duration[1].ToString()) > StatisticsBase.GetInstance().AlertAudioDuration)
                {
                    mediaPlayer.Stop();
                    Settings.GetInstance().isThresholdNotifierEnded = true;
                }
            }
            catch (Exception GeneralException)
            {

            }
        }

        /// <summary>
        /// Menus the BTN click.
        /// </summary>
        private void MenuBtnClick()
        {
            try
            {
                logger.Debug("MainWindowViewModel : MenuBtnClick : Method Entry");
                BtnContextMenu = Settings.GetInstance().GadgetContextMenu;

                BtnContextMenu.IsOpen = true;
                BtnContextMenu.Focus();
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : MenuBtnClick Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : MenuBtnClick : Method Exit");
            }
        }

        /// <summary>
        /// Handles the Click event of the menuItem control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="RoutedEventArgs" /> instance containing the event data.</param>
        void menuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.Debug("MainWindowViewModel : MenuItem_Click : Method Entry");
                System.Windows.Controls.MenuItem objItem = sender as System.Windows.Controls.MenuItem;
                Dictionary<string, string> DictTaggedStats = new Dictionary<string, string>();
                if (objItem != null)
                {
                    object selectedMenu = objItem.Header;
                    if (selectedMenu is TextBlock)
                    {
                        TextBlock temp = selectedMenu as TextBlock;
                        switch (temp.Tag.ToString())
                        {
                            case "Hide My Statistics":
                                logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : " + temp.Tag.ToString());
                                isShowMyStatistics = false;
                                AddMenuItems();
                                StatDisplayTimer.Stop();
                                StatDisplayTimer.Interval = new TimeSpan(0, 0, 0);
                                StatDisplayTimer.Start();
                                break;
                            case "Hide Contact Center Statistics":
                                logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : " + temp.Tag.ToString());
                                isShowCCStatistics = false;
                                StatDisplayTimer.Stop();
                                StatDisplayTimer.Interval = new TimeSpan(0, 0, 0);
                                StatDisplayTimer.Start();
                                AddMenuItems();
                                break;
                            case "Show My Statistics":
                                logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : " + temp.Tag.ToString());
                                isShowMyStatistics = true;
                                AddMenuItems();
                                break;
                            case "Show Contact Center Statistics":
                                logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : " + temp.Tag.ToString());
                                isShowCCStatistics = true;
                                AddMenuItems();
                                break;
                            case "Always on Top":
                                logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : " + temp.Tag.ToString());
                                if (IsTopMost)
                                {
                                    IsTopMost = false;
                                }
                                else
                                {
                                    IsTopMost = true;
                                }
                                break;
                            case "My Statistics":
                                logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : " + temp.Tag.ToString());
                                objStatTicker.ShowMyStatistisAID(true);
                                break;
                            case "CC Statistics":
                                logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : " + temp.Tag.ToString());
                                objStatTicker.ShowCCStatisticsAID(true);
                                break;
                            case "Horizontal":
                                logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : " + temp.Tag.ToString());
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                                {
                                    Settings.GetInstance().DictEnableDisableChannels["tagvertical"] = false;
                                }
                                else
                                {
                                    Settings.GetInstance().DictEnableDisableChannels.Add("tagvertical", false);
                                }

                                isMenuClicked = true;
                                OrientationChanged();
                                TagOrientation();
                                AddMenuItems();
                                break;
                            case "Vertical":
                                logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : " + temp.Tag.ToString());
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                                {
                                    Settings.GetInstance().DictEnableDisableChannels["tagvertical"] = true;
                                }
                                else
                                {
                                    Settings.GetInstance().DictEnableDisableChannels.Add("tagvertical", true);
                                }
                                isMenuClicked = true;
                                OrientationChanged();
                                TagOrientation();
                                AddMenuItems();
                                break;
                            case "Show Header":
                                isShowHeader = !isShowHeader;
                                Settings.GetInstance().DictEnableDisableChannels["isheaderenabled"] = isShowHeader;
                                isHeaderClicked = true;
                                defaultHeader = true;
                                HeaderValues();
                                break;
                            //case "Queue Configuration":
                            //    ObjectConfigWindow objView = new ObjectConfigWindow();
                            //    Settings.GetInstance().QueueConfigVM = new ObjectConfigWindowViewModel();
                            //    //Settings.GetInstance().ObjectConfigVM.LoadObjectConfiguration();
                            //    objView.DataContext = Settings.GetInstance().QueueConfigVM;
                            //    objView.Show();
                            //    break;
                            default:
                                logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : " + temp.Tag.ToString());
                                taggedStats = string.Empty;
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("notificationclose"))
                                {
                                    logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : NotificationClose : Key Found");
                                    if (Settings.GetInstance().DictEnableDisableChannels["notificationclose"])
                                    {
                                        logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : NotificationClose : False");
                                        isMinimized = true;
                                        foreach (Window window in Application.Current.Windows)
                                        {
                                            if (window.Title == "StatGadget")
                                            {
                                                window.Hide();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : NotificationClose : False");
                                        #region Close Gadget

                                        isMinimized = false;
                                        int tagno = 1;
                                        string statDisplayName = string.Empty;
                                        string ServerTaggedStats = string.Empty;
                                        string DBTaggedStats = string.Empty;

                                        output = new OutputValues();
                                        if (hashTaggedStats.Count > 0)
                                        {
                                            SortedDictionary<int, string> tempdic = new SortedDictionary<int, string>();

                                            foreach (TagGadgetControl temptagcontrol in MyTagControlCollection)
                                            {
                                                foreach (DictionaryEntry entry in hashTaggedStats)
                                                {
                                                    if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
                                                    {
                                                        string[] keyvalues = entry.Key.ToString().Split('_');
                                                        int tagn = Convert.ToInt32(keyvalues[1]);
                                                        tempdic.Add(tagn, entry.Value.ToString());
                                                    }
                                                }
                                            }

                                            foreach (TagGadgetControl temptagcontrol in MyTagControlCollection2)
                                            {
                                                foreach (DictionaryEntry entry in hashTaggedStats)
                                                {
                                                    if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
                                                    {
                                                        string[] keyvalues = entry.Key.ToString().Split('_');
                                                        int tagn = Convert.ToInt32(keyvalues[1]);

                                                        if (!tempdic.ContainsKey(tagn))
                                                            tempdic.Add(tagn, entry.Value.ToString());
                                                    }
                                                }
                                            }

                                            foreach (string statName in tempdic.Values)
                                            {
                                                string Dilimitor = "_@";
                                                string[] stat = statName.Split(new[] { Dilimitor }, StringSplitOptions.None);
                                                string[] statobj = stat[1].Split('\n');

                                                #region DB Tagged Stats

                                                if (DBTaggedStats.Equals(string.Empty))
                                                {
                                                    if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
                                                    {
                                                        DBTaggedStats = stat[0];
                                                    }
                                                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                                    {
                                                        if (statobj.Length == 1)
                                                        {
                                                            DBTaggedStats = stat[0];
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
                                                    {
                                                        DBTaggedStats = DBTaggedStats + "," + stat[0];
                                                    }
                                                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                                    {
                                                        if (statobj.Length == 1)
                                                        {
                                                            DBTaggedStats = DBTaggedStats + "," + stat[0];
                                                        }
                                                    }
                                                }

                                                #endregion

                                                #region Server Tagged Stats
                                                if (ServerTaggedStats.Equals(string.Empty) && statDisplayName.Equals(string.Empty))
                                                {
                                                    if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                                                    {
                                                        ServerTaggedStats = statobj[0] + "@" + stat[0];
                                                    }
                                                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                                    {
                                                        if (statobj.Length > 1)
                                                        {
                                                            ServerTaggedStats = statobj[0] + "@" + stat[0];
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                                                    {
                                                        ServerTaggedStats = ServerTaggedStats + "," + statobj[0] + "@" + stat[0];
                                                    }
                                                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                                    {
                                                        if (statobj.Length > 1)
                                                        {
                                                            ServerTaggedStats = ServerTaggedStats + "," + statobj[0] + "@" + stat[0];
                                                        }
                                                    }
                                                }

                                                #endregion
                                                #region old code commented by santha
                                                //if (taggedStats.Equals(string.Empty) && statDisplayName.Equals(string.Empty))
                                                //{
                                                //    statDisplayName = tagno + "_&" + statobj[1] + "@" + statobj[0];
                                                //    taggedStats = statobj[0] + "@" + stat[0];
                                                //}
                                                //else
                                                //{
                                                //    statDisplayName = statDisplayName + "," + tagno + "_&" + statobj[1] + "@" + statobj[0];
                                                //    taggedStats = taggedStats + "," + statobj[0] + "@" + stat[0];
                                                //}
                                                #endregion
                                                tagno++;
                                            }

                                            if (!DictTaggedStats.ContainsKey(StatisticsEnum.StatSource.StatServer.ToString()))
                                                DictTaggedStats.Add(StatisticsEnum.StatSource.StatServer.ToString(), ServerTaggedStats);

                                            if (!DictTaggedStats.ContainsKey(StatisticsEnum.StatSource.DB.ToString()))
                                                DictTaggedStats.Add(StatisticsEnum.StatSource.DB.ToString(), DBTaggedStats);

                                            output = objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);

                                        }
                                        else
                                        {

                                            output = objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);

                                        }

                                        if (!StatisticsBase.GetInstance().isPlugin)
                                        {
                                            //objNotify.Visible = false;
                                            Environment.Exit(0);
                                        }
                                        else
                                        {
                                            foreach (Window window in Application.Current.Windows)
                                            {
                                                objStatTicker.UnSubscribe("StatTickerFive");
                                                window.Close();
                                            }
                                        }
                                    }

                                        #endregion
                                }
                                else
                                {
                                    logger.Warn("MainWindowViewModel : MenuItem_Click : Menu Clicked : NotificationClose : Key Missing");
                                    #region Close Gadget

                                    isMinimized = false;
                                    int tagno = 1;
                                    string statDisplayName = string.Empty;

                                    output = new OutputValues();
                                    if (hashTaggedStats.Count > 0)
                                    {
                                        SortedDictionary<int, string> tempdic = new SortedDictionary<int, string>();

                                        foreach (TagGadgetControl temptagcontrol in MyTagControlCollection)
                                        {
                                            foreach (DictionaryEntry entry in hashTaggedStats)
                                            {
                                                if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
                                                {
                                                    string[] keyvalues = entry.Key.ToString().Split('_');
                                                    int tagn = Convert.ToInt32(keyvalues[1]);
                                                    tempdic.Add(tagn, entry.Value.ToString());
                                                }
                                            }
                                        }

                                        foreach (TagGadgetControl temptagcontrol in MyTagControlCollection2)
                                        {
                                            foreach (DictionaryEntry entry in hashTaggedStats)
                                            {
                                                if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
                                                {
                                                    string[] keyvalues = entry.Key.ToString().Split('_');
                                                    int tagn = Convert.ToInt32(keyvalues[1]);

                                                    if (!tempdic.ContainsKey(tagn))
                                                        tempdic.Add(tagn, entry.Value.ToString());
                                                }
                                            }
                                        }

                                        foreach (string statName in tempdic.Values)
                                        {
                                            string Dilimitor = "_@";
                                            string[] stat = statName.Split(new[] { Dilimitor }, StringSplitOptions.None);
                                            string[] statobj = stat[1].Split('\n');

                                            if (taggedStats.Equals(string.Empty) && statDisplayName.Equals(string.Empty))
                                            {
                                                statDisplayName = tagno + "_&" + statobj[1] + "@" + statobj[0];
                                                taggedStats = statobj[0] + "@" + stat[0];
                                            }
                                            else
                                            {
                                                statDisplayName = statDisplayName + "," + tagno + "_&" + statobj[1] + "@" + statobj[0];
                                                taggedStats = taggedStats + "," + statobj[0] + "@" + stat[0];
                                            }
                                            tagno++;
                                        }

                                        output = objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);

                                    }
                                    else
                                    {

                                        output = objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);

                                    }

                                    if (!StatisticsBase.GetInstance().isPlugin)
                                    {
                                        //objNotify.Visible = false;
                                        Environment.Exit(0);
                                    }
                                    else
                                    {
                                        foreach (Window window in Application.Current.Windows)
                                        {
                                            if (window.Title == "StatGadget")
                                            {
                                                window.Hide();
                                            }
                                        }
                                    }

                                    #endregion
                                }
                                objStatTicker.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Closed);// Added to notification gadget window closed to AID
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : MenuItem_Click Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : MenuItem_Click : Method Exit");
            }
        }

        /// <summary>
        /// Next Statistics.
        /// </summary>
        void NextClick()
        {
            try
            {
                logger.Debug("MainWindowViewModel : NextClick : Method Entry");

                isNext = true;
                isPrevious = false;
                StatDisplayTimer.Stop();

                StatDisplayTimer.Interval = new TimeSpan(0, 0, 0);
                StatDisplayTimer.Start();
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : NextClick Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : NextClick : Method Exit");
            }
        }

        void NotificationSubitem_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                System.Windows.Controls.MenuItem objItem = sender as System.Windows.Controls.MenuItem;
                string ObjHeader = objItem.Header.ToString();

                switch (ObjHeader)
                {
                    case "Music":
                        isNotifierAudioSelected = !isNotifierAudioSelected;
                        break;
                    case "Popup":
                        isNotiferPopupSelected = !isNotiferPopupSelected;
                        break;
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Handles the Select event of the menu control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void NotifyMenuSelect(object sender, EventArgs e)
        {
            MenuItem menu;
            string statDisplayName = string.Empty;
            SortedDictionary<int, string> tempdic = new SortedDictionary<int, string>();
            Dictionary<string, string> DictTaggedStats = new Dictionary<string, string>();
            try
            {
                logger.Debug("MainWindowViewModel : NotifyMenuSelect : Method Entry");
                menu = sender as MenuItem;

                if (menu != null && menu.Name == "cmenuClose")
                {

                    int tagno = 1;
                    taggedStats = string.Empty;
                    string DBTaggedStats = string.Empty;
                    string ServerTaggedStats = string.Empty;

                    if (hashTaggedStats.Count > 0)
                    {
                        SortedDictionary<int, string> tempdict = new SortedDictionary<int, string>();

                        foreach (TagGadgetControl temptagcontrol in MyTagControlCollection)
                        {
                            foreach (DictionaryEntry entry in hashTaggedStats)
                            {
                                if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
                                {
                                    string[] keyvalues = entry.Key.ToString().Split('_');
                                    int tagn = Convert.ToInt32(keyvalues[1]);
                                    tempdic.Add(tagn, entry.Value.ToString());
                                }
                            }
                        }

                        foreach (TagGadgetControl temptagcontrol in MyTagControlCollection2)
                        {
                            foreach (DictionaryEntry entry in hashTaggedStats)
                            {
                                if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
                                {
                                    string[] keyvalues = entry.Key.ToString().Split('_');
                                    int tagn = Convert.ToInt32(keyvalues[1]);

                                    if (!tempdic.ContainsKey(tagn))
                                        tempdic.Add(tagn, entry.Value.ToString());
                                }
                            }
                        }

                        foreach (string statName in tempdic.Values)
                        {
                            string Dilimitor = "_@";
                            string[] stat = statName.Split(new[] { Dilimitor }, StringSplitOptions.None);
                            string[] statobj = stat[1].Split('\n');

                            #region DB Tagged Stats

                            if (DBTaggedStats.Equals(string.Empty))
                            {
                                if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
                                {
                                    DBTaggedStats = stat[0];
                                }
                                else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                {
                                    if (statobj.Length == 1)
                                    {
                                        DBTaggedStats = stat[0];
                                    }
                                }
                            }
                            else
                            {
                                if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
                                {
                                    DBTaggedStats = DBTaggedStats + "," + stat[0];
                                }
                                else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                {
                                    if (statobj.Length == 1)
                                    {
                                        DBTaggedStats = DBTaggedStats + "," + stat[0];
                                    }
                                }
                            }

                            #endregion

                            #region Server Tagged Stats
                            if (ServerTaggedStats.Equals(string.Empty) && statDisplayName.Equals(string.Empty))
                            {
                                if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                                {
                                    ServerTaggedStats = statobj[0] + "@" + stat[0];
                                }
                                else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                {
                                    if (statobj.Length > 1)
                                    {
                                        ServerTaggedStats = statobj[0] + "@" + stat[0];
                                    }
                                }
                            }
                            else
                            {
                                if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                                {
                                    ServerTaggedStats = ServerTaggedStats + "," + statobj[0] + "@" + stat[0];
                                }
                                else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                                {
                                    if (statobj.Length > 1)
                                    {
                                        ServerTaggedStats = ServerTaggedStats + "," + statobj[0] + "@" + stat[0];
                                    }
                                }
                            }

                            #endregion

                            #region old code commented by santha
                            //if (taggedStats.Equals(string.Empty) && statDisplayName.Equals(string.Empty))
                            //{
                            //    statDisplayName = tagno + "_&" + statobj[1] + "@" + statobj[0];
                            //    taggedStats = statobj[0] + "@" + stat[0];
                            //}
                            //else
                            //{
                            //    statDisplayName = statDisplayName + "," + tagno + "_&" + statobj[1] + "@" + statobj[0];
                            //    taggedStats = taggedStats + "," + statobj[0] + "@" + stat[0];
                            //}
                            #endregion
                            tagno++;
                        }

                        if (!DictTaggedStats.ContainsKey(StatisticsEnum.StatSource.StatServer.ToString()))
                            DictTaggedStats.Add(StatisticsEnum.StatSource.StatServer.ToString(), ServerTaggedStats);

                        if (!DictTaggedStats.ContainsKey(StatisticsEnum.StatSource.DB.ToString()))
                            DictTaggedStats.Add(StatisticsEnum.StatSource.DB.ToString(), DBTaggedStats);

                        output = objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);

                    }
                    else
                    {
                        output = objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);

                    }

                    //objNotify.Visible = false;
                    Environment.Exit(0);
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : NotifyMenuSelect : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : NotifyMenuSelect : Method Exit");
            }
        }

        /// <summary>
        /// Handles the DoubleClick event of the objNotify control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void objNotifyDoubleClick(object sender, EventArgs e)
        {
            try
            {
                logger.Debug("MainWindowViewModel : ObjNotifyDoubleClick : Method Entry");
                if (isMinimized)
                {
                    logger.Warn("MainWindowViewModel : ObjNotifyDoubleClick : IsMinimized : True");
                    isMinimized = false;

                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.Title == "StatGadget")
                        {
                            window.WindowState = WindowState.Maximized;
                            window.Show();

                            if (!IsTopMost)
                            {
                                IsTopMost = false;
                            }

                            if (window.Topmost != true)
                            {
                                IsTopMost = true;
                                IsTopMost = false;
                            }

                        }
                    }

                }
                else
                {
                    logger.Warn("MainWindowViewModel : ObjNotifyDoubleClick : IsMinimized : False");
                    foreach (Window window in Application.Current.Windows)
                    {
                        if (window.Title == "StatGadget")
                        {
                            if (window.WindowState == WindowState.Minimized)
                            {
                                window.WindowState = WindowState.Maximized;
                                window.Show();
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : objNotifyDoubleClick : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : objNotifyDoubleClick : Method Exit");
            }
        }

        /// <summary>
        /// Handles the Tick event of the PausedStatTimer1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="EventArgs" /> instance containing the event data.</param>
        void PausedStatTimerTick(object sender, EventArgs e)
        {
            try
            {
                logger.Debug("MainWindowViewModel : PausedStatTimerTick : Method Entry");
                StatisticsInfo statInfo = Settings.GetInstance().DicStatInfo[PausedTag];

                StatValue = statInfo.StatValue;

                StatValueColor = new SolidColorBrush();
                if (statInfo.StatColor.Name.ToString().Contains('#'))
                {
                    StatValueColor = (SolidColorBrush)(new BrushConverter().ConvertFrom(statInfo.StatColor.Name.ToString()));
                }
                else
                {
                    StatValueColor.Color = System.Windows.Media.Color.FromArgb(statInfo.StatColor.A, statInfo.StatColor.R, statInfo.StatColor.G, statInfo.StatColor.B);
                }

                ShowThresholdBreach(statInfo.RefId, statInfo.StatName, statInfo.IsLevelTwo);
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : PausedStatTimerTick Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : PausedStatTimerTick : Method Exit");
            }
        }

        /// <summary>
        /// Plays the pause click.
        /// </summary>
        /// <param name="obj">The obj.</param>
        void PlayPauseClick(object obj)
        {
            string Theme = string.Empty;
            try
            {
                logger.Debug("MainWindowViewModel : PlayPauseClick : Method Entry");
                if (obj.ToString() == "Play")
                {
                    logger.Warn("MainWindowViewModel : PlayPauseClick : Play");
                    PlayPause = "Pause";

                    Theme = Settings.GetInstance().Theme;

                    switch (Theme)
                    {
                        case "Outlook8":
                            PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/play_blue.png"));
                            break;
                        case "Yahoo":
                            PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/play_yahoo.png"));
                            break;
                        case "Grey":
                            PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/play_gray.png"));
                            break;
                        case "BB_Theme1":
                            PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/play_gray.png"));
                            break;
                        default:
                            PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/play_blue.png"));
                            break;
                    }

                    PlayPauseTooltip = "Play";
                    PausedTag = Settings.GetInstance().CurrStatRefId;
                    StatDisplayTimer.Stop();

                    PausedStatTimer1.Interval = new TimeSpan(0, 0, 1);
                    PausedStatTimer1.Tick += PausedStatTimerTick;
                    PausedStatTimer1.Start();
                }
                else
                {
                    logger.Warn("MainWindowViewModel : PlayPauseClick : Pause");
                    PlayPause = "Play";
                    PausedStatTimer1.Stop();

                    Theme = Settings.GetInstance().Theme;

                    switch (Theme)
                    {
                        case "Outlook8":
                            PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/stop_blue.png"));
                            break;
                        case "Yahoo":
                            PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/stop_yahoo.png"));
                            break;
                        case "Grey":
                            PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/stop_gray.png"));
                            break;
                        case "BB_Theme1":
                            PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/stop_gray.png"));
                            break;
                        default:
                            PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/stop_blue.png"));
                            break;
                    }

                    PlayPauseTooltip = "Stop";
                    StatDisplayTimer.Start();
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : PlayPauseClick Method : " + ex.Message);
            }
            finally
            {
                Theme = null;
                GC.Collect();
                logger.Debug("MainWindowViewModel : PlayPauseClick : Method Exit");
            }
        }

        /// <summary>
        /// Previous the statistics.
        /// </summary>
        void PreviousClick()
        {
            try
            {
                logger.Debug("MainWindowViewModel : PreviousClick : Method Entry");
                isPrevious = true;
                isNext = false;

                StatDisplayTimer.Stop();

                StatDisplayTimer.Interval = new TimeSpan(0, 0, 0);
                StatDisplayTimer.Start();
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : PreviousClick Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : PreviousClick : Method Exit");
            }
        }

        /// <summary>
        /// Saves the tagged statistics.
        /// </summary>
        /// <param name="statistics">The statistics.</param>
        void SaveTaggedStatistics(string statistics)
        {
            string appPath = string.Empty;
            try
            {
                logger.Debug("MainWindowViewModel : SaveTaggedStatistics : Method Entry");

                appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                string configFile = System.IO.Path.Combine(appPath, Settings.execonfig);
                ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                configFileMap.ExeConfigFilename = configFile;

                var statkey = ConfigurationManager.AppSettings[Settings.GetInstance().UserName + "_Statistics"];
                if (string.IsNullOrEmpty(statkey))
                {
                    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                    config.AppSettings.Settings.Remove(Settings.GetInstance().UserName + "_Statistics");
                    config.AppSettings.Settings.Add(Settings.GetInstance().UserName + "_Statistics", statistics);
                    config.Save(ConfigurationSaveMode.Minimal);
                }
                else
                {
                    System.Configuration.Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                    config.AppSettings.Settings[Settings.GetInstance().UserName + "_Statistics"].Value = statistics;
                    config.Save();
                }

            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : SaveTaggedStatistics Method : " + ex.Message);
            }
            finally
            {
                appPath = null;
                GC.Collect();
                logger.Debug("MainWindowViewModel : SaveTaggedStatistics : Method Exit");
            }
        }

        private void SaveTag_Click(object sender, RoutedEventArgs e)
        {
            savetaggedstats();

        }

        private void SetNotifierProperties()
        {
            logger.Debug("MainWindowViewModel : SetNotifierProperties : Method Entry");
            try
            {
                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("notifyprimaryscreen"))
                {
                    if (Settings.GetInstance().DictEnableDisableChannels["notifyprimaryscreen"])
                    {
                        tbarHeight = (int)SystemParameters.PrimaryScreenHeight - (int)SystemParameters.WorkArea.Height;
                        growlNotifications.Left = SystemParameters.WorkArea.Left + SystemParameters.WorkArea.Width - leftOffset;
                        growlNotifications.MaximumHeight = (int)SystemParameters.WorkArea.Height;
                        growlNotifications.MAX_NOTIFICATIONS = (int)SystemParameters.WorkArea.Height;
                        growlNotifications.Height = SystemParameters.PrimaryScreenHeight;
                        growlNotifications.Top = SystemParameters.WorkArea.Top - tbarHeight;
                        logger.Warn("MainWindowViewModel : SetNotifierProperties : Notify Primary Screen : True");
                    }
                    else
                    {
                        foreach (var screen in Screen.AllScreens)
                        {
                            string Dilimitor = "\\";
                            string[] tempArray = screen.DeviceName.Split(new[] { Dilimitor }, StringSplitOptions.None);

                            if (tempArray[3].ToString() != PrimaryScreen[3].ToString())
                            {
                                tbarHeight = 0;
                                growlNotifications.Left = (PrimaryScreenWidth + screen.Bounds.Width) - leftOffset;
                                growlNotifications.MaximumHeight = screen.Bounds.Height;
                                growlNotifications.MAX_NOTIFICATIONS = screen.Bounds.Height;
                                growlNotifications.Height = screen.Bounds.Height;
                                growlNotifications.Top = 0 - tbarHeight;
                            }
                        }

                        logger.Warn("MainWindowViewModel : SetNotifierProperties : Notify Primary Screen : False");
                    }
                }

                growlNotifications.NotifierClosed += new GrowlNotifications.NotificationClose(growlNotifications_NotifierClosed);

                growlNotifications.SetDisplayTime(StatisticsBase.GetInstance().AlertNotifyTime);
            }
            catch (Exception GeneralException)
            {
                logger.Error("MainWindowViewModel : SetNotifierProperties : " + GeneralException.Message);
            }
            logger.Debug("MainWindowViewModel : SetNotifierProperties : Method Exit");
        }

        /// <summary>
        /// States the changed.
        /// </summary>
        private void StateChanged()
        {
            try
            {

                if (Main_Window.WindowState == WindowState.Minimized)
                {
                    isMinimized = true;
                    //Microsoft.Windows.Controls.MessageBox.Show("Gadget minimized");
                    Main_Window.Hide();
                }
                else if (Main_Window.WindowState == WindowState.Maximized)
                {
                    Main_Window.WindowState = WindowState.Normal;
                    Main_Window.Show();
                }
                else
                {
                    //Microsoft.Windows.Controls.MessageBox.Show("Gadget NOrmal");
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : StateChanged : " + ex.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Stats the name un tagged.
        /// </summary>
        /// <param name="statisticName">Name of the statistic.</param>
        /// <returns></returns>
        private string StatNameUnTagged(string statisticName)
        {
            try
            {
                logger.Debug("MainWindowViewModel : StatNameUnTagged : Method Entry");
                if (statisticName.Length > 28)
                {
                    statisticName = statisticName.Substring(0, 28);
                    statisticName = statisticName + "..";
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : StatNameUnTagged Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : StatNameUnTagged : Method Exit");
            }
            return statisticName;
        }

        /// <summary>
        /// Stats the ticker_ add tagged stats.
        /// </summary>
        private void StatTickerAddTaggedStats()
        {
            string strDilimitor = "_&";
            try
            {
                logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : Method Entry");
                bool addHashTaggedStats = true;
                SortedList statDict = new SortedList();

                Settings.GetInstance().DictTaggedStats = objStatTicker.GetTaggedStats();

                foreach (string statName in Settings.GetInstance().DictTaggedStats.Keys)
                {
                    string[] tagn = statName.Split(',');

                    if (!dictAddedStatistics.ContainsKey(Convert.ToInt32(tagn[0])))
                    {
                        dictAddedStatistics.Add(Convert.ToInt32(tagn[0]), tagn[1] + "_&" + Settings.GetInstance().DictTaggedStats[statName].ToString());
                    }

                    string[] statObjects = statName.Split(',', '@');
                    string[] statobj = statObjects[1].Split('\n');
                    listTaggedStatObjects.Add(statobj[0]);
                }

                MyTagControlCollection2 = new ObservableCollection<UserControl>();
                MyTagControlTempCollection = new ObservableCollection<UserControl>();

                RemainTags = dictAddedStatistics.Count;
                int currentTagno = 0;

                if (dictAddedStatistics.Count > 0)
                {
                    #region Add Tagged Stats

                    foreach (string stats in dictAddedStatistics.Values)
                    {

                        currentTagno++;

                        string[] statName = stats.Split(new[] { strDilimitor }, StringSplitOptions.None);

                        if (addHashTaggedStats)
                        {
                            TagNo = currentTagno;
                            addHashTaggedStats = false;
                            foreach (string statTagged in dictAddedStatistics.Values)
                            {

                                string TaggedStatList = ConfigurationManager.AppSettings.Get(Settings.GetInstance().UserName + "_Statistics");

                                if (TaggedStatList == null)
                                {
                                    string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

                                    string configFile = System.IO.Path.Combine(appPath, Settings.execonfig);
                                    ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
                                    configFileMap.ExeConfigFilename = configFile;

                                    Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
                                    KeyValueConfigurationElement tempvalue = config.AppSettings.Settings[Settings.GetInstance().UserName + "_Statistics"];
                                    if (tempvalue != null)
                                        TaggedStatList = tempvalue.Value;
                                    else
                                        TaggedStatList = null;
                                }

                                if (TaggedStatList != string.Empty && TaggedStatList != null)
                                {
                                    string[] statlist = TaggedStatList.Split(',');

                                    foreach (string statsname in statlist)
                                    {
                                        string[] statorder = statsname.Split(new[] { strDilimitor }, StringSplitOptions.None);
                                        if (TagNo.ToString() == statorder[0])
                                        {
                                            if (!statDict.ContainsKey(statorder[0]))
                                                statDict.Add(statorder[0], statorder[1]);
                                        }
                                    }
                                }

                                string statval = string.Empty;
                                foreach (int entry in dictAddedStatistics.Keys)
                                {
                                    if (dictAddedStatistics[entry].ToString() == statTagged)
                                    {
                                        statval = entry.ToString();
                                    }
                                }

                                string tagName = "tag_" + TagNo;

                                string[] stattags = statTagged.Split(new[] { strDilimitor }, StringSplitOptions.None);
                                string[] stats1 = stattags[0].Split('@');
                                if (!hashTaggedStats.Contains(tagName))
                                {
                                    if (statDict.Count != 0)
                                    {
                                        string stattool1 = statDict[TagNo.ToString()].ToString();
                                        string[] tooltip1 = stattool1.Split('@');
                                        if (tooltip1.Length > 1)
                                            hashTaggedStats.Add(tagName, stats1[1] + "_@" + tooltip1[1] + "\n" + tooltip1[0]);
                                        else
                                            hashTaggedStats.Add(tagName, stats1[0] + "_@" + tooltip1[0]);
                                    }
                                    else
                                    {
                                        //code changed by arun on 11-01-2016
                                        string[] tagstatname = statName[0].Split('@');
                                        if (stats1.Length > 1)
                                            hashTaggedStats.Add(tagName, stattags[0]);
                                        else
                                            hashTaggedStats.Add(tagName, stats1[0]);
                                    }
                                }

                                TagNo++;
                            }
                        }

                        TagNo = currentTagno;
                        userTagControl = new TagGadgetControl();

                        if (removedNo.Count != 0)
                        {
                            userTagControl.Tag = removedNo.Dequeue();
                        }
                        else
                        {
                            userTagControl.Tag = "tag_" + TagNo;
                        }

                        userTagControl.DataContext = this;
                        if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
                        {
                            TagStatValueMargin = new Thickness(0, 0, 2, 0);
                        }
                        else
                        {
                            TagStatValueMargin = new Thickness(15, 0, 2, 0);
                        }

                        string NewStatName = statName[1];

                        userTagControl.lblStatName.Text = NewStatName;

                        userTagControl.lblStatName.Tag = statName[0];

                        userTagControl.MainGrid.Background = BackgroundColor;

                        string[] StatTT = statName[0].Split('@');

                        userTagControl.lblStatName.ToolTip = StatTT[0];

                        userTagControl.lblStatValue.Text = StatValue ?? "-";
                        hashTagDetails.Add(statName[1] + "_@" + TagNo, userTagControl.Tag);

                        tempTagheight = TagGadgetHeight;

                        if (statDict.Count != 0)
                        {
                            string stattool = statDict[TagNo.ToString()].ToString();
                            string[] tooltip = stattool.Split('@');
                            StatToolTip = tooltip[1] + "\n" + tooltip[0];
                            if (tooltip.Length > 1)
                                userTagControl.lblStatName.ToolTip = tooltip[1] + "\n" + tooltip[0];
                            else
                                userTagControl.lblStatName.ToolTip = tooltip[0];
                        }

                        string Dilimitor = "\n";
                        if (userTagControl.lblStatName.ToolTip != null)
                        {
                            string[] StatObj = userTagControl.lblStatName.ToolTip.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);

                            userTagControl.lblStatObj.Text = StatObj[0].ToString();

                            TagStatisticsObject = StatObj[0].ToString();
                        }

                        //userTagControl.StatObjTooltip.Text = userTagControl.lblStatObj.Text;
                        //userTagControl.StatValueTooltip.Text = userTagControl.lblStatValue.Text;

                        ((MainWindowViewModel)userTagControl.DataContext).GadgetTagValue = "tag_" + TagNo;

                        if (Settings.GetInstance().HshAllTagNames != null)
                        {
                            if (!Settings.GetInstance().HshAllTagNames.ContainsKey(statName[0]))
                            {
                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
                                    {

                                        logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : TagVertical : True");
                                        Tag_Width = (int)GadgetWidth;
                                        if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                        {
                                            WrapHorizontalWidth = (Tag_Width * TagNo);
                                        }
                                        else
                                        {
                                            WrapHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
                                        }

                                        WrapWidth = Tag_Width;
                                        WrapVerticalHeight = (((int)userTagControl.MainBorder.Height)) * TagNo;

                                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                                        {
                                            if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                            {
                                                NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth) - 1;

                                                NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);

                                                WindowHorizontalWidth = (Tag_Width * (NumberOfTags_Horizontally)) + (int)GadgetWidth;
                                                WindowVerticalWidth = Tag_Width;

                                                WindowHorizontalHeight = (int)userControl.MainBorder.Height;
                                                WindowVerticalHeight = (int)userControl.MainBorder.Height + (((int)userTagControl.MainBorder.Height) * TagNo);

                                            }
                                            else
                                            {
                                                NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth);
                                                NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);

                                                WindowHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
                                                WindowVerticalWidth = Tag_Width;

                                                WindowHorizontalHeight = (int)userTagControl.MainBorder.Height * 2;
                                                WindowVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;

                                            }
                                        }

                                        if (TagNo > NumberOfTags_Horizontally)
                                            TotalTagsExceeded++;

                                        if (TagNo == 1)
                                        {
                                            userTagControl.MainGrid.Margin = new Thickness(2, 2, 2, 2);
                                        }
                                        else
                                        {
                                            userTagControl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                        }
                                    }
                                    else
                                    {
                                        logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : TagVertical : False");
                                        Tag_Width = (int)GadgetWidth;

                                        if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                        {
                                            WrapHorizontalWidth = (Tag_Width * TagNo);
                                        }
                                        else
                                        {
                                            WrapHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
                                        }

                                        WrapWidth = Tag_Width * NumberOfTags_Horizontally;
                                        WrapVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;

                                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                                        {
                                            if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                            {
                                                NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth) - 1;

                                                NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);

                                                WindowHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally) + (int)GadgetWidth;
                                                WindowVerticalWidth = Tag_Width;

                                                WindowHorizontalHeight = (int)userControl.MainBorder.Height;
                                                WindowVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;

                                            }
                                            else
                                            {
                                                NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth);
                                                NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);

                                                WindowHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
                                                WindowVerticalWidth = Tag_Width;

                                                WindowHorizontalHeight = (int)userTagControl.MainBorder.Height * 2;
                                                WindowVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;
                                            }
                                        }

                                        if (TagNo > NumberOfTags_Horizontally)
                                        {
                                            TotalTagsExceeded++;
                                            userTagControl.MainGrid.Margin = new Thickness(0, 0, 2, 2);
                                        }
                                        else
                                        {
                                            userTagControl.MainGrid.Margin = new Thickness(0, 2, 2, 2);
                                        }
                                    }
                                }

                                if ((!Settings.GetInstance().DictEnableDisableChannels["tagvertical"] && TagNo <= (NumberOfTags_Horizontally * 2)) || !Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                {
                                    MyTagControlCollection.Add(userTagControl);
                                    Wrap2Visibility = Visibility.Collapsed;
                                    isThirdRowActive = false;
                                }
                                else
                                {
                                    if (!isThirdRowActive)
                                    {
                                        Wrap2Visibility = Visibility.Visible;
                                        Wrap2Margin = new Thickness(0, 0, 0, 0);
                                        Wrap2Rows = ++Wrap2Rows;
                                    }

                                    MyTagControlCollection2.Add(userTagControl);
                                    Wrap2Collection.Add(userTagControl);
                                    isThirdRowActive = true;

                                    if (((float)MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1) >= 1) && ((float)MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1) > Wrap2Rows))
                                        Wrap2Rows = ++Wrap2Rows;
                                    WrapWidth = 0;
                                }

                                MyTagControlTempCollection.Add(userTagControl);

                                if (!Settings.GetInstance().HshAllTagNames.ContainsKey(statName[0]))
                                    Settings.GetInstance().HshAllTagNames.Add(statName[0], userTagControl);
                                if (!Settings.GetInstance().HshAllTagList.ContainsKey(userTagControl.Tag.ToString()))
                                    Settings.GetInstance().HshAllTagList.Add(userTagControl.Tag.ToString(), userTagControl);
                                RemainTags--;
                            }
                            else
                            {
                                TagNo--;
                            }

                        }
                    }

                    #endregion
                }
                else
                {
                    if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                    {
                        if (!Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                        {
                            Views.MessageBox msgbox = new Views.MessageBox();

                            string errorList = "No Statistics Configured for the particular agent.";

                            ViewModels.MessageBoxViewModel mboxviewmodel;

                            if (StatisticsBase.GetInstance().isPlugin)
                            {
                                mboxviewmodel = new ViewModels.MessageBoxViewModel("Problem Encountered", errorList, msgbox, "MainWindow", Settings.GetInstance().Theme);
                            }
                            else
                            {
                                mboxviewmodel = new ViewModels.MessageBoxViewModel("Problem Encountered", errorList, msgbox, "LoginWindow", Settings.GetInstance().Theme);
                            }

                            msgbox.DataContext = mboxviewmodel;
                            msgbox.ShowDialog();

                            foreach (Window window in Application.Current.Windows)
                            {
                                if (window.Title == "StatGadget")
                                {
                                    window.Close();

                                }
                            }

                            objStatTicker.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Ended);

                        }
                    }
                }

                HeaderValues();
                TagOrientation();

                foreach (string key in hashTagDetails.Keys)
                {
                    //System.Windows.MessageBox.Show("HaseTageDetail=" + key);
                    string Dilimitor = "_@";
                    string[] keys = key.Split(new[] { Dilimitor }, StringSplitOptions.None);

                    foreach (int dictkey in dictAddedStatistics.Keys)
                    {
                        //System.Windows.MessageBox.Show("dictAddedStatistics=" + dictkey);
                        if (dictkey.ToString() == keys[1])
                        {
                            //System.Windows.MessageBox.Show("keys[0]=" + keys[0]);
                            if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
                            {
                                string[] statObjects = dictAddedStatistics[dictkey].ToString().Split('@');
                                //System.Windows.MessageBox.Show("dictaddedStats=" + dictAddedStatistics[dictkey].ToString());
                                //System.Windows.MessageBox.Show("Add " + hashTagDetails[key] + "@" + statObjects[0]);
                                DicTemp.Add(hashTagDetails[key] + "@" + statObjects[0], keys[0]);
                                //System.Windows.MessageBox.Show(DicAsString(DicTemp));
                                //System.Windows.MessageBox.Show("hashTagDetails" + hashTagDetails[key]);
                                //System.Windows.MessageBox.Show("hashTagDetails" + statObjects[0]);
                                //System.Windows.MessageBox.Show("hashTagDetails" + keys[0]);
                                //System.Windows.MessageBox.Show("DictTemp=" + hashTagDetails[key] + "@" + statObjects[0], keys[0]);
                                //System.Windows.MessageBox.Show("DictTemp=" + hashTagDetails[key] + "@" + statObjects[0]+","+ keys[0]);
                            }
                            else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
                            {
                                Dilimitor = "_&";
                                string[] stObjs = dictAddedStatistics[dictkey].Split(new[] { Dilimitor }, StringSplitOptions.None);
                                //System.Windows.MessageBox.Show("Add " + hashTagDetails[key] + "@" + stObjs[0]);
                                DicTemp.Add(hashTagDetails[key] + "@" + stObjs[0], keys[0]);
                            }
                            else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
                            {
                                if (dictAddedStatistics[dictkey].Contains('@'))
                                {
                                    string[] statObjects = dictAddedStatistics[dictkey].ToString().Split('@');
                                    //System.Windows.MessageBox.Show("Add " + hashTagDetails[key] + "@" + statObjects[0]);
                                    DicTemp.Add(hashTagDetails[key] + "@" + statObjects[0], keys[0]);
                                }
                                else if (dictAddedStatistics[dictkey].Contains("_&"))
                                {
                                    Dilimitor = "_&";
                                    string[] stObjs = dictAddedStatistics[dictkey].Split(new[] { Dilimitor }, StringSplitOptions.None);
                                    //System.Windows.MessageBox.Show("Add " + hashTagDetails[key] + "@" + stObjs[0]);
                                    DicTemp.Add(hashTagDetails[key] + "@" + stObjs[0], keys[0]);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : StatTickerAddTaggedStats Method : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : Method Exit");
            }
        }

        /// <summary>
        /// Tags the button.
        /// </summary>
        /// <param name="statname">The statname.</param>
        /// <returns></returns>
        private string TagButton(string statname, string statValue)
        {
            string formattedstat = string.Empty;
            try
            {
                logger.Debug("MainWindowViewModel : TagButton : Method Entry");
                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagbutton"))
                {
                    if (Settings.GetInstance().DictEnableDisableChannels["tagbutton"])
                    {
                        logger.Warn("MainWindowViewModel : TagButton : TagButton : True");
                        VBMainStatName = 118;
                        LblMainStatName = 98;
                        if (statValue.Contains(':'))
                            formattedstat = formatStatName(statname, 13, 11);
                        else
                            formattedstat = formatStatName(statname, 24, 22);
                    }
                    else
                    {
                        logger.Warn("MainWindowViewModel : TagButton : TagButton : False");
                        VBMainStatName = 143;
                        LblMainStatName = 120;
                        if (statValue.Contains(':'))
                            formattedstat = formatStatName(statname, 20, 18);
                        else
                            formattedstat = formatStatName(statname, 26, 24);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : TagButton : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : TagButton : Method Exit");
            }
            return formattedstat;
        }

        /// <summary>
        /// Tags the orientation.
        /// </summary>
        private void TagOrientation()
        {
            bool isFirst = true;
            try
            {
                logger.Debug("MainWindowViewModel : TagOrientation : Method Entry");

                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
                {
                    if (Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
                    {

                        logger.Warn("MainWindowViewModel : TagOrientation : TagVertical : True");
                        Wrap2Orientation = Orientation.Vertical;

                        if (isMenuClicked)
                        {
                            logger.Warn("MainWindowViewModel : TagOrientation : IsMenuClicked : True");
                            MyTagControlCollection2 = new ObservableCollection<UserControl>();

                            foreach (TagGadgetControl tagCtrl in MyTagControlTempCollection)
                            {
                                if (isFirst)
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                    isFirst = false;
                                }
                                else
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                }

                                if (MyTagControlCollection2.Count < (NumberOfTags_Vertically - 1))
                                    MyTagControlCollection2.Add(tagCtrl);
                            }

                            MyTagControlCollection.Clear();

                        }
                        else
                        {
                            logger.Warn("MainWindowViewModel : TagOrientation : IsMenuClicked : False");
                            if (MyTagControlCollection2.Count == 0)
                            {
                                foreach (TagGadgetControl tagCtrl in MyTagControlTempCollection)
                                {
                                    MyTagControlCollection2.Add(tagCtrl);
                                }
                            }

                            MyTagControlCollection.Clear();

                            foreach (TagGadgetControl tagCtrl in MyTagControlCollection2)
                            {
                                if (isFirst)
                                {
                                    if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                                    {
                                        tagCtrl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                    }
                                    else
                                    {
                                        tagCtrl.MainGrid.Margin = new Thickness(2, 2, 2, 2);
                                    }

                                    isFirst = false;
                                }
                                else
                                {
                                    tagCtrl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                }
                            }
                        }

                        Wrap2Height = (MyTagControlCollection2.Count * (int)TagGadgetHeight);
                        Wrap2Width = (int)GadgetWidth;
                        rightMargin = primwidh - Window_Width;
                        bottomMargin = priheight - Window_Height;

                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                            {
                                logger.Warn("MainWindowViewModel : TagOrientation : MainGadget : True");
                                MainGridHeight = (int)GadgetHeight;
                                Wrap2Margin = new Thickness(0, 0, 0, 0);
                                Wrap2Visibility = Visibility.Visible;

                                if (TagNo != 0)
                                {
                                    TotalGridHeight = MainGridHeight + Wrap2Height;
                                }
                                else
                                {
                                    TotalGridHeight = (int)GadgetHeight;
                                }
                            }
                            else
                            {
                                logger.Warn("MainWindowViewModel : TagOrientation : MainGadget : False");
                                Wrap2Visibility = Visibility.Visible;
                                Wrap2Margin = new Thickness(0, -GadgetHeight, 0, 0);

                                if (TagNo != 0)
                                {
                                    WrapWidth = 0;
                                    MainGridHeight = Wrap2Height;
                                    TotalGridHeight = MainGridHeight;
                                }
                                else
                                {
                                    MainGridHeight = (int)GadgetHeight;
                                }
                            }
                        }
                    }
                    else
                    {
                        logger.Warn("MainWindowViewModel : TagOrientation : TagVertical : False");
                        int tagno = 1;
                        WrapOrientation = Orientation.Horizontal;
                        Wrap2Orientation = Orientation.Horizontal;

                        if (!IsSecondaryOrientaion)
                        {
                            if (ExceededTagCollection.Count != 0)
                            {
                                foreach (TagGadgetControl tagCtrl in ExceededTagCollection)
                                {
                                    MyTagControlCollection.Add(tagCtrl);
                                }
                            }
                        }

                        foreach (TagGadgetControl tagCtrl in MyTagControlCollection)
                        {
                            if (tagno > NumberOfTags_Horizontally)
                            {
                                tagCtrl.MainGrid.Margin = new Thickness(0, 0, 2, 2);
                                TagGadgetControl tempgadget = new TagGadgetControl();
                                tempgadget = (TagGadgetControl)MyTagControlCollection[1];

                                if (IsSecondScreen && tagno > (NumberOfTags_Horizontally * 2) && IsSecondaryOrientaion)
                                {
                                    TempTagCollection.Add(tagCtrl);
                                    ExceededTagCollection.Add(tagCtrl);
                                }
                            }
                            else
                            {
                                tagCtrl.MainGrid.Margin = new Thickness(0, 2, 2, 2);
                            }

                            tagno++;
                        }

                        if (IsSecondScreen && IsSecondaryOrientaion)
                        {
                            if (TempTagCollection.Count != 0)
                            {
                                foreach (TagGadgetControl tempTag in TempTagCollection)
                                {
                                    MyTagControlCollection.Remove(tempTag);
                                }
                            }

                            foreach (TagGadgetControl tagCtrl in MyTagControlCollection2)
                            {
                                TempTagCollection.Add(tagCtrl);
                            }

                            MyTagControlCollection2.Clear();

                            foreach (TagGadgetControl tagCtrl in TempTagCollection)
                            {
                                MyTagControlCollection2.Add(tagCtrl);
                            }
                        }
                        else if (!IsSecondaryOrientaion)
                        {
                            if (ExceededTagCollection.Count != 0)
                            {
                                MyTagControlCollection2.Clear();
                                foreach (TagGadgetControl tagCtrl in Wrap2Collection)
                                {
                                    MyTagControlCollection2.Add(tagCtrl);
                                }
                            }
                        }

                        isMenuClicked = false;

                        if (!isThirdRowActive)
                        {
                            logger.Warn("MainWindowViewModel : TagOrientation : IsThirdRowActive : False");
                            if (WrapMaxWidth < WindowHorizontalWidth)
                            {

                                if (TotalTagsExceeded != 0)
                                {
                                    WrapWidth = NumberOfTags_Horizontally * Tag_Width;
                                    WrapHeight = (int)GadgetHeight;
                                }
                                else
                                {
                                    WrapWidth = TagNo * (int)GadgetWidth;
                                    WrapHeight = (int)GadgetHeight;
                                }
                            }
                            else
                            {
                                if (TagNo > (NumberOfTags_Horizontally))
                                {
                                    WrapWidth = (int)GadgetWidth * NumberOfTags_Horizontally;
                                }
                                else
                                {
                                    WrapWidth = (int)GadgetWidth * TagNo;
                                }

                                if (MyTagControlCollection.Count <= NumberOfTags_Horizontally)
                                    WrapHeight = (int)GadgetHeight;
                                else
                                    WrapHeight = (int)GadgetHeight * 2;
                            }
                        }
                        else
                        {
                            logger.Warn("MainWindowViewModel : TagOrientation : IsThirdRowActive : True");
                            if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                            {
                                if (TagNo > NumberOfTags_Horizontally)
                                    WrapWidth = Tag_Width * NumberOfTags_Horizontally;
                                else
                                    WrapWidth = Tag_Width * TagNo;
                            }
                            else
                            {
                                WrapWidth = NumberOfTags_Horizontally * Tag_Width;
                            }
                            WrapHeight = (int)GadgetHeight;

                            Wrap2Width = MyTagControlCollection2.Count * (int)GadgetWidth;

                            if (Wrap2Width > (GadgetWidth * (NumberOfTags_Horizontally + 1)))
                            {
                                Wrap2Width = (int)GadgetWidth * (NumberOfTags_Horizontally + 1);

                                if ((MyTagControlCollection2.Count) % (NumberOfTags_Horizontally + 1) == 0)
                                    Wrap2Height = (int)TagGadgetHeight * (MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1));
                                else
                                    Wrap2Height = (int)TagGadgetHeight * ((MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1)) + 1);
                            }
                            else
                                Wrap2Height = (int)TagGadgetHeight;
                        }

                        rightMargin = primwidh - Window_Width;
                        bottomMargin = priheight - Window_Height;

                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
                        {
                            if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
                            {
                                logger.Warn("MainWindowViewModel : TagOrientation : MainGadget : True");
                                WrapMargin = new Thickness(0, 0, 0, 0);

                                if (!isThirdRowActive)
                                {
                                    MainGridHeight = (int)GadgetHeight;
                                    TotalGridHeight = MainGridHeight;
                                }
                                else
                                {
                                    MainGridHeight = (int)GadgetHeight;
                                    TotalGridHeight = MainGridHeight + Wrap2Height;
                                    int tempWrapRows = 0;
                                    tagno = 0;
                                    foreach (TagGadgetControl tagCtrl1 in MyTagControlCollection2)
                                    {
                                        tagno++;
                                        if (tagno == 1 || (tagno == ((NumberOfTags_Horizontally + 1) * (tempWrapRows)) + 1))
                                        {
                                            tagCtrl1.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                            tempWrapRows++;
                                        }
                                        else if (tagno > NumberOfTags_Horizontally + 2)
                                        {
                                            tagCtrl1.MainGrid.Margin = new Thickness(0, 0, 2, 2);
                                        }
                                        else
                                        {
                                            tagCtrl1.MainGrid.Margin = new Thickness(0, 0, 2, 2);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                logger.Warn("MainWindowViewModel : TagOrientation : MainGadget : False");
                                tagno = 0;
                                WrapMargin = new Thickness(-GadgetWidth, 0, 0, 0);
                                int tempWrapRows = 0;

                                foreach (TagGadgetControl tagCtrl1 in MyTagControlCollection)
                                {
                                    tagno++;
                                    if (tagno == 1)
                                    {
                                        tagCtrl1.MainGrid.Margin = new Thickness(2, 2, 2, 2);
                                        tempWrapRows++;
                                    }
                                    else if (tagno == ((NumberOfTags_Horizontally) * (tempWrapRows)) + 1)
                                    {
                                        tagCtrl1.MainGrid.Margin = new Thickness(2, 0, 2, 2);
                                        tempWrapRows++;
                                    }
                                    else if (tagno > NumberOfTags_Horizontally + 1)
                                    {
                                        tagCtrl1.MainGrid.Margin = new Thickness(0, 0, 2, 2);
                                    }
                                    else
                                    {
                                        tagCtrl1.MainGrid.Margin = new Thickness(0, 2, 2, 2);
                                    }
                                }

                                if (tagno <= NumberOfTags_Horizontally)
                                    MainGridHeight = (int)GadgetHeight;
                                else
                                {
                                    if (tagno % NumberOfTags_Horizontally == 0)
                                        MainGridHeight = (int)GadgetHeight * (tagno / NumberOfTags_Horizontally);
                                    else
                                        MainGridHeight = (int)GadgetHeight * ((tagno / NumberOfTags_Horizontally) + 1);
                                    WrapHeight = MainGridHeight;
                                }

                                TotalGridHeight = MainGridHeight;
                            }
                        }
                    }
                }

                isMenuClicked = false;
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : TagOrientation : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MainWindowViewModel : TagOrientation : Method Exit");
            }
        }

        /// <summary>
        /// Themes the selector.
        /// </summary>
        private void ThemeSelector()
        {
            string Theme = string.Empty;
            try
            {
                logger.Debug("MainWindowViewModel : ThemeSelector : Method Entry");
                Theme = Settings.GetInstance().Theme;

                switch (Theme)
                {
                    case "Outlook8":
                        logger.Warn("MainWindowViewModel : ThemeSelector : Theme : " + Theme);
                        ThemeColor = new SolidColorBrush();
                        ThemeColor = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                        BackgroundColor = new SolidColorBrush();
                        BackgroundColor = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                        MainStatNameColor = new SolidColorBrush();
                        MainStatNameColor = new BrushConverter().ConvertFromString("#000000") as SolidColorBrush;
                        PreviousImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/previous_blue.png"));
                        PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/stop_blue.png"));
                        NextImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/next_blue.png"));
                        MenuImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/eject_blue.png"));
                        break;

                    case "Yahoo":
                        logger.Warn("MainWindowViewModel : ThemeSelector : Theme : " + Theme);
                        ThemeColor = new SolidColorBrush();
                        ThemeColor = new BrushConverter().ConvertFromString("#340481") as SolidColorBrush;
                        BackgroundColor = new SolidColorBrush();
                        BackgroundColor = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                        MainStatNameColor = new SolidColorBrush();
                        MainStatNameColor = new BrushConverter().ConvertFromString("#000000") as SolidColorBrush;
                        PreviousImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/previous_yahoo.png"));
                        PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/stop_yahoo.png"));
                        NextImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/next_yahoo.png"));
                        MenuImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/eject_yahoo.png"));
                        break;
                    case "Grey":
                        logger.Warn("MainWindowViewModel : ThemeSelector : Theme : " + Theme);
                        ThemeColor = new SolidColorBrush();
                        ThemeColor = new BrushConverter().ConvertFromString("#c6c7c6") as SolidColorBrush;
                        BackgroundColor = new SolidColorBrush();
                        BackgroundColor = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                        MainStatNameColor = new SolidColorBrush();
                        MainStatNameColor = new BrushConverter().ConvertFromString("#000000") as SolidColorBrush;
                        PreviousImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/previous_gray.png"));
                        PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/stop_gray.png"));
                        NextImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/next_gray.png"));
                        MenuImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/eject_gray.png"));
                        break;
                    case "BB_Theme1":
                        logger.Warn("MainWindowViewModel : ThemeSelector : Theme : " + Theme);
                        ThemeColor = new SolidColorBrush();
                        ThemeColor = new BrushConverter().ConvertFromString("#5C6C7A") as SolidColorBrush;
                        BackgroundColor = new SolidColorBrush();
                        BackgroundColor = new BrushConverter().ConvertFromString("#6082B6") as SolidColorBrush;
                        MainStatNameColor = new SolidColorBrush();
                        MainStatNameColor = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                        PreviousImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/previous_gray.png"));
                        PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/stop_gray.png"));
                        NextImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/next_gray.png"));
                        MenuImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/eject_gray.png"));
                        break;
                    default:
                        logger.Warn("MainWindowViewModel : ThemeSelector : Theme : " + Theme);
                        Views.MessageBox msgbox = new Views.MessageBox();
                        ViewModels.MessageBoxViewModel mboxvmodel = new MessageBoxViewModel("Information", "Not able to apply theme", msgbox, "MainWindow", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxvmodel;
                        msgbox.ShowDialog();

                        ThemeColor = new SolidColorBrush();
                        ThemeColor = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                        BackgroundColor = new SolidColorBrush();
                        BackgroundColor = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                        MainStatNameColor = new SolidColorBrush();
                        MainStatNameColor = new BrushConverter().ConvertFromString("#000000") as SolidColorBrush;
                        PreviousImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/previous_blue.png"));
                        PlayPauseImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/stop_blue.png"));
                        NextImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/next_blue.png"));
                        MenuImg = new BitmapImage(new Uri("pack://application:,,,/StatTickerFive;component/Images/eject_blue.png"));

                        break;
                }
                App.Current.Resources["borderbrush"] = ThemeColor;

            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : ThemeSelector : " + ex.Message);
            }
            finally
            {
                Theme = null;
                GC.Collect();
                logger.Debug("MainWindowViewModel : ThemeSelector : Method Exit");
            }
        }

        /// <summary>
        /// Remove the untag button.
        /// </summary>
        /// <param name="statname">The statname.</param>
        /// <returns></returns>
        private string UnTagButton(string statname, string statvalue)
        {
            string formattedstat = string.Empty;
            try
            {
                logger.Debug("MainWindowViewModel : UnTagButton : Method Entry");
                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("untagbutton"))
                {
                    if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
                    {
                        logger.Warn("MainWindowViewModel : UnTagButton : UnTagButton : True");
                        UnTagVisibility = Visibility.Visible;
                        VBStatName = 125;
                        LblStatName = 123;

                        TagStatWidth = 88;
                        if (statvalue.Contains(':'))
                            formattedstat = formatStatName(statname, 20, 18);
                        else
                            formattedstat = formatStatName(statname, 26, 24);

                    }
                    else
                    {
                        logger.Warn("MainWindowViewModel : UnTagButton : UnTagButton : False");
                        UnTagVisibility = Visibility.Collapsed;
                        VBStatName = 127;
                        LblStatName = 126;
                        TagStatWidth = 92;
                        if (statvalue.Contains(':'))
                            formattedstat = formatStatName(statname, 20, 20);
                        else
                            formattedstat = formatStatName(statname, 28, 28);
                    }
                }

            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : UnTagButton : " + ex.Message);
            }
            finally
            {
                GC.Collect();

                logger.Debug("MainWindowViewModel : UnTagButton : Method Exit");
            }
            return formattedstat;
        }

        private void WinClicked()
        {
            try
            {
                if (IsSecondScreen || (DeviceName[3].ToString() != PrimaryScreen[3].ToString()))
                {
                    NumberOfTags_Horizontally = (int)(SecondaryScreenWidth / GadgetWidth) - 1;
                    NumberOfTags_Vertically = (int)(SecondaryScreenHeight / TagGadgetHeight);
                    IsSecondaryOrientaion = true;
                    OrientationChanged();
                    TagOrientation();
                    HeaderValues();
                    IsSecondScreen = false;
                }
                else if (IsSecondaryOrientaion)
                {
                    IsSecondaryOrientaion = false;
                    NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth) - 1;
                    NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
                    OrientationChanged();
                    TagOrientation();
                    TempTagCollection.Clear();
                    ExceededTagCollection.Clear();
                }

            }
            catch (Exception GeneralException)
            {

            }
        }

        /// <summary>
        /// Windows the closing.
        /// </summary>
        private void WindowClosed()
        {
            //objNotify.Visible = false;
            Environment.Exit(0);
        }

        private void WindowClosing()
        {
            try
            {
                StatisticsBase.GetInstance().isPlugin = true;
                if (StatisticsBase.GetInstance().isPlugin)
                {
                    objStatTicker.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Ended);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void WinLocChange()
        {
            try
            {

                foreach (Window window in Application.Current.Windows)
                {
                    if (window.Title == "StatGadget")
                    {

                        var screen = System.Windows.Forms.Screen.FromHandle(new System.Windows.Interop.WindowInteropHelper(window).Handle);

                        var Resolution = screen.Bounds.Size;
                        var Positions = window.Left;
                        string Dilimitor = "\\";
                        DeviceName = screen.DeviceName.Split(new[] { Dilimitor }, StringSplitOptions.None);

                        if (DeviceName[3].ToString() != PrimaryScreen[3].ToString() && !IsSecondaryOrientaion)
                        {
                            PreviousScreen = DeviceName[3].ToString();
                            IsSecondScreen = true;
                            SecondaryScreenHeight = Resolution.Height;
                            SecondaryScreenWidth = Resolution.Width;
                        }
                        else if (DeviceName[3].ToString() == PrimaryScreen[3].ToString())
                        {
                            PreviousScreen = DeviceName[3].ToString();
                            IsSecondScreen = false;
                        }
                    }
                }

            }
            catch (Exception GeneraLException)
            {

            }
        }

        #endregion Methods

        #region Nested Types

        // Structure contain information about low-level keyboard input event
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public Keys key;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr extra;
        }

        #endregion Nested Types

        #region Other

        ///// <summary>
        ///// Stats the ticker_ add tagged stats.
        ///// </summary>
        //private void StatTickerAddTaggedStats()
        //{
        //    string strDilimitor = "_&";
        //    try
        //    {
        //        logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : Method Entry");
        //        bool addHashTaggedStats = true;
        //        SortedList statDict = new SortedList();
        //        Settings.GetInstance().DictTaggedStats = objStatTicker.GetTaggedStats();
        //        foreach (string statName in Settings.GetInstance().DictTaggedStats.Keys)
        //        {
        //            string[] tagn = statName.Split(',');
        //            if (!dictAddedStatistics.ContainsKey(Convert.ToInt32(tagn[0])))
        //            {
        //                dictAddedStatistics.Add(Convert.ToInt32(tagn[0]), tagn[1] + "_&" + Settings.GetInstance().DictTaggedStats[statName].ToString());
        //            }
        //            string[] statObjects = statName.Split(',', '@');
        //            string[] statobj = statObjects[1].Split('\n');
        //            listTaggedStatObjects.Add(statobj[0]);
        //        }
        //        MyTagControlCollection2 = new ObservableCollection<UserControl>();
        //        MyTagControlTempCollection = new ObservableCollection<UserControl>();
        //        RemainTags = dictAddedStatistics.Count;
        //        int currentTagno = 0;
        //        if (dictAddedStatistics.Count > 0)
        //        {
        //            #region Add Tagged Stats
        //            foreach (string stats in dictAddedStatistics.Values)
        //            {
        //                currentTagno++;
        //                string[] statName = stats.Split(new[] { strDilimitor }, StringSplitOptions.None);
        //                if (addHashTaggedStats)
        //                {
        //                    TagNo = currentTagno;
        //                    addHashTaggedStats = false;
        //                    foreach (string statTagged in dictAddedStatistics.Values)
        //                    {
        //                        string TaggedStatList = ConfigurationManager.AppSettings.Get(Settings.GetInstance().UserName + "_Statistics");
        //                        if (TaggedStatList == null)
        //                        {
        //                            string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //                            string configFile = System.IO.Path.Combine(appPath, Settings.execonfig);
        //                            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
        //                            configFileMap.ExeConfigFilename = configFile;
        //                            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        //                            KeyValueConfigurationElement tempvalue = config.AppSettings.Settings[Settings.GetInstance().UserName + "_Statistics"];
        //                            if (tempvalue != null)
        //                                TaggedStatList = tempvalue.Value;
        //                            else
        //                                TaggedStatList = null;
        //                        }
        //                        if (TaggedStatList != string.Empty && TaggedStatList != null)
        //                        {
        //                            string[] statlist = TaggedStatList.Split(',');
        //                            foreach (string statsname in statlist)
        //                            {
        //                                string[] statorder = statsname.Split(new[] { strDilimitor }, StringSplitOptions.None);
        //                                if (TagNo.ToString() == statorder[0])
        //                                {
        //                                    if (!statDict.ContainsKey(statorder[0]))
        //                                        statDict.Add(statorder[0], statorder[1]);
        //                                }
        //                            }
        //                        }
        //                        string statval = string.Empty;
        //                        foreach (int entry in dictAddedStatistics.Keys)
        //                        {
        //                            if (dictAddedStatistics[entry].ToString() == statTagged)
        //                            {
        //                                statval = entry.ToString();
        //                            }
        //                        }
        //                        string tagName = "tag_" + TagNo;
        //                        string[] stattags = statTagged.Split(new[] { strDilimitor }, StringSplitOptions.None);
        //                        string[] stats1 = stattags[0].Split('@');
        //                        if (!hashTaggedStats.Contains(tagName))
        //                        {
        //                            if (statDict.Count != 0)
        //                            {
        //                                string stattool1 = statDict[TagNo.ToString()].ToString();
        //                                string[] tooltip1 = stattool1.Split('@');
        //                                if (tooltip1.Length > 1)
        //                                    hashTaggedStats.Add(tagName, stats1[1] + "_@" + tooltip1[1] + "\n" + tooltip1[0]);
        //                                else
        //                                    hashTaggedStats.Add(tagName, stats1[0] + "_@" + tooltip1[0]);
        //                            }
        //                            else
        //                            {
        //                                string[] tagstatname = statName[0].Split('@');
        //                                if (stats1.Length > 1)
        //                                    hashTaggedStats.Add(tagName, stats1[1]);
        //                                else
        //                                    hashTaggedStats.Add(tagName, stats1[0]);
        //                            }
        //                        }
        //                        TagNo++;
        //                    }
        //                }
        //                TagNo = currentTagno;
        //                userTagControl = new TagGadgetControl();
        //                if (removedNo.Count != 0)
        //                {
        //                    userTagControl.Tag = removedNo.Dequeue();
        //                }
        //                else
        //                {
        //                    userTagControl.Tag = "tag_" + TagNo;
        //                }
        //                userTagControl.DataContext = this;
        //                if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
        //                {
        //                    TagStatValueMargin = new Thickness(0, 0, 2, 0);
        //                }
        //                else
        //                {
        //                    TagStatValueMargin = new Thickness(15, 0, 2, 0);
        //                }
        //                string NewStatName = statName[1];
        //                userTagControl.lblStatName.Text = NewStatName;
        //                userTagControl.lblStatName.Tag = statName[0];
        //                userTagControl.MainGrid.Background = BackgroundColor;
        //                string[] StatTT = statName[0].Split('@');
        //                userTagControl.lblStatName.ToolTip = StatTT[0];
        //                userTagControl.lblStatValue.Text = StatValue ?? "-";
        //                hashTagDetails.Add(statName[1] + "_@" + TagNo, userTagControl.Tag);
        //                tempTagheight = TagGadgetHeight;
        //                if (statDict.Count != 0)
        //                {
        //                    string stattool = statDict[TagNo.ToString()].ToString();
        //                    string[] tooltip = stattool.Split('@');
        //                    StatToolTip = tooltip[1] + "\n" + tooltip[0];
        //                    if (tooltip.Length > 1)
        //                        userTagControl.lblStatName.ToolTip = tooltip[1] + "\n" + tooltip[0];
        //                    else
        //                        userTagControl.lblStatName.ToolTip = tooltip[0];
        //                }
        //                string Dilimitor = "\n";
        //                if (userTagControl.lblStatName.ToolTip != null)
        //                {
        //                    string[] StatObj = userTagControl.lblStatName.ToolTip.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);
        //                    userTagControl.lblStatObj.Text = StatObj[0].ToString();
        //                    TagStatisticsObject = StatObj[0].ToString();
        //                }
        //                ((MainWindowViewModel)userTagControl.DataContext).GadgetTagValue = "tag_" + TagNo;
        //                if (Settings.GetInstance().HshAllTagNames != null)
        //                {
        //                    if (!Settings.GetInstance().HshAllTagNames.ContainsKey(statName[0]))
        //                    {
        //                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
        //                        {
        //                            if (Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
        //                            {
        //                                logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : TagVertical : True");
        //                                Tag_Width = (int)GadgetWidth;
        //                                if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                                {
        //                                    WrapHorizontalWidth = (Tag_Width * TagNo);
        //                                }
        //                                else
        //                                {
        //                                    WrapHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
        //                                }
        //                                WrapWidth = Tag_Width;
        //                                WrapVerticalHeight = (((int)userTagControl.MainBorder.Height)) * TagNo;
        //                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
        //                                {
        //                                    if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                                    {
        //                                        NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth) - 1;
        //                                        NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
        //                                        WindowHorizontalWidth = (Tag_Width * (NumberOfTags_Horizontally)) + (int)GadgetWidth;
        //                                        WindowVerticalWidth = Tag_Width;
        //                                        WindowHorizontalHeight = (int)userControl.MainBorder.Height;
        //                                        WindowVerticalHeight = (int)userControl.MainBorder.Height + (((int)userTagControl.MainBorder.Height) * TagNo);
        //                                    }
        //                                    else
        //                                    {
        //                                        NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth);
        //                                        NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
        //                                        WindowHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
        //                                        WindowVerticalWidth = Tag_Width;
        //                                        WindowHorizontalHeight = (int)userTagControl.MainBorder.Height * 2;
        //                                        WindowVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;
        //                                    }
        //                                }
        //                                if (TagNo > NumberOfTags_Horizontally)
        //                                    TotalTagsExceeded++;
        //                                if (TagNo == 1)
        //                                {
        //                                    userTagControl.MainGrid.Margin = new Thickness(2, 2, 2, 2);
        //                                }
        //                                else
        //                                {
        //                                    userTagControl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : TagVertical : False");
        //                                Tag_Width = (int)GadgetWidth;
        //                                if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                                {
        //                                    WrapHorizontalWidth = (Tag_Width * TagNo);
        //                                }
        //                                else
        //                                {
        //                                    WrapHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
        //                                }
        //                                WrapWidth = Tag_Width * NumberOfTags_Horizontally;
        //                                WrapVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;
        //                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
        //                                {
        //                                    if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                                    {
        //                                        NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth) - 1;
        //                                        NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
        //                                        WindowHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally) + (int)GadgetWidth;
        //                                        WindowVerticalWidth = Tag_Width;
        //                                        WindowHorizontalHeight = (int)userControl.MainBorder.Height;
        //                                        WindowVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;
        //                                    }
        //                                    else
        //                                    {
        //                                        NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth);
        //                                        NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
        //                                        WindowHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
        //                                        WindowVerticalWidth = Tag_Width;
        //                                        WindowHorizontalHeight = (int)userTagControl.MainBorder.Height * 2;
        //                                        WindowVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;
        //                                    }
        //                                }
        //                                if (TagNo > NumberOfTags_Horizontally)
        //                                {
        //                                    TotalTagsExceeded++;
        //                                    userTagControl.MainGrid.Margin = new Thickness(0, 0, 2, 2);
        //                                }
        //                                else
        //                                {
        //                                    userTagControl.MainGrid.Margin = new Thickness(0, 2, 2, 2);
        //                                }
        //                            }
        //                        }
        //                        if ((!Settings.GetInstance().DictEnableDisableChannels["tagvertical"] && TagNo <= (NumberOfTags_Horizontally * 2)) || !Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                        {
        //                            MyTagControlCollection.Add(userTagControl);
        //                            Wrap2Visibility = Visibility.Collapsed;
        //                            isThirdRowActive = false;
        //                        }
        //                        else
        //                        {
        //                            if (!isThirdRowActive)
        //                            {
        //                                Wrap2Visibility = Visibility.Visible;
        //                                Wrap2Margin = new Thickness(0, 0, 0, 0);
        //                                Wrap2Rows = ++Wrap2Rows;
        //                            }
        //                            MyTagControlCollection2.Add(userTagControl);
        //                            Wrap2Collection.Add(userTagControl);
        //                            isThirdRowActive = true;
        //                            if (((float)MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1) >= 1) && ((float)MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1) > Wrap2Rows))
        //                                Wrap2Rows = ++Wrap2Rows;
        //                            WrapWidth = 0;
        //                        }
        //                        MyTagControlTempCollection.Add(userTagControl);
        //                        if (!Settings.GetInstance().HshAllTagNames.ContainsKey(statName[0]))
        //                            Settings.GetInstance().HshAllTagNames.Add(statName[0], userTagControl);
        //                        if (!Settings.GetInstance().HshAllTagList.ContainsKey(userTagControl.Tag.ToString()))
        //                            Settings.GetInstance().HshAllTagList.Add(userTagControl.Tag.ToString(), userTagControl);
        //                        RemainTags--;
        //                    }
        //                    else
        //                    {
        //                        TagNo--;
        //                    }
        //                }
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
        //            {
        //                if (!Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                {
        //                    Views.MessageBox msgbox = new Views.MessageBox();
        //                    string errorList = "No Statistics Configured for the particular agent.";
        //                    ViewModels.MessageBoxViewModel mboxviewmodel;
        //                    if (StatisticsBase.GetInstance().isPlugin)
        //                    {
        //                        mboxviewmodel = new ViewModels.MessageBoxViewModel("Problem Encountered", errorList, msgbox, "MainWindow", Settings.GetInstance().Theme);
        //                    }
        //                    else
        //                    {
        //                        mboxviewmodel = new ViewModels.MessageBoxViewModel("Problem Encountered", errorList, msgbox, "LoginWindow", Settings.GetInstance().Theme);
        //                    }
        //                    msgbox.DataContext = mboxviewmodel;
        //                    msgbox.ShowDialog();
        //                    foreach (Window window in Application.Current.Windows)
        //                    {
        //                        if (window.Title == "StatGadget")
        //                        {
        //                            window.Close();
        //                        }
        //                    }
        //                    objStatTicker.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Ended);
        //                }
        //            }
        //        }
        //        HeaderValues();
        //        TagOrientation();
        //        foreach (string key in hashTagDetails.Keys)
        //        {
        //            string Dilimitor = "_@";
        //            string[] keys = key.Split(new[] { Dilimitor }, StringSplitOptions.None);
        //            foreach (int dictkey in dictAddedStatistics.Keys)
        //            {
        //                System.Windows.MessageBox.Show("dictAddedStats" + dictAddedStatistics.Keys);
        //                if (dictkey.ToString() == keys[1])
        //                {
        //                    if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
        //                    {
        //                        string[] statObjects = dictAddedStatistics[dictkey].ToString().Split('@');
        //                        DicTemp.Add(hashTagDetails[key] + "@" + statObjects[0], keys[0]);
        //                    }
        //                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
        //                    {
        //                        Dilimitor = "_&";
        //                        string[] stObjs = dictAddedStatistics[dictkey].Split(new[] { Dilimitor }, StringSplitOptions.None);
        //                        DicTemp.Add(hashTagDetails[key] + "@" + stObjs[0], keys[0]);
        //                    }
        //                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
        //                    {
        //                        if (dictAddedStatistics[dictkey].Contains('@'))
        //                        {
        //                            string[] statObjects = dictAddedStatistics[dictkey].ToString().Split('@');
        //                            DicTemp.Add(hashTagDetails[key] + "@" + statObjects[0], keys[0]);
        //                        }
        //                        else if (dictAddedStatistics[dictkey].Contains("_&"))
        //                        {
        //                            Dilimitor = "_&";
        //                            string[] stObjs = dictAddedStatistics[dictkey].Split(new[] { Dilimitor }, StringSplitOptions.None);
        //                            DicTemp.Add(hashTagDetails[key] + "@" + stObjs[0], keys[0]);
        //                        }
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("MainWindowViewModel : StatTickerAddTaggedStats Method : " + ex.Message);
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //        logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : Method Exit");
        //    }
        //}
        ///// <summary>
        ///// Stats the ticker_ add tagged stats.
        ///// </summary>
        //private void StatTickerAddTaggedStats()
        //{
        //    string strDilimitor = "_&";
        //    try
        //    {
        //        logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : Method Entry");
        //        bool addHashTaggedStats = true;
        //        SortedList statDict = new SortedList();
        //        Settings.GetInstance().DictTaggedStats = objStatTicker.GetTaggedStats();
        //        foreach (string statName in Settings.GetInstance().DictTaggedStats.Keys)
        //        {
        //            string[] tagn = statName.Split(',');
        //            if (!dictAddedStatistics.ContainsKey(Convert.ToInt32(tagn[0])))
        //            {
        //                dictAddedStatistics.Add(Convert.ToInt32(tagn[0]), tagn[1] + "_&" + Settings.GetInstance().DictTaggedStats[statName].ToString());
        //            }
        //            string[] statObjects = statName.Split(',', '@');
        //            string[] statobj = statObjects[1].Split('\n');
        //            listTaggedStatObjects.Add(statobj[0]);
        //        }
        //        MyTagControlCollection2 = new ObservableCollection<UserControl>();
        //        MyTagControlTempCollection = new ObservableCollection<UserControl>();
        //        RemainTags = dictAddedStatistics.Count;
        //        int currentTagno = 0;
        //        if (dictAddedStatistics.Count > 0)
        //        {
        //            #region Add Tagged Stats
        //            foreach (string stats in dictAddedStatistics.Values)
        //            {
        //                currentTagno++;
        //                string[] statName = stats.Split(new[] { strDilimitor }, StringSplitOptions.None);
        //                if (addHashTaggedStats)
        //                {
        //                    TagNo = currentTagno;
        //                    addHashTaggedStats = false;
        //                    foreach (string statTagged in dictAddedStatistics.Values)
        //                    {
        //                        string TaggedStatList = ConfigurationManager.AppSettings.Get(Settings.GetInstance().UserName + "_Statistics");
        //                        if (TaggedStatList == null)
        //                        {
        //                            string appPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        //                            string configFile = System.IO.Path.Combine(appPath, Settings.execonfig);
        //                            ExeConfigurationFileMap configFileMap = new ExeConfigurationFileMap();
        //                            configFileMap.ExeConfigFilename = configFile;
        //                            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configFileMap, ConfigurationUserLevel.None);
        //                            KeyValueConfigurationElement tempvalue = config.AppSettings.Settings[Settings.GetInstance().UserName + "_Statistics"];
        //                            if (tempvalue != null)
        //                                TaggedStatList = tempvalue.Value;
        //                            else
        //                                TaggedStatList = null;
        //                        }
        //                        if (TaggedStatList != string.Empty && TaggedStatList != null)
        //                        {
        //                            string[] statlist = TaggedStatList.Split(',');
        //                            foreach (string statsname in statlist)
        //                            {
        //                                string[] statorder = statsname.Split(new[] { strDilimitor }, StringSplitOptions.None);
        //                                if (TagNo.ToString() == statorder[0])
        //                                {
        //                                    if (!statDict.ContainsKey(statorder[0]))
        //                                        statDict.Add(statorder[0], statorder[1]);
        //                                }
        //                            }
        //                        }
        //                        string statval = string.Empty;
        //                        foreach (int entry in dictAddedStatistics.Keys)
        //                        {
        //                            if (dictAddedStatistics[entry].ToString() == statTagged)
        //                            {
        //                                statval = entry.ToString();
        //                            }
        //                        }
        //                        string tagName = "tag_" + TagNo;
        //                        string[] stattags = statTagged.Split(new[] { strDilimitor }, StringSplitOptions.None);
        //                        string[] stats1 = stattags[0].Split('@');
        //                        if (!hashTaggedStats.Contains(tagName))
        //                        {
        //                            if (statDict.Count != 0)
        //                            {
        //                                string stattool1 = statDict[TagNo.ToString()].ToString();
        //                                string[] tooltip1 = stattool1.Split('@');
        //                                if (tooltip1.Length > 1)
        //                                    hashTaggedStats.Add(tagName, stats1[1] + "_@" + tooltip1[1] + "\n" + tooltip1[0]);
        //                                else
        //                                    hashTaggedStats.Add(tagName, stats1[0] + "_@" + tooltip1[0]);
        //                            }
        //                            else
        //                            {
        //                                string[] tagstatname = statName[0].Split('@');
        //                                if (stats1.Length > 1)
        //                                    hashTaggedStats.Add(tagName, stats1[1]);
        //                                else
        //                                    hashTaggedStats.Add(tagName, stats1[0]);
        //                            }
        //                        }
        //                        TagNo++;
        //                    }
        //                }
        //                TagNo = currentTagno;
        //                userTagControl = new TagGadgetControl();
        //                if (removedNo.Count != 0)
        //                {
        //                    userTagControl.Tag = removedNo.Dequeue();
        //                }
        //                else
        //                {
        //                    userTagControl.Tag = "tag_" + TagNo;
        //                }
        //                userTagControl.DataContext = this;
        //                if (Settings.GetInstance().DictEnableDisableChannels["untagbutton"])
        //                {
        //                    TagStatValueMargin = new Thickness(0, 0, 2, 0);
        //                }
        //                else
        //                {
        //                    TagStatValueMargin = new Thickness(15, 0, 2, 0);
        //                }
        //                string NewStatName = statName[1];
        //                userTagControl.lblStatName.Text = NewStatName;
        //                userTagControl.lblStatName.Tag = statName[0];
        //                userTagControl.MainGrid.Background = BackgroundColor;
        //                userTagControl.lblStatValue.Text = StatValue ?? "-";
        //                hashTagDetails.Add(statName[1] + "_@" + TagNo, userTagControl.Tag);
        //                tempTagheight = TagGadgetHeight;
        //                if (statDict.Count != 0)
        //                {
        //                    string stattool = statDict[TagNo.ToString()].ToString();
        //                    string[] tooltip = stattool.Split('@');
        //                    StatToolTip = tooltip[1] + "\n" + tooltip[0];
        //                    if (tooltip.Length > 1)
        //                        userTagControl.lblStatName.ToolTip = tooltip[1] + "\n" + tooltip[0];
        //                    else
        //                        userTagControl.lblStatName.ToolTip = tooltip[0];
        //                }
        //                string Dilimitor = "\n";
        //                if (userTagControl.lblStatName.ToolTip != null)
        //                {
        //                    string[] StatObj = userTagControl.lblStatName.ToolTip.ToString().Split(new[] { Dilimitor }, StringSplitOptions.None);
        //                    // userTagControl.lblStatObj.Text = StatObj[0].ToString();
        //                    TagStatisticsObject = StatObj[0].ToString();
        //                }
        //                ((MainWindowViewModel)userTagControl.DataContext).GadgetTagValue = "tag_" + TagNo;
        //                if (Settings.GetInstance().HshAllTagNames != null)
        //                {
        //                    if (!Settings.GetInstance().HshAllTagNames.ContainsKey(statName[0]))
        //                    {
        //                        if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("tagvertical"))
        //                        {
        //                            if (Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
        //                            {
        //                                logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : TagVertical : True");
        //                                Tag_Width = (int)GadgetWidth;
        //                                if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                                {
        //                                    WrapHorizontalWidth = (Tag_Width * TagNo);
        //                                }
        //                                else
        //                                {
        //                                    WrapHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
        //                                }
        //                                WrapWidth = Tag_Width;
        //                                WrapVerticalHeight = (((int)userTagControl.MainBorder.Height)) * TagNo;
        //                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
        //                                {
        //                                    if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                                    {
        //                                        NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth) - 1;
        //                                        NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
        //                                        WindowHorizontalWidth = (Tag_Width * (NumberOfTags_Horizontally)) + (int)GadgetWidth;
        //                                        WindowVerticalWidth = Tag_Width;
        //                                        WindowHorizontalHeight = (int)userControl.MainBorder.Height;
        //                                        WindowVerticalHeight = (int)userControl.MainBorder.Height + (((int)userTagControl.MainBorder.Height) * TagNo);
        //                                    }
        //                                    else
        //                                    {
        //                                        NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth);
        //                                        NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
        //                                        WindowHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
        //                                        WindowVerticalWidth = Tag_Width;
        //                                        WindowHorizontalHeight = (int)userTagControl.MainBorder.Height * 2;
        //                                        WindowVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;
        //                                    }
        //                                }
        //                                if (TagNo > NumberOfTags_Horizontally)
        //                                    TotalTagsExceeded++;
        //                                if (TagNo == 1)
        //                                {
        //                                    userTagControl.MainGrid.Margin = new Thickness(2, 2, 2, 2);
        //                                }
        //                                else
        //                                {
        //                                    userTagControl.MainGrid.Margin = new Thickness(2, 0, 2, 2);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : TagVertical : False");
        //                                Tag_Width = (int)GadgetWidth;
        //                                if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                                {
        //                                    WrapHorizontalWidth = (Tag_Width * TagNo);
        //                                }
        //                                else
        //                                {
        //                                    WrapHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
        //                                }
        //                                WrapWidth = Tag_Width * NumberOfTags_Horizontally;
        //                                WrapVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;
        //                                if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
        //                                {
        //                                    if (Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                                    {
        //                                        NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth) - 1;
        //                                        NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
        //                                        WindowHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally) + (int)GadgetWidth;
        //                                        WindowVerticalWidth = Tag_Width;
        //                                        WindowHorizontalHeight = (int)userControl.MainBorder.Height;
        //                                        WindowVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;
        //                                    }
        //                                    else
        //                                    {
        //                                        NumberOfTags_Horizontally = (int)(Screen.PrimaryScreen.WorkingArea.Width / GadgetWidth);
        //                                        NumberOfTags_Vertically = (int)(Screen.PrimaryScreen.WorkingArea.Height / TagGadgetHeight);
        //                                        WindowHorizontalWidth = (Tag_Width * NumberOfTags_Horizontally);
        //                                        WindowVerticalWidth = Tag_Width;
        //                                        WindowHorizontalHeight = (int)userTagControl.MainBorder.Height * 2;
        //                                        WindowVerticalHeight = ((int)userTagControl.MainBorder.Height) * TagNo;
        //                                    }
        //                                }
        //                                if (TagNo > NumberOfTags_Horizontally)
        //                                {
        //                                    TotalTagsExceeded++;
        //                                    userTagControl.MainGrid.Margin = new Thickness(0, 0, 2, 2);
        //                                }
        //                                else
        //                                {
        //                                    userTagControl.MainGrid.Margin = new Thickness(0, 2, 2, 2);
        //                                }
        //                            }
        //                        }
        //                        if ((!Settings.GetInstance().DictEnableDisableChannels["tagvertical"] && TagNo <= (NumberOfTags_Horizontally * 2)) || !Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                        {
        //                            MyTagControlCollection.Add(userTagControl);
        //                            Wrap2Visibility = Visibility.Collapsed;
        //                            isThirdRowActive = false;
        //                        }
        //                        else
        //                        {
        //                            if (!isThirdRowActive)
        //                            {
        //                                Wrap2Visibility = Visibility.Visible;
        //                                Wrap2Margin = new Thickness(0, 0, 0, 0);
        //                                Wrap2Rows = ++Wrap2Rows;
        //                            }
        //                            MyTagControlCollection2.Add(userTagControl);
        //                            Wrap2Collection.Add(userTagControl);
        //                            isThirdRowActive = true;
        //                            if (((float)MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1) >= 1) && ((float)MyTagControlCollection2.Count / (NumberOfTags_Horizontally + 1) > Wrap2Rows))
        //                                Wrap2Rows = ++Wrap2Rows;
        //                            WrapWidth = 0;
        //                        }
        //                        MyTagControlTempCollection.Add(userTagControl);
        //                        if (!Settings.GetInstance().HshAllTagNames.ContainsKey(statName[0]))
        //                            Settings.GetInstance().HshAllTagNames.Add(statName[0], userTagControl);
        //                        if (!Settings.GetInstance().HshAllTagList.ContainsKey(userTagControl.Tag.ToString()))
        //                            Settings.GetInstance().HshAllTagList.Add(userTagControl.Tag.ToString(), userTagControl);
        //                        RemainTags--;
        //                    }
        //                    else
        //                    {
        //                        TagNo--;
        //                    }
        //                }
        //            }
        //            #endregion
        //        }
        //        else
        //        {
        //            if (Settings.GetInstance().DictEnableDisableChannels.ContainsKey("maingadget"))
        //            {
        //                if (!Settings.GetInstance().DictEnableDisableChannels["maingadget"])
        //                {
        //                    Views.MessageBox msgbox = new Views.MessageBox();
        //                    string errorList = "No Statistics Configured for the particular agent.";
        //                    ViewModels.MessageBoxViewModel mboxviewmodel;
        //                    if (StatisticsBase.GetInstance().isPlugin)
        //                    {
        //                        mboxviewmodel = new ViewModels.MessageBoxViewModel("Problem Encountered", errorList, msgbox, "MainWindow", Settings.GetInstance().Theme);
        //                    }
        //                    else
        //                    {
        //                        mboxviewmodel = new ViewModels.MessageBoxViewModel("Problem Encountered", errorList, msgbox, "LoginWindow", Settings.GetInstance().Theme);
        //                    }
        //                    msgbox.DataContext = mboxviewmodel;
        //                    msgbox.ShowDialog();
        //                    foreach (Window window in Application.Current.Windows)
        //                    {
        //                        if (window.Title == "StatGadget")
        //                        {
        //                            window.Close();
        //                        }
        //                    }
        //                    objStatTicker.ShowGadgetState(Pointel.Statistics.Core.Utility.StatisticsEnum.GadgetState.Ended);
        //                }
        //            }
        //        }
        //        HeaderValues();
        //        TagOrientation();
        //        foreach (string key in hashTagDetails.Keys)
        //        {
        //            string Dilimitor = "_@";
        //            string[] keys = key.Split(new[] { Dilimitor }, StringSplitOptions.None);
        //            foreach (int dictkey in dictAddedStatistics.Keys)
        //            {
        //                if (dictkey.ToString() == keys[1])
        //                {
        //                    if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
        //                    {
        //                        string[] statObjects = dictAddedStatistics[dictkey].ToString().Split('@');
        //                        DicTemp.Add(hashTagDetails[key] + "@" + statObjects[0], keys[0]);
        //                    }
        //                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
        //                    {
        //                        Dilimitor = "_&";
        //                        string[] stObjs = dictAddedStatistics[dictkey].Split(new[] { Dilimitor }, StringSplitOptions.None);
        //                        DicTemp.Add(hashTagDetails[key] + "@" + stObjs[0], keys[0]);
        //                    }
        //                    else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
        //                    {
        //                        if (dictAddedStatistics[dictkey].Contains('@'))
        //                        {
        //                            string[] statObjects = dictAddedStatistics[dictkey].ToString().Split('@');
        //                            DicTemp.Add(hashTagDetails[key] + "@" + statObjects[0], keys[0]);
        //                        }
        //                        else if (dictAddedStatistics[dictkey].Contains("_&"))
        //                        {
        //                            Dilimitor = "_&";
        //                            string[] stObjs = dictAddedStatistics[dictkey].Split(new[] { Dilimitor }, StringSplitOptions.None);
        //                            DicTemp.Add(hashTagDetails[key] + "@" + stObjs[0], keys[0]);
        //                        }
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.Error("MainWindowViewModel : StatTickerAddTaggedStats Method : " + ex.Message);
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //        logger.Debug("MainWindowViewModel : StatTickerAddTaggedStats : Method Exit");
        //    }
        //}
        //public void gadgetclose()
        //{
        //    try
        //    {
        //        logger.Debug("MainWindowViewModel : GadgetClose : Method Entry");
        //        #region Close Gadget
        //        if (!StatisticsBase.GetInstance().isPlugin)
        //        {
        //            isMinimized = false;
        //            int tagno = 1;
        //            string statDisplayName = string.Empty;
        //            taggedStats = string.Empty;
        //            Dictionary<string, string> DictTaggedStats = new Dictionary<string, string>();
        //            string ServerTaggedStats = string.Empty;
        //            string DBTaggedStats = string.Empty;
        //            output = new OutputValues();
        //            if (hashTaggedStats.Count > 0)
        //            {
        //                SortedDictionary<int, string> tempdic = new SortedDictionary<int, string>();
        //                if (MyTagControlCollection != null)
        //                {
        //                    foreach (TagGadgetControl temptagcontrol in MyTagControlCollection)
        //                    {
        //                        foreach (DictionaryEntry entry in hashTaggedStats)
        //                        {
        //                            if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
        //                            {
        //                                string[] keyvalues = entry.Key.ToString().Split('_');
        //                                int tagn = Convert.ToInt32(keyvalues[1]);
        //                                tempdic.Add(tagn, entry.Value.ToString());
        //                            }
        //                        }
        //                    }
        //                }
        //                if (MyTagControlCollection2 != null)
        //                {
        //                    foreach (TagGadgetControl temptagcontrol in MyTagControlCollection2)
        //                    {
        //                        foreach (DictionaryEntry entry in hashTaggedStats)
        //                        {
        //                            if (temptagcontrol.Tag.ToString() == entry.Key.ToString())
        //                            {
        //                                string[] keyvalues = entry.Key.ToString().Split('_');
        //                                int tagn = Convert.ToInt32(keyvalues[1]);
        //                                if (!tempdic.ContainsKey(tagn))
        //                                    tempdic.Add(tagn, entry.Value.ToString());
        //                            }
        //                        }
        //                    }
        //                }
        //                foreach (string statName in tempdic.Values)
        //                {
        //                    string Dilimitor = "_@";
        //                    string[] stat = statName.Split(new[] { Dilimitor }, StringSplitOptions.None);
        //                    string[] statobj = stat[1].Split('\n');
        //                    if (taggedStats.Equals(string.Empty) && statDisplayName.Equals(string.Empty))
        //                    {
        //                        if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
        //                        {
        //                            statDisplayName = tagno + "@" + statobj[0];
        //                            DBTaggedStats = stat[0];
        //                        }
        //                        else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
        //                        {
        //                            statDisplayName = tagno + "_&" + statobj[1] + "@" + statobj[0];
        //                            ServerTaggedStats = statobj[0] + "@" + stat[0];
        //                        }
        //                        else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
        //                        {
        //                            if (statobj.Length > 1)
        //                            {
        //                                statDisplayName = tagno + "_&" + statobj[1] + "@" + statobj[0];
        //                                ServerTaggedStats = statobj[0] + "@" + stat[0];
        //                            }
        //                            else
        //                            {
        //                                statDisplayName = tagno + "@" + statobj[0];
        //                                DBTaggedStats = stat[0];
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.DB.ToString())
        //                        {
        //                            statDisplayName = statDisplayName + "," + tagno + "@" + statobj[0];
        //                            DBTaggedStats = DBTaggedStats + "," + stat[0];
        //                        }
        //                        else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.StatServer.ToString())
        //                        {
        //                            statDisplayName = statDisplayName + "," + tagno + "_&" + statobj[1] + "@" + statobj[0];
        //                            ServerTaggedStats = ServerTaggedStats + "," + statobj[0] + "@" + stat[0];
        //                        }
        //                        else if (Settings.GetInstance().ApplicationType == StatisticsEnum.StatSource.All.ToString())
        //                        {
        //                            if (statobj.Length > 1)
        //                            {
        //                                statDisplayName = statDisplayName + "," + tagno + "_&" + statobj[1] + "@" + statobj[0];
        //                                ServerTaggedStats = ServerTaggedStats + "," + statobj[0] + "@" + stat[0];
        //                            }
        //                            else
        //                            {
        //                                statDisplayName = statDisplayName + "," + tagno + "@" + statobj[0];
        //                                DBTaggedStats = DBTaggedStats + "," + stat[0];
        //                            }
        //                        }
        //                    }
        //                    tagno++;
        //                }
        //                if (!DictTaggedStats.ContainsKey(StatisticsEnum.StatSource.StatServer.ToString()))
        //                    DictTaggedStats.Add(StatisticsEnum.StatSource.StatServer.ToString(), ServerTaggedStats);
        //                if (!DictTaggedStats.ContainsKey(StatisticsEnum.StatSource.DB.ToString()))
        //                    DictTaggedStats.Add(StatisticsEnum.StatSource.DB.ToString(), ServerTaggedStats);
        //                output = objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);
        //                // if commented for dont write statistics on stattickerfive.exe.config file -02/09/2014
        //                // if (output.MessageCode == "2007")
        //                //   SaveTaggedStatistics(statDisplayName);
        //            }
        //            else
        //            {
        //                objStatTicker.SaveAgentsTaggedStats(DictTaggedStats, Settings.GetInstance().DictEnableDisableChannels["tagvertical"], AppTop, AppLeft, isShowHeader);
        //                // if commented for dont write statistics on stattickerfive.exe.config file -02/09/2014
        //                // if (output.MessageCode == "2007")
        //                //   SaveTaggedStatistics(string.Empty);
        //            }
        //           // objNotify.Visible = false;
        //            Environment.Exit(0);
        //        }
        //        else
        //        {
        //            foreach (Window window in Application.Current.Windows)
        //                if (window.Title == "StatGadget")
        //                    window.Hide();
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        //System.Windows.MessageBox.Show("MainWindowViewModel : gadgetclose : " + ex.Message);
        //        logger.Error("MainWindowViewModel : gadgetclose : " + ex.Message);
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //        logger.Debug("MainWindowViewModel : GadgetClose : Method Exit");
        //    }
        //}

        #endregion Other
    }
}