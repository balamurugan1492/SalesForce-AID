namespace Pointel.Windows.Views.Common.Editor.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using System.Windows.Threading;

    using Pointel.HTML.Text.Editor.Settings;
    using Pointel.Windows.Views.Common.Editor.CustomControls;
    using Pointel.Windows.Views.Common.Editor.Helper;
    using Pointel.Windows.Views.Common.Editor.HTMLConverter;
    using Pointel.Configuration.Manager;

    //, IDisposable, IComponentConnector
    /// <summary>
    /// Interaction logic for HTMLEditor.xaml
    /// </summary>
    public partial class HTMLEditor : UserControl
    {
        #region Fields

        public SpellChecker _spellChecker = null;

        internal EditorBox editorBox = new EditorBox();

        private static Dictionary<MenuItem, String> ElementToGroupNames = new Dictionary<MenuItem, String>();
        private static double[] staticFontSizes;
        private static bool _isPasteText = false;

        private bool changeWithCaretInsertion;
        private bool changingFontNameOrSize;
        string[] chars = new string[] { ",", ".", "/", "!", "@", "#", "$", "%", "^", "&", "*", "\"", ";", "_", "(", ")", ":", "|", "[", "]", "-", "~", "<", ">", "?", "`", "+", "=", "{", "}", ";", "''" };
        private string CustomDictionary = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString() + @"\Pointel\dic\dictionary.lex";
        EditorDataContext editorDataContext;
        private IList<FontFamily> fontNames = null;
        private IList<double> fontSizes = null;
        private HyperLinkPicker hyperLinkPicker;
        private List<string> ignoreWords = new List<string>();
        private ImagePicker imagePicker;
        private bool isControlEditable = false;
        private bool isDesignMode;
        bool isTextChanged = false;
        private bool isTextInitialized;
        private bool isUpdateProperty = false;
        private string metric = string.Empty;
        List<TextRange> rangeList = new List<TextRange>();
        private TablePicker tablePicker;
        Thread updateRTB;
        private DependencyProperty WrapLengthProperty;
        private bool _isEnableSpellCheck = false;
        private Pointel.Logger.Core.ILog _logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private string _rtbContent = string.Empty;
        private ContextMenu _rtbContextMenu = new ContextMenu();
        private string _rtbTextContent = string.Empty;

        //private void UpdateRTB(RichTextBox rtBox)
        //{
        //    var textRange = new TextRange(rtBox.Document.ContentStart, rtBox.Document.ContentEnd);
        //    var txt = textRange.Text;
        //    txt = txt.Replace("\r\n", " ");
        //    txt = removeSpecialChars(txt);
        //    _spellChecker.SpellCheck(txt);
        //    if (!string.IsNullOrEmpty(textRange.Text))
        //        updateRTBText(_isEnableSpellCheck, _spellChecker.MisspelledWords);
        //}
        //private void updateRTBText(bool isHighlight, ObservableCollection<string> words)
        //{
        //    Dispatcher.CurrentDispatcher.BeginInvoke((Action)(delegate
        //    {
        //        // Frame mispellwords
        //        string mispelledWords = string.Empty;
        //        for (int i = 0; i < words.Count; i++)
        //        {
        //            if (!string.IsNullOrEmpty(mispelledWords))
        //                mispelledWords = mispelledWords + "|" + words[i].ToString();
        //            else
        //                mispelledWords = words[i].ToString();
        //        }
        //        Regex reg = new Regex(mispelledWords, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //        var start = mainRTB.Document.ContentStart;
        //        while (start != null && start.CompareTo(mainRTB.Document.ContentEnd) < 0)
        //        {
        //            if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
        //            {
        //                string text = start.GetTextInRun(LogicalDirection.Forward);
        //                var match = reg.Match(start.GetTextInRun(LogicalDirection.Forward));
        //                if (match.Length > 0)
        //                {
        //                    var textrange = new TextRange(start.GetPositionAtOffset(match.Index, LogicalDirection.Forward), start.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Backward));
        //                    textrange.ApplyPropertyValue(Inline.TextDecorationsProperty, _txtDecorationCollection);
        //                    start = textrange.End;
        //                }
        //                if (match.Success)
        //                    text = text.Replace(match.ToString(), string.Empty);
        //                if (text.Length > 0)
        //                {
        //                    string correctWords = string.Empty;
        //                    text = text.Replace("\r\n", " ");
        //                    text = removeSpecialChars(text);
        //                    var tempWords = text.Split(' ');
        //                    foreach (var tempWord in tempWords)
        //                    {
        //                        if (!string.IsNullOrEmpty(correctWords))
        //                            correctWords = correctWords + "|" + tempWord;
        //                        else
        //                            correctWords = tempWord;
        //                    }
        //                    Regex regCorrectWord = new Regex(correctWords, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        //                    match = regCorrectWord.Match(start.GetTextInRun(LogicalDirection.Forward));
        //                    var textrange = new TextRange(start.GetPositionAtOffset(match.Index, LogicalDirection.Forward), start.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Backward));
        //                    textrange.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
        //                    start = textrange.End;
        //                }
        //            }
        //            start = start.GetNextContextPosition(LogicalDirection.Forward);
        //        }
        //    }), DispatcherPriority.DataBind);
        //    #region Old Code
        //    //string pattern = @"[^\W\d](\w|[-']{1,2}(?=\w))*";
        //    //TextPointer pointer = document.ContentStart;
        //    //var textRun = new TextRange(document.ContentStart, document.ContentEnd).Text;
        //    //MatchCollection matches = Regex.Matches(textRun, pattern);
        //    //foreach (Match match in matches)
        //    //{
        //    //    while (pointer != null)
        //    //    {
        //    //        if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
        //    //        {
        //    //            string textRun1 = pointer.GetTextInRun(LogicalDirection.Forward);
        //    //            // Find the starting index of any substring that matches "word".
        //    //            int indexInRun = textRun1.IndexOf(match.Value);
        //    //            if (indexInRun >= 0)
        //    //            {
        //    //                TextPointer start = pointer.GetPositionAtOffset(indexInRun);
        //    //                TextPointer end = start.GetPositionAtOffset(match.Value.Length);
        //    //                var wordrange = new TextRange(start.GetPositionAtOffset(match.Index, LogicalDirection.Forward), start.GetPositionAtOffset(match.Index + match.Length, LogicalDirection.Backward));
        //    //              //  var wordrange = (new TextRange(start, end));
        //    //                //var fontfamily = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.FontFamilyProperty);
        //    //                //var fontstyle = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.FontStyleProperty);
        //    //                //var fontweight = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.FontWeightProperty);
        //    //                //var fontstr = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.FontStretchProperty);
        //    //                //var fontsize = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.FontSizeProperty);
        //    //                //var fontfore = wordrange.GetPropertyValue(System.Windows.Documents.TextElement.ForegroundProperty);
        //    //                //var dfrtg = new FormattedText(wordrange.Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, new Typeface((FontFamily)fontfamily, (FontStyle)fontstyle, (FontWeight)fontweight, (FontStretch)fontstr), (double)fontsize, (Brush)fontfore);
        //    //                if (_spellChecker.MisspelledWords.Contains(match.Value) && _isEnableSpellCheck)
        //    //                {
        //    //                    wordrange.ApplyPropertyValue(Inline.TextDecorationsProperty, CustomTextDecorationCollection());
        //    //                    //Canvas canvas = new Canvas();
        //    //                    //Rectangle dfr = new Rectangle() { Fill = wavyBrush, Height = 5, Width = dfrtg.Width };
        //    //                    //canvas.Children.Add(dfr);
        //    //                    //new InlineUIContainer(canvas, start);
        //    //                    //while the text wrapping ui will collapse
        //    //                }
        //    //                else
        //    //                {
        //    //                    wordrange.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
        //    //                    //var sder = pointer.GetAdjacentElement(pointer.LogicalDirection);
        //    //                }
        //    //                break;
        //    //            }
        //    //        }
        //    //        pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
        //    //    }
        //    //}
        //    #endregion
        //}
        TextDecorationCollection _txtDecorationCollection = null;
        private ContextMenu _txtMessageContextMenu = new ContextMenu();
        private ContextMenu _txtSuggestionContextMenu = new ContextMenu();

        #endregion Fields

        [Bindable(true)]
        public string RTBTextContent
        {
            get
            {
                return _rtbTextContent;
            }
            set
            {
                if (_rtbTextContent != value)
                {
                    _rtbTextContent = value;
                }
            }
        }
        void hyperLinkPicker_LinkSelected(string DispalyName, Uri uri)
        {
            try
            {
                TextRange tr = new TextRange(mainRTB.Selection.Start, mainRTB.Selection.End);
                tr.Text = DispalyName;
                Hyperlink hlink = new Hyperlink(tr.Start, tr.End);
                hlink.NavigateUri = uri;
            }
            catch (Exception ex)
            {
                _logger.Error("Error while adding hyperlink " + ex.Message);
            }

        }

        #region Constructors
        public HTMLEditor(bool isEditable, bool isHtml, string content)
        {
            try
            {

                InitializeComponent();
                isControlEditable = isEditable;
                editorDataContext = new EditorDataContext();
                this.DataContext = editorDataContext;

                staticFontSizes = new double[] { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
                fontNames = new ObservableCollection<FontFamily>(Fonts.SystemFontFamilies);
                fontSizes = new ObservableCollection<double>(staticFontSizes);

                cmbFontFamily.ItemsSource = fontNames;
                cmbFontSize.ItemsSource = staticFontSizes;
                ToggleFontColor.Click += new RoutedEventHandler(OnFontColorClick);
                ToggleLineColor.Click += new RoutedEventHandler(OnLineColorClick);
                FontColorContextMenu.Opened += new RoutedEventHandler(OnFontColorContextMenuOpened);
                FontColorContextMenu.Closed += new RoutedEventHandler(OnFontColorContextMenuClosed);
                LineColorContextMenu.Opened += new RoutedEventHandler(OnLineColorContextMenuOpened);
                LineColorContextMenu.Closed += new RoutedEventHandler(OnLineColorContextMenuClosed);
                FontColorPicker.SelectedColorChanged += new EventHandler<PropertyChangedEventArgs<Color>>(OnFontColorPickerSelectedColorChanged);
                LineColorPicker.SelectedColorChanged += new EventHandler<PropertyChangedEventArgs<Color>>(OnLineColorPickerSelectedColorChanged);

                DataObject.AddPastingHandler(mainRTB, new DataObjectPastingEventHandler(OnPaste));
                cmbFontFamily.SelectionChanged += (s, e) => mainRTB.Selection.ApplyPropertyValue(TextElement.FontFamilyProperty, e.AddedItems[0]);
                cmbFontSize.SelectionChanged += (s, e) => mainRTB.Selection.ApplyPropertyValue(TextElement.FontSizeProperty, e.AddedItems[0]);

                FlowDocument doc = new FlowDocument();
                if (isHtml)
                {
                    mainRTB.SetHTML(content);
                    editorDataContext.ShoworHideHTMLControls = Visibility.Visible;
                    cmbHTMLFormat.SelectedIndex = 1;
                    mainRTB.Format = Common.EditorType.HTML;
                }
                else
                {
                    content = mainRTB.GetPlaneText(content);
                    Paragraph paraContent = new Paragraph();
                    paraContent.Inlines.Add(new Run(content));
                    doc.Blocks.Add(paraContent);
                    if (doc != null)
                        mainRTB.Document = doc;
                    cmbHTMLFormat.SelectedIndex = 0;
                    editorDataContext.ShoworHideHTMLControls = Visibility.Collapsed;
                    mainRTB.Format = Common.EditorType.Text;
                }
                if (isEditable)
                {
                    rowToolbar.Height = GridLength.Auto;
                    brdMain.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#D6D7D6")));
                    brdMain.BorderThickness = new Thickness(1);

                    // Spell Checker
                    // Start
                    mainRTB.ContextMenu = null;
                    _spellChecker = null;
                    CustomTextDecorationCollection();
                    _spellChecker = new SpellChecker();

                    MenuItem _mnuCopy = new MenuItem();
                    _mnuCopy.Header = "Copy";
                    //_mnuCopy.InputGestureText = "Ctrl+C";
                    _mnuCopy.Click += new RoutedEventHandler(rtbContextMenu_Click);
                    _mnuCopy.Margin = new System.Windows.Thickness(2);
                    _rtbContextMenu.Items.Add(_mnuCopy);

                    MenuItem _mnuSelectAll = new MenuItem();
                    _mnuSelectAll.Header = "Select All";
                    _mnuSelectAll.Margin = new System.Windows.Thickness(2);
                    // _mnuSelectAll.InputGestureText = "Ctrl+A";
                    _mnuSelectAll.Click += new RoutedEventHandler(rtbContextMenu_Click);
                    //_rtbContextMenu.Style = (Style)FindResource("Contextmenu");
                    _rtbContextMenu.Items.Add(_mnuSelectAll);
                    _rtbContextMenu.Opened += new RoutedEventHandler(_rtbContextMenu_Opened);

                    MenuItem _mnuItemCut = new MenuItem();
                    _mnuItemCut.Header = "Cut";
                    _mnuItemCut.Margin = new System.Windows.Thickness(2);
                    _mnuItemCut.Click += new RoutedEventHandler(txtMessageContextMenu_Click);
                    _txtMessageContextMenu.Items.Add(_mnuItemCut);

                    MenuItem _mnuItemCopy = new MenuItem();
                    _mnuItemCopy.Header = "Copy";
                    _mnuItemCopy.Margin = new System.Windows.Thickness(2);
                    _mnuItemCopy.Click += new RoutedEventHandler(txtMessageContextMenu_Click);
                    _txtMessageContextMenu.Items.Add(_mnuItemCopy);

                    MenuItem _mnuItemPaste = new MenuItem();
                    _mnuItemPaste.Header = "Paste";
                    _mnuItemPaste.Margin = new System.Windows.Thickness(2);
                    _mnuItemPaste.Click += new RoutedEventHandler(txtMessageContextMenu_Click);
                    _txtMessageContextMenu.Items.Add(_mnuItemPaste);

                    MenuItem _mnuItemSelectAll = new MenuItem();
                    _mnuItemSelectAll.Header = "Select All";
                    _mnuItemSelectAll.Margin = new System.Windows.Thickness(2);
                    _mnuItemSelectAll.Click += new RoutedEventHandler(txtMessageContextMenu_Click);
                    _txtMessageContextMenu.Items.Add(_mnuItemSelectAll);
                    if (!((ConfigContainer.Instance().AllKeys.Contains("interaction.spellcheck.is-mandatory") &&
                                      ((string)ConfigContainer.Instance().GetValue("interaction.spellcheck.is-mandatory")).ToLower().Equals("true"))))
                    {
                        MenuItem _mnuItemSpellCheck = new MenuItem();
                        _mnuItemSpellCheck.Header = "Enable Spell Checking";
                        _isEnableSpellCheck = false;
                        _mnuItemSpellCheck.Margin = new System.Windows.Thickness(2);
                        _mnuItemSpellCheck.Click += new RoutedEventHandler(txtMessageContextMenu_Click);
                        _txtMessageContextMenu.Items.Add(_mnuItemSpellCheck);
                    }
                    // _txtMessageContextMenu.Style = (Style)FindResource("Contextmenu");

                    MenuItem _mnuItemLanguages = new MenuItem();
                    _mnuItemLanguages.Header = "Languages";
                    _mnuItemLanguages.Margin = new System.Windows.Thickness(2);
                    _mnuItemLanguages.Click += new RoutedEventHandler(txtMessageContextMenu_Click);

                    AbstractDictionary[] abstractDictionary = _spellChecker.FindDictionaries("Dictionaries");
                    foreach (AbstractDictionary current in abstractDictionary)
                    {
                        MenuItem menuItem = new MenuItem();
                        menuItem.Click += new RoutedEventHandler(this._mnuLang_Click);
                        menuItem.IsCheckable = true;
                        menuItem.Tag = current;
                        ElementToGroupNames.Add(menuItem, "Language");
                        menuItem.Checked += MenuItemChecked;
                        menuItem.Margin = new System.Windows.Thickness(2);
                        menuItem.Header = current.ToString();
                        if (current.Name == "HunSpellDic_en-US")
                        {
                            menuItem.IsChecked = true;
                            _mnuLang_Click(menuItem, null);
                        }
                        _mnuItemLanguages.Items.Add(menuItem);
                    }

                    _txtMessageContextMenu.Items.Add(_mnuItemLanguages);
                    //_txtMessageContextMenu.Style = (Style)FindResource("Contextmenu");

                    _txtMessageContextMenu.Opened += new RoutedEventHandler(txtMessageContextMenu_Opened);
                    //end

                }
                else
                {
                    rowToolbar.Height = new GridLength(0);
                    mainRTB.IsReadOnly = true;
                    brdMain.BorderBrush = Brushes.Transparent;
                    brdMain.BorderThickness = new Thickness(0);
                }

                RTBContent = content;
                RTBTextContent = content;

                isTextChanged = false;
                if ((ConfigContainer.Instance().AllKeys.Contains("interaction.spellcheck.is-mandatory") &&
                                   ((string)ConfigContainer.Instance().GetValue("interaction.spellcheck.is-mandatory")).ToLower().Equals("true")))
                    btnSpellCheck.Visibility = Visibility.Collapsed;
                else
                    btnSpellCheck.Visibility = Visibility.Visible;

                CreateLEXFile(CustomDictionary);
            }
            catch (Exception ex)
            {
                string exception = ex.ToString();
            }
        }

        #endregion Constructors

        #region Properties

        public EditorBox EditorBox
        {
            get
            {
                return this.editorBox;
            }
        }

        //
        // Summary:
        //     Gets or sets a Editor user control string .
        //
        // Returns:
        //     Editor content in HTML format
        [Bindable(true)]
        public string RTBContent
        {
            get
            {
                return _rtbContent;
            }
            set
            {
                if (_rtbContent != value)
                {
                    _rtbContent = value;
                }
            }
        }


        public double WrapLength
        {
            get
            {
                return (double)base.GetValue(WrapLengthProperty);
            }
            set
            {
                base.SetValue(WrapLengthProperty, value);
            }
        }

        #endregion Properties

        #region Methods

        public void CreateLEXFile(string LEXFile)
        {
            try
            {
                if (!Directory.Exists(System.IO.Path.GetDirectoryName(LEXFile)))
                    Directory.CreateDirectory(System.IO.Path.GetDirectoryName(LEXFile));
                if (!File.Exists(LEXFile))
                {
                    using (StreamWriter streamWriter = new StreamWriter(CustomDictionary, true))
                    {
                        streamWriter.WriteLine("");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error While Creating LEX file ( " + LEXFile + ") -" + ex.Message);

            }
        }

        public void AddResponseContent(string responseContent)
        {
            responseContent = responseContent.Replace("\r\n", "<br />");
            responseContent = responseContent.Replace("\n", "<br />");
            if (cmbHTMLFormat.SelectedIndex == 0)
            {
                responseContent = mainRTB.GetPlaneText(responseContent);
                InsertText(responseContent, true);
            }
            else
            {
                InsertHTML(responseContent, true);
            }
            mainRTB.Focus();
        }

        public void GetContent()
        {
            try
            {
                if (mainRTB != null)
                {
                    if (cmbHTMLFormat.SelectedIndex == 0)
                    {
                        this.RTBContent = string.Empty;
                        this.RTBTextContent = mainRTB.GetText();
                    }
                    else if (cmbHTMLFormat.SelectedIndex == 1)
                    {
                        this.RTBContent = mainRTB.GetHTML();
                        this.RTBTextContent = mainRTB.GetText();
                    }

                }
            }
            catch //(Exception ex)
            {
            }
        }

        public string GetContentinTextFormat()
        {
            if (mainRTB != null)
                return mainRTB.GetText();
            else
                return string.Empty;
        }

        public string GetFontSize()
        {
            if (this.cmbFontSize.SelectedItem != null)
            {
                return this.ConvertFontSizeToPxMetric((double)this.cmbFontSize.SelectedItem).ToString();
            }
            return "";
        }

        public string GetResponseContent()
        {
            if (cmbHTMLFormat.SelectedIndex == 0)
                return mainRTB.GetText();
            else
                return mainRTB.GetHTML();
        }

        public string GetXAML()
        {
            return mainRTB.GetXAML();
        }

        public void InsertHTML(string html)
        {
            try
            {
                mainRTB.InsertHTML(html);
            }
            catch //(Exception ex)
            {
            }
        }

        public void InsertHTML(string html, bool insertBeforeCursor)
        {
            try
            {
                mainRTB.InsertHTML(html, insertBeforeCursor);
            }
            catch //(Exception ex)
            {
            }
        }

        public void InsertText(string text)
        {
            try
            {
                mainRTB.InsertText(text);
            }
            catch //(Exception ex)
            {
            }
        }

        public void InsertText(string text, bool insertBeforeCursor)
        {
            try
            {
                mainRTB.InsertText(text, insertBeforeCursor);
            }
            catch //(Exception ex)
            {
            }
        }

        public void LoadImages()
        {
            this.mainRTB.LoadImages();
        }

        public void Print(Paragraph generalContent)
        {
            try
            {
                PrintPreviewWindow print = new PrintPreviewWindow(mainRTB.Document, generalContent);
                print.ShowDialog();
            }
            catch //(Exception ex)
            {
            }
        }

        public void SetFontName(string family)
        {
            try
            {
                this.changingFontNameOrSize = true;
                try
                {
                    FontFamily item = new FontFamily(family);
                    int num = -1;
                    for (int i = 0; i < this.fontNames.Count; i++)
                    {
                        if (this.fontNames[i] != null)
                        {
                            if (string.Compare(family, this.fontNames[i].ToString(), true) == 0)
                            {
                                num = i;
                                break;
                            }
                            if (string.Compare(family, this.fontNames[i].ToString(), true) < 0)
                            {
                                this.fontNames.Insert(i, item);
                                num = i;
                                break;
                            }
                        }
                    }
                    if (num == -1)
                    {
                        this.fontNames.Add(item);
                        num = this.fontNames.Count - 1;
                    }
                    this.cmbFontFamily.SelectedIndex = num;
                }
                catch
                {
                }
                this.changingFontNameOrSize = false;
            }
            catch (Exception ex)
            {
            }
        }

        public void SetFontSize(string size)
        {
            double fontSize = 0.0;
            if (double.TryParse(size, NumberStyles.Float, CultureInfo.InvariantCulture, out fontSize))
            {
                this.SetFontSize(fontSize);
            }
        }

        public void SetFontSize(double size)
        {
            this.changingFontNameOrSize = true;
            try
            {
                double num = this.ConvertFontSizeFromPxMetric(size);
                if (!this.fontSizes.Contains(num))
                {
                    bool flag = false;
                    for (int i = 0; i < this.fontSizes.Count; i++)
                    {
                        if (num < this.fontSizes[i])
                        {
                            this.fontSizes.Insert(i, num);
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        this.fontSizes.Add(num);
                    }
                }
                size = num;
                this.cmbFontSize.SelectedItem = size;
            }
            catch
            {
            }
            try
            {
                this.cmbFontSize.Text = size.ToString();
            }
            catch
            {
            }
            this.changingFontNameOrSize = false;
        }

        public void SetHTML(string html)
        {
            try
            {
                this.isTextInitialized = true;
                mainRTB.SetHTML(html);
                this.InitializeDocument();
            }
            catch //(Exception ex)
            {
            }
        }

        public void SpellcheckMandatory()
        {
            //if (!_isEnableSpellCheck)
            // {
            _isEnableSpellCheck = true;
            StartSpellCheck();
            foreach (MenuItem mi in _txtMessageContextMenu.Items)
            {
                switch ((string)mi.Header)
                {
                    case "Enable Spell Checking":
                        mi.Header = "Disable Spell Checking";
                        break;
                    case "Disable Spell Checking":
                        mi.Header = "Enable Spell Checking";
                        break;
                }
            }
            // }
        }

        internal void SetXAML(string xaml)
        {
            try
            {
                this.isTextInitialized = true;
                mainRTB.SetXAML(xaml);
            }
            catch //(Exception ex)
            {
            }
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            try
            {
                var isText = e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.Text, true);
                if (!isText) return;
                var text = e.SourceDataObject.GetData(DataFormats.Text) as string;
                string data = HtmlToXamlConverter.ConvertHtmlToXaml(text, true);
                FlowDocument doc = new FlowDocument();
                doc = SetRTF(data);
                if (!string.IsNullOrEmpty(data))
                    _isPasteText = true;
            }
            catch //(Exception ex)
            {
            }
        }

        private static FlowDocument SetRTF(string xamlString)
        {
            if (!string.IsNullOrEmpty(xamlString))
            {
                var html = HtmlFromXamlConverter.ConvertXamlToHtml(xamlString);
                var flowDocument = new System.Windows.Documents.FlowDocument();
                var stringReader = new System.IO.StringReader(xamlString);
                var xmlTextReader = new System.Xml.XmlTextReader(stringReader);
                flowDocument = (System.Windows.Documents.FlowDocument)System.Windows.Markup.XamlReader.Load(xmlTextReader);
                return flowDocument;
            }
            else
            {
                return null;
            }
        }

        private static void WrapLengthPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            try
            {
                HTMLEditor editor = d as HTMLEditor;
                if (editor != null)
                {
                    if (editor.isDesignMode)
                    {
                        return;
                    }
                    editor.EditorBox.Document.PageWidth = (double)e.NewValue;
                }
            }
            catch //(Exception ex)
            {
            }
        }

        private void AddToDictionary(string entry)
        {
            using (StreamWriter streamWriter = new StreamWriter(CustomDictionary, true))
            {
                streamWriter.WriteLine(entry);
            }
            //IgnoreAll(entry);
        }

        private void brdMainRTB_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            try
            {
                brdMainRTB.SizeChanged -= brdMainRTB_SizeChanged;
                mainRTB.Height = brdMainRTB.ActualHeight - 5;
                mainRTB.Width = brdMainRTB.ActualWidth - 5;
                brdMainRTB.SizeChanged += brdMainRTB_SizeChanged;
            }
            catch //(Exception ex)
            {
            }
        }

        private void btnInsertHLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (hyperLinkPicker != null)
                {
                    hyperLinkPicker.LinkSelected -= hyperLinkPicker_LinkSelected;
                    hyperLinkPicker = null;
                }
                hyperLinkPicker = new HyperLinkPicker(editorDataContext);
                hyperLinkPicker.LinkSelected += new HyperLinkPicker.PassLinkToRTB(hyperLinkPicker_LinkSelected);
                var menuConsultItem = new MenuItem();
                menuConsultItem.StaysOpenOnClick = true;
                menuConsultItem.Background = Brushes.Transparent;
                menuConsultItem.Header = hyperLinkPicker;
                menuConsultItem.Margin = new Thickness(-10, -5, -21, -5);
                menuConsultItem.Width = Double.NaN;
                editorDataContext.contextMenuUC.Items.Clear();
                editorDataContext.contextMenuUC.Items.Add(menuConsultItem);
                editorDataContext.contextMenuUC.PlacementTarget = btnInsertHLink;
                editorDataContext.contextMenuUC.Placement = PlacementMode.Bottom;
                editorDataContext.contextMenuUC.IsOpen = true;
                editorDataContext.contextMenuUC.StaysOpen = true;
                editorDataContext.contextMenuUC.Focus();
            }
            catch //(Exception ex)
            {
            }
        }

        private void btnInsertImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (imagePicker != null)
                {
                    imagePicker.ImageSelected -= imagePicker_ImageSelected;
                    imagePicker = null;
                }
                imagePicker = new ImagePicker(editorDataContext);
                imagePicker.ImageSelected += new ImagePicker.PassImageToRTB(imagePicker_ImageSelected);
                var menuConsultItem = new MenuItem();
                menuConsultItem.StaysOpenOnClick = true;
                menuConsultItem.Background = Brushes.Transparent;
                menuConsultItem.Header = imagePicker;
                menuConsultItem.Margin = new Thickness(-20, -5, -21, -5);
                menuConsultItem.Width = Double.NaN;
                editorDataContext.contextMenuUC.Items.Clear();
                editorDataContext.contextMenuUC.Items.Add(menuConsultItem);
                //editorDataContext.contextMenuUC.PlacementTarget = btnInsertImage;
                editorDataContext.contextMenuUC.Placement = PlacementMode.Bottom;
                editorDataContext.contextMenuUC.IsOpen = true;
                editorDataContext.contextMenuUC.StaysOpen = true;
                editorDataContext.contextMenuUC.Focus();
            }
            catch// (Exception ex)
            {
            }
        }

        private void btnInsertTable_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (tablePicker != null)
                {
                    tablePicker.TableSelected -= tablePicker_TableSelected;
                    tablePicker = null;
                }
                tablePicker = new TablePicker(editorDataContext);
                tablePicker.TableSelected += new TablePicker.PassTableToRTB(tablePicker_TableSelected);
                var menuConsultItem = new MenuItem();
                menuConsultItem.StaysOpenOnClick = true;
                menuConsultItem.Background = Brushes.Transparent;
                menuConsultItem.Header = tablePicker;
                menuConsultItem.Margin = new Thickness(-10, -5, -21, -5);
                menuConsultItem.Width = Double.NaN;
                editorDataContext.contextMenuUC.Items.Clear();
                editorDataContext.contextMenuUC.Items.Add(menuConsultItem);
                editorDataContext.contextMenuUC.PlacementTarget = btnInsertTable;
                editorDataContext.contextMenuUC.Placement = PlacementMode.Bottom;
                editorDataContext.contextMenuUC.IsOpen = true;
                editorDataContext.contextMenuUC.StaysOpen = true;
                editorDataContext.contextMenuUC.Focus();
            }
            catch //(Exception ex)
            {
            }
        }

        private void btnSpellCheck_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_isEnableSpellCheck)
                {
                    _isEnableSpellCheck = false;
                    _spellChecker.IgnoredWords.Clear();
                    StopSpellCheck();
                }
                else
                {
                    _isEnableSpellCheck = true;
                    StartSpellCheck();
                }
                foreach (MenuItem mi in _txtMessageContextMenu.Items)
                {
                    switch ((string)mi.Header)
                    {
                        case "Enable Spell Checking":
                            mi.Header = "Disable Spell Checking";
                            break;
                        case "Disable Spell Checking":
                            mi.Header = "Enable Spell Checking";
                            break;
                    }
                }
            }
            catch //(Exception ex)
            {

            }
        }

        private bool checkfullword(string mispellWord, string text)
        {
            text = text.Replace("\r\n", " ");
            text = text.Replace(@"\", "");
            text = removeSpecialChars(text);
            var tempWords = text.Split(' ');
            foreach (var tempWord in tempWords)
            {
                if (mispellWord.Trim().ToLower() == tempWord.Trim().ToLower())
                    return true;
            }
            return false;
        }

        private void cmbFontFamily_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                FontFamily fontFamily = this.cmbFontFamily.SelectedItem as FontFamily;
                if (fontFamily != null && !this.changeWithCaretInsertion)
                {
                    this.mainRTB.Selection.ApplyPropertyValue(Control.FontFamilyProperty, fontFamily);
                }
                if (this.changingFontNameOrSize)
                {
                    return;
                }
                this.FocusOnEditor();
            }
            catch //(Exception ex)
            {
            }
        }

        private void cmbFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (this.cmbFontSize.SelectedItem != null && !this.changeWithCaretInsertion)
                {
                    double size = (double)this.cmbFontSize.SelectedItem;
                    this.mainRTB.Selection.ApplyPropertyValue(Control.FontSizeProperty, this.ConvertFontSizeToPxMetric(size));
                    if (this.changingFontNameOrSize)
                    {
                        return;
                    }
                    this.FocusOnEditor();
                }
            }
            catch //(Exception ex)
            {
            }
        }

        private void cmbHTMLFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //isFormattochange = true;
            var comboBox = sender as ComboBox;
            ComboBoxItem cbi = (ComboBoxItem)comboBox.SelectedItem;

            if (cbi.Content.Equals("Text"))
            {
                mainRTB.Format = Common.EditorType.Text;
                string text = mainRTB.GetText();
                mainRTB.SetText(text);
                editorDataContext.ShoworHideHTMLControls = Visibility.Collapsed;
            }
            else
            {
                mainRTB.Format = Common.EditorType.HTML;
                editorDataContext.ShoworHideHTMLControls = Visibility.Visible;
            }
        }

        private double ConvertFontSizeFromPxMetric(double size)
        {
            double num = size;
            if (this.metric == "point")
            {
                num *= 0.75;
                int num2 = (int)num;
                if (num - (double)num2 != 0.5)
                {
                    num = Math.Round(num);
                }
            }
            return num;
        }

        private double ConvertFontSizeToPxMetric(double size)
        {
            double num = size;
            if (this.metric == "point")
            {
                num /= 0.75;
                num = Math.Round(num);
            }
            return num;
        }

        private void CustomTextDecorationCollection()
        {
            _txtDecorationCollection = new TextDecorationCollection();
            TextDecoration myUnderline = new TextDecoration();
            myUnderline.Location = TextDecorationLocation.Baseline;
            // Set the solid color brush.Brushes.Red
            VisualBrush _wavyBrush = (VisualBrush)mainRTB.FindResource("WavyBrush");
            myUnderline.Pen = new Pen(_wavyBrush, 10);
            myUnderline.PenThicknessUnit = TextDecorationUnit.FontRecommended;

            // Set the underline decoration to the text block.
            _txtDecorationCollection.Add(myUnderline);
        }

        private void FocusOnEditor()
        {
            if (this.changeWithCaretInsertion)
            {
                return;
            }
            Keyboard.Focus(this.EditorBox);
        }

        private Block GetCurrentBlock(LogicalDirection direction)
        {
            try
            {
                FrameworkContentElement obj = mainRTB.CaretPosition.GetAdjacentElement(direction) as FrameworkContentElement;

                while (obj != null)
                {
                    if (obj as Block != null) { return obj as Block; }
                    obj = obj.Parent as FrameworkContentElement;
                }
            }
            catch //(Exception ex)
            {

            }
            return null;
        }

        private string GetWordAtPointer(TextPointer textPointer)
        {
            return string.Join(string.Empty, GetWordCharactersBefore(textPointer), GetWordCharactersAfter(textPointer));
        }

        private string GetWordCharactersAfter(TextPointer textPointer)
        {
            var fowards = textPointer.GetTextInRun(LogicalDirection.Forward);
            var wordCharactersAfterPointer = new string(fowards.TakeWhile(c => !char.IsSeparator(c) && !char.IsPunctuation(c)).ToArray());
            return wordCharactersAfterPointer;
        }

        private string GetWordCharactersBefore(TextPointer textPointer)
        {
            var backwards = textPointer.GetTextInRun(LogicalDirection.Backward);
            var wordCharactersBeforePointer = new string(backwards.Reverse().TakeWhile(c => !char.IsSeparator(c) && !char.IsPunctuation(c)).Reverse().ToArray());
            return wordCharactersBeforePointer;
        }

        private void HandleSelectionChange()
        {
            try
            {
                this.changeWithCaretInsertion = true;
                TextSelection selection = mainRTB.Selection;
                object unsetValue = DependencyProperty.UnsetValue;
                object propertyValue = selection.GetPropertyValue(Control.FontFamilyProperty);
                if (propertyValue != unsetValue && propertyValue != null)
                {
                    try
                    {
                        FontFamily item = (FontFamily)propertyValue;
                        int num = -1;
                        for (int i = 0; i < this.fontNames.Count; i++)
                        {
                            if (this.fontNames[i] != null)
                            {
                                if (string.Compare(propertyValue.ToString(), this.fontNames[i].ToString(), true) == 0)
                                {
                                    num = i;
                                    break;
                                }
                                if (string.Compare(propertyValue.ToString(), this.fontNames[i].ToString(), true) < 0)
                                {
                                    this.fontNames.Insert(i, item);
                                    num = i;
                                    break;
                                }
                            }
                        }
                        if (num == -1)
                        {
                            this.fontNames.Add(item);
                            num = this.fontNames.Count - 1;
                        }
                        this.cmbFontFamily.SelectedIndex = num;
                        goto IL_FE;
                    }
                    catch
                    {
                        goto IL_FE;
                    }
                }
                this.cmbFontFamily.SelectedIndex = -1;
            IL_FE:
                object obj = selection.GetPropertyValue(Control.FontSizeProperty);
                if (obj != unsetValue)
                {
                    try
                    {
                        double num2 = this.ConvertFontSizeFromPxMetric((double)obj);
                        if (!this.fontSizes.Contains(num2))
                        {
                            bool flag = false;
                            for (int j = 0; j < this.fontSizes.Count; j++)
                            {
                                if (num2 < this.fontSizes[j])
                                {
                                    this.fontSizes.Insert(j, num2);
                                    flag = true;
                                    break;
                                }
                            }
                            if (!flag)
                            {
                                this.fontSizes.Add(num2);
                            }
                        }
                        obj = num2;
                        this.cmbFontSize.SelectedItem = obj;
                    }
                    catch
                    {
                    }
                    try
                    {
                        this.cmbFontSize.Text = obj.ToString();
                        goto IL_1C2;
                    }
                    catch
                    {
                        goto IL_1C2;
                    }
                }
                this.cmbFontSize.SelectedIndex = -1;
            IL_1C2:
                object propertyValue2 = selection.GetPropertyValue(Control.ForegroundProperty);
                if (propertyValue2 != unsetValue)
                {
                    this.FontColorPicker.SelectedColor = ((SolidColorBrush)propertyValue2).Color;
                }
                object propertyValue3 = selection.GetPropertyValue(Control.FontWeightProperty);
                if (propertyValue3 != unsetValue)
                {
                    this.buttonBold.IsChecked = new bool?((FontWeight)propertyValue3 == FontWeights.Bold);
                }
                else
                {
                    this.buttonBold.IsChecked = new bool?(false);
                }
                object propertyValue4 = selection.GetPropertyValue(Control.FontStyleProperty);
                if (propertyValue4 != unsetValue)
                {
                    this.buttonItalic.IsChecked = new bool?((FontStyle)propertyValue4 == FontStyles.Italic);
                }
                else
                {
                    this.buttonItalic.IsChecked = new bool?(false);
                }
                object propertyValue5 = selection.GetPropertyValue(Inline.TextDecorationsProperty);
                if (propertyValue5 != null && propertyValue5 != unsetValue)
                {
                    TextDecorationCollection textDecorationCollection = propertyValue5 as TextDecorationCollection;
                    bool value = false;
                    foreach (TextDecoration current in textDecorationCollection)
                    {
                        if (current.Location == TextDecorationLocation.Underline)
                        {
                            value = true;
                        }
                    }
                    this.buttonUnderline.IsChecked = new bool?(value);
                }
                else
                {
                    this.buttonUnderline.IsChecked = new bool?(false);
                }
                this.changeWithCaretInsertion = false;
            }
            catch //(Exception ex)
            {
            }
        }

        private void IgnoreAll(string word)
        {
            //_spellChecker.MisspelledWords.Remove(word);
            //ignoreWords.Add(word);
            //var textRange = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
            //var text = textRange.Text;
            //text = text.Replace("\r\n", "");
            //text = removeSpecialChars(text);
            //_spellChecker.SpellCheck(text);
            //if (!string.IsNullOrEmpty(textRange.Text) && _isEnableSpellCheck)
            //    updateRTBText(false, _spellChecker.MisspelledWords);
        }

        void imagePicker_ImageSelected(string filePath)
        {
            try
            {
                Image image = new Image();
                BitmapImage bitmap = new BitmapImage(new Uri(filePath, UriKind.Absolute));
                image.Source = bitmap;
                InlineUIContainer inlineUIContainer = new InlineUIContainer(image);
                mainRTB.CaretPosition.Paragraph.Inlines.Add(inlineUIContainer);
            }
            catch //(Exception ex)
            {
            }
        }

        private void InitializeDocument()
        {
            try
            {
                this.mainRTB.Document.PageWidth = this.WrapLength;
                this.mainRTB.FontSize = base.FontSize;
                if (this.mainRTB.CaretPosition != null)
                {
                    if (this.mainRTB.CaretPosition.Paragraph != null)
                    {
                        this.mainRTB.CaretPosition.Paragraph.Margin = new Thickness(0.0);
                        return;
                    }
                    if (this.mainRTB.CaretPosition.IsAtLineStartPosition)
                    {
                        this.mainRTB.Document.Blocks.Add(new Paragraph
                        {
                            Margin = new Thickness(0.0)
                        });
                    }
                }
            }
            catch //(Exception ex)
            {
            }
        }

        private void mainRTB_KeyUp(object sender, KeyEventArgs e)
        {
            if (isControlEditable)
            {
                var rtxtBox = sender as RichTextBox;
                if (e.Key == Key.Space)
                {
                    var endPointer = rtxtBox.CaretPosition;
                    var endIndex = rtxtBox.Document.ContentStart.GetOffsetToPosition(endPointer);
                    var startPointer = rtxtBox.Document.ContentStart.GetPositionAtOffset(endIndex - 1);
                    var wordrange = (new TextRange(startPointer, endPointer));
                    if (wordrange.Text == " ")
                        wordrange.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                }
                if (e.Key == Key.Space || e.Key == Key.Back || e.Key == Key.Delete)
                {
                    if (_isEnableSpellCheck)
                        StartSpellCheck();
                }
            }
        }

        private void mainRTB_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isUpdateProperty = false;
        }

        private void mainRTB_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            isUpdateProperty = false;
        }

        private void mainRTB_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            isUpdateProperty = false;
        }

        private void mainRTB_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isUpdateProperty = true;
        }

        private void mainRTB_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (isControlEditable)
                {
                    mainRTB.Focus();
                    var textRange = new TextRange(mainRTB.Document.ContentStart, mainRTB.CaretPosition);
                    int catatPos = textRange.Text.Length;
                    if (catatPos >= 0)
                    {
                        if (mainRTB.Selection.Text == string.Empty)
                        {
                            var mousePosition = Mouse.GetPosition(mainRTB);
                            var textPointer = mainRTB.GetPositionFromPoint(mousePosition, false);
                            if (textPointer != null)
                            {
                                string word = GetWordAtPointer(textPointer);
                                if (!_isEnableSpellCheck) return;
                                if (_spellChecker.MisspelledWords.Contains(word))
                                {
                                    _txtSuggestionContextMenu.Items.Clear();
                                    var start = mainRTB.CaretPosition;
                                    var t = start.GetTextInRun(LogicalDirection.Backward);
                                    var end = start.GetNextContextPosition(LogicalDirection.Backward);
                                    var t1 = end.GetTextInRun(LogicalDirection.Backward);
                                    mainRTB.Selection.Select(start, end);
                                    _spellChecker.SelectedMisspelledWord = word;
                                    _spellChecker.LoadSuggestions(_spellChecker.SelectedMisspelledWord);
                                    foreach (var item in _spellChecker.SuggestedWords)
                                    {
                                        var menuSuggestion = new MenuItem();
                                        menuSuggestion.Margin = new System.Windows.Thickness(2);
                                        menuSuggestion.Header = item;
                                        menuSuggestion.Tag = textPointer;
                                        menuSuggestion.Click += new RoutedEventHandler(menuSuggestion_Click);
                                        _txtSuggestionContextMenu.Items.Add(menuSuggestion);
                                    }
                                    _txtSuggestionContextMenu.Items.Add(new Separator());
                                    MenuItem _mnuItemIgnore = new MenuItem();
                                    _mnuItemIgnore.Header = "Ignore All";
                                    _mnuItemIgnore.Tag = textPointer;
                                    _mnuItemIgnore.Click += new RoutedEventHandler(menuSuggestion_Click);
                                    _txtSuggestionContextMenu.Items.Add(_mnuItemIgnore);

                                    _txtSuggestionContextMenu.Items.Add(new Separator());
                                    MenuItem _mnuAddToDic = new MenuItem();
                                    _mnuAddToDic.Header = "Add To Dictionary";
                                    _mnuAddToDic.Tag = textPointer;
                                    _mnuAddToDic.Click += new RoutedEventHandler(menuSuggestion_Click);
                                    _txtSuggestionContextMenu.Items.Add(_mnuAddToDic);

                                    // _txtSuggestionContextMenu.Style = (Style)FindResource("Contextmenu");
                                    _txtSuggestionContextMenu.PlacementTarget = this.mainRTB;
                                    _txtSuggestionContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                                    _txtSuggestionContextMenu.IsOpen = true;
                                    _txtSuggestionContextMenu.StaysOpen = true;
                                    _txtSuggestionContextMenu.Focus();
                                }
                                else
                                {
                                    _txtMessageContextMenu.PlacementTarget = this.mainRTB;
                                    _txtMessageContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                                    _txtMessageContextMenu.IsOpen = true;
                                    _txtMessageContextMenu.StaysOpen = true;
                                    _txtMessageContextMenu.Focus();
                                }
                            }
                            else
                            {
                                _txtMessageContextMenu.PlacementTarget = this.mainRTB;
                                _txtMessageContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                                _txtMessageContextMenu.IsOpen = true;
                                _txtMessageContextMenu.StaysOpen = true;
                                _txtMessageContextMenu.Focus();
                            }
                        }
                        else
                        {
                            _txtMessageContextMenu.PlacementTarget = this.mainRTB;
                            _txtMessageContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.MousePoint;
                            _txtMessageContextMenu.IsOpen = true;
                            _txtMessageContextMenu.StaysOpen = true;
                            _txtMessageContextMenu.Focus();
                        }

                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while mainRTB_PreviewMouseRightButtonDown() :" + generalException.ToString());
            }
        }

        private void mainRTB_SelectionChanged(object sender, RoutedEventArgs e)
        {
            try
            {
                object temp = null;
                if (isUpdateProperty)
                {
                    this.HandleSelectionChange();
                    isUpdateProperty = false;
                }
                temp = mainRTB.Selection.GetPropertyValue(Inline.FontFamilyProperty);
                cmbFontFamily.SelectedItem = temp;
                temp = mainRTB.Selection.GetPropertyValue(Inline.FontSizeProperty);
                cmbFontSize.Text = temp.ToString();
                temp = mainRTB.Selection.GetPropertyValue(Inline.FontWeightProperty);
                buttonBold.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontWeights.Bold));
                temp = mainRTB.Selection.GetPropertyValue(Inline.FontStyleProperty);
                buttonItalic.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(FontStyles.Italic));
                temp = mainRTB.Selection.GetPropertyValue(Inline.TextDecorationsProperty);
                buttonUnderline.IsChecked = (temp != DependencyProperty.UnsetValue) && (temp.Equals(TextDecorations.Underline));

            }
            catch// (Exception ex)
            {
            }
        }

        private void mainRTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var rtb = sender as RichTextBox;
                if (rtb != null)
                {
                    FlowDocument doc1 = rtb.Document;
                    //var isText = e.SourceDataObject.GetDataPresent(System.Windows.DataFormats.Text, true);
                    //if (!isText) return;
                    //var text = e.SourceDataObject.GetData(DataFormats.Text) as string;
                    string data = HtmlToXamlConverter.ConvertHtmlToXaml(doc1.ToString(), true);

                    FlowDocument doc = new FlowDocument();
                    doc = SetRTF(data);
                    if (doc != null)
                        editorDataContext.RTBDocument = doc;
                    //dummyRTB.Document = mainRTB.Document;
                    //this.RTBContent = mainRTB.GetHTML();
                    //this.RTBTextContent = mainRTB.GetText();

                    var textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
                    if (textRange.Text != string.Empty && textRange.Text.Length > 0 && textRange.Text != "\r\n\r\n")
                    {
                        if (_isPasteText)
                        {
                            _isPasteText = false;
                            if (_isEnableSpellCheck)
                                StartSpellCheck();
                        }
                    }
                    else
                    {
                        rtb.Document.Blocks.Clear();
                    }
                }
                //isFormattochange = false;
            }
            catch (Exception ex)
            {
            }
        }

        void MenuItemChecked(object sender, RoutedEventArgs e)
        {
            var menuItem = e.OriginalSource as MenuItem;
            foreach (var item in ElementToGroupNames)
            {
                if (item.Key != menuItem)
                {
                    item.Key.IsChecked = false;
                }
            }
        }

        void menuSuggestion_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                {
                    if (menuitem != null)
                    {
                        if (menuitem.Header.ToString() == "Add To Dictionary")
                        {
                            if (_spellChecker != null)
                            {
                                AddToDictionary(_spellChecker.SelectedMisspelledWord);
                                _spellChecker.AddTodictionary(_spellChecker.SelectedMisspelledWord);
                                StartSpellCheck();
                            }
                        }
                        else if (menuitem.Header.ToString() == "Ignore All")
                        {
                            _spellChecker.IgnoredWords.Add(_spellChecker.SelectedMisspelledWord);
                            for (int i = 0; i < _spellChecker.MisspelledWords.Count; i++)
                            {
                                if (_spellChecker.SelectedMisspelledWord.ToLower() == _spellChecker.MisspelledWords[i].ToLower())
                                {
                                    _spellChecker.MisspelledWords.Remove(_spellChecker.MisspelledWords[i]);
                                    i--;
                                }

                            }
                            UpdateRTB_BackgroundWorker();
                        }
                        else
                        {
                            if (_spellChecker != null)
                            {
                                _spellChecker.SelectedSuggestedWord = menuitem.Header.ToString();
                                if (_spellChecker.SelectedSuggestedWord != "(no suggestions)")
                                {

                                    _spellChecker.MisspelledWords.Remove(_spellChecker.SelectedMisspelledWord);
                                    ReplaceWordAtPointer(menuitem.Tag as TextPointer, _spellChecker.SelectedSuggestedWord);
                                    _spellChecker.SuggestedWords.Clear();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while menuSuggestion_Click() :" + generalException.ToString());
            }
        }

        //private void cmbFontSize_TextChanged(object sender, TextChangedEventArgs e)
        //{
        //    try
        //    {
        //        double size = double.Parse(this.cmbFontSize.Text);
        //        this.mainRTB.Selection.ApplyPropertyValue(Control.FontSizeProperty, this.ConvertFontSizeToPxMetric(size));
        //    }
        //    catch
        //    {
        //    }
        //}
        private void OnFontColorClick(object sender, RoutedEventArgs e)
        {
            try
            {
                FrameworkElement fxElement = sender as FrameworkElement;
                if (fxElement != null && FontColorContextMenu != null)
                {
                    FontColorContextMenu.PlacementTarget = fxElement;
                    FontColorContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    FontColorContextMenu.IsOpen = true;
                }
            }
            catch // (Exception ex)
            {
            }
        }

        private void OnFontColorContextMenuClosed(object sender, RoutedEventArgs e)
        {
            ToggleFontColor.IsChecked = false;
        }

        private void OnFontColorContextMenuOpened(object sender, RoutedEventArgs e)
        {
            FontColorPicker.Reset();
            ToggleFontColor.IsChecked = true;
        }

        private void OnFontColorPickerSelectedColorChanged(object sender, PropertyChangedEventArgs<Color> e)
        {
            TextRange range = new TextRange(mainRTB.Selection.Start, mainRTB.Selection.End);
            range.ApplyPropertyValue(FlowDocument.ForegroundProperty, new SolidColorBrush(e.NewValue));
        }

        private void OnLineColorClick(object sender, RoutedEventArgs e)
        {
            try
            {
                FrameworkElement fxElement = sender as FrameworkElement;
                if (fxElement != null && LineColorContextMenu != null)
                {
                    LineColorContextMenu.PlacementTarget = fxElement;
                    LineColorContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
                    LineColorContextMenu.IsOpen = true;
                }
            }
            catch// (Exception ex)
            {
            }
        }

        private void OnLineColorContextMenuClosed(object sender, RoutedEventArgs e)
        {
            ToggleLineColor.IsChecked = false;
        }

        private void OnLineColorContextMenuOpened(object sender, RoutedEventArgs e)
        {
            LineColorPicker.Reset();
            ToggleLineColor.IsChecked = true;
        }

        private void OnLineColorPickerSelectedColorChanged(object sender, PropertyChangedEventArgs<Color> e)
        {
            TextRange range = new TextRange(mainRTB.Selection.Start, mainRTB.Selection.End);
            range.ApplyPropertyValue(FlowDocument.BackgroundProperty, new SolidColorBrush(e.NewValue));
        }

        private string removeSpecialChars(string word)
        {
            for (int index = 0; index < chars.Length; index++)
            {
                if (word.Contains(chars[index]))
                    word = word.Replace(chars[index], " ");
            }
            word = Regex.Replace(word, @"\s+", " ");
            return word;
        }

        private void ReplaceWordAtPointer(TextPointer textPointer, string replacementWord)
        {
            textPointer.DeleteTextInRun(-GetWordCharactersBefore(textPointer).Count());
            textPointer.DeleteTextInRun(GetWordCharactersAfter(textPointer).Count());
            textPointer.InsertTextInRun(replacementWord);
            mainRTB.CaretPosition = textPointer.GetNextContextPosition(LogicalDirection.Forward);
            UpdateRTB_BackgroundWorker();
        }

        void rtbContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                {
                    if (menuitem != null)
                    {
                        if (menuitem.Header.ToString() == "Copy")
                        {
                            mainRTB.Copy();
                        }
                        else if (menuitem.Header.ToString() == "Select All")
                        {
                            mainRTB.SelectAll();
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while rtbContextMenu_Click() :" + exception.ToString());
            }
        }

        private void StartSpellCheck()
        {
            // Find mispellwords from content and add in collection
            // Start
            var textRange = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
            var txt = textRange.Text;
            txt = txt.Replace("\r\n", " ");
            txt = txt.Replace(@"\", "");
            txt = removeSpecialChars(txt);
            _spellChecker.SpellCheck(txt);
            // End
            UpdateRTB_BackgroundWorker();
        }

        private void StopSpellCheck()
        {
            _spellChecker.MisspelledWords.Clear();
            UpdateRTB_BackgroundWorker();
        }

        void tablePicker_TableSelected(Table table)
        {
            try
            {
                Block current = GetCurrentBlock(LogicalDirection.Backward);
                mainRTB.Document.Blocks.InsertBefore(current, table);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void ToolStripButtonSubscript_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentAlignment = mainRTB.Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);
                BaselineAlignment newAlignment = ((BaselineAlignment)currentAlignment == BaselineAlignment.Subscript) ? BaselineAlignment.Baseline : BaselineAlignment.Subscript;
                mainRTB.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, newAlignment);
            }
            catch //(Exception generalException)
            {

            }
        }

        private void ToolStripButtonSuperscript_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var currentAlignment = mainRTB.Selection.GetPropertyValue(Inline.BaselineAlignmentProperty);
                BaselineAlignment newAlignment = ((BaselineAlignment)currentAlignment == BaselineAlignment.Superscript) ? BaselineAlignment.Baseline : BaselineAlignment.Superscript;
                mainRTB.Selection.ApplyPropertyValue(Inline.BaselineAlignmentProperty, newAlignment);
            }
            catch //(Exception generalException)
            {

            }
        }

        void txtMessageContextMenu_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuitem = sender as MenuItem;
                {
                    if (menuitem != null)
                    {
                        if (menuitem.Header.ToString() == "Cut")
                        {
                            mainRTB.Cut();
                        }
                        else if (menuitem.Header.ToString() == "Copy")
                        {
                            mainRTB.Copy();
                        }
                        else if (menuitem.Header.ToString() == "Paste")
                        {
                            mainRTB.Paste();
                        }

                        else if (menuitem.Header.ToString() == "Select All")
                        {
                            mainRTB.SelectAll();
                        }
                        else if (menuitem.Header.ToString() == "Enable Spell Checking")
                        {
                            _isEnableSpellCheck = true;
                            btnSpellCheck.IsChecked = true;
                            menuitem.Header = "Disable Spell Checking";
                            StartSpellCheck();
                        }
                        else if (menuitem.Header.ToString() == "Disable Spell Checking")
                        {
                            _isEnableSpellCheck = false;
                            btnSpellCheck.IsChecked = false;
                            _spellChecker.IgnoredWords.Clear();
                            _spellChecker.MisspelledWords.Clear();
                            menuitem.Header = "Enable Spell Checking";
                            StopSpellCheck();
                        }

                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while txtMessageContextMenu_Click() :" + exception.ToString());
            }
        }

        void txtMessageContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            try
            {
                ContextMenu contextmenu = sender as ContextMenu;
                if (contextmenu != null)
                    foreach (MenuItem mi in contextmenu.Items)
                    {
                        if ((string)mi.Header == "Paste")
                        {
                            if (Clipboard.ContainsText())
                            {
                                mi.IsEnabled = true;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                        if ((string)mi.Header == "Copy")
                        {
                            if (mainRTB.Document.Blocks != null)
                            {
                                if (mainRTB.Selection.Text != string.Empty)
                                    mi.IsEnabled = true;
                                else
                                    mi.IsEnabled = false;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                        if ((string)mi.Header == "Cut")
                        {
                            if (mainRTB.Document.Blocks != null)
                            {
                                if (mainRTB.Selection.Text != string.Empty)
                                    mi.IsEnabled = true;
                                else
                                    mi.IsEnabled = false;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                        if ((string)mi.Header == "Select All")
                        {
                            if (mainRTB.Document.Blocks != null)
                            {
                                mi.IsEnabled = true;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                        //if (!_isEnableSpellCheck)
                        //{

                        //    menuitem.Header = "Enable Spell Checking";
                        //    StartSpellCheck();
                        //}
                        //else if (_isEnableSpellCheck)
                        //{
                        //    _isEnableSpellCheck = false;
                        //    btnSpellCheck.IsChecked = false;
                        //    _spellChecker.IgnoredWords.Clear();
                        //    _spellChecker.MisspelledWords.Clear();
                        //    menuitem.Header = "Disable Spell Checking";
                        //    StopSpellCheck();
                        //}
                        //if ((string)mi.Header == "Enable Spell Checking")
                        //{
                        //    _isEnableSpellCheck = false;
                        //}
                        //if ((string)mi.Header == "Disable Spell Checking")
                        //{
                        //    _isEnableSpellCheck = true;
                        //}
                    }
            }
            catch (Exception exception)
            {
                _logger.Error(" Error occurred while txtMessageContextMenu_Opened() :" + exception.ToString());
            }
        }

        private void UpdateRTB_BackgroundWorker()
        {
            if (updateRTB != null && updateRTB.IsAlive)
                updateRTB.Abort();
            updateRTB = new Thread(UpdateSpellcheck);
            updateRTB.IsBackground = true;
            updateRTB.Priority = ThreadPriority.BelowNormal;
            updateRTB.Start();
        }

        private void UpdateSelectedFontFamily()
        {
            try
            {
                object value = mainRTB.Selection.GetPropertyValue(TextElement.FontFamilyProperty);
                FontFamily currentFontFamily = (FontFamily)((value == DependencyProperty.UnsetValue) ? null : value);
                if (currentFontFamily != null)
                {
                    cmbFontFamily.SelectedItem = currentFontFamily;
                }
            }
            catch //(Exception generalException)
            {

            }
        }

        private void UpdateSelectedFontSize()
        {
            try
            {
                object value = mainRTB.Selection.GetPropertyValue(TextElement.FontSizeProperty);
                cmbFontSize.SelectedValue = (value == DependencyProperty.UnsetValue) ? null : value;
            }
            catch // (Exception generalException)
            {

            }
        }

        private void UpdateSpellcheck()
        {
            try
            {
                // Add mispellwords in Regex example: thi|anf|
                // Start
                string mispelledWords = string.Empty;
                if (_spellChecker.MisspelledWords.Count > 0)
                {
                    for (int i = 0; i < _spellChecker.MisspelledWords.Count; i++)
                    {
                        if (!string.IsNullOrEmpty(mispelledWords))
                            mispelledWords = mispelledWords + "|" + _spellChecker.MisspelledWords[i].ToString();
                        else
                            mispelledWords = _spellChecker.MisspelledWords[i].ToString();
                    }
                }
                Regex regMispell = new Regex(mispelledWords, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                // End

                // Loop for the richtextbox content one by one
                var start = mainRTB.Document.ContentStart;
                while (start != null && start.CompareTo(mainRTB.Document.ContentEnd) < 0)
                {
                    //Dispatcher.CurrentDispatcher.BeginInvoke((Action)(delegate
                    //{
                    if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                    {
                        try
                        {
                            // Get the word
                            string text = start.GetTextInRun(LogicalDirection.Forward);

                            //if (_spellChecker.MisspelledWords.Count > 0)
                            //{
                            Match matchedMispellWords = regMispell.Match(start.GetTextInRun(LogicalDirection.Forward));
                            if (matchedMispellWords.Length > 0 && checkfullword(matchedMispellWords.Value, text))
                            {
                                mainRTB.Dispatcher.Invoke((Action)(delegate
                                   {
                                       try
                                       {
                                           var textrange = new TextRange(start.GetPositionAtOffset(matchedMispellWords.Index, LogicalDirection.Forward), start.GetPositionAtOffset(matchedMispellWords.Index + matchedMispellWords.Length, LogicalDirection.Backward));
                                           textrange.ApplyPropertyValue(Inline.TextDecorationsProperty, _txtDecorationCollection);
                                           start = textrange.End;

                                           //Canvas canvas = new Canvas();
                                           //Rectangle dfr = new Rectangle() { Fill = wavyBrush, Height = 5, Width = dfrtg.Width };
                                           //canvas.Children.Add(dfr);
                                           //new InlineUIContainer(canvas, start);
                                       }
                                       catch (Exception ex)
                                       {
                                           _logger.Error("update spell check" + ex.Message);
                                       }
                                   }));
                                Thread.Sleep(80);
                            }
                            else
                            {
                                if (text.Trim().Length > 0 && !_spellChecker.MisspelledWords.Contains(text))
                                {
                                    // Add correct words in Regex example: the|any|
                                    // Start

                                    string correctWords = string.Empty;
                                    text = text.Replace("\r\n", " ");
                                    text = text.Replace(@"\", "");
                                    text = removeSpecialChars(text);
                                    var tempWords = text.Split(' ');
                                    foreach (var tempWord in tempWords)
                                    {
                                        if (!string.IsNullOrEmpty(correctWords))
                                            correctWords = correctWords + "|" + tempWord;
                                        else
                                            correctWords = tempWord;
                                    }
                                    Regex regCorrectWord = new Regex(correctWords, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                                    //End
                                    mainRTB.Dispatcher.Invoke((Action)(delegate
                                       {
                                           try
                                           {
                                               Match matchedCorrectWords = regCorrectWord.Match(start.GetTextInRun(LogicalDirection.Forward));
                                               var textrange = new TextRange(start.GetPositionAtOffset(matchedCorrectWords.Index, LogicalDirection.Forward), start.GetPositionAtOffset(matchedCorrectWords.Index + matchedCorrectWords.Length, LogicalDirection.Backward));
                                               textrange.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                                               start = textrange.End;
                                           }
                                           catch (Exception ex)
                                           {
                                               _logger.Error("update spell check" + ex.Message);
                                           }
                                       }));
                                    Thread.Sleep(80);
                                }
                            }
                            // Replace the styled mispelled word to empty example : text: org the -->replace "org" to string.empty

                            //}

                            //Apply normal style for other words
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error occured while updating Spellcheck : " + ex.Message);
                        }
                    }
                    mainRTB.Dispatcher.Invoke((Action)(delegate
                    {
                        start = start.GetNextContextPosition(LogicalDirection.Forward);
                    }));
                }
            }
            catch (Exception ex)
            {
                _logger.Error("Error occured while updating Spellcheck : " + ex.Message);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (isControlEditable)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.ContextIdle,
            new Action(delegate()
            {
                this.mainRTB.Focus();
            }));
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
        }

        void _mnuLang_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menuItem = sender as MenuItem;
                if (menuItem != null)
                {
                    _spellChecker.SetDictionary(menuItem.Tag as AbstractDictionary);
                    string[] lines = System.IO.File.ReadAllLines(CustomDictionary);
                    if (lines != null && lines.Length > 0)
                        for (int i = 0; i < lines.Length; i++)
                            _spellChecker.AddTodictionary(lines[i]);
                    var textRange = new TextRange(mainRTB.Document.ContentStart, mainRTB.Document.ContentEnd);
                    textRange.ApplyPropertyValue(Inline.TextDecorationsProperty, null);
                    if (_isEnableSpellCheck)
                        StartSpellCheck();
                }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while _mnuLang_Click() :" + generalException.ToString());
            }
        }

        void _rtbContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            try
            {
                ContextMenu contextmenu = sender as ContextMenu;
                if (contextmenu != null)
                    foreach (MenuItem mi in contextmenu.Items)
                    {
                        if ((string)mi.Header == "Copy")
                        {
                            if (mainRTB.Document != null)
                            {

                                if (mainRTB.Selection.Text != string.Empty)
                                    mi.IsEnabled = true;
                                else
                                    mi.IsEnabled = false;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                        if ((string)mi.Header == "Select All")
                        {
                            if (mainRTB.Document != null)
                            {
                                mi.IsEnabled = true;
                            }
                            else
                            {
                                mi.IsEnabled = false;
                            }
                        }
                    }
            }
            catch (Exception generalException)
            {
                _logger.Error("Error occurred while _rtbContextMenu_Opened() :" + generalException.ToString());
            }
        }

        #endregion Methods

        #region Other

        private void mainRTB_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Key == Key.T && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
                {
                    e.Handled = true;
                }
            }
            catch (Exception generalException)
            {
                _logger.Error(" Error occurred while menuSuggestion_Click() :" + generalException.ToString());
            }
        }

        //private void mainRTBundo(object sender, KeyEventArgs e)
        //{
        //    if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control && isFormattochange)
        //    {
        //        isFormattochange = false;
        //        if (cmbHTMLFormat.SelectedIndex == 0)
        //            cmbHTMLFormat.SelectedIndex = 1;
        //        else
        //            cmbHTMLFormat.SelectedIndex = 0;
        //    }
        //}
        // bool isFormattochange = false;

        #endregion Other
    }
}