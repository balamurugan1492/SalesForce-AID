using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Pointel.Windows.Views.Common.Editor.CustomControls
{
    public class RichTextBoxEx : RichTextBox
    {
        public static readonly DependencyProperty IsReadOnlyWithNavigationProperty = DependencyProperty.Register("IsReadOnlyWithNavigation", typeof(bool), typeof(RichTextBoxEx), new UIPropertyMetadata(false, new PropertyChangedCallback(RichTextBoxEx.IsReadOnlyWithNavigationPropertyChangedCallback)));
        public bool IsReadOnlyWithNavigation
        {
            get
            {
                return (bool)base.GetValue(RichTextBoxEx.IsReadOnlyWithNavigationProperty);
            }
            set
            {
                base.SetValue(RichTextBoxEx.IsReadOnlyWithNavigationProperty, value);
            }
        }
        public RichTextBoxEx()
        {
            base.CommandBindings.Add(new CommandBinding(ApplicationCommands.Cut, new ExecutedRoutedEventHandler(RichTextBoxEx.ExecutedCutRoutedCommand), new CanExecuteRoutedEventHandler(RichTextBoxEx.CanExecuteCutRoutedCommand)));
            base.CommandBindings.Add(new CommandBinding(ApplicationCommands.Copy, new ExecutedRoutedEventHandler(RichTextBoxEx.ExecutedCopyRoutedCommand), new CanExecuteRoutedEventHandler(RichTextBoxEx.CanExecuteCopyRoutedCommand)));
            base.CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, new ExecutedRoutedEventHandler(RichTextBoxEx.ExecutedPasteRoutedCommand), new CanExecuteRoutedEventHandler(RichTextBoxEx.CanExecutePasteRoutedCommand)));
        }
        private static void IsReadOnlyWithNavigationPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RichTextBoxEx richTextBoxEx = d as RichTextBoxEx;
            if (richTextBoxEx != null && richTextBoxEx.IsReadOnlyWithNavigation)
            {
                richTextBoxEx.IsReadOnly = false;
            }
        }
        private static void ExecutedCutRoutedCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextBoxEx richTextBoxEx = sender as RichTextBoxEx;
            if (richTextBoxEx != null)
            {
                if (!richTextBoxEx.IsReadOnly && !richTextBoxEx.IsReadOnlyWithNavigation)
                {
                    richTextBoxEx.Cut();
                }
                e.Handled = true;
            }
        }
        private static void CanExecuteCutRoutedCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            RichTextBoxEx richTextBoxEx = sender as RichTextBoxEx;
            if (richTextBoxEx != null)
            {
                if (richTextBoxEx.IsReadOnly || richTextBoxEx.IsReadOnlyWithNavigation)
                {
                    e.CanExecute = false;
                    return;
                }
                e.CanExecute = !richTextBoxEx.Selection.IsEmpty;
            }
        }
        private static void ExecutedCopyRoutedCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextBoxEx richTextBoxEx = sender as RichTextBoxEx;
            if (richTextBoxEx != null)
            {
                richTextBoxEx.Copy();
                e.Handled = true;
            }
        }
        private static void CanExecuteCopyRoutedCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            RichTextBoxEx richTextBoxEx = sender as RichTextBoxEx;
            if (richTextBoxEx != null)
            {
                if (richTextBoxEx.IsReadOnly || richTextBoxEx.IsReadOnlyWithNavigation)
                {
                    e.CanExecute = !richTextBoxEx.Selection.IsEmpty;
                    return;
                }
                e.CanExecute = !richTextBoxEx.Selection.IsEmpty;
            }
        }
        private static void ExecutedPasteRoutedCommand(object sender, ExecutedRoutedEventArgs e)
        {
            RichTextBoxEx richTextBoxEx = sender as RichTextBoxEx;
            if (richTextBoxEx != null && !richTextBoxEx.IsReadOnly)
            {
                if (richTextBoxEx.IsReadOnlyWithNavigation)
                {
                    return;
                }
                richTextBoxEx.Paste();
            }
        }
        private static void CanExecutePasteRoutedCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            RichTextBoxEx richTextBoxEx = sender as RichTextBoxEx;
            if (richTextBoxEx != null)
            {
                if (richTextBoxEx.IsReadOnly || richTextBoxEx.IsReadOnlyWithNavigation)
                {
                    e.CanExecute = false;
                    return;
                }
                e.CanExecute = !richTextBoxEx.IsReadOnly;
            }
        }
        protected override void OnDrop(DragEventArgs e)
        {
            if (!this.IsReadOnlyWithNavigation)
            {
                base.OnDrop(e);
            }
        }
        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            if (this.IsReadOnlyWithNavigation && e.SystemText != " ")
            {
                e.Handled = true;
                return;
            }
            base.OnPreviewTextInput(e);
        }
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (this.IsReadOnlyWithNavigation && e.Key == Key.Space)
            {
                e.Handled = true;
                return;
            }
            base.OnPreviewKeyDown(e);
        }
    }
}
