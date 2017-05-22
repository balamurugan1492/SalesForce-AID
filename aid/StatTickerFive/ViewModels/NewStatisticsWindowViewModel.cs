using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Windows.Media.Effects;
using StatTickerFive.Helpers;
using Pointel.Statistics.Core.Utility;
using System.Windows.Input;
using System.Windows;
using System.Collections.ObjectModel;
using StatTickerFive.Views;

namespace StatTickerFive.ViewModels
{
    public class NewStatisticsWindowViewModel:NotificationObject
    {

        StatisticsSupport objStatSupport = new StatisticsSupport();
        string ObjectId = string.Empty;

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

        private ObservableCollection<ObjectValues> _ObjectStatistics;
        public ObservableCollection<ObjectValues> ObjectStatistics
        {
            get
            {
                return _ObjectStatistics;
            }
            set
            {
                _ObjectStatistics = value;
                RaisePropertyChanged(() => ObjectStatistics);
            }
        }

        public ICommand DragCmd { get { return new DelegateCommand(WinDrag); } }
        public ICommand CloseCmd { get { return new DelegateCommand(WinClose); } }
        public ICommand SaveCmd { get { return new DelegateCommand(SaveNewStatistics); } }     
        

        public NewStatisticsWindowViewModel(string Uid)
        {
            try
            {
                ObjectStatistics = new ObservableCollection<ObjectValues>();
                Settings.GetInstance().LstNewStatistics = new List<string>();
                ObjectId = Uid;

                Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> dictTheme = new Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush>();
                dictTheme = objStatSupport.ThemeSelector(Settings.GetInstance().Theme);

                TitleBackground = dictTheme[StatisticsEnum.ThemeColors.TitleBackground];
                BackgroundColor = dictTheme[StatisticsEnum.ThemeColors.BackgroundColor];
                TitleForeground = dictTheme[StatisticsEnum.ThemeColors.TitleForeground];
                BorderBrush = dictTheme[StatisticsEnum.ThemeColors.BorderBrush];
               
                List<string> LstStatistics;

                if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(Uid))
                {
                    LstStatistics = new List<string>();
                    LstStatistics = Settings.GetInstance().DictAgentStatisitics[Uid];

                    foreach (string stat in Settings.GetInstance().DictExistingApplicationStats.Keys)
                    {
                        if (!LstStatistics.Contains(stat))
                        {
                            ObjectStatistics.Add(new ObjectValues() { isGridChecked = false, ObjectName = stat, ObjectDescription = objStatSupport.GetDescription(stat) });
                        }
                    }
                }

                if (Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(Uid))
                {
                    LstStatistics = new List<string>();
                    LstStatistics = Settings.GetInstance().DictAgentGroupStatisitics[Uid];

                    foreach (string stat in Settings.GetInstance().DictExistingApplicationStats.Keys)
                    {
                        if (!LstStatistics.Contains(stat))
                        {
                            ObjectStatistics.Add(new ObjectValues() { isGridChecked = false, ObjectName = stat, ObjectDescription = objStatSupport.GetDescription(stat) });
                        }
                    }
                }


            }
            catch (Exception GeneralException)
            {
            }
        }

        public void WinDrag(object obj)
        {
            try
            {
                foreach (Window current in Application.Current.Windows)
                {
                    if(current.Title==obj.ToString())
                    {
                        current.DragMove();
                    }
                }
            }
            catch (Exception GeneralException)
            {
            }
        }

        public void WinClose(object obj)
        {
            try
            {
                foreach (Window current in Application.Current.Windows)
                {
                    if (current.Title == obj.ToString())
                    {
                        current.Close();
                    }
                }
            }
            catch (Exception GeneralException)
            {
            }
        }      
        
        public void SaveNewStatistics(object obj)
        {
            List<string> lstUpdatedStats = new List<string>();
            try
            {
                foreach(string newStats in Settings.GetInstance().LstNewStatistics)
                {
                    lstUpdatedStats.Add(newStats);
                    Settings.GetInstance().ObjTabVm.ObjectStatistics.Add(new ObjectValues() { isGridChecked = true, ObjectName = newStats, ObjectDescription = objStatSupport.GetDescription(newStats) });

                    if (Settings.GetInstance().DictAgentStatisitics.ContainsKey(ObjectId))
                    {
                        List<string> LstStatistics = new List<string>();
                        LstStatistics = Settings.GetInstance().DictAgentStatisitics[ObjectId];

                        if (!LstStatistics.Contains(newStats))
                        {
                            LstStatistics.Add(newStats);
                        }
                    }

                    if (Settings.GetInstance().DictAgentGroupStatisitics.ContainsKey(ObjectId))
                    {
                        List<string> LstStatistics = new List<string>();
                        LstStatistics = Settings.GetInstance().DictAgentGroupStatisitics[ObjectId];

                        if (!LstStatistics.Contains(newStats))
                        {
                            LstStatistics.Add(newStats);
                        }
                    }
                }

                //Settings.GetInstance().DictUpdatedStatistics.Add(ObjectId,lstUpdatedStats);

                foreach (Window current in Application.Current.Windows)
                {
                    if (current.Title == obj.ToString())
                    {
                        current.Close();
                    }
                }
            }
            catch (Exception GeneralException)
            {
            }
        }

    }
}
