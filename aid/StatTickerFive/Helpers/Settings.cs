using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using Genesyslab.Platform.Commons.Collections;
using StatTickerFive.ViewModels;
using StatTickerFive.Views;

namespace StatTickerFive.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    public class Settings:NotificationObject
    {
        static Settings singletonInstance;
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static Settings GetInstance()
        {
            if (singletonInstance == null)
            {
                singletonInstance = new Settings();
                return singletonInstance;
            }
            return singletonInstance;
        }

        public static Settings Instance
        {
            get
            {
                if (singletonInstance == null)
                {
                    singletonInstance = new Settings();
                    return singletonInstance;
                }
                return singletonInstance;
            }
        }

        public ViewModels.AdminConfigWindowViewModel adminConfigVM = null;
        public string UserName = string.Empty;
        public string Password = string.Empty;
        public string Host = string.Empty;
        public string Port = string.Empty;
        public string ClientName = string.Empty;
        public string ApplicationName = string.Empty;
        public bool IsPasswordEnabled = false;
        public bool IsHostEnabled = false;
        public bool IsPortEnabled = false;
        public bool IsAppNameEnabled = false;
        public bool IsAppTypeEnabled = false;
        public string DefaultAuthentication = string.Empty;
        public string ApplicationType = string.Empty;
        public int DisplayTime = 0;
        public string Position = string.Empty;
        public double Width = 0;
        public bool IsStatBold = false;
        public int ErrorDisplayCount = 0;
        public bool RunMultipleInstances = false;
        public bool ReadApplicationConfigServer = false;
        public string StatSource = string.Empty;
        public string Theme = string.Empty;
        public bool AllowTrans = false;
        public bool IsCMEAuthenticated;
        public bool IsDBAuthenticated;
        public string DefaultUsername = string.Empty;
        public string DefaultPassword = string.Empty;

        public string Place = string.Empty;
        public int StatCount;

        //For Admin Seperate Application
        public MainWindow mainview = null;
        public MainWindowViewModel mainVm = null;

        public string CurrStatRefId;
        //added
        public bool IsPluginReaded = false;
        public bool isMinimized;

        public string conversionPattern = string.Empty;
        public string datePattern = string.Empty;
        public string logLevel = string.Empty;
        public string logFileName = string.Empty;
        public string maxFileSize = string.Empty;
        public string maxSizeRoll = string.Empty;
        public string logFilePath = string.Empty;
        public string logUserName = string.Empty;

        public bool isFirstTime = false;
        public bool isStatConfigured = false;
        public bool isThresholdNotifierEnded = true;

        //DB values
        public string dbHost = string.Empty;
        public string dbPort = string.Empty;
        public string dbType = string.Empty;
        public string dbName = string.Empty;
        public string dbUsername = string.Empty;
        public string dbPassword = string.Empty;
        public string dbSid = string.Empty;
        public string dbSname = string.Empty;
        public string dbSource = string.Empty;
        public string dbLoginQuery = string.Empty;

        private ContextMenu _gadgetContextMenu;
        /// <summary>
        /// Gets or sets the gadget context menu.
        /// </summary>
        /// <value>The gadget context menu.</value>
        public ContextMenu GadgetContextMenu
        {
            get { return _gadgetContextMenu; }
            set { _gadgetContextMenu = value; }
        }

        public Dictionary<string, bool> DictEnableDisableChannels = new Dictionary<string, bool>();
        public Dictionary<string, string> DictErrorValues = new Dictionary<string, string>();
        public Dictionary<string, bool> DictThresholdBreach = new Dictionary<string, bool>();

        public Hashtable HshAllTagNames = new Hashtable();
        public Hashtable HshAllTagList = new Hashtable();
        public Hashtable HshStatInfo = new Hashtable();

        public SortedDictionary<string, StatisticsInfo> DicStatInfo = new SortedDictionary<string, StatisticsInfo>();


        public Dictionary<string, string> DictTaggedStats = new Dictionary<string, string>();
        public Dictionary<string, string> DictAllStats = new Dictionary<string, string>();
        public Dictionary<string, string> DictScreenProperties = new Dictionary<string, string>();

        public const string ipValidation = @"^(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])$";
        public const string portValidation = @"^\d{1,5}$";

        public const string execonfig = "StatTickerFive.exe.config";

        //Admin Objects
        //public bool isConfigNewStats = false;
        //public bool isConfigureNextStat = false;
        public bool IsConfigureStats = false;
        public bool IsCancelSaving = false;

        //Application Annex Values
        public Dictionary<string, Dictionary<string, string>> DictExistingApplicationStats = new Dictionary<string,Dictionary<string,string>>();

        //Server Statistics
        public Dictionary<string, KeyValueCollection> DictServerStatistics = new Dictionary<string, KeyValueCollection>();

        //Server Filters
        public Dictionary<string, string> DictServerFilters = null;

        //Statistic Type
        public Dictionary<string, string> DictStatFormats = null;

        //Statistics for all types
        public List<string> ListAgentStatistics = new List<string>();
        public List<string> ListAgentGroupStatistics = new List<string>();
        public List<string> ListACDQueueStatistics = new List<string>();
        public List<string> ListGroupQueueStatistics = new List<string>();
        public List<string> ListVirtualQueueStatistics = new List<string>();

        //StatisticsDescription
        public Dictionary<string, string> DictStatisticsDesc = new Dictionary<string, string>();

        //Configured Statistics Collection
        public Dictionary<string, Dictionary<string, string>> DictConfigStats = new Dictionary<string, Dictionary<string, string>>();
        public Dictionary<string, Dictionary<string, string>> DictNewStats = new Dictionary<string, Dictionary<string, string>>();

        //Property Exist
        public bool IsPropertyExist;

        //Configuration to be overwrite
        public bool ToOverWrite = false;
        //Statistics Viewmodel object instance
        public ViewModels.StatisticsWindowViewModel editVMObj = null;
        //Selected format value
        public string SelectedFormat = string.Empty;
        //Threshold values
        public string TValue1 = string.Empty;
        public string TValue2 = string.Empty;
        
        //Configured statistics
        public Dictionary<string, List<string>> LstConfiguredAgentStats = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> LstConfiguredAGroupStats = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> LstConfiguredACDStats = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> LstConfiguredDNStats = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> LstConfiguredVQStats = new Dictionary<string, List<string>>();

        //Configured Statistics Types
        public bool IsAgentRemain = true;
        public bool IsAGroupRemain = true;
        public bool IsACDRemain = true;
        public bool IsDNRemain = true;
        public bool IsVQRemain = true;

        //Existing (or) New Statistics
        public bool IsExistingStat = false;

        //Save configured objects
        public bool IsSaveConfiguredObjs = false;

        //Style colors
        private string _mouseOver;
        public string MouseOver
        {
            get
            {
                return _mouseOver;
            }
            set
            {
                _mouseOver = value;
                RaisePropertyChanged(() => MouseOver);
            }
        }

        private string _mousePressed;
        public string MousePressed
        {
            get
            {
                return _mousePressed;
            }
            set
            {
                _mousePressed = value;
                RaisePropertyChanged(() => MousePressed);
            }
        }

        private string _Forecolor;
        public string Forecolor
        {
            get
            {
                return _Forecolor;
            }
            set
            {
                _Forecolor = value;
                RaisePropertyChanged(() => Forecolor);
            }
        }

        private string _Backcolor;
        public string Backcolor
        {
            get
            {
                return _Backcolor;
            }
            set
            {
                _Backcolor = value;
                RaisePropertyChanged(() => Backcolor);
            }
        }

        private string _TitleBackcolor;
        public string TitleBackcolor
        {
            get
            {
                return _TitleBackcolor;
            }
            set
            {
                _TitleBackcolor = value;
                RaisePropertyChanged(() => TitleBackcolor);
            }
        }

        //Object Collections
        public List<string> LstAgentList = new List<string>();
        public List<string> LstAgentGroupList = new List<string>();

        //ObjectStatistics Collection
        public Dictionary<string, List<string>> DictAgentStatisitics = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> DictAgentGroupStatisitics = new Dictionary<string, List<string>>();

        public Dictionary<string, Dictionary<string, List<string>>> DictAgentObjects = new Dictionary<string, Dictionary<string, List<string>>>();        
        public Dictionary<string, Dictionary<string, List<string>>> DictAgentGroupObjects = new Dictionary<string, Dictionary<string, List<string>>>();

        public Dictionary<string, Dictionary<string, List<string>>> DictApplicationObjects = new Dictionary<string, Dictionary<string, List<string>>>();

        public Dictionary<string, Dictionary<List<string>, List<string>>> ExistingValuesDictionary = new Dictionary<string, Dictionary<List<string>, List<string>>>();

        //New Statistics for single object
        public List<string> LstNewStatistics = new List<string>();

        //Dynamic Tab Item ViewModel
        public TabValueViewModel ObjTabVm;

        //Newly Added statistics
        public Dictionary<string, List<ObjectValues>> DictUpdatedStatistics = new Dictionary<string, List<ObjectValues>>();

        //Configure new objects in agent group and agent level
        public bool IsConfigureLevelObjects = false;

        //Store new Objects in agent group and agent level
        //public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> NewStatisticsDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
       //Session variable for checked stats at Userlevel
        public Dictionary<string, List<ObjectValues>> CheckedStatList = new Dictionary<string, List<ObjectValues>>();

        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> ConfiguredObjectsDictionary = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();

        //Store new Objects in agent group and agent level
        public Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>> DictConfiguredStatisticsObjects = new Dictionary<string, Dictionary<string, Dictionary<string, List<string>>>>();
        public bool IsApplicationObjectConfigWindow;


        //Configured statistics
        public Dictionary<string, List<string>> DictConfiguredAgentStats = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> DictConfiguredAGroupStats = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> DictConfiguredACDStats = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> DictConfiguredDNStats = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> DictConfiguredVQStats = new Dictionary<string, List<string>>();

        //ADDP Timeout
        public int AddpServerTimeOut = 0;
        public int AddpClientTimeOut = 0;

        //Queue Selection
        public bool IsQueueSelect = false;
        public bool IsGadgetShow = false;
        //Object Configuration Window
        public ObjectConfigWindowViewModel QueueConfigVM = null;
        public ObjectConfigWindow QueueConfigWin;
    }
}
