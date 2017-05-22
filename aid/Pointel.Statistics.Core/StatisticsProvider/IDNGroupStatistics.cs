#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
#endregion

namespace Pointel.Statistics.Core.StatisticsProvider
{
    /// <summary>
    /// IDNGroupStatistics
    /// </summary>
    internal interface IDNGroupStatistics
    {
        int ReferenceID
        {
            get;
        }

        string DNGroupName
        {
            get;
            set;
        }

        string TempStatName
        {
            get;
            set;
        }

        string DisplayName
        {
            get;
            set;
        }

        Color ThresholdColorOne
        {
            get;
            set;
        }

        Color ThresholdColorTwo
        {
            get;
            set;
        }

        Color StatColor
        {
            get;
            set;
        }

        string FilterName
        {
            get;
            set;
        }

        string StatisticsFormat
        {
            get;
            set;
        }

        string StatisticsName
        {
            get;
            set;
        }

        string StatisticsType
        {
            get;
            set;
        }

        string ThresholdLevelOne
        {
            get;
            set;
        }

        string ThresholdLevelTwo
        {
            get;
            set;
        }

        string ToolTipName
        {
            get;
            set;
        }
        string ServerName
        {
            get;
            set;
        }
    }
}
