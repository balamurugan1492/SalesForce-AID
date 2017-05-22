using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StatTickerFive.Helpers;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Input;
using System.Windows;
using StatTickerFive.Views;
using Pointel.Statistics.Core.Utility;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace StatTickerFive.ViewModels
{
    public class StatisticsWindowViewModel : NotificationObject
    {
        #region Public Declarations

        public Window StatisticsPropertieswindow = null;
        public string Section = string.Empty;
        public string Statistics = string.Empty;
        public string EmptyField = "Enter a value here";
        public string DecimalFormat = "Value in decimal format";
        public string TimeFormat = "Value in Time format";
        public string ThresholdMessage2 = "Value Greater than Threshold2";
        public string ThresholdMessage1 = "Value less than Threshold2";
       
        StatisticsSupport objStatSupport = new StatisticsSupport();

        #endregion

        #region Constructor

        public StatisticsWindowViewModel(Window window, string SectionTitle,string statistics)
        {
            DataGridRowHeight = new GridLength(0);
            StatisticsPropertieswindow = window;
            Section = SectionTitle;
            Statistics = statistics;
            
            Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> dictTheme = new Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush>();
            dictTheme = objStatSupport.ThemeSelector(Settings.GetInstance().Theme);
            TitleBackground = dictTheme[StatisticsEnum.ThemeColors.TitleBackground];
            BackgroundColor = dictTheme[StatisticsEnum.ThemeColors.BackgroundColor];
            TitleForeground = dictTheme[StatisticsEnum.ThemeColors.TitleForeground];
            BorderBrush = dictTheme[StatisticsEnum.ThemeColors.BorderBrush];
            ShadowEffect = new DropShadowBitmapEffect();
            ShadowEffect.Color = (Color)BorderBrush.Color;            
        }

        public StatisticsWindowViewModel()
        {
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

        private string _sectionName;
        public string SectionName
        {
            get
            {
                return _sectionName;
            }
            set
            {
                if (value != null)
                {
                    _sectionName = value;
                    RaisePropertyChanged(() => SectionName);
                }
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                if (value != null)
                {
                    _displayName = value;
                    RaisePropertyChanged(() => DisplayName);
                }
            }
        }

        private string _statName;
        public string StatisticsName
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
                    RaisePropertyChanged(() => StatisticsName);
                }
            }
        }
        
        private List<string> _filterSource;
        public List<string> FilterSource
        {
            get
            {
                return _filterSource;
            }
            set
            {
                if (value != null)
                {
                    _filterSource = value;
                    RaisePropertyChanged(() => FilterSource);
                }
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
                RaisePropertyChanged(()=>SelectedSection);
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

        private List<string> _formatSource;
        public List<string> FormatSource
        {
            get
            {
                return _formatSource;
            }
            set
            {
                if (value != null)
                {
                    _formatSource = value;
                    RaisePropertyChanged(() => FormatSource);
                }
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


        #endregion

        #region Commands

        public ICommand ActivatedCommand { get { return new DelegateCommand(WinActivated); } }
        public ICommand DeactivateCommand { get { return new DelegateCommand(WinDeActivated); } }
        public ICommand WinLoadCommand { get { return new DelegateCommand(WinLoad); } }
        public ICommand DragCmd { get { return new DelegateCommand(DragMove); } }
       // public ICommand PropertySave { get { return new DelegateCommand(SaveServerEditbtnClicked); } }
        public ICommand PropertyClose { get { return new DelegateCommand(SaveServerCancelbtnClicked); } }
        public ICommand StatisticsSearch { get { return new DelegateCommand(SearchStatisticsbtnClicked); } }
        public ICommand DataGridLostFocusCommand { get { return new DelegateCommand(DataGridLostFocus); } }
        public ICommand DataGridRadiobtnCheckedCommand { get { return new DelegateCommand(DataGridRadiobtnChecked); } }
        public ICommand FormatTypeChanged { get { return new DelegateCommand(FormatTypeSelectionChanged); } }
        public ICommand IsSavebtnEnabledCommand { get { return new DelegateCommand(SavebtnMouseOver); } }
        public ICommand TextChangedCommand { get { return new DelegateCommand(TextChangedEvent); } }
        public ICommand SectionChanged { get { return new DelegateCommand(SectionChangedEvent); } }
        

        #endregion

        #region Methods

        /// <summary>
        /// Wins the activated.
        /// </summary>
        private void WinActivated()
        {
            try
            {
                //object obj = (ColorConverter.ConvertFromString("#0070C5"));
                //if (obj != null)
                //    BorderBrush = new SolidColorBrush((Color)obj);

                ShadowEffect = new DropShadowBitmapEffect();
                ShadowEffect.ShadowDepth = 0;
                ShadowEffect.Opacity = 0.5;
                ShadowEffect.Softness = 0.5;
                //object convertFromString = ColorConverter.ConvertFromString("#003660");
                //if (convertFromString != null)
                //    ShadowEffect.Color = (Color)convertFromString;


            }
            catch (Exception ex)
            {

            }
            finally
            {
                GC.Collect();
            }
        }

        /// <summary>
        /// Wins the de activated.
        /// </summary>
        private void WinDeActivated()
        {
            ShadowEffect.Opacity = 0;
        }

        /// <summary>
        /// Drags the move.
        /// </summary>
        private void DragMove(object obj)
        {
            foreach (Window currentwindow in System.Windows.Application.Current.Windows)
            {
                if (currentwindow.Title == obj.ToString())
                    currentwindow.DragMove();
            }
        }

        /// <summary>
        /// Wins the load.
        /// </summary>
        private void WinLoad()
        {
            WinActivated();

            IsSavebtnEnable = true;
            StatNameColSpan = 2;
            StatNameSearch = Visibility.Visible;
            Title = "Statistics Properties";
            
            FilterSource = new List<string>();
            FilterSource.Add("None");
            foreach (string Filter in Settings.GetInstance().DictServerFilters.Keys)
            {
                if (!FilterSource.Contains(Filter))
                    FilterSource.Add(Filter);
            }

            FormatSource = new List<string>();
            foreach (string Format in Settings.GetInstance().DictStatFormats.Keys)
            {
                if (!FormatSource.Contains(Format))
                    FormatSource.Add(Format + " " + Settings.GetInstance().DictStatFormats[Format]);
            }

            if (Settings.GetInstance().DictExistingApplicationStats.ContainsKey(Section))
            {
                Dictionary<string, string> statProperties = new Dictionary<string, string>();
                statProperties = Settings.GetInstance().DictExistingApplicationStats[Section];
                SelectedFilter = statProperties[StatisticsEnum.StatProperties.Filter.ToString()];
                SelectedFormat = statProperties[StatisticsEnum.StatProperties.Format.ToString()] + " " + Settings.GetInstance().DictStatFormats[statProperties[StatisticsEnum.StatProperties.Format.ToString()]];
                StatisticsColor = (System.Windows.Media.Color)ColorConverter.ConvertFromString(statProperties[StatisticsEnum.StatProperties.Color1.ToString()]);
                ThresholdColor1 = (System.Windows.Media.Color)ColorConverter.ConvertFromString(statProperties[StatisticsEnum.StatProperties.Color2.ToString()]);
                ThresholdColor2 = (System.Windows.Media.Color)ColorConverter.ConvertFromString(statProperties[StatisticsEnum.StatProperties.Color3.ToString()]);
            }
            else
            {
                StatisticsColor = (System.Windows.Media.Color)ColorConverter.ConvertFromString("Black");
                ThresholdColor1 = (System.Windows.Media.Color)ColorConverter.ConvertFromString("Green");
                ThresholdColor2 = (System.Windows.Media.Color)ColorConverter.ConvertFromString("Red");
            }


            Sections = new System.Collections.Generic.List<string>();
            Sections.Add(StatisticsEnum.SectionName.acd.ToString());
            Sections.Add(StatisticsEnum.SectionName.agent.ToString());
            Sections.Add(StatisticsEnum.SectionName.group.ToString());
            Sections.Add(StatisticsEnum.SectionName.dn.ToString());
            Sections.Add(StatisticsEnum.SectionName.vq.ToString());

            if (!string.IsNullOrEmpty(SectionName))
            {
                string[] SectionTitle = SectionName.Split('-');
                if (Sections.Contains(SectionTitle[0].ToString().ToLower()))
                {
                    SelectedSection = SectionTitle[0].ToString().ToLower();
                    SectionName = SectionName.Replace(SectionTitle[0].ToString().ToLower() + "-", "");
                }
                SectionChangedEvent(SelectedSection);
            }
            else
            {
                switch (objStatSupport.GetSupportedObject(Section))
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


            if (string.IsNullOrEmpty(SelectedFilter))
            {
                SelectedFilter = FilterSource[0];
            }
            if (string.IsNullOrEmpty(SelectedFormat))
            {
                SelectedFormat = FormatSource[0];
                Settings.GetInstance().SelectedFormat = SelectedFormat;
            }
            else
            {
                Settings.GetInstance().SelectedFormat = SelectedFormat;
            }

            KeyToDifferentiateStyles = "1";
            Settings.GetInstance().TValue1 = Threshold1;
            Settings.GetInstance().TValue2 = Threshold2;

        }

        #region Section Type changed

        public void SectionChangedEvent(object obj)
        {
            try
            {
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

                foreach (KeyValuePair<string, string> item in Settings.GetInstance().DictStatisticsDesc)
                {
                    string tempStatName = string.Empty;
                    string tempStatDesc = string.Empty;
                    tempStatName = item.Key.ToString().Length > 17 ? item.Key.ToString().Substring(0, 16) + ".." : item.Key.ToString();

                    tempStatDesc = item.Value.ToString().Length > 36 ? item.Value.ToString().Substring(0, 35) + ".." : item.Value.ToString();

                    SearchStatnameCollection.Add(new SearchedStatistics() { SearchedStatname = tempStatName, SearchedStatDescription = tempStatDesc, SearchedStatnameTooltip = item.Key });
                }
                
            }
            catch (Exception GeneralException)
            {
            }
        }

        #endregion


        //#region SaveServerEditbtnClicked

        ///// <summary>
        ///// Saves the server editbtn clicked.
        ///// </summary>
        ///// <param name="obj">The obj.</param>
        //public void SaveServerEditbtnClicked(object obj)
        //{
        //    bool isConfigurationsCorrect = false;
        //    bool isStatAlreadyConfigured = false;
        //    try
        //    {
        //        var values = (object[])obj;
        //        string sectionName = values[1].ToString() + "-" + values[0].ToString();
        //        string statisticsName = values[2].ToString();
        //        //string[] section = sectionName.Split('-');

        //        #region Store Configured Stats

        //        if (sectionName.StartsWith("agent"))
        //        {
        //            if (Settings.GetInstance().ListAgentStatistics.Contains(statisticsName))
        //                isConfigurationsCorrect = true;
        //        }
        //        else if (sectionName.StartsWith("group"))
        //        {
        //            if (Settings.GetInstance().ListAgentGroupStatistics.Contains(statisticsName))
        //                isConfigurationsCorrect = true;
        //        }
        //        else if (sectionName.StartsWith("acd"))
        //        {
        //            if (Settings.GetInstance().ListACDQueueStatistics.Contains(statisticsName))
        //                isConfigurationsCorrect = true;
        //        }
        //        else if (sectionName.StartsWith("vq"))
        //        {
        //            if (Settings.GetInstance().ListVirtualQueueStatistics.Contains(statisticsName))
        //                isConfigurationsCorrect = true;
        //        }
        //        else if (sectionName.StartsWith("dn"))
        //        {
        //            if (Settings.GetInstance().ListGroupQueueStatistics.Contains(statisticsName))
        //                isConfigurationsCorrect = true;
        //        }

        //        if (isConfigurationsCorrect)
        //        {
        //            Dictionary<string, string> dictTempStatProperty = new System.Collections.Generic.Dictionary<string, string>();

        //            dictTempStatProperty.Add(StatisticsEnum.StatProperties.DisplayName.ToString(), DisplayName);
        //            dictTempStatProperty.Add(StatisticsEnum.StatProperties.StatName.ToString(), StatisticsName);
        //            dictTempStatProperty.Add(StatisticsEnum.StatProperties.TooltipName.ToString(), TooltipValue);
        //            dictTempStatProperty.Add(StatisticsEnum.StatProperties.ThresLevel1.ToString(), Threshold1);
        //            dictTempStatProperty.Add(StatisticsEnum.StatProperties.ThresLevel2.ToString(), Threshold2);
        //            dictTempStatProperty.Add(StatisticsEnum.StatProperties.Color1.ToString(), StatisticsColor.ToString());
        //            dictTempStatProperty.Add(StatisticsEnum.StatProperties.Color2.ToString(), ThresholdColor1.ToString());
        //            dictTempStatProperty.Add(StatisticsEnum.StatProperties.Color3.ToString(), ThresholdColor2.ToString());

        //            if (SelectedFilter == "None" || SelectedFilter == string.Empty)
        //            {
        //                SelectedFilter = "";
        //            }
        //            dictTempStatProperty.Add(StatisticsEnum.StatProperties.Filter.ToString(), SelectedFilter);

        //            if (SelectedFormat != string.Empty && SelectedFormat != "")
        //            {
        //                string[] formatSelected = SelectedFormat.Split('(');
        //                dictTempStatProperty.Add(StatisticsEnum.StatProperties.Format.ToString(), formatSelected[0].ToString().Trim());
        //            }

        //            if (Settings.GetInstance().IsExistingStat && statisticsName==Statistics)
        //            {
        //                if (Settings.GetInstance().LstConfiguredAgentStats.Contains(statisticsName))
        //                    Settings.GetInstance().LstConfiguredAgentStats.Remove(statisticsName);
        //                else if (Settings.GetInstance().LstConfiguredAGroupStats.Contains(statisticsName))
        //                    Settings.GetInstance().LstConfiguredAGroupStats.Remove(statisticsName);
        //                else if (Settings.GetInstance().LstConfiguredACDStats.Contains(statisticsName))
        //                    Settings.GetInstance().LstConfiguredACDStats.Remove(statisticsName);
        //                else if (Settings.GetInstance().LstConfiguredDNStats.Contains(statisticsName))
        //                    Settings.GetInstance().LstConfiguredDNStats.Remove(statisticsName);
        //                else if (Settings.GetInstance().LstConfiguredVQStats.Contains(statisticsName))
        //                    Settings.GetInstance().LstConfiguredVQStats.Remove(statisticsName);

        //                if (Settings.GetInstance().DictConfigStats.ContainsKey(Section))
        //                    Settings.GetInstance().DictConfigStats.Remove(Section);

        //                    foreach (StatisticsProperties StatProp in Settings.GetInstance().adminConfigVM.ConfiguredStatistics)
        //                    {
        //                        if (StatProp.SectionName == Section)
        //                        {
        //                            Settings.GetInstance().adminConfigVM.ConfiguredStatistics.Remove(StatProp);
        //                            break;
        //                        }
        //                    }
        //            }


        //            if (sectionName.StartsWith("agent"))
        //            {
        //                if (Settings.GetInstance().LstConfiguredAgentStats.Contains(statisticsName))
        //                    isStatAlreadyConfigured = true;
        //            }

        //            if (sectionName.StartsWith("group"))
        //            {
        //                if (Settings.GetInstance().LstConfiguredAGroupStats.Contains(statisticsName))
        //                    isStatAlreadyConfigured = true;
        //            }

        //            if (sectionName.StartsWith("acd"))
        //            {
        //                if (Settings.GetInstance().LstConfiguredACDStats.Contains(statisticsName))
        //                    isStatAlreadyConfigured = true;
        //            }

        //            if (sectionName.StartsWith("dn"))
        //            {
        //                if (Settings.GetInstance().LstConfiguredDNStats.Contains(statisticsName))
        //                    isStatAlreadyConfigured = true;
        //            }

        //            if (sectionName.StartsWith("vq"))
        //            {
        //                if (Settings.GetInstance().LstConfiguredVQStats.Contains(statisticsName))
        //                    isStatAlreadyConfigured = true;
        //            }

        //            if (!isStatAlreadyConfigured)
        //            {
        //                if (!Settings.GetInstance().DictConfigStats.ContainsKey(sectionName) || statisticsName!= Statistics)
        //                {

        //                    Settings.GetInstance().DictConfigStats.Add(sectionName, dictTempStatProperty);

        //                    if (sectionName.StartsWith("agent"))
        //                        if (!Settings.GetInstance().LstConfiguredAgentStats.Contains(statisticsName))
        //                            Settings.GetInstance().LstConfiguredAgentStats.Add(statisticsName);

        //                    if (sectionName.StartsWith("group"))
        //                        if (!Settings.GetInstance().LstConfiguredAGroupStats.Contains(statisticsName))
        //                            Settings.GetInstance().LstConfiguredAGroupStats.Add(statisticsName);

        //                    if (sectionName.StartsWith("acd"))
        //                        if (!Settings.GetInstance().LstConfiguredACDStats.Contains(statisticsName))
        //                            Settings.GetInstance().LstConfiguredACDStats.Add(statisticsName);

        //                    if (sectionName.StartsWith("dn"))
        //                        if (!Settings.GetInstance().LstConfiguredDNStats.Contains(statisticsName))
        //                            Settings.GetInstance().LstConfiguredDNStats.Add(statisticsName);

        //                    if (sectionName.StartsWith("vq"))
        //                        if (!Settings.GetInstance().LstConfiguredVQStats.Contains(statisticsName))
        //                            Settings.GetInstance().LstConfiguredVQStats.Add(statisticsName);


        //                    objStatSupport.CheckStaticsticsObject(statisticsName);

        //                    if (Settings.GetInstance().IsExistingStat)
        //                    {
        //                        Settings.GetInstance().adminConfigVM.ConfiguredStatistics.Add(new StatisticsProperties() { isGridChecked = true, SectionName = sectionName, DisplayName = DisplayName, EditName = "Edit", IsExistProperty = true });

        //                        if (Settings.GetInstance().DictExistingApplicationStats.ContainsKey(Section))
        //                        {
        //                            Settings.GetInstance().DictExistingApplicationStats.Remove(Section);
        //                        }
        //                        Settings.GetInstance().DictExistingApplicationStats.Add(sectionName, dictTempStatProperty);

        //                        for (int item = 0; item < Settings.GetInstance().adminConfigVM.ServerStatistics.Count; item++)
        //                        {
        //                            if (string.Compare(Settings.GetInstance().adminConfigVM.ServerStatistics[item].SectionName.Trim(), sectionName.Trim(), true) == 0)
        //                            {
        //                                Settings.GetInstance().adminConfigVM.ServerStatistics.RemoveAt(item);
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {

        //                        if (!Settings.GetInstance().IsAgentRemain && !Settings.GetInstance().IsAGroupRemain &&
        //                            !Settings.GetInstance().IsACDRemain && !Settings.GetInstance().IsDNRemain && !Settings.GetInstance().IsVQRemain)
        //                        {
        //                            for (int item = 0; item < Settings.GetInstance().adminConfigVM.ServerStatistics.Count; item++)
        //                            {
        //                                if (string.Compare(Settings.GetInstance().adminConfigVM.ServerStatistics[item].SectionName.Trim(), Section.Trim(), true) == 0)
        //                                {
        //                                    Settings.GetInstance().adminConfigVM.ConfiguredStatistics.Add(new StatisticsProperties() { isGridChecked = true, SectionName = sectionName, DisplayName = DisplayName, EditName = "Edit", IsExistProperty = true });
        //                                    if (!Settings.GetInstance().DictExistingApplicationStats.ContainsKey(SectionName))
        //                                    {
        //                                        Settings.GetInstance().DictExistingApplicationStats.Add(sectionName, dictTempStatProperty);
        //                                    }
        //                                    Settings.GetInstance().adminConfigVM.ServerStatistics.RemoveAt(item);
        //                                }
        //                            }
        //                        }
        //                        else
        //                        {
        //                            for (int item = 0; item < Settings.GetInstance().adminConfigVM.ServerStatistics.Count; item++)
        //                            {
        //                                if (string.Compare(Settings.GetInstance().adminConfigVM.ServerStatistics[item].SectionName.Trim(), Section.Trim(), true) == 0)
        //                                {
        //                                    Settings.GetInstance().adminConfigVM.ConfiguredStatistics.Add(new StatisticsProperties() { isGridChecked = true, SectionName = sectionName, DisplayName = DisplayName, EditName = "Edit", IsExistProperty = true });

        //                                    if (!Settings.GetInstance().DictExistingApplicationStats.ContainsKey(SectionName))
        //                                    {
        //                                        Settings.GetInstance().DictExistingApplicationStats.Add(sectionName, dictTempStatProperty);
        //                                    }

        //                                    Settings.GetInstance().adminConfigVM.ServerStatistics[item].isGridChecked = false;
        //                                }
        //                            }
        //                        }
        //                    }
        //                    StatisticsPropertieswindow.Close();
        //                }
        //                else
        //                {
        //                    Views.MessageBox msgbox;
        //                    ViewModels.MessageBoxViewModel mboxvmodel;
        //                    msgbox = new Views.MessageBox();
        //                    mboxvmodel = new MessageBoxViewModel("Information", "Provided Section name already exist. Please provide a different section name.", msgbox, "MainWindow", Settings.GetInstance().Theme);
        //                    msgbox.DataContext = mboxvmodel;
        //                    msgbox.ShowDialog();
        //                }
        //            }
        //            else
        //            {
        //                Views.MessageBox msgbox;
        //                ViewModels.MessageBoxViewModel mboxvmodel;
        //                msgbox = new Views.MessageBox();
        //                mboxvmodel = new MessageBoxViewModel("Information", "Provided statistics already configured. Please provide different object (or) different statistics", msgbox, "MainWindow", Settings.GetInstance().Theme);
        //                msgbox.DataContext = mboxvmodel;
        //                msgbox.ShowDialog();
        //            }
        //        }
        //        else
        //        {
        //            Views.MessageBox msgbox;
        //            ViewModels.MessageBoxViewModel mboxvmodel;
        //            msgbox = new Views.MessageBox();
        //            mboxvmodel = new MessageBoxViewModel("Information", "Provided Section name doesn't match the object of the configured statistics. Please give the correct section name.", msgbox, "MainWindow", Settings.GetInstance().Theme);
        //            msgbox.DataContext = mboxvmodel;
        //            msgbox.ShowDialog();
        //        }

        //        #endregion
        //        //}
        //    }
        //    catch (Exception GeneraLException)
        //    {
        //    }
        //}

        //#endregion

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
                foreach (Window win in Application.Current.Windows)
                {
                    if (win.Title == "StatisticsPropertiesWindow")
                    {
                        win.Close();
                    }
                }
                //}
            }
            catch (Exception GeneralException)
            {
            }
        }
        #endregion

        #region SearchStatisticsbtnClicked

        private void SearchStatisticsbtnClicked()
        {
            DataGridRowHeight = new GridLength(200);
        }
        #endregion

        #region DataGridLostFocus

        private void DataGridLostFocus(object obj)
        {
            //if (obj != null)
            //{
            //    SearchedStatistics statObj = obj as SearchedStatistics;

            //    if (!string.IsNullOrEmpty(statObj.SearchedStatname))
            //    {
            //        DataGridRowHeight = new GridLength(0);

            //    }
            //}
            DataGridRowHeight = new GridLength(0);

        }
        #endregion

        #region DataGridRadiobtnChecked

        private void DataGridRadiobtnChecked(object obj)
        {
            if (obj != null)
            {
                StatisticsName = obj.ToString();
            }
        }
        #endregion

        #region FormatTypeSelectionChanged

        private void FormatTypeSelectionChanged(object obj)
        {
            Settings.GetInstance().SelectedFormat = obj.ToString();
            Threshold1 = EmptyField;
            Threshold2 = EmptyField;

        }

        #endregion

        #region SavebtnMouseOver

        private void SavebtnMouseOver()
        {
            if ((SectionName == EmptyField || SectionName.Trim().ToString() == string.Empty) || (DisplayName == EmptyField || DisplayName.Trim().ToString() == string.Empty) || (StatisticsName == EmptyField || StatisticsName.Trim().ToString() == string.Empty)
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

        #endregion

        #region TextChangedEvent

        private void TextChangedEvent()
        {
            if ((SectionName == EmptyField || SectionName.Trim().ToString() == string.Empty) || (DisplayName == EmptyField || DisplayName.Trim().ToString() == string.Empty) || (StatisticsName == EmptyField || StatisticsName.Trim().ToString() == string.Empty)
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
            Settings.GetInstance().TValue1 = Threshold1;
            Settings.GetInstance().TValue2 = Threshold2;
        }

        #endregion

        #endregion
    }
}
