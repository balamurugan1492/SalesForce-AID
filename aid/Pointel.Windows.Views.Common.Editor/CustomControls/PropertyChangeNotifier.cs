using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace Pointel.Windows.Views.Common.Editor.CustomControls
{
    public sealed class PropertyChangeNotifier : DependencyObject, IDisposable
    {
        private WeakReference _propertySource;
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(PropertyChangeNotifier), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(PropertyChangeNotifier.OnPropertyChanged)));
        public event EventHandler ValueChanged;
        public DependencyObject PropertySource
        {
            get
            {
                DependencyObject result;
                try
                {
                    result = (this._propertySource.IsAlive ? (this._propertySource.Target as DependencyObject) : null);
                }
                catch
                {
                    result = null;
                }
                return result;
            }
        }
        [Bindable(true), Category("Behavior"), Description("Returns/sets the value of the property")]
        public object Value
        {
            get
            {
                return base.GetValue(PropertyChangeNotifier.ValueProperty);
            }
            set
            {
                base.SetValue(PropertyChangeNotifier.ValueProperty, value);
            }
        }
        public PropertyChangeNotifier(DependencyObject propertySource, string path)
            : this(propertySource, new PropertyPath(path, new object[0]))
        {
        }
        public PropertyChangeNotifier(DependencyObject propertySource, DependencyProperty property)
            : this(propertySource, new PropertyPath(property))
        {
        }
        public PropertyChangeNotifier(DependencyObject propertySource, PropertyPath property)
        {
            if (propertySource == null)
            {
                throw new ArgumentNullException("propertySource");
            }
            if (property == null)
            {
                throw new ArgumentNullException("property");
            }
            this._propertySource = new WeakReference(propertySource);
            Binding binding = new Binding();
            binding.Path = property;
            binding.Mode = BindingMode.OneWay;
            binding.Source = propertySource;
            BindingOperations.SetBinding(this, PropertyChangeNotifier.ValueProperty, binding);
        }
        private static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyChangeNotifier propertyChangeNotifier = d as PropertyChangeNotifier;
            if (propertyChangeNotifier != null && propertyChangeNotifier.ValueChanged != null)
            {
                propertyChangeNotifier.ValueChanged(propertyChangeNotifier, EventArgs.Empty);
            }
        }
        public void Dispose()
        {
            BindingOperations.ClearBinding(this, PropertyChangeNotifier.ValueProperty);
        }
    }
}
