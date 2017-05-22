using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Pointel.HTML.Text.Editor.Settings;

namespace Pointel.Windows.Views.Common.Editor.Controls
{
    /// <summary>
    /// Interaction logic for ImagePicker.xaml
    /// </summary>
    public partial class ImagePicker : UserControl
    {
        private EditorDataContext editorDataContext;
        public delegate void PassImageToRTB(string filePath);
        public event PassImageToRTB ImageSelected;
        public ImagePicker(EditorDataContext _editorDataContext)
        {
            InitializeComponent();
            editorDataContext = _editorDataContext;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (System.Windows.Forms.OpenFileDialog dialog =
                 new System.Windows.Forms.OpenFileDialog())
            {
                dialog.Filter = "Image files (*.bmp, *.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.bmp; *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
                dialog.FilterIndex = 0;
                if (System.Windows.Forms.DialogResult.OK == dialog.ShowDialog())
                {
                    var onlyFileName = System.IO.Path.GetFileName(dialog.FileName);
                    UrlText.Text = dialog.FileName;
                    LoadImage(dialog.FileName);
                    editorDataContext.contextMenuUC.StaysOpen = true;
                    editorDataContext.contextMenuUC.IsOpen = true;
                    ResizeSlider.Value = 0;
                }
            }
        }
        private void LoadImage(string uri)
        {
            BitmapImage img = null;
            try
            {
                StatusPrompt.Content = "";
                PreviewImage.Source = null;
                // bindingContext.Image = null;

                // 加载图像
                if (uri != null)
                {
                    //Regex validator = new Regex("^(https?|ftp|file)://[-a-zA-Z0-9+&@#/%?=~_|!:,.;]*[-a-zA-Z0-9+&@#/%=~_|]");
                    //if (validator.IsMatch(uri))
                    {
                        Uri u = new Uri(uri, UriKind.RelativeOrAbsolute);
                        img = new BitmapImage(u);
                        PreviewImage.Source = img;

                        // 更新绑定上下文
                        //bindingContext.ImageUrl = u.ToString();
                        //bindingContext.Image = img;
                        //bindingContext.OriginalWidth = img.PixelWidth;
                        //bindingContext.OriginalHeight = img.PixelHeight;


                        ScrollToCenter();
                        using (FileStream fileStream = new FileStream(uri, FileMode.Open, FileAccess.Read))
                        {
                            BitmapFrame frame = BitmapFrame.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                            txtWidth.Tag = txtWidth.Text = frame.PixelWidth.ToString();
                            txtHeight.Tag = txtHeight.Text = frame.PixelHeight.ToString();
                            PreviewImage.Width = frame.PixelWidth;
                            PreviewImage.Height = frame.PixelHeight;
                            Size s = new Size(frame.PixelWidth, frame.PixelHeight);
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
                img = null;
            }
        }
        private void ScrollToCenter()
        {
            if (PreviewImage.Width > PreviewScroll.ViewportWidth)
            {
                PreviewScroll.ScrollToHorizontalOffset((PreviewImage.Width - PreviewScroll.ViewportWidth) / 2);
            }

            if (PreviewImage.Height > PreviewScroll.ViewportHeight)
            {
                PreviewScroll.ScrollToVerticalOffset((PreviewImage.Height - PreviewScroll.ViewportHeight) / 2);
            }
        }

        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            double val = ResizeSlider.Value + 10;
            if (val > 100) val = 100;
            ResizeSlider.Value = val;
        }

        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            double val = ResizeSlider.Value - 10;
            if (val < -100) val = -100;
            ResizeSlider.Value = val;
        }

        private void ResizeSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.NewValue == 0)
            {
                PreviewImage.Width = Convert.ToDouble(txtWidth.Tag.ToString());
                PreviewImage.Height = Convert.ToDouble(txtHeight.Tag.ToString());
            }
            else
            {
                double percentageWidth = Math.Round(Convert.ToDouble(txtWidth.Tag.ToString()) * (Math.Abs(e.NewValue - e.OldValue) / 100));
                double percentageHeight = Math.Round(Convert.ToDouble(txtHeight.Tag.ToString()) * (Math.Abs(e.NewValue - e.OldValue) / 100));
                if (e.NewValue < e.OldValue)
                {
                    double tempwidth = PreviewImage.Width - percentageWidth;
                    double tempHeight = PreviewImage.Height - percentageWidth;
                    if (tempwidth < 0 || tempHeight < 0) return;
                    PreviewImage.Width -= percentageWidth;
                    PreviewImage.Height -= percentageHeight;
                }
                else
                {
                    PreviewImage.Width += percentageWidth;
                    PreviewImage.Height += percentageHeight;
                }
                txtHeight.Text = PreviewImage.Height.ToString();
                txtWidth.Text = PreviewImage.Width.ToString();
            }
            #region Old code
            //try
            //{
            //    if (e.NewValue == 0)
            //    {
            //        PreviewImage.Width = Convert.ToDouble(txtWidth.Tag.ToString());
            //        PreviewImage.Height = Convert.ToDouble(txtHeight.Tag.ToString());
            //    }
            //    else
            //    {
            //        double percentage = Math.Abs(e.NewValue) / 100;
            //        int _width = Convert.ToInt32(Math.Round(percentage * Convert.ToDouble(txtWidth.Tag.ToString())));
            //        int _height = Convert.ToInt32(Math.Round(percentage * Convert.ToDouble(txtHeight.Tag.ToString())));
            //        if (_width > 0 && _height > 0)
            //        {
            //            if (e.NewValue < 0)
            //            {
            //                if (e.NewValue < e.OldValue)
            //                {
            //                    PreviewImage.Width = Math.Round(Convert.ToDouble(txtWidth.Tag.ToString()) - _width);
            //                    PreviewImage.Height = Math.Round(Convert.ToDouble(txtHeight.Tag.ToString()) - _height);
            //                }
            //                else
            //                {
            //                    PreviewImage.Width = Math.Round(Convert.ToDouble(txtWidth.Tag.ToString()) + _width);
            //                    PreviewImage.Height = Math.Round(Convert.ToDouble(txtHeight.Tag.ToString()) + _height);
            //                }
            //            }
            //            else
            //            {

            //                if (e.NewValue < e.OldValue)
            //                {
            //                    PreviewImage.Width = Math.Round(PreviewImage.Width - _width);
            //                    PreviewImage.Height = Math.Round(PreviewImage.Height - _height);
            //                }
            //                else
            //                {
            //                    PreviewImage.Width = Math.Round(Convert.ToDouble(txtWidth.Tag.ToString()) + _width);
            //                    PreviewImage.Height = Math.Round(Convert.ToDouble(txtHeight.Tag.ToString()) + _height);
            //                }
            //            }
            //        }
            //    }
            //    txtHeight.Text = PreviewImage.Height.ToString() + ", " + e.OldValue.ToString();
            //    txtWidth.Text = PreviewImage.Width.ToString() + ", " + e.NewValue.ToString();
            //}
            //catch
            //{

            //}
            #endregion Old code
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            editorDataContext.contextMenuUC.StaysOpen = false;
            editorDataContext.contextMenuUC.IsOpen = false;
        }

        private void OkayButton_Click(object sender, RoutedEventArgs e)
        {
            if (PreviewImage != null)
            {
                if (!string.IsNullOrEmpty(UrlText.Text))
                    ImageSelected.Invoke(UrlText.Text.ToString());
                editorDataContext.contextMenuUC.StaysOpen = false;
                editorDataContext.contextMenuUC.IsOpen = false;
            }
        }
    }
}
