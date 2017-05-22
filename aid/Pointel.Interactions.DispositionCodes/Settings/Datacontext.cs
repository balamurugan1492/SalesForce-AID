using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;

namespace Pointel.Interactions.DispositionCodes.Settings
{
    public class Datacontext
    {
        private Dictionary<string, string> _voiceDispositionCodes = new Dictionary<string, string>();
        private Dictionary<string, string> _chatDispositionCodes = new Dictionary<string, string>();
        private Dictionary<string, string> _emailDispositionCodes = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, string>> _voiceSubDispositionCodes = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, Dictionary<string, string>> _emailSubDispositionCodes = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, Dictionary<string, string>> _chatSubDispositionCodes = new Dictionary<string, Dictionary<string, string>>();
        private bool _isMultiDispositionEnabled = false;
        private Visibility _singleDispositionPanelVisibility;
        private Visibility _multiDispositionPanelVisibility;

        private string _dispositionCodeKey;
        private string _dispositionName;

        public Dictionary<string, string> VoiceDispositionCodes
        {
            get
            {
                return _voiceDispositionCodes;
            }
            set
            {
                if (_voiceDispositionCodes != value)
                {
                    _voiceDispositionCodes = value;
                }
            }
        }

        public string DispositionName
        {
            get
            {
                return _dispositionName;
            }
            set
            {
                if (_dispositionName != value)
                {
                    _dispositionName = value;
                }
            }
        }

        public string DispositionCodeKey
        {
            get
            {
                return _dispositionCodeKey;
            }
            set
            {
                if (_dispositionCodeKey != value)
                {
                    _dispositionCodeKey = value;
                }
            }
        }

        public Dictionary<string, string> ChatDispositionCodes
        {
            get
            {
                return _chatDispositionCodes;
            }
            set
            {
                if (_chatDispositionCodes != value)
                {
                    _chatDispositionCodes = value;
                }
            }
        }

        public Dictionary<string, string> EmailDispositionCodes
        {
            get
            {
                return _emailDispositionCodes;
            }
            set
            {
                if (_emailDispositionCodes != value)
                {
                    _emailDispositionCodes = value;
                }
            }
        }

        public Dictionary<string, Dictionary<string, string>> VoiceSubDispositionCodes
        {
            get
            {
                return _voiceSubDispositionCodes;
            }
            set
            {
                if (_voiceSubDispositionCodes != value)
                {
                    _voiceSubDispositionCodes = value;
                }
            }
        }

        public Dictionary<string, Dictionary<string, string>> EmailSubDispositionCodes
        {
            get
            {
                return _emailSubDispositionCodes;
            }
            set
            {
                if (_emailSubDispositionCodes != value)
                {
                    _emailSubDispositionCodes = value;
                }
            }
        }

        public Dictionary<string, Dictionary<string, string>> ChatSubDispositionCodes
        {
            get
            {
                return _chatSubDispositionCodes;
            }
            set
            {
                if (_chatSubDispositionCodes != value)
                {
                    _chatSubDispositionCodes = value;
                }
            }
        }

        public bool IsMultiDispositionEnabled
        {
            get
            {
                return _isMultiDispositionEnabled;
            }
            set
            {
                if (_isMultiDispositionEnabled != value)
                {
                    _isMultiDispositionEnabled = value;
                }
            }
        }

        public Visibility SingleDispositionPanelVisibility
        {
            get
            {
                return _singleDispositionPanelVisibility;
            }
            set
            {
                if (_singleDispositionPanelVisibility != value)
                {
                    _singleDispositionPanelVisibility = value;
                }
            }
        }

        public Visibility MultiDispositionPanelVisibility
        {
            get
            {
                return _multiDispositionPanelVisibility;
            }
            set
            {
                if (_multiDispositionPanelVisibility != value)
                {
                    _multiDispositionPanelVisibility = value;
                }
            }
        }
    }
}
