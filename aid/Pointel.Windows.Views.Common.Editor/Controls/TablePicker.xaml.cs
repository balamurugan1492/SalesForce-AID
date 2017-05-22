using System;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Pointel.HTML.Text.Editor.Settings;

namespace Pointel.Windows.Views.Common.Editor.Controls
{
    /// <summary>
    /// Interaction logic for TablePicker.xaml
    /// </summary>
    public partial class TablePicker : UserControl
    {
        private EditorDataContext editorDataContext;
        public delegate void PassTableToRTB(Table table);
        public event PassTableToRTB TableSelected;

        public TablePicker(EditorDataContext _editorDataContext)
        {
            InitializeComponent();
            editorDataContext = _editorDataContext;
            cbBGColor.SelectedItem = "White";
            cbBorder.SelectedItem = "Black";
            AlignmentSelection.SelectedIndex = 2;
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            var color = (Color)ColorConverter.ConvertFromString(cbBorder.SelectedValue.ToString());
            var bgcolor = (Color)ColorConverter.ConvertFromString(cbBGColor.SelectedValue.ToString());
            var brush = new SolidColorBrush(color);
            var backBrush = new SolidColorBrush(bgcolor);
            var alignment = AlignmentSelection.SelectedValue.ToString();
            Table table = new Table();
            int numberOfColumns = 0;
            int numberOfRows = 0;
            TableRowGroup tableRowGroup = new TableRowGroup();
            if (!string.IsNullOrEmpty(txtSpacing.Text))
                table.CellSpacing = Convert.ToDouble(txtSpacing.Text);
            if (!string.IsNullOrEmpty(txtPadding.Text))
                table.Padding = new Thickness(Convert.ToDouble(txtPadding.Text));
            if (!string.IsNullOrEmpty(txtColumns.Text))
                numberOfColumns = Convert.ToInt32(txtColumns.Text);
            if (!string.IsNullOrEmpty(txtColumns.Text))
                numberOfRows = Convert.ToInt32(txtRows.Text);
            for (int y = 0; y < numberOfRows; y++)
            {
                TableRow row = new TableRow();
                for (int x = 0; x < numberOfColumns; x++)
                {
                    TableCell cell = new TableCell();
                    cell.BorderBrush = brush;
                    cell.BorderThickness = new Thickness(0.5);
                    row.Cells.Add(cell);
                    row.Background = backBrush;
                }
                tableRowGroup.Rows.Add(row);
            }
            table.RowGroups.Add(tableRowGroup);
            table.BorderBrush = brush;
            table.BorderThickness = new Thickness(1);
            table.Background = backBrush;
            switch (alignment)
            {
                case "Right":
                    table.TextAlignment = TextAlignment.Right;
                    break;
                case "Left":
                    table.TextAlignment = TextAlignment.Left;
                    break;
                case "Center":
                    table.TextAlignment = TextAlignment.Center;
                    break;
                case "Justify":
                    table.TextAlignment = TextAlignment.Justify;
                    break;
            }
            TableSelected.Invoke(table);
            editorDataContext.contextMenuUC.StaysOpen = false;
            editorDataContext.contextMenuUC.IsOpen = false;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editorDataContext.contextMenuUC.StaysOpen = false;
            editorDataContext.contextMenuUC.IsOpen = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            ArrayList ColorList = new ArrayList();
            Type colorType = typeof(System.Windows.Media.Brushes);
            PropertyInfo[] propInfoList = colorType.GetProperties(BindingFlags.Static | BindingFlags.DeclaredOnly | BindingFlags.Public);
            foreach (PropertyInfo c in propInfoList)
            {
                this.cbBorder.Items.Add(c.Name);
                this.cbBGColor.Items.Add(c.Name);
            }
        }

        private void cbBorder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = sender as ComboBox;
            var color = (Color)ColorConverter.ConvertFromString(selectedItem.SelectedValue.ToString());
            var brush = new SolidColorBrush(color);
            showBorderColor.Fill = brush;
        }

        private void cbBGColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = sender as ComboBox;
            var color = (Color)ColorConverter.ConvertFromString(selectedItem.SelectedValue.ToString());
            var brush = new SolidColorBrush(color);
            showBGColor.Fill = brush;
        }
    }
}
