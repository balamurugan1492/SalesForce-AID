using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Pointel.Windows.Views.Common.Editor.CustomControls
{
    public class ImageColorPicker : Image
    {
        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ImageColorPicker), new FrameworkPropertyMetadata(Colors.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty SelectorProperty = DependencyProperty.Register("Selector", typeof(Drawing), typeof(ImageColorPicker), new FrameworkPropertyMetadata(new GeometryDrawing(Brushes.White, new Pen(Brushes.Black, 1.0), new EllipseGeometry(default(Point), 5.0, 5.0)), FrameworkPropertyMetadataOptions.AffectsRender), new ValidateValueCallback(ImageColorPicker.ValidateSelector));
        private Point position = default(Point);
        private RenderTargetBitmap cachedTargetBitmap;
        public Color SelectedColor
        {
            get
            {
                return (Color)base.GetValue(ImageColorPicker.SelectedColorProperty);
            }
            set
            {
                base.SetValue(ImageColorPicker.SelectedColorProperty, value);
            }
        }
        public Drawing Selector
        {
            get
            {
                return (Drawing)base.GetValue(ImageColorPicker.SelectorProperty);
            }
            set
            {
                base.SetValue(ImageColorPicker.SelectorProperty, value);
            }
        }
        private Point Position
        {
            get
            {
                return this.position;
            }
            set
            {
                Point point = this.RestrictedPosition(value);
                if (this.position != point)
                {
                    this.position = point;
                    Color color = this.PickColor(this.position.X, this.position.Y);
                    if (color == this.SelectedColor)
                    {
                        base.InvalidateVisual();
                    }
                    this.SelectedColor = color;
                }
            }
        }
        private RenderTargetBitmap TargetBitmap
        {
            get
            {
                if (this.cachedTargetBitmap == null)
                {
                    DrawingImage drawingImage = base.Source as DrawingImage;
                    if (drawingImage != null)
                    {
                        DrawingVisual drawingVisual = new DrawingVisual();
                        using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                        {
                            drawingContext.DrawDrawing(drawingImage.Drawing);
                        }
                        Rect contentBounds = drawingVisual.ContentBounds;
                        drawingVisual.Transform = new ScaleTransform(base.ActualWidth / contentBounds.Width, base.ActualHeight / contentBounds.Height);
                        this.cachedTargetBitmap = new RenderTargetBitmap((int)base.ActualWidth, (int)base.ActualHeight, 96.0, 96.0, PixelFormats.Pbgra32);
                        this.cachedTargetBitmap.Render(drawingVisual);
                    }
                }
                return this.cachedTargetBitmap;
            }
        }
        private static bool ValidateSelector(object value)
        {
            return value != null;
        }
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (base.ActualWidth == 0.0 || base.ActualHeight == 0.0)
            {
                return;
            }
            dc.PushTransform(new TranslateTransform(this.Position.X, this.Position.Y));
            dc.DrawDrawing(this.Selector);
            dc.Pop();
        }
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            this.cachedTargetBitmap = null;
            if (sizeInfo.PreviousSize.Width > 0.0 && sizeInfo.PreviousSize.Height > 0.0)
            {
                this.Position = new Point(this.Position.X * sizeInfo.NewSize.Width / sizeInfo.PreviousSize.Width, this.Position.Y * sizeInfo.NewSize.Height / sizeInfo.PreviousSize.Height);
            }
        }
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (e.Property.Name == "Source")
            {
                this.cachedTargetBitmap = null;
                if (base.Source != null)
                {
                    Point arg_2D_0 = this.Position;
                }
                return;
            }
            base.OnPropertyChanged(e);
        }
        private Point RestrictedPosition(Point point)
        {
            double num = point.X;
            double num2 = point.Y;
            if (num < 0.0)
            {
                num = 0.0;
            }
            else
            {
                if (num > base.ActualWidth)
                {
                    num = base.ActualWidth;
                }
            }
            if (num2 < 0.0)
            {
                num2 = 0.0;
            }
            else
            {
                if (num2 > base.ActualHeight)
                {
                    num2 = base.ActualHeight;
                }
            }
            return new Point(num, num2);
        }
        private void SetPositionIfInBounds(Point pt)
        {
            if (pt.X >= 0.0 && pt.X <= base.ActualWidth && pt.Y >= 0.0 && pt.Y <= base.ActualHeight)
            {
                this.Position = pt;
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.SetPositionIfInBounds(e.GetPosition(this));
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            this.SetPositionIfInBounds(e.GetPosition(this));
        }
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.SetPositionIfInBounds(e.GetPosition(this));
            }
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Point point = e.GetPosition(this);
                this.Position = new Point(point.X, point.Y);
            }
        }
        private Color PickColor(double x, double y)
        {
            if (base.Source == null)
            {
                throw new InvalidOperationException("Image Source not set");
            }
            BitmapSource bitmapSource = base.Source as BitmapSource;
            if (bitmapSource != null)
            {
                x *= (double)bitmapSource.PixelWidth / base.ActualWidth;
                if ((int)x > bitmapSource.PixelWidth - 1)
                {
                    x = (double)(bitmapSource.PixelWidth - 1);
                }
                else
                {
                    if (x < 0.0)
                    {
                        x = 0.0;
                    }
                }
                y *= (double)bitmapSource.PixelHeight / base.ActualHeight;
                if ((int)y > bitmapSource.PixelHeight - 1)
                {
                    y = (double)(bitmapSource.PixelHeight - 1);
                }
                else
                {
                    if (y < 0.0)
                    {
                        y = 0.0;
                    }
                }
                if (bitmapSource.Format == PixelFormats.Indexed4)
                {
                    byte[] array = new byte[1];
                    int stride = (bitmapSource.PixelWidth * bitmapSource.Format.BitsPerPixel + 3) / 4;
                    bitmapSource.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), array, stride, 0);
                    return bitmapSource.Palette.Colors[array[0] >> 4];
                }
                if (bitmapSource.Format == PixelFormats.Indexed8)
                {
                    byte[] array2 = new byte[1];
                    int stride2 = (bitmapSource.PixelWidth * bitmapSource.Format.BitsPerPixel + 7) / 8;
                    bitmapSource.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), array2, stride2, 0);
                    return bitmapSource.Palette.Colors[(int)array2[0]];
                }
                byte[] array3 = new byte[4];
                int stride3 = (bitmapSource.PixelWidth * bitmapSource.Format.BitsPerPixel + 7) / 8;
                bitmapSource.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), array3, stride3, 0);
                return Color.FromArgb(array3[3], array3[2], array3[1], array3[0]);
            }
            else
            {
                DrawingImage drawingImage = base.Source as DrawingImage;
                if (drawingImage != null)
                {
                    RenderTargetBitmap targetBitmap = this.TargetBitmap;
                    x *= (double)targetBitmap.PixelWidth / base.ActualWidth;
                    if ((int)x > targetBitmap.PixelWidth - 1)
                    {
                        x = (double)(targetBitmap.PixelWidth - 1);
                    }
                    else
                    {
                        if (x < 0.0)
                        {
                            x = 0.0;
                        }
                    }
                    y *= (double)targetBitmap.PixelHeight / base.ActualHeight;
                    if ((int)y > targetBitmap.PixelHeight - 1)
                    {
                        y = (double)(targetBitmap.PixelHeight - 1);
                    }
                    else
                    {
                        if (y < 0.0)
                        {
                            y = 0.0;
                        }
                    }
                    byte[] array4 = new byte[4];
                    int stride4 = (targetBitmap.PixelWidth * targetBitmap.Format.BitsPerPixel + 7) / 8;
                    targetBitmap.CopyPixels(new Int32Rect((int)x, (int)y, 1, 1), array4, stride4, 0);
                    return Color.FromArgb(array4[3], array4[2], array4[1], array4[0]);
                }
                throw new InvalidOperationException("Unsupported Image Source Type");
            }
        }
    }
}
