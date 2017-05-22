using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Pointel.Statistics.Core.StatisticsProvider;

namespace Pointel.Statistics.Core.Utility
{
    internal class AdminValues:IAdminValues
    {
        #region Property

        private int _maxTabs = 0;
        public int MaxTabs
        {
            get { return _maxTabs; }
            set { _maxTabs = value; }
        }

        private int _NRReasonCode = 0;
        public int NRReasonCode
        {
            get { return _NRReasonCode; }
            set { _NRReasonCode = value; }
        }

        private List<string> _AdminUsers;
        public List<string> AdminUsers
        {
            get { return _AdminUsers; }
            set { _AdminUsers = value; }
        }

        private string _NRReasonName = string.Empty;
        public string NRReasonName
        {
            get { return _NRReasonName; }
            set { _NRReasonName = value; }
        }

        private Color _ExistingColor;
        public Color ExistingColor
        {
            get { return _ExistingColor; }
            set { _ExistingColor = value; }
        }

        private Color _NewColor;
        public Color NewColor
        {
            get { return _NewColor; }
            set { _NewColor = value; }
        }

        private bool _AgentNR ;
        public bool AgentNR
        {
            get { return _AgentNR; }
            set { _AgentNR = value; }
        }

        private Color _LogoutColor;
        public Color LogoutColor
        {
            get { return _LogoutColor; }
            set { _LogoutColor = value; }
        }

        private Color _NRColor;
        public Color NRColor
        {
            get { return _NRColor; }
            set { _NRColor = value; }
        }

        private Color _ReadyColor;
        public Color ReadyColor
        {
            get { return _ReadyColor; }
            set { _ReadyColor = value; }
        }

        private bool _AutoAgents;
        public bool AutoAgents
        {
            get { return _AutoAgents; }
            set { _AutoAgents = value; }
        }

        #endregion
    }
}
