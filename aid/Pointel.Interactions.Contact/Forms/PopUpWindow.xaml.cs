using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using System.Windows.Media.Effects;
using System.Windows.Interop;
using System.Runtime.InteropServices;
namespace Pointel.Interactions.Contact.Forms
{
    /// <summary>
    /// Interaction logic for PopUpWindow.xaml
    /// </summary>
    public partial class PopUpWindow : Window
    {
        #region Fields

        private DropShadowBitmapEffect _shadowEffect = new DropShadowBitmapEffect();
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
            "AID");
        private const int GWL_STYLE = -16; //WPF's Message code for Title Bar's Style 
        private const int WS_SYSMENU = 0x80000; //WPF's Message code for System Menu
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        #endregion Fields
        public string Message
        {
            get;
            set;
        }
        public static PopUpWindow _frmMessage = null;

        public static PopUpWindow GetInstance()
        {
            if (_frmMessage == null)
            {
                _frmMessage = new PopUpWindow();
                return _frmMessage;
            }
            return _frmMessage;
        }
        #region PopUpWindow
        public PopUpWindow()
        {
            
            InitializeComponent();
            _shadowEffect.ShadowDepth = 0;
            _shadowEffect.Opacity = 0.5;
            _shadowEffect.Softness = 0.5;
            _shadowEffect.Color = (Color)ColorConverter.ConvertFromString("#003660");
        }
        #endregion

        #region Window_Loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);
            this.Topmost = true;
            lblAlert.Text = Message;
        }
        #endregion

        #region Window_Activated
        private void Window_Activated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = new SolidColorBrush((Color)(ColorConverter.ConvertFromString("#0070C5")));
            MainBorder.BitmapEffect = _shadowEffect;
        }
        #endregion

        #region Window_Deactivated
        private void Window_Deactivated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = Brushes.Black;
            MainBorder.BitmapEffect = null;
        }
        #endregion

        #region btnOK_Click
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
        #endregion

        #region btnCancel_Click
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //if (emailSettings.popupWindow != null)
                //{
                //    emailSettings.popupWindow = null;
                //    _frmMessage = null;
                //}
                this.DialogResult = false;
            }
            catch (Exception generalException)
            {
                logger.Error("btnOK_Click:" + generalException.ToString());
            }
        }
        #endregion

        #region Window_Closing
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                //if (emailSettings.popupWindow != null)
                //{
                //    emailSettings.popupWindow = null;
                //}

                _frmMessage = null;
            }
            catch (Exception ex)
            {
                logger.Error("Window_Closing:" + ex.ToString());
            }
        }
        #endregion

        #region Label_MouseLeftButtonDown
        private void Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.DragMove();
                if (this.Left < 0)
                    this.Left = 0;
                if (this.Top < 0)
                    this.Top = 0;
                if (this.Left > System.Windows.SystemParameters.WorkArea.Right - this.Width)
                    this.Left = System.Windows.SystemParameters.WorkArea.Right - this.Width;
                if (this.Top > System.Windows.SystemParameters.WorkArea.Bottom - this.Height)
                    this.Top = System.Windows.SystemParameters.WorkArea.Bottom - this.Height;
            }
            catch { }
        }
        #endregion

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                case Key.N:
                    DialogResult = false;
                    break;
                case Key.Y:
                    DialogResult = true;
                    break;
            }
                
        }
    }
}
