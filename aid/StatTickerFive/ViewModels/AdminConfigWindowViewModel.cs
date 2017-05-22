using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StatTickerFive.Helpers;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using StatTickerFive.Views;
using Pointel.Logger.Core;
using Pointel.Statistics.Core;
using Genesyslab.Platform.Commons.Collections;
using Pointel.Statistics.Core.Utility;
using System.Windows.Threading;
using System.Windows.Controls;

namespace StatTickerFive.ViewModels
{
    public class AdminConfigWindowViewModel : NotificationObject
    {
        #region Public Declarations

        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        StatisticsBase objStatTicker = new StatisticsBase();
        StatisticsSupport objStatSupport = new StatisticsSupport();

        //Statistics Properties
        public Window StatisticsPropertieswindow = null;
        public string SectionType = string.Empty;
        public string ExistingSection = string.Empty;
        public string Statistics = string.Empty;
        public string EmptyField = "Enter a value here";
        public string DecimalFormat = "Value in decimal format";
        public string TimeFormat = "Value in Time format";
        public string ThresholdMessage2 = "Value Greater than Threshold2";
        public string ThresholdMessage1 = "Value less than Threshold2";
        public string ExistFilter = string.Empty;
        public string ExistSectionType = string.Empty;
        delegate void UpdateApplicationDetails(string objectType);
        static event UpdateApplicationDetails _UpdateApplicationDetails;

        public bool isObjectStatAlreadyConfigured = true;
        public bool isThresholdFirstTime = true;
        public StatisticsProperties stat;

        #endregion

        #region Constructor
        public AdminConfigWindowViewModel()
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : Constructor - Entry");
                ConfiguredStatistics = new ObservableCollection<StatisticsProperties>();
                ServerStatistics = new ObservableCollection<StatisticsProperties>();
                GridWidth = new GridLength(0);
                TitleSpan = 0;

                WinActivated();
                ApplicationName = Settings.GetInstance().ApplicationName;

                Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> dictTheme = new Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush>();
                dictTheme = objStatSupport.ThemeSelector(Settings.GetInstance().Theme);

                TitleBackground = dictTheme[StatisticsEnum.ThemeColors.TitleBackground];
                BackgroundColor = dictTheme[StatisticsEnum.ThemeColors.BackgroundColor];
                TitleForeground = dictTheme[StatisticsEnum.ThemeColors.TitleForeground];
                BorderBrush = dictTheme[StatisticsEnum.ThemeColors.BorderBrush];
                ShadowEffect = new DropShadowBitmapEffect();
                ShadowEffect.Color = (Color)BorderBrush.Color;

                Sections = new System.Collections.Generic.List<string>();
                Sections.Add(StatisticsEnum.SectionName.acd.ToString());
                Sections.Add(StatisticsEnum.SectionName.agent.ToString());
                Sections.Add(StatisticsEnum.SectionName.group.ToString());
                Sections.Add(StatisticsEnum.SectionName.dn.ToString());
                Sections.Add(StatisticsEnum.SectionName.vq.ToString());

                //ApplicationStatisticsConfig();

                FormatSource = new ObservableCollection<string>();
                FilterSource = new ObservableCollection<string>();
                MediaTypeSource = new ObservableCollection<string>();
                SortMediaTypeSource = new ObservableCollection<string>();
                SortMediaTypeSource.Clear();
                _UpdateApplicationDetails += ApplicationUpdate;

                ApplicationStatisticsConfig();
                SortMeidaTypeChanged(SortMediaTypeSource[0]);

            }
            catch (Exception generalException)
            {
                logger.Error("AdminConfigWindowViewModel : Constructor - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : Constructor - Exit");
        }
        #endregion

        #region Properties

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

        private SolidColorBrush _existingColor;
        public SolidColorBrush ExistingColor
        {
            get
            {
                return _existingColor;
            }
            set
            {
                _existingColor = value;
                RaisePropertyChanged(() => ExistingColor);
            }
        }

        private SolidColorBrush _serverColor;
        public SolidColorBrush ServerColor
        {
            get
            {
                return _serverColor;
            }
            set
            {
                _serverColor = value;
                RaisePropertyChanged(() => ServerColor);
            }
        }

        private GridLength _GridWidth;
        public GridLength GridWidth
        {
            get
            {
                return _GridWidth;
            }
            set
            {
                _GridWidth = value;
                RaisePropertyChanged(() => GridWidth);
            }
        }

        private int _TitleSpan;
        public int TitleSpan
        {
            get
            {
                return _TitleSpan;
            }
            set
            {
                _TitleSpan = value;
                RaisePropertyChanged(() => TitleSpan);
            }
        }

        private ObservableCollection<ObjectValues> _TempCheckedObjectList;
        public ObservableCollection<ObjectValues> TempCheckedObjectList
        {
            get { return _TempCheckedObjectList; }
            set
            {
                _TempCheckedObjectList = value;
                RaisePropertyChanged(() => TempCheckedObjectList);
            }
        }

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

