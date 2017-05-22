using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//Added Namespaces
using StatTickerFive.Helpers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Collections.ObjectModel;
using Pointel.Statistics.Core;
using Pointel.Logger.Core;
using Pointel.Statistics.Core.Utility;
using StatTickerFive.Views;
using System.Windows.Controls;

namespace StatTickerFive.ViewModels
{
    public class ObjectConfigWindowViewModel : NotificationObject
    {
        #region Declaration
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        StatisticsBase objStatTicker = new StatisticsBase();
        public List<string> SwitchNames = new List<string>();
        List<string> ObjectLists = new List<string>();
        List<string> SwitchLists = new List<string>();
        List<string> CheckedObjectsList = new List<string>();
        // Dictionary<string, List<string>> ObjectDictionary = new Dictionary<string, List<string>>();   
        string Window = string.Empty;
        string previousObject = string.Empty;
        public bool isFirstTime = false;
        bool isReadObjectFirstTime = false;
        bool isNewObjectsWindow = false;
        StatisticsSupport objStatSupport = new StatisticsSupport();
        ObjectType obj = new ObjectType();
        List<string> NewObjectList = new List<string>();
        string SelectedStatName = string.Empty;
        public string ObjectId = string.Empty;
        string objectType = string.Empty;

        StatisticsBase objStatBase = new StatisticsBase();
        List<string> RetrivedObjectLists = new List<string>();
        public List<string> SelectedStatList = new List<string>();
        List<string> SelectedObjectList = new List<string>();
        public ObjectType TypeObj = null;
        int NumberofObjectsChecked = 0;
        int MaxObjectsChecked;
        #endregion

        #region Constructor

        public ObjectConfigWindowViewModel()
        {
            try
            {
                //Microsoft.Windows.Controls.MessageBox.Show("Opening config");
                logger.Debug("ObjectConfigWindowViewModel : Constructor - Entry");
                ConfiguredStatistics = new ObservableCollection<StatisticsProperties>();
                TempCollection = new ObservableCollection<StatisticsProperties>();
                ObjectTypes = new ObservableCollection<ObjectType>();
                SelectedObject = new ObservableCollection<ObjectProperties>();
                ApplicationName = Settings.GetInstance().ApplicationName;

                Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> dictTheme = new Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush>();
                dictTheme = objStatSupport.ThemeSelector(Settings.GetInstance().Theme);
                TitleBackground = dictTheme[StatisticsEnum.ThemeColors.TitleBackground];
                BackgroundColor = dictTheme[StatisticsEnum.ThemeColors.BackgroundColor];
                TitleForeground = dictTheme[StatisticsEnum.ThemeColors.TitleForeground];
                BorderBrush = dictTheme[StatisticsEnum.ThemeColors.BorderBrush];
                ShadowEffect = new DropShadowBitmapEffect();
                ShadowEffect.Color = (Color)BorderBrush.Color;
                WinActivated();
                //LoadObjectTypes();
                isFirstTime = true;
                GetConfiguredObjects();
                MaxObjectsChecked = objStatBase.GetMaxObjectCount();
                ObjectSwitchNameGridColumnVisibility = true;
                ObjectNameColumnWidth = 223;
                GridLength length = new GridLength(0);
                WindowWidth = length;

                if (objStatBase.IsDisplayStatistics())
                {
                    StatisticsRowHeight = new GridLength(283);
                }
                else
                {
                    StatisticsRowHeight = new GridLength(0);
                }
                LoadObjectConfiguration();
                IsTopmost = true;
                IsTopmost = false;
                //Window = "ApplicationConfigurations";
                //ConfiguredStatistics = new ObservableCollection<StatisticsProperties>();
                //TempCollection = new ObservableCollection<StatisticsProperties>();
                //ObjectTypes = new ObservableCollection<ObjectType>();
                //SelectedObject = new ObservableCollection<ObjectProperties>();

                //LoadConfigStatistics();
                //ObjectId = Settings.GetInstance().ApplicationName;
                //WinLoad();
                //ObjectTypeChanged(new ObjectType() { Text = StatisticsEnum.ObjectType.ACDQueue.ToString(), });

                //PreviousVisible = Visibility.Visible;
                //Settings.GetInstance().isObjectConfigWindowOpened = true;
                //logger.Debug("ObjectConfigWindowViewModel : Constructor - Exit");
            }
            catch (Exception generalException)
            {
                //Microsoft.Windows.Controls.MessageBox.Show("Opening config excepiton :" + generalException.Message);
                logger.Error("ObjectConfigWindowViewModel : Constructor - Exception caught" + generalException.Message.ToString());
            }


        }


        //public ObjectConfigWindowViewModel(string objType, string statName, string Uid)
        //{
        //    try
        //    {
        //        logger.Debug("ObjectConfigWindowViewModel : Constructor - Entry");
        //        Window = "ObjectConfigurations";
        //        obj.Text = objType;
        //        SelectedStatName = statName;
        //        ObjectId = Uid;

        //        ConfiguredStatistics = new ObservableCollection<StatisticsProperties>();
        //        TempCollection = new ObservableCollection<StatisticsProperties>();
        //        ObjectTypes = new ObservableCollection<ObjectType>();
        //        SelectedObject = new ObservableCollection<ObjectProperties>();
        //        Settings.GetInstance().isObjectConfigWindowOpened = true;
        //        WinLoad();
        //        ReadUserLevelObjects(objType, statName, Uid);
        //        logger.Debug("ObjectConfigWindowViewModel : Constructor - Exit");
        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("ObjectConfigWindowViewModel : Constructor - Exception caught" + generalException.Message.ToString());
        //    }
        //}

        //public ObjectConfigWindowViewModel(List<string> lstStatistics, string Uid)
        //{

        //    try
        //    {
        //        logger.Debug("ObjectConfigWindowViewModel : Constructor - Entry");
        //        Window = "ObjectConfigurations";
        //        ConfiguredStatistics = new ObservableCollection<StatisticsProperties>();
        //        SelectedObject = new ObservableCollection<ObjectProperties>();
        //        ObjectTypes = new ObservableCollection<ObjectType>();
        //        ConfiguredStatistics.Clear();
        //        SelectedObject.Clear();
        //        ObjectId = Uid;

        //        foreach (string statistics in lstStatistics)
        //        {
        //            if (statistics.StartsWith("acd") || statistics.StartsWith("dn") || statistics.StartsWith("vq"))
        //            {
        //                ConfiguredStatistics.Add(new StatisticsProperties() { isGridChecked = false, SectionName = statistics, DisplayName = objStatSupport.GetDescription(statistics) });
        //            }
        //        }

        //        isFirstTime = true;
        //        isNewObjectsWindow = true;
        //        WinLoad();
        //        ObjectTypeChanged(new ObjectType() { Text = StatisticsEnum.ObjectType.ACDQueue.ToString(), });
        //        Settings.GetInstance().isObjectConfigWindowOpened = true;
        //        PreviousVisible = Visibility.Collapsed;

        //        logger.Debug("ObjectConfigWindowViewModel : Constructor - Exit");

        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("ObjectConfigWindowViewModel : Constructor - Exception caught" + generalException.Message.ToString());
        //    }

        //}

        #endregion

        #region Property

        #region ShadowEffect
        private DropShadowBitmapEffect _shadowEffect;
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
        #endregion

        #region BorderBrush
        private SolidColorBrush _borderBrush;
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
        #endregion

        #region ApplicationName
        private String _applicationName = "Admin Configurations";
        public String ApplicationName
        {
            get { return _applicationName; }
            set
            {
                _applicationName = value;
                RaisePropertyChanged(() => ApplicationName);
            }
        }
        #endregion

        #region isGridChecked
        private bool _isGridChecked;
        public bool isGridChecked
        {
            get { return _isGridChecked; }
            set
            {
                _isGridChecked = value;
                RaisePropertyChanged(() => isGridChecked);
            }
        }
        #endregion

        #region IsCheckBoxEnabled
        private bool _isCheckBoxEnabled;
        public bool IsCheckBoxEnabled
        {
            get { return _isCheckBoxEnabled; }
            set
            {
                _isCheckBoxEnabled = value;
                RaisePropertyChanged(() => IsCheckBoxEnabled);
            }
        }
        #endregion

        #region IsComboBoxEnabled
        private bool _isComboBoxEnabled;
        public bool IsComboBoxEnabled
        {
            get { return _isComboBoxEnabled; }
            set
            {
                _isComboBoxEnabled = value;
                RaisePropertyChanged(() => IsComboBoxEnabled);
            }
        }
        #endregion

        #region ObjectTypeName
        private string _objectTypeName;
        public string ObjectTypeName
        {
            get { return _objectTypeName; }
            set
            {
                _objectTypeName = value;
                RaisePropertyChanged(() => ObjectTypeName);
            }
        }
        #endregion

