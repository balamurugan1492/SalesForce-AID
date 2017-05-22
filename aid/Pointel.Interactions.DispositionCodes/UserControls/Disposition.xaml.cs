using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Pointel.Interactions.DispositionCodes.InteractionHandler;
using Pointel.Interactions.DispositionCodes.Settings;
using Pointel.Interactions.DispositionCodes.Utilities;
using System.Threading;

namespace Pointel.Interactions.DispositionCodes.UserControls
{
    /// <summary>
    /// Interaction logic for Disposition.xaml
    /// </summary>
    public partial class Disposition : UserControl
    {
        private string _previousDispositionCode = string.Empty;
        private int _combo = 0;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");

        private Dictionary<string, string> _selectedDisposition = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, string>> _selectedSubDisposition = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> _dispositionTree = new Dictionary<string, string>();
        private Dictionary<string, string> _rdDispositionCodes = new Dictionary<string, string>();
        private Pointel.Interactions.IPlugins.MediaTypes _selectedInteractionType;
        public delegate void NotifyDispositionCode(Pointel.Interactions.IPlugins.MediaTypes mediaType, DispositionData data);
        public event NotifyDispositionCode NotifyDispositionCodeEvent;
        private string _dispositionCodesSelected = string.Empty;
        private string _interactionId = string.Empty;
        private Listener _listener = new Listener();

        private Dictionary<string, string> _mainCodes = new Dictionary<string, string>();
        private Dictionary<string, Dictionary<string, string>> _selectedSubcodes = new Dictionary<string, Dictionary<string, string>>();
        private Dictionary<string, string> _receivedDispositionCodes = new Dictionary<string, string>();

        private Datacontext _datacontext;
        public Disposition(Datacontext datacontext)
        {
            _datacontext = datacontext;
        }

        public void Dispositions(Pointel.Interactions.IPlugins.MediaTypes interactionType, DispositionData dispositionData)
        {
            try
            {
                InitializeComponent();
                Loaded += Disposition_Loaded;
                _previousDispositionCode = string.Empty;
                _selectedInteractionType = interactionType;
                _interactionId = dispositionData.InteractionID;
                switch (_selectedInteractionType)
                {
                    case IPlugins.MediaTypes.Voice:
                        _mainCodes = _datacontext.VoiceDispositionCodes;
                        _selectedSubcodes = _datacontext.VoiceSubDispositionCodes;
                        break;
                    case IPlugins.MediaTypes.Email:
                        _mainCodes = _datacontext.EmailDispositionCodes;
                        _selectedSubcodes = _datacontext.EmailSubDispositionCodes;
                        break;
                    case IPlugins.MediaTypes.Chat:
                        _mainCodes = _datacontext.ChatDispositionCodes;
                        _selectedSubcodes = _datacontext.ChatSubDispositionCodes;
                        break;
                }
                LoadDispositionCodes();
            }
            catch (Exception ex)
            {
                _logger.Error("DispositionWindow Constructor : Error : " + ex.Message.ToString());
            }
        }

