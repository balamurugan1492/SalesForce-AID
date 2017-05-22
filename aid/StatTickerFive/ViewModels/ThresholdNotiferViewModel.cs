using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Effects;
using StatTickerFive.Helpers;
using StatTickerFive.Views;
using Pointel.Logger.Core;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows;

namespace StatTickerFive.ViewModels
{
    public class ThresholdNotiferViewModel : NotificationObject
    {
        #region Public Declarations

        #region Field Declaration
        static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        #endregion
        string TitleBackgroundColor = string.Empty;

        #endregion

        #region Properties


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

        private string _NotificationTitle;
        /// <summary>
        /// Gets or sets the notification title.
        /// </summary>
        /// <value>The notification title.</value>
        public string NotificationTitle
        {
            get
            {
                return _NotificationTitle;
            }
            set
            {
                _NotificationTitle = value;
                RaisePropertyChanged(() => NotificationTitle);
            }
        }

        private string _NotificationContent;
        /// <summary>
        /// Gets or sets the content of the notification.
        /// </summary>
        /// <value>The content of the notification.</value>
        public string NotificationContent
        {
            get
            {
                return _NotificationContent;
            }
            set
            {
                _NotificationContent = value;
                RaisePropertyChanged(() => NotificationContent);
            }
        }

        private SolidColorBrush _TitleBackground;
        /// <summary>
        /// Gets or sets the color of the theme.
        /// </summary>
        /// <value>The color of the theme.</value>
        public SolidColorBrush TitleBackground
        {
            get
            {
                return _TitleBackground;
            }
            set
            {
                _TitleBackground = value;
                RaisePropertyChanged(() => TitleBackground);
            }
        }

        private SolidColorBrush _TitleColor;
        /// <summary>
        /// Gets or sets the color of the title.
        /// </summary>
        /// <value>The color of the title.</value>
        public SolidColorBrush TitleColor
        {
            get
            {
                return _TitleColor;
            }
            set
            {
                _TitleColor = value;
                RaisePropertyChanged(() => TitleColor);
            }
        }


        private SolidColorBrush _ContentBackground;
        public SolidColorBrush ContentBackground
        {
            get
            {
                return _ContentBackground;
            }
            set
            {
                _ContentBackground = value;
                RaisePropertyChanged(() => ContentBackground);
            }
        }

        private FontWeight _ContentWeight;
        public FontWeight ContentWeight
        {
            get
            {
                return _ContentWeight;
            }
            set
            {
                if (value != null)
                {
                    _ContentWeight = value;
                    RaisePropertyChanged(() => ContentWeight);
                }
            }
        }

        #endregion

        #region Methods

        #region ThresholdNotiferViewModel Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomNotifierViewModel" /> class.
        /// </summary>
        public ThresholdNotiferViewModel(string theme, string titleBG, string titleFG, string contentBG, bool isStatBold)
        {
            ShadowEffect = new DropShadowBitmapEffect();
            object convertFromString;

            try
            {
                logger.Debug("ThresholdNotiferViewModel : Constructor : Entry");

                ShadowEffect.ShadowDepth = 0;
                ShadowEffect.Opacity = 0.5;
                ShadowEffect.Softness = 0.5;
                convertFromString = ColorConverter.ConvertFromString("#003660");
                if (convertFromString != null)
                    ShadowEffect.Color = (Color)convertFromString;

                NotifierTheme(theme, titleBG, titleFG, contentBG);

                if (isStatBold)
                    ContentWeight = FontWeights.Bold;
                else
                    ContentWeight = FontWeights.Normal;
            }
            catch (Exception ex)
            {
                logger.Error("ThresholdNotiferViewModel : Constructor : " + ex.Message);
            }
            finally
            {
                ShadowEffect = null;
                convertFromString = null;
                GC.Collect();
            }
            logger.Debug("ThresholdNotiferViewModel : Constructor : Exit");
        }

        #endregion

        #region NotifierTheme

