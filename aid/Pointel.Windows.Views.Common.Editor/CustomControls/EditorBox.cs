using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using ICSharpCode.SharpZipLib.Zip;
using Pointel.Windows.Views.Common.Editor.Common;
using Pointel.Windows.Views.Common.Editor.Controls;
using Pointel.Windows.Views.Common.Editor.HTMLConverter;

namespace Pointel.Windows.Views.Common.Editor.CustomControls
{
	public class EditorBox : RichTextBoxNoAutoResize, IDisposable
	{
		private bool debug = false;
		private ScrollViewer scrollViewer;
		private static Regex regexFlowDocument = new Regex("^<FlowDocument[^>]*>(.*)</FlowDocument>$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		private static Regex regexImageToolTipSource = new Regex("\\[(http.*)\\]$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		private static Regex regexHtmlDocument = new Regex("^<body[^>]*>(.*)</FlowDocument>$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		private static List<ICommand> commandsToRemoveInReadOnly = new List<ICommand>
		{
			EditingCommands.AlignCenter,
			EditingCommands.AlignJustify,
			EditingCommands.AlignLeft,
			EditingCommands.AlignRight,
			EditingCommands.Backspace,
			EditingCommands.CorrectSpellingError,
			EditingCommands.DecreaseFontSize,
			EditingCommands.DecreaseIndentation,
			EditingCommands.Delete,
			EditingCommands.DeleteNextWord,
			EditingCommands.DeletePreviousWord,
			EditingCommands.EnterLineBreak,
			EditingCommands.EnterParagraphBreak,
			EditingCommands.IgnoreSpellingError,
			EditingCommands.IncreaseFontSize,
			EditingCommands.IncreaseIndentation,
			EditingCommands.ToggleBold,
			EditingCommands.ToggleBullets,
			EditingCommands.ToggleInsert,
			EditingCommands.ToggleItalic,
			EditingCommands.ToggleNumbering,
			EditingCommands.ToggleSubscript,
			EditingCommands.ToggleSuperscript,
			EditingCommands.ToggleUnderline,
			EditingCommands.ToggleUnderline,
			ApplicationCommands.Cut,
			ApplicationCommands.Paste,
			ApplicationCommands.Delete,
			ApplicationCommands.Undo,
			ApplicationCommands.Redo
		};
		private static List<ICommand> commandsToRemoveInFormatText = new List<ICommand>
		{
			EditingCommands.DecreaseFontSize,
			EditingCommands.DecreaseIndentation,
			EditingCommands.IncreaseFontSize,
			EditingCommands.IncreaseIndentation,
			EditingCommands.ToggleBold,
			EditingCommands.ToggleBullets,
			EditingCommands.ToggleItalic,
			EditingCommands.ToggleNumbering,
			EditingCommands.ToggleSubscript,
			EditingCommands.ToggleSuperscript,
			EditingCommands.ToggleUnderline
		};
		public static readonly DependencyProperty OneLineOnlyProperty = DependencyProperty.Register("OneLineOnly", typeof(bool), typeof(EditorBox), new UIPropertyMetadata(false, new PropertyChangedCallback(EditorBox.OneLineOnlyPropertyChangedCallback)));
		public new static readonly DependencyProperty IsReadOnlyWithNavigationProperty = DependencyProperty.Register("IsReadOnlyWithNavigation", typeof(bool), typeof(EditorBox), new UIPropertyMetadata(false, new PropertyChangedCallback(EditorBox.IsReadOnlyWithNavigationPropertyChangedCallback)));
		public static readonly DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(EditorType), typeof(EditorBox), new UIPropertyMetadata(EditorType.HTML));
		public static readonly DependencyProperty ShowImagesProperty = DependencyProperty.Register("ShowImages", typeof(bool), typeof(EditorBox), new UIPropertyMetadata(true, new PropertyChangedCallback(EditorBox.ShowImagesPropertyChangedCallback)));
		private Rect dragRect;
		private bool dragStarted;
		private bool isDragging;
		private IDictionary<string, MemoryStream> imageCache = new Dictionary<string, MemoryStream>();
		private object imageCacheSyncRoot = new object();
		private WebClient webClient;
		private object webClientSyncRoot = new object();
		public event ScrollChangedEventHandler ScrollChanged;
		public bool OneLineOnly
		{
			get
			{
				return (bool)base.GetValue(EditorBox.OneLineOnlyProperty);
			}
			set
			{
				base.SetValue(EditorBox.OneLineOnlyProperty, value);
			}
		}
		public new bool IsReadOnlyWithNavigation
		{
			get
			{
				return (bool)base.GetValue(EditorBox.IsReadOnlyWithNavigationProperty);
			}
			set
			{
				base.SetValue(EditorBox.IsReadOnlyWithNavigationProperty, value);
			}
		}
		public EditorType Format
		{
			get
			{
				return (EditorType)base.GetValue(EditorBox.FormatProperty);
			}
			set
			{
				base.SetValue(EditorBox.FormatProperty, value);
			}
		}
		public bool ShowImages
		{
			get
			{
				return (bool)base.GetValue(EditorBox.ShowImagesProperty);
			}
			set
			{
				base.SetValue(EditorBox.ShowImagesProperty, value);
			}
		}
		public IWebProxy Proxy
		{
			get;
			set;
		}
		public EditorBox()
		{
			CommandManager.AddPreviewCanExecuteHandler(this, new CanExecuteRoutedEventHandler(this.CanExecuteRoutedEventCallback));
			base.CommandBindings.Clear();
			base.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, new ExecutedRoutedEventHandler(EditorBox.ExecutedCutRoutedCommand), new CanExecuteRoutedEventHandler(EditorBox.CanExecuteCutRoutedCommand)));
			base.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(EditorBox.ExecutedCopyRoutedCommand), new CanExecuteRoutedEventHandler(EditorBox.CanExecuteCopyRoutedCommand)));
			base.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, new ExecutedRoutedEventHandler(EditorBox.ExecutedPasteRoutedCommand), new CanExecuteRoutedEventHandler(EditorBox.CanExecutePasteRoutedCommand)));
			base.CommandBindings.Add(new CommandBinding(EditingCommands.EnterParagraphBreak, new ExecutedRoutedEventHandler(EditorBox.ExecutedEnterParagraphBreakRoutedCommand)));
			base.CommandBindings.Add(new CommandBinding(EditingCommands.EnterLineBreak, new ExecutedRoutedEventHandler(EditorBox.ExecutedEnterLineBreakRoutedCommand)));
			this.Proxy = WebRequest.DefaultWebProxy;
		}
		~EditorBox()
		{
			this.Dispose();
		}
		public override void OnApplyTemplate()
		{
			try
			{
				base.OnApplyTemplate();
				if (this.scrollViewer != null)
				{
					this.scrollViewer.ScrollChanged -= new ScrollChangedEventHandler(this.scrollViewer_ScrollChanged);
				}
				this.scrollViewer = (base.GetTemplateChild("PART_ContentHost") as ScrollViewer);
				if (this.scrollViewer != null)
				{
					this.scrollViewer.ScrollChanged += new ScrollChangedEventHandler(this.scrollViewer_ScrollChanged);
				}
			}
			catch //(Exception ex)
			{

			}
		}
		public void Dispose()
		{ 
			if (this.scrollViewer != null)
			{
				this.scrollViewer.ScrollChanged -= new ScrollChangedEventHandler(this.scrollViewer_ScrollChanged);
			}
			object obj;
			Monitor.Enter(obj = this.imageCacheSyncRoot);
			try
			{
				foreach (MemoryStream current in this.imageCache.Values)
				{
					if (current != null)
					{
						current.Dispose();
					}
				}
				this.imageCache.Clear();
			}
			finally
			{
				Monitor.Exit(obj);
			}
			if (this.webClient != null)
			{
				this.webClient.Dispose();
			}
		}
		private void CanExecuteRoutedEventCallback(object sender, CanExecuteRoutedEventArgs e)
		{
			try
			{
				if (this.IsReadOnlyWithNavigation && EditorBox.commandsToRemoveInReadOnly.Contains(e.Command))
				{
					e.CanExecute = false;
					e.Handled = true;
				}
				if (this.Format == EditorType.Text && EditorBox.commandsToRemoveInFormatText.Contains(e.Command))
				{
					e.CanExecute = false;
					e.Handled = true;
				}
			}
			catch //(Exception ex)
			{

			}
		}
		private static void OneLineOnlyPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{ 
			try
			{
				EditorBox editorBox = d as EditorBox;
				if (editorBox != null)
				{
					if (editorBox.OneLineOnly)
					{
						editorBox.VerticalScrollBarVisibility = (editorBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden);
						return;
					}
					editorBox.VerticalScrollBarVisibility = (editorBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto);
				} 
			}
			catch //(Exception ex)
			{

			}
		}
		private static void IsReadOnlyWithNavigationPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{ 
			try
			{
				EditorBox editorBox = d as EditorBox;
				if (editorBox != null && editorBox.IsReadOnlyWithNavigation)
				{
					editorBox.IsReadOnly = false;
				}
			}
			catch //(Exception ex)
			{

			}
		}
		private static void ShowImagesPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			 try
			{
				Pointel.Windows.Views.Common.Editor.Controls.HTMLEditor editor = d as HTMLEditor;
				if (editor != null && editor.EditorBox != null && !(bool)e.OldValue && (bool)e.NewValue)
				{                
					editor.LoadImages();
				} 
			 }
			catch //(Exception ex)
			{

			}
		}
		private static void ExecutedCutRoutedCommand(object sender, ExecutedRoutedEventArgs e)
		{
			try
			{
				EditorBox editorBox = sender as EditorBox;
				if (editorBox != null)
				{
					if (editorBox.Format == EditorType.Text)
					{
						string text = editorBox.Selection.Text;
						editorBox.Selection.Text = string.Empty;
						Clipboard.SetText(text);
					}
					else
					{
						Clipboard.SetDataObject(editorBox.GetSelectedData());
						editorBox.Selection.Text = string.Empty;
					}
					e.Handled = true;
				} 
			}
			catch //(Exception ex)
			{

			}
		}
		private static void CanExecuteCutRoutedCommand(object sender, CanExecuteRoutedEventArgs e)
		{ 
			try
			{
				EditorBox editorBox = sender as EditorBox;
				if (editorBox != null)
				{
					if (editorBox.Format == EditorType.Text)
					{
						e.CanExecute = (!editorBox.IsReadOnly && !editorBox.IsReadOnlyWithNavigation && !editorBox.Selection.IsEmpty);
						return;
					}
					e.CanExecute = (!editorBox.IsReadOnly && !editorBox.IsReadOnlyWithNavigation && !editorBox.Selection.IsEmpty);
				} 
			}
			catch //(Exception ex)
			{

			}
		}
		private static void ExecutedCopyRoutedCommand(object sender, ExecutedRoutedEventArgs e)
		{ 
			try
			{
				EditorBox editorBox = sender as EditorBox;
				if (editorBox != null)
				{
					if (editorBox.Format == EditorType.Text)
					{
						Clipboard.SetText(editorBox.Selection.Text);
					}
					else
					{
						Clipboard.SetDataObject(editorBox.GetSelectedData());
					}
					e.Handled = true;
				} 
			}
			catch //(Exception ex)
			{

			}
		}
		private static void CanExecuteCopyRoutedCommand(object sender, CanExecuteRoutedEventArgs e)
		{ 
			try
			{
				EditorBox editorBox = sender as EditorBox;
				if (editorBox != null)
				{
					if (editorBox.Format == EditorType.Text)
					{
						e.CanExecute = !editorBox.Selection.IsEmpty;
						return;
					}
					e.CanExecute = !editorBox.Selection.IsEmpty;
				}
			}
			catch //(Exception ex)
			{

			}
		}
		private static void ExecutedPasteRoutedCommand(object sender, ExecutedRoutedEventArgs e)
		{ 
			try
			{
				EditorBox editorBox = sender as EditorBox;
				if (editorBox != null && !EditorBox.PasteOrDropOperation(editorBox, Clipboard.GetDataObject(), e))
				{
					editorBox.Paste();
				}
			}
			catch //(Exception ex)
			{

			}
		}
		private static void CanExecutePasteRoutedCommand(object sender, CanExecuteRoutedEventArgs e)
		{ 
			try
			{
				EditorBox editorBox = sender as EditorBox;
				if (editorBox != null)
				{
					if (editorBox.Format == EditorType.Text)
					{
						e.CanExecute = (!editorBox.IsReadOnly && !editorBox.IsReadOnlyWithNavigation && Clipboard.ContainsText(TextDataFormat.Text));
					}
					else
					{
						e.CanExecute = (!editorBox.IsReadOnly && !editorBox.IsReadOnlyWithNavigation);
					}
				}
				e.Handled = !e.CanExecute;
			}
			catch //(Exception ex)
			{

			}
		}
		private static bool PasteOrDropOperation(EditorBox editorBox, IDataObject dataObject, RoutedEventArgs e)
		{
			if (editorBox.Format == EditorType.Text)
			{
				string text = null;
				if (dataObject.GetDataPresent(DataFormats.UnicodeText, true))
				{
					text = (string)dataObject.GetData(DataFormats.UnicodeText, true);
				}
				if (text == null && dataObject.GetDataPresent(DataFormats.Text, true))
				{
					text = (string)dataObject.GetData(DataFormats.Text, true);
				}
				if (text == null && dataObject.GetDataPresent(DataFormats.StringFormat, true))
				{
					text = (string)dataObject.GetData(DataFormats.StringFormat, true);
				}
				if (text != null)
				{
					if (editorBox.OneLineOnly)
					{
						text = text.Replace("\r", "").Replace("\n", " ");
					}
					editorBox.InsertText(text);
					editorBox.Selection.Text = "";
					e.Handled = true;
					return true;
				}
				return false;
			}
			else
			{
				if (dataObject.GetDataPresent(DataFormats.Html, true))
				{
					string html = (string)dataObject.GetData(DataFormats.Html, true);
					html = EditorBox.GetHTMLFromClipboardFormat(html);
					editorBox.InsertHTML(html);
					e.Handled = true;
					return true;
				}
				return false;
			} 
		}
		private static string GetHTMLFromClipboardFormat(string html)
		{
			int num = html.IndexOf("StartHTML", StringComparison.InvariantCultureIgnoreCase);
			if (num != -1)
			{
				num = html.IndexOf(":", num);
				if (num != -1)
				{
					num++;
					int num2 = html.IndexOf("\n", num);
					if (num2 != -1)
					{
						string s = html.Substring(num, num2 - num).Trim(new char[]
						{
							'\r',
							'\n',
							' '
						});
						int num3 = 0;
						try
						{
							num3 = int.Parse(s);
							num3 -= 2;
							int num4 = html.IndexOf("<html", StringComparison.InvariantCultureIgnoreCase);
							if (num4 != -1 && Math.Abs(num3 - num4) < 10)
							{
								num3 = num4;
							}
						}
						catch //(Exception)
						{
						}
						return html.Substring(num3);
					}
				}
			}
			return html;
		}
		protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			this.isDragging = true;
			try
			{
				Point position = e.GetPosition(this);
				TextPointer positionFromPoint = base.GetPositionFromPoint(position, true);
				int num = (int)SystemParameters.MinimumHorizontalDragDistance;
				int num2 = (int)SystemParameters.MinimumVerticalDragDistance;
				this.dragRect = new Rect(position.X - (double)num, position.Y - (double)num2, (double)(num * 2), (double)(num2 * 2));
				this.dragStarted = (base.Selection.Contains(positionFromPoint) && base.Selection.Start != base.Selection.End);
				if (!this.dragStarted)
				{
					base.OnPreviewMouseLeftButtonDown(e);
				}
			}
			
			catch //(Exception ex)
			{

			}
		}
		protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
		{ 
			try
			{
				if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control || base.IsReadOnly || this.IsReadOnlyWithNavigation)
				{
					Point point = default(Point);
					FrameworkElement frameworkElement = this.scrollViewer.Content as FrameworkElement;
					if (frameworkElement != null)
					{
						point = frameworkElement.PointToScreen(point);
						point = base.PointFromScreen(point);
					}
					Rect rect = new Rect(point, new Size(this.scrollViewer.ViewportWidth, this.scrollViewer.ViewportHeight));
					Point position = e.GetPosition(this);
					if (base.IsMouseOver && rect.Contains(Mouse.GetPosition(this)))
					{
						TextPointer positionFromPoint = base.GetPositionFromPoint(position, true);
						Hyperlink hyperlink = (positionFromPoint.GetAdjacentElement(LogicalDirection.Forward) as Hyperlink) ?? this.GetHyperlinkAncestor(positionFromPoint);
						if (hyperlink != null)
						{
							try
							{
								using (Process process = new Process())
								{
									process.StartInfo.FileName = hyperlink.NavigateUri.ToString();
									process.StartInfo.Verb = "Open";
									process.StartInfo.UseShellExecute = true;
									process.Start();
								}
								e.Handled = true;
							}
							catch
							{
							}
							return;
						}
					}
				}
				base.OnPreviewMouseLeftButtonUp(e);
				this.isDragging = false;
				this.dragStarted = false; 
			}
			catch //(Exception ex)
			{

			}
		}
		protected override void OnMouseMove(MouseEventArgs e)
		{ 
			try
			{
				if (this.dragStarted)
				{
					Point position = e.GetPosition(this);
					if (!this.dragRect.Contains(position.X, position.Y))
					{
						this.dragStarted = false;
						DragDropEffects dragDropEffects = DragDropEffects.Copy;
						if (!base.IsReadOnly)
						{
							dragDropEffects |= DragDropEffects.Move;
						}
						DataObject selectedData = this.GetSelectedData();
						DragDropEffects dragDropEffects2 = DragDrop.DoDragDrop(this, selectedData, dragDropEffects);
						if (!base.IsReadOnly && dragDropEffects2 == DragDropEffects.Move)
						{
							base.Selection.Text = "";
							return;
						}
					}
				}
				else
				{
					base.OnMouseMove(e);
				} 
			}
			catch //(Exception ex)
			{

			}
		}
		private DataObject GetSelectedData()
		{ 
			DataObject dataObject = new DataObject();
			string selectedText = this.GetSelectedText();
			string text = this.GetSelectedHTML();
			EditorBox.regexHtmlDocument = new Regex("^.*<body[^>]*>(.*)</body[^>]*>.*$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
			Match match = EditorBox.regexHtmlDocument.Match(text);
			if (match.Success)
			{
				text = match.Groups[1].ToString();
			}
			string format = "Version:0.9\r\nStartHTML:{0:D8}\r\nEndHTML:{1:D8}\r\nStartFragment:{2:D8}\r\nEndFragment:{3:D8}\r\n";
			string text2 = "<html>\r\n<body>\r\n<!--StartFragment-->";
			string text3 = "<!--EndFragment-->\r\n</body>\r\n</html>";
			int length = string.Format(format, new object[]
			{
				0,
				0,
				0,
				0
			}).Length;
			int num = length + text2.Length;
			int num2 = num + text.Length;
			int num3 = num2 + text3.Length;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(string.Format(format, new object[]
			{
				length,
				num3,
				num,
				num2
			}));
			stringBuilder.Append(text2);
			stringBuilder.Append(text);
			stringBuilder.Append(text3);
			text = stringBuilder.ToString();
			dataObject.SetData(DataFormats.Html, text);
			dataObject.SetData(DataFormats.UnicodeText, selectedText);
			dataObject.SetData(DataFormats.Text, Encoding.ASCII.GetString(Encoding.ASCII.GetBytes(selectedText)));
			return dataObject; 
		}
		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
		{ 
			try
			{
			if (!base.IsEnabled)
			{
				return;
			}
			e.Handled = true;
			e.Action = DragAction.Continue;
			bool flag = (e.KeyStates & DragDropKeyStates.LeftMouseButton) == DragDropKeyStates.None;
			if (e.EscapePressed)
			{
				e.Action = DragAction.Cancel;
				return;
			}
			if (flag)
			{
				e.Action = DragAction.Drop;
			} }
			catch //(Exception ex)
			{

			}
		}
		protected override void OnGiveFeedback(GiveFeedbackEventArgs e)
		{ try
			{
			if (!base.IsEnabled)
			{
				return;
			}
			e.UseDefaultCursors = true;
			e.Handled = true;}
			catch //(Exception ex)
			{

			}
		}
		protected override void OnDragEnter(DragEventArgs e)
		{ try
			{
			if (!this.AllowDragDrop(e))
			{
				return;
			}
			if ((e.AllowedEffects & DragDropEffects.Move) != DragDropEffects.None)
			{
				e.Effects = DragDropEffects.Move;
			}
			bool flag = (e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.None;
			if (flag)
			{
				e.Effects |= DragDropEffects.Copy;
			}}
			catch //(Exception ex)
			{

			}
		}
		protected override void OnDragOver(DragEventArgs e)
		{ try
			{
			if (!this.AllowDragDrop(e))
			{
				return;
			}
			if ((e.AllowedEffects & DragDropEffects.Move) != DragDropEffects.None)
			{
				e.Effects = DragDropEffects.Move;
			}
			bool flag = (e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.None;
			if (flag)
			{
				e.Effects |= DragDropEffects.Copy;
			}
			if (this.scrollViewer != null)
			{
				Point position = e.GetPosition(this.scrollViewer);
				double viewportHeight = this.scrollViewer.ViewportHeight;
				double num = 16.0;
				if (position.Y < num)
				{
					if (position.Y > num / 2.0)
					{
						base.LineUp();
						return;
					}
					base.PageUp();
					return;
				}
				else
				{
					if (position.Y > viewportHeight - num)
					{
						if (position.Y < viewportHeight - num / 2.0)
						{
							base.LineDown();
							return;
						}
						base.PageDown();
					}
				}
			}}
			catch //(Exception ex)
			{

			}
		}
		private bool AllowDragDrop(DragEventArgs e)
		{ 
			if (!this.IsReadOnlyWithNavigation && !base.IsReadOnly)
			{
				Window window = Window.GetWindow(this);
				if (window == null)
				{
					return true;
				}
				if (window.IsEnabled)
				{
					return true;
				}
			}
			e.Effects = DragDropEffects.None;
			return false;
		}
		protected override void OnDrop(DragEventArgs e)
		{ try
			{
			if (!this.AllowDragDrop(e))
			{
				return;
			}
			if (e.Data == null || e.AllowedEffects == DragDropEffects.None)
			{
				e.Effects = DragDropEffects.None;
			}
			else
			{
				if ((e.KeyStates & DragDropKeyStates.ControlKey) != DragDropKeyStates.None)
				{
					e.Effects = DragDropEffects.Copy;
				}
				else
				{
					if (e.Effects != DragDropEffects.Copy)
					{
						e.Effects = DragDropEffects.Move;
					}
				}
				Point position = e.GetPosition(this);
				TextPointer positionFromPoint = base.GetPositionFromPoint(position, true);
				if (positionFromPoint != null)
				{
					bool flag = false;
					using (base.DeclareChangeBlock())
					{
						if ((e.Effects & DragDropEffects.Move) != DragDropEffects.None && this.isDragging)
						{
							base.Selection.Text = string.Empty;
						}
						base.CaretPosition = positionFromPoint;
						if (!(flag = EditorBox.PasteOrDropOperation(this, e.Data, e)))
						{
							base.OnDrop(e);
						}
						else
						{
							e.Handled = true;
						}
					}
					if (flag)
					{
						if (e.Handled)
						{
							Window window = Window.GetWindow(this);
							if (window != null)
							{
								window.Activate();
							}
							base.Focus();
						}
						else
						{
							e.Effects = DragDropEffects.None;
						}
					}
				}
			}
			this.isDragging = false;}
			catch //(Exception ex)
			{

			}
		}
		private Hyperlink GetHyperlinkAncestor(TextPointer position)
		{ 
			Inline inline = position.Parent as Inline;
			while (inline != null && !(inline is Hyperlink))
			{
				inline = (inline.Parent as Inline);
			}
			return inline as Hyperlink;
		}
		public string GetText()
		{ 
			TextRange textRange = new TextRange(base.Document.ContentStart, base.Document.ContentEnd);
			string text = textRange.Text;
			if (text.EndsWith("\r\n"))
			{
				text = text.Substring(0, text.Length - 2);
			}
			return text;
		}
		public void SetText(string text)
		{ 
			TextRange textRange = new TextRange(base.Document.ContentStart, base.Document.ContentEnd);
			textRange.Text = text;
		}
		public string GetHTML()
		{ 
			if (this.Format == EditorType.Text)
			{
				return string.Empty;
			}
			TextRange selection = new TextRange(base.Document.ContentStart, base.Document.ContentEnd);
			return this.GetHTMLSelection(selection);
		}
		public void SetHTML(string html)
		{ try
			{
			if (this.Format == EditorType.Text)
			{
				string planeText = this.GetPlaneText(html);
				this.SetText(planeText);
				return;
			}
			string text = HtmlToXamlConverter.ConvertHtmlToXaml(html, true);
			base.Document.Blocks.Clear();
			if (!string.IsNullOrEmpty(text))
			{
				using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
				{
					ParserContext parserContext = new ParserContext();
					parserContext.XmlnsDictionary.Add("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
					parserContext.XmlnsDictionary.Add("x", "http://schemas.microsoft.com/winfx/2006/xaml");
					new FlowDocument();
					object obj = XamlReader.Load(memoryStream, parserContext);
					base.Document = (obj as FlowDocument);                  
				}
				if (this.ShowImages)
				{
					this.LoadImages();
				}
			}}
			catch //(Exception ex)
			{

			}
		}
		public string GetXAML()
		{ 
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				XamlWriter.Save(base.Document, memoryStream);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				string text = Encoding.UTF8.GetString(memoryStream.ToArray());
				Match match = EditorBox.regexFlowDocument.Match(text);
				if (match.Success)
				{
					text = "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\">" + match.Groups[1].ToString() + "</Section>";
				}
				result = text;
			}
			return result;
		}
		public string GetRTF()
		{
			string result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				TextRange textRange = new TextRange(base.Document.ContentStart, base.Document.ContentEnd);
				textRange.Save(memoryStream, DataFormats.Rtf);
				memoryStream.Seek(0L, SeekOrigin.Begin);
				string @string = Encoding.UTF8.GetString(memoryStream.ToArray());
				result = @string;
			}
			return result;
		}
		public void SetXAML(string newXAML)
		{
			try
			{
			if (!string.IsNullOrEmpty(newXAML))
			{
				using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(newXAML)))
				{
					try
					{
						if (base.Selection.CanLoad("Xaml"))
						{
							base.Selection.Load(memoryStream, "Xaml");
							base.Selection.Select(base.Selection.End, base.Selection.End);
						}
					}
					catch// (Exception)
					{
					}
				}
			}
			}
			catch (Exception ex)
			{

			}
		}
		public void InsertText(string text)
		{ try
			{
			this.InsertText(text, true);
			}
			catch //(Exception ex)
			{

			}
		}
		public void InsertText(string text, bool insertBeforeCursor)
		{ try
			{
			base.Selection.Text = text;
			if (insertBeforeCursor)
			{
				base.Selection.Select(base.Selection.End, base.Selection.End);
				return;
			}
			base.Selection.Select(base.Selection.Start, base.Selection.Start);}
			catch //(Exception ex)
			{

			}
		}
		public void InsertHTML(string html)
		{ try
			{
			this.InsertHTML(html, true);}
			catch //(Exception ex)
			{

			}
		}
		public void InsertHTML(string html, bool insertBeforeCursor)
		{ try
			{
			if (this.Format == EditorType.Text)
			{
				string planeText = this.GetPlaneText(html);
				this.InsertText(planeText);
				return;
			}
			string text = HtmlToXamlConverter.ConvertHtmlToXaml(html, false);
			if (!string.IsNullOrEmpty(text))
			{
				using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
				{
					if (base.Selection.CanLoad("Xaml"))
					{
						base.Selection.Load(memoryStream, "Xaml");
						if (insertBeforeCursor)
						{
							base.Selection.Select(base.Selection.End, base.Selection.End);
						}
						else
						{
							base.Selection.Select(base.Selection.Start, base.Selection.Start);
						}
					}
				}
				if (this.ShowImages)
				{
					this.LoadImages();
				}
			}}
			catch //(Exception ex)
			{

			}
		}
		public string GetPlaneText(string html)
		{ 
			string result = "";
			string text = HtmlToXamlConverter.ConvertHtmlToXaml(html, true);
			if (!string.IsNullOrEmpty(text))
			{
				using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
				{
					FlowDocument flowDocument = XamlReader.Load(memoryStream, new ParserContext
					{
						XmlnsDictionary = 
						{

							{
								"",
								"http://schemas.microsoft.com/winfx/2006/xaml/presentation"
							},

							{
								"x",
								"http://schemas.microsoft.com/winfx/2006/xaml"
							}
						}
					}) as FlowDocument;
					if (flowDocument != null)
					{
						TextRange textRange = new TextRange(flowDocument.ContentStart, flowDocument.ContentEnd);
						result = textRange.Text;
					}
				}
			}
			return result;
		}
		public string GetSelectedText()
		{ 
			return base.Selection.Text;
		}
		public string GetSelectedHTML()
		{
			if (this.Format == EditorType.Text)
			{
				return string.Empty;
			}
			return this.GetHTMLSelection(base.Selection);
		}
		public static byte[] ReadFully(Stream input)
		{
			byte[] buffer = new byte[16 * 1024];
			using (MemoryStream ms = new MemoryStream())
			{
				int read;
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					ms.Write(buffer, 0, read);
				}
				return ms.ToArray();
			}
		}
		private string GetHTMLSelection(TextRange selection)
		{ 
			using (MemoryStream memoryStream = new MemoryStream())
			{
				try
				{
					string text = "";
					bool flag = false;
					if (selection.CanSave(DataFormats.XamlPackage))
					{
						selection.Save(memoryStream, DataFormats.XamlPackage, true);
						memoryStream.Seek(0L, SeekOrigin.Begin);
						using (ZipFile zipFile = new ZipFile(memoryStream))
						{
							try
							{
								ZipEntry entry = zipFile.GetEntry("Xaml/Document.xaml");
								if (entry != null)
								{
									Stream inputStream = zipFile.GetInputStream(entry);
									byte[] array = ReadFully(inputStream);
									//int num = inputStream.Read(array, 0, (int)entry.get_Size());
									//long arg_85_0 = (long)num;
									//long arg_84_0 = entry.get_Size();
									text = Encoding.UTF8.GetString(array);
									flag = true;
									inputStream.Close();
								}
							}
							catch (Exception ex)
							{
								if (this.debug)
								{
									Console.WriteLine(ex.ToString());
								}
							}
						}
					}
					if (!flag && selection.CanSave("Xaml"))
					{
						selection.Save(memoryStream, "Xaml");
						memoryStream.Seek(0L, SeekOrigin.Begin);
						text = Encoding.UTF8.GetString(memoryStream.ToArray());
					}
					Match match = EditorBox.regexFlowDocument.Match(text);
					if (match.Success)
					{
						text = "<FlowDocument><Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\">" + match.Groups[1].ToString() + "</Section></FlowDocument>";
					}
					else
					{
						text = "<FlowDocument>" + text + "</FlowDocument>";
					}
					return HtmlFromXamlConverter.ConvertXamlToHtml(text);
				}
				catch //(Exception)
				{
				}
			}
			return null;
		}

		private IEnumerable<Image> FindImages(FlowDocument document)
		{ 
			return document.Blocks.SelectMany(block => FindImages(block));
		}

		private IEnumerable<Image> FindImages(Block block)
		{ 
			if (block is Table)
			{
				return ((Table)block).RowGroups
					.SelectMany(x => x.Rows)
					.SelectMany(x => x.Cells)
					.SelectMany(x => x.Blocks)
					.SelectMany(innerBlock => FindImages(innerBlock));
			}
			if (block is Paragraph)
			{
				return ((Paragraph)block).Inlines
					.OfType<InlineUIContainer>()
					.Where(x => x.Child is Image)
					.Select(x => x.Child as Image);
			}
			if (block is BlockUIContainer)
			{
				Image i = ((BlockUIContainer)block).Child as Image;
				return i == null
							? new List<Image>()
							: new List<Image>(new[] { i });
			}
			if (block is List)
			{
				return ((List)block).ListItems.SelectMany(listItem => listItem
																	  .Blocks
																	  .SelectMany(innerBlock => FindImages(innerBlock)));
			}
			if (block is Section)
			{
				return ((Section)block).Blocks.SelectMany(x => FindImages(x));
			}
			throw new InvalidOperationException("Unknown block type: " + block.GetType());
		}

		public void LoadImages()
		{ try
			{
			foreach (Image current in base.Document.FindChildren<Image>())
			{
				if (this.debug)
				{
					Console.WriteLine("Image source: " + current.Tag);
				}
				if (current.Source == null)
				{
					this.LoadImage(current);
				}
			}
			//foreach (Image current in FindImages(base.Document))
			//{
			//    if (this.debug)
			//    {
			//        Console.WriteLine("Image source: " + current.Tag);
			//    }
			//    if (current.Source == null)
			//    {
			//        this.LoadImage(current);
			//    }
			//}
			}
			catch //(Exception ex)
			{

			}
		}


		internal void LoadImage(Image image)
		{
			 try
			{BitmapImage bitmapImage = null;
			string url = image.Tag as string;
			string text = image.ToolTip as string;
			if (!string.IsNullOrEmpty(text))
			{
				Match match = EditorBox.regexImageToolTipSource.Match(text);
				if (match.Success)
				{
					url = match.Groups[1].ToString();
				}
			}
			MemoryStream memoryStream = null;
			bool flag = false;
			if (!string.IsNullOrEmpty(url))
			{
				object obj;
				Monitor.Enter(obj = this.imageCacheSyncRoot);
				try
				{
					flag = this.imageCache.TryGetValue(url, out memoryStream);
				}
				finally
				{
					Monitor.Exit(obj);
				}
				if (flag)
				{
					try
					{
						memoryStream.Seek(0L, SeekOrigin.Begin);
						bitmapImage = new BitmapImage();
						bitmapImage.BeginInit();
						bitmapImage.StreamSource = memoryStream;
						bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
						bitmapImage.EndInit();
						bitmapImage.UriSource = new Uri(url);
						bitmapImage.Freeze();
						image.Source = bitmapImage;
						return;
					}
					catch (Exception ex)
					{
						if (this.debug)
						{
							Console.WriteLine("Exception: " + ex.Message);
						}
						//ContainerAccessPoint.Container.Resolve<ILogger>().Error(string.Format("Cannot download the image: {0}", url), ex);
						return;
					}
				}
				Uri uri = null;
				try
				{
					uri = new Uri(url, UriKind.Absolute);
				}
				catch
				{
				}
				if (uri != null && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
				{
					Thread thread = new Thread(delegate()
					{
						try
						{
							HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
							webRequest.Proxy = this.Proxy;
							webRequest.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0)";
							webRequest.BeginGetResponse(delegate(IAsyncResult ar)
							{
								WebResponse webResponse = null;
								try
								{
									webResponse = webRequest.EndGetResponse(ar);
									if (webResponse != null)
									{
										Stream responseStream = webResponse.GetResponseStream();
										MemoryStream memoryStream2 = responseStream as MemoryStream;
										if (memoryStream2 == null)
										{
											memoryStream2 = new MemoryStream();
											byte[] array = new byte[8192];
											int count;
											while ((count = responseStream.Read(array, 0, array.Length)) > 0)
											{
												memoryStream2.Write(array, 0, count);
											}
										}
										object obj2;
										Monitor.Enter(obj2 = this.imageCacheSyncRoot);
										try
										{
											if (!this.imageCache.ContainsKey(url))
											{
												this.imageCache.Add(url, memoryStream2);
											}
										}
										finally
										{
											Monitor.Exit(obj2);
										}
										this.Dispatcher.BeginInvoke((Action)delegate
										{
											try
											{
												memoryStream2.Seek(0L, SeekOrigin.Begin);
												bitmapImage = new BitmapImage();
												bitmapImage.BeginInit();
												bitmapImage.StreamSource = memoryStream2;
												bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
												bitmapImage.EndInit();
												bitmapImage.UriSource = new Uri(url);
												bitmapImage.Freeze();
												image.Source = bitmapImage;
											}
											catch (Exception ex6)
											{
												if (this.debug)
												{
													Console.WriteLine("Exception: " + ex6.Message);
												}
											}
										}, new object[0]);
									}
								}
								catch (WebException ex4)
								{
									if (this.debug)
									{
										Console.WriteLine("WebException: " + ex4.Message);
									}
									//ContainerAccessPoint.Container.Resolve<ILogger>().Error(string.Format("Cannot download the image: {0}, Reason: {1}", url, ex4.Message));
								}
								catch (Exception ex5)
								{
									if (this.debug)
									{
										Console.WriteLine("Exception: " + ex5.Message);
									}
									//ContainerAccessPoint.Container.Resolve<ILogger>().Error(string.Format("Cannot download the image: {0}, Reason: {1}", url, ex5.Message));
								}
								finally
								{
									if (webResponse != null)
									{
										webResponse.Close();
									}
								}
							}, null);
						}
						catch //(WebException ex2)
						{
							//ContainerAccessPoint.Container.Resolve<ILogger>().Error(string.Format("Cannot download the image: {0}, Reason: {1}", url, ex2.Message));
						}
						//catch //(Exception ex3)
						//{
						//    //ContainerAccessPoint.Container.Resolve<ILogger>().Error(string.Format("Cannot download the image: {0}, Reason: {1}", url, ex3.Message));
						//}
					});
					{
						thread.IsBackground = true;
						thread.Priority = ThreadPriority.BelowNormal;
					};
					thread.Start();
				}
			}}
			catch //(Exception ex)
			{

			}
		}

		protected override void OnTextChanged(TextChangedEventArgs e)
		{ try
			{
			base.OnTextChanged(e);
			if (e.Changes.Count > 0)
			{
				foreach (TextChange current in e.Changes)
				{
					if (this.ShowImages && e.UndoAction == UndoAction.Undo && current.AddedLength > 0)
					{
						TextPointer textPointer = base.Document.ContentStart.GetPositionAtOffset(current.Offset);
						if (textPointer != null)
						{
							for (int i = 0; i < current.AddedLength; i++)
							{
								DependencyObject adjacentElement = textPointer.GetAdjacentElement(LogicalDirection.Forward);
								Image image = adjacentElement as Image;
								if (image != null)
								{
									//this.LoadImage(image);
								}
								textPointer = textPointer.GetNextContextPosition(LogicalDirection.Forward);
								if (textPointer == null)
								{
									break;
								}
							}
						}
					}
				}
			}}
			catch //(Exception ex)
			{

			}
		}
		private static void ExecutedEnterParagraphBreakRoutedCommand(object sender, ExecutedRoutedEventArgs e)
		{ try
			{
			EditorBox editorBox = sender as EditorBox;
			if (editorBox != null)
			{
				if (!editorBox.OneLineOnly)
				{
					editorBox.CaretPosition = editorBox.CaretPosition.InsertParagraphBreak();
					if (editorBox.CaretPosition != null && editorBox.CaretPosition.Paragraph != null)
					{
						editorBox.CaretPosition.Paragraph.Margin = new Thickness(0.0);
					}
				}
				e.Handled = true;
			}}
			catch //(Exception ex)
			{

			}
		}
		private static void ExecutedEnterLineBreakRoutedCommand(object sender, ExecutedRoutedEventArgs e)
		{
			 try
			{EditorBox editorBox = sender as EditorBox;
			if (editorBox != null)
			{
				if (!editorBox.OneLineOnly)
				{
					editorBox.CaretPosition = editorBox.CaretPosition.InsertLineBreak();
				}
				e.Handled = true;
			}}
			catch //(Exception ex)
			{

			}
		}
		protected override void OnPreviewTextInput(TextCompositionEventArgs e)
		{ try
			{
			if (this.IsReadOnlyWithNavigation && e.SystemText != " ")
			{
				e.Handled = true;
				return;
			}
			base.OnPreviewTextInput(e);}
			catch //(Exception ex)
			{

			}
		}
		protected override void OnPreviewKeyDown(KeyEventArgs e)
		{ try
			{
			base.OnPreviewKeyDown(e);
			if (this.IsReadOnlyWithNavigation && e.Key == Key.Space)
			{
				e.Handled = true;
			}}
			catch //(Exception ex)
			{

			}
		}
		private void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
		{ 
			try
			{
				if (this.ScrollChanged != null)
				{
					this.ScrollChanged(this, e);
				}
			}
			catch// (Exception ex)
			{

			}
		}
	}
}
