#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
#endregion

namespace Pointel.Statistics.Core.StatisticsProvider
{
    internal interface IDBValules
    {
        int ReferenceID
        {
            get;
        }

        Color Color1
        {
            get;
            set;
        }

        Color Color2
        {
            get;
            set;
        }

        Color Color3
        {
            get;
            set;
        }

        String DisplayName
        {
            get;
            set;
        }

        String Query
        {
            get;
            set;
        }

        String Threshold1
        {
            get;
            set;
        }

        String Threshold2
        {
            get;
            set;
        }

        String TooltipName
        {
            get;
            set;
        }

        String TempStat
        {
            get;
            set;
        }

        String Format
        {
            get;
            set;
        }
    }
}