        /// <summary>
        /// Notifiers the theme.
        /// </summary>
        public void NotifierTheme(string theme, string titleBG, string titleFG, string contentBG)
        {
            try
            {
                logger.Debug("ThresholdNotiferViewModel : NotifierTheme : Entry");
                logger.Info("ThresholdNotiferViewModel : NotifierTheme : Theme : "+theme);
                logger.Info("ThresholdNotiferViewModel : NotifierTheme : Theme Title Background : " + titleBG);
                logger.Info("ThresholdNotiferViewModel : NotifierTheme : Theme Title Foreground: " + titleFG);
                logger.Info("ThresholdNotiferViewModel : NotifierTheme : Theme Content Background: " + contentBG);

                switch (theme)
                {
                    case "Outlook8":
                        TitleBackground = new SolidColorBrush();
                        TitleBackground = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                        BorderBrush = new SolidColorBrush();
                        BorderBrush = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                        TitleColor = new SolidColorBrush();
                        TitleColor = new BrushConverter().ConvertFromString("#000000") as SolidColorBrush;
                        ContentBackground = new SolidColorBrush();
                        ContentBackground = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                        break;
                    case "Yahoo":
                        TitleBackground = new SolidColorBrush();
                        TitleBackground = new BrushConverter().ConvertFromString("#340481") as SolidColorBrush;
                        BorderBrush = new SolidColorBrush();
                        BorderBrush = new BrushConverter().ConvertFromString("#340481") as SolidColorBrush;
                        TitleColor = new SolidColorBrush();
                        TitleColor = new BrushConverter().ConvertFromString("#000000") as SolidColorBrush;
                        ContentBackground = new SolidColorBrush();
                        ContentBackground = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                        break;
                    case "Grey":
                        TitleBackground = new SolidColorBrush();
                        TitleBackground = new BrushConverter().ConvertFromString("#c6c7c6") as SolidColorBrush;
                        BorderBrush = new SolidColorBrush();
                        BorderBrush = new BrushConverter().ConvertFromString("#c6c7c6") as SolidColorBrush;
                        TitleColor = new SolidColorBrush();
                        TitleColor = new BrushConverter().ConvertFromString("#000000") as SolidColorBrush;
                        ContentBackground = new SolidColorBrush();
                        ContentBackground = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                        break;
                    case "BB_Theme1":
                        TitleBackground = new SolidColorBrush();
                        TitleBackground = new BrushConverter().ConvertFromString("#5C6C7A") as SolidColorBrush;
                        BorderBrush = new SolidColorBrush();
                        BorderBrush = new BrushConverter().ConvertFromString("#5C6C7A") as SolidColorBrush;
                        TitleColor = new SolidColorBrush();
                        TitleColor = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                        ContentBackground = new SolidColorBrush();
                        ContentBackground = new BrushConverter().ConvertFromString("#6082B6") as SolidColorBrush;
                        break;
                    case "Custom":
                        TitleBackground = new SolidColorBrush();
                        TitleBackground = new BrushConverter().ConvertFromString(titleBG) as SolidColorBrush;
                        BorderBrush = new SolidColorBrush();
                        BorderBrush = new BrushConverter().ConvertFromString(titleBG) as SolidColorBrush;
                        TitleColor = new SolidColorBrush();
                        TitleColor = new BrushConverter().ConvertFromString(titleFG) as SolidColorBrush;
                        ContentBackground = new SolidColorBrush();
                        ContentBackground = new BrushConverter().ConvertFromString(contentBG) as SolidColorBrush;
                        break;

                    default:
                        TitleBackground = new SolidColorBrush();
                        TitleBackground = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                        BorderBrush = new SolidColorBrush();
                        BorderBrush = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                        TitleColor = new SolidColorBrush();
                        TitleColor = new BrushConverter().ConvertFromString("#000000") as SolidColorBrush;
                        ContentBackground = new SolidColorBrush();
                        ContentBackground = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error("ThresholdNotiferViewModel : NotifierTheme : " + ex.Message);
            }
            finally
            {
                GC.Collect();
            }

            logger.Debug("ThresholdNotiferViewModel : NotifierTheme : Exit");
        }

        #endregion

        #endregion
    }
}
