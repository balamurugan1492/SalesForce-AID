using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StatTickerFive.Helpers;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Lucene.Net.QueryParsers;
using StatTickerFive.UserControls.Views;
using System.Windows.Media;
using Pointel.Statistics.Core.Utility;
using System.Windows.Media.Effects;
using System.Windows;
using Pointel.Statistics.Core;
using Pointel.Logger.Core;

namespace StatTickerFive.ViewModels
{
    public class UserLevelStatisticsConfigViewModel : NotificationObject
    {
        #region Declaration
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        StatisticsSearch objStatSearch = new StatisticsSearch();
        StatisticsSupport objStatSupport = new StatisticsSupport();
        StatisticsBase objStatBase = new StatisticsBase();
       // string Uid = string.Empty;
        bool isFirstTime = false;
        List<string> lstTemp;
        bool isFirstSelected = true;
        int MaxTags = 0;
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
                RaisePropertyChanged(()=>SearchValue);
            }
        }

        private ObservableCollection<ObjectValues> _ObjectCollection;
        public ObservableCollection<ObjectValues> ObjectCollection
        {
            get { return _ObjectCollection; }
            set
            {
                _ObjectCollection = value;
                RaisePropertyChanged(() => ObjectCollection);
            }
        }

        private ObservableCollection<TabItem> _TabValues;
        public ObservableCollection<TabItem> TabValues
        {
            get { return _TabValues; }
            set
            {
                _TabValues = value;
                RaisePropertyChanged(() => TabValues);
            }
        }

        private Visibility _GridVisible;
        public Visibility GridVisible
        {
            get
            {
                return _GridVisible;
            }
            set
            {
                _GridVisible = value;
                RaisePropertyChanged(() => GridVisible);
            }
        }

        private int _ObjectIndex;
        public int ObjectIndex
        {
            get
            {
                return _ObjectIndex;
            }
            set
            {
                _ObjectIndex = value;
                RaisePropertyChanged(() => ObjectIndex);
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
        #endregion

        #region Command
        public ICommand ActivatedCommand { get { return new DelegateCommand(WinActivated); } }
        public ICommand DeactivateCommand { get { return new DelegateCommand(WinDeActivated); } }
        public ICommand WinLoadCommand { get { return new DelegateCommand(WinLoad); } }
        public ICommand DragCmd { get { return new DelegateCommand(DragMove); } }
        public ICommand SearchObjectCommand { get { return new DelegateCommand(SearchObject); } }
        public ICommand TypeSelected { get { return new DelegateCommand(ObjectTypeSelected); } }
        public ICommand ObjectSelectedCommand { get { return new DelegateCommand(ObjectSelected); } }
        public ICommand ObjectDeselectedCommand { get { return new DelegateCommand(ObjectDeselected); } }
        public ICommand ConfigCancelCmd { get { return new DelegateCommand(ConfigCancel); } }
        public ICommand SaveStatistics { get { return new DelegateCommand(SaveStatisticsConfiguration); } }
        #endregion

        #region Constructor
        public UserLevelStatisticsConfigViewModel()
        {
            try
            {
                logger.Debug("UserLevelStatisticsConfigViewModel : Constructor - Entry");
                ObjectCollection = new ObservableCollection<ObjectValues>();

                TabValues = new ObservableCollection<TabItem>();

                Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> dictTheme = new Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush>();
                dictTheme = objStatSupport.ThemeSelector(Settings.GetInstance().Theme);

                TitleBackground = dictTheme[StatisticsEnum.ThemeColors.TitleBackground];
                BackgroundColor = dictTheme[StatisticsEnum.ThemeColors.BackgroundColor];
                TitleForeground = dictTheme[StatisticsEnum.ThemeColors.TitleForeground];
                BorderBrush = dictTheme[StatisticsEnum.ThemeColors.BorderBrush];
                ShadowEffect = new DropShadowBitmapEffect();
                ShadowEffect.Color = (Color)BorderBrush.Color;

                foreach (Window currentwindow in System.Windows.Application.Current.Windows)
                {
                    if (currentwindow.Title == "ObjectConfigurations")
                        currentwindow.Close();
                }

                Settings.GetInstance().DictAgentStatisitics = objStatBase.GetAgentValues();
                Settings.GetInstance().DictAgentObjects = objStatBase.GetAgentObjects();

                Settings.GetInstance().DictAgentGroupStatisitics = objStatBase.GetAgentGroupValues();
                Settings.GetInstance().DictAgentGroupObjects = objStatBase.GetAgentGroupObjects();

                MaxTags = objStatBase.GetMaxTabs();
                ObjectIndex = 0;
                GridWidth = new GridLength(0);
                TitleSpan = 1;

                GridVisible = Visibility.Visible;
                logger.Debug("UserLevelStatisticsConfigViewModel : Constructor - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("UserLevelStatisticsConfigViewModel : Constructor - Exception caught" + generalException.Message.ToString());
            }
        }
        #endregion

        #region Method

        #region WinActivated
        public void WinActivated()
        {
            try
            {
                ShadowEffect = new DropShadowBitmapEffect();
                ShadowEffect.ShadowDepth = 0;
                ShadowEffect.Opacity = 0.5;
                ShadowEffect.Softness = 0.5;
            }
            catch (Exception ex)
            {

            }
            finally
            {
                GC.Collect();
            }
        }
        #endregion

        #region WinDeActivated
        public void WinDeActivated()
        {
            try
            {
                logger.Debug("UserLevelStatisticsConfigViewModel : WinActivated Method() - Entry");
                ShadowEffect.Opacity = 0;
                logger.Debug("UserLevelStatisticsConfigViewModel : WinActivated Method() - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("UserLevelStatisticsConfigViewModel : WinActivated Method() - Exception caught" + generalException.Message.ToString());
            }

        }
        #endregion

        #region WinLoad
        public void WinLoad()
        {
            try
            {

            }
            catch (Exception GeneralException)
            {
            }
        }
        #endregion

        #region DragMove
        public void DragMove(object obj)
        {
            try
            {
                logger.Debug("UserLevelStatisticsConfigViewModel : DragMove Method() - Entry");
                foreach (Window currentwindow in System.Windows.Application.Current.Windows)
                {
                    if (currentwindow.Title == obj.ToString())
                        currentwindow.DragMove();
                }
                logger.Debug("UserLevelStatisticsConfigViewModel : DragMove Method() - Exit");
            }
            catch (Exception GeneralException)
            {
                logger.Error("UserLevelStatisticsConfigViewModel : DragMove Method() - Exception caught" + GeneralException.Message.ToString());
            }
        }
        #endregion

        #region ConfigCancel
        public void ConfigCancel()
        {

        }
        #endregion

        #region ObjectTypeSelected
        public void ObjectTypeSelected(object obj)
        {
            try
            {
                //ObjectCollection = new ObservableCollection<ObjectValues>();
                //selectedType=obj.ToString();
                //lstTemp = new List<string>();
                //lstTemp = statSearch.Search(SearchValue, obj.ToString());

                //if (lstTemp.Count != 0)
                //{
                //    foreach(string objectName in lstTemp )
                //    {
                //        ObjectCollection.Add(new ObjectValues() { ObjectName = objectName });
                //    }
                //}
            }
            catch (Exception GeneralException)
            {

            }
        }
        #endregion

        #region SearchObject
        public void SearchObject()
        {
            try
            {
                logger.Debug("UserLevelStatisticsConfigViewModel : SearchObject Method() - Entry");
                if (!string.IsNullOrEmpty(SearchValue))
                {
                    ObjectCollection = new ObservableCollection<ObjectValues>();

                    // GridVisible = Visibility.Visible;

                    lstTemp = new List<string>();
                    lstTemp = objStatSearch.Search(SearchValue);

                    if (lstTemp.Count != 0)
                    {
                        foreach (string objectName in lstTemp)
                        {
                            string[] values = objectName.Split(',');
                            if (values.Length > 1)
                            {
                                if (TabValues.Count != 0)
                                {
                                    bool isTabExist = false;
                                    foreach (TabItem tab in TabValues)
                                    {
                                        if (tab.Header.ToString() == values[0].ToString())
                                        {
                                            isTabExist = true;
                                            break;
                                        }
                                    }

                                    if (!isTabExist)
                                        ObjectCollection.Add(new ObjectValues() { isGridChecked = false, ObjectName = values[0].ToString(), UniqueId = values[1].ToString() });
                                    else
                                        ObjectCollection.Add(new ObjectValues() { isGridChecked = true, ObjectName = values[0].ToString(), UniqueId = values[1].ToString() });
                                }
                                else
                                {
                                    ObjectCollection.Add(new ObjectValues() { isGridChecked = false, ObjectName = values[0].ToString(), UniqueId = values[1].ToString() });
                                }
                            }
                            else
                            {
                                ObjectCollection.Add(new ObjectValues() { isGridChecked = false, ObjectName = values[0].ToString(), UniqueId = values[0].ToString() });
                            }
                        }
                    }
                }
                else
                {

                    //GridVisible = Visibility.Collapsed;
                    ObjectCollection.Clear();
                    //TabValues.Clear();
                    ObjectIndex = 0;
                    isFirstSelected = true;
                }
                logger.Debug("UserLevelStatisticsConfigViewModel : SearchObject Method() - Exit");
            }
            catch (Exception GeneralException)
            {
                logger.Error("UserLevelStatisticsConfigViewModel : SearchObject Method() - Exception caught" + GeneralException.Message.ToString());
            }
        }
        #endregion

        #region ObjectSelected
        public void ObjectSelected(object obj)
        {
            try
            {
                if (GridWidth == new GridLength(0))
                {
                    GridWidth = new GridLength(500);
                    TitleSpan = 2;
                }

                ObjectValues objval = obj as ObjectValues;
                
                if (TabValues.Count < MaxTags)
                {

                    TabItem temptab = new TabItem();
                    temptab.Header = objval.ObjectName;
                    temptab.Tag = objval.UniqueId;
                    temptab.MouseLeftButtonUp += new MouseButtonEventHandler(temptab_MouseLeftButtonUp);
                    temptab.Unloaded += new RoutedEventHandler(temptab_Unloaded);
                    TabValues.Add(temptab);

                    if (isFirstSelected)
                    {
                        temptab_MouseLeftButtonUp(temptab, null);
                        isFirstSelected = false;
                    }
                }
                else
                {
                    Views.MessageBox msgbox;
                    ViewModels.MessageBoxViewModel mboxvmodel;
                    msgbox = new Views.MessageBox();
                    mboxvmodel = new MessageBoxViewModel("Information", "Maximum number of tabs Exceeded. Please Deselect some objects to add more.", msgbox, "MainWindow", Settings.GetInstance().Theme);
                    msgbox.DataContext = mboxvmodel;
                    msgbox.ShowDialog();
                    objval.isGridChecked = false;
                }
            }
            catch (Exception GeneralException)
            {

            }
        }
        #endregion

        #region temptab_Unloaded
        void temptab_Unloaded(object sender, RoutedEventArgs e)
        {
            try
            {
                ObjectIndex = 0;
                temptab_MouseLeftButtonUp(TabValues[0],null);
            }
            catch (Exception GeneralException)
            {

            }
        }
        #endregion 

        #region ObjectDeselected
        public void ObjectDeselected(object obj)
        {
            try
            {
                ObjectValues objval = obj as ObjectValues;

                foreach (TabItem currentTab in TabValues)
                {
                    if (currentTab.Tag.ToString() == objval.UniqueId)
                    {
                        TabValues.Remove(currentTab);
                        break;
                    }
                }

                if (TabValues.Count == 0)
                {
                    GridWidth = new GridLength(0);
                    TitleSpan = 1;
                }
            }
            catch (Exception GeneralException)
            {

            }
        }
        #endregion

        #region temptab_MouseLeftButtonUp
        void temptab_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                //if (isFirstTime)
                //{
                //    Views.MessageBox msgbox;
                //    ViewModels.MessageBoxViewModel mboxvmodel;
                //    msgbox = new Views.MessageBox();
                //    mboxvmodel = new MessageBoxViewModel("Information", "Do you want to save the configurations ?", msgbox, "ObjectChanged", Settings.GetInstance().Theme);
                //    msgbox.DataContext = mboxvmodel;
                //    msgbox.ShowDialog();

                //    if (Settings.GetInstance().IsSaveConfiguredObjs)
                //    {
                //        Settings.GetInstance().ObjTabVm.SaveNewStatistics();
                //    }
                //}
                //isFirstTime = true;

                logger.Debug("UserLevelStatisticsConfigViewModel : temptab_MouseLeftButtonUp Method() - Entry");
                
                TabItem currentTab = sender as TabItem;

                TabValue tab = new TabValue();
                //Settings.GetInstance().ObjTabVm = new TabValueViewModel(currentTab.Tag.ToString());
                tab.DataContext = Settings.GetInstance().ObjTabVm;
                currentTab.Content = tab;
                logger.Debug("UserLevelStatisticsConfigViewModel : temptab_MouseLeftButtonUp Method() - Exit");
            }
            catch (Exception GeneralException)
            {
                logger.Error("UserLevelStatisticsConfigViewModel : temptab_MouseLeftButtonUp Method() - Exception caught" + GeneralException.Message.ToString());
            }
        }
        #endregion

        #region SaveStatisticsConfiguration
        public void SaveStatisticsConfiguration()
        {
            try
            {
                logger.Debug("UserLevelStatisticsConfigViewModel : SaveStatisticsConfiguration Method() - Entry");
                foreach (TabItem currentTab in TabValues)
                {
                    //Settings.GetInstance().ObjTabVm = new TabValueViewModel(currentTab.Tag.ToString());
                    Settings.GetInstance().ObjTabVm.SaveObjectValues();
                }
                logger.Debug("UserLevelStatisticsConfigViewModel : SaveStatisticsConfiguration Method - Exit");
            }
            catch (Exception generalException)
            {
                logger.Error("UserLevelStatisticsConfigViewModel : SaveStatisticsConfiguration Method() - Exception caught" + generalException.Message.ToString());
            }

        }
        #endregion
        
        #endregion
    }
}
