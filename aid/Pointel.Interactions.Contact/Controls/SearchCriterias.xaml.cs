using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Pointel.Interactions.Contact.Helpers;

namespace Pointel.Interactions.Contact.Controls
{
    /// <summary>
    /// Interaction logic for AdvanceSearch.xaml
    /// </summary>
    public partial class SearchCriterias : UserControl
    {
        public delegate void EventUserControlClose(SearchCriterias instance);
        public event EventUserControlClose eventUserControlClose;
        public enum UserControlType { Status, StartDate, EndDate, Common }
        private string _selectedItem = string.Empty;
        private List<ComboBoxItem> _listComboBoxItem;
        public SearchCriterias(UserControlType userControlType, List<ComboBoxItem> mainComboBoxItemList)
        {
            InitializeComponent();
            _listComboBoxItem = mainComboBoxItemList;
            ChangeVisbility(userControlType);
        }

        private void ChangeVisbility(UserControlType userControlType)
        {
            switch (userControlType)
            {
                case UserControlType.Status:
                    grdDate.Visibility = System.Windows.Visibility.Collapsed;
                    grdCommon.Visibility = System.Windows.Visibility.Collapsed;
                    grdStatus.Visibility = System.Windows.Visibility.Visible;
                    cmbstatus.SelectionChanged -= ComboBox_SelectionChanged;
                    cmbstatus.ItemsSource = _listComboBoxItem.Select(x => x.Content.ToString()).ToList();
                    if (string.IsNullOrEmpty(_selectedItem))
                        cmbstatus.SelectedItem = _listComboBoxItem.Where(x => x.IsSelected == true).Select(y => y.Content.ToString()).FirstOrDefault();
                    else
                        cmbstatus.SelectedItem = _selectedItem;
                    cmbstatus.SelectionChanged += ComboBox_SelectionChanged;
                    break;
                case UserControlType.StartDate:
                case UserControlType.EndDate:
                        txtdate_Value.Text = string.Empty;
                        dtpdate_DatePicker.SelectedDate = null;
                        grdStatus.Visibility = System.Windows.Visibility.Collapsed;
                        grdCommon.Visibility = System.Windows.Visibility.Collapsed;
                        grdDate.Visibility = System.Windows.Visibility.Visible;
                        cmbdate.SelectionChanged -= ComboBox_SelectionChanged;
                        cmbdate.ItemsSource = _listComboBoxItem.Select(x => x.Content.ToString()).ToList();
                        if (string.IsNullOrEmpty(_selectedItem))
                            cmbdate.SelectedItem = _listComboBoxItem.Where(x => x.IsSelected == true).Select(y => y.Content.ToString()).FirstOrDefault();
                        else
                            cmbdate.SelectedItem = _selectedItem;
                        cmbdate.SelectionChanged += ComboBox_SelectionChanged;
                    break;
                case UserControlType.Common:
                    txtcommon_Value.Text = string.Empty;
                    grdStatus.Visibility = System.Windows.Visibility.Collapsed;
                    grdDate.Visibility = System.Windows.Visibility.Collapsed;
                    grdCommon.Visibility = System.Windows.Visibility.Visible;
                    cmbcommon.SelectionChanged -= ComboBox_SelectionChanged;
                    cmbcommon.ItemsSource = _listComboBoxItem.Select(x => x.Content.ToString()).ToList();
                    if (string.IsNullOrEmpty(_selectedItem))
                        cmbcommon.SelectedItem = _listComboBoxItem.Where(x => x.IsSelected == true).Select(y => y.Content.ToString()).FirstOrDefault();
                    else
                        cmbcommon.SelectedItem = _selectedItem;
                    cmbcommon.SelectionChanged += ComboBox_SelectionChanged;
                    break;
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cmb = sender as ComboBox;
            _selectedItem = cmb.SelectedValue.ToString();
            switch (_selectedItem.Replace(" ", "").ToString().ToLower())
            {
                case "status":
                    ChangeVisbility(UserControlType.Status);
                    break;
                case "startdate":
                case "enddate":
                    ChangeVisbility(UserControlType.StartDate);
                    break;
                default:
                    ChangeVisbility(UserControlType.Common);
                    break;
            }
        }
        public Criteria GetCriteria()
        {
            Criteria _criteria = new Criteria();
            UIElement _uiElement = grdMain.Children.Cast<UIElement>().Where(x => x.Visibility == System.Windows.Visibility.Visible).FirstOrDefault();
            if (_uiElement is Grid)
            {
                Grid tempGrid = _uiElement as Grid;
                switch (tempGrid.Name)
                {
                    case "grdStatus":
                        _criteria.Field = "Status";
                        _criteria.Condition = SearchCondition.Contains;
                        _criteria.Value = (cmbstatus_Condition.SelectedItem as ComboBoxItem).Content.ToString();
                        break;
                    case "grdDate":
                        _criteria.Field = cmbdate.SelectedValue.ToString();
                        SearchCondition searchCondition;
                        if (Enum.TryParse<SearchCondition>((cmbdate_Condition.SelectedItem as ComboBoxItem).Content.ToString().Replace(" ", ""), true, out searchCondition))
                        {
                            _criteria.Condition = searchCondition;
                        }
                        _criteria.Value = txtdate_Value.Text;
                        break;
                    case "grdCommon":
                        _criteria.Field = cmbcommon.SelectedItem.ToString();
                        _criteria.Condition = SearchCondition.Contains;
                        _criteria.Value = txtcommon_Value.Text;
                        break;
                }
            }
            return _criteria;
        }

        private void cmbdate_DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime? date = dtpdate_DatePicker.SelectedDate;
            if (date == null) return;
            txtdate_Value.Text = date.Value.ToShortDateString();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            eventUserControlClose.Invoke(this);
        }

        public void ResetUI()
        {
            UIElement _uiElement = grdMain.Children.Cast<UIElement>().Where(x => x.Visibility == System.Windows.Visibility.Visible).FirstOrDefault();
            if (_uiElement is Grid)
            {
                Grid tempGrid = _uiElement as Grid;
                var tempTextBox = tempGrid.Children.OfType<TextBox>().Where(x => x.GetType() == typeof(TextBox));
                foreach (TextBox temptxt in tempTextBox)
                {
                    temptxt.Text = string.Empty;
                }
            }
        }

        private void grdMain_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //grdStatus.Width = grdMain.ActualWidth;
            //grdStatus.UpdateLayout();
            //grdDate.Width = grdMain.ActualWidth;
            //grdDate.UpdateLayout();
            //grdCommon.Width = grdMain.ActualWidth;
            //grdCommon.UpdateLayout();
        }
    }
}
