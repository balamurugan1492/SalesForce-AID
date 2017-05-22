using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Pointel.Interactions.Contact.Helpers;

namespace Pointel.Interactions.Contact.Controls
{
    /// <summary>
    /// Interaction logic for AdvanceSearch.xaml
    /// </summary>
    public partial class AdvanceSearch : UserControl
    {
        private List<string> _listSearchAttributes = new List<string>();
        private ContextMenu _cmenuSearchAttribute = new ContextMenu();
        private List<ComboBoxItem> _searchAttributes = new List<ComboBoxItem>();
        private ObservableCollection<SearchCriterias> _searchCriteria = new ObservableCollection<SearchCriterias>();
        public delegate void EventReceivedFilteredData(List<Criteria> searchCriterias, MatchCondition matchCondition);
        public event EventReceivedFilteredData eventReceivedFilteredData;
        public AdvanceSearch(List<ComboBoxItem> SearchAttributes)
        {
            InitializeComponent();
            foreach (var item in SearchAttributes)
            {
                ComboBoxItem tempItem = new ComboBoxItem() { Content = item.Content, Tag = item.Tag, IsSelected = item.IsSelected };
                _searchAttributes.Add(tempItem);
            }
            CreateAddButtonContextMenu();

            #region Load default search

            foreach (var item in _searchAttributes)
            {
                if (item.Tag == null) continue;
                if (item.Tag.ToString() == "Default")
                {
                    List<ComboBoxItem> _tempList = new List<ComboBoxItem>();
                    foreach (var item1 in _searchAttributes)
                    {
                        ComboBoxItem cmbItem = new ComboBoxItem() { Content = item1.Content };
                        if (item.Content == item1.Content)
                            cmbItem.IsSelected = true;
                        _tempList.Add(cmbItem);
                    }
                    SearchCriterias.UserControlType tempUCType;
                    switch (item.Content.ToString().ToLower().Trim().Replace(" ", ""))
                    {
                        case "status":
                            tempUCType = SearchCriterias.UserControlType.Status;
                            break;
                        case "startdate":
                            tempUCType = SearchCriterias.UserControlType.StartDate;
                            break;
                        case "enddate":
                            tempUCType = SearchCriterias.UserControlType.EndDate;
                            break;
                        default:
                            tempUCType = SearchCriterias.UserControlType.Common;
                            break;
                    }
                    AddConditionUserControl(tempUCType, _tempList);
                }
            }

            #endregion
        }
      
        public void Search_Click(object sender, RoutedEventArgs e)
        {
            List<Criteria> _listCriteria = new List<Criteria>();
            foreach (SearchCriterias searchCriterias in _searchCriteria)
            {
                _listCriteria.Add(searchCriterias.GetCriteria());
            }
            MatchCondition _matchCondition = rtbMAllC.IsChecked == true ? MatchCondition.MatchAll : MatchCondition.MatchAny;
            eventReceivedFilteredData.Invoke(_listCriteria, _matchCondition);
        }

        private void btnAddCondition_Click(object sender, RoutedEventArgs e)
        {
            _cmenuSearchAttribute.PlacementTarget = btnAddCondition;
            _cmenuSearchAttribute.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            _cmenuSearchAttribute.IsOpen = true;
            _cmenuSearchAttribute.StaysOpen = true;
        }

        private void mItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem tempMenuItem = sender as MenuItem;
            SearchCriterias.UserControlType tempUCType;
            switch (tempMenuItem.Header.ToString().ToLower().Trim().Replace(" ", ""))
            {
                case "status":
                    tempUCType = SearchCriterias.UserControlType.Status;
                    break;
                case "startdate":
                    tempUCType = SearchCriterias.UserControlType.StartDate;
                    break;
                case "enddate":
                    tempUCType = SearchCriterias.UserControlType.EndDate;
                    break;
                default:
                    tempUCType = SearchCriterias.UserControlType.Common;
                    break;
            }
            List<ComboBoxItem> _tempList = new List<ComboBoxItem>();
            foreach (var item in _searchAttributes)
            {
                if (item.Content.ToString() == tempMenuItem.Header.ToString())
                    item.IsSelected = true;
                else
                    item.IsSelected = false;
                _tempList.Add(item);
            }
            AddConditionUserControl(tempUCType, _tempList);
            scrollVSearchCriteria.ScrollToBottom();
        }

        private void searchCriterias_eventUserControlClose(SearchCriterias instance)
        {
            if (_searchCriteria.Count > 1)
            {
                instance.eventUserControlClose -= searchCriterias_eventUserControlClose;
                _searchCriteria.Remove(instance);
                if (_searchCriteria.Count == 1)
                {
                    UIElement _uiElement = _searchCriteria[0].grdMain.Children.Cast<UIElement>().Where(x => x.Visibility == System.Windows.Visibility.Visible).FirstOrDefault();
                    if (_uiElement is Grid)
                    {
                        Grid tempGrid = _uiElement as Grid;
                        var tempButton = tempGrid.Children.OfType<Button>().Where(x => x.GetType() == typeof(Button));
                        foreach (Button tempbtn in tempButton)
                        {
                            tempbtn.Visibility = System.Windows.Visibility.Collapsed;
                        }
                    }
                }
                instance = null;
            }
            else
            {
                instance.ResetUI();
            }
            itcSearchCriteria.ItemsSource = _searchCriteria;
        }

        private void AddConditionUserControl(SearchCriterias.UserControlType userControlType, List<ComboBoxItem> listComboBoxItem)
        {

            SearchCriterias searchCriterias = new SearchCriterias(userControlType, listComboBoxItem);
            searchCriterias.eventUserControlClose += new SearchCriterias.EventUserControlClose(searchCriterias_eventUserControlClose);
            if (scrollVSearchCriteria.ActualWidth > 10)
                searchCriterias.Width = scrollVSearchCriteria.ActualWidth - 10;
            _searchCriteria.Add(searchCriterias);
            if (_searchCriteria.Count == 2)
            {
                UIElement _uiElement = _searchCriteria[0].grdMain.Children.Cast<UIElement>().Where(x => x.Visibility == System.Windows.Visibility.Visible).FirstOrDefault();
                if (_uiElement is Grid)
                {
                    Grid tempGrid = _uiElement as Grid;
                    var tempButton = tempGrid.Children.OfType<Button>().Where(x => x.GetType() == typeof(Button));
                    foreach (Button tempbtn in tempButton)
                    {
                        tempbtn.Visibility = System.Windows.Visibility.Visible;
                    }
                }
            }
            itcSearchCriteria.ItemsSource = _searchCriteria;
        }

        private void CreateAddButtonContextMenu()
        {
            _listSearchAttributes = _searchAttributes.Select(x => x.Content.ToString()).ToList();
            _cmenuSearchAttribute.Items.Clear();
            foreach (var item in _listSearchAttributes)
            {
                MenuItem mItem = new MenuItem() { Header = item.ToString() };
                mItem.Click += new RoutedEventHandler(mItem_Click);
                _cmenuSearchAttribute.Items.Add(mItem);
            }
        }

        private void itcSearchCriteria_Loaded(object sender, RoutedEventArgs e)
        {
            if (scrollVSearchCriteria.ActualWidth > 20)
                itcSearchCriteria.Width = scrollVSearchCriteria.ActualWidth - 20;
        }

        private void scrollVSearchCriteria_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            scrollVSearchCriteria.SizeChanged -= scrollVSearchCriteria_SizeChanged;
            if (scrollVSearchCriteria.ActualWidth > 20)
                itcSearchCriteria.Width = scrollVSearchCriteria.ActualWidth - 20;
            if (e.WidthChanged && !e.NewSize.IsEmpty)
            {
                if (e.NewSize.Width > 10)
                {
                    foreach (var item in _searchCriteria)
                    {
                        if (e.NewSize.Width > 10)
                        item.Width = e.NewSize.Width - 10;
                    }
                }
            }
            scrollVSearchCriteria.SizeChanged += new SizeChangedEventHandler(scrollVSearchCriteria_SizeChanged);
        }
    }
}