        #region Text
        private string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                RaisePropertyChanged(() => Text);
            }
        }
        #endregion

        #region StatName
        private string _statName;
        public string StatName
        {
            get { return _statName; }
            set
            {
                _statName = value;
                RaisePropertyChanged(() => StatName);
            }
        }
        #endregion

        #region DisplayName
        private string _displayName;
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                _displayName = value;
                RaisePropertyChanged(() => DisplayName);
            }
        }
        #endregion

        #region QueueName
        private string _queueName;
        public string QueueName
        {
            get { return _queueName; }
            set
            {
                _queueName = value;
                RaisePropertyChanged(() => QueueName);
            }
        }
        #endregion

        #region ObjectHeaderName
        private string _objectHeaderName;
        public string ObjectHeaderName
        {
            get { return _objectHeaderName; }
            set
            {
                if (value != null)
                {
                    _objectHeaderName = value;
                    RaisePropertyChanged(() => ObjectHeaderName);
                }

            }
        }
        #endregion

        #region SwitchtHeaderName
        private string _switchtHeaderName;
        public string SwitchtHeaderName
        {
            get { return _switchtHeaderName; }
            set
            {
                if (value != null)
                {
                    _switchtHeaderName = value;
                    RaisePropertyChanged(() => SwitchtHeaderName);
                }

            }
        }
        #endregion

        #region ObjectTypeName

        private string _ObjectType;
        public string ObjectType
        {
            get
            {
                return _ObjectType;
            }
            set
            {
                if(value!=null)
                {
                    _ObjectType = value;
                    RaisePropertyChanged(() => ObjectType);
                }
            }
        }

        #endregion

        #region ObjectSwitchNameGridColumnVisibility
        private bool _objectSwitchNameGridColumnVisibility;
        public bool ObjectSwitchNameGridColumnVisibility
        {
            get
            {
                return _objectSwitchNameGridColumnVisibility;
            }
            set
            {
                _objectSwitchNameGridColumnVisibility = value;
                RaisePropertyChanged(() => ObjectSwitchNameGridColumnVisibility);
            }
        }
        #endregion

        #region ObservableCollection ConfiguredStatistics

        private ObservableCollection<StatisticsProperties> _configuredStatistics;
        public ObservableCollection<StatisticsProperties> ConfiguredStatistics
        {
            get { return _configuredStatistics; }
            set
            {
                _configuredStatistics = value;
                RaisePropertyChanged(() => ConfiguredStatistics);
            }
        }
        #endregion

        #region ObservableCollection TempCollection

        private ObservableCollection<StatisticsProperties> _tempCollection;
        public ObservableCollection<StatisticsProperties> TempCollection
        {
            get { return _tempCollection; }
            set
            {
                _tempCollection = value;
                RaisePropertyChanged(() => TempCollection);
            }
        }
        #endregion

        #region ObservableCollection ObjectTypes

        private ObservableCollection<ObjectType> _objectTypes;
        public ObservableCollection<ObjectType> ObjectTypes
        {
            get { return _objectTypes; }
            set
            {
                _objectTypes = value;
                RaisePropertyChanged(() => ObjectTypes);
            }
        }
        #endregion

        #region ObservableCollection SelectedObject

        private ObservableCollection<ObjectProperties> _selectedObject;
        public ObservableCollection<ObjectProperties> SelectedObject
        {
            get { return _selectedObject; }
            set
            {
                _selectedObject = value;
                RaisePropertyChanged(() => SelectedObject);
            }
        }
        #endregion

        #region SelectedObjectType

        private string _selectedObjectType;
        public string SelectedObjectType
        {
            get
            {
                return _selectedObjectType;
            }
            set
            {
                if (value != null)
                {
                    _selectedObjectType = value;
                    RaisePropertyChanged(() => SelectedObjectType);
                }
            }
        }

        #endregion

        #region ObjectNameColumnWidth
        private double _objectNameColumnWidth;
        public double ObjectNameColumnWidth
        {
            get
            {
                return _objectNameColumnWidth;
            }
            set
            {
                _objectNameColumnWidth = value;
                RaisePropertyChanged(() => ObjectNameColumnWidth);
            }
        }
        #endregion

        #region ObjectIndex
        private int _objectIndex;
        public int ObjectIndex
        {
            get
            {
                return _objectIndex;
            }
            set
            {
                _objectIndex = value;
                RaisePropertyChanged(() => ObjectIndex);
            }
        }
        #endregion

        #region TitleBackground
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
        #endregion

        #region TitleForeground
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
        #endregion

        #region IsAgentGroupChecked
        private bool _isAgentGroupChecked;
        public bool IsAgentGroupChecked
        {
            get
            {
                return _isAgentGroupChecked;
            }
            set
            {
                _isAgentGroupChecked = value;
                RaisePropertyChanged(() => IsAgentGroupChecked);
            }
        }
        #endregion

        #region IsAgentChecked
        private bool _isAgentChecked;
        public bool IsAgentChecked
        {
            get
            {
                return _isAgentChecked;
            }
            set
            {
                _isAgentChecked = value;
                RaisePropertyChanged(() => IsAgentChecked);
            }
        }
        #endregion

        #region WindowWidth
        private GridLength _windowWidth;
        public GridLength WindowWidth
        {
            get
            {
                return _windowWidth;
            }
            set
            {
                _windowWidth = value;
                RaisePropertyChanged(() => WindowWidth);
            }
        }
        #endregion

        private GridLength _StatisticsRowHeight;
        public GridLength StatisticsRowHeight
        {
            get
            {
                return _StatisticsRowHeight;
            }
            set
            {
                _StatisticsRowHeight = value;
                RaisePropertyChanged(() => StatisticsRowHeight);
            }
        }

        #region BackgroundColor
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
        #endregion

        #region Previous Button Visibility

        private Visibility _PreviousVisible;
        public Visibility PreviousVisible
        {
            get
            {
                return _PreviousVisible;
            }
            set
            {
                _PreviousVisible = value;
                RaisePropertyChanged(() => PreviousVisible);
            }
        }

        private bool _IsTopmost;
        public bool IsTopmost
        {
            get
            {
                return _IsTopmost;
            }
            set
            {
                _IsTopmost = value;
                RaisePropertyChanged(() => IsTopmost);
            }
        }


        #endregion

        private string _SelectedCount;
        public string SelectedCount
        {
            get
            {
                return _SelectedCount;
            }
            set
            {
                _SelectedCount = value;
                RaisePropertyChanged(() => SelectedCount);
            }
        }

        private bool _IsDeselectAllChecked;
        public bool IsDeselectAllChecked
        {
            get
            {
                return _IsDeselectAllChecked;
            }
            set
            {
                _IsDeselectAllChecked = value;
                RaisePropertyChanged(() => IsDeselectAllChecked);
            }
        }

        #endregion

        #region Commands

        public ICommand ActivatedCommand { get { return new DelegateCommand(WinActivated); } }
        public ICommand DeactivateCommand { get { return new DelegateCommand(WinDeActivated); } }
        public ICommand DragCmd { get { return new DelegateCommand(DragObjectConfigWindow); } }
        public ICommand ConfigCancelCmd { get { return new DelegateCommand(ConfigurationCancel); } }
        public ICommand ObjectTypeChangedCommand { get { return new DelegateCommand(ObjectTypeChanged); } }
        public ICommand ConfiguredStatChecked { get { return new DelegateCommand(ConfiguredStatisticsChecked); } }
        public ICommand ConfiguredStatUnChecked { get { return new DelegateCommand(ConfiguredStatisticsUnChecked); } }
        public ICommand SaveObjectsCommand { get { return new DelegateCommand(SaveObjects); } }
        public ICommand AgentGroupConfigurationCommand { get { return new DelegateCommand(AgentGroupToggleButtonClicked); } }
        public ICommand AgentConfigurationCommand { get { return new DelegateCommand(AgentToggleButtonClicked); } }
        public ICommand SaveCmd { get { return new DelegateCommand(SaveNewObjects); } }
        public ICommand ObjectCheckedCommand { get { return new DelegateCommand(NewObjectChecked); } }
        public ICommand CheckedObjectsCommand { get { return new DelegateCommand(ObjectsChecked); } }
        public ICommand DeselectUncheck { get { return new DelegateCommand(DeselectAll); } }


        #endregion

        #region methods

        #region LoadObjectConfiguration
        public void LoadObjectConfiguration()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : LoadObjectConfiguration Method - Entry");
                Window = "ApplicationConfigurations";
                LoadConfigStatistics();
                ObjectId = Settings.GetInstance().ApplicationName;
                SwitchNames.Clear();
                SwitchNames = objStatTicker.GetSwitches();
                GetConfiguredObjects();
                ObjectTypeChanged();

                PreviousVisible = Visibility.Visible;
                logger.Debug("ObjectConfigWindowViewModel : LoadObjectConfiguration Method  - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : LoadObjectConfiguration Method  - Exception caught" + generalException.Message.ToString());
            }
        }
        #endregion

        #region LoadObjectConfiguration
        public void LoadObjectConfiguration(List<string> lstStatistics, string Uid)
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : LoadObjectConfiguration(List<string> lstStatistics, string Uid) Method  - Entry");
                Window = "Agent/AgentGroupConfigurations";
                ConfiguredStatistics.Clear();
                SelectedObject.Clear();
                ObjectId = Uid;

                foreach (string statistics in lstStatistics)
                {
                    if (statistics.StartsWith("acd") || statistics.StartsWith("dn") || statistics.StartsWith("vq"))
                    {
                        ConfiguredStatistics.Add(new StatisticsProperties() { isGridChecked = false, SectionName = statistics, DisplayName = objStatSupport.GetDescription(statistics) });
                    }
                }

                //isFirstTime = true;
                isNewObjectsWindow = true;
                SwitchNames.Clear();
                SwitchNames = objStatTicker.GetSwitches();
                GetConfiguredObjects();
                //if (TypeObj == null)
                //                    ObjectTypeChanged(new ObjectType() { Text = StatisticsEnum.ObjectType.ACDQueue.ToString(), });
                //else
                //ObjectTypeChanged(TypeObj);
                PreviousVisible = Visibility.Collapsed;

                logger.Debug("ObjectConfigWindowViewModel : LoadObjectConfiguration(List<string> lstStatistics, string Uid) Method  - Exit");

            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : LoadObjectConfiguration Method(List<string> lstStatistics, string Uid)  - Exception caught" + generalException.Message.ToString());
            }
        }
        #endregion

        #region LoadObjectConfiguration
        public void LoadObjectConfiguration(string objType, string statName, string Uid)
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : LoadObjectConfiguration(string objType, string statName, string Uid) Method  - Entry");
                Window = "Agent/AgentGroupConfigurations";
                obj.Text = objType;
                SelectedStatName = statName;
                ObjectId = Uid;
                SwitchNames.Clear();
                SwitchNames = objStatTicker.GetSwitches();
                GetConfiguredObjects();
                ReadUserLevelObjects(objType, statName, Uid);
                logger.Debug("ObjectConfigWindowViewModel : LoadObjectConfiguration(string objType, string statName, string Uid) Method  - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : LoadObjectConfiguration(string objType, string statName, string Uid) Method  - Exception caught" + generalException.Message.ToString());
            }
        }
        #endregion

        #region WinActivated
        /// <summary>
        /// Wins the activated.
        /// </summary>
        private void WinActivated()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : WinActivated() Method - Entry");

                ShadowEffect = new DropShadowBitmapEffect();
                ShadowEffect.ShadowDepth = 0;
                ShadowEffect.Opacity = 0.5;
                ShadowEffect.Softness = 0.5;

                IsTopmost = true;
                IsTopmost = false;
                logger.Debug("ObjectConfigWindowViewModel : WinActivated() Method - Exit");
            }
            catch (Exception generalException)
            {
                logger.Debug("ObjectConfigWindowViewModel : WinActivated() Method - Exception caught" + generalException.Message.ToString());
            }
            finally
            {
                GC.Collect();
            }
        }
        #endregion

        #region WinDeActivated
        /// <summary>
        /// Wins the de activated.
        /// </summary>
        private void WinDeActivated()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : WinDeActivated() Method - Entry");
                ShadowEffect.Opacity = 0;
                logger.Debug("ObjectConfigWindowViewModel : WinDeActivated() Method - Exit");

            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : WinDeActivated() Method - Exception caught" + generalException.Message.ToString());
            }

        }
        #endregion

        #region DragMove
        /// <summary>
        /// Drags the move.
        /// </summary>
        private void DragObjectConfigWindow(object obj)
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : DragObjectConfigWindow() Method - Entry");
                foreach (Window currentwindow in System.Windows.Application.Current.Windows)
                {
                    if (currentwindow.Title == obj.ToString())
                        currentwindow.DragMove();
                }
                logger.Debug("ObjectConfigWindowViewModel : DragObjectConfigWindow() Method - Exit");

            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : DragObjectConfigWindow() Method - Exception caught" + generalException.Message.ToString());
            }

        }
        #endregion

        #region WinLoad
        /// <summary>
        /// Wins the load.
        /// </summary>
        public void WinLoad()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel :WinLoad() Method - Entry");



                //ApplicationName = Settings.GetInstance().ApplicationName;

                //Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> dictTheme = new Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush>();
                //dictTheme = objStatSupport.ThemeSelector(Settings.GetInstance().Theme);
                //TitleBackground = dictTheme[StatisticsEnum.ThemeColors.TitleBackground];
                //BackgroundColor = dictTheme[StatisticsEnum.ThemeColors.BackgroundColor];
                //TitleForeground = dictTheme[StatisticsEnum.ThemeColors.TitleForeground];
                //BorderBrush = dictTheme[StatisticsEnum.ThemeColors.BorderBrush];
                //ShadowEffect = new DropShadowBitmapEffect();
                //ShadowEffect.Color = (Color)BorderBrush.Color;
                //WinActivated();
                //if (!Settings.GetInstance().isObjectConfigWindowOpened)
                //    LoadObjectTypes();
                //SwitchNames.Clear();
                //SwitchNames = objStatTicker.GetSwitches();

                ////isFirstTime = true;
                //GetConfiguredObjects();

                //ObjectSwitchNameGridColumnVisibility = true;
                //ObjectNameColumnWidth = 271;
                //GridLength length = new GridLength(0);
                //WindowWidth = length;

            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel :WinLoad() Method - Exception caught:" + generalException);
            }
            logger.Debug("ObjectConfigWindowViewModel :WinLoad() Method - Exit");
        }
        #endregion

        #region ConfigurationCancel

        private void ConfigurationCancel(object obj)
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : ConfigurationCancel() Method - Entry");
                //if (Window == "ApplicationConfigurations")
                //{
                //    //bool newObjectFlag = false;

                //    //if (Settings.GetInstance().ObjectUserLevelConfigVM == null)
                //    //{
                //    //    Views.StatisticsConfigWindow userlevelView = new Views.StatisticsConfigWindow();
                //    //    Settings.GetInstance().ObjectUserLevelConfigVM = new UserLevelStatisticsConfigViewModel();
                //    //    userlevelView.DataContext = Settings.GetInstance().ObjectUserLevelConfigVM;
                //    //    userlevelView.Show();
                //    //}
                //    //else
                //    //{
                //    //    foreach (Window currentwindow in System.Windows.Application.Current.Windows)
                //    //    {
                //    //        if (currentwindow.Title == "UserLevelConfigurations")
                //    //            currentwindow.Show();
                //    //    }
                //    //}


                //    //if ((Settings.GetInstance().ObjectUserLevelConfigVM.TabValues.Count > 0) && (newObjectFlag))
                //    //{
                //    //    for (int item = 0; item < Settings.GetInstance().ObjectUserLevelConfigVM.TabValues.Count; item++)
                //    //    {
                //    //        TabItem TBItem = Settings.GetInstance().ObjectUserLevelConfigVM.TabValues[item];
                //    //        if (TBItem.IsSelected)
                //    //        {
                //    //            Settings.GetInstance().ObjectUserLevelConfigVM.ObjectIndex = item;
                //    //            Settings.GetInstance().ObjectUserLevelConfigVM.temptab_MouseLeftButtonUp(TBItem, null);
                //    //            break;
                //    //        }

                //    //    }

                //    //}

                //    Settings.GetInstance().PreviousWindow = "ObjectConfigurations";
                //    Settings.GetInstance().isObjectConfigWindowOpened = false;
                //    Settings.GetInstance().isNewObjectConfigWindowOpened = false;
                foreach (Window currentwindow in System.Windows.Application.Current.Windows)
                {
                    if (currentwindow.Title == obj.ToString())
                        currentwindow.Close();
                }
                //}
                //else if (Window == "Agent/AgentGroupConfigurations")
                //{

                //    foreach (Window currentwindow in System.Windows.Application.Current.Windows)
                //    {
                //        if (currentwindow.Name == "EditObject")
                //            currentwindow.Close();
                //        if (currentwindow.Title == "NewObjectConfig")
                //            currentwindow.Close();
                //    }
                //    Settings.GetInstance().PreviousWindow = "ObjectConfigurations";
                //    Settings.GetInstance().isEditObjectConfigWindowOpened = false;
                //    Settings.GetInstance().isNewObjectConfigWindowOpened = false;
                //    Window = "ApplicationConfigurations";
                //}
                //Settings.GetInstance().isUserLevelConfigWindowOpened = true;
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : ConfigurationCancel() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("ObjectConfigWindowViewModel : ConfigurationCancel() Method - Exit");

        }

        #endregion

        #region ApplicationStatisticsConfig
        private void ApplicationStatisticsConfig()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : ApplicationStatisticsConfig() Method - entry");
                LoadConfigStatistics();

            }
            catch (Exception GeneralException)
            {
                logger.Error("ObjectConfigWindowViewModel : ApplicationStatisticsConfig() Method - Exception caught" + GeneralException.InnerException.ToString());
            }
            logger.Debug("ObjectConfigWindowViewModel : ApplicationStatisticsConfig() Method - exit");

        }
        #endregion

        #region LoadConfigStatistics

        public void LoadConfigStatistics()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : LoadConfigStatistics() Method - entry");
                Dictionary<string, Dictionary<string, string>> DictTempExistingApplicationStats = null;

                DictTempExistingApplicationStats = objStatTicker.GetApplicationAnnex(Settings.GetInstance().ApplicationName);
                DictTempExistingApplicationStats.OrderBy(a => a.Key);
                Settings.GetInstance().DictExistingApplicationStats = DictTempExistingApplicationStats;
                ConfiguredStatistics.Clear();
                foreach (string SectionName in Settings.GetInstance().DictExistingApplicationStats.Keys)
                {
                    Dictionary<string, string> statDetails = new Dictionary<string, string>();

                    statDetails = Settings.GetInstance().DictExistingApplicationStats[SectionName];
                    string DisplayName = string.Empty;

                    foreach (string key in statDetails.Keys)
                    {
                        if (string.Compare(key, StatisticsEnum.StatProperties.DisplayName.ToString(), true) == 0)
                        {
                            DisplayName = statDetails[key].ToString();
                        }
                    }

                    if (!SectionName.StartsWith("agent") && !SectionName.StartsWith("group"))
                    {
                        ConfiguredStatistics.Add(new StatisticsProperties()
                        {
                            isGridChecked = false,

                            SectionName = SectionName,

                            DisplayName = DisplayName,

                            EditName = "Edit",

                            IsCheckBoxEnabled = true,

                        });
                    }

                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("ObjectConfigWindowViewModel : LoadConfigStatistics() Method - Exception caught" + GeneralException.InnerException.ToString());
            }
            logger.Debug("ObjectConfigWindowViewModel : LoadConfigStatistics() Method - exit");
        }

        #endregion

        #region LoadObjectTypes
        private void LoadObjectTypes()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : LoadObjectTypes() Method - Entry");
                ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.ACDQueue.ToString(), IsComboBoxEnabled = false });
                ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.DNGroup.ToString(), IsComboBoxEnabled = false });
                ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.VirtualQueue.ToString(), IsComboBoxEnabled = false });

            }
            catch (Exception GeneralException)
            {
                logger.Error("ObjectConfigWindowViewModel : LoadObjectTypes() Method - Exception caught " + GeneralException.Message);
            }
            logger.Debug("ObjectConfigWindowViewModel : LoadObjectTypes() Method - Exit");
        }
        #endregion

        #region ObjectTypeChanged

        /// <summary>
        /// Objects the type changed.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void ObjectTypeChanged()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : ObjectTypeChanged() Method - Entry");

                //TypeObj = obj as ObjectType;
                List<string> lstObjects = new List<string>();
                lstObjects = objStatBase.GetObjectTypes();

                foreach (string objType in lstObjects)
                {

                    objectType = objType;
                    List<string> statList = new List<string>();
                    foreach (StatisticsProperties StatObj in ConfiguredStatistics)
                    {
                        if (StatObj.isGridChecked)
                        {
                            statList.Add(StatObj.SectionName);
                        }

                        if ((StatObj.SectionName.StartsWith("acd")) && (string.Compare(objectType, StatisticsEnum.ObjectType.ACDQueue.ToString(), false) == 0))
                        {
                            StatObj.IsCheckBoxEnabled = true;
                        }
                        else if ((StatObj.SectionName.StartsWith("dn")) && (string.Compare(objectType, StatisticsEnum.ObjectType.DNGroup.ToString(), false) == 0))
                        {
                            StatObj.IsCheckBoxEnabled = true;
                        }
                        else if ((StatObj.SectionName.StartsWith("vq")) && (string.Compare(objectType, StatisticsEnum.ObjectType.VirtualQueue.ToString(), false) == 0))
                        {
                            StatObj.IsCheckBoxEnabled = true;
                        }
                        else
                        {
                            StatObj.IsCheckBoxEnabled = false;
                            StatObj.isGridChecked = false;
                        }
                    }
                    if (string.Compare(objectType, StatisticsEnum.ObjectType.ACDQueue.ToString(), false) == 0)
                    {
                        //ObjectHeaderName = StatisticsEnum.ObjectType.ACDQueue.ToString();
                        ObjectHeaderName = "Object(s)";
                        ObjectType = "Object Type(s)";
                        SwitchtHeaderName = "SwitchName(s)";
                    }
                    else if (string.Compare(objectType, StatisticsEnum.ObjectType.DNGroup.ToString(), false) == 0)
                    {
                        ObjectHeaderName = "Object(s)";
                        ObjectType = "Object Type(s)";
                    }
                    else if (string.Compare(objectType, StatisticsEnum.ObjectType.VirtualQueue.ToString(), false) == 0)
                    {
                        ObjectHeaderName = "Object(s)";
                        ObjectType = "Object Type(s)";
                        SwitchtHeaderName = "SwitchName(s)";
                    }

                    RetrivedObjectLists = GetObjectList(objectType);

                    string Dilimitor = "_@";
                    //SelectedObject.Clear();
                    for (int count = 0; count < RetrivedObjectLists.Count; count++)
                    {
                        if (SwitchLists.Count > 0)
                        {
                            SelectedObject.Add(new ObjectProperties()
                            {

                                ObjectName = RetrivedObjectLists[count].Split(new[] { Dilimitor }, StringSplitOptions.None)[0].ToString(),
                                ObjectSwitchName = SwitchLists[count],
                                IsObjectChecked = false,
                                TypeObject = objType,
                            });
                        }
                        else
                        {
                            SelectedObject.Add(new ObjectProperties()
                            {
                                ObjectName = RetrivedObjectLists[count].Split(new[] { Dilimitor }, StringSplitOptions.None)[0].ToString(),
                                IsObjectChecked = false,
                                TypeObject = objType,
                            });
                        }


                    }

                    ReadUserLevelObjects(objectType);

                    //}
                    //SelectedStatList.Clear();
                    //SelectedObjectList.Clear();
                }

                if (NumberofObjectsChecked == 0)
                    IsDeselectAllChecked = false;
                else
                    IsDeselectAllChecked = true;

                SelectedCount = NumberofObjectsChecked.ToString() + " Object(s) Selected";
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : ObjectTypeChanged() Method - Exception caught" + generalException);
            }
            logger.Debug("ObjectConfigWindowViewModel : ObjectTypeChanged() Method - Exit");
        }

        #endregion

        #region ReadUserLevelObjects

        private void ReadUserLevelObjects(string objType, string statName, string Uid)
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : ReadUserLevelObjects Method - Entry");

                Dictionary<string, Dictionary<string, List<string>>> ObjectsDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                ObjectsDictionary.Add("Objects", new Dictionary<string, List<string>>());

                if (Settings.GetInstance().DictApplicationObjects.ContainsKey(ObjectId))
                {
                    foreach (string key in Settings.GetInstance().DictApplicationObjects[ObjectId].Keys)
                    {
                        List<string> lstAppObject = new List<string>(Settings.GetInstance().DictApplicationObjects[ObjectId][key]);
                        ObjectsDictionary["Objects"].Add(key, lstAppObject);
                    }
                }
                else if (Settings.GetInstance().DictAgentObjects.ContainsKey(ObjectId))
                {
                    foreach (string key in Settings.GetInstance().DictAgentObjects[ObjectId].Keys)
                    {
                        List<string> lstAgentObjects = new List<string>(Settings.GetInstance().DictAgentObjects[ObjectId][key]);
                        ObjectsDictionary["Objects"].Add(key, lstAgentObjects);
                    }
                }
                else if (Settings.GetInstance().DictAgentGroupObjects.ContainsKey(ObjectId))
                {
                    foreach (string key in Settings.GetInstance().DictAgentGroupObjects[ObjectId].Keys)
                    {
                        List<string> lstAgentGroupObjects = new List<string>(Settings.GetInstance().DictAgentGroupObjects[ObjectId][key]);
                        ObjectsDictionary["Objects"].Add(key, lstAgentGroupObjects);
                    }
                }

                List<string> RetrivedObjectLists = null;
                List<string> ExistObjectNameList = null;

                RetrivedObjectLists = GetObjectList(objType);
                SelectedObject.Clear();
                for (int count = 0; count < RetrivedObjectLists.Count; count++)
                {
                    if (SwitchLists.Count > 0)
                    {

                        if ((string.Compare(objType, StatisticsEnum.ObjectType.ACDQueue.ToString(), true) == 0))
                        {
                            ExistObjectNameList = ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.ACDQueue.ToString()];
                            if (ExistObjectNameList.Contains(RetrivedObjectLists[count] + "_@" + SwitchLists[count] + "_@" + statName))
                            {
                                SelectedObject.Add(new ObjectProperties()
                                {
                                    ObjectName = RetrivedObjectLists[count],
                                    ObjectSwitchName = SwitchLists[count],
                                    TypeObject = StatisticsEnum.ObjectType.ACDQueue.ToString(),
                                    IsObjectChecked = true,
                                });
                                ++NumberofObjectsChecked;
                            }
                            else
                            {
                                SelectedObject.Add(new ObjectProperties()
                                {
                                    ObjectName = RetrivedObjectLists[count],
                                    ObjectSwitchName = SwitchLists[count],
                                    TypeObject = StatisticsEnum.ObjectType.ACDQueue.ToString(),
                                    IsObjectChecked = false,
                                });
                            }
                        }
                        if ((string.Compare(objType, StatisticsEnum.ObjectType.VirtualQueue.ToString(), true) == 0))
                        {
                            ExistObjectNameList = ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.VirtualQueue.ToString()];
                            if (ExistObjectNameList.Contains(RetrivedObjectLists[count] + "_@" + SwitchLists[count] + "_@" + statName))
                            {
                                SelectedObject.Add(new ObjectProperties()
                                {
                                    ObjectName = RetrivedObjectLists[count],
                                    ObjectSwitchName = SwitchLists[count],
                                    TypeObject = StatisticsEnum.ObjectType.VirtualQueue.ToString(),
                                    IsObjectChecked = true,
                                });
                                ++NumberofObjectsChecked;
                            }
                            else
                            {
                                SelectedObject.Add(new ObjectProperties()
                                {
                                    ObjectName = RetrivedObjectLists[count],
                                    ObjectSwitchName = SwitchLists[count],
                                    TypeObject = StatisticsEnum.ObjectType.VirtualQueue.ToString(),
                                    IsObjectChecked = false,
                                });
                            }
                        }


                    }
                    else
                    {
                        ExistObjectNameList = ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.DNGroup.ToString()];
                        if (ExistObjectNameList.Contains(RetrivedObjectLists[count] + "_@" + statName))
                        {
                            SelectedObject.Add(new ObjectProperties()
                            {
                                ObjectName = RetrivedObjectLists[count],
                                TypeObject = StatisticsEnum.ObjectType.DNGroup.ToString(),
                                IsObjectChecked = true,
                            });
                            ++NumberofObjectsChecked;
                        }
                        else
                        {
                            SelectedObject.Add(new ObjectProperties()
                            {
                                ObjectName = RetrivedObjectLists[count],
                                TypeObject = StatisticsEnum.ObjectType.DNGroup.ToString(),
                                IsObjectChecked = false,
                            });
                        }

                    }

                }


            }
            catch (Exception GeneralException)
            {
                logger.Error("ObjectConfigWindowViewModel : ReadUserLevelObjects Method - Exception caught" + GeneralException.Message.ToString());
            }
            finally
            {

            }
            logger.Debug("ObjectConfigWindowViewModel : ReadUserLevelObjects Method - Exit");
        }

        #endregion

        #region ReadUserLevelObjects

        //public void ReadUserLevelObjects(string objType, List<string> checkedStatNameList)
        public void ReadUserLevelObjects(string objType)
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : ReadUserLevelObjects Method - Entry");

                string Dilimitor = "_@";

                Settings.GetInstance().DictAgentStatisitics = objStatBase.GetAgentValues();
                Settings.GetInstance().DictAgentObjects = objStatBase.GetAgentObjects();

                Dictionary<string, Dictionary<string, List<string>>> ObjectsDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                ObjectsDictionary.Add("Objects", new Dictionary<string, List<string>>());

                if (Settings.GetInstance().DictApplicationObjects.ContainsKey(ObjectId))
                {
                    foreach (string key in Settings.GetInstance().DictApplicationObjects[ObjectId].Keys)
                    {
                        List<string> lstAppObjects = new List<string>(Settings.GetInstance().DictApplicationObjects[ObjectId][key]);
                        ObjectsDictionary["Objects"].Add(key, lstAppObjects);
                    }
                }

                //if (Settings.GetInstance().DictAgentGroupObjects.ContainsKey(ObjectId))
                //{
                //    foreach (string key in Settings.GetInstance().DictAgentGroupObjects[ObjectId].Keys)
                //    {
                //        List<string> lstAgentGroupObjects = new List<string>(Settings.GetInstance().DictAgentGroupObjects[ObjectId][key]);
                //        ObjectsDictionary["Objects"].Add(key, lstAgentGroupObjects);
                //    }
                //}

                string agentId = objStatBase.GetAgentId();
                if (Settings.GetInstance().DictAgentObjects.ContainsKey(agentId))
                {
                    foreach (string key in Settings.GetInstance().DictAgentObjects[agentId].Keys)
                    {
                        List<string> lstAgentObjects = new List<string>(Settings.GetInstance().DictAgentObjects[agentId][key]);
                        ObjectsDictionary["Objects"].Add(key, lstAgentObjects);
                    }
                }

                List<string> ExistObjectNameList = null;

                ////foreach (ObjectProperties objProperty in SelectedObject)
                ////{
                ////    objProperty.IsObjectChecked = false;
                ////}

                for (int count = 0; count < RetrivedObjectLists.Count; count++)
                {
                    if (SwitchLists.Count > 0)
                    {
                        if ((string.Compare(objType, StatisticsEnum.ObjectType.ACDQueue.ToString(), true) == 0) || (string.Compare(objType, StatisticsEnum.ObjectType.VirtualQueue.ToString(), true) == 0))
                        {
                            if ((string.Compare(objType, StatisticsEnum.ObjectType.ACDQueue.ToString(), true) == 0))
                            {
                                ExistObjectNameList = ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.ACDQueue.ToString()];
                            }
                            if ((string.Compare(objType, StatisticsEnum.ObjectType.VirtualQueue.ToString(), true) == 0))
                            {
                                ExistObjectNameList = ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.VirtualQueue.ToString()];
                            }

                            //foreach (string str in checkedStatNameList)
                            {
                                foreach (string ExistObjectName in ExistObjectNameList)
                                {

                                    if ((string.Compare(ExistObjectName, RetrivedObjectLists[count], true) == 0))
                                    {
                                        foreach (ObjectProperties objProperty in SelectedObject)
                                        {
                                            if ((string.Compare(objProperty.ObjectName, RetrivedObjectLists[count].Split(new[] { Dilimitor }, StringSplitOptions.None)[0].ToString(), true) == 0) && (string.Compare(objProperty.ObjectSwitchName, SwitchLists[count], true) == 0))
                                            {
                                                objProperty.IsObjectChecked = true;
                                                ++NumberofObjectsChecked;
                                                break;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (string.Compare(objType, StatisticsEnum.ObjectType.DNGroup.ToString(), true) == 0)
                        {
                            ExistObjectNameList = ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.DNGroup.ToString()];
                            //foreach (string str in checkedStatNameList)
                            {
                                foreach (string ExistObjectName in ExistObjectNameList)
                                {
                                    if ((string.Compare(ExistObjectName, RetrivedObjectLists[count], true) == 0))
                                    {
                                        foreach (ObjectProperties objProperty in SelectedObject)
                                        {
                                            if (string.Compare(objProperty.ObjectName, RetrivedObjectLists[count], true) == 0)
                                            {
                                                objProperty.IsObjectChecked = true;
                                                ++NumberofObjectsChecked;
                                                break;
                                            }
                                        }
                                    }
                                }


                            }
                        }
                    }


                }


            }
            catch (Exception GeneralException)
            {
                logger.Error("ObjectConfigWindowViewModel : ReadUserLevelObjects Method - Exception caught" + GeneralException.Message.ToString());
            }
            finally
            {

            }
            logger.Debug("ObjectConfigWindowViewModel : ReadUserLevelObjects Method - Exit");
        }

        #endregion

        #region GetObjectList

        /// <summary>
        /// Gets the object list.
        /// </summary>
        /// <param name="selectedObjectType">Type of the selected object.</param>
        /// <returns></returns>
        private List<string> GetObjectList(string selectedObjectType)
        {
            try
            {
                ObjectLists.Clear();
                SwitchLists.Clear();
                logger.Debug("ObjectConfigWindowViewModel : GetQueueList() Method - Entry");

                //Commented for GUD PHS, by Elango.
                //if (string.Compare(selectedObjectType, StatisticsEnum.ObjectType.ACDQueue.ToString(), true) == 0)
                //{
                //    List<string> TempList = new List<string>();
                //    string[] TempStringArray = null;
                //    TempList = objStatTicker.ReadAllACDQueues(SwitchNames);
                //    TempList.Sort();
                //    string Dilimitor = "_@";
                //    foreach (string obj in TempList)
                //    {
                //        TempStringArray = obj.Split(new[] { Dilimitor }, StringSplitOptions.None);
                //        //ObjectLists.Add(TempStringArray[0]);
                //        SwitchLists.Add(TempStringArray[1]);
                //    }
                //    ObjectLists = TempList;

                //    ObjectSwitchNameGridColumnVisibility = true;
                //    ObjectNameColumnWidth = 223;
                //}

                if (string.Compare(selectedObjectType, StatisticsEnum.ObjectType.DNGroup.ToString(), true) == 0)
                {
                    ObjectLists = objStatTicker.ReadAllDNGroups();
                    ObjectLists.Sort();
                    ObjectSwitchNameGridColumnVisibility = false;
                    ObjectNameColumnWidth = 452;
                }
                else if (string.Compare(selectedObjectType, StatisticsEnum.ObjectType.VirtualQueue.ToString(), true) == 0)
                {
                    List<string> TempList = new List<string>();
                    string[] TempStringArray = null;
                    SwitchLists.Clear();
                    TempList = objStatTicker.ReadAllVirtualQueues(SwitchNames);
                    TempList.Sort();
                    string Dilimitor = "_@";
                    foreach (string obj in TempList)
                    {
                        TempStringArray = obj.Split(new[] { Dilimitor }, StringSplitOptions.None);
                        //ObjectLists.Add(TempStringArray[0]);
                        SwitchLists.Add(TempStringArray[1]);
                    }
                    ObjectLists = TempList;
                    ObjectSwitchNameGridColumnVisibility = true;
                    ObjectNameColumnWidth = 223;
                }

            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : GetQueueList() Method - Exception caught" + generalException);
            }
            return ObjectLists;
            logger.Debug("ObjectConfigWindowViewModel : GetQueueList() Method - Exit");
        }
        #endregion

        #region ConfiguredStatisticsChecked

        public void ConfiguredStatisticsChecked(object obj)
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : ConfiguredStatisticsChecked() Method - Entry");

                StatisticsProperties statPropertiesObj = obj as StatisticsProperties;
                string[] temp;
                temp = statPropertiesObj.SectionName.ToString().Trim().Split('-');

                #region Changing Grid Values

                foreach (StatisticsProperties statProperties in ConfiguredStatistics)
                {
                    if (!statProperties.SectionName.Contains(temp[0].ToString()))
                    {
                        statProperties.IsCheckBoxEnabled = false;
                    }
                }

                #endregion

                #region Old

                //ConfiguredStatistics.Clear();

                //foreach (string SectionName in Settings.GetInstance().DictExistingApplicationStats.Keys)
                //{
                //    Dictionary<string, string> statDetails = new Dictionary<string, string>();

                //    statDetails = Settings.GetInstance().DictExistingApplicationStats[SectionName];
                //    string DisplayName = string.Empty;
                //    bool IsCheckBoxEnabled = true;
                //    bool isGridChecked = false;
                //    string[] SplitStats;
                //    SplitStats = SectionName.Trim().Split('-');
                //    if (string.Compare(SplitStats[0], temp[0], true) == 0)
                //    {
                //        IsCheckBoxEnabled = true;

                //    }
                //    else
                //    {
                //        IsCheckBoxEnabled = false;

                //    }
                //    if (string.Compare(statPropertiesObj.SectionName.ToString(), SectionName, true) == 0)
                //    {
                //        isGridChecked = true;
                //    }
                //    foreach(var item in TempCollection)
                //    {
                //        if (string.Compare(item.SectionName, SectionName, true) == 0)
                //        {
                //            isGridChecked = true;
                //        }
                //    }
                //    //else
                //    //{
                //    //    isGridChecked = false;
                //    //}
                //    foreach (string key in statDetails.Keys)
                //    {
                //        if (string.Compare(key, StatisticsEnum.StatProperties.DisplayName.ToString(), true) == 0)
                //        {
                //            DisplayName = statDetails[key].ToString();
                //        }


                //    }

                //    ConfiguredStatistics.Add(new StatisticsProperties()
                //    {
                //        isGridChecked = isGridChecked,

                //        SectionName = SectionName,

                //        DisplayName = DisplayName,

                //        EditName = "Edit",

                //        IsCheckBoxEnabled = IsCheckBoxEnabled,

                //    });


                //}
                //TempCollection.Clear();
                //foreach (var item in ConfiguredStatistics)
                //{
                //    if (item.isGridChecked == true)
                //    {
                //        TempCollection.Add(item);
                //    }

                //}

                #endregion

                #region Changing Combobox value property

                if (temp[0] == "agent")
                {
                    foreach (ObjectType Objects in ObjectTypes)
                    {
                        if (string.Compare(Objects.Text, StatisticsEnum.ObjectType.Agent.ToString(), true) == 0)
                        {
                            Objects.IsComboBoxEnabled = true;
                        }
                        else
                        {
                            Objects.IsComboBoxEnabled = false;
                        }
                    }

                    #region Old

                    //ObjectTypes.Clear();
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.ACDQueue.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.Agent.ToString(), IsComboBoxEnabled = true });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.AgentGroup.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.DNGroup.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.VirtualQueue.ToString(), IsComboBoxEnabled = false });

                    #endregion
                }
                else if (temp[0] == "group")
                {
                    foreach (ObjectType Objects in ObjectTypes)
                    {
                        if (string.Compare(Objects.Text, StatisticsEnum.ObjectType.AgentGroup.ToString(), true) == 0)
                        {
                            Objects.IsComboBoxEnabled = true;
                        }
                        else
                        {
                            Objects.IsComboBoxEnabled = false;
                        }
                    }

                    #region Old

                    //ObjectTypes.Clear();
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.ACDQueue.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.Agent.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.AgentGroup.ToString(), IsComboBoxEnabled = true });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.DNGroup.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.VirtualQueue.ToString(), IsComboBoxEnabled = false });

                    #endregion
                }
                else if (temp[0] == "acd")
                {
                    foreach (ObjectType Objects in ObjectTypes)
                    {
                        if (string.Compare(Objects.Text, StatisticsEnum.ObjectType.ACDQueue.ToString(), true) == 0)
                        {
                            Objects.IsComboBoxEnabled = true;
                        }
                        else
                        {
                            Objects.IsComboBoxEnabled = false;
                        }
                    }

                    #region Old

                    //ObjectTypes.Clear();
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.ACDQueue.ToString(), IsComboBoxEnabled = true });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.Agent.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.AgentGroup.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.DNGroup.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.VirtualQueue.ToString(), IsComboBoxEnabled = false });

                    #endregion
                }
                else if (temp[0] == "dn")
                {
                    foreach (ObjectType Objects in ObjectTypes)
                    {
                        if (string.Compare(Objects.Text, StatisticsEnum.ObjectType.DNGroup.ToString(), true) == 0)
                        {
                            Objects.IsComboBoxEnabled = true;
                        }
                        else
                        {
                            Objects.IsComboBoxEnabled = false;
                        }
                    }

                    #region Old

                    //ObjectTypes.Clear();
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.ACDQueue.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.AgentGroup.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.DNGroup.ToString(), IsComboBoxEnabled = true });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.VirtualQueue.ToString(), IsComboBoxEnabled = false });

                    #endregion
                }
                else if (temp[0] == "vq")
                {
                    foreach (ObjectType Objects in ObjectTypes)
                    {
                        if (string.Compare(Objects.Text, StatisticsEnum.ObjectType.VirtualQueue.ToString(), true) == 0)
                        {
                            Objects.IsComboBoxEnabled = true;
                        }
                        else
                        {
                            Objects.IsComboBoxEnabled = false;
                        }
                    }

                    #region Old

                    //ObjectTypes.Clear();
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.ACDQueue.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.Agent.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.AgentGroup.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.DNGroup.ToString(), IsComboBoxEnabled = false });
                    //ObjectTypes.Add(new ObjectType() { Text = StatisticsEnum.ObjectType.VirtualQueue.ToString(), IsComboBoxEnabled = true });

                    #endregion

                #endregion

                }
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : ConfiguredStatisticsChecked() Method - Exception caught" + generalException);
            }
            logger.Debug("ObjectConfigWindowViewModel : ConfiguredStatisticsChecked() Method - exit");
        }
        #endregion

        #region ConfiguredStatisticsUnChecked
        private void ConfiguredStatisticsUnChecked(object obj)
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : ConfiguredStatisticsUnChecked() Method - Entry");
                bool temp = true;

                TempCollection.Clear();
                foreach (var item in ConfiguredStatistics)
                {
                    if (item.isGridChecked == true)
                    {
                        TempCollection.Add(item);
                        temp = false;
                    }

                }
                if (temp)
                {
                    ConfiguredStatistics.Clear();
                    foreach (string SectionName in Settings.GetInstance().DictExistingApplicationStats.Keys)
                    {
                        Dictionary<string, string> statDetails = new Dictionary<string, string>();

                        statDetails = Settings.GetInstance().DictExistingApplicationStats[SectionName];
                        string DisplayName = string.Empty;

                        foreach (string key in statDetails.Keys)
                        {
                            if (string.Compare(key, StatisticsEnum.StatProperties.DisplayName.ToString(), true) == 0)
                            {
                                DisplayName = statDetails[key].ToString();
                            }
                        }

                        ConfiguredStatistics.Add(new StatisticsProperties()
                        {
                            isGridChecked = false,

                            SectionName = SectionName,

                            DisplayName = DisplayName,

                            EditName = "Edit",

                            IsCheckBoxEnabled = true,

                        });


                    }
                }

            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : ConfiguredStatisticsUnChecked() Method - Entry" + generalException.Message.ToString());
            }
            logger.Debug("ObjectConfigWindowViewModel : ConfiguredStatisticsUnChecked() Method - Exit");

        }
        #endregion

        #region GetExistObjectList

        /// <summary>
        /// Gets the exist object list.
        /// </summary>
        /// <param name="SelectedObject">The selected object.</param>
        /// <param name="flag">if set to <c>true</c> [flag].</param>
        /// <returns></returns>
        private List<string> GetExistObjectList(string SelectedObject, bool flag)
        {
            List<string> RetrivedObjectList = new List<string>();
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : GetExistObjectList() Method - Entry");
                Dictionary<List<string>, List<string>> ExistObjectsDictionary = new Dictionary<List<string>, List<string>>();
                //List<string> RetrivedObjectList = new List<string>();
                ExistObjectsDictionary = Settings.GetInstance().ExistingValuesDictionary[SelectedObject];
                foreach (KeyValuePair<List<string>, List<string>> TempObj in ExistObjectsDictionary)
                {
                    if (flag)
                    {
                        RetrivedObjectList = TempObj.Key;
                    }
                    else
                    {
                        RetrivedObjectList = TempObj.Value;
                    }

                }
                logger.Debug("ObjectConfigWindowViewModel : GetExistObjectList() Method - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : GetExistObjectList() Method - Exception caught" + generalException.Message.ToString());
            }
            return RetrivedObjectList;
        }
        #endregion

        #region AgentGroupToggleButtonClicked

        /// <summary>
        /// Agents the group toggle button clicked.
        /// </summary>
        public void AgentGroupToggleButtonClicked()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : AgentGroupToggleButtonClicked() Method - Entry");
                GridLength length = new GridLength(0);
                if ((WindowWidth != length) && (IsAgentChecked == false))
                {
                    WindowWidth = length;
                }
                else
                {
                    WindowWidth = GridLength.Auto;
                    IsAgentGroupChecked = true;
                    IsAgentChecked = false;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : AgentGroupToggleButtonClicked() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("ObjectConfigWindowViewModel : AgentGroupToggleButtonClicked() Method - Exit");
        }

        #endregion

        #region AgentToggleButtonClicked

        /// <summary>
        /// Agents the toggle button clicked.
        /// </summary>
        public void AgentToggleButtonClicked()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : AgentToggleButtonClicked() Method - Entry");
                GridLength length = new GridLength(0);
                if ((WindowWidth != length) && (IsAgentGroupChecked == false))
                {
                    WindowWidth = length;
                }
                else
                {
                    WindowWidth = GridLength.Auto;
                    IsAgentChecked = true;
                    IsAgentGroupChecked = false;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : AgentToggleButtonClicked() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("ObjectConfigWindowViewModel : AgentToggleButtonClicked() Method - Exit");
        }

        #endregion

        #region SaveNewObjects
        public void SaveNewObjects(object obj)
        {

            try
            {
                logger.Debug("ObjectConfigWindowViewModel : SaveNewObjects Method - Entry");
                //bool ErrorFlag;
                Dictionary<string, Dictionary<string, List<string>>> TempObjectDictionary = new Dictionary<string, Dictionary<string, List<string>>>();

                if (Settings.GetInstance().DictAgentObjects.ContainsKey(ObjectId))
                {
                    Dictionary<string, List<string>> ObjectsDictionary = new Dictionary<string, List<string>>();

                    foreach (string key in Settings.GetInstance().DictAgentObjects[ObjectId].Keys)
                    {
                        List<string> list = new List<string>(Settings.GetInstance().DictAgentObjects[ObjectId][key]);

                        ObjectsDictionary.Add(key, list);
                    }
                    //ObjectsDictionary = Settings.GetInstance().DictAgentObjects[ObjectId];
                    foreach (ObjectProperties objProperties in SelectedObject)
                    {

                        if (objProperties.IsObjectChecked)
                        {


                            if (SelectedStatName.StartsWith("acd"))
                            {
                                if (!ObjectsDictionary[StatisticsEnum.ObjectType.ACDQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.ACDQueue.ToString()].Add(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName);
                                }

                            }
                            else if (SelectedStatName.StartsWith("dn"))
                            {
                                if (!ObjectsDictionary[StatisticsEnum.ObjectType.DNGroup.ToString()].Contains(objProperties.ObjectName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.DNGroup.ToString()].Add(objProperties.ObjectName + "_@" + SelectedStatName);

                                }
                            }
                            else if (SelectedStatName.StartsWith("vq"))
                            {
                                if (!ObjectsDictionary[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Add(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName);

                                }
                            }
                        }
                        else
                        {
                            if (SelectedStatName.StartsWith("acd"))
                            {
                                if (ObjectsDictionary[StatisticsEnum.ObjectType.ACDQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.ACDQueue.ToString()].Remove(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName);
                                }

                            }
                            else if (SelectedStatName.StartsWith("dn"))
                            {
                                if (ObjectsDictionary[StatisticsEnum.ObjectType.DNGroup.ToString()].Contains(objProperties.ObjectName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.DNGroup.ToString()].Remove(objProperties.ObjectName + "_@" + SelectedStatName);

                                }
                            }
                            else if (SelectedStatName.StartsWith("vq"))
                            {
                                if (ObjectsDictionary[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Remove(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName);

                                }
                            }
                        }

                    }
                    Settings.GetInstance().DictAgentObjects[ObjectId].Clear();
                    foreach (string key in ObjectsDictionary.Keys)
                    {
                        List<string> lstObjects = new List<string>(ObjectsDictionary[key]);
                        Settings.GetInstance().DictAgentObjects[ObjectId].Add(key, lstObjects);
                    }

                }
                else if (Settings.GetInstance().DictAgentGroupObjects.ContainsKey(ObjectId))
                {
                    Dictionary<string, List<string>> ObjectsDictionary = new Dictionary<string, List<string>>();
                    foreach (string key in Settings.GetInstance().DictAgentGroupObjects[ObjectId].Keys)
                    {
                        List<string> list = new List<string>(Settings.GetInstance().DictAgentGroupObjects[ObjectId][key]);
                        ObjectsDictionary.Add(key, list);
                    }
                    //ObjectsDictionary = Settings.GetInstance().DictAgentGroupObjects[ObjectId];
                    foreach (ObjectProperties objProperties in SelectedObject)
                    {

                        if (objProperties.IsObjectChecked)
                        {


                            if (SelectedStatName.StartsWith("acd"))
                            {
                                if (!ObjectsDictionary[StatisticsEnum.ObjectType.ACDQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.ACDQueue.ToString()].Add(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName);
                                }

                            }
                            else if (SelectedStatName.StartsWith("dn"))
                            {
                                if (!ObjectsDictionary[StatisticsEnum.ObjectType.DNGroup.ToString()].Contains(objProperties.ObjectName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.DNGroup.ToString()].Add(objProperties.ObjectName + "_@" + SelectedStatName);

                                }
                            }
                            else if (SelectedStatName.StartsWith("vq"))
                            {
                                if (!ObjectsDictionary[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Add(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName);

                                }
                            }
                        }
                        else
                        {
                            if (SelectedStatName.StartsWith("acd"))
                            {
                                if (ObjectsDictionary[StatisticsEnum.ObjectType.ACDQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.ACDQueue.ToString()].Remove(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName);
                                }

                            }
                            else if (SelectedStatName.StartsWith("dn"))
                            {
                                if (ObjectsDictionary[StatisticsEnum.ObjectType.DNGroup.ToString()].Contains(objProperties.ObjectName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.DNGroup.ToString()].Remove(objProperties.ObjectName + "_@" + SelectedStatName);

                                }
                            }
                            else if (SelectedStatName.StartsWith("vq"))
                            {
                                if (ObjectsDictionary[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName))
                                {
                                    ObjectsDictionary[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Remove(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName + "_@" + SelectedStatName);

                                }
                            }
                        }

                    }
                    Settings.GetInstance().DictAgentGroupObjects[ObjectId].Clear();
                    foreach (string key in ObjectsDictionary.Keys)
                    {
                        List<string> lstAgentObjects = new List<string>(ObjectsDictionary[key]);
                        Settings.GetInstance().DictAgentGroupObjects[ObjectId].Add(key, lstAgentObjects);
                    }

                }


                //ErrorFlag = objStatBase.SaveLevelStatsObjects(Settings.GetInstance().NewStatisticsDictionary);
                foreach (Window current in Application.Current.Windows)
                {
                    if (current.Title == obj.ToString())
                    {
                        current.Close();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : SaveNewObjects Method - Exception" + generalException.Message.ToString());
            }
            finally
            {

            }
            logger.Debug("ObjectConfigWindowViewModel : SaveNewObjects Method - Exit");

        }
        #endregion

        #region NewObjectChecked
        public void NewObjectChecked()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : NewObjectChecked() Method - Entry");
                SelectedStatList.Clear();
                foreach (StatisticsProperties StatObj in ConfiguredStatistics)
                {
                    if (StatObj.isGridChecked)
                    {
                        SelectedStatList.Add(StatObj.SectionName);
                    }
                }
                ReadUserLevelObjects(objectType);
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : NewObjectChecked() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("ObjectConfigWindowViewModel : NewObjectChecked() Method - Exit");

        }
        #endregion

        #region ObjectsChecked
        public void ObjectsChecked(object obj)
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : ObjectsChecked() Method - Entry");
                ObjectProperties configuredObj = obj as ObjectProperties;
                bool isIncrement = true;

                if (configuredObj.IsObjectChecked)
                {
                    if (NumberofObjectsChecked < MaxObjectsChecked)
                    {
                        SelectedObjectList.Clear();
                        foreach (ObjectProperties objProperties in SelectedObject)
                        {

                            if (objProperties.IsObjectChecked)
                            {
                                SelectedObjectList.Add(objProperties.ObjectName);
                                if (NumberofObjectsChecked < MaxObjectsChecked && isIncrement)
                                {
                                    isIncrement = false;
                                    ++NumberofObjectsChecked;
                                    SelectedCount = NumberofObjectsChecked.ToString() + " Object(s) Selected";
                                    IsDeselectAllChecked = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        Views.MessageBox msgbox;
                        ViewModels.MessageBoxViewModel mboxvmodel;
                        msgbox = new Views.MessageBox();
                        mboxvmodel = new MessageBoxViewModel("AID Message", "Maximum " + MaxObjectsChecked.ToString() + " Queues are allowed.", msgbox, "MainWindow", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxvmodel;
                        msgbox.ShowDialog();
                        configuredObj.IsObjectChecked = false;
                    }
                }
                else
                {
                    NumberofObjectsChecked--;
                    SelectedCount = NumberofObjectsChecked.ToString() + " Object(s) Selected";
                    if(NumberofObjectsChecked==0)
                    IsDeselectAllChecked = false;
                }

            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : ObjectsChecked() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("ObjectConfigWindowViewModel : ObjectsChecked() Method - Exit");
        }
        #endregion

        #region SaveObjects
        public void SaveObjects()
        {

            try
            {
                logger.Debug("ObjectConfigWindowViewModel : SaveObjects() Method - Exit");

                bool ErrorFlag = false;
                SelectedStatList.Clear();
                foreach (StatisticsProperties StatObj in ConfiguredStatistics)
                {
                    if (StatObj.isGridChecked)
                    {
                        SelectedStatList.Add(StatObj.SectionName);
                    }
                }
                SelectedObjectList.Clear();
                foreach (ObjectProperties objProperties in SelectedObject)
                {
                    if (objProperties.IsObjectChecked)
                    {
                        SelectedObjectList.Add(objProperties.ObjectName);
                    }
                }

                //if (SelectedStatList.Count > 0 && SelectedObjectList.Count > 0)
                if (SelectedObjectList.Count > 0)
                {                    
                    if (SelectedObjectList.Count <= MaxObjectsChecked)
                    {
                        #region Save objects

                        Dictionary<string, Dictionary<string, List<string>>> ObjectsDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                        ObjectsDictionary.Add("Objects", new Dictionary<string, List<string>>());

                        if (Settings.GetInstance().DictApplicationObjects.ContainsKey(ObjectId))
                        {
                            foreach (string key in Settings.GetInstance().DictApplicationObjects[ObjectId].Keys)
                            {
                                List<string> lstAppObjects = new List<string>(Settings.GetInstance().DictApplicationObjects[ObjectId][key]);
                                ObjectsDictionary["Objects"].Add(key, lstAppObjects);
                            }
                        }

                        string agentId = objStatBase.GetAgentId();
                        if (Settings.GetInstance().DictAgentObjects.ContainsKey(agentId))
                        {
                            if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(agentId))
                            {
                                string Dilimitor = "_@";
                                Dictionary<string, List<string>> dictStats = new Dictionary<string, List<string>>();
                                dictStats.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), new List<string>());
                                dictStats.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), new List<string>());
                                dictStats.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), new List<string>());

                                foreach (string stat in Settings.GetInstance().DictAgentStatisitics[agentId])
                                {

                                    foreach (string key in Settings.GetInstance().DictAgentObjects[agentId].Keys)
                                    {
                                        if ((stat.StartsWith("acd") && (key == "ACDQueue")) || (stat.StartsWith("dn") && (key == "DNGroup")) || (stat.StartsWith("vq") && (key == "VirtualQueue")))
                                        {
                                            foreach (string obj in Settings.GetInstance().DictAgentObjects[agentId][key])
                                            {
                                                string[] tempArray = null;
                                                tempArray = obj.Split(new[] { Dilimitor }, StringSplitOptions.None);

                                                if (stat.StartsWith("acd"))
                                                {
                                                    if (string.Compare(tempArray[2], stat, true) == 0)
                                                    {
                                                        dictStats[StatisticsEnum.ObjectType.ACDQueue.ToString()].Add(obj);
                                                    }
                                                }
                                                else if (stat.StartsWith("dn"))
                                                {
                                                    if (string.Compare(tempArray[1], stat, true) == 0)
                                                    {
                                                        dictStats[StatisticsEnum.ObjectType.DNGroup.ToString()].Add(obj);
                                                    }
                                                }
                                                else if (stat.StartsWith("vq"))
                                                {
                                                    if (string.Compare(tempArray[2], stat, true) == 0)
                                                    {
                                                        dictStats[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Add(obj);
                                                    }
                                                }

                                            }
                                        }


                                    }
                                }
                                ObjectsDictionary["Objects"] = dictStats;
                            }

                        }
                        else if (Settings.GetInstance().DictAgentGroupObjects.ContainsKey(ObjectId))
                        {
                            if (Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(ObjectId))
                            {
                                string Dilimitor = "_@";
                                Dictionary<string, List<string>> dictStats = new Dictionary<string, List<string>>();
                                dictStats.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), new List<string>());
                                dictStats.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), new List<string>());
                                dictStats.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), new List<string>());

                                foreach (string stat in Settings.GetInstance().DictAgentGroupStatisitics[ObjectId])
                                {

                                    foreach (string key in Settings.GetInstance().DictAgentGroupObjects[ObjectId].Keys)
                                    {
                                        if ((stat.StartsWith("acd") && (key == "ACDQueue")) || (stat.StartsWith("dn") && (key == "DNGroup")) || (stat.StartsWith("vq") && (key == "VirtualQueue")))
                                        {
                                            foreach (string obj in Settings.GetInstance().DictAgentGroupObjects[ObjectId][key])
                                            {
                                                string[] tempArray = null;
                                                tempArray = obj.Split(new[] { Dilimitor }, StringSplitOptions.None);

                                                if (stat.StartsWith("acd"))
                                                {
                                                    if (string.Compare(tempArray[2], stat, true) == 0)
                                                    {
                                                        dictStats[StatisticsEnum.ObjectType.ACDQueue.ToString()].Add(obj);
                                                    }
                                                }
                                                else if (stat.StartsWith("dn"))
                                                {
                                                    if (string.Compare(tempArray[1], stat, true) == 0)
                                                    {
                                                        dictStats[StatisticsEnum.ObjectType.DNGroup.ToString()].Add(obj);
                                                    }
                                                }
                                                else if (stat.StartsWith("vq"))
                                                {
                                                    if (string.Compare(tempArray[2], stat, true) == 0)
                                                    {
                                                        dictStats[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Add(obj);
                                                    }
                                                }

                                            }
                                        }


                                    }
                                }
                                ObjectsDictionary["Objects"] = dictStats;
                            }

                        }

                        foreach (ObjectProperties objProperties in SelectedObject)
                        {

                            if (objProperties.IsObjectChecked)
                            {

                                if (string.Compare(objProperties.TypeObject, "acdqueue", true) == 0)
                                {
                                    if (!ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.ACDQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName))
                                    {
                                        ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.ACDQueue.ToString()].Add(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName);
                                    }

                                }
                                else if (string.Compare(objProperties.TypeObject, "dngroup", true) == 0)
                                {
                                    if (!ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.DNGroup.ToString()].Contains(objProperties.ObjectName))
                                    {
                                        ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.DNGroup.ToString()].Add(objProperties.ObjectName);

                                    }
                                }
                                else if (string.Compare(objProperties.TypeObject, "VirtualQueue", true) == 0)
                                {
                                    if (!ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.VirtualQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName))
                                    {
                                        ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.VirtualQueue.ToString()].Add(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName);

                                    }
                                }

                            }
                            else
                            {
                                if (string.Compare(objProperties.TypeObject, "acdqueue", true) == 0)
                                {
                                    if (ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.ACDQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName))
                                    {
                                        ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.ACDQueue.ToString()].Remove(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName);
                                    }

                                }
                                else if (string.Compare(objProperties.TypeObject, "groupqueues", true) == 0)
                                {
                                    if (ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.DNGroup.ToString()].Contains(objProperties.ObjectName))
                                    {
                                        ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.DNGroup.ToString()].Remove(objProperties.ObjectName);

                                    }
                                }
                                else if (string.Compare(objProperties.TypeObject, "VirtualQueue", true) == 0)
                                {
                                    if (ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.VirtualQueue.ToString()].Contains(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName))
                                    {
                                        ObjectsDictionary["Objects"][StatisticsEnum.ObjectType.VirtualQueue.ToString()].Remove(objProperties.ObjectName + "_@" + objProperties.ObjectSwitchName);

                                    }
                                }
                            }
                        }

                        if (Settings.GetInstance().DictApplicationObjects.ContainsKey(ObjectId))
                        {
                            Settings.GetInstance().DictApplicationObjects[ObjectId].Clear();
                            foreach (string key in ObjectsDictionary["Objects"].Keys)
                            {
                                List<string> lstAppObjects = new List<string>(ObjectsDictionary["Objects"][key]);
                                Settings.GetInstance().DictApplicationObjects[ObjectId].Add(key, lstAppObjects);
                            }

                        }
                        else if (Settings.GetInstance().DictAgentObjects.ContainsKey(ObjectId))
                        {
                            Settings.GetInstance().DictAgentObjects[ObjectId].Clear();
                            foreach (string key in ObjectsDictionary["Objects"].Keys)
                            {
                                List<string> lstAgentObjects = new List<string>(ObjectsDictionary["Objects"][key]);
                                Settings.GetInstance().DictAgentObjects[ObjectId].Add(key, lstAgentObjects);
                            }
                            //Settings.GetInstance().DictAgentObjects[ObjectId] = ObjectsDictionary["Objects"];
                        }
                        else if (Settings.GetInstance().DictAgentGroupObjects.ContainsKey(ObjectId))
                        {
                            Settings.GetInstance().DictAgentGroupObjects[ObjectId].Clear();
                            foreach (string key in ObjectsDictionary["Objects"].Keys)
                            {
                                List<string> lstAgentGroupObjects = new List<string>(ObjectsDictionary["Objects"][key]);
                                Settings.GetInstance().DictAgentGroupObjects[ObjectId].Add(key, lstAgentGroupObjects);
                            }
                            //Settings.GetInstance().DictAgentGroupObjects[ObjectId] = ObjectsDictionary["Objects"];
                        }
                        if (Settings.GetInstance().DictConfiguredStatisticsObjects.ContainsKey(ObjectId))
                        {
                            Settings.GetInstance().DictConfiguredStatisticsObjects[ObjectId]["Objects"] = ObjectsDictionary["Objects"];
                        }
                        else
                        {
                            Dictionary<string, List<string>> StatisticsDictionary = new Dictionary<string, List<string>>();
                            //StatisticsDictionary.Add(StatisticsEnum.ObjectType.Agent.ToString(), new List<string>());
                            //StatisticsDictionary.Add(StatisticsEnum.ObjectType.AgentGroup.ToString(), new List<string>());
                            //StatisticsDictionary.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), new List<string>());
                            //StatisticsDictionary.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), new List<string>());
                            //StatisticsDictionary.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), new List<string>());
                            ObjectsDictionary.Add("Statistics", StatisticsDictionary);

                            Settings.GetInstance().DictConfiguredStatisticsObjects.Add(ObjectId, ObjectsDictionary);
                        }

                        ErrorFlag = objStatBase.SaveLevelStatsObjects(Settings.GetInstance().DictConfiguredStatisticsObjects, ObjectId);

                        #endregion
                    }
                }
                else
                {
                    Dictionary<string, Dictionary<string, List<string>>> ObjectsDictionary = new Dictionary<string, Dictionary<string, List<string>>>();

                    Dictionary<string, List<string>> dictStats = new Dictionary<string, List<string>>();
                    dictStats.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), new List<string>());
                    dictStats.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), new List<string>());
                    dictStats.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), new List<string>());

                    Dictionary<string, List<string>> StatisticsDictionary = new Dictionary<string, List<string>>();
                    ObjectsDictionary.Add("Statistics", StatisticsDictionary);
                    ObjectsDictionary.Add("Objects", new Dictionary<string, List<string>>());
                    ObjectsDictionary["Objects"] = dictStats;
                    if (!Settings.GetInstance().DictConfiguredStatisticsObjects.ContainsKey(ObjectId))
                        Settings.GetInstance().DictConfiguredStatisticsObjects.Add(ObjectId, ObjectsDictionary);
                    else
                        Settings.GetInstance().DictConfiguredStatisticsObjects[ObjectId] = ObjectsDictionary;

                    ErrorFlag = objStatBase.SaveLevelStatsObjects(Settings.GetInstance().DictConfiguredStatisticsObjects, ObjectId);
                }


                if (ErrorFlag)
                {
                    #region Updating the receiving statistics
                    
                    //Commented by Elango, to restrict from updating the receiving stats in live
                    //objStatBase.CancelStatRequest();
                    //if (Settings.GetInstance().mainVm != null)
                    //{
                    //    Settings.GetInstance().DicStatInfo.Clear();
                    //    Settings.GetInstance().DictAllStats = objStatTicker.GetAllStats();
                    //    Settings.GetInstance().HshAllTagList.Clear();
                    //    MainWindowViewModel.hashTaggedStats.Clear();
                    //    Settings.GetInstance().mainVm.MyTagControlCollection.Clear();
                    //    Settings.GetInstance().mainVm.MyTagControlTempCollection.Clear();
                    //    Settings.GetInstance().mainVm.MyTagControlCollection2.Clear();
                    //    MainWindowViewModel.TagNo = 0;
                    //    if (Settings.GetInstance().DictEnableDisableChannels["tagvertical"])
                    //    {
                    //        Settings.GetInstance().mainVm.TotalGridHeight = (int)Settings.GetInstance().mainVm.GadgetHeight;
                    //        Settings.GetInstance().mainVm.WrapWidth = Settings.GetInstance().mainVm.TagWidth * 1;
                    //    }
                    //    else
                    //    {
                    //        Settings.GetInstance().mainVm.TotalGridHeight = (int)Settings.GetInstance().mainVm.GadgetHeight;
                    //        Settings.GetInstance().mainVm.WrapWidth = Settings.GetInstance().mainVm.TagWidth * 1;
                    //    }
                    //}
                    //objStatBase.SendToAIDPipe();
                    //objStatBase.StartUpdatedStatistics();
                    //NumberofObjectsChecked = 1;

                    ConfigurationCancel("ObjectConfigurations");
                    #endregion
                }
                else
                {
                    if (SelectedObjectList.Count > MaxObjectsChecked)
                    {
                        Views.MessageBox msgbox;
                        ViewModels.MessageBoxViewModel mboxvmodel;
                        msgbox = new Views.MessageBox();
                        mboxvmodel = new MessageBoxViewModel("Information", "Selected Objects count exceeds the Configured(" + MaxObjectsChecked + ") number. Please Check and Save again.", msgbox, "MainWindow", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxvmodel;
                        msgbox.ShowDialog();
                    }
                    else
                    {

                        Views.MessageBox msgbox;
                        ViewModels.MessageBoxViewModel mboxvmodel;
                        msgbox = new Views.MessageBox();
                        mboxvmodel = new MessageBoxViewModel("Information", "Objects not Configured, Save again.", msgbox, "MainWindow", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxvmodel;
                        msgbox.ShowDialog();
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : SaveObjects() Method - Exception" + generalException.Message.ToString());
            }
            finally
            {

            }
            logger.Debug("ObjectConfigWindowViewModel : SaveObjects() Method - Exit");

        }
        #endregion

        #region GetConfiguredObjects
        public void GetConfiguredObjects()
        {
            try
            {
                logger.Debug("ObjectConfigWindowViewModel : GetServerObjects() Method - Entry");
                Dictionary<string, Dictionary<string, List<string>>> AgentObjectsDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                Dictionary<string, Dictionary<string, List<string>>> AgentGroupDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                Dictionary<string, Dictionary<string, List<string>>> AgentGroupObjectsDictionary = new Dictionary<string, Dictionary<string, List<string>>>();
                Dictionary<string, Dictionary<string, List<string>>> ApplicationObjectsDictionary = new Dictionary<string, Dictionary<string, List<string>>>();

                Dictionary<string, List<string>> ObjectValuesDictionary = new Dictionary<string, List<string>>();

                AgentObjectsDictionary.Add("Objects", new Dictionary<string, List<string>>());

                if (Settings.GetInstance().DictAgentObjects.ContainsKey(ObjectId))
                {

                    foreach (string item in Settings.GetInstance().DictAgentObjects[ObjectId].Keys)
                    {
                        List<string> lstAgentObjects = new List<string>(Settings.GetInstance().DictAgentObjects[ObjectId][item]);
                        AgentObjectsDictionary["Objects"].Add(item, lstAgentObjects);
                    }
                    foreach (string key in objStatBase.ReadStatisticsObjects(ObjectId, StatisticsEnum.ObjectType.Agent.ToString()).Keys)
                    {
                        Dictionary<string, List<string>> dictAgentGroupObjects = new Dictionary<string, List<string>>(objStatBase.ReadStatisticsObjects(ObjectId, StatisticsEnum.ObjectType.Agent.ToString())[key]);
                        AgentGroupObjectsDictionary.Add(key, dictAgentGroupObjects);
                    }
                    foreach (string key in AgentGroupObjectsDictionary.Keys)
                    {
                        if (string.Compare(key, "Application", true) == 0)
                        {
                            Dictionary<string, List<string>> tempDictionary = new Dictionary<string, List<string>>();
                            tempDictionary.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), new List<string>());
                            tempDictionary.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), new List<string>());
                            tempDictionary.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), new List<string>());
                            AgentGroupObjectsDictionary.Remove("Objects");
                            AgentGroupObjectsDictionary.Add("Objects", tempDictionary);
                            break;
                        }
                    }
                }

                if (Settings.GetInstance().DictAgentObjects.ContainsKey(ObjectId))
                {
                    ObjectValuesDictionary.Clear();
                    foreach (string key in AgentObjectsDictionary["Objects"].Keys)
                    {
                        List<string> lstAgentObject = new List<string>(AgentObjectsDictionary["Objects"][key]);
                        ObjectValuesDictionary.Add(key, lstAgentObject);
                    }

                    if (ObjectValuesDictionary[StatisticsEnum.ObjectType.ACDQueue.ToString()].Count == 0)
                    {
                        if (AgentGroupObjectsDictionary["Objects"][StatisticsEnum.ObjectType.ACDQueue.ToString()].Count == 0)
                        {
                            ObjectValuesDictionary.Remove(StatisticsEnum.ObjectType.ACDQueue.ToString());
                            ObjectValuesDictionary.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), new List<string>(Settings.GetInstance().DictApplicationObjects[Settings.GetInstance().ApplicationName.ToString()][StatisticsEnum.ObjectType.ACDQueue.ToString()]));

                        }
                        else
                        {
                            ObjectValuesDictionary.Remove(StatisticsEnum.ObjectType.ACDQueue.ToString());
                            ObjectValuesDictionary.Add(StatisticsEnum.ObjectType.ACDQueue.ToString(), new List<string>(AgentGroupObjectsDictionary["Objects"][StatisticsEnum.ObjectType.ACDQueue.ToString()]));

                        }
                    }
                    if (ObjectValuesDictionary[StatisticsEnum.ObjectType.DNGroup.ToString()].Count == 0)
                    {
                        if (AgentGroupObjectsDictionary["Objects"][StatisticsEnum.ObjectType.DNGroup.ToString()].Count == 0)
                        {
                            ObjectValuesDictionary.Remove(StatisticsEnum.ObjectType.DNGroup.ToString());
                            ObjectValuesDictionary.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), new List<string>(Settings.GetInstance().DictApplicationObjects[Settings.GetInstance().ApplicationName.ToString()][StatisticsEnum.ObjectType.DNGroup.ToString()]));

                        }
                        else
                        {
                            ObjectValuesDictionary.Remove(StatisticsEnum.ObjectType.DNGroup.ToString());
                            ObjectValuesDictionary.Add(StatisticsEnum.ObjectType.DNGroup.ToString(), new List<string>(AgentGroupObjectsDictionary["Objects"][StatisticsEnum.ObjectType.DNGroup.ToString()]));

                        }
                    }
                    if (ObjectValuesDictionary[StatisticsEnum.ObjectType.VirtualQueue.ToString()].Count == 0)
                    {
                        if (AgentGroupObjectsDictionary["Objects"][StatisticsEnum.ObjectType.VirtualQueue.ToString()].Count == 0)
                        {
                            ObjectValuesDictionary.Remove(StatisticsEnum.ObjectType.VirtualQueue.ToString());
                            ObjectValuesDictionary.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), new List<string>(Settings.GetInstance().DictApplicationObjects[Settings.GetInstance().ApplicationName.ToString()][StatisticsEnum.ObjectType.VirtualQueue.ToString()]));

                        }
                        else
                        {
                            ObjectValuesDictionary.Remove(StatisticsEnum.ObjectType.VirtualQueue.ToString());
                            ObjectValuesDictionary.Add(StatisticsEnum.ObjectType.VirtualQueue.ToString(), new List<string>(AgentGroupObjectsDictionary["Objects"][StatisticsEnum.ObjectType.VirtualQueue.ToString()]));

                        }
                    }

                    Settings.GetInstance().DictAgentObjects[ObjectId].Clear();
                    foreach (string key in ObjectValuesDictionary.Keys)
                    {
                        List<string> lstAgentObjects = new List<string>(ObjectValuesDictionary[key]);
                        Settings.GetInstance().DictAgentObjects[ObjectId].Add(key, lstAgentObjects);
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : GetServerObjects() Method - Exception caught" + generalException.Message.ToString());
            }
            finally
            {

            }
            logger.Debug("ObjectConfigWindowViewModel : GetServerObjects() Method - Exit");
        }

        #endregion

        public void DeselectAll(object obj)
        {
            try
            {

                logger.Debug("ObjectConfigWindowViewModel : DeselectAll() Method - Entry");

                CheckBox tmpcbox = obj as CheckBox;

                if (tmpcbox.IsChecked == true)
                {
                    IsDeselectAllChecked = false;
                    //tmpcbox.IsChecked = false;
                }
                else
                {
                    foreach (ObjectProperties objProperties in SelectedObject)
                    {
                        if (objProperties.IsObjectChecked)
                        {
                            objProperties.IsObjectChecked = false;
                        }
                    }
                    NumberofObjectsChecked = 0;
                    SelectedCount = NumberofObjectsChecked.ToString() + " Object(s) Selected";
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ObjectConfigWindowViewModel : DeselectAll() Method - Exception caught" + generalException.Message.ToString());
            }
        }

        #endregion
    }
}