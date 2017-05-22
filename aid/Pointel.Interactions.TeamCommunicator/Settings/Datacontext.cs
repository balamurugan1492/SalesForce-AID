/*
* =====================================
* Pointel.Interactions.TeamCommunicator.Settings
* ====================================
* Project    : Agent Interaction Desktop
* Created on : 05-Sep-2014
* Author     : Manikandan
* Owner      : Pointel Solutions
* ====================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq.Expressions;
using System.Windows.Controls;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel;
using Genesyslab.Platform.ApplicationBlocks.ConfigurationObjectModel.CfgObjects;
using Genesyslab.Platform.Commons.Protocols;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Pointel.Interactions.TeamCommunicator.Helpers;
using Version = Lucene.Net.Util.Version;
using System.IO;
using Genesyslab.Platform.Reporting.Protocols;
using Genesyslab.Platform.ApplicationBlocks.Commons.Protocols;

namespace Pointel.Interactions.TeamCommunicator.Settings
{
    public class Datacontext : INotifyPropertyChanged
    {
        #region Single instance

        private static Datacontext _instance = null;

        public static Datacontext GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Datacontext();
                return _instance;
            }
            return _instance;
        }

        #endregion Single instance

        public event PropertyChangedEventHandler PropertyChanged;
        private ObservableCollection<ITeamCommunicator> _teamCommunicator = new ObservableCollection<ITeamCommunicator>();
        private Pointel.Interactions.IPlugins.InteractionType _currentInteractionType;
        private Pointel.Interactions.IPlugins.OperationType _selectedOperationType;
        public System.Windows.Media.Imaging.BitmapImage CurrentMediaImageSource = new System.Windows.Media.Imaging.BitmapImage();

        private Datacontext.SelectorFilters _selectedType;
        private string _userName = "";
        private string _internalTargets;
        private string _favoriteDisplayName;
        private string _selectedItemType;
        private string _uniqueIdentity;
        private string _routingAddress;
        private string _selectedDN = string.Empty;
        private string _logFilterLevel = "AID";
        private string _corporateFavoriteFile = "";

        private int _maxSuggestionSize = 10;
        private int _recentMaxRecords = 10;
        private int _maxFavouriteSize = 50;
        private int _tenantDBID;
        private string _tenantName;


        private List<SelectorFilters> _filterList = new List<SelectorFilters>();
        private List<string> _internalFavoriteList = new List<string>();
        private List<string> _customFavoriteList = new List<string>();
        private List<string> _categoryNames = new List<string>();
        private List<string> _emailWorkbins = new List<string>();
        private List<string> _teamWorkbins = new List<string>();

        private bool _isStatAlive;
        private bool _isNewFavoriteItem;
        private bool _isEditFavorite;
        private bool _isSupervisorMoveWorkbinEnabled = false;
        private bool _isSupervisorMoveQueueEnabled = false;
        private bool _isOpenStatServer = false;

        public DataTable dtFavorites = new DataTable();
        public DataTable dtRecentInteractions = new DataTable();
        public DataTable dtPersons = new DataTable();
        public DataTable dtAgentgroups = new DataTable();
        public DataTable dtSkills = new DataTable();
        public DataTable dtInteractionQueues = new DataTable();

        public Hashtable hshAgentGroupStatus = new Hashtable();//To store all agent groups and status
        public Dictionary<string, string> SearchedList = new Dictionary<string, string>();//To store the searched list
        public Hashtable hshAgentStatus = new Hashtable();//To store all agents employee id and agent status
        public Hashtable hshAgentVoiceStatus = new Hashtable();//To store all agents employee id and agent voice status
        public Hashtable hshAgentEmailStatus = new Hashtable();//To store all agents employee id and agent email status
        public Hashtable hshAgentChatStatus = new Hashtable();//To store all agents employee id and agent chat status
        public Hashtable hshAgentPlace = new Hashtable();//To store all agents employee id and place
        public Hashtable hshAgentDN = new Hashtable();//To store all agents employee id and dn
        public Hashtable hshAgentEmployeeIdUserName = new Hashtable();//To store all agents employee id and dn
        public Hashtable currentMediaStatus = new Hashtable();

        //Collection Values
        public IMessage ContactsIMessage = null;

        public Dictionary<string, Lucene.Net.Documents.Document> SearchedAgentDocuments = new Dictionary<string, Lucene.Net.Documents.Document>();
        public Dictionary<string, Lucene.Net.Documents.Document> SearchedAgentGroupDocuments = new Dictionary<string, Lucene.Net.Documents.Document>();
        public Dictionary<string, Lucene.Net.Documents.Document> SearchedSkillDocuments = new Dictionary<string, Lucene.Net.Documents.Document>();
        public Dictionary<string, Lucene.Net.Documents.Document> SearchedInteractionQueueDocuments = new Dictionary<string, Lucene.Net.Documents.Document>();
        public Dictionary<string, Lucene.Net.Documents.Document> SearchedContactDocuments = new Dictionary<string, Lucene.Net.Documents.Document>();

        public System.Windows.Controls.MenuItem MnuCall = new System.Windows.Controls.MenuItem();
        public System.Windows.Controls.MenuItem MnuAddFavorite = new System.Windows.Controls.MenuItem();
        public System.Windows.Controls.MenuItem MnuRemoveFavorite = new System.Windows.Controls.MenuItem();
        public System.Windows.Controls.MenuItem MnuEditFavorite = new System.Windows.Controls.MenuItem();
        public System.Windows.Controls.MenuItem MnuItemMedia = new System.Windows.Controls.MenuItem();

        public ContextMenu ActionMenu = new ContextMenu();

        //private ProtocolManagementService _protocolManager= null;
        public string StatServerName = "statistics";
        public ICollection<CfgPerson> AllPersons;

        public List<Pointel.Interactions.IPlugins.AgentMediaStatus> AvailableChatStatus = new List<Pointel.Interactions.IPlugins.AgentMediaStatus>();
        public List<Pointel.Interactions.IPlugins.AgentMediaStatus> AvailableEmailStatus = new List<Pointel.Interactions.IPlugins.AgentMediaStatus>();

        //---------------------------------------
        //Lucene

        public Analyzer Analyzer = new StandardAnalyzer(Version.LUCENE_29);
        //public Lucene.Net.Store.Directory Directory = new RAMDirectory();
        public Lucene.Net.Store.Directory Directory;// = FSDirectory.Open(new DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString()
        //+ @"\Pointel\temp"));
        public IndexWriter Writer;

        public enum SelectorFilters
        {
            AllTypes,
            Agent,
            AgentGroup,
            Skill,
            InteractionQueue,
            RoutingPoint,
            Queue,
            Contact,
            DN
        };

        public string[] ConfigAgentGroupAttributes = new string[] { "ContactType", "DBID", "LevelType", "LevelDBID", "DateTime", "RecentMediaStartDate", 
            "RecentMediaType", "RecentMediaState", "RecentMediaDirection", "RecentMediaStatus", "Category", "Name", "TenantName", "TenantPassword", "CorporateCategories" };

        public string[] ConfigAgentAttributes = new string[] { 
            "ContactType", "DBID", "LevelType", "LevelDBID", "DateTime", "RecentMediaStartDate", "RecentMediaType", "RecentMediaState", "RecentMediaDirection", 
            "RecentMediaStatus", "Category", "EmailAddress", "EmployeeID", "ExternalID", "FirstName", "LastName", 
            "Phones", "UserName", "TenantName", "TenantPassword", "CorporateCategories"
         };

        public string[] ConfigInteractionQueueAttributes = new string[] { 
            "ContactType", "DBID", "LevelType", "LevelDBID", "DateTime", "RecentMediaStartDate", "RecentMediaType", "RecentMediaState", "RecentMediaDirection", 
            "RecentMediaStatus", "Category", "Name", "TenantName", "TenantPassword", "MediaList", "DisplayName", 
            "CorporateCategories"
         };

        public string[] ConfigQueueAttributes = new string[] { 
            "ContactType", "DBID", "LevelType", "LevelDBID", "DateTime", "RecentMediaStartDate", "RecentMediaType", "RecentMediaState", "RecentMediaDirection", 
            "RecentMediaStatus", "Category", "Number", "Name", "Location", "TenantName", "TenantPassword", 
            "CorporateCategories"
         };

        public string[] ConfigDNAttributes = new string[] { 
            "ContactType", "DBID", "LevelType", "LevelDBID", "DateTime", "RecentMediaStartDate", "RecentMediaType", "RecentMediaState", "RecentMediaDirection",
            "RecentMediaStatus", "Category", "Number", "Name", "Location", "TenantName", "TenantPassword", 
            "CorporateCategories"
         };

        public string[] ConfigSkillAttributes = new string[] { "ContactType", "DBID", "LevelType", "LevelDBID", "DateTime", "RecentMediaStartDate",
            "RecentMediaType", "RecentMediaState", "RecentMediaDirection", "RecentMediaStatus", "Category", "TenantName", "TenantPassword", "Name", "CorporateCategories" };

        public string[] ConfigContactAttributes = new string[] { "ContactType", "DBID", "LevelType", "LevelDBID", "DateTime", "RecentMediaStartDate", 
            "RecentMediaType", "RecentMediaState", "RecentMediaDirection", "RecentMediaStatus", "Category", "EmailAddress", "EmployeeID", "ExternalID", "FirstName", "LastName", 
            "PhoneNumber", "UserName", "TenantName", "TenantPassword", "ContactId"
        };

        //---------------------------------------


        #region Properties

        public int TenantDBID
        {
            get
            {
                return _tenantDBID;
            }
            set
            {
                if (_tenantDBID != value)
                {
                    _tenantDBID = value;
                    RaisePropertyChanged(() => TenantDBID);
                }
            }
        }

        public string TenantName
        {
            get
            {
                return _tenantName;
            }
            set
            {
                if (_tenantName != value)
                {
                    _tenantName = value;
                    RaisePropertyChanged(() => TenantName);
                }
            }
        }

        public string CorporateFavoriteFile
        {
            get
            {
                return _corporateFavoriteFile;
            }
            set
            {
                if (_corporateFavoriteFile != value)
                {
                    _corporateFavoriteFile = value;
                    RaisePropertyChanged(() => CorporateFavoriteFile);
                }
            }
        }

        public string UniqueIdentity
        {
            get
            {
                return _uniqueIdentity;
            }
            set
            {
                if (_uniqueIdentity != value)
                {
                    _uniqueIdentity = value;
                    RaisePropertyChanged(() => UniqueIdentity);
                }
            }
        }

        public string LogFilterLevel
        {
            get
            {
                return _logFilterLevel;
            }
            set
            {
                if (_logFilterLevel != value)
                {
                    _logFilterLevel = value;
                    RaisePropertyChanged(() => LogFilterLevel);
                }
            }
        }

        public List<string> EmailWorkbins
        {
            get
            {
                return _emailWorkbins;
            }
            set
            {
                if (_emailWorkbins != value)
                {
                    _emailWorkbins = value;
                    RaisePropertyChanged(() => EmailWorkbins);
                }
            }
        }

        public List<string> TeamWorkbins
        {
            get
            {
                return _teamWorkbins;
            }
            set
            {
                if (_teamWorkbins != value)
                {
                    _teamWorkbins = value;
                    RaisePropertyChanged(() => TeamWorkbins);
                }
            }
        }

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

        public string SelectedDN
        {
            get
            {
                return _selectedDN;
            }
            set
            {
                if (_selectedDN != value)
                {
                    _selectedDN = value;
                    RaisePropertyChanged(() => SelectedDN);
                }
            }
        }

        public int MaxSuggestionSize
        {
            get
            {
                return _maxSuggestionSize;
            }
            set
            {
                if (_maxSuggestionSize != value)
                {
                    _maxSuggestionSize = value;
                    RaisePropertyChanged(() => MaxSuggestionSize);
                }
            }
        }

        public int RecentMaxRecords
        {
            get
            {
                return _recentMaxRecords;
            }
            set
            {
                if (_recentMaxRecords != value)
                {
                    _recentMaxRecords = value;
                    RaisePropertyChanged(() => RecentMaxRecords);
                }
            }
        }
        public int MaxFavouriteSize
        {
            get
            {
                return _maxFavouriteSize;
            }
            set
            {
                if (_maxFavouriteSize != value)
                {
                    _maxFavouriteSize = value;
                    RaisePropertyChanged(() => MaxFavouriteSize);
                }
            }
        }


        public List<string> CustomFavoriteList
        {
            get
            {
                return _customFavoriteList;
            }
            set
            {
                if (_customFavoriteList != value)
                {
                    _customFavoriteList = value;
                    RaisePropertyChanged(() => CustomFavoriteList);
                }
            }
        }

        public List<string> InternalFavoriteList
        {
            get
            {
                return _internalFavoriteList;
            }
            set
            {
                if (_internalFavoriteList != value)
                {
                    _internalFavoriteList = value;
                    RaisePropertyChanged(() => InternalFavoriteList);
                }
            }
        }

        public List<SelectorFilters> FilterList
        {
            get
            {
                return _filterList;
            }
            set
            {
                if (_filterList != value)
                {
                    _filterList = value;
                    RaisePropertyChanged(() => FilterList);
                }
            }
        }

        public string RoutingAddress
        {
            get
            {
                return _routingAddress;
            }
            set
            {
                if (_routingAddress != value)
                {
                    _routingAddress = value;
                    RaisePropertyChanged(() => RoutingAddress);
                }
            }
        }

        public string FavoriteItemSelectedType
        {
            get
            {
                return _selectedItemType;
            }
            set
            {
                if (_selectedItemType != value)
                {
                    _selectedItemType = value;
                    RaisePropertyChanged(() => FavoriteItemSelectedType);
                }
            }
        }

        public bool IsStatAlive
        {
            get
            {
                return _isStatAlive;
            }
            set
            {
                if (_isStatAlive != value)
                {
                    _isStatAlive = value;
                    RaisePropertyChanged(() => IsStatAlive);
                }
            }
        }

        public bool IsEditFavorite
        {
            get
            {
                return _isEditFavorite;
            }
            set
            {
                if (_isEditFavorite != value)
                {
                    _isEditFavorite = value;
                    RaisePropertyChanged(() => IsEditFavorite);
                }
            }
        }

        public bool IsFavoriteItem
        {
            get
            {
                return _isNewFavoriteItem;
            }
            set
            {
                if (_isNewFavoriteItem != value)
                {
                    _isNewFavoriteItem = value;
                    RaisePropertyChanged(() => IsFavoriteItem);
                }
            }
        }

        public bool IsSupervisorMoveWorkbinEnabled
        {
            get
            {
                return _isSupervisorMoveWorkbinEnabled;
            }
            set
            {
                if (_isSupervisorMoveWorkbinEnabled != value)
                {
                    _isSupervisorMoveWorkbinEnabled = value;
                    RaisePropertyChanged(() => IsSupervisorMoveWorkbinEnabled);
                }
            }
        }

        public bool IsSupervisorMoveQueueEnabled
        {
            get
            {
                return _isSupervisorMoveQueueEnabled;
            }
            set
            {
                if (_isSupervisorMoveQueueEnabled != value)
                {
                    _isSupervisorMoveQueueEnabled = value;
                    RaisePropertyChanged(() => IsSupervisorMoveQueueEnabled);
                }
            }
        }

        public bool IsOpenStatServer
        {
            get
            {
                return _isOpenStatServer;
            }
            set
            {
                if (_isOpenStatServer != value)
                {
                    _isOpenStatServer = value;
                    RaisePropertyChanged(() => IsOpenStatServer);
                }
            }
        }

        public string FavoriteDisplayName
        {
            get
            {
                return _favoriteDisplayName;
            }
            set
            {
                if (_favoriteDisplayName != value)
                {
                    _favoriteDisplayName = value;
                    RaisePropertyChanged(() => FavoriteDisplayName);
                }
            }
        }

        public List<string> CategoryNamesList
        {
            get
            {
                return _categoryNames;
            }
            set
            {
                if (_categoryNames != value)
                {
                    _categoryNames = value;
                    RaisePropertyChanged(() => CategoryNamesList);
                }
            }
        }

        public string InternalTargets
        {
            get
            {
                return _internalTargets;
            }
            set
            {
                if (_internalTargets != value)
                {
                    _internalTargets = value;
                    RaisePropertyChanged(() => InternalTargets);
                }
            }
        }

        public Datacontext.SelectorFilters SelectedType
        {
            get
            {
                return _selectedType;
            }
            set
            {
                if (_selectedType != value)
                {
                    _selectedType = value;
                    RaisePropertyChanged(() => SelectedType);
                }
            }
        }

        public ObservableCollection<ITeamCommunicator> TeamCommunicator
        {
            get
            {
                return _teamCommunicator;
            }
            set
            {
                if (_teamCommunicator != value)
                {
                    _teamCommunicator = value;
                    RaisePropertyChanged(() => TeamCommunicator);
                }
            }
        }

        public Pointel.Interactions.IPlugins.InteractionType CurrentInteractionType
        {
            get
            {
                return _currentInteractionType;
            }
            set
            {
                if (_currentInteractionType != value)
                {
                    _currentInteractionType = value;
                    RaisePropertyChanged(() => CurrentInteractionType);
                }
            }
        }

        public Pointel.Interactions.IPlugins.OperationType SelectedOperationType
        {
            get
            {
                return _selectedOperationType;
            }
            set
            {
                if (_selectedOperationType != value)
                {
                    _selectedOperationType = value;
                    RaisePropertyChanged(() => SelectedOperationType);
                }
            }
        }

        //public ProtocolManagementService ProtocolManager
        //{
        //    get
        //    {
        //        return _protocolManager;
        //    }
        //    set
        //    {
        //        if (_protocolManager != value)
        //        {
        //            _protocolManager = value;
        //        }
        //    }
        //}

        #endregion

        #region INotifyPropertyChabge

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

        #endregion INotifyPropertyChabge

    }
}
