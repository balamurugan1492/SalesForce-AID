using System;
using System.Windows;

namespace Pointel.Windows.Views.Common.Editor.CustomControls
{
    public class RichTextBoxNoAutoResize : RichTextBoxEx
    {
        public static readonly DependencyProperty UseAutoResizeProperty = DependencyProperty.Register("UseAutoResize", typeof(bool), typeof(RichTextBoxNoAutoResize), new UIPropertyMetadata(false));
        public bool UseAutoResize
        {
            get
            {
                return (bool)base.GetValue(RichTextBoxNoAutoResize.UseAutoResizeProperty);
            }
            set
            {
                base.SetValue(RichTextBoxNoAutoResize.UseAutoResizeProperty, value);
            }
        }
        protected override Size MeasureOverride(Size constraint)
        {

            if (this.UseAutoResize)
            {
                return base.MeasureOverride(constraint);
            }
            Size size = new Size(double.IsPositiveInfinity(constraint.Width) ? 1.0 : constraint.Width, 1.0);
            try
            {
                if (this.VisualChildrenCount > 0)
                {
                    UIElement uIElement = this.GetVisualChild(0) as UIElement;
                    if (uIElement != null)
                    {
                        uIElement.Measure(size);
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return size;
        }
    }
}
