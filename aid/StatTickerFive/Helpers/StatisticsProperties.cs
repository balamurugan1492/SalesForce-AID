using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Markup;

namespace StatTickerFive.Helpers
{
    public class StatisticsProperties : NotificationObject
    {
        private string _sectionTooltip;
        public string SectionTooltip
        {
            get
            {
                return _sectionTooltip;
            }
            set
            {
                _sectionTooltip = value;
                RaisePropertyChanged(() => SectionTooltip);
            }
        }

        private string _displayTooltip;
        public string DisplayTooltip
        {
            get
            {
                return _displayTooltip;
            }
            set
            {
                _displayTooltip = value;
                RaisePropertyChanged(() => DisplayTooltip);
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

        private string _sectionName;
        public string SectionName
        {
            get
            {
                return _sectionName;
            }
            set
            {
                _sectionName = value;
                RaisePropertyChanged(() => SectionName);
            }
        }

        private string _editName;
        public string EditName
        {
            get
            {
                return _editName;
            }
            set
            {
                _editName = value;
                RaisePropertyChanged(() => EditName);
            }
        }

        private string _displayName;
        public string DisplayName
        {
            get
            {
                return _displayName;
            }
            set
            {
                _displayName = value;
                RaisePropertyChanged(() => DisplayName);
            }
        }

        private bool _isCheckBoxEnabled;
        public bool IsCheckBoxEnabled
        {
            get
            {
                return _isCheckBoxEnabled;
            }
            set
            {
                _isCheckBoxEnabled = value;
                RaisePropertyChanged(() => IsCheckBoxEnabled);
            }
        }

        private bool _isExistProperty;
        public bool IsExistProperty
        {
            get
            {
                return _isExistProperty;
            }
            set
            {
                if (value != null)
                {
                    _isExistProperty = value;
                    RaisePropertyChanged(() => IsExistProperty);
                }
            }
        }
    }
    public class SearchedStatistics
    {
        public string SearchedStatname { get; set; }

        public string SearchedStatDescription { get; set; }

        public string SearchedStatnameTooltip { get; set; }

        public string SearchedStatDescTooltip { get; set; }
    }
    public class ObjectProperties : NotificationObject
    {
        private string _objectName;
        public string ObjectName
        {
            get
            {
                return _objectName;
            }
            set
            {
                if (value != null)
                {
                    _objectName = value;
                    RaisePropertyChanged(() => ObjectName);

                }
            }
        }

        private bool _isObjectChecked;
        public bool IsObjectChecked
        {
            get
            {
                return _isObjectChecked;
            }
            set
            {
                if (value != null)
                {
                    _isObjectChecked = value;
                    RaisePropertyChanged(() => IsObjectChecked);
                }

            }
        }

        #region ObjectSwitchName
        private string _objectSwitchName;
        public string ObjectSwitchName
        {
            get { return _objectSwitchName; }
            set
            {
                _objectSwitchName = value;
                RaisePropertyChanged(() => ObjectSwitchName);
            }
        }
        #endregion

        private string _objectType;
        public string ObjectType
        {
            get
            {
              return _objectType;
            }
            set
            {
                _objectType=value;
                RaisePropertyChanged(()=>ObjectType);
            }
        }

        private string _typeObject;
        public string TypeObject
        {
            get
            {
                return _typeObject;
            }
            set
            {
                _typeObject = value;
                RaisePropertyChanged(() => TypeObject);
            }
        }
    }
    public class ObjectType : NotificationObject
    {
        public string Text { get; set; }

        private bool _isComboboxEnabled;
        public bool IsComboBoxEnabled
        {
            get
            {
                return _isComboboxEnabled;
            }
            set
            {
                _isComboboxEnabled = value;
                RaisePropertyChanged(() => IsComboBoxEnabled);
            }
        }
    }
    public class LengthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataGridLengthConverter converter = new DataGridLengthConverter();
            var res = converter.ConvertFrom(value);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataGridLength length = (DataGridLength)value;
            return length.DisplayValue;
        }
    }

    [ContentProperty("Name")]
    public class Reference : System.Windows.Markup.Reference
    {
        public Reference() : base() { }
        public Reference(string name) : base(name) { }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException("serviceProvider");
            }
            IProvideValueTarget valueTargetProvider = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (valueTargetProvider != null)
            {
                DependencyObject targetObject = valueTargetProvider.TargetObject as DependencyObject;
                if (targetObject != null && DesignerProperties.GetIsInDesignMode(targetObject))
                {
                    return null;
                }
            }
            return base.ProvideValue(serviceProvider);
        }
    }

}