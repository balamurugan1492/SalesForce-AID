using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StatTickerFive.Helpers
{
    public class ObjectValues:NotificationObject
    {
        private string _objectsNameTT;
        public string ObjectsNameTT
        {
            get
            {
                return _objectsNameTT;
            }
            set
            {
                _objectsNameTT = value;
                RaisePropertyChanged(() => ObjectsNameTT);
            }
        }

        private string _DescriptionTT;
        public string DescriptionTT
        {
            get
            {
                return _DescriptionTT;
            }
            set
            {
                _DescriptionTT = value;
                RaisePropertyChanged(() => DescriptionTT);
            }
        }


        private bool _isGridChecked;
        public bool isGridChecked
        {
            get
            {
                return _isGridChecked;
            }
            set
            {
                _isGridChecked = value;
                RaisePropertyChanged(() => isGridChecked);
            }
        }

        private string _objectName;
        public string ObjectName
        {
            get
            {
                return _objectName;
            }
            set
            {
                _objectName = value;
                RaisePropertyChanged(() => ObjectName);
            }
        }

        private string _uniqueId;
        public string UniqueId
        {
            get
            {
                return _uniqueId;
            }
            set
            {
                _uniqueId = value;
                RaisePropertyChanged(()=>UniqueId);
            }
        }

        private string _objectDescription;
        public string ObjectDescription
        {
            get
            {
                return _objectDescription;
            }
            set
            {
                _objectDescription = value;
                RaisePropertyChanged(() => ObjectDescription);
            }
        }
    }
}