        void Disposition_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        public void ReLoadDispositionCodes(Dictionary<string, string> dispositionCodes, string connectionId)
        {
            try
            {
                _interactionId = connectionId;
                if (_isdefaultSent)
                { _isdefaultSent = false; return; }
                if (dispositionCodes == null || dispositionCodes.Count <= 0)
                    return;
                stkMultiCodePanel.Children.Clear();
                _receivedDispositionCodes = dispositionCodes;
                _dispositionTree.Clear();
                KeyValuePair<string, string> selectedDispositionCode = new KeyValuePair<string, string>();
                if (!_datacontext.IsMultiDispositionEnabled)
                {
                    if (_receivedDispositionCodes.ContainsKey(_datacontext.DispositionCodeKey))
                    {
                        if (_receivedDispositionCodes[_datacontext.DispositionCodeKey] == "None")
                        {
                            selectedDispositionCode = new KeyValuePair<string, string>("None", "None");
                        }
                        else
                            selectedDispositionCode = _receivedDispositionCodes.Where(x =>
                                        x.Value.Equals(_receivedDispositionCodes[_datacontext.DispositionCodeKey].ToString())).FirstOrDefault();
                    }
                    StackPanel stPanel = new StackPanel()
                    {
                        Name = "stk0",
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(5)
                    };

                    TextBlock txtMain = new TextBlock()
                    {
                        Text = _datacontext.DispositionName,
                        Name = "txt0",
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        FontFamily = new System.Windows.Media.FontFamily("Calibri"),
                        Background = Brushes.White,
                        Foreground = (Brush)(new BrushConverter().ConvertFrom("#007edf")),
                        Margin = new Thickness(2),
                        Width = double.NaN,
                        FontSize = 14,
                        Height = 20
                    };
                    stPanel.Children.Add(txtMain);
                    foreach (string disposition in _mainCodes.Keys)
                    {
                        var rdbutton2 = new RadioButton();
                        rdbutton2.GroupName = "DispositionCode";
                        if (disposition == "DefaultDispositionCode")
                            continue;
                        //rdbutton2.Checked += radioDispButton_Checked;
                        rdbutton2.Content = disposition;
                        //if (selectedDispositionCode.Value.Split(',').Length <= 1)
                        {
                            if (disposition.Split(',')[0] == selectedDispositionCode.Value.Split(',')[0] && _receivedDispositionCodes.Count == 1)
                                rdbutton2.IsChecked = true;
                        }
                        rdbutton2.Margin = new Thickness(2);
                        rdbutton2.Height = 18;
                        rdbutton2.Width = Double.NaN;
                        stPanel.Children.Add(rdbutton2);
                    }

                    if (!_mainCodes.ContainsKey("None"))
                    {
                        var rdbutton2 = new RadioButton();
                        rdbutton2.GroupName = "DispositionCode";
                        //rdbutton2.Checked += radioDispButton_Checked;
                        rdbutton2.Content = "None";
                        if (rdbutton2.Content.ToString() == selectedDispositionCode.Key.ToString())
                            rdbutton2.IsChecked = true;
                        rdbutton2.Margin = new Thickness(2);
                        rdbutton2.Height = 18;
                        rdbutton2.Width = Double.NaN;
                        stPanel.Children.Add(rdbutton2);
                    }
                    stkMultiCodePanel.Children.Add(stPanel);
                }
                else
                {
                    if (_selectedSubcodes.Count == 0)//No child collection configured so add in checkbox or radio button
                    {
                        if (_datacontext.IsMultiDispositionEnabled)
                        {
                            selectedDispositionCode = _receivedDispositionCodes.Where(x =>
                                       x.Value.Equals(_receivedDispositionCodes[_datacontext.DispositionCodeKey].ToString())).FirstOrDefault();

                            string[] dispositionListArray = selectedDispositionCode.Value.Split(',');
                            Dictionary<string, string> dispositionList = new Dictionary<string, string>();
                            if (dispositionListArray.Length > 0)
                            {
                                foreach (string value in dispositionListArray)
                                {
                                    if (value != "")
                                    {
                                        KeyValuePair<string, string> kvp = _mainCodes.Where(a => a.Key.Equals(value)).First();
                                        if (kvp.Key != null)
                                        {
                                            dispositionList.Add(kvp.Key, kvp.Value);
                                        }
                                    }
                                }
                            }
                            StackPanel stPanel = new StackPanel()
                            {
                                Name = "stk0",
                                Orientation = Orientation.Vertical,
                                Margin = new Thickness(5)
                            };
                            foreach (string disposition in _mainCodes.Keys)
                            {
                                var chkBox = new CheckBox();
                                //rdButton.GroupName = "DispositionCode";
                                //chkBox.Checked += chkBox_Checked;
                                chkBox.Content = disposition;
                                if (disposition == "DefaultDispositionCode")
                                    continue;

                                if (dispositionList.ContainsKey(disposition.Split(',')[0]))
                                    chkBox.IsChecked = true;

                                chkBox.Margin = new Thickness(2);
                                chkBox.Height = 18;
                                chkBox.Width = Double.NaN;
                                stPanel.Children.Add(chkBox);
                            }
                            stkMultiCodePanel.Children.Add(stPanel);
                        }
                    }
                    else //Has child collection
                    {
                        if (dispositionCodes.Count > 1)// Has child collection and has selected many dispositions
                        {
                            var element = _mainCodes.Where(x => x.Value.Contains(dispositionCodes[_datacontext.DispositionName])).FirstOrDefault();//.Select(xy=> xy.Key);
                            if (element.Key != null)
                            {
                                TextBlock txtMain = new TextBlock()
                                {
                                    Text = "Disposition",
                                    Name = "txt0",
                                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                                    FontFamily = new System.Windows.Media.FontFamily("Calibri"),
                                    Background = Brushes.White,
                                    Foreground = (Brush)(new BrushConverter().ConvertFrom("#007edf")),
                                    Margin = new Thickness(2),
                                    Width = double.NaN,
                                    FontSize = 14,
                                    Height = 20
                                };
                                ComboBox cmbMain = new ComboBox()
                                {
                                    Name = "cmb0",
                                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                                    Height = 23,
                                    Margin = new Thickness(2),
                                    Width = 115
                                };
                                cmbMain.SelectionChanged += new SelectionChangedEventHandler(cmbChildTraverse_SelectionChanged);
                                cmbMain.ItemsSource = _mainCodes;
                                cmbMain.DisplayMemberPath = "Key";
                                cmbMain.SelectedValuePath = "Value";
                                StackPanel stPanel = new StackPanel()
                                {
                                    Name = "stk0",
                                    Orientation = Orientation.Vertical,
                                    Margin = new Thickness(5)
                                };
                                stPanel.Children.Add(txtMain);
                                stPanel.Children.Add(cmbMain);
                                stkMultiCodePanel.Children.Add(stPanel);
                                cmbMain.ItemsSource = _mainCodes;
                                cmbMain.SelectedItem = element;
                            }
                        }
                        else //Has child and selected only one disposition code
                        {
                            KeyValuePair<string, string> kvp = dispositionCodes.First();
                            bool isChildAvailable = false;
                            isChildAvailable = (_mainCodes.Values.ToList()).Any(b => (_selectedSubcodes.Keys.ToList()).Any(a => b.Contains(a)));
                            if (isChildAvailable)//Selected disposition has collection so display combobox
                            {
                                TextBlock txtMain = new TextBlock()
                                {
                                    Text = "Disposition",
                                    Name = "txt0",
                                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                                    FontFamily = new System.Windows.Media.FontFamily("Calibri"),
                                    Background = Brushes.White,
                                    Foreground = (Brush)(new BrushConverter().ConvertFrom("#007edf")),
                                    Margin = new Thickness(2),
                                    Width = double.NaN,
                                    FontSize = 14,
                                    Height = 20
                                };
                                ComboBox cmbMain = new ComboBox()
                                {
                                    Name = "cmb0",
                                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                                    Height = 23,
                                    Margin = new Thickness(2),
                                    Width = 115
                                };
                                cmbMain.SelectionChanged += new SelectionChangedEventHandler(cmbChildTraverse_SelectionChanged);
                                cmbMain.ItemsSource = _mainCodes;
                                cmbMain.DisplayMemberPath = "Key";
                                cmbMain.SelectedValuePath = "Value";
                                StackPanel stPanel = new StackPanel()
                                {
                                    Name = "stk0",
                                    Orientation = Orientation.Vertical,
                                    Margin = new Thickness(5)
                                };
                                stPanel.Children.Add(txtMain);
                                stPanel.Children.Add(cmbMain);
                                stkMultiCodePanel.Children.Add(stPanel);
                                cmbMain.ItemsSource = _mainCodes;
                                KeyValuePair<string, string> itemSelected = new KeyValuePair<string, string>();
                                if (!string.IsNullOrEmpty(kvp.Key))
                                {
                                    if (_mainCodes.ContainsKey(kvp.Value))
                                        itemSelected = _mainCodes.Where(v => v.Key.Equals(kvp.Value)).First();
                                }
                                if (itemSelected.Value != null)
                                {
                                    if (!itemSelected.Value.Contains(','))
                                        cmbMain.SelectedItem = itemSelected;
                                }
                            }
                        }

                    }
                }
                foreach (ComboBox cmbBox in FindVisualChildren<ComboBox>(stkMultiCodePanel))
                {
                    cmbBox.SelectionChanged -= new SelectionChangedEventHandler(cmbChildTraverse_SelectionChanged);
                    cmbBox.SelectionChanged += new SelectionChangedEventHandler(cmbbox_SelectionChanged);
                }
                foreach (RadioButton rdButton in FindVisualChildren<RadioButton>(stkMultiCodePanel))
                {
                    rdButton.Checked -= radioDispButton_Checked;
                    rdButton.Checked += radioDispButton_Checked;
                }
                foreach (CheckBox chkBox in FindVisualChildren<CheckBox>(stkMultiCodePanel))
                {
                    chkBox.Unchecked += chkBox_Checked;
                    chkBox.Checked += chkBox_Checked;
                }
            }
            catch (Exception ex)
            {
                string error = ex.Message.ToString();
            }
        }

