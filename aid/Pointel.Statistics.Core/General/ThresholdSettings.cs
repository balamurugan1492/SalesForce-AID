#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
#endregion

#region Pointel Namespace
using Pointel.Logger.Core;
using Pointel.Statistics.Core.Utility;
using Pointel.Statistics.Core.ConnectionManager;
#endregion

namespace Pointel.Statistics.Core.General
{
    /// <summary>
    /// This class contains to threshold levels for the statistics value.
    /// </summary>
    internal class ThresholdSettings
    {
        #region Field Declaration
        static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        #endregion

        #region Get Threshold stat color

        /// <summary>
        /// Gets the color of the agent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="objectid">The name.</param>
        /// <param name="statDisplayName">Display name of the stat.</param>
        /// <param name="statName">Name of the stat.</param>
        /// <returns></returns>
        public Color ThresholdColor(string value, string objectid, string refid, string statformat)
        {
            Color agentColor = Color.Black;
            try
            {
                logger.Debug("ThresholdSettings : ThresholdColor Method : Entry");
                if (StatisticsSetting.GetInstance().ThresholdValues.ContainsKey(refid))
                {
                    List<string> localValues = (List<string>)StatisticsSetting.GetInstance().ThresholdValues[refid];
                    if (localValues != null)
                    {
                        if (string.Compare(statformat, "t", true) == 0)
                        {
                            DateTime thresholdDate1 = Convert.ToDateTime(localValues[0]);
                            DateTime thresholdDate2 = Convert.ToDateTime(localValues[1]);
                            DateTime realValue = Convert.ToDateTime(value);

                            if (Convert.ToInt32(realValue.TimeOfDay.TotalSeconds) <= Convert.ToInt32(thresholdDate1.TimeOfDay.TotalSeconds))
                            {
                                agentColor = GetColor(refid, 0);
                                StatisticsSetting.GetInstance().isLevelTwo = false;
                                StatisticsSetting.GetInstance().isThresholdBreach = false;
                            }
                            else if (Convert.ToInt32(realValue.TimeOfDay.TotalSeconds) > Convert.ToInt32(thresholdDate1.TimeOfDay.TotalSeconds)
                                && Convert.ToInt32(realValue.TimeOfDay.TotalSeconds) <= Convert.ToInt32(thresholdDate2.TimeOfDay.TotalSeconds))
                            {
                                agentColor = GetColor(refid, 1);
                                StatisticsSetting.GetInstance().isLevelTwo = false;
                                StatisticsSetting.GetInstance().isThresholdBreach = true;
                            }
                            else if (Convert.ToInt32(realValue.TimeOfDay.TotalSeconds) > Convert.ToInt32(thresholdDate2.TimeOfDay.TotalSeconds))
                            {
                                agentColor = GetColor(refid, 2);
                                StatisticsSetting.GetInstance().isLevelTwo = true;
                                StatisticsSetting.GetInstance().isThresholdBreach = true;
                            }
                        }
                        else if (string.Compare(statformat, "d", true) == 0)
                        {
                            int thresValue1 = Convert.ToInt32(localValues[0]);
                            int thresValue2 = Convert.ToInt32(localValues[1]);
                            if (Convert.ToInt32(value) <= thresValue1)
                            {
                                agentColor = GetColor(refid, 0);
                                StatisticsSetting.GetInstance().isLevelTwo = false;
                                StatisticsSetting.GetInstance().isThresholdBreach = false;
                            }
                            else if (Convert.ToInt32(value) > thresValue1 && Convert.ToInt32(value) <= thresValue2)
                            {
                                agentColor = GetColor(refid, 1);
                                StatisticsSetting.GetInstance().isLevelTwo = false;
                                StatisticsSetting.GetInstance().isThresholdBreach = true;
                            }
                            else if (Convert.ToInt32(value) > thresValue2)
                            {
                                agentColor = GetColor(refid, 2);
                                StatisticsSetting.GetInstance().isLevelTwo = true;
                                StatisticsSetting.GetInstance().isThresholdBreach = true;
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ThresholdSettings : ThresholdColor Method : " + generalException.Message);
            }
            finally
            {
                logger.Debug("ThresholdSettings : ThresholdColor Method : Exit");
                GC.Collect();
            }
            return agentColor;
        }
        #endregion

        #region GetColor
        /// <summary>
        /// Gets the color.
        /// </summary>
        /// <param name="statName">Name of the stat.</param>
        /// <param name="thresholdType">Type of the threshold.</param>
        /// <returns></returns>
        Color GetColor(string refid, int thresholdType)
        {
            Color result = new Color();
            try
            {
                logger.Debug("ThresholdSettings : GetColor Method : Entry");
                if (StatisticsSetting.GetInstance().ThresholdColors.ContainsKey(refid))
                {
                    List<Color> colorCollection = (List<Color>)StatisticsSetting.GetInstance().ThresholdColors[refid];
                    try
                    {
                        result = colorCollection[thresholdType];
                    }
                    catch (Exception colorException)
                    {
                        result = Color.Black;
                        logger.Error("ThresholdSettings : GetColor Method : " + colorException.Message);
                    }
                }
                else
                {
                    result = Color.Black;
                }
            }
            catch (Exception generalException)
            {
                logger.Error("ThresholdSettings : GetColor Method : " + generalException.Message);
            }
            finally
            {
                logger.Debug("ThresholdSettings : GetColor Method : Exit");
                GC.Collect();
            }
            return result;
        }
        #endregion
    }
}
