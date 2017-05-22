using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Pointel.Validation;


namespace Pointel.Interactions.Contact.Controls
{
    public delegate void ValueClickHandler(string value);

    /// <summary>
    /// Interaction logic for ButtonWithDropDown.xaml
    /// </summary>
    public partial class ButtonWithDropDown : UserControl, INotifyPropertyChanged
    {
        public ButtonWithDropDown()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register("Values", typeof(string), typeof(ButtonWithDropDown),
            new PropertyMetadata(null, new PropertyChangedCallback(OnValueChanged)));


        public event ValueClickHandler ValueClick;

        //private string attrValues = string.Empty;
        private string attrPrefix = string.Empty;


        public string PrefixText
        {
            get
            {
                return attrPrefix;
            }
            set
            {
                attrPrefix = value;
                TxtPrefix.Text = value;
            }
        }

        public string Values
        {
            get
            {
                return (string)GetValue(ValuesProperty);
            }
            set
            {
                //if (!string.IsNullOrEmpty(value))
                //{
                SetValue(ValuesProperty, value);

                //}
            }
        }

        public ImageSource Icon
        {
            get
            {
                return imgIcon.Source;
            }
            set
            {
                imgIcon.Source = value;
            }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            string value = e.NewValue as string;
            if (string.IsNullOrEmpty(value)) return;
            ButtonWithDropDown btn = d as ButtonWithDropDown;
            if (btn != null)
            {
                if (value.Contains(","))
                {
                    ContextMenu mnuOption = new ContextMenu();
                    mnuOption.Style = (Style)btn.FindResource("ContextMenu");
                    foreach (string option in value.Split(','))
                    {
                        if (string.IsNullOrEmpty(option) || string.IsNullOrWhiteSpace(option)) continue;
                        //E-mail to 
                        if (!string.IsNullOrEmpty(btn.PrefixText) && !string.IsNullOrWhiteSpace(btn.PrefixText) && btn.PrefixText.Equals("E-mail to "))
                            if (!option.IsEmailAddress()) continue;
                        MenuItem item = new MenuItem();
                        item.Style = (Style)btn.FindResource("Menuitem");
                        item.Header = btn.PrefixText + option;
                        item.ToolTip = "Call to " + option;
                        item.Tag = option;
                        item.Click += new RoutedEventHandler(btn.item_Click);
                        mnuOption.Items.Add(item);
                        btn.btnOption.ContextMenu = mnuOption;
                        //if (btn.btnImg.ToolTip == null)
                            btn.btnImg.ToolTip = btn.PrefixText + option;
                        if (btn.btnImg.Tag != null)
                            btn.btnImg.Tag = option;
                    }
                }
                else
                {
                    btn.btnImg.ToolTip = btn.PrefixText + value;
                    btn.btnImg.Tag = value;
                    btn.btnOption.Visibility = Visibility.Collapsed;
                }

            }
        }

        void item_Click(object sender, RoutedEventArgs e)
        {
            InvokeClickEvent((sender as MenuItem).Tag.ToString());
        }

        private void btnOption_Click(object sender, RoutedEventArgs e)
        {
            if (btnOption.ContextMenu != null)
                btnOption.ContextMenu.IsOpen = true;
        }


        private void InvokeClickEvent(string valus)
        {
            if (ValueClick != null)
                ValueClick.Invoke(valus);
        }

        private void btnImg_Click(object sender, RoutedEventArgs e)
        {
            if (btnImg.Tag != null)
                InvokeClickEvent(btnImg.Tag.ToString());
            else if (btnOption.ContextMenu != null)
                btnOption.ContextMenu.IsOpen = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