        #region CheckBoxID
        private bool _checkBoxID;
        public bool CheckBoxID
        {
            get { return _checkBoxID; }
            set
            {
                _checkBoxID = value;
                RaisePropertyChanged(() => CheckBoxID);
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

        #region EditName
        private string _editName;
        public string EditName
        {
            get { return _editName; }
            set
            {
                _editName = value;
                RaisePropertyChanged(() => EditName);
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

        #region IsEditEnable
        private bool _isEditEnable;
        public bool IsEditEnable
        {
            get { return _isEditEnable; }
            set
            {
                _isEditEnable = value;
                RaisePropertyChanged(() => IsEditEnable);
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

        #region ObservableCollection ServerStatistics
        private ObservableCollection<StatisticsProperties> _serverStatistics;
        public ObservableCollection<StatisticsProperties> ServerStatistics
        {
            get { return _serverStatistics; }
            set
            {
                _serverStatistics = value;
                RaisePropertyChanged(() => ServerStatistics);
            }
        }
        #endregion

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

        //Statistics Properties
        private string _title;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != null)
                {
                    _title = value;
                    RaisePropertyChanged(() => Title);
                }
            }

        }

        private int _statNameColSpan;
        public int StatNameColSpan
        {
            get
            {
                return _statNameColSpan;
            }
            set
            {
                if (value != null)
                {
                    _statNameColSpan = value;
                    RaisePropertyChanged(() => StatNameColSpan);
                }
            }
        }

        private Visibility _statNameSearch;
        public Visibility StatNameSearch
        {
            get
            {
                return _statNameSearch;
            }
            set
            {
                if (value != null)
                {
                    _statNameSearch = value;
                    RaisePropertyChanged(() => StatNameSearch);
                }
            }
        }

        private string _StatSectionName;
        public string StatSectionName
        {
            get
            {
                return _StatSectionName;
            }
            set
            {
                if (value != null)
                {
                    _StatSectionName = value;
                    RaisePropertyChanged(() => StatSectionName);
                }
            }
        }

        private string _PropDisplayName;
        public string PropDisplayName
        {
            get
            {
                return _PropDisplayName;
            }
            set
            {
                if (value != null)
                {
                    _PropDisplayName = value;
                    RaisePropertyChanged(() => PropDisplayName);
                }
            }
        }

        private string _PropMediaType;
        public string PropMediaType
        {
            get
            {
                return _PropMediaType;
            }
            set
            {
                if (value != null)
                {
                    _PropMediaType = value;
                    RaisePropertyChanged(() => PropMediaType);
                }
            }
        }

        private string _StatisticsName;
        public string StatisticsName
        {
            get
            {
                return _StatisticsName;
            }
            set
            {
                if (value != null)
                {
                    _StatisticsName = value;
                    RaisePropertyChanged(() => StatisticsName);
                }
            }
        }

        private ObservableCollection<string> _filterSource;
        public ObservableCollection<string> FilterSource
        {
            get
            {
                return _filterSource;
            }
            set
            {
                //if (value != null)
                //{
                _filterSource = value;
                RaisePropertyChanged(() => FilterSource);
                //}
            }
        }

        private ObservableCollection<string> _mediaTypeSource;
        public ObservableCollection<string> MediaTypeSource
        {
            get
            {
                return _mediaTypeSource;
            }
            set
            {
                //if (value != null)
                //{
                _mediaTypeSource = value;
                RaisePropertyChanged(() => MediaTypeSource);
                //}
            }
        }
        private List<string> _sections;
        public List<string> Sections
        {
            get
            {
                return _sections;
            }
            set
            {
                _sections = value;
                RaisePropertyChanged(() => Sections);
            }
        }

        private string _selectedSection;
        public string SelectedSection
        {
            get
            {
                return _selectedSection;
            }
            set
            {
                _selectedSection = value;
                RaisePropertyChanged(() => SelectedSection);
            }
        }

        private string _selectedFilter;
        public string SelectedFilter
        {
            get
            {
                return _selectedFilter;
            }
            set
            {
                if (value != null)
                {
                    _selectedFilter = value;
                    RaisePropertyChanged(() => SelectedFilter);
                }
            }
        }
        private string _selectedMediaType;
        public string SelectedMediaType
        {
            get
            {
                return _selectedMediaType;
            }
            set
            {
                //if (value != null)
                //{
                _selectedMediaType = value;
                RaisePropertyChanged(() => SelectedMediaType);
                //}
            }
        }

        private string _sortSelectedMediaType;
        public string SortSelectedMediaType
        {
            get
            {
                return _sortSelectedMediaType;
            }
            set
            {
                if (value != null)
                {
                    _sortSelectedMediaType = value;
                    RaisePropertyChanged(() => SortSelectedMediaType);
                }
            }
        }
        private ObservableCollection<string> _formatSource;
        public ObservableCollection<string> FormatSource
        {
            get
            {
                return _formatSource;
            }
            set
            {
                //if (value != null)
                //{
                _formatSource = value;
                RaisePropertyChanged(() => FormatSource);
                //}
            }
        }
        private ObservableCollection<string> _sortMediaTypeSource;
        public ObservableCollection<string> SortMediaTypeSource
        {
            get
            {
                return _sortMediaTypeSource;
            }
            set
            {
                //if (value != null)
                //{
                _sortMediaTypeSource = value;
                RaisePropertyChanged(() => SortMediaTypeSource);
                //}
            }
        }


        private string _selectedFormat;
        public string SelectedFormat
        {
            get
            {
                return _selectedFormat;
            }
            set
            {
                if (value != null)
                {
                    _selectedFormat = value;
                    RaisePropertyChanged(() => SelectedFormat);
                }
            }
        }

        private string _tooltipValue;
        public string TooltipValue
        {
            get
            {
                return _tooltipValue;
            }
            set
            {
                if (value != null)
                {
                    _tooltipValue = value;
                    RaisePropertyChanged(() => TooltipValue);
                }
            }
        }

        private string _threshold1;
        public string Threshold1
        {
            get
            {
                return _threshold1;
            }
            set
            {
                if (value != null)
                {
                    _threshold1 = value;
                    RaisePropertyChanged(() => Threshold1);
                }
            }
        }

        private string _threshold2;
        public string Threshold2
        {
            get
            {
                return _threshold2;
            }
            set
            {
                if (value != null)
                {
                    _threshold2 = value;
                    RaisePropertyChanged(() => Threshold2);
                }
            }
        }

        private System.Windows.Media.Color _statisticsColor;
        public System.Windows.Media.Color StatisticsColor
        {
            get
            {
                return _statisticsColor;
            }
            set
            {
                if (value != null)
                {
                    _statisticsColor = value;
                    RaisePropertyChanged(() => StatisticsColor);
                }
            }
        }

        private System.Windows.Media.Color _thresholdColor1;
        public System.Windows.Media.Color ThresholdColor1
        {
            get
            {
                return _thresholdColor1;
            }
            set
            {
                if (value != null)
                {
                    _thresholdColor1 = value;
                    RaisePropertyChanged(() => ThresholdColor1);
                }
            }
        }

        private System.Windows.Media.Color _thresholdColor2;
        public System.Windows.Media.Color ThresholdColor2
        {
            get
            {
                return _thresholdColor2;
            }
            set
            {
                if (value != null)
                {
                    _thresholdColor2 = value;
                    RaisePropertyChanged(() => ThresholdColor2);
                }
            }
        }

        private ObservableCollection<SearchedStatistics> _searchStatnameCollection;
        public ObservableCollection<SearchedStatistics> SearchStatnameCollection
        {
            get
            {
                return _searchStatnameCollection;
            }
            set
            {
                if (value != null)
                {
                    _searchStatnameCollection = value;
                    RaisePropertyChanged(() => SearchStatnameCollection);
                }
            }
        }

        private string _searchedStatname;
        public string SearchedStatname
        {
            get
            {
                return _searchedStatname;
            }
            set
            {
                if (value != null)
                {
                    _searchedStatname = value;
                    RaisePropertyChanged(() => SearchedStatname);
                }
            }
        }

        private string _searchedStatDescription;
        public string SearchedStatDescription
        {
            get
            {
                return _searchedStatDescription;
            }
            set
            {
                if (value != null)
                {
                    _searchedStatDescription = value;
                    RaisePropertyChanged(() => SearchedStatDescription);
                }
            }
        }

        private GridLength _dataGridRowHeight;
        public GridLength DataGridRowHeight
        {
            get
            {
                return _dataGridRowHeight;
            }
            set
            {
                if (value != null)
                {
                    _dataGridRowHeight = value;
                    RaisePropertyChanged(() => DataGridRowHeight);
                }
            }
        }

        private string _keyToDifferentiateStyles;
        public string KeyToDifferentiateStyles
        {
            get
            {
                return _keyToDifferentiateStyles;
            }
            set
            {
                if (value != null)
                {
                    _keyToDifferentiateStyles = value;
                    RaisePropertyChanged(() => KeyToDifferentiateStyles);
                }
            }
        }

        private bool _isSavebtnEnable;
        public bool IsSavebtnEnable
        {
            get
            {
                return _isSavebtnEnable;
            }
            set
            {
                if (value != null)
                {
                    _isSavebtnEnable = value;
                    RaisePropertyChanged(() => IsSavebtnEnable);
                }
            }
        }

        #region SectionNameWaterMarkText
        private string _SectionNameWaterMarkText;
        public string SectionNameWaterMarkText
        {
            get
            {
                return _SectionNameWaterMarkText;
            }
            set
            {
                if (value != null)
                {
                    _SectionNameWaterMarkText = value;
                    RaisePropertyChanged(() => SectionNameWaterMarkText);
                }
            }
        }
        #endregion

        #region SectionNameWaterMarkColor
        private SolidColorBrush _SectionNameWaterMarkColor;
        public SolidColorBrush SectionNameWaterMarkColor
        {
            get
            {
                return _SectionNameWaterMarkColor;
            }
            set
            {
                if (value != null)
                {
                    _SectionNameWaterMarkColor = value;
                    RaisePropertyChanged(() => SectionNameWaterMarkColor);
                }
            }
        }
        #endregion

        #region DisplayNameWaterMarkText
        private string _DisplayNameWaterMarkText;
        public string DisplayNameWaterMarkText
        {
            get
            {
                return _DisplayNameWaterMarkText;
            }
            set
            {
                if (value != null)
                {
                    _DisplayNameWaterMarkText = value;
                    RaisePropertyChanged(() => DisplayNameWaterMarkText);
                }
            }
        }
        #endregion

        #region DisplayNameWaterMarkColor
        private SolidColorBrush _DisplayNameWaterMarkColor;
        public SolidColorBrush DisplayNameWaterMarkColor
        {
            get
            {
                return _DisplayNameWaterMarkColor;
            }
            set
            {
                if (value != null)
                {
                    _DisplayNameWaterMarkColor = value;
                    RaisePropertyChanged(() => DisplayNameWaterMarkColor);
                }
            }
        }
        #endregion

        #region StatisticsNameWaterMarkText
        private string _StatisticsNameWaterMarkText;
        public string StatisticsNameWaterMarkText
        {
            get
            {
                return _StatisticsNameWaterMarkText;
            }
            set
            {
                if (value != null)
                {
                    _StatisticsNameWaterMarkText = value;
                    RaisePropertyChanged(() => StatisticsNameWaterMarkText);
                }
            }
        }
        #endregion

        #region StatisticsNameWaterMarkColor
        private SolidColorBrush _StatisticsNameWaterMarkColor;
        public SolidColorBrush StatisticsNameWaterMarkColor
        {
            get
            {
                return _StatisticsNameWaterMarkColor;
            }
            set
            {
                if (value != null)
                {
                    _StatisticsNameWaterMarkColor = value;
                    RaisePropertyChanged(() => StatisticsNameWaterMarkColor);
                }
            }
        }
        #endregion

        #region TooltipWaterMarkText
        private string _TooltipWaterMarkText;
        public string TooltipWaterMarkText
        {
            get
            {
                return _TooltipWaterMarkText;
            }
            set
            {
                if (value != null)
                {
                    _TooltipWaterMarkText = value;
                    RaisePropertyChanged(() => TooltipWaterMarkText);
                }
            }
        }
        #endregion

        #region TooltipWaterMarkColor
        private SolidColorBrush _TooltipWaterMarkColor;
        public SolidColorBrush TooltipWaterMarkColor
        {
            get
            {
                return _TooltipWaterMarkColor;
            }
            set
            {
                if (value != null)
                {
                    _TooltipWaterMarkColor = value;
                    RaisePropertyChanged(() => TooltipWaterMarkColor);
                }
            }
        }
        #endregion

        #region Threshold1WaterMarkText
        private string _Threshold1WaterMarkText;
        public string Threshold1WaterMarkText
        {
            get
            {
                return _Threshold1WaterMarkText;
            }
            set
            {
                if (value != null)
                {
                    _Threshold1WaterMarkText = value;
                    RaisePropertyChanged(() => Threshold1WaterMarkText);
                }
            }
        }
        #endregion

        #region Threshold1WaterMarkColor
        private SolidColorBrush _Threshold1WaterMarkColor;
        public SolidColorBrush Threshold1WaterMarkColor
        {
            get
            {
                return _Threshold1WaterMarkColor;
            }
            set
            {
                if (value != null)
                {
                    _Threshold1WaterMarkColor = value;
                    RaisePropertyChanged(() => Threshold1WaterMarkColor);
                }
            }
        }
        #endregion

        #region Threshold2WaterMarkText
        private string _Threshold2WaterMarkText;
        public string Threshold2WaterMarkText
        {
            get
            {
                return _Threshold2WaterMarkText;
            }
            set
            {
                if (value != null)
                {
                    _Threshold2WaterMarkText = value;
                    RaisePropertyChanged(() => Threshold2WaterMarkText);
                }
            }
        }
        #endregion

        #region Threshold2WaterMarkColor
        private SolidColorBrush _Threshold2WaterMarkColor;
        public SolidColorBrush Threshold2WaterMarkColor
        {
            get
            {
                return _Threshold2WaterMarkColor;
            }
            set
            {
                if (value != null)
                {
                    _Threshold2WaterMarkColor = value;
                    RaisePropertyChanged(() => Threshold2WaterMarkColor);
                }
            }
        }
        #endregion

        //#region MediaTypeWaterMarkText
        //private string _MediaTypeWaterMarkText;
        //public string MediaTypeWaterMarkText
        //{
        //    get
        //    {
        //        return _MediaTypeWaterMarkText;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            _MediaTypeWaterMarkText = value;
        //            RaisePropertyChanged(() => MediaTypeWaterMarkText);
        //        }
        //    }
        //}
        //#endregion

        //#region MediaTypeWaterMarkColor
        //private SolidColorBrush _MediaTypeWaterMarkColor;
        //public SolidColorBrush MediaTypeWaterMarkColor
        //{
        //    get
        //    {
        //        return _MediaTypeWaterMarkColor;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            _MediaTypeWaterMarkColor = value;
        //            RaisePropertyChanged(() => MediaTypeWaterMarkColor);
        //        }
        //    }
        //}
        //#endregion

        #region SearchValue
        private string _searchValue;
        public string SearchValue
        {
            get
            {
                return _searchValue;
            }
            set
            {
                _searchValue = value;
                RaisePropertyChanged(() => SearchValue);
            }
        }
        #endregion

        #endregion

        #region Commands

        public ICommand ActivatedCommand { get { return new DelegateCommand(WinActivated); } }
        public ICommand DeactivateCommand { get { return new DelegateCommand(WinDeActivated); } }
        public ICommand WinLoadCommand { get { return new DelegateCommand(WinLoad); } }
        public ICommand DragCmd { get { return new DelegateCommand(DragMove); } }
        public ICommand ConfigSkipCmd { get { return new DelegateCommand(ConfigurationSkip); } }

        public ICommand ApplicationClick { get { return new DelegateCommand(ApplicationStatisticsConfig); } }
        public ICommand EditExistingStatistics { get { return new DelegateCommand(ExsitStatisticsEditbtnClicked); } }
        public ICommand ConfiguredStatSelected { get { return new DelegateCommand(ExistCheckBoxChecked); } }
        public ICommand SaveStatistics { get { return new DelegateCommand(StatisticsSaved); } }

        public ICommand PropertySave { get { return new DelegateCommand(SaveServerEditbtnClicked); } }
        public ICommand PropertyClose { get { return new DelegateCommand(SaveServerCancelbtnClicked); } }
        public ICommand StatisticsSearch { get { return new DelegateCommand(SearchStatisticsbtnClicked); } }
        public ICommand DataGridLostFocusCommand { get { return new DelegateCommand(DataGridLostFocus); } }
        public ICommand DataGridRadiobtnCheckedCommand { get { return new DelegateCommand(DataGridRadiobtnChecked); } }
        public ICommand FormatTypeChanged { get { return new DelegateCommand(FormatTypeSelectionChanged); } }
        //public ICommand MediaTypeChangedCommand { get { return new DelegateCommand(MediaTypeChanged); } }
        public ICommand IsSavebtnEnabledCommand { get { return new DelegateCommand(SavebtnMouseOver); } }
        public ICommand TextChangedCommand { get { return new DelegateCommand(TextChangedEvent); } }
        public ICommand SectionChanged { get { return new DelegateCommand(SectionChangedEvent); } }
        public ICommand CloseObjectvaluesCommand { get { return new DelegateCommand(StatPropertiesCancel); } }
        public ICommand TextBoxLostFocusCommand { get { return new DelegateCommand(TextBoxLostFocus); } }
        public ICommand FilterTypeChangedCommand { get { return new DelegateCommand(FilterTypeChanged); } }
        public ICommand SortMediaTypeChangedCommand { get { return new DelegateCommand(SortMeidaTypeChanged); } }
        public ICommand ActivatedCheckboxCommand { get { return new DelegateCommand(ServerCheckBoxChecked); } }


        #endregion

        #region Methods

        #region WinActivated
        /// <summary>
        /// Wins the activated.
        /// </summary>
        private void WinActivated()
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : WinActivated() Method - Entry");
                ShadowEffect = new DropShadowBitmapEffect();
                ShadowEffect.ShadowDepth = 0;
                ShadowEffect.Opacity = 0.5;
                ShadowEffect.Softness = 0.5;

            }
            catch (Exception ex)
            {
                logger.Error("AdminConfigWindowViewModel : WinActivated() - Exception caught" + ex.Message.ToString());
            }
            finally
            {
                GC.Collect();
            }
            logger.Debug("AdminConfigWindowViewModel : WinActivated() Method - Exit");
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
                logger.Debug("AdminConfigWindowViewModel : WinDeActivated() Method - Entry");
                ShadowEffect.Opacity = 0;

            }
            catch (Exception generalException)
            {
                logger.Error("AdminConfigWindowViewModel : WinDeActivated() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : WinDeActivated() Method - Exit");
        }
        #endregion

        #region DragMove
        /// <summary>
        /// Drags the move.
        /// </summary>
        private void DragMove(object obj)
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : DragMove() Method - Entry");
                foreach (Window currentwindow in System.Windows.Application.Current.Windows)
                {
                    if (currentwindow.Title == obj.ToString())
                        currentwindow.DragMove();
                }
            }
            catch (Exception generalException)
            {
                logger.Error("AdminConfigWindowViewModel : DragMove() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : DragMove() Method - Exit");
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
                logger.Debug("AdminConfigWindowViewModel : WinLoad() Method - Entry");
                ApplicationName = Settings.GetInstance().ApplicationName;
                WinActivated();                

                //ApplicationStatisticsConfig();
                //SortMeidaTypeChanged(SortMediaTypeSource[0]);

            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigWindowViewModel : WinLoad() - Exception caught" + GeneralException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : WinLoad() Method - Exit");
        }
        #endregion

        #region ConfigurationSkip
        private void ConfigurationSkip()
        {
            try
            {

                logger.Debug("AdminConfigWindowViewModel : ConfigurationSkip() Method - Entry");

                Settings.GetInstance().IsApplicationObjectConfigWindow = true;

                if (Settings.GetInstance().QueueConfigVM == null)
                {
                    ObjectConfigWindow objView = new ObjectConfigWindow();
                    Settings.GetInstance().QueueConfigVM = new ObjectConfigWindowViewModel();
                    Settings.GetInstance().QueueConfigVM.LoadObjectConfiguration();
                    objView.DataContext = Settings.GetInstance().QueueConfigVM;
                    foreach (Window win in Application.Current.Windows)
                    {
                        if (win.Title == "AdminConfigurations")
                        {
                            win.Hide();
                        }
                    }
                    objView.Show();
                }
                else
                {
                    foreach (Window win in Application.Current.Windows)
                    {
                        if (win.Title == "AdminConfigurations")
                        {
                            win.Hide();
                        }
                    }
                    foreach (Window win in Application.Current.Windows)
                    {
                        if (win.Title == "ObjectConfigurations")
                        {
                            win.Show();
                        }
                    }
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigWindowViewModel : ConfigurationSkip() - Exception caught" + GeneralException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : ConfigurationSkip() Method - Exit");
        }
        #endregion

        #region ApplicationStatisticsConfig
        private void ApplicationStatisticsConfig()
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : ApplicationStatisticsConfig() Method - Entry");
                ExistingColor = new SolidColorBrush();
                ServerColor = new SolidColorBrush();
                ExistingColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(objStatTicker.GetExistingStatColor()));
                ServerColor = new SolidColorBrush((Color)ColorConverter.ConvertFromString(objStatTicker.GetServerStatColor()));
                LoadCollectionData();
                LoadServerCollectionData();

            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigWindowViewModel : ApplicationStatisticsConfig() Method - Exception caught" + GeneralException.InnerException.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : ApplicationStatisticsConfig() Method - Exit");
        }
        #endregion

        #region StatisticsSaved
        public void StatisticsSaved()
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : StatisticsSaved() Method - Entry");
                if (Settings.GetInstance().DictConfigStats != null)
                {

                    //if (Settings.GetInstance().DictExistingApplicationStats != null)
                    //{
                    //    foreach (string key in Settings.GetInstance().DictExistingApplicationStats.Keys)
                    //    {
                    //        if (!Settings.GetInstance().DictConfigStats.ContainsKey(key))
                    //        {
                    //            Settings.GetInstance().DictConfigStats.Add(key, Settings.GetInstance().DictExistingApplicationStats[key]);
                    //        }
                    //    }

                    Settings.GetInstance().IsPropertyExist = objStatTicker.CheckExistingProperties("statistics");

                    if (Settings.GetInstance().IsPropertyExist)
                    {
                        Settings.GetInstance().IsCancelSaving = false;
                        Views.MessageBox msgbox;
                        ViewModels.MessageBoxViewModel mboxvmodel;
                        msgbox = new Views.MessageBox();
                        mboxvmodel = new MessageBoxViewModel("Information", "Values existing already. Do you want to overwrite (or) Append. ", msgbox, "AdminConfig", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxvmodel;
                        msgbox.ShowDialog();
                    }

                    if (!Settings.GetInstance().IsCancelSaving)
                    {
                        objStatTicker.SaveApplicationValues(Settings.GetInstance().DictConfigStats, Settings.GetInstance().ToOverWrite);
                        if (Settings.GetInstance().QueueConfigVM == null)
                        {

                            foreach (Window win in Application.Current.Windows)
                            {
                                if (win.Title == "AdminConfigurations")
                                {
                                    win.Hide();
                                }
                            }

                        }
                        else
                        {
                            foreach (Window win in Application.Current.Windows)
                            {
                                if (win.Title == "AdminConfigurations")
                                {
                                    win.Hide();
                                }
                            }
                            foreach (Window win in Application.Current.Windows)
                            {
                                if (win.Title == "ObjectConfigurations")
                                {
                                    win.Close();
                                }
                            }

                        }
                        ObjectConfigWindow objView = new ObjectConfigWindow();
                        Settings.GetInstance().QueueConfigVM = new ObjectConfigWindowViewModel();
                        Settings.GetInstance().QueueConfigVM.LoadObjectConfiguration();
                        objView.DataContext = Settings.GetInstance().QueueConfigVM;
                        objView.Show();
                        LoadServerCollectionData();
                        //ObjectConfigWindow objView = new ObjectConfigWindow();
                        //if (Settings.GetInstance().ObjectConfigVM == null)
                        //{
                        //    Settings.GetInstance().ObjectConfigVM = new ObjectConfigWindowViewModel();
                        //}
                        //Settings.GetInstance().ObjectConfigVM.LoadObjectConfiguration();                        
                        //objView.DataContext = Settings.GetInstance().ObjectConfigVM;

                        //foreach (Window win in Application.Current.Windows)
                        //{
                        //    if (win.Title == "AdminConfigurations")
                        //    {
                        //        win.Hide();
                        //    }
                        //}
                        //Settings.GetInstance().PreviousWindow = "AdminConfigurations";
                        //objView.ShowDialog();
                    }
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigWindowViewModel : StatisticsSaved() - Exception caught" + GeneralException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : StatisticsSaved() Method - Exit");
        }
        #endregion

        #region LoadCollectionData

        private void LoadCollectionData()
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : LoadCollectionData() Method - Entry");
                Dictionary<string, Dictionary<string, string>> DictTempExistingApplicationStats = null;
                DictTempExistingApplicationStats = objStatTicker.GetApplicationAnnex(Settings.GetInstance().ApplicationName);

                Settings.GetInstance().DictExistingApplicationStats.Clear();
                ConfiguredStatistics.Clear();
                Settings.GetInstance().DictConfiguredAgentStats.Clear();
                Settings.GetInstance().DictConfiguredAGroupStats.Clear();
                Settings.GetInstance().DictConfiguredACDStats.Clear();
                Settings.GetInstance().DictConfiguredDNStats.Clear();
                Settings.GetInstance().DictConfiguredVQStats.Clear();

                foreach (KeyValuePair<string, Dictionary<string, string>> item in DictTempExistingApplicationStats.OrderBy(a => a.Key))
                {
                    Settings.GetInstance().DictExistingApplicationStats.Add(item.Key, item.Value);
                }

                foreach (string SectionName in Settings.GetInstance().DictExistingApplicationStats.Keys)
                {
                    Dictionary<string, string> statDetails = new Dictionary<string, string>();

                    statDetails = Settings.GetInstance().DictExistingApplicationStats[SectionName];
                    string DisplayName = string.Empty;
                    string MediaType = string.Empty;
                    string TooltipName = string.Empty;
                    List<string> filterList = new List<string>();
                    filterList.Add(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]);
                    DisplayName = statDetails[StatisticsEnum.StatProperties.DisplayName.ToString()].ToString();
                    TooltipName = statDetails[StatisticsEnum.StatProperties.TooltipName.ToString()].ToString();

                    //if (SectionName.StartsWith("agent"))
                    //{

                    //    if (!Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()]))
                    //    {
                    //        Settings.GetInstance().DictConfiguredAgentStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()], filterList);
                    //    }
                    //    else
                    //    {
                    //        bool flag = true;
                    //        foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredAgentStats)
                    //        {
                    //            if ((string.Compare(item.Key, statDetails[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (item.Value.Contains(statDetails[StatisticsEnum.StatProperties.Filter.ToString()])))
                    //            {
                    //                flag = false;
                    //            }
                    //        }
                    //        if (flag)
                    //        {
                    //            Settings.GetInstance().DictConfiguredAgentStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()], filterList);
                    //        }
                    //    }
                    //}

                    //if (SectionName.StartsWith("group"))
                    //{
                    //    if (!Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()]))
                    //    {
                    //        Settings.GetInstance().DictConfiguredAGroupStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()], filterList);
                    //    }
                    //    else
                    //    {
                    //        bool flag = true;
                    //        foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredAGroupStats)
                    //        {
                    //            if ((string.Compare(item.Key, statDetails[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (item.Value.Contains(statDetails[StatisticsEnum.StatProperties.Filter.ToString()])))
                    //            {
                    //                flag = false;
                    //            }
                    //        }
                    //        if (flag)
                    //        {
                    //            Settings.GetInstance().DictConfiguredAGroupStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()], filterList);
                    //        }
                    //    }


                    //}

                    //if (SectionName.StartsWith("acd"))
                    //{
                    //    if (!Settings.GetInstance().DictConfiguredACDStats.ContainsKey(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()]))
                    //    {
                    //        Settings.GetInstance().DictConfiguredACDStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()], filterList);
                    //    }
                    //    else
                    //    {
                    //        bool flag = true;
                    //        foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredACDStats)
                    //        {
                    //            if ((string.Compare(item.Key, statDetails[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (item.Value.Contains(statDetails[StatisticsEnum.StatProperties.Filter.ToString()])))
                    //            {
                    //                flag = false;
                    //            }
                    //        }
                    //        if (flag)
                    //        {
                    //            Settings.GetInstance().DictConfiguredACDStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()], filterList);
                    //        }
                    //    }


                    //}

                    //if (SectionName.StartsWith("dn"))
                    //{
                    //    if (!Settings.GetInstance().DictConfiguredDNStats.ContainsKey(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()]))
                    //    {
                    //        Settings.GetInstance().DictConfiguredDNStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()], filterList);
                    //    }
                    //    else
                    //    {
                    //        bool flag = true;
                    //        foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredDNStats)
                    //        {
                    //            if ((string.Compare(item.Key, statDetails[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (item.Value.Contains(statDetails[StatisticsEnum.StatProperties.Filter.ToString()])))
                    //            {
                    //                flag = false;
                    //            }
                    //        }
                    //        if (flag)
                    //        {
                    //            Settings.GetInstance().DictConfiguredDNStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()], filterList);
                    //        }
                    //    }


                    //}

                    //if (SectionName.StartsWith("vq"))
                    //{
                    //    if (!Settings.GetInstance().DictConfiguredVQStats.ContainsKey(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()]))
                    //    {
                    //        Settings.GetInstance().DictConfiguredVQStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()], filterList);
                    //    }
                    //    else
                    //    {
                    //        bool flag = true;
                    //        foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredVQStats)
                    //        {
                    //            if ((string.Compare(item.Key, statDetails[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (item.Value.Contains(statDetails[StatisticsEnum.StatProperties.Filter.ToString()])))
                    //            {
                    //                flag = false;

                    //            }
                    //        }
                    //        if (flag)
                    //        {
                    //            Settings.GetInstance().DictConfiguredVQStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()] + statDetails[StatisticsEnum.StatProperties.Filter.ToString()], filterList);
                    //        }
                    //    }

                    //}

                    if (SectionName.StartsWith("agent"))
                    {

                        if (!Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(statDetails[StatisticsEnum.StatProperties.StatName.ToString()]))
                        {
                            Settings.GetInstance().DictConfiguredAgentStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()], filterList);
                        }
                        else
                        {
                            if (!Settings.GetInstance().DictConfiguredAgentStats[statDetails[StatisticsEnum.StatProperties.StatName.ToString()]].Contains(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]))
                            {
                                Settings.GetInstance().DictConfiguredAgentStats[statDetails[StatisticsEnum.StatProperties.StatName.ToString()]].Add(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]);
                            }
                        }
                    }

                    if (SectionName.StartsWith("group"))
                    {
                        if (!Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(statDetails[StatisticsEnum.StatProperties.StatName.ToString()]))
                        {
                            Settings.GetInstance().DictConfiguredAGroupStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()], filterList);
                        }
                        else
                        {
                            if (!Settings.GetInstance().DictConfiguredAGroupStats[statDetails[StatisticsEnum.StatProperties.StatName.ToString()]].Contains(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]))
                            {
                                Settings.GetInstance().DictConfiguredAGroupStats[statDetails[StatisticsEnum.StatProperties.StatName.ToString()]].Add(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]);
                            }
                        }

                    }

                    if (SectionName.StartsWith("acd"))
                    {
                        if (!Settings.GetInstance().DictConfiguredACDStats.ContainsKey(statDetails[StatisticsEnum.StatProperties.StatName.ToString()]))
                        {
                            Settings.GetInstance().DictConfiguredACDStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()], filterList);
                        }
                        else
                        {
                            if (!Settings.GetInstance().DictConfiguredACDStats[statDetails[StatisticsEnum.StatProperties.StatName.ToString()]].Contains(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]))
                            {
                                Settings.GetInstance().DictConfiguredACDStats[statDetails[StatisticsEnum.StatProperties.StatName.ToString()]].Add(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]);
                            }
                        }

                    }

                    if (SectionName.StartsWith("dn"))
                    {
                        if (!Settings.GetInstance().DictConfiguredDNStats.ContainsKey(statDetails[StatisticsEnum.StatProperties.StatName.ToString()]))
                        {
                            Settings.GetInstance().DictConfiguredDNStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()], filterList);
                        }
                        else
                        {
                            if (!Settings.GetInstance().DictConfiguredDNStats[statDetails[StatisticsEnum.StatProperties.StatName.ToString()]].Contains(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]))
                            {
                                Settings.GetInstance().DictConfiguredDNStats[statDetails[StatisticsEnum.StatProperties.StatName.ToString()]].Add(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]);
                            }
                        }

                    }

                    if (SectionName.StartsWith("vq"))
                    {
                        if (!Settings.GetInstance().DictConfiguredVQStats.ContainsKey(statDetails[StatisticsEnum.StatProperties.StatName.ToString()]))
                        {
                            Settings.GetInstance().DictConfiguredVQStats.Add(statDetails[StatisticsEnum.StatProperties.StatName.ToString()], filterList);
                        }
                        else
                        {
                            if (!Settings.GetInstance().DictConfiguredVQStats[statDetails[StatisticsEnum.StatProperties.StatName.ToString()]].Contains(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]))
                            {
                                Settings.GetInstance().DictConfiguredVQStats[statDetails[StatisticsEnum.StatProperties.StatName.ToString()]].Add(statDetails[StatisticsEnum.StatProperties.Filter.ToString()]);
                            }
                        }


                    }

                    //foreach (string key in statDetails.Keys)
                    //{
                    //    if (string.Compare(key, StatisticsEnum.StatProperties.DisplayName.ToString(), true) == 0)
                    //    {
                    //        DisplayName = statDetails[key].ToString();
                    //    }




                    //}

                    ConfiguredStatistics.Add(new StatisticsProperties()
                    {
                        isGridChecked = true,

                        SectionName = SectionName.Length > 35 ? SectionName.Substring(0, 32) + ".." : SectionName,

                        SectionTooltip = SectionName.Length > 35 ? SectionName : TooltipName,


                        DisplayName = DisplayName.Length > 35 ? DisplayName.Substring(0, 33) + ".." : DisplayName,

                        DisplayTooltip = DisplayName.Length > 35 ? DisplayName : TooltipName,

                        EditName = "Edit",

                        IsExistProperty = true,

                    });

                    if (!Settings.GetInstance().DictConfigStats.ContainsKey(SectionName))
                        Settings.GetInstance().DictConfigStats.Add(SectionName, statDetails);
                    //filterList.Clear();
                }

            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigWindowViewModel : LoadCollectionData method() - Exception caught" + GeneralException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : LoadCollectionData() Method - Exit");
        }

        #endregion

        #region LoadServerCollectionData
        private void LoadServerCollectionData()
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : LoadServerCollectionData() Method - Entry");
                //Settings.GetInstance().DictServerStatistics = objStatTicker.GetServerStats();
                Dictionary<string, KeyValueCollection> DictTempServerStatistics = null;
                DictTempServerStatistics = objStatTicker.GetServerStats();

                Settings.GetInstance().DictServerStatistics.Clear();
                ServerStatistics.Clear();
                foreach (KeyValuePair<string, KeyValueCollection> item in DictTempServerStatistics.OrderBy(a => a.Key))
                {
                    Settings.GetInstance().DictServerStatistics.Add(item.Key, item.Value);
                }

                objStatSupport.FilterStatistics();

                foreach (string statname in Settings.GetInstance().DictServerStatistics.Keys)
                {
                    KeyValueCollection tempCollection = (KeyValueCollection)Settings.GetInstance().DictServerStatistics[statname];

                    if (tempCollection.ContainsKey("Description"))
                    {
                        ServerStatistics.Add(new StatisticsProperties() { isGridChecked = false, SectionName = statname.Length > 35 ? statname.Substring(0, 32) + ".." : statname, SectionTooltip = statname, DisplayTooltip = tempCollection["Description"].ToString(), DisplayName = tempCollection["Description"].ToString().Length > 35 ? tempCollection["Description"].ToString().Substring(0, 33) + ".." : tempCollection["Description"].ToString(), EditName = "Edit", IsExistProperty = false, });
                    }
                    else
                    {
                        ServerStatistics.Add(new StatisticsProperties() { isGridChecked = false, SectionName = statname.Length > 35 ? statname.Substring(0, 32) + ".." : statname, SectionTooltip = statname, DisplayTooltip = statname, DisplayName = statname.Length > 35 ? statname.Substring(0, 33) + ".." : statname, EditName = "Edit", IsExistProperty = false, });
                    }

                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigWindowViewModel : LoadServerCollectionData() Method - Exception caught" + GeneralException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : LoadServerCollectionData() Method - Exit");
        }
        #endregion

        #region ExsitStatisticsEditbtnClicked

        private void ExsitStatisticsEditbtnClicked(object obj)
        {
            try
            {

                logger.Debug("AdminConfigWindowViewModel : ExsitStatisticsEditbtnClicked() Method - Entry");
                isObjectStatAlreadyConfigured = false;
                isThresholdFirstTime = false;
                string statName = string.Empty;
                stat = obj as StatisticsProperties;

                Dictionary<string, string> Properties = new Dictionary<string, string>();

                if (Settings.GetInstance().DictExistingApplicationStats.ContainsKey(stat.SectionName))
                {
                    Properties = Settings.GetInstance().DictExistingApplicationStats[stat.SectionName];
                    statName = Properties[StatisticsEnum.StatProperties.StatName.ToString()];
                    Statistics = Properties[StatisticsEnum.StatProperties.StatName.ToString()];
                    ExistingSection = stat.SectionName;
                    ExistFilter = Properties[StatisticsEnum.StatProperties.Filter.ToString()];
                    if (stat.SectionName.StartsWith("agent"))
                    {
                        ExistSectionType = StatisticsEnum.SectionName.agent.ToString();
                    }
                    else if (stat.SectionName.StartsWith("group"))
                    {
                        ExistSectionType = StatisticsEnum.SectionName.group.ToString();
                    }
                    else if (stat.SectionName.StartsWith("acd"))
                    {
                        ExistSectionType = StatisticsEnum.SectionName.acd.ToString();
                    }
                    else if (stat.SectionName.StartsWith("vq"))
                    {
                        ExistSectionType = StatisticsEnum.SectionName.vq.ToString();
                    }
                    else if (stat.SectionName.StartsWith("dn"))
                    {
                        ExistSectionType = StatisticsEnum.SectionName.dn.ToString();
                    }
                }
                else
                {
                    statName = stat.SectionName;
                    ExistingSection = stat.SectionName;
                    ExistSectionType = Sections[0].ToString();
                }


                //if(!stat.IsExistProperty)
                //Settings.GetInstance().IsExistingStat = !stat.IsExistProperty;
                //else
                Settings.GetInstance().IsExistingStat = stat.IsExistProperty;

                StatSectionName = string.Empty;
                PropDisplayName = string.Empty;
                //PropMediaType = string.Empty;
                StatisticsName = string.Empty;
                TooltipValue = string.Empty;
                Threshold1 = string.Empty;
                Threshold2 = string.Empty;
                if (Settings.GetInstance().DictExistingApplicationStats.ContainsKey(stat.SectionName))
                {
                    //Dictionary<string, string> statProperties = new Dictionary<string, string>();
                    Properties = Settings.GetInstance().DictExistingApplicationStats[stat.SectionName];

                    StatSectionName = stat.SectionName;
                    PropDisplayName = Properties[StatisticsEnum.StatProperties.DisplayName.ToString()];
                    //PropMediaType = Properties[StatisticsEnum.StatProperties.MediaType.ToString()];
                    StatisticsName = Properties[StatisticsEnum.StatProperties.StatName.ToString()];
                    TooltipValue = Properties[StatisticsEnum.StatProperties.TooltipName.ToString()];
                    Threshold1 = Properties[StatisticsEnum.StatProperties.ThresLevel1.ToString()];
                    Threshold2 = Properties[StatisticsEnum.StatProperties.ThresLevel2.ToString()];
                }
                else
                {
                    StatSectionName = stat.SectionName;
                    StatisticsName = stat.SectionName;
                }

                // Settings.GetInstance().editVMObj.SearchStatnameCollection.Clear();

                GridWidth = new GridLength(400);
                TitleSpan = 2;
                DataGridRowHeight = new GridLength(0);


                IsSavebtnEnable = true;
                StatNameColSpan = 2;
                StatNameSearch = Visibility.Visible;
                Title = "Statistics Properties";

                FilterSource.Clear();
                FilterSource.Add("None");
                foreach (string Filter in Settings.GetInstance().DictServerFilters.Keys)
                {
                    if (!FilterSource.Contains(Filter))
                        FilterSource.Add(Filter);
                }

                MediaTypeSource.Clear();

                FormatSource.Clear();
                foreach (string Format in Settings.GetInstance().DictStatFormats.Keys)
                {
                    if (!FormatSource.Contains(Format))
                        FormatSource.Add(Format + " " + Settings.GetInstance().DictStatFormats[Format]);
                }

                if (Settings.GetInstance().DictExistingApplicationStats.ContainsKey(StatSectionName))
                {
                    Dictionary<string, string> statProperties = new Dictionary<string, string>();
                    statProperties = Settings.GetInstance().DictExistingApplicationStats[StatSectionName];
                    SelectedFilter = statProperties[StatisticsEnum.StatProperties.Filter.ToString()];
                    SelectedFormat = statProperties[StatisticsEnum.StatProperties.Format.ToString()] + " " + Settings.GetInstance().DictStatFormats[statProperties[StatisticsEnum.StatProperties.Format.ToString()]];
                    StatisticsColor = (System.Windows.Media.Color)ColorConverter.ConvertFromString(statProperties[StatisticsEnum.StatProperties.Color1.ToString()]);
                    ThresholdColor1 = (System.Windows.Media.Color)ColorConverter.ConvertFromString(statProperties[StatisticsEnum.StatProperties.Color2.ToString()]);
                    ThresholdColor2 = (System.Windows.Media.Color)ColorConverter.ConvertFromString(statProperties[StatisticsEnum.StatProperties.Color3.ToString()]);
                }
                else
                {
                    SelectedFilter = FilterSource[0];
                    SelectedMediaType = MediaTypeSource[0];
                    SelectedFormat = FormatSource[0];
                    StatisticsColor = (System.Windows.Media.Color)ColorConverter.ConvertFromString("Black");
                    ThresholdColor1 = (System.Windows.Media.Color)ColorConverter.ConvertFromString("Green");
                    ThresholdColor2 = (System.Windows.Media.Color)ColorConverter.ConvertFromString("Red");
                }

                if (!string.IsNullOrEmpty(StatSectionName))
                {
                    string[] SectionTitle = StatSectionName.Split('-');
                    if (Sections.Contains(SectionTitle[0].ToString().ToLower()))
                    {
                        SelectedSection = SectionTitle[0].ToString().ToLower();
                        StatSectionName = StatSectionName.Replace(SectionTitle[0].ToString().ToLower() + "-", "");
                    }
                    isObjectStatAlreadyConfigured = false;
                    SectionChangedEvent(SelectedSection);
                }
                else
                {
                    switch (objStatSupport.GetSupportedObject(StatSectionName))
                    {
                        case "Agent":
                            SelectedSection = StatisticsEnum.SectionName.agent.ToString();
                            SectionChangedEvent(StatisticsEnum.SectionName.agent.ToString());
                            break;
                        case "AgentGroup":
                            SelectedSection = StatisticsEnum.SectionName.group.ToString();
                            SectionChangedEvent(StatisticsEnum.SectionName.group.ToString());
                            break;
                        case "Queue":
                            SelectedSection = StatisticsEnum.SectionName.acd.ToString();
                            SectionChangedEvent(StatisticsEnum.SectionName.acd.ToString());
                            break;
                        case "GroupQueus":
                            SelectedSection = StatisticsEnum.SectionName.dn.ToString();
                            SectionChangedEvent(StatisticsEnum.SectionName.dn.ToString());
                            break;
                        case "RoutePoint":
                            SelectedSection = StatisticsEnum.SectionName.vq.ToString();
                            SectionChangedEvent(StatisticsEnum.SectionName.vq.ToString());
                            break;
                    }

                }

                if (SelectedFilter == string.Empty || SelectedFilter == "")
                    SelectedFilter = "None";

                if (SelectedMediaType == string.Empty || SelectedMediaType == "")
                    SelectedMediaType = "None";

                if (string.IsNullOrEmpty(SelectedSection))
                {
                    SelectedSection = Sections[0];
                    SectionChangedEvent(Sections[0]);
                }
                if ((string.IsNullOrEmpty(SelectedFilter)) && (FilterSource.Count > 0))
                {
                    SelectedFilter = FilterSource[0];
                }
                else if ((!FilterSource.Contains(SelectedFilter)) && (FilterSource.Count > 0))
                {
                    SelectedFilter = FilterSource[0];
                }
                if ((string.IsNullOrEmpty(SelectedMediaType)) && (MediaTypeSource.Count > 0))
                {
                    SelectedMediaType = MediaTypeSource[0];
                }
                else if ((!MediaTypeSource.Contains(SelectedMediaType)) && (MediaTypeSource.Count > 0))
                {
                    SelectedMediaType = MediaTypeSource[0];
                }
                if (string.IsNullOrEmpty(SelectedFormat))
                {
                    isThresholdFirstTime = false;
                    SelectedFormat = FormatSource[0];
                    Settings.GetInstance().SelectedFormat = SelectedFormat;
                }
                else
                {
                    Settings.GetInstance().SelectedFormat = SelectedFormat;
                }

                if (string.IsNullOrEmpty(StatSectionName))
                {
                    SectionNameWaterMarkText = EmptyField;
                    SectionNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                }
                else
                {
                    SectionNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                }

                if (string.IsNullOrEmpty(PropDisplayName))
                {
                    DisplayNameWaterMarkText = EmptyField;
                    DisplayNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                }
                else
                {
                    DisplayNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                }
                //if (string.IsNullOrEmpty(PropMediaType))
                //{
                //    MediaTypeWaterMarkText = EmptyField;
                //    MediaTypeWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                //}
                //else
                //{
                //    MediaTypeWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                //}
                if (string.IsNullOrEmpty(StatisticsName))
                {
                    StatisticsNameWaterMarkText = EmptyField;
                    StatisticsNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));

                }
                else
                {
                    StatisticsNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                }
                if (string.IsNullOrEmpty(TooltipValue))
                {
                    TooltipWaterMarkText = EmptyField;
                    TooltipWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                }
                else
                {
                    TooltipWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                }
                if (string.IsNullOrEmpty(Threshold1))
                {
                    Threshold1WaterMarkText = EmptyField;
                    Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));

                }
                else
                {
                    Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                }
                if (string.IsNullOrEmpty(Threshold2))
                {
                    Threshold2WaterMarkText = EmptyField;
                    Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                }
                else
                {
                    Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                }
                isThresholdFirstTime = true;

            }
            catch (Exception generalException)
            {
                string sd;
                sd = generalException.Message;
                logger.Error("AdminConfigWindowViewModel : ExsitStatisticsEditbtnClicked() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : ExsitStatisticsEditbtnClicked() Method - Exit");
        }

        #endregion

        #region ExistCheckBoxChecked

        public void ExistCheckBoxChecked(object obj)
        {
            StatisticsProperties stat = obj as StatisticsProperties;
            try
            {
                logger.Debug("AdminConfigWindowViewModel : ExistCheckBoxChecked() Method - Entry");
                for (int item = 0; item < ConfiguredStatistics.Count; item++)
                {
                    if (string.Compare(ConfiguredStatistics[item].SectionName.Trim(), stat.SectionName.Trim(), true) == 0)
                    {
                        //ConfiguredStatistics[item].IsExistProperty = false;

                        ServerStatistics.Add(ConfiguredStatistics[item]);

                        ObservableCollection<StatisticsProperties> TempServerStatistics = new ObservableCollection<StatisticsProperties>();
                        foreach (StatisticsProperties tempStat in ServerStatistics)
                        {
                            TempServerStatistics.Add(tempStat);
                        }
                        ServerStatistics.Clear();
                        foreach (StatisticsProperties tempStat in TempServerStatistics.OrderBy(a => a.SectionName))
                        {
                            ServerStatistics.Add(tempStat);
                        }

                        Dictionary<string, string> statProperties = new Dictionary<string, string>();
                        statProperties = Settings.GetInstance().DictExistingApplicationStats[stat.SectionName];
                        string statName = statProperties[StatisticsEnum.StatProperties.StatName.ToString()];

                        //objStatFilter.FilterStatistics();
                        ConfiguredStatistics.RemoveAt(item);

                        if (Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(statName))
                        {
                            foreach (KeyValuePair<string, List<string>> AgentItem in Settings.GetInstance().DictConfiguredAgentStats)
                            {
                                if ((string.Compare(AgentItem.Key, statProperties[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (AgentItem.Value.Contains(statProperties[StatisticsEnum.StatProperties.Filter.ToString()])))
                                {
                                    AgentItem.Value.Remove(statProperties[StatisticsEnum.StatProperties.Filter.ToString()]);
                                    Settings.GetInstance().DictConfiguredAgentStats[AgentItem.Key] = AgentItem.Value;
                                    break;
                                }
                            }

                        }
                        else if (Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(statName))
                        {
                            foreach (KeyValuePair<string, List<string>> AgentGroupItem in Settings.GetInstance().DictConfiguredAGroupStats)
                            {
                                if ((string.Compare(AgentGroupItem.Key, statProperties[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (AgentGroupItem.Value.Contains(statProperties[StatisticsEnum.StatProperties.Filter.ToString()])))
                                {
                                    AgentGroupItem.Value.Remove(statProperties[StatisticsEnum.StatProperties.Filter.ToString()]);
                                    Settings.GetInstance().DictConfiguredAGroupStats[AgentGroupItem.Key] = AgentGroupItem.Value;
                                    break;
                                }
                            }
                        }
                        else if (Settings.GetInstance().DictConfiguredACDStats.ContainsKey(statName))
                        {
                            foreach (KeyValuePair<string, List<string>> ACDItem in Settings.GetInstance().DictConfiguredACDStats)
                            {
                                if ((string.Compare(ACDItem.Key, statProperties[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (ACDItem.Value.Contains(statProperties[StatisticsEnum.StatProperties.Filter.ToString()])))
                                {
                                    ACDItem.Value.Remove(statProperties[StatisticsEnum.StatProperties.Filter.ToString()]);
                                    Settings.GetInstance().DictConfiguredACDStats[ACDItem.Key] = ACDItem.Value;
                                    break;
                                }
                            }
                        }
                        else if (Settings.GetInstance().DictConfiguredDNStats.ContainsKey(statName))
                        {
                            foreach (KeyValuePair<string, List<string>> DNItem in Settings.GetInstance().DictConfiguredDNStats)
                            {
                                if ((string.Compare(DNItem.Key, statProperties[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (DNItem.Value.Contains(statProperties[StatisticsEnum.StatProperties.Filter.ToString()])))
                                {
                                    DNItem.Value.Remove(statProperties[StatisticsEnum.StatProperties.Filter.ToString()]);
                                    Settings.GetInstance().DictConfiguredDNStats[DNItem.Key] = DNItem.Value;
                                    break;
                                }
                            }
                        }
                        else if (Settings.GetInstance().DictConfiguredVQStats.ContainsKey(statName))
                        {
                            foreach (KeyValuePair<string, List<string>> VQItem in Settings.GetInstance().DictConfiguredVQStats)
                            {
                                if ((string.Compare(VQItem.Key, statProperties[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (VQItem.Value.Contains(statProperties[StatisticsEnum.StatProperties.Filter.ToString()])))
                                {
                                    VQItem.Value.Remove(statProperties[StatisticsEnum.StatProperties.Filter.ToString()]);
                                    Settings.GetInstance().DictConfiguredVQStats[VQItem.Key] = VQItem.Value;
                                    break;
                                }
                            }
                        }

                        if (Settings.GetInstance().DictConfigStats.ContainsKey(stat.SectionName))
                            Settings.GetInstance().DictConfigStats.Remove(stat.SectionName);

                        //if (Settings.GetInstance().DictExistingApplicationStats.ContainsKey(stat.SectionName))
                        //{
                        //    Settings.GetInstance().DictExistingApplicationStats.Remove(stat.SectionName);
                        //}
                    }
                }
                logger.Debug("AdminConfigWindowViewModel : ExistCheckBoxChecked() Method - Exit");
            }
            catch (Exception GeneralException)
            {
                string sss = string.Empty;
                sss = GeneralException.Message;
                logger.Error("AdminConfigWindowViewModel : ExistCheckBoxChecked() Method - Exception caught" + GeneralException.Message.ToString());
            }
        }

        #endregion

        #region ServerCheckBoxChecked
        public void ServerCheckBoxChecked(object obj)
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : ServerCheckBoxChecked() Method - Entry");
                StatisticsProperties stat = obj as StatisticsProperties;
                if (!stat.isGridChecked)
                {
                    StatPropertiesCancel();
                }
                logger.Debug("AdminConfigWindowViewModel : ServerCheckBoxChecked() Method - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("AdminConfigWindowViewModel : ServerCheckBoxChecked() Method - Exception caught" + generalException.ToString());
            }
        }
        #endregion

        #region SaveServerCancelbtnClicked

        public void SaveServerCancelbtnClicked()
        {
            try
            {
                //if (Settings.GetInstance().isConfigNewStats)
                //{
                //    foreach (Window win in Application.Current.Windows)
                //    {
                //        if (win.Title == "StatisticsPropertiesWindow")
                //        {
                //            win.Close();
                //        }
                //    }

                //    AdminConfigWindow adminConfigView = new AdminConfigWindow();
                //    Settings.GetInstance().adminConfigVM = new AdminConfigWindowViewModel();
                //    adminConfigView.DataContext = Settings.GetInstance().adminConfigVM;
                //    adminConfigView.Show();
                //}
                //else
                //{
                //foreach (Window win in Application.Current.Windows)
                //{
                //    if (win.Title == "StatisticsPropertiesWindow")
                //    {
                //        win.Close();
                //    }
                //}
                //}
                logger.Debug("AdminConfigWindowViewModel : SaveServerCancelbtnClicked() Method - Entry");
                DataGridRowHeight = new GridLength(0);
                logger.Debug("AdminConfigWindowViewModel : SaveServerCancelbtnClicked() Method - Exit");
            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigWindowViewModel : SaveServerCancelbtnClicked() Method - Exception caught" + GeneralException.Message.ToString());
            }
        }

        public void StatPropertiesCancel()
        {
            logger.Debug("AdminConfigWindowViewModel : LoadCollectionData() Method - Entry");
            GridWidth = new GridLength(0);
            DataGridRowHeight = new GridLength(0);
            TitleSpan = 1;
            logger.Debug("AdminConfigWindowViewModel : LoadCollectionData() Method - Entry");
        }

        #endregion

        #region Section Type changed

        public void SectionChangedEvent(object obj)
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : SectionChangedEvent() Method - Entry");
                SectionType = obj.ToString();

                Settings.GetInstance().DictStatisticsDesc.Clear();
                SearchStatnameCollection = new ObservableCollection<SearchedStatistics>();

                if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.agent.ToString(), true) == 0)
                {
                    objStatSupport.GetDescriptions("Agent");
                }
                else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.group.ToString(), true) == 0)
                {
                    objStatSupport.GetDescriptions("GroupAgents");
                }
                else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.acd.ToString(), true) == 0)
                {
                    objStatSupport.GetDescriptions("Queue");
                }
                else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.dn.ToString(), true) == 0)
                {
                    objStatSupport.GetDescriptions("GroupQueues");
                }
                else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.vq.ToString(), true) == 0)
                {
                    objStatSupport.GetDescriptions("RoutePoint");
                }
                string Filter = null;
                if (SelectedFilter == "None" || SelectedFilter == string.Empty)
                {
                    Filter = "";
                }
                else
                {
                    Filter = SelectedFilter;
                }

                foreach (KeyValuePair<string, string> item in Settings.GetInstance().DictStatisticsDesc)
                {

                    string tempStatName = string.Empty;
                    string tempStatDesc = string.Empty;
                    tempStatName = item.Key.ToString().Length > 25 ? item.Key.ToString().Substring(0, 23) + ".." : item.Key.ToString();

                    tempStatDesc = item.Value.ToString().Length > 36 ? item.Value.ToString().Substring(0, 35) + ".." : item.Value.ToString();

                    if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.agent.ToString(), true) == 0)
                    {
                        if (!Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(item.Key))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                        else if (!Settings.GetInstance().DictConfiguredAgentStats[item.Key].Contains(Filter))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                    }
                    else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.group.ToString(), true) == 0)
                    {
                        if (!Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(item.Key))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                        else if (!Settings.GetInstance().DictConfiguredAGroupStats[item.Key].Contains(Filter))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                    }
                    else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.acd.ToString(), true) == 0)
                    {
                        if (!Settings.GetInstance().DictConfiguredACDStats.ContainsKey(item.Key))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                        else if (!Settings.GetInstance().DictConfiguredACDStats[item.Key].Contains(Filter))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                    }
                    else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.dn.ToString(), true) == 0)
                    {
                        if (!Settings.GetInstance().DictConfiguredDNStats.ContainsKey(item.Key))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                        else if (!Settings.GetInstance().DictConfiguredDNStats[item.Key].Contains(Filter))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                    }
                    else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.vq.ToString(), true) == 0)
                    {
                        if (!Settings.GetInstance().DictConfiguredVQStats.ContainsKey(item.Key))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                        else if (!Settings.GetInstance().DictConfiguredVQStats[item.Key].Contains(Filter))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                    }

                    //SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key });

                }

                if (isObjectStatAlreadyConfigured)
                {
                    bool temp = true;
                    foreach (SearchedStatistics stat in SearchStatnameCollection)
                    {
                        if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.agent.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(stat.SearchedStatname))
                            {
                                if (Settings.GetInstance().DictConfiguredAgentStats[stat.SearchedStatname].Contains(Filter))
                                {
                                    temp = false;
                                }
                            }
                        }
                        else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.group.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(stat.SearchedStatname))
                            {
                                if (Settings.GetInstance().DictConfiguredAGroupStats[stat.SearchedStatname].Contains(Filter))
                                {
                                    temp = false;
                                }
                            }
                        }
                        else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.acd.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredACDStats.ContainsKey(stat.SearchedStatname))
                            {
                                if (Settings.GetInstance().DictConfiguredACDStats[stat.SearchedStatname].Contains(Filter))
                                {
                                    temp = false;
                                }
                            }
                        }
                        else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.dn.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredDNStats.ContainsKey(stat.SearchedStatname))
                            {
                                if (Settings.GetInstance().DictConfiguredDNStats[stat.SearchedStatname].Contains(Filter))
                                {
                                    temp = false;
                                }
                            }
                        }
                        else if (string.Compare(obj.ToString(), StatisticsEnum.SectionName.vq.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredVQStats.ContainsKey(stat.SearchedStatname))
                            {
                                if (Settings.GetInstance().DictConfiguredVQStats[stat.SearchedStatname].Contains(Filter))
                                {
                                    temp = false;
                                }
                            }
                        }

                    }
                    if (!temp)
                    {
                        StatisticsName = string.Empty;
                        StatisticsNameWaterMarkText = EmptyField;
                        StatisticsNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                    }
                    else
                    {
                        StatisticsNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                    }
                }

                isObjectStatAlreadyConfigured = true;
            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigWindowViewModel : SectionChangedEvent() Method - Exception caught" + GeneralException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : SectionChangedEvent() Method - Exit");
        }

        #endregion

        #region SaveServerEditbtnClicked

        /// <summary>
        /// Saves the server editbtn clicked.
        /// </summary>
        /// <param name="obj">The obj.</param>
        public void SaveServerEditbtnClicked(object obj)
        {
            bool isConfigurationsCorrect = false;
            bool isStatAlreadyConfigured = false;

            try
            {
                logger.Debug("AdminConfigWindowViewModel : SaveServerEditbtnClicked() Method - Entry");
                var values = (object[])obj;
                string sectionName = values[1].ToString() + "-" + values[0].ToString();
                string statisticsName = values[2].ToString();
                //string[] section = sectionName.Split('-');
                List<string> StatFilterList = new List<string>();

                #region Store Configured Stats

                if (sectionName.StartsWith("agent"))
                {
                    if (Settings.GetInstance().ListAgentStatistics.Contains(statisticsName))
                        isConfigurationsCorrect = true;
                }
                else if (sectionName.StartsWith("group"))
                {
                    if (Settings.GetInstance().ListAgentGroupStatistics.Contains(statisticsName))
                        isConfigurationsCorrect = true;
                }
                else if (sectionName.StartsWith("acd"))
                {
                    if (Settings.GetInstance().ListACDQueueStatistics.Contains(statisticsName))
                        isConfigurationsCorrect = true;
                }
                else if (sectionName.StartsWith("vq"))
                {
                    if (Settings.GetInstance().ListVirtualQueueStatistics.Contains(statisticsName))
                        isConfigurationsCorrect = true;
                }
                else if (sectionName.StartsWith("dn"))
                {
                    if (Settings.GetInstance().ListGroupQueueStatistics.Contains(statisticsName))
                        isConfigurationsCorrect = true;
                }

                if (SelectedFilter == "None" || SelectedFilter == string.Empty)
                {
                    SelectedFilter = "";
                }
                StatFilterList.Add(SelectedFilter);

                if (isConfigurationsCorrect)
                {
                    Dictionary<string, string> dictTempStatProperty = new System.Collections.Generic.Dictionary<string, string>();

                    dictTempStatProperty.Add(StatisticsEnum.StatProperties.DisplayName.ToString(), PropDisplayName);
                    dictTempStatProperty.Add(StatisticsEnum.StatProperties.StatName.ToString(), StatisticsName);
                    dictTempStatProperty.Add(StatisticsEnum.StatProperties.TooltipName.ToString(), TooltipValue);
                    dictTempStatProperty.Add(StatisticsEnum.StatProperties.ThresLevel1.ToString(), Threshold1);
                    dictTempStatProperty.Add(StatisticsEnum.StatProperties.ThresLevel2.ToString(), Threshold2);
                    dictTempStatProperty.Add(StatisticsEnum.StatProperties.Color1.ToString(), StatisticsColor.ToString());
                    dictTempStatProperty.Add(StatisticsEnum.StatProperties.Color2.ToString(), ThresholdColor1.ToString());
                    dictTempStatProperty.Add(StatisticsEnum.StatProperties.Color3.ToString(), ThresholdColor2.ToString());

                    if (SelectedFilter == "None" || SelectedFilter == string.Empty)
                    {
                        SelectedFilter = "";
                    }
                    dictTempStatProperty.Add(StatisticsEnum.StatProperties.Filter.ToString(), SelectedFilter);

                    if (SelectedFormat != string.Empty && SelectedFormat != "")
                    {
                        string[] formatSelected = SelectedFormat.Split('(');
                        dictTempStatProperty.Add(StatisticsEnum.StatProperties.Format.ToString(), formatSelected[0].ToString().Trim());
                    }


                    if (Settings.GetInstance().IsExistingStat)
                    {
                        if (string.Compare(ExistSectionType, StatisticsEnum.SectionName.agent.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredAgentStats[Statistics].Contains(ExistFilter))
                            {
                                Settings.GetInstance().DictConfiguredAgentStats[Statistics].RemoveAt(Settings.GetInstance().DictConfiguredAgentStats[Statistics].IndexOf(ExistFilter));
                            }

                        }
                        else if (string.Compare(ExistSectionType, StatisticsEnum.SectionName.group.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredAGroupStats[Statistics].Contains(ExistFilter))
                            {
                                Settings.GetInstance().DictConfiguredAGroupStats[Statistics].RemoveAt(Settings.GetInstance().DictConfiguredAGroupStats[Statistics].IndexOf(ExistFilter));
                            }

                        }
                        else if (string.Compare(ExistSectionType, StatisticsEnum.SectionName.acd.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredACDStats[Statistics].Contains(ExistFilter))
                            {
                                Settings.GetInstance().DictConfiguredACDStats[Statistics].RemoveAt(Settings.GetInstance().DictConfiguredACDStats[Statistics].IndexOf(ExistFilter));
                            }
                        }
                        else if (string.Compare(ExistSectionType, StatisticsEnum.SectionName.dn.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredDNStats[Statistics].Contains(ExistFilter))
                            {
                                Settings.GetInstance().DictConfiguredDNStats[Statistics].RemoveAt(Settings.GetInstance().DictConfiguredDNStats[Statistics].IndexOf(ExistFilter));
                            }
                        }
                        else if (string.Compare(ExistSectionType, StatisticsEnum.SectionName.vq.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredVQStats[Statistics].Contains(ExistFilter))
                            {
                                Settings.GetInstance().DictConfiguredVQStats[Statistics].RemoveAt(Settings.GetInstance().DictConfiguredVQStats[Statistics].IndexOf(ExistFilter));
                            }
                        }
                        if (sectionName.StartsWith("agent"))
                        {

                            int count = 0;
                            if (Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(statisticsName))
                            {
                                foreach (KeyValuePair<string, List<string>> AgentItem in Settings.GetInstance().DictConfiguredAgentStats)
                                {
                                    if ((string.Compare(AgentItem.Key, dictTempStatProperty[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (AgentItem.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()])))
                                    {
                                        foreach (string str in AgentItem.Value)
                                        {
                                            if (string.Compare(str, dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()], true) == 0)
                                            {
                                                count++;
                                            }
                                        }
                                        if (count > 0)
                                        {
                                            isStatAlreadyConfigured = true;
                                        }
                                        else
                                        {
                                            AgentItem.Value.Remove(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]);
                                            Settings.GetInstance().DictConfiguredAgentStats[AgentItem.Key] = AgentItem.Value;
                                            break;
                                        }
                                    }

                                }
                            }

                        }
                        else if (sectionName.StartsWith("group"))
                        {

                            int count = 0;
                            if (Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(statisticsName))
                            {
                                foreach (KeyValuePair<string, List<string>> AgentGroupItem in Settings.GetInstance().DictConfiguredAGroupStats)
                                {
                                    if ((string.Compare(AgentGroupItem.Key, dictTempStatProperty[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (AgentGroupItem.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()])))
                                    {
                                        foreach (string str in AgentGroupItem.Value)
                                        {
                                            if (string.Compare(str, dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()], true) == 0)
                                            {
                                                count++;
                                            }
                                        }
                                        if (count > 0)
                                        {
                                            isStatAlreadyConfigured = true;
                                        }
                                        else
                                        {
                                            AgentGroupItem.Value.Remove(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]);
                                            Settings.GetInstance().DictConfiguredAGroupStats[AgentGroupItem.Key] = AgentGroupItem.Value;
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                        else if (sectionName.StartsWith("acd"))
                        {

                            int count = 0;
                            if (Settings.GetInstance().DictConfiguredACDStats.ContainsKey(statisticsName))
                            {
                                foreach (KeyValuePair<string, List<string>> ACDItem in Settings.GetInstance().DictConfiguredACDStats)
                                {
                                    if ((string.Compare(ACDItem.Key, dictTempStatProperty[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (ACDItem.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()])))
                                    {
                                        foreach (string str in ACDItem.Value)
                                        {
                                            if (string.Compare(str, dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()], true) == 0)
                                            {
                                                count++;
                                            }
                                        }
                                        if (count > 0)
                                        {
                                            isStatAlreadyConfigured = true;
                                        }
                                        else
                                        {
                                            ACDItem.Value.Remove(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]);
                                            Settings.GetInstance().DictConfiguredACDStats[ACDItem.Key] = ACDItem.Value;
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                        else if (sectionName.StartsWith("dn"))
                        {

                            int count = 0;
                            if (Settings.GetInstance().DictConfiguredDNStats.ContainsKey(statisticsName))
                            {
                                foreach (KeyValuePair<string, List<string>> DNItem in Settings.GetInstance().DictConfiguredDNStats)
                                {
                                    if ((string.Compare(DNItem.Key, dictTempStatProperty[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (DNItem.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()])))
                                    {
                                        foreach (string str in DNItem.Value)
                                        {
                                            if (string.Compare(str, dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()], true) == 0)
                                            {
                                                count++;
                                            }
                                        }
                                        if (count > 0)
                                        {
                                            isStatAlreadyConfigured = true;
                                        }
                                        else
                                        {
                                            DNItem.Value.Remove(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]);
                                            Settings.GetInstance().DictConfiguredDNStats[DNItem.Key] = DNItem.Value;
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                        else if (sectionName.StartsWith("vq"))
                        {

                            int count = 0;
                            if (Settings.GetInstance().DictConfiguredVQStats.ContainsKey(statisticsName))
                            {
                                foreach (KeyValuePair<string, List<string>> VQItem in Settings.GetInstance().DictConfiguredVQStats)
                                {
                                    if ((string.Compare(VQItem.Key, dictTempStatProperty[StatisticsEnum.StatProperties.StatName.ToString()], true) == 0) && (VQItem.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()])))
                                    {
                                        foreach (string str in VQItem.Value)
                                        {
                                            if (string.Compare(str, dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()], true) == 0)
                                            {
                                                count++;
                                            }
                                        }
                                        if (count > 0)
                                        {
                                            isStatAlreadyConfigured = true;
                                        }
                                        else
                                        {
                                            VQItem.Value.Remove(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]);
                                            Settings.GetInstance().DictConfiguredVQStats[VQItem.Key] = VQItem.Value;
                                            break;
                                        }
                                    }

                                }
                            }
                        }


                    }

                    if (sectionName.StartsWith("agent"))
                    {
                        if (Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(statisticsName))
                        {
                            foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredAgentStats)
                            {
                                if ((string.Compare(item.Key, statisticsName, true) == 0) && (item.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()])))
                                {
                                    isStatAlreadyConfigured = true;
                                }
                            }
                        }
                    }

                    if (sectionName.StartsWith("group"))
                    {
                        if (Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(statisticsName))
                        {
                            foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredAGroupStats)
                            {
                                if ((string.Compare(item.Key, statisticsName, true) == 0) && (item.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()])))
                                {
                                    isStatAlreadyConfigured = true;
                                }
                            }
                        }

                    }

                    if (sectionName.StartsWith("acd"))
                    {
                        if (Settings.GetInstance().DictConfiguredACDStats.ContainsKey(statisticsName))
                        {
                            foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredACDStats)
                            {
                                if ((string.Compare(item.Key, statisticsName, true) == 0) && (item.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()])))
                                {
                                    isStatAlreadyConfigured = true;
                                }
                            }
                        }

                    }

                    if (sectionName.StartsWith("dn"))
                    {
                        if (Settings.GetInstance().DictConfiguredDNStats.ContainsKey(statisticsName))
                        {
                            foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredDNStats)
                            {
                                if ((string.Compare(item.Key, statisticsName, true) == 0) && (item.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()])))
                                {
                                    isStatAlreadyConfigured = true;
                                }
                            }
                        }

                    }

                    if (sectionName.StartsWith("vq"))
                    {
                        if (Settings.GetInstance().DictConfiguredVQStats.ContainsKey(statisticsName))
                        {
                            foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredVQStats)
                            {
                                if ((string.Compare(item.Key, statisticsName, true) == 0) && (item.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()])))
                                {
                                    isStatAlreadyConfigured = true;
                                }
                            }
                        }
                    }



                    if (!isStatAlreadyConfigured)
                    {
                        if (!Settings.GetInstance().DictConfigStats.ContainsKey(sectionName) || (sectionName == ExistingSection))
                        {
                            if (Settings.GetInstance().DictConfigStats.ContainsKey(sectionName))
                                Settings.GetInstance().DictConfigStats.Remove(sectionName);
                            else if (Settings.GetInstance().DictConfigStats.ContainsKey(ExistingSection))
                                Settings.GetInstance().DictConfigStats.Remove(ExistingSection);

                            foreach (StatisticsProperties StatProp in ConfiguredStatistics)
                            {
                                if (StatProp.SectionName == sectionName || StatProp.SectionName == ExistingSection)
                                {
                                    ConfiguredStatistics.Remove(StatProp);
                                    break;
                                }
                            }
                            Settings.GetInstance().DictConfigStats.Add(sectionName, dictTempStatProperty);

                            if (sectionName.StartsWith("agent"))
                            {
                                if (Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(statisticsName))
                                {
                                    foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredAgentStats)
                                    {
                                        if (!((string.Compare(item.Key, statisticsName, true) == 0) && (item.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]))))
                                        {
                                            Settings.GetInstance().DictConfiguredAgentStats[statisticsName].Add(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]);
                                            break;
                                        }
                                    }

                                }
                                else
                                {
                                    Settings.GetInstance().DictConfiguredAgentStats.Add(statisticsName, StatFilterList);
                                }
                            }



                            if (sectionName.StartsWith("group"))
                            {
                                if (Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(statisticsName))
                                {
                                    foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredAGroupStats)
                                    {
                                        if (!((string.Compare(item.Key, statisticsName, true) == 0) && (item.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]))))
                                        {
                                            Settings.GetInstance().DictConfiguredAGroupStats[statisticsName].Add(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Settings.GetInstance().DictConfiguredAGroupStats.Add(statisticsName, StatFilterList);
                                }
                            }

                            if (sectionName.StartsWith("acd"))
                            {
                                if (Settings.GetInstance().DictConfiguredACDStats.ContainsKey(statisticsName))
                                {
                                    foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredACDStats)
                                    {
                                        if (!((string.Compare(item.Key, statisticsName, true) == 0) && (item.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]))))
                                        {
                                            Settings.GetInstance().DictConfiguredACDStats[statisticsName].Add(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Settings.GetInstance().DictConfiguredACDStats.Add(statisticsName, StatFilterList);
                                }
                            }
                            if (sectionName.StartsWith("dn"))
                            {
                                if (Settings.GetInstance().DictConfiguredDNStats.ContainsKey(statisticsName))
                                {
                                    foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredDNStats)
                                    {
                                        if (!((string.Compare(item.Key, statisticsName, true) == 0) && (item.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]))))
                                        {
                                            Settings.GetInstance().DictConfiguredDNStats[statisticsName].Add(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Settings.GetInstance().DictConfiguredDNStats.Add(statisticsName, StatFilterList);
                                }
                            }
                            if (sectionName.StartsWith("vq"))
                            {
                                if (Settings.GetInstance().DictConfiguredVQStats.ContainsKey(statisticsName))
                                {
                                    foreach (KeyValuePair<string, List<string>> item in Settings.GetInstance().DictConfiguredVQStats)
                                    {
                                        if (!((string.Compare(item.Key, statisticsName, true) == 0) && (item.Value.Contains(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]))))
                                        {
                                            Settings.GetInstance().DictConfiguredVQStats[statisticsName].Add(dictTempStatProperty[StatisticsEnum.StatProperties.Filter.ToString()]);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    Settings.GetInstance().DictConfiguredVQStats.Add(statisticsName, StatFilterList);
                                }
                            }


                            objStatSupport.CheckStaticsticsObject(statisticsName);

                            if (Settings.GetInstance().IsExistingStat)
                            {
                                ConfiguredStatistics.Add(new StatisticsProperties() { isGridChecked = true, SectionName = sectionName.Length > 35 ? sectionName.Substring(0, 32) + ".." : sectionName, SectionTooltip = sectionName, DisplayName = PropDisplayName.Length > 35 ? PropDisplayName.Substring(0, 33) + ".." : PropDisplayName, DisplayTooltip = PropDisplayName, EditName = "Edit", IsExistProperty = true });

                                if (Settings.GetInstance().DictExistingApplicationStats.ContainsKey(sectionName))
                                {
                                    Settings.GetInstance().DictExistingApplicationStats.Remove(sectionName);
                                }
                                else if (Settings.GetInstance().DictExistingApplicationStats.ContainsKey(ExistingSection))
                                {
                                    Settings.GetInstance().DictExistingApplicationStats.Remove(ExistingSection);
                                }
                                Settings.GetInstance().DictExistingApplicationStats.Add(sectionName, dictTempStatProperty);

                                string[] secName = sectionName.Split('-');
                                for (int item = 0; item < ServerStatistics.Count; item++)
                                {
                                    if (string.Compare(ServerStatistics[item].SectionName.Trim(), ExistingSection, true) == 0)
                                    {
                                        ServerStatistics.RemoveAt(item);
                                        break;
                                    }
                                }
                            }
                            else
                            {

                                if (!Settings.GetInstance().IsAgentRemain && !Settings.GetInstance().IsAGroupRemain &&
                                    !Settings.GetInstance().IsACDRemain && !Settings.GetInstance().IsDNRemain && !Settings.GetInstance().IsVQRemain)
                                {
                                    for (int item = 0; item < ServerStatistics.Count; item++)
                                    {
                                        if (string.Compare(ServerStatistics[item].SectionName.Trim(), values[2].ToString().Trim(), true) == 0)
                                        {
                                            ConfiguredStatistics.Add(new StatisticsProperties() { isGridChecked = true, SectionName = sectionName.Length > 35 ? sectionName.Substring(0, 32) + ".." : sectionName, SectionTooltip = sectionName,DisplayName = PropDisplayName.Length > 35 ? PropDisplayName.Substring(0, 33) + ".." : PropDisplayName, DisplayTooltip = PropDisplayName, EditName = "Edit", IsExistProperty = true });

                                            if (!Settings.GetInstance().DictExistingApplicationStats.ContainsKey(sectionName))
                                            {
                                                Settings.GetInstance().DictExistingApplicationStats.Add(sectionName, dictTempStatProperty);
                                            }
                                            ServerStatistics.RemoveAt(item);
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    for (int item = 0; item < ServerStatistics.Count; item++)
                                    {
                                        if (string.Compare(ServerStatistics[item].SectionName.Trim(), values[2].ToString().Trim(), true) == 0)
                                        {
                                            ConfiguredStatistics.Add(new StatisticsProperties() { isGridChecked = true, SectionName = sectionName.Length > 35 ? sectionName.Substring(0, 32) + ".." : sectionName, SectionTooltip = sectionName, DisplayName = PropDisplayName.Length > 35 ? PropDisplayName.Substring(0, 33) + ".." : PropDisplayName, DisplayTooltip = PropDisplayName, EditName = "Edit", IsExistProperty = true });


                                            if (!Settings.GetInstance().DictExistingApplicationStats.ContainsKey(sectionName))
                                            {
                                                Settings.GetInstance().DictExistingApplicationStats.Add(sectionName, dictTempStatProperty);
                                            }

                                            ServerStatistics[item].isGridChecked = false;
                                            break;
                                        }
                                    }
                                }
                            }
                            GridWidth = new GridLength(0);
                            DataGridRowHeight = new GridLength(0);

                            TitleSpan = 1;
                        }
                        else
                        {
                            Views.MessageBox msgbox;
                            ViewModels.MessageBoxViewModel mboxvmodel;
                            msgbox = new Views.MessageBox();
                            mboxvmodel = new MessageBoxViewModel("Information", "Provided Section name already exist. Please provide a different section name.", msgbox, "MainWindow", Settings.GetInstance().Theme);
                            msgbox.DataContext = mboxvmodel;
                            msgbox.ShowDialog();
                        }
                    }
                    else
                    {
                        Views.MessageBox msgbox;
                        ViewModels.MessageBoxViewModel mboxvmodel;
                        msgbox = new Views.MessageBox();
                        mboxvmodel = new MessageBoxViewModel("Information", "Provided statistics already configured. Please provide different object (or) different statistics", msgbox, "MainWindow", Settings.GetInstance().Theme);
                        msgbox.DataContext = mboxvmodel;
                        msgbox.ShowDialog();
                    }
                }
                else
                {
                    Views.MessageBox msgbox;
                    ViewModels.MessageBoxViewModel mboxvmodel;
                    msgbox = new Views.MessageBox();
                    mboxvmodel = new MessageBoxViewModel("Information", "Provided Section name doesn't match the object of the configured statistics. Please give the correct section name.", msgbox, "MainWindow", Settings.GetInstance().Theme);
                    msgbox.DataContext = mboxvmodel;
                    msgbox.ShowDialog();
                }
                #endregion
                //}

            }
            catch (Exception GeneraLException)
            {
                logger.Debug("AdminConfigWindowViewModel : SaveServerEditbtnClicked() Method - Exception caught" + GeneraLException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : SaveServerEditbtnClicked() Method - Exit");
        }

        #endregion

        #region SearchStatisticsbtnClicked

        private void SearchStatisticsbtnClicked()
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : SearchStatisticsbtnClicked() Method - Entry");
                DataGridRowHeight = new GridLength(300);
                logger.Debug("AdminConfigWindowViewModel : SearchStatisticsbtnClicked() Method - Exit");
            }
            catch (Exception generalException)
            {
                logger.Debug("AdminConfigWindowViewModel : SearchStatisticsbtnClicked() Method - Exception caught" + generalException.Message.ToString());
            }
        }
        #endregion

        #region DataGridLostFocus

        private void DataGridLostFocus(object obj)
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : DataGridLostFocus() Method - Entry");
                DataGridRowHeight = new GridLength(0);
                logger.Debug("AdminConfigWindowViewModel : DataGridLostFocus() Method - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("AdminConfigWindowViewModel : DataGridLostFocus() Method - Exception caught" + generalException.Message.ToString());
            }

        }
        #endregion

        #region DataGridRadiobtnChecked

        private void DataGridRadiobtnChecked(object obj)
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : DataGridRadiobtnChecked() Method - Entry");
                if (obj != null)
                {
                    StatisticsName = obj.ToString();
                }
                logger.Debug("AdminConfigWindowViewModel : DataGridRadiobtnChecked() Method - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("AdminConfigWindowViewModel : DataGridRadiobtnChecked() Method - Exception caught" + generalException.Message.ToString());
            }
        }
        #endregion

        #region FormatTypeSelectionChanged

        private void FormatTypeSelectionChanged(object obj)
        {

            try
            {
                logger.Debug("AdminConfigWindowViewModel : FormatTypeSelectionChanged() Method - Entry");
                if (obj != null)
                {
                    Settings.GetInstance().SelectedFormat = obj.ToString();
                    if (string.IsNullOrEmpty(Threshold1))
                    {
                        Threshold1 = string.Empty;
                        Threshold1WaterMarkText = EmptyField;
                        Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                    }
                    else
                    {
                        if (isThresholdFirstTime)
                        {
                            Threshold1Validation();
                        }

                    }
                    if (string.IsNullOrEmpty(Threshold2))
                    {
                        Threshold2 = string.Empty;

                        Threshold2WaterMarkText = EmptyField;

                        Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                    }
                    else
                    {
                        if (isThresholdFirstTime)
                        {
                            Threshold2Validation();
                        }

                    }
                }
                isThresholdFirstTime = true;


            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigWindowViewModel : FormatTypeSelectionChanged() Method - Exception caught" + GeneralException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : FormatTypeSelectionChanged() Method - Exit");

        }

        #endregion

        //#region MediaTypeChangedCommand
        //public void MediaTypeChanged(object obj)
        //{
        //    try
        //    {
        //        logger.Debug("AdminConfigWindowViewModel : MediaTypeChanged() Method - Exit");

        //    }
        //    catch (Exception generalException)
        //    {
        //        logger.Error("AdminConfigWindowViewModel : MediaTypeChanged() Method - Exit");
        //    }
        //    logger.Debug("AdminConfigWindowViewModel : MediaTypeChanged() Method - Exit");
        //}
        //#endregion

        #region SavebtnMouseOver

        private void SavebtnMouseOver()
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : SavebtnMouseOver() Method - Entry");
                if ((StatSectionName == EmptyField || StatSectionName.Trim().ToString() == string.Empty) || (PropDisplayName == EmptyField || PropDisplayName.Trim().ToString() == string.Empty) || (StatisticsName == EmptyField || StatisticsName.Trim().ToString() == string.Empty)
                  || (TooltipValue == EmptyField || TooltipValue.Trim().ToString() == string.Empty) || (Threshold1 == EmptyField || Threshold1.Trim().ToString() == string.Empty) || (Threshold2 == EmptyField || Threshold2.Trim().ToString() == string.Empty) || (Threshold1 == DecimalFormat || Threshold1.Trim().ToString() == string.Empty)
                  || (Threshold2 == DecimalFormat || Threshold2.Trim().ToString() == string.Empty) || (Threshold1 == TimeFormat || Threshold1.Trim().ToString() == string.Empty) || (Threshold2 == TimeFormat || Threshold2.Trim().ToString() == string.Empty)
                  || (Threshold1 == ThresholdMessage1 || Threshold1.Trim().ToString() == string.Empty) || (Threshold2 == ThresholdMessage2 || Threshold2.Trim().ToString() == string.Empty))
                {
                    IsSavebtnEnable = false;
                }
                else
                {
                    IsSavebtnEnable = true;
                }
                logger.Debug("AdminConfigWindowViewModel : SavebtnMouseOver() Method - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("AdminConfigWindowViewModel : SavebtnMouseOver() Method - Exception caught" + generalException.Message.ToString());
            }
        }

        #endregion

        #region TextChangedEvent

        private void TextChangedEvent()
        {
            try
            {

                logger.Debug("AdminConfigWindowViewModel : TextChangedEvent() Method - Entry");
                if ((StatSectionName == EmptyField || StatSectionName.Trim().ToString() == string.Empty) || (PropDisplayName == EmptyField || PropDisplayName.Trim().ToString() == string.Empty) || (StatisticsName == EmptyField || StatisticsName.Trim().ToString() == string.Empty)
                  || (TooltipValue == EmptyField || TooltipValue.Trim().ToString() == string.Empty) || (Threshold1 == EmptyField || Threshold1.Trim().ToString() == string.Empty) || (Threshold2 == EmptyField || Threshold2.Trim().ToString() == string.Empty) || (Threshold1 == DecimalFormat || Threshold1.Trim().ToString() == string.Empty)
                  || (Threshold2 == DecimalFormat || Threshold2.Trim().ToString() == string.Empty) || (Threshold1 == TimeFormat || Threshold1.Trim().ToString() == string.Empty) || (Threshold2 == TimeFormat || Threshold2.Trim().ToString() == string.Empty)
                  || (Threshold1 == ThresholdMessage1 || Threshold1.Trim().ToString() == string.Empty) || (Threshold2 == ThresholdMessage2 || Threshold2.Trim().ToString() == string.Empty))
                {

                    IsSavebtnEnable = false;
                }
                else
                {
                    IsSavebtnEnable = true;
                }

            }
            catch (Exception generalException)
            {
                logger.Debug("AdminConfigWindowViewModel : TextChangedEvent() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : TextChangedEvent() Method - Exit");
        }

        #endregion

        #region TextBoxLostFocus
        private void TextBoxLostFocus(object obj)
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : TextBoxLostFocus() Method - Entry");
                string property = obj as string;
                switch (property)
                {
                    case "SectionNameWaterMarkText": if (string.IsNullOrEmpty(StatSectionName))
                        {
                            SectionNameWaterMarkText = EmptyField;
                            SectionNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                        }
                        else
                        {
                            SectionNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                        }
                        break;
                    case "DisplayNameWaterMarkText": if (string.IsNullOrEmpty(PropDisplayName))
                        {
                            DisplayNameWaterMarkText = EmptyField;
                            DisplayNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                        }
                        else
                        {
                            DisplayNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                        }
                        break;
                    //case "MediaTypeWaterMarkText": if (string.IsNullOrEmpty(PropMediaType))
                    //    {
                    //        MediaTypeWaterMarkText = EmptyField;
                    //        MediaTypeWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                    //    }
                    //    else
                    //    {
                    //        MediaTypeWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                    //    }
                    //    break;
                    case "StatisticsNameWaterMarkText": if (string.IsNullOrEmpty(StatisticsName))
                        {
                            StatisticsNameWaterMarkText = EmptyField;
                            StatisticsNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                        }
                        else
                        {
                            StatisticsNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                        }
                        break;
                    case "TooltipWaterMarkText": if (string.IsNullOrEmpty(TooltipValue))
                        {
                            TooltipWaterMarkText = EmptyField;
                            TooltipWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                        }
                        else
                        {
                            TooltipWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                        }
                        break;
                    case "Threshold1WaterMarkText": Threshold1Validation();
                        break;
                    case "Threshold2WaterMarkText": Threshold2Validation();
                        break;

                }
                logger.Debug("AdminConfigWindowViewModel : TextBoxLostFocus() Method - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("AdminConfigWindowViewModel : TextBoxLostFocus() Method - Exception caught" + generalException.Message.ToString());
            }
        }
        #endregion

        #region Threshold1Validation
        private void Threshold1Validation()
        {
            try
            {
                logger.Debug("AdminConfigwindowViewModel : Threshold1Validation() Method - Threshold 1 validation starts");
                string selectedFormat = Settings.GetInstance().SelectedFormat;
                decimal d;
                DateTime datetime;
                if (string.IsNullOrEmpty(Threshold1))
                {
                    Threshold1WaterMarkText = EmptyField;
                    Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectedFormat))
                    {
                        if (selectedFormat[0].ToString() == "D")
                        {

                            if (!decimal.TryParse(Threshold1, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out d))
                            {
                                Threshold1WaterMarkText = DecimalFormat;
                                Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                                Threshold1 = string.Empty;
                            }
                            else if (string.IsNullOrEmpty(Threshold2))
                            {
                                Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                            }
                            else if (int.Parse(Threshold1) >= int.Parse(Threshold2))
                            {
                                Threshold1WaterMarkText = ThresholdMessage1;
                                Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                                Threshold1 = string.Empty;
                            }
                            else
                            {
                                Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                            }
                        }
                        if (selectedFormat[0].ToString() == "T")
                        {
                            if (!DateTime.TryParseExact(Threshold1, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out datetime))
                            {
                                Threshold1WaterMarkText = TimeFormat;
                                Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                                Threshold1 = string.Empty;
                            }
                            else if (string.IsNullOrEmpty(Threshold2))
                            {
                                Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));

                            }
                            else
                            {

                                DateTime t2 = DateTime.Parse(Threshold2);
                                DateTime t1 = DateTime.Parse(Threshold1);
                                if ((DateTime.Compare(t1, t2) > 0) || (DateTime.Compare(t1, t2) == 0))
                                {
                                    Threshold1WaterMarkText = ThresholdMessage1;
                                    Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                                    Threshold1 = string.Empty;
                                }
                                else
                                {
                                    Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                                }
                            }
                        }
                        if (selectedFormat[0].ToString() == "S")
                        {
                            Threshold1WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                        }


                    }

                }
                logger.Debug("CustomValidator : Threshold1Validation() Method - Threshold 2 validation ends");
            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigwindowViewModel : Threshold1Validation() Method - Exception caught-" + GeneralException.Message.ToString());
            }

        }
        #endregion

        #region Threshold2Validation
        private void Threshold2Validation()
        {
            try
            {
                logger.Debug("AdminConfigwindowViewModel : Threshold2Validation() Method - Threshold 2 validation starts");
                string selectedFormat = Settings.GetInstance().SelectedFormat;
                decimal d;
                DateTime datetime;
                if (string.IsNullOrEmpty(Threshold2))
                {
                    Threshold2WaterMarkText = EmptyField;
                    Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                    Threshold2 = string.Empty;
                }
                else
                {
                    if (!string.IsNullOrEmpty(selectedFormat))
                    {
                        if (selectedFormat[0].ToString() == "D")
                        {
                            if (!decimal.TryParse(Threshold2, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out d))
                            {
                                Threshold2WaterMarkText = DecimalFormat;
                                Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                                Threshold2 = string.Empty;
                            }
                            else if (string.IsNullOrEmpty(Threshold1))
                            {
                                Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                            }
                            else if (int.Parse(Threshold2) <= int.Parse(Threshold1))
                            {
                                Threshold2WaterMarkText = ThresholdMessage2;
                                Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                                Threshold2 = string.Empty;
                            }
                            else
                            {
                                Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                            }
                        }
                        if (selectedFormat[0].ToString() == "T")
                        {
                            if (!DateTime.TryParseExact(Threshold2, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out datetime))
                            {
                                Threshold2WaterMarkText = TimeFormat;
                                Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                                Threshold2 = string.Empty;
                            }
                            else if (string.IsNullOrEmpty(Threshold1))
                            {
                                Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                            }
                            else
                            {
                                DateTime t1 = DateTime.Parse(Threshold1);
                                DateTime t2 = DateTime.Parse(Threshold2);
                                if ((DateTime.Compare(t1, t2) > 0) || (DateTime.Compare(t1, t2) == 0))
                                {
                                    Threshold2WaterMarkText = ThresholdMessage2;
                                    Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                                    Threshold2 = string.Empty;

                                }
                                else
                                {
                                    Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                                }
                            }
                        }
                        if (selectedFormat[0].ToString() == "S")
                        {
                            Threshold2WaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                        }
                    }

                }
                logger.Debug("AdminConfigwindowViewModel : Threshold2Validation() Method - Threshold 2 validation ends");
            }
            catch (Exception GeneralException)
            {
                logger.Error("AdminConfigwindowViewModel : Threshold2Validation() Method - Exception caught-" + GeneralException.Message.ToString());
            }

        }
        #endregion

        #region FilterTypeChanged
        private void FilterTypeChanged(object obj)
        {
            try
            {
                logger.Debug("AdminConfigWindowViewModel : FilterTypeChanged() Method - Entry");
                Settings.GetInstance().DictStatisticsDesc.Clear();
                SearchStatnameCollection = new ObservableCollection<SearchedStatistics>();

                if (string.Compare(SectionType, StatisticsEnum.SectionName.agent.ToString(), true) == 0)
                {
                    objStatSupport.GetDescriptions("Agent");
                }
                else if (string.Compare(SectionType, StatisticsEnum.SectionName.group.ToString(), true) == 0)
                {
                    objStatSupport.GetDescriptions("GroupAgents");
                }
                else if (string.Compare(SectionType, StatisticsEnum.SectionName.acd.ToString(), true) == 0)
                {
                    objStatSupport.GetDescriptions("Queue");
                }
                else if (string.Compare(SectionType, StatisticsEnum.SectionName.dn.ToString(), true) == 0)
                {
                    objStatSupport.GetDescriptions("GroupQueues");
                }
                else if (string.Compare(SectionType, StatisticsEnum.SectionName.vq.ToString(), true) == 0)
                {
                    objStatSupport.GetDescriptions("RoutePoint");
                }
                string Filter = null;
                if (obj.ToString() == "None" || obj.ToString() == string.Empty)
                {
                    Filter = "";
                }
                else
                {
                    Filter = obj.ToString();
                }

                foreach (KeyValuePair<string, string> item in Settings.GetInstance().DictStatisticsDesc)
                {

                    string tempStatName = string.Empty;
                    string tempStatDesc = string.Empty;
                    tempStatName = item.Key.ToString().Length > 25 ? item.Key.ToString().Substring(0, 23) + ".." : item.Key.ToString();

                    tempStatDesc = item.Value.ToString().Length > 36 ? item.Value.ToString().Substring(0, 35) + ".." : item.Value.ToString();

                    if (string.Compare(SectionType, StatisticsEnum.SectionName.agent.ToString(), true) == 0)
                    {
                        if (!Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(item.Key))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                        else if (!Settings.GetInstance().DictConfiguredAgentStats[item.Key].Contains(Filter))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                    }
                    else if (string.Compare(SectionType, StatisticsEnum.SectionName.group.ToString(), true) == 0)
                    {
                        if (!Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(item.Key))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                        else if (!Settings.GetInstance().DictConfiguredAGroupStats[item.Key].Contains(Filter))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                    }
                    else if (string.Compare(SectionType, StatisticsEnum.SectionName.acd.ToString(), true) == 0)
                    {
                        if (!Settings.GetInstance().DictConfiguredACDStats.ContainsKey(item.Key))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                        else if (!Settings.GetInstance().DictConfiguredACDStats[item.Key].Contains(Filter))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                    }
                    else if (string.Compare(SectionType, StatisticsEnum.SectionName.dn.ToString(), true) == 0)
                    {
                        if (!Settings.GetInstance().DictConfiguredDNStats.ContainsKey(item.Key))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                        else if (!Settings.GetInstance().DictConfiguredDNStats[item.Key].Contains(Filter))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                    }
                    else if (string.Compare(SectionType, StatisticsEnum.SectionName.vq.ToString(), true) == 0)
                    {
                        if (!Settings.GetInstance().DictConfiguredVQStats.ContainsKey(item.Key))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                        else if (!Settings.GetInstance().DictConfiguredVQStats[item.Key].Contains(Filter))
                        {
                            SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key, SearchedStatDescTooltip = item.Value });
                        }
                    }
                }

                if (isObjectStatAlreadyConfigured)
                {
                    bool temp = true;
                    foreach (SearchedStatistics stat in SearchStatnameCollection)
                    {
                        if (string.Compare(SectionType, StatisticsEnum.SectionName.agent.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(stat.SearchedStatname))
                            {
                                if (Settings.GetInstance().DictConfiguredAgentStats[stat.SearchedStatname].Contains(Filter))
                                {
                                    temp = false;
                                }
                            }
                        }
                        else if (string.Compare(SectionType, StatisticsEnum.SectionName.group.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(stat.SearchedStatname))
                            {
                                if (Settings.GetInstance().DictConfiguredAGroupStats[stat.SearchedStatname].Contains(Filter))
                                {
                                    temp = false;
                                }
                            }
                        }
                        else if (string.Compare(SectionType, StatisticsEnum.SectionName.acd.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredACDStats.ContainsKey(stat.SearchedStatname))
                            {
                                if (Settings.GetInstance().DictConfiguredACDStats[stat.SearchedStatname].Contains(Filter))
                                {
                                    temp = false;
                                }
                            }
                        }
                        else if (string.Compare(SectionType, StatisticsEnum.SectionName.dn.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredDNStats.ContainsKey(stat.SearchedStatname))
                            {
                                if (Settings.GetInstance().DictConfiguredDNStats[stat.SearchedStatname].Contains(Filter))
                                {
                                    temp = false;
                                }
                            }
                        }
                        else if (string.Compare(SectionType, StatisticsEnum.SectionName.vq.ToString(), true) == 0)
                        {
                            if (Settings.GetInstance().DictConfiguredVQStats.ContainsKey(stat.SearchedStatname))
                            {
                                if (Settings.GetInstance().DictConfiguredVQStats[stat.SearchedStatname].Contains(Filter))
                                {
                                    temp = false;
                                }
                            }
                        }

                    }
                    if (!temp)
                    {
                        StatisticsName = string.Empty;
                        StatisticsNameWaterMarkText = EmptyField;
                        StatisticsNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Red"));
                    }
                    else
                    {
                        StatisticsNameWaterMarkColor = (SolidColorBrush)(new BrushConverter().ConvertFrom("Gray"));
                    }
                }

                isObjectStatAlreadyConfigured = true;

            }
            catch (Exception generalException)
            {
                logger.Debug("AdminConfigWindowViewModel : FilterTypeChanged() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : FilterTypeChanged() Method - Exit");
        }
        #endregion

        #region SortMeidaTypeChanged
        private void SortMeidaTypeChanged(object selectedMediaType)
        {
            try
            {
                var Sourcequery = from type in SortMediaTypeSource orderby type.Equals(selectedMediaType.ToString()) descending select type;
                ObservableCollection<string> TempSortMediaTypeSource = new ObservableCollection<string>();
                foreach (string str in Sourcequery)
                {
                    TempSortMediaTypeSource.Add(str);
                }
                SortMediaTypeSource.Clear();
                SortMediaTypeSource = TempSortMediaTypeSource;
                SortSelectedMediaType = selectedMediaType.ToString();
                ObservableCollection<StatisticsProperties> tempConfigstatsCollection = new ObservableCollection<StatisticsProperties>();
              
                ConfiguredStatistics.Clear();
                ConfiguredStatistics = tempConfigstatsCollection;
                logger.Debug("AdminConfigWindowViewModel : SortMeidaTypeChanged() Method - Entry");
            }
            catch (Exception generalException)
            {
                logger.Error("AdminConfigWindowViewModel : SortMeidaTypeChanged() Method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("AdminConfigWindowViewModel : SortMeidaTypeChanged() Method - Exit");
        }
        #endregion

        #region NotifyStatErrorMessage
        public void NotifyStatErrorMessage(OutputValues output)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region NotifyAgentGlobalStatus
        public void NotifyAgentGlobalStatus(string agentid, string status, string media)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region NotifyAgentExtendedStatus
        public void NotifyAgentExtendedStatus(string agentid, Dictionary<string, string> status)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region NotifyReportIndividualStatus
        public void NotifyReportIndividualStatus(Dictionary<string, string> agentStatus)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region NotifyReportIndividualStatistics
        public void NotifyReportIndividualStatistics(Dictionary<string, Dictionary<string, string[]>> agentStatistics)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region NotifyAgentStatistics
        public void NotifyAgentStatistics(string agentid, string statisticsName, string statisticsValue, string mediaType, string toolTip, System.Drawing.Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region NotifyAgentGroupStatistics
        public void NotifyAgentGroupStatistics(string agentid, string statisticsName, string statisticsValue, string mediaType, string toolTip, System.Drawing.Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region NotifyQueueStatistics
        public void NotifyQueueStatistics(string agentid, string statisticsName, string statisticsValue, string mediaType, string toolTip, System.Drawing.Color statColor, string statType, bool isThresholdBreach, bool isLevelTwo)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region NotifyToApplication
        public void NotifyToApplication(string objectType)
        {
            try
            {
                logger.Debug("AdminMonitorViewModel : NotifyToApplication() method - Entry");
                _UpdateApplicationDetails.Invoke(objectType);
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : NotifyToApplication Method : Exception caught " + ex.Message);
            }
        }
        #endregion

        #region ApplicationUpdate
        public void ApplicationUpdate(string objType)
        {
            try
            {
                logger.Debug("AdminMonitorViewModel : ApplicationUpdate() method - Entry");
                System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(delegate
                {
                    Views.MessageBox msgbox;
                    ViewModels.MessageBoxViewModel mboxvmodel;
                    msgbox = new Views.MessageBox();
                    mboxvmodel = new MessageBoxViewModel("Information", "Objects was Changed...Values will be updated. ", msgbox, "MainWindow", Settings.GetInstance().Theme);
                    msgbox.DataContext = mboxvmodel;
                    msgbox.ShowDialog();
                    if (Settings.GetInstance().adminConfigVM != null)
                    {
                        if (objType == "Application")
                        {
                            Settings.GetInstance().DictServerFilters.Clear();
                            Settings.GetInstance().DictServerFilters = new Dictionary<string, string>(objStatTicker.ReadFilters());
                            //Settings.GetInstance().LstServerMediaTypes = objStatTicker.ReadMediaTypes();
                            SortMediaTypeSource.Clear();
                            ApplicationStatisticsConfig();
                            if (SortMediaTypeSource.Count > 0)
                            {
                                SortMeidaTypeChanged(SortMediaTypeSource[0]);
                            }

                        }
                    }
                    if (Settings.GetInstance().QueueConfigVM != null)
                    {
                        if ((objType == "Application") || (objType == "Switch"))
                        {
                            LoadObjectConfigWindow();
                        }

                    }
                    if (Settings.GetInstance().ObjTabVm != null)
                    {
                        if (Settings.GetInstance().ObjTabVm.ObjectConfigVM != null)
                        {
                            if (objType != "Application")
                            {
                                LoadEditObjectConfigWindow(objType);
                            }
                        }
                        LoadNewObjectConfigWindow();
                    }


                }), DispatcherPriority.ContextIdle, new object[0]);
            }
            catch (Exception ex)
            {
                logger.Error("MainWindowViewModel : ApplicationUpdate Method : Exception caught " + ex.Message);
            }

            logger.Debug("AdminMonitorViewModel : ApplicationUpdate() method - Exit");
        }
        #endregion

        #region LoadNewObjectConfigWindow
        private void LoadNewObjectConfigWindow()
        {
            try
            {
                logger.Debug("MainWindowViewModel : LoadNewObjectConfigWindow Method : Entry ");

                if (Settings.GetInstance().ObjTabVm == null)
                {
                    Settings.GetInstance().ObjTabVm = new TabValueViewModel();
                }
                else
                {
                    if (Settings.GetInstance().ObjTabVm.NewObjectConfigVM != null)
                    {
                        Settings.GetInstance().DictApplicationObjects.Clear();
                        Settings.GetInstance().DictApplicationObjects.Add(Settings.GetInstance().ApplicationName, new Dictionary<string, List<string>>(objStatTicker.GetExistingValues()));

                        if (Settings.GetInstance().ObjTabVm.SelectedStatistics.ObjectsNameTT.StartsWith(StatisticsEnum.SectionName.acd.ToString()))
                        {
                            Settings.GetInstance().ObjTabVm.NewObjectConfigVM.LoadObjectConfiguration(StatisticsEnum.ObjectType.ACDQueue.ToString(), Settings.GetInstance().ObjTabVm.SelectedStatistics.ObjectsNameTT.ToString(), Settings.GetInstance().ObjTabVm.ObjectId);

                        }
                        else if (Settings.GetInstance().ObjTabVm.SelectedStatistics.ObjectsNameTT.StartsWith(StatisticsEnum.SectionName.dn.ToString()))
                        {
                            Settings.GetInstance().ObjTabVm.NewObjectConfigVM.LoadObjectConfiguration(StatisticsEnum.ObjectType.DNGroup.ToString(), Settings.GetInstance().ObjTabVm.SelectedStatistics.ObjectsNameTT.ToString(), Settings.GetInstance().ObjTabVm.ObjectId);

                        }
                        else if (Settings.GetInstance().ObjTabVm.SelectedStatistics.ObjectsNameTT.StartsWith(StatisticsEnum.SectionName.vq.ToString()))
                        {
                            Settings.GetInstance().ObjTabVm.NewObjectConfigVM.LoadObjectConfiguration(StatisticsEnum.ObjectType.VirtualQueue.ToString(), Settings.GetInstance().ObjTabVm.SelectedStatistics.ObjectsNameTT.ToString(), Settings.GetInstance().ObjTabVm.ObjectId);

                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("MainWindowViewModel : LoadNewObjectConfigWindow Method : Exception caught " + generalException.Message.ToString());
            }
            finally
            {

            }
            logger.Debug("MainWindowViewModel : LoadNewObjectConfigWindow Method : Exit ");

        }
        #endregion

        #region LoadObjectConfigWindow
        private void LoadObjectConfigWindow()
        {
            try
            {
                logger.Debug("MainWindowViewModel : LoadObjectConfigWindow Method : Entry ");
                if (Settings.GetInstance().QueueConfigVM != null)
                {
                    Settings.GetInstance().QueueConfigVM.SwitchNames.Clear();
                    foreach (string key in objStatTicker.GetSwitches())
                    {
                        Settings.GetInstance().QueueConfigVM.SwitchNames.Add(key);
                    }
                    //Settings.GetInstance().ObjectConfigVM.SwitchNames = objStatTicker.GetSwitches();
                    List<string> SelectedStatList = new List<string>();
                    foreach (string str in Settings.GetInstance().QueueConfigVM.SelectedStatList)
                        SelectedStatList.Add(str);
                    Settings.GetInstance().QueueConfigVM.ConfiguredStatistics.Clear();
                    Settings.GetInstance().QueueConfigVM.LoadConfigStatistics();
                    Settings.GetInstance().DictApplicationObjects.Clear();
                    Settings.GetInstance().DictAgentGroupObjects.Clear();
                    Settings.GetInstance().DictAgentObjects.Clear();
                    Settings.GetInstance().DictApplicationObjects.Add(Settings.GetInstance().ApplicationName, new Dictionary<string, List<string>>(objStatTicker.GetExistingValues()));


                    //Settings.GetInstance().ObjectConfigVM.WinLoad();
                    Settings.GetInstance().QueueConfigVM.isFirstTime = true;
                    //Settings.GetInstance().ObjectConfigVM.ObjectTypeChanged(Settings.GetInstance().ObjectConfigVM.TypeObj);
                    foreach (string sectionName in SelectedStatList)
                    {
                        foreach (StatisticsProperties StatObj in Settings.GetInstance().QueueConfigVM.ConfiguredStatistics)
                        {
                            if (StatObj.SectionName == sectionName)
                            {
                                StatObj.isGridChecked = true;
                            }
                        }
                        Settings.GetInstance().QueueConfigVM.SelectedStatList.Add(sectionName);
                    }

                   // Settings.GetInstance().ObjectConfigVM.ReadUserLevelObjects(Settings.GetInstance().ObjectConfigVM.TypeObj.Text.ToString(), SelectedStatList);
                }


            }
            catch (Exception generalException)
            {
                logger.Error("MainWindowViewModel : LoadObjectConfigWindow Method : Exception caught " + generalException.Message.ToString());
            }
            finally
            {

            }
            logger.Debug("MainWindowViewModel : LoadObjectConfigWindow Method : Exit");
        }
        #endregion

        #region LoadUserLevelConfigWindow
        private void LoadUserLevelConfigWindow()
        {
            try
            {
                logger.Debug("MainWindowViewModel : LoadUserLevelConfigWindow Method : Entry");

                TempCheckedObjectList = new ObservableCollection<ObjectValues>();

                TabItem currentTab = null;
                int objectIndex = 0;                

                if (Settings.GetInstance().ObjTabVm != null)
                {
                    if (Settings.GetInstance().ObjTabVm.NewObjectConfigVM != null)
                    {
                        LoadNewObjectConfigWindow();
                    }

                }

            }
            catch (Exception generalException)
            {
                logger.Error("MainWindowViewModel : LoadUserLevelConfigWindow Method : Exception caught" + generalException.Message.ToString());
            }
            finally
            {

            }
            logger.Debug("MainWindowViewModel : LoadUserLevelConfigWindow Method : Exit");
        }
        #endregion

        #region LoadEditObjectConfigWindow
        private void LoadEditObjectConfigWindow(string objType)
        {
            try
            {
                logger.Debug("AdminMonitorViewModel : LoadEditObjectConfigWindow() method - Entry");

                List<string> SelectedStatList = new List<string>();
                foreach (string str in Settings.GetInstance().ObjTabVm.ObjectConfigVM.SelectedStatList)
                    SelectedStatList.Add(str);
                Settings.GetInstance().ObjTabVm.LstStatistics.Clear();
                if (objType == "AgentGroup")
                {
                    foreach (string sectionName in Settings.GetInstance().DictAgentGroupStatisitics[Settings.GetInstance().ObjTabVm.ObjectId])
                    {
                        if ((sectionName.StartsWith("acd")) || (sectionName.StartsWith("dn")) || (sectionName.StartsWith("vq")))
                        {
                            Settings.GetInstance().ObjTabVm.LstStatistics.Add(sectionName);
                        }
                    }
                }
                else if (objType == "Agent")
                {
                    foreach (string sectionName in Settings.GetInstance().DictAgentStatisitics[Settings.GetInstance().ObjTabVm.ObjectId])
                    {
                        if ((sectionName.StartsWith("acd")) || (sectionName.StartsWith("dn")) || (sectionName.StartsWith("vq")))
                        {
                            Settings.GetInstance().ObjTabVm.LstStatistics.Add(sectionName);
                        }
                    }

                }

                Settings.GetInstance().ObjTabVm.ObjectConfigVM.isFirstTime = true;
                Settings.GetInstance().ObjTabVm.ObjectConfigVM.LoadObjectConfiguration(Settings.GetInstance().ObjTabVm.LstStatistics, Settings.GetInstance().ObjTabVm.ObjectId);

                //Settings.GetInstance().ObjectConfigVM.ObjectTypeChanged(Settings.GetInstance().ObjectConfigVM.TypeObj);
                foreach (string sectionName in SelectedStatList)
                {
                    foreach (StatisticsProperties StatObj in Settings.GetInstance().ObjTabVm.ObjectConfigVM.ConfiguredStatistics)
                    {
                        if (StatObj.SectionName == sectionName)
                        {
                            StatObj.isGridChecked = true;
                            Settings.GetInstance().ObjTabVm.ObjectConfigVM.NewObjectChecked();
                        }
                    }
                    Settings.GetInstance().ObjTabVm.ObjectConfigVM.SelectedStatList.Add(sectionName);
                }



                //Settings.GetInstance().ObjTabVm.ObjectConfigVM.ReadUserLevelObjects(Settings.GetInstance().ObjTabVm.ObjectConfigVM.TypeObj.Text.ToString(), SelectedStatList);


            }
            catch (Exception generalException)
            {
                logger.Error("AdminMonitorViewModel : LoadEditObjectConfigWindow() method - Exception caught" + generalException.Message.ToString());
            }
            logger.Debug("AdminMonitorViewModel : LoadEditObjectConfigWindow() method - Exit");
        }
        #endregion

        #endregion

    }
}
