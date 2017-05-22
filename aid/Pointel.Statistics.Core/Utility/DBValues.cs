#region System Namespace
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
#endregion

#region Pointel Namespace
using Pointel.Statistics.Core.StatisticsProvider;
#endregion

namespace Pointel.Statistics.Core.Utility
{
    internal class DBValues : IDBValules
    {
        #region Property

        private int _referenceId = 0;
        public int ReferenceID
        {
            get { return _referenceId; }
            set { _referenceId = value; }
        }

        private Color _color1;
        public Color Color1
        {
            get { return _color1; }
            set { _color1 = value; }
        }

        private Color _color2;
        public Color Color2
        {
            get { return _color2; }
            set { _color2 = value; }
        }

        private Color _color3;
        public Color Color3
        {
            get { return _color3; }
            set { _color3 = value; }
        }

        private String _displayName;
        public String DisplayName
        {
            get { return _displayName; }
            set { _displayName = value; }
        }

        private String _query;
        public String Query
        {
            get { return _query; }
            set { _query = value; }
        }

        private String _threshold1;
        public String Threshold1
        {
            get { return _threshold1; }
            set { _threshold1 = value; }
        }

        private String _threshold2;
        public String Threshold2
        {
            get { return _threshold2; }
            set { _threshold2 = value; }
        }

        private String _toolTipName;
        public String TooltipName
        {
            get { return _toolTipName; }
            set { _toolTipName = value; }
        }

        private String _tempStat;
        public String TempStat
        {
            get { return _tempStat; }
            set { _tempStat = value; }
        }

        private String _format;
        public String Format
        {
            get { return _format; }
            set { _format = value; }
        }

        #endregion
    }
}
