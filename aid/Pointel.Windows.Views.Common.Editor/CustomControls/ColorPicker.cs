using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Pointel.Windows.Views.Common.Editor.CustomControls
{
	public class ColorPicker : Button, IComponentConnector
	{
		private Color selectedColor = Colors.Transparent;
		private Color customColor = Colors.Transparent;
		private bool isContexMenuOpened;
		private static IList<Color> allColors = ColorPicker.GetColors();
		public static readonly DependencyProperty SelectedColorNameProperty = DependencyProperty.Register("SelectedColorName", typeof(string), typeof(ColorPicker), new UIPropertyMetadata("Unknown"));
		private PropertyChangeNotifier notifier;
		internal Expander epDefaultcolor;
		internal ListBox DefaultPicker;
		internal Expander epCustomcolor;
		internal ImageColorPicker imageColorPicker;
		internal Rectangle recContentCustom;
		internal Rectangle recContent;
		private bool _contentLoaded;
		public event Action<Color> SelectedColorChanged;
		public string SelectedColorName
		{
			get
			{
				return (string)base.GetValue(ColorPicker.SelectedColorNameProperty);
			}
			set
			{
				base.SetValue(ColorPicker.SelectedColorNameProperty, value);
			}
		}
		public Color SelectedColor
		{
			get
			{
				return this.selectedColor;
			}
			set
			{
				if (this.selectedColor != value)
				{
					this.selectedColor = value;
					this.CustomColor = value;
				}
			}
		}
		private Color CustomColor
		{
			get
			{
				return this.customColor;
			}
			set
			{
				if (this.customColor != value)
				{
					this.customColor = value;
					this.UpdatePreview();
				}
			}
		}
		public ColorPicker()
		{
			this.InitializeComponent();
			this.SelectedColorName = new BrushConverter().ConvertToString(new SolidColorBrush(this.selectedColor));
			this.DefaultPicker.ItemsSource = ColorPicker.allColors;
		}
		private static bool NearlyEqual(float a, float b, float epsilon)
		{
			float num = Math.Abs(a);
			float num2 = Math.Abs(b);
			float num3 = Math.Abs(a - b);
			if (a * b == 0f)
			{
				return num3 < epsilon * epsilon;
			}
			return num3 / (num + num2) < epsilon;
		}
		private static IList<Color> GetColors()
		{
			List<string> list = new List<string>
			{
				"#FFFFFF",
				"#C0C0C0",
				"#808080",
				"#000000",
				"#FF0000",
				"#800000",
				"#FFFF00",
				"#808000",
				"#00FF00",
				"#008000",
				"#00FFFF",
				"#008080",
				"#0000FF",
				"#000080",
				"#FF00FF",
				"#800080"
			};
			List<Color> list2 = new List<Color>();
			foreach (string current in list)
			{
				list2.Add((Color)ColorConverter.ConvertFromString(current));
			}
			return list2;
		}
		private void CanColor_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
			this.CustomColor = this.imageColorPicker.SelectedColor;
			base.ContextMenu.IsOpen = false;
		}
		private void DefaultPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (this.DefaultPicker.SelectedValue != null)
			{
				this.customColor = (Color)this.DefaultPicker.SelectedValue;
			}
			for (FrameworkElement frameworkElement = this; frameworkElement != null; frameworkElement = (frameworkElement.Parent as FrameworkElement))
			{
				if (frameworkElement is ContextMenu)
				{
					((ContextMenu)frameworkElement).IsOpen = false;
					return;
				}
				if (frameworkElement.Parent == null)
				{
					return;
				}
			}
		}
		private bool SimmilarColor(Color pointColor, Color selectedColor)
		{
			int num = Math.Abs((int)(pointColor.R - selectedColor.R)) + Math.Abs((int)(pointColor.G - selectedColor.G)) + Math.Abs((int)(pointColor.B - selectedColor.B));
			return num < 20;
		}
		private void UpdatePreview()
		{
			this.recContent.Fill = new SolidColorBrush(this.CustomColor);
			this.recContentCustom.Fill = new SolidColorBrush(this.CustomColor);
		}
		private void TabItem_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			e.Handled = true;
		}
		private void epDefaultcolor_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			this.epCustomcolor.IsExpanded = false;
		}
		private void epCustomcolor_Expanded(object sender, RoutedEventArgs e)
		{
			this.epDefaultcolor.IsExpanded = false;
		}
		private void ContextMenu_Opened(object sender, RoutedEventArgs e)
		{
			this.isContexMenuOpened = true;
		}
		private void ContextMenu_Closed(object sender, RoutedEventArgs e)
		{
			if (!base.ContextMenu.IsOpen)
			{
				Brush brush = new SolidColorBrush(this.CustomColor);
				this.recContent.Fill = brush;
				this.SelectedColorName = new BrushConverter().ConvertToString(brush);
				if (this.SelectedColorChanged != null)
				{
					this.SelectedColorChanged(this.CustomColor);
				}
			}
			this.isContexMenuOpened = false;
		}
		private void b_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (!this.isContexMenuOpened && base.ContextMenu != null && !base.ContextMenu.IsOpen)
			{
				base.ContextMenu.PlacementTarget = this;
				base.ContextMenu.Placement = PlacementMode.Bottom;
				ContextMenuService.SetPlacement(this, PlacementMode.Bottom);
				base.ContextMenu.IsOpen = true;
			}
		}
		private void OnSelectedColorChanged(object sender, EventArgs e)
		{
			this.CustomColor = this.imageColorPicker.SelectedColor;
		}
		private void Button_Loaded(object sender, RoutedEventArgs e)
		{
			this.notifier = new PropertyChangeNotifier(this.imageColorPicker, "SelectedColor");
			this.notifier.ValueChanged += new EventHandler(this.OnSelectedColorChanged);
		}
		private void Button_Unloaded(object sender, RoutedEventArgs e)
		{
			if (this.notifier != null)
			{
				this.notifier.ValueChanged -= new EventHandler(this.OnSelectedColorChanged);
				this.notifier.Dispose();
			}
		}
		[DebuggerNonUserCode]
		public void InitializeComponent()
		{
			if (this._contentLoaded)
			{
				return;
			}
			this._contentLoaded = true;
			Uri resourceLocator = new Uri("/Genesyslab.Desktop.Modules.Windows;component/views/common/editor/colorpicker.xaml", UriKind.Relative);
			Application.LoadComponent(this, resourceLocator);
		}
		[DebuggerNonUserCode]
		internal Delegate _CreateDelegate(Type delegateType, string handler)
		{
			return Delegate.CreateDelegate(delegateType, this, handler);
		}
		[EditorBrowsable(EditorBrowsableState.Never), DebuggerNonUserCode]
		void IComponentConnector.Connect(int connectionId, object target)
		{
			switch (connectionId)
			{
				case 1:
					((ColorPicker)target).PreviewMouseLeftButtonUp += new MouseButtonEventHandler(this.b_PreviewMouseLeftButtonUp);
					((ColorPicker)target).Loaded += new RoutedEventHandler(this.Button_Loaded);
					((ColorPicker)target).Unloaded += new RoutedEventHandler(this.Button_Unloaded);
					return;
				case 2:
					((ContextMenu)target).Opened += new RoutedEventHandler(this.ContextMenu_Opened);
					((ContextMenu)target).Closed += new RoutedEventHandler(this.ContextMenu_Closed);
					return;
				case 3:
					this.epDefaultcolor = (Expander)target;
					this.epDefaultcolor.PreviewMouseLeftButtonDown += new MouseButtonEventHandler(this.epDefaultcolor_PreviewMouseLeftButtonDown);
					return;
				case 4:
					this.DefaultPicker = (ListBox)target;
					this.DefaultPicker.SelectionChanged += new SelectionChangedEventHandler(this.DefaultPicker_SelectionChanged);
					return;
				case 5:
					this.epCustomcolor = (Expander)target;
					this.epCustomcolor.Expanded += new RoutedEventHandler(this.epCustomcolor_Expanded);
					return;
				case 6:
					this.imageColorPicker = (ImageColorPicker)target;
					return;
				case 7:
					this.recContentCustom = (Rectangle)target;
					return;
				case 8:
					this.recContent = (Rectangle)target;
					return;
				default:
					this._contentLoaded = true;
					return;
			}
		}
	}
}
