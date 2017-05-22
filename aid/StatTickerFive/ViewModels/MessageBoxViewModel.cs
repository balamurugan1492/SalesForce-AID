using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using StatTickerFive.Helpers;
using StatTickerFive.Views;
using Pointel.Logger;
using Pointel.Statistics.Core.Utility;
using System.Collections.Generic;

namespace StatTickerFive.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public class MessageBoxViewModel : NotificationObject
    {
        #region Public Declarations

        private static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        Window Messagebox = null;
        StatisticsSupport statSupport = new StatisticsSupport();
        string basewin;

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

        private string _MessageboxContent;
        public string MessageboxContent
        {
            get
            {
                return _MessageboxContent;
            }
            set
            {
                _MessageboxContent = value;
                RaisePropertyChanged(() => MessageboxContent);
            }
        }

        private string _MessageboxTitle;
        public string MessageboxTitle
        {
            get
            {
                return _MessageboxTitle;
            }
            set
            {
                _MessageboxTitle = value;
                RaisePropertyChanged(() => MessageboxTitle);
            }
        }

        private ContentControl _rightbuttonContent;
        public ContentControl rightbuttonContent
        {
            get
            {
                return _rightbuttonContent;
            }
            set
            {
                _rightbuttonContent = value;
                RaisePropertyChanged(() => rightbuttonContent);
            }
        }

        private ContentControl _leftbuttonContent;
        public ContentControl leftbuttonContent
        {
            get
            {
                return _leftbuttonContent;
            }
            set
            {
                _leftbuttonContent = value;
                RaisePropertyChanged(() => leftbuttonContent);
            }
        }

        private Visibility _leftbuttonVisible;
        public Visibility leftbuttonVisible
        {
            get
            {
                return _leftbuttonVisible;
            }
            set
            {
                _leftbuttonVisible = value;
                RaisePropertyChanged(() => leftbuttonVisible);
            }
        }

        private Visibility _rightbuttonVisible;
        public Visibility rightbuttonVisible
        {
            get
            {
                return _rightbuttonVisible;
            }
            set
            {
                _rightbuttonVisible = value;
                RaisePropertyChanged(() => rightbuttonVisible);
            }
        }

        private Visibility _closeVisibility;
        public Visibility CloseVisibility
        {
            get
            {
                return _closeVisibility;
            }
            set
            {
                _closeVisibility = value;
                RaisePropertyChanged(() => CloseVisibility);
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

        public ICommand MsgBoxClose { get { return new DelegateCommand(MboxClose); } }
        public ICommand ActivatedCommand { get { return new DelegateCommand(WinActivated); } }
        public ICommand DeactivateCommand { get { return new DelegateCommand(WinDeActivated); } }
        public ICommand DragCmd { get { return new DelegateCommand(DragMove); } }
        public ICommand MsgCancelCmd { get { return new DelegateCommand(MsgCancel); } }

        #endregion

        #region Methods

        /// <summary>
        /// MSGs the cancel.
        /// </summary>
        private void MsgCancel()
        {
            try
            {
                foreach (Window current in Application.Current.Windows)
                {
                    if (current.Name == "MsgWindow")
                    {
                        Settings.GetInstance().IsCancelSaving = true;
                        current.Close();
                    }
                }
            }
            catch (Exception GeneralException)
            {

            }
        }

        /// <summary>
        /// Mboxes the close.
        /// </summary>
        private void MboxClose(object obj)
        {
            try
            {
                ContentControl temp = new ContentControl();

                switch (basewin)
                {
                    case "LoginWindow":
                        Application.Current.Shutdown();
                        //Environment.Exit(0);
                        break;
                    case "MainWindow":
                        Messagebox.DialogResult = true;
                        Messagebox.Close();
                        break;
                    case "AdminConfig":
                        temp = obj as ContentControl;
                        if (temp.Content.ToString() == "Overwrite")
                            Settings.GetInstance().ToOverWrite = true;
                        else
                            Settings.GetInstance().ToOverWrite = false;

                        Messagebox.DialogResult = true;
                        Messagebox.Close();

                        break;
                    case "ObjectChanged":
                        temp = obj as ContentControl;
                        if (temp.Content.ToString() == "Yes")
                        {
                            //Settings.GetInstance().isConfigNewStats = true;
                            //Settings.GetInstance().isConfigureNextStat = true;
                            //Settings.GetInstance().SaveConfigurations = true;
                            Settings.GetInstance().IsConfigureStats = true;
                            Settings.GetInstance().IsSaveConfiguredObjs = true;
                            Settings.GetInstance().IsConfigureLevelObjects = true;
                        }
                        else
                        {
                            //Settings.GetInstance().isConfigNewStats = false;
                            //Settings.GetInstance().isConfigureNextStat = false;
                            //Settings.GetInstance().SaveConfigurations = false;
                            Settings.GetInstance().IsConfigureStats = false;
                            Settings.GetInstance().IsSaveConfiguredObjs = false;
                            Settings.GetInstance().IsConfigureLevelObjects = false;
                        }
                        Messagebox.DialogResult = true;
                        Messagebox.Close();
                        break;
                }
            }
            catch (Exception ex)
            {

                logger.Error("MessageBoxViewModel : Constructor : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MessageBoxViewModel : MboxClose : Method Exit");
            }

        }       

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageBoxViewModel" /> class.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="content">The content.</param>
        /// <param name="window">The window.</param>
        /// <param name="basewindow">The basewindow.</param>
        public MessageBoxViewModel(string title, string content, Window window, string basewindow,string theme)
        {
            try
            {
                logger.Debug("MessageBoxViewModel : Constructor : Entry");
                Messagebox = window;
                basewin = basewindow;

                Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> dictTheme = new Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush>();
                dictTheme = statSupport.ThemeSelector(Settings.GetInstance().Theme);

                TitleBackground = dictTheme[StatisticsEnum.ThemeColors.TitleBackground];
                BackgroundColor = dictTheme[StatisticsEnum.ThemeColors.BackgroundColor];
                TitleForeground = dictTheme[StatisticsEnum.ThemeColors.TitleForeground];
                BorderBrush = dictTheme[StatisticsEnum.ThemeColors.BorderBrush];
                ShadowEffect = new DropShadowBitmapEffect();
                ShadowEffect.Color = (Color)BorderBrush.Color;

                switch (basewindow)
                {
                    case "LoginWindow":
                        rightbuttonVisible = Visibility.Visible;
                        leftbuttonVisible = Visibility.Hidden;
                        rightbuttonContent = new ContentControl();
                        rightbuttonContent.Foreground = Brushes.White;
                        rightbuttonContent.Content = "Exit";
                        CloseVisibility = Visibility.Collapsed;
                        break;
                    case "MainWindow":
                        rightbuttonVisible = Visibility.Visible;
                        leftbuttonVisible = Visibility.Hidden;
                        rightbuttonContent = new ContentControl();
                        rightbuttonContent.Foreground = Brushes.White;
                        rightbuttonContent.Content = "Ok";
                        CloseVisibility = Visibility.Collapsed;
                        break;
                    case "ObjectChanged":
                        leftbuttonVisible = Visibility.Visible;
                        leftbuttonContent = new ContentControl();
                        leftbuttonContent.Foreground = Brushes.White;
                        leftbuttonContent.Content = "Yes";

                        rightbuttonVisible = Visibility.Visible;
                        rightbuttonContent = new ContentControl();
                        rightbuttonContent.Foreground = Brushes.White;
                        rightbuttonContent.Content = "No";
                        CloseVisibility = Visibility.Collapsed;
                        break;
                    case "AdminConfig":
                        CloseVisibility = Visibility.Visible;

                        leftbuttonVisible = Visibility.Visible;
                        leftbuttonContent = new ContentControl();
                        leftbuttonContent.Foreground = Brushes.White;
                        leftbuttonContent.Content = "Append";

                        rightbuttonVisible = Visibility.Visible;
                        rightbuttonContent = new ContentControl();
                        rightbuttonContent.Foreground = Brushes.White;
                        rightbuttonContent.Content = "Overwrite";
                        break;
                    case "ObjectConfig":
                        CloseVisibility = Visibility.Visible;

                        leftbuttonVisible = Visibility.Visible;
                        leftbuttonContent = new ContentControl();
                        leftbuttonContent.Foreground = Brushes.White;
                        leftbuttonContent.Content = "Yes";

                        rightbuttonVisible = Visibility.Visible;
                        rightbuttonContent = new ContentControl();
                        rightbuttonContent.Foreground = Brushes.White;
                        rightbuttonContent.Content = "No";
                        break;
                }

                WinActivated();
                MessageboxContent = content;
                MessageboxTitle = title;
            }
            catch (Exception ex)
            {
                logger.Error("MessageBoxViewModel : Constructor : " + ex.Message);
            }
            finally
            {
                GC.Collect();
                logger.Debug("MessageBoxViewModel : Constructor : Exit");
            }
        }

        /// <summary>
        /// Wins the activated.
        /// </summary>
        private void WinActivated()
        {
            object obj;
            object convertFromString;
            try
            {

                obj = (ColorConverter.ConvertFromString("#0070C5"));
                if (obj != null)
                   // BorderBrush = new SolidColorBrush((Color)obj);

                ShadowEffect = new DropShadowBitmapEffect();
                ShadowEffect.ShadowDepth = 0;
                ShadowEffect.Opacity = 0.5;
                ShadowEffect.Softness = 0.5;
                //convertFromString = ColorConverter.ConvertFromString("#003660");
               // if (convertFromString != null)
                   // ShadowEffect.Color = (Color)convertFromString;

            }
            catch (Exception ex)
            {
                logger.Error("MessageBoxViewModel : WinActivated : " + ex.Message);
            }
            finally
            {
                obj = null;
                convertFromString = null;
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
        private void DragMove()
        {
            try
            {
                foreach (Window window in Application.Current.Windows)
                {
                    if (window.Name == "MsgWindow")
                        window.DragMove();
                }
            }
            catch (Exception ex)
            {
                logger.Error("MessageBoxViewModel : DragMove : " + ex.Message);
            }
            finally
            {
                GC.Collect();
            }
        }

        #endregion
    }
}