        void cmbChildTraverse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cmbBoxSelect = sender as ComboBox;
                string selectedValue = cmbBoxSelect.SelectedValue.ToString();
                if (cmbBoxSelect.Name == "cmb0")
                {
                    UIElement firstIdex = stkMultiCodePanel.Children[0];
                    stkMultiCodePanel.Children.Clear();
                    stkMultiCodePanel.Children.Add(firstIdex);
                    _dispositionTree.Clear();
                    _combo = 0;
                }
                else
                {
                    UIElement[] element = new UIElement[stkMultiCodePanel.Children.Count];
                    for (int count = 0; count < element.Count(); count++)
                    {
                        element[count] = stkMultiCodePanel.Children[count];
                    }
                    foreach (UIElement child in element)
                    {
                        if (child is StackPanel)
                        {
                            StackPanel stk = (StackPanel)child;
                            try
                            {
                                int name = Convert.ToInt32(cmbBoxSelect.Name.Replace("cmb", string.Empty).ToString());
                                TextBlock tempText = null;
                                if (stk.Children[0] is TextBlock)
                                    tempText = (TextBlock)stk.Children[0];
                                int deleteName = Convert.ToInt32(stk.Name.Replace("stk", string.Empty).ToString());
                                if (deleteName > name)
                                {
                                    stkMultiCodePanel.Children.Remove(stk);
                                }
                                if (deleteName >= name)
                                {
                                    if (tempText != null && _dispositionTree.Keys.Contains(tempText.Text))
                                        _dispositionTree.Remove(tempText.Text);
                                }
                            }
                            catch { }
                        }
                    }
                }
                //********************************************************************//
                if (cmbBoxSelect.SelectedValue.ToString() != string.Empty)
                {
                    if (cmbBoxSelect.SelectedValue.ToString().Split(',').Length > 1)
                    {
                        selectedValue = cmbBoxSelect.SelectedValue.ToString().Split(',')[1];
                    }
                    //else
                    //    cmbBoxSelect.SelectedValue.ToString();
                    if (_selectedSubcodes.ContainsKey(selectedValue))
                    {
                        bool isChildPresent = (_selectedSubcodes[selectedValue].Values.ToList()).Any(b => (_selectedSubcodes.Keys.ToList()).Any(a => b.Contains(a)));
                        //Condition checks whether the selected collection has a child collection
                        if (isChildPresent)
                        {
                            _combo++;
                            Dictionary<string, string> elementstoParse = new Dictionary<string, string>();
                            elementstoParse = _selectedSubcodes[selectedValue];
                            if (_receivedDispositionCodes.ContainsKey(selectedValue))
                            {
                                var elements = elementstoParse.Where(x => x.Value.Contains(_receivedDispositionCodes[selectedValue])).FirstOrDefault();//.Select(xy=> xy.Key);
                                if (elements.Key != null)
                                {
                                    ComboBox cmbBox = new ComboBox();
                                    cmbBox.ItemsSource = _selectedSubcodes[selectedValue];
                                    cmbBox.DisplayMemberPath = "Key";
                                    cmbBox.SelectedValuePath = "Value";
                                    cmbBox.Name = "cmb" + _combo.ToString();
                                    cmbBox.Height = 23;
                                    cmbBox.Width = 115;
                                    cmbBox.Margin = new Thickness(2);
                                    cmbBox.SelectionChanged += new SelectionChangedEventHandler(cmbChildTraverse_SelectionChanged);

                                    TextBlock subDispositionCode = new TextBlock();
                                    subDispositionCode.Width = double.NaN;
                                    subDispositionCode.FontSize = 14;
                                    subDispositionCode.Name = "txt" + _combo.ToString();
                                    subDispositionCode.Margin = new Thickness(2);
                                    subDispositionCode.Height = 20;
                                    var bc = new BrushConverter();
                                    subDispositionCode.Foreground = (Brush)bc.ConvertFrom("#007edf");
                                    subDispositionCode.Background = new SolidColorBrush(Colors.White);
                                    subDispositionCode.Text = selectedValue;
                                    subDispositionCode.FontFamily = new System.Windows.Media.FontFamily("Calibri");
                                    StackPanel stPanel = new StackPanel()
                                    {
                                        Orientation = Orientation.Vertical,
                                        Name = "stk" + _combo.ToString(),
                                        Margin = new Thickness(5)
                                    };

                                    try
                                    {
                                        var textblock = LogicalTreeHelper.FindLogicalNode(stkMultiCodePanel, cmbBoxSelect.Name.Replace("cmb", "txt"));
                                        if (textblock is TextBlock)
                                            _dispositionTree.Add((textblock as TextBlock).Text, (cmbBoxSelect.SelectedValue.ToString().Split(','))[0]);
                                    }
                                    catch { }
                                    stPanel.Children.Add(subDispositionCode);
                                    stPanel.Children.Add(cmbBox);

                                    stkMultiCodePanel.Children.Add(stPanel);
                                    cmbBox.SelectedItem = elements;
                                }
                            }
                            else
                            {
                                var elementSelected = elementstoParse.Where(x => x.Key.Contains(_receivedDispositionCodes[_datacontext.DispositionCodeKey])).FirstOrDefault();

                                if (_receivedDispositionCodes.ContainsKey(_datacontext.DispositionCodeKey))
                                {
                                    var collection = elementstoParse.Where(x => x.Value.Contains(',')).FirstOrDefault();
                                    if (collection.Key != null)//****************//Next selection has dropdownlist
                                    {
                                        ComboBox cmbBox = new ComboBox();
                                        cmbBox.ItemsSource = _selectedSubcodes[selectedValue];
                                        cmbBox.DisplayMemberPath = "Key";
                                        cmbBox.SelectedValuePath = "Value";
                                        cmbBox.Name = "cmb" + _combo.ToString();
                                        cmbBox.Height = 23;
                                        cmbBox.Width = 115;
                                        cmbBox.Margin = new Thickness(2);
                                        cmbBox.SelectionChanged += new SelectionChangedEventHandler(cmbChildTraverse_SelectionChanged);

                                        TextBlock subDispositionCode = new TextBlock();
                                        subDispositionCode.Width = double.NaN;
                                        subDispositionCode.FontSize = 14;
                                        subDispositionCode.Name = "txt" + _combo.ToString();
                                        subDispositionCode.Margin = new Thickness(2);
                                        subDispositionCode.Height = 20;
                                        var bc = new BrushConverter();
                                        subDispositionCode.Foreground = (Brush)bc.ConvertFrom("#007edf");
                                        subDispositionCode.Background = new SolidColorBrush(Colors.White);
                                        subDispositionCode.Text = selectedValue;
                                        subDispositionCode.FontFamily = new System.Windows.Media.FontFamily("Calibri");
                                        StackPanel stPanel = new StackPanel()
                                        {
                                            Orientation = Orientation.Vertical,
                                            Name = "stk" + _combo.ToString(),
                                            Margin = new Thickness(5)
                                        };

                                        try
                                        {
                                            var textblock = LogicalTreeHelper.FindLogicalNode(stkMultiCodePanel, cmbBoxSelect.Name.Replace("cmb", "txt"));
                                            if (textblock is TextBlock)
                                                _dispositionTree.Add((textblock as TextBlock).Text, (cmbBoxSelect.SelectedValue.ToString().Split(','))[0]);
                                        }
                                        catch { }
                                        stPanel.Children.Add(subDispositionCode);
                                        stPanel.Children.Add(cmbBox);

                                        stkMultiCodePanel.Children.Add(stPanel);
                                        cmbBox.SelectedItem = elementSelected;
                                    }
                                    else//Next selection has radiobutton
                                    {
                                        KeyValuePair<string, string> selectedDispositionCode = _receivedDispositionCodes.Where(x =>
                                            x.Value.Equals(_receivedDispositionCodes[_datacontext.DispositionCodeKey].ToString())).FirstOrDefault();
                                        var subCodes = _selectedSubcodes[selectedValue];
                                        StackPanel stkLastChild = new StackPanel();
                                        _rdDispositionCodes = elementstoParse;
                                        foreach (string disposition in elementstoParse.Keys)
                                        {
                                            var rdbutton2 = new RadioButton();
                                            rdbutton2.GroupName = "DispositionCode";
                                            // rdbutton2.Checked += radioDispButton_Checked;
                                            rdbutton2.Content = disposition;
                                            rdbutton2.Margin = new Thickness(2);
                                            rdbutton2.Height = 18;
                                            rdbutton2.Width = Double.NaN;
                                            if (subCodes[disposition] == selectedDispositionCode.Key)
                                                rdbutton2.IsChecked = true;
                                            stkLastChild.Children.Add(rdbutton2);
                                        }

                                        if (!subCodes.ContainsKey("None"))
                                        {
                                            var rdbutton2 = new RadioButton();
                                            rdbutton2.GroupName = "DispositionCode";
                                            //rdbutton2.Checked += radioDispButton_Checked;
                                            rdbutton2.Content = "None";
                                            rdbutton2.Margin = new Thickness(2);
                                            rdbutton2.Height = 18;
                                            rdbutton2.Width = Double.NaN;
                                            stkLastChild.Children.Add(rdbutton2);
                                        }
                                        TextBlock subDispositionCode = new TextBlock();
                                        subDispositionCode.Width = double.NaN;
                                        subDispositionCode.FontSize = 14;
                                        subDispositionCode.Name = "txt" + _combo.ToString();
                                        subDispositionCode.Margin = new Thickness(2);
                                        subDispositionCode.Height = 20;
                                        var bc = new BrushConverter();
                                        subDispositionCode.Foreground = (Brush)bc.ConvertFrom("#007edf");
                                        subDispositionCode.Background = new SolidColorBrush(Colors.White);
                                        subDispositionCode.Text = selectedValue;
                                        subDispositionCode.FontFamily = new System.Windows.Media.FontFamily("Calibri");
                                        StackPanel stPanel = new StackPanel()
                                        {
                                            Orientation = Orientation.Vertical,
                                            Name = "stk" + _combo.ToString(),
                                            Margin = new Thickness(5)
                                        };

                                        stPanel.Children.Add(subDispositionCode);
                                        stPanel.Children.Add(stkLastChild);

                                        stkMultiCodePanel.Children.Add(stPanel);

                                    }
                                }
                            }
                        }
                        else//No child collection present 
                        {
                            var subCodes = _selectedSubcodes[selectedValue];
                            StackPanel stkLastChild = new StackPanel();
                            _rdDispositionCodes = subCodes;
                            KeyValuePair<string, string> codeValue = _receivedDispositionCodes.Where(x =>
                                x.Key.Equals(_datacontext.DispositionCodeKey)).FirstOrDefault();
                            if (_datacontext.IsMultiDispositionEnabled)//Multi disposition is enabled so add in check box
                            {
                                Dictionary<string, string> dispositionList = new Dictionary<string, string>();
                                if (codeValue.Value != null && codeValue.Value != string.Empty)
                                {
                                    string[] dispositionListArray = codeValue.Value.Split(',');
                                    if (dispositionListArray.Length > 0)
                                    {
                                        foreach (string value in dispositionListArray)
                                        {
                                            if (value != "")
                                            {
                                                KeyValuePair<string, string> kvp = subCodes.Where(a => a.Key.Equals(value)).First();
                                                if (kvp.Key != null)
                                                {
                                                    dispositionList.Add(kvp.Key, kvp.Value);
                                                }
                                            }
                                        }
                                    }
                                }
                                foreach (string disposition in subCodes.Keys)
                                {
                                    var chkBox = new CheckBox();
                                    //chkBox.Checked += chkBox_Checked;
                                    chkBox.Content = disposition;
                                    if (disposition == "DefaultDispositionCode")
                                        continue;
                                    if (dispositionList.ContainsKey(disposition))
                                        chkBox.IsChecked = true;
                                    chkBox.Margin = new Thickness(2);
                                    chkBox.Height = 18;
                                    chkBox.Width = Double.NaN;
                                    stkLastChild.Children.Add(chkBox);
                                }
                            }
                            else
                            {
                                foreach (string disposition in subCodes.Keys)
                                {
                                    var rdbutton2 = new RadioButton();
                                    rdbutton2.GroupName = "DispositionCode";
                                    if (disposition == "DefaultDispositionCode")
                                        continue;
                                    // rdbutton2.Checked += radioDispButton_Checked;
                                    rdbutton2.Content = disposition;
                                    rdbutton2.Margin = new Thickness(2);
                                    rdbutton2.Height = 18;
                                    rdbutton2.Width = Double.NaN;
                                    if (codeValue.Value != null && codeValue.Value != string.Empty)
                                    {
                                        if (subCodes[disposition] == codeValue.Value)
                                            rdbutton2.IsChecked = true;
                                    }
                                    stkLastChild.Children.Add(rdbutton2);
                                }

                                if (!subCodes.ContainsKey("None"))
                                {
                                    var rdbutton2 = new RadioButton();
                                    rdbutton2.GroupName = "DispositionCode";
                                    //rdbutton2.Checked += radioDispButton_Checked;

                                    if (codeValue.Value != null && codeValue.Value != string.Empty)
                                    {
                                        if (codeValue.Value == "None")
                                            rdbutton2.IsChecked = true;
                                    }
                                    rdbutton2.Content = "None";
                                    rdbutton2.Margin = new Thickness(2);
                                    rdbutton2.Height = 18;
                                    rdbutton2.Width = Double.NaN;
                                    stkLastChild.Children.Add(rdbutton2);
                                }
                            }
                            TextBlock subDispositionCode = new TextBlock();
                            subDispositionCode.Width = double.NaN;
                            subDispositionCode.FontSize = 14;
                            subDispositionCode.Name = "txt" + _combo.ToString();
                            subDispositionCode.Margin = new Thickness(2);
                            subDispositionCode.Height = 20;
                            var bc = new BrushConverter();
                            subDispositionCode.Foreground = (Brush)bc.ConvertFrom("#007edf");
                            subDispositionCode.Background = new SolidColorBrush(Colors.White);
                            subDispositionCode.Text = selectedValue;
                            subDispositionCode.FontFamily = new System.Windows.Media.FontFamily("Calibri");
                            StackPanel stPanel = new StackPanel()
                            {
                                Orientation = Orientation.Vertical,
                                Name = "stk" + _combo.ToString(),
                                Margin = new Thickness(5)
                            };

                            stPanel.Children.Add(subDispositionCode);
                            stPanel.Children.Add(stkLastChild);

                            stkMultiCodePanel.Children.Add(stPanel);

                            try
                            {

                                var textblock = LogicalTreeHelper.FindLogicalNode(stkMultiCodePanel, cmbBoxSelect.Name.Replace("cmb", "txt"));
                                if (textblock is TextBlock)
                                    _dispositionTree.Add((textblock as TextBlock).Text, (cmbBoxSelect.SelectedValue.ToString().Split(','))[0]);
                            }
                            catch { }
                        }
                    }
                    else
                    {
                        try
                        {

                            var textblock = LogicalTreeHelper.FindLogicalNode(stkMultiCodePanel, cmbBoxSelect.Name.Replace("cmb", "txt"));
                            if (textblock is TextBlock)
                                _dispositionTree.Add((textblock as TextBlock).Text, (cmbBoxSelect.SelectedValue.ToString().Split(','))[0]);
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LoadDispositionCodes() : Error : " + ex.Message.ToString());
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        public void LoadDispositionCodes()
        {
            try
            {
                if (_datacontext.IsMultiDispositionEnabled)
                {
                    stkMultiCodePanel.Children.Clear();
                    ComboBox cmbMain = new ComboBox()
                    {
                        Name = "cmb0",
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                        Height = 23,
                        Margin = new Thickness(2),
                        Width = 115
                    };
                    StackPanel stPanel = new StackPanel()
                    {
                        Name = "stk0",
                        Orientation = Orientation.Vertical,
                        Margin = new Thickness(5)
                    };
                    switch (_selectedInteractionType)
                    {
                        case IPlugins.MediaTypes.Voice:
                            cmbMain.ItemsSource = _datacontext.VoiceDispositionCodes;
                            _selectedDisposition = _datacontext.VoiceDispositionCodes;
                            _selectedSubDisposition = _datacontext.VoiceSubDispositionCodes;
                            break;
                        case IPlugins.MediaTypes.Email:
                            cmbMain.ItemsSource = _datacontext.EmailDispositionCodes;
                            _selectedDisposition = _datacontext.EmailDispositionCodes;
                            _selectedSubDisposition = _datacontext.EmailSubDispositionCodes;
                            break;
                        case IPlugins.MediaTypes.Chat:
                            cmbMain.ItemsSource = _datacontext.ChatDispositionCodes;
                            _selectedDisposition = _datacontext.ChatDispositionCodes;
                            _selectedSubDisposition = _datacontext.ChatSubDispositionCodes;
                            break;
                    }

                    if (_selectedSubDisposition.Count > 0)
                    {
                        TextBlock txtMain = new TextBlock()
                        {
                            Text = _datacontext.DispositionName,
                            Name = "txt0",
                            HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                            FontFamily = new System.Windows.Media.FontFamily("Calibri"),
                            Background = Brushes.White,
                            Foreground = (Brush)(new BrushConverter().ConvertFrom("#007edf")),
                            Margin = new Thickness(2),
                            Width = double.NaN,
                            FontSize = 14,
                            Height = 20
                        };
                        cmbMain.SelectionChanged += new SelectionChangedEventHandler(cmbbox_SelectionChanged);
                        cmbMain.DisplayMemberPath = "Key";
                        cmbMain.SelectedValuePath = "Value";
                        cmbMain.Width = 115;
                        stPanel.Children.Add(txtMain);
                        stPanel.Children.Add(cmbMain);
                    }
                    else
                    {
                        //---- Multi Disposition Code is enabled but No sub disposition code is configured
                        if (_selectedDisposition.Count > 0)
                        {
                            _rdDispositionCodes = _selectedDisposition;
                            foreach (string disposition in _selectedDisposition.Keys)
                            {
                                var chkBox = new CheckBox();
                                //rdButton.GroupName = "DispositionCode";
                                //chkBox.Checked += chkBox_Checked;
                                //chkBox.Unchecked += chkBox_Checked;
                                chkBox.Content = disposition;

                                if (disposition == "DefaultDispositionCode")
                                    continue;
                                var rdbutton2 = new RadioButton();
                                rdbutton2.GroupName = "DispositionCode";
                                //rdbutton2.Checked += radioDispButton_Checked;
                                //rdbutton2.MouseEnter += radioDispButton_MouseEnter;
                                if (_selectedDisposition.ContainsKey("DefaultDispositionCode"))
                                {
                                    //if (disposition == _selectedDisposition["DefaultDispositionCode"])
                                    //    chkBox.IsChecked = true;
                                }

                                chkBox.Margin = new Thickness(2);
                                chkBox.Height = 18;
                                chkBox.Width = Double.NaN;
                                stPanel.Children.Add(chkBox);
                            }
                        }
                        //else
                        //    stPanel.Children.Add(cmbMain);
                    }
                    stkMultiCodePanel.Children.Add(stPanel);

                    foreach (CheckBox chkBox in FindVisualChildren<CheckBox>(stkMultiCodePanel))
                    {
                        chkBox.Unchecked += chkBox_Checked;
                        chkBox.Checked += chkBox_Checked;
                    }
                }
                else
                {
                    switch (_selectedInteractionType)
                    {
                        case IPlugins.MediaTypes.Voice:
                            AddSingleDispositionCodes(_datacontext.VoiceDispositionCodes);
                            break;
                        case IPlugins.MediaTypes.Email:
                            AddSingleDispositionCodes(_datacontext.EmailDispositionCodes);
                            break;
                        case IPlugins.MediaTypes.Chat:
                            AddSingleDispositionCodes(_datacontext.ChatDispositionCodes);
                            break;

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LoadDispositionCodes() : Error : " + ex.Message.ToString());
            }
        }

        private void chkBox_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                _dispositionCodesSelected = "";
                foreach (CheckBox chkBox in FindVisualChildren<CheckBox>(stkMultiCodePanel))
                {
                    if (chkBox.IsChecked == true)
                    {
                        if (_dispositionCodesSelected == string.Empty)
                            _dispositionCodesSelected += _rdDispositionCodes.Where(x => x.Key.Contains(chkBox.Content.ToString())).First().Key;
                        else
                            _dispositionCodesSelected += "," + _rdDispositionCodes.Where(x => x.Key.Contains(chkBox.Content.ToString())).First().Key;
                    }
                }
                if (btnDone.Visibility == System.Windows.Visibility.Hidden || btnDone.Visibility == System.Windows.Visibility.Collapsed)
                {
                    NotifyDispositionCodeEvent.Invoke(_selectedInteractionType, new DispositionData()
                    {
                        DispostionCollection = _dispositionTree,
                        DispostionCode = _dispositionCodesSelected,
                        InteractionID = _interactionId
                    });
                    _dispositionCodesSelected = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("LoadDispositionCodes() : Error : " + ex.Message.ToString());
            }
        }

        public void ReLoadDispositionCodes(bool markDone)
        {
            if (markDone)
                btnDone.Visibility = System.Windows.Visibility.Visible;
            else
                btnDone.Visibility = System.Windows.Visibility.Collapsed;

            ///other reload functionalities done here
        }

        void Listener__refreshUI()
        {
            LoadDispositionCodes();
        }
        private bool _isdefaultSent = false;
        void AddSingleDispositionCodes(Dictionary<string, string> dispositionCodes)
        {
            try
            {
                stkMultiCodePanel.Children.Clear();
                _rdDispositionCodes = dispositionCodes;

                TextBlock txtMain = new TextBlock()
                {
                    Text = _datacontext.DispositionName,
                    Name = "txt0",
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                    FontFamily = new System.Windows.Media.FontFamily("Calibri"),
                    Background = Brushes.White,
                    Foreground = (Brush)(new BrushConverter().ConvertFrom("#007edf")),
                    Margin = new Thickness(2),
                    Width = double.NaN,
                    FontSize = 14,
                    Height = 20
                };

                StackPanel stPanel = new StackPanel()
                {
                    Orientation = Orientation.Vertical,
                    Name = "stk" + _combo.ToString(),
                    Margin = new Thickness(5)
                };
                stPanel.Children.Add(txtMain);
                if (!dispositionCodes.ContainsKey("None"))
                {
                    dispositionCodes.Add("None", "None");
                    //var rdbutton2 = new RadioButton();
                    //rdbutton2.GroupName = "DispositionCode";
                    ////rdbutton2.Checked += radioDispButton_Checked;
                    //rdbutton2.Content = "None";
                    //rdbutton2.Margin = new Thickness(2);
                    //rdbutton2.Height = 18;
                    //rdbutton2.Width = Double.NaN;
                    //stPanel.Children.Add(rdbutton2);
                }
                bool _defaultDispositionAvailable = dispositionCodes.ContainsKey("DefaultDispositionCode");
                stkMultiCodePanel.Children.Add(stPanel);
                foreach (var item in dispositionCodes)
                {
                    //if (item.Key == "DefaultDispositionCode")
                    //    continue;
                    var rdbutton2 = new RadioButton();
                    rdbutton2.GroupName = "DispositionCode";
                    //rdbutton2.Checked += radioDispButton_Checked;
                    //rdbutton2.MouseEnter += radioDispButton_MouseEnter;
                    if (dispositionCodes.ContainsKey("DefaultDispositionCode"))
                    {
                        if (item.Key == dispositionCodes["DefaultDispositionCode"])
                        {
                            rdbutton2.IsChecked = true;
                            _isdefaultSent = true;

                            NotifyDispositionCodeEvent.Invoke(_selectedInteractionType, new DispositionData()
                            {
                                DispostionCollection = _dispositionTree,
                                DispostionCode = item.Key,
                                InteractionID = _interactionId
                            });
                        }



                        _previousDispositionCode = item.Key;
                        _dispositionCodesSelected = string.Empty;
                    }
                    if (item.Key == "None")
                        rdbutton2.IsChecked = !_defaultDispositionAvailable;

                    rdbutton2.Content = item.Key;
                    rdbutton2.Margin = new Thickness(2);
                    rdbutton2.Height = 18;
                    rdbutton2.Width = Double.NaN;
                    stPanel.Children.Add(rdbutton2);
                }


                foreach (RadioButton rdButton in FindVisualChildren<RadioButton>(stkMultiCodePanel))
                {
                    rdButton.Checked += radioDispButton_Checked;
                    //rdButton.Unchecked += rdButton_Unchecked;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("AddSingleDispositionCodes() : Error : " + ex.Message.ToString());
            }
        }

        //void rdButton_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    Thread _thread = new Thread(delegate(object s)
        //    {
        //        try
        //        {
        //            Thread.Sleep(50);
        //            stkMultiCodePanel.Dispatcher.Invoke((Action)(delegate
        //            {
        //                if (!FindVisualChildren<RadioButton>(stkMultiCodePanel).Any(x => x.IsChecked == true))
        //                    (s as RadioButton).IsChecked = true;
        //            }));
                    
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.Error("rdButton_Unchecked() : Error : " + ex.Message.ToString());
        //        }
        //    });
        //    _thread.Start(sender);
        //}

        private void radioDispButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(_interactionId)) return;
                var radioButton = (RadioButton)sender;
                if (radioButton.Content.ToString() == "None")
                {
                    _dispositionCodesSelected = "None";
                    //return;
                }
                else
                {
                    var temp = _rdDispositionCodes.FirstOrDefault(x => x.Key.Contains(radioButton.Content.ToString()));
                    _dispositionCodesSelected = temp.Key;
                }
                if (_previousDispositionCode == _dispositionCodesSelected) return;
                if (btnDone.Visibility == System.Windows.Visibility.Hidden || btnDone.Visibility == System.Windows.Visibility.Collapsed)
                {
                    NotifyDispositionCodeEvent.Invoke(_selectedInteractionType, new DispositionData()
                    {
                        DispostionCollection = _dispositionTree,
                        DispostionCode = _dispositionCodesSelected,
                        InteractionID = _interactionId
                    });
                    _previousDispositionCode = _dispositionCodesSelected;
                    _dispositionCodesSelected = string.Empty;
                }
            }
            catch (Exception ex)
            {
                _logger.Error("radioDispButton_Checked() : Error : " + ex.Message.ToString());
            }
        }

        private void cmbbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                ComboBox cmbBoxSelect = sender as ComboBox;
                if (cmbBoxSelect.Name == "cmb0")
                {
                    UIElement firstIdex = stkMultiCodePanel.Children[0];
                    stkMultiCodePanel.Children.Clear();
                    stkMultiCodePanel.Children.Add(firstIdex);
                    _dispositionTree.Clear();
                    _combo = 0;
                }
                else
                {
                    UIElement[] element = new UIElement[stkMultiCodePanel.Children.Count];
                    for (int count = 0; count < element.Count(); count++)
                        element[count] = stkMultiCodePanel.Children[count];

                    int name = Convert.ToInt32(cmbBoxSelect.Name.Replace("cmb", string.Empty).ToString());
                    foreach (UIElement child in element)
                    {
                        if (child is StackPanel)
                        {
                            StackPanel stk = (StackPanel)child;
                            try
                            {
                                TextBlock tempText = null;
                                if (stk.Children[0] is TextBlock)
                                    tempText = (TextBlock)stk.Children[0];
                                int deleteName = Convert.ToInt32(stk.Name.Replace("stk", string.Empty).ToString());
                                if (deleteName > name)
                                {
                                    stkMultiCodePanel.Children.Remove(stk);
                                }
                                if (deleteName >= name)
                                {
                                    if (tempText != null && _dispositionTree.Keys.Contains(tempText.Text))
                                        _dispositionTree.Remove(tempText.Text);
                                }
                            }
                            catch { }
                        }
                    }
                }
                if (cmbBoxSelect.SelectedValue.ToString() != string.Empty)
                {

                    _dispositionCodesSelected = ((KeyValuePair<string, string>)cmbBoxSelect.SelectedItem).Key.ToString();
                    if (cmbBoxSelect.SelectedValue.ToString().Contains(","))
                        _dispositionCodesSelected = (cmbBoxSelect.SelectedValue.ToString().Split(','))[1].Trim();

                    if (_selectedSubcodes.ContainsKey(_dispositionCodesSelected))
                    {
                        bool isChildAvailable = false;
                        isChildAvailable = (_selectedSubcodes[_dispositionCodesSelected].Values.ToList()).Any(b => (_selectedSubcodes.Keys.ToList()).Any(a => b.Contains(a)));
                        if (isChildAvailable)
                        {
                            _combo++;
                            ComboBox cmbBox = new ComboBox();
                            cmbBox.ItemsSource = _selectedSubcodes[_dispositionCodesSelected];

                            cmbBox.DisplayMemberPath = "Key";
                            cmbBox.SelectedValuePath = "Value";

                            cmbBox.Name = "cmb" + _combo.ToString();
                            cmbBox.Height = 23;
                            cmbBox.Width = 115;
                            cmbBox.Margin = new Thickness(2);
                            cmbBox.SelectionChanged += new SelectionChangedEventHandler(cmbbox_SelectionChanged);

                            TextBlock subDispositionCode = new TextBlock();
                            subDispositionCode.Width = double.NaN;
                            subDispositionCode.FontSize = 14;
                            subDispositionCode.Name = "txt" + _combo.ToString();
                            subDispositionCode.Margin = new Thickness(2);
                            subDispositionCode.Height = 20;
                            var bc = new BrushConverter();
                            subDispositionCode.Foreground = (Brush)bc.ConvertFrom("#007edf");
                            subDispositionCode.Background = new SolidColorBrush(Colors.White);
                            subDispositionCode.Text = _dispositionCodesSelected;
                            subDispositionCode.FontFamily = new System.Windows.Media.FontFamily("Calibri");
                            StackPanel stPanel = new StackPanel()
                            {
                                Orientation = Orientation.Vertical,
                                Name = "stk" + _combo.ToString(),
                                Margin = new Thickness(5)
                            };

                            stPanel.Children.Add(subDispositionCode);

                            stPanel.Children.Add(cmbBox);
                            stkMultiCodePanel.Children.Add(stPanel);
                        }
                        else
                        {
                            var subCodes = _selectedSubcodes[_dispositionCodesSelected];
                            StackPanel stkLastChild = new StackPanel();
                            _rdDispositionCodes = subCodes;
                            if (_datacontext.IsMultiDispositionEnabled)
                            {
                                foreach (string disposition in subCodes.Keys)
                                {
                                    var chkBox = new CheckBox();
                                    //rdButton.GroupName = "DispositionCode";
                                    chkBox.Checked += chkBox_Checked;
                                    chkBox.Unchecked += chkBox_Checked;
                                    if (disposition == "DefaultDispositionCode")
                                        continue;
                                    chkBox.Content = disposition;

                                    chkBox.Margin = new Thickness(2);
                                    chkBox.Height = 18;
                                    chkBox.Width = Double.NaN;
                                    stkLastChild.Children.Add(chkBox);
                                }
                            }
                            else
                            {
                                foreach (string disposition in subCodes.Keys)
                                {
                                    var rdbutton2 = new RadioButton();
                                    rdbutton2.GroupName = "DispositionCode";
                                    rdbutton2.Checked += radioDispButton_Checked;
                                    rdbutton2.Content = disposition;
                                    rdbutton2.Margin = new Thickness(2);
                                    rdbutton2.Height = 18;
                                    rdbutton2.Width = Double.NaN;
                                    stkLastChild.Children.Add(rdbutton2);
                                }
                                if (!subCodes.ContainsKey("None"))
                                {
                                    var rdbutton2 = new RadioButton();
                                    rdbutton2.GroupName = "DispositionCode";
                                    rdbutton2.Content = "None";
                                    rdbutton2.Margin = new Thickness(2);
                                    rdbutton2.Height = 18;
                                    rdbutton2.Width = Double.NaN;
                                    stkLastChild.Children.Add(rdbutton2);
                                }
                            }

                            TextBlock subDispositionCode = new TextBlock();
                            subDispositionCode.Width = double.NaN;
                            subDispositionCode.Name = "txt" + _combo.ToString();
                            subDispositionCode.Margin = new Thickness(2);
                            subDispositionCode.FontSize = 14;
                            subDispositionCode.Height = 20;
                            var bc = new BrushConverter();
                            subDispositionCode.Foreground = (Brush)bc.ConvertFrom("#007edf");
                            subDispositionCode.Background = new SolidColorBrush(Colors.White);
                            subDispositionCode.Text = _dispositionCodesSelected;
                            subDispositionCode.FontFamily = new System.Windows.Media.FontFamily("Calibri");
                            StackPanel stPanel = new StackPanel()
                            {
                                Orientation = Orientation.Vertical,
                                Name = "stk" + _combo.ToString(),
                                Margin = new Thickness(5)
                            };

                            stPanel.Children.Add(subDispositionCode);

                            stPanel.Children.Add(stkLastChild);
                            stkMultiCodePanel.Children.Add(stPanel);
                        }
                        try
                        {
                            var textblock = LogicalTreeHelper.FindLogicalNode(stkMultiCodePanel, cmbBoxSelect.Name.Replace("cmb", "txt"));
                            if (textblock is TextBlock)
                                _dispositionTree.Add((textblock as TextBlock).Text, (cmbBoxSelect.SelectedValue.ToString().Split(','))[0]);
                        }
                        catch { }
                    }
                    else
                    {
                        if (btnDone.Visibility == System.Windows.Visibility.Hidden || btnDone.Visibility == System.Windows.Visibility.Collapsed)
                        {
                            NotifyDispositionCodeEvent.Invoke(_selectedInteractionType, new DispositionData() { DispostionCollection = _dispositionTree, DispostionCode = _dispositionCodesSelected, InteractionID = _interactionId });
                            _dispositionCodesSelected = string.Empty;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("cmbbox_SelectionChanged() : Error : " + ex.Message.ToString());
            }
        }


        private void btnDone_Click(object sender, RoutedEventArgs e)
        {
            NotifyDispositionCodeEvent.Invoke(_selectedInteractionType, new DispositionData() { DispostionCollection = _dispositionTree, DispostionCode = _dispositionCodesSelected, InteractionID = _interactionId });
            _dispositionCodesSelected = string.Empty;
        }

        //public void DoneButtonVisibilityChange(bool visibility)
        //{
        //    if (visibility)
        //        btnDone.Visibility = System.Windows.Visibility.Visible;
        //    else
        //        btnDone.Visibility = System.Windows.Visibility.Collapsed;
        //}
    }
}
