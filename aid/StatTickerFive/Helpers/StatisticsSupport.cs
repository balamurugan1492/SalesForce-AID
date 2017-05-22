using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Pointel.Logger.Core;
using Genesyslab.Platform.Commons.Collections;
using Pointel.Statistics.Core.Utility;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Resources;

namespace StatTickerFive.Helpers
{
    public class StatisticsSupport
    {
        #region Declaration
        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        #endregion

        #region Filter Statistics

        /// <summary>
        /// Filters the statistics.
        /// </summary>
        public void FilterStatistics()
        {
            try
            {
                logger.Debug("StatisticsSupport : FilterStatistics Method : Entry");

                foreach (string statistics in Settings.GetInstance().DictServerStatistics.Keys)
                {
                    KeyValueCollection kvpcollection = new KeyValueCollection();
                    kvpcollection = Settings.GetInstance().DictServerStatistics[statistics];

                    if (kvpcollection.ContainsKey("Objects"))
                    {
                        string Objects = string.Empty;

                        Objects = kvpcollection["Objects"].ToString();

                        if (Objects.Contains(StatisticsEnum.StatisticsObject.Agent.ToString()))
                        {
                            if (!Settings.GetInstance().ListAgentStatistics.Contains(statistics))
                                Settings.GetInstance().ListAgentStatistics.Add(statistics);
                        }

                        if (Objects.Contains(StatisticsEnum.StatisticsObject.GroupAgents.ToString()))
                        {
                            if (!Settings.GetInstance().ListAgentGroupStatistics.Contains(statistics))
                                Settings.GetInstance().ListAgentGroupStatistics.Add(statistics);
                        }

                        if (Objects.Contains(StatisticsEnum.StatisticsObject.Queue.ToString()))
                        {
                            if (!Settings.GetInstance().ListACDQueueStatistics.Contains(statistics))
                                Settings.GetInstance().ListACDQueueStatistics.Add(statistics);
                        }

                        if (Objects.Contains(StatisticsEnum.StatisticsObject.GroupQueues.ToString()))
                        {
                            if (!Settings.GetInstance().ListGroupQueueStatistics.Contains(statistics))
                                Settings.GetInstance().ListGroupQueueStatistics.Add(statistics);
                        }

                        if (Objects.Contains(StatisticsEnum.StatisticsObject.RoutePoint.ToString()))
                        {
                            if (!Settings.GetInstance().ListVirtualQueueStatistics.Contains(statistics))
                                Settings.GetInstance().ListVirtualQueueStatistics.Add(statistics);
                        }
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsSupport : FilterStatistics Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsSupport : FilterStatistics Method : Exit");
        }

        #endregion

        #region Get Descriptions

        /// <summary>
        /// Gets the descriptions.
        /// </summary>
        public void GetDescriptions(string objecType)
        {
            try
            {
                logger.Debug("StatisticsSupport : GetDescriptions Method : Entry");

                foreach (string statistics in Settings.GetInstance().DictServerStatistics.Keys)
                {
                    KeyValueCollection kvpcollection = new KeyValueCollection();
                    kvpcollection = Settings.GetInstance().DictServerStatistics[statistics];

                    if (kvpcollection.ContainsKey("Objects"))
                    {
                        if (kvpcollection["Objects"].ToString().Contains(objecType))
                        {
                            if (kvpcollection.ContainsKey("Description"))
                            {
                                if (!Settings.GetInstance().DictStatisticsDesc.ContainsKey(statistics))
                                    Settings.GetInstance().DictStatisticsDesc.Add(statistics, kvpcollection["Description"].ToString());
                            }
                            else
                            {
                                if (!Settings.GetInstance().DictStatisticsDesc.ContainsKey(statistics))
                                    Settings.GetInstance().DictStatisticsDesc.Add(statistics, statistics);
                            }
                        }

                    }

                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsSupport : GetDescriptions Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsSupport : GetDescriptions Method : Exit");
        }

        #endregion

        #region GetSupportedObject
        /// <summary>
        /// Gets the supported object.
        /// </summary>
        /// <param name="statName">Name of the stat.</param>
        /// <returns></returns>
        public string GetSupportedObject(string statName)
        {
            string objectType = string.Empty;
            try
            {
                logger.Debug("StatisticsSupport : GetDescriptions Method : Entry");

                foreach (string statistics in Settings.GetInstance().DictServerStatistics.Keys)
                {
                    if (string.Compare(statistics, statName, true) == 0)
                    {
                        KeyValueCollection kvpcollection = new KeyValueCollection();
                        kvpcollection = Settings.GetInstance().DictServerStatistics[statistics];

                        if (kvpcollection.ContainsKey("Objects"))
                        {
                            if (kvpcollection["Objects"].ToString().Contains(","))
                            {
                                string[] Temp = kvpcollection["Objects"].ToString().Split(',');
                                objectType = Temp[0].ToString();
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {

            }
            return objectType;
        }

        #endregion

        #region CheckStatisticsObject
        /// <summary>
        /// Checks the staticstics object.
        /// </summary>
        /// <param name="statisticsName">Name of the statistics.</param>
        public void CheckStaticsticsObject(string statisticsName)
        {
            logger.Debug("StatisticsSupport : CheckStaticsticsObject Method : Entry");
            try
            {
                if (Settings.GetInstance().ListAgentStatistics.Contains(statisticsName))
                {
                    if (Settings.GetInstance().DictConfiguredAgentStats.ContainsKey(statisticsName))
                    {
                        if (Settings.GetInstance().DictConfiguredAgentStats[statisticsName].Count == Settings.GetInstance().DictServerFilters.Count + 1)
                            Settings.GetInstance().IsAgentRemain = false;
                        else
                            Settings.GetInstance().IsAgentRemain = true;
                    }

                    else
                        Settings.GetInstance().IsAgentRemain = true;
                }
                else
                    Settings.GetInstance().IsAgentRemain = false;

                if (Settings.GetInstance().ListAgentGroupStatistics.Contains(statisticsName))
                {
                    if (Settings.GetInstance().DictConfiguredAGroupStats.ContainsKey(statisticsName))
                    {
                        if (Settings.GetInstance().DictConfiguredAGroupStats[statisticsName].Count == Settings.GetInstance().DictServerFilters.Count + 1)
                            Settings.GetInstance().IsAGroupRemain = false;
                        else
                            Settings.GetInstance().IsAGroupRemain = true;
                    }

                    else
                        Settings.GetInstance().IsAGroupRemain = true;
                }
                else
                    Settings.GetInstance().IsAGroupRemain = false;

                if (Settings.GetInstance().ListACDQueueStatistics.Contains(statisticsName))
                {
                    if (Settings.GetInstance().DictConfiguredACDStats.ContainsKey(statisticsName))
                    {
                        if (Settings.GetInstance().DictConfiguredACDStats[statisticsName].Count == Settings.GetInstance().DictServerFilters.Count + 1)
                            Settings.GetInstance().IsACDRemain = false;
                        else
                            Settings.GetInstance().IsACDRemain = true;
                    }

                    else
                        Settings.GetInstance().IsACDRemain = true;
                }
                else
                    Settings.GetInstance().IsACDRemain = false;

                if (Settings.GetInstance().ListGroupQueueStatistics.Contains(statisticsName))
                {
                    if (Settings.GetInstance().DictConfiguredDNStats.ContainsKey(statisticsName))
                    {
                        if (Settings.GetInstance().DictConfiguredDNStats[statisticsName].Count == Settings.GetInstance().DictServerFilters.Count + 1)
                            Settings.GetInstance().IsDNRemain = false;
                        else
                            Settings.GetInstance().IsDNRemain = true;
                    }
                    else
                        Settings.GetInstance().IsDNRemain = true;
                }
                else
                    Settings.GetInstance().IsDNRemain = false;

                if (Settings.GetInstance().ListVirtualQueueStatistics.Contains(statisticsName))
                {
                    if (Settings.GetInstance().DictConfiguredVQStats.ContainsKey(statisticsName))
                    {
                        if (Settings.GetInstance().DictConfiguredVQStats[statisticsName].Count == Settings.GetInstance().DictServerFilters.Count + 1)
                            Settings.GetInstance().IsVQRemain = false;
                        else
                            Settings.GetInstance().IsVQRemain = true;
                    }

                    else
                        Settings.GetInstance().IsVQRemain = true;
                }
                else
                    Settings.GetInstance().IsVQRemain = false;

            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsSupport : CheckStaticsticsObject Method : " + GeneralException.Message);
            }
            logger.Debug("StatisticsSupport : CheckStaticsticsObject Method : Exit");
        }
        #endregion

        #region ThemeSelector
        public Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> ThemeSelector(string theme)
        {
            Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush> tempDict = new Dictionary<StatisticsEnum.ThemeColors, SolidColorBrush>();
            SolidColorBrush tempSolidColorBrush;
            try
            {

                if (string.Compare(theme, "outlook8", true) == 0)
                {
                    logger.Warn("StatisticsSupport : ThemeSelector : Theme : " + theme);
                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.TitleBackground, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.BackgroundColor, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.TitleForeground, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.BorderBrush, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.MouseOver, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#0061ac") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.MousePressed, tempSolidColorBrush);

                }
                else if (string.Compare(theme, "yahoo", true) == 0)
                {
                    logger.Warn("StatisticsSupport : ThemeSelector : Theme : " + theme);
                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#340481") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.TitleBackground, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.BackgroundColor, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.TitleForeground, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#340481") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.BorderBrush, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#8568b3") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.MouseOver, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#340481") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.MousePressed, tempSolidColorBrush);
                }
                else if (string.Compare(theme, "grey", true) == 0)
                {
                    logger.Warn("StatisticsSupport : ThemeSelector : Theme : " + theme);
                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#c6c7c6") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.TitleBackground, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.BackgroundColor, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#000000") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.TitleForeground, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#c6c7c6") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.BorderBrush, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#dcdddc") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.MouseOver, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#c6c7c6") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.MousePressed, tempSolidColorBrush);
                }
                else if (string.Compare(theme, "BB_theme1", true) == 0)
                {
                    logger.Warn("StatisticsSupport : ThemeSelector : Theme : " + theme);
                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#5C6C7A") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.TitleBackground, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.BackgroundColor, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.TitleForeground, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#5C6C7A") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.BorderBrush, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#9da6af") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.MouseOver, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#5c6c7a") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.MousePressed, tempSolidColorBrush);
                }
                else
                {
                    logger.Warn("StatisticsSupport : ThemeSelector : Theme : " + theme);
                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.TitleBackground, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.BackgroundColor, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#000000") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.TitleForeground, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.BorderBrush, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.MouseOver, tempSolidColorBrush);

                    tempSolidColorBrush = new SolidColorBrush();
                    tempSolidColorBrush = new BrushConverter().ConvertFromString("#007edf") as SolidColorBrush;
                    tempDict.Add(StatisticsEnum.ThemeColors.MousePressed, tempSolidColorBrush);
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsSupport : ThemeSelector : " + GeneralException.Message);
            }
            logger.Debug("StatisticsSupport : ThemeSelector : Exit");
            return tempDict;
        }
        #endregion

        #region GetDescription
        public string GetDescription(string statisticsName)
        {
            string DisplayName = string.Empty;
            try
            {
                logger.Debug("StatisticsSupport : GetDescription Method : Entry");
                Dictionary<string, string> statDetails = new Dictionary<string, string>();
                statDetails = Settings.GetInstance().DictExistingApplicationStats[statisticsName];


                foreach (string key in statDetails.Keys)
                {
                    if (string.Compare(key, StatisticsEnum.StatProperties.DisplayName.ToString(), true) == 0)
                    {
                        DisplayName = statDetails[key].ToString();
                    }
                }
            }
            catch (Exception GeneralException)
            {
                logger.Error("StatisticsSupport : GetDescription Method : Exception caught" + GeneralException.Message.ToString());
            }
            return DisplayName;
            logger.Debug("StatisticsSupport : GetDescription Method : Exit");
        }
        #endregion

        #region SetImageData
        public BitmapImage SetImageData(Uri uri)
        {
            StreamResourceInfo imageInfo = System.Windows.Application.GetResourceStream(uri);
            var bitmap = new BitmapImage();
            try
            {
                logger.Debug("StatisticsSupport : SetImageData Method : Entry");
                byte[] imageBytes = ReadFully(imageInfo.Stream);
                using (Stream stream = new MemoryStream(imageBytes))
                {
                    bitmap.BeginInit();
                    bitmap.StreamSource = stream;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    bitmap.UriSource = uri;
                    if (bitmap.CanFreeze)
                        bitmap.Freeze();
                }
                imageBytes = null;
                return bitmap;
            }
            catch (Exception generalException)
            {
                return null;
                logger.Error("StatisticsSupport : SetImageData Method : Exception caught" + generalException.Message.ToString());
            }
            finally
            {
                imageInfo = null;
                bitmap = null;
            }
            logger.Debug("StatisticsSupport : SetImageData Method : Exit");
        }
        #endregion

        #region ReadFully
        private byte[] ReadFully(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                logger.Debug("StatisticsSupport : ReadFully Method : Entry");
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
            logger.Debug("StatisticsSupport : ReadFully Method : Exit");
        }
        #endregion

    }
}
