using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Windows.Threading;

namespace Pointel.Statistics.TaskbarNotifier
{
    [ContentProperty("Text")]
    [DefaultEvent("MouseDoubleClick")]
    public class NotifyIcon : FrameworkElement, IAddChild
    {
        #region Events

        public static readonly RoutedEvent MouseDownEvent = EventManager.RegisterRoutedEvent(
            "MouseDown", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

        public static readonly RoutedEvent MouseUpEvent = EventManager.RegisterRoutedEvent(
            "MouseUp", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

        public static readonly RoutedEvent MouseClickEvent = EventManager.RegisterRoutedEvent(
            "MouseClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

        public static readonly RoutedEvent MouseDoubleClickEvent = EventManager.RegisterRoutedEvent(
            "MouseDoubleClick", RoutingStrategy.Bubble, typeof(MouseButtonEventHandler), typeof(NotifyIcon));

        #endregion

        #region Dependency properties

        public static readonly DependencyProperty BalloonTipIconProperty =
            DependencyProperty.Register("BalloonTipIcon", typeof(BalloonTipIcon), typeof(NotifyIcon));

        public static readonly DependencyProperty BalloonTipTextProperty =
            DependencyProperty.Register("BalloonTipText", typeof(string), typeof(NotifyIcon));

        public static readonly DependencyProperty BalloonTipTitleProperty =
            DependencyProperty.Register("BalloonTipTitle", typeof(string), typeof(NotifyIcon));

        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(NotifyIcon));

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(NotifyIcon));

        #endregion

        System.Windows.Forms.NotifyIcon notifyIcon;
        bool initialized;

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            //InitializeNotifyIcon();
            Dispatcher.ShutdownStarted += OnDispatcherShutdownStarted;
        }

        private void OnDispatcherShutdownStarted(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
        }

        private void InitializeNotifyIcon()
        {
            notifyIcon = new System.Windows.Forms.NotifyIcon();
            notifyIcon.Text = Text;
            notifyIcon.Icon = FromImageSource(Icon);
            notifyIcon.Visible = FromVisibility(Visibility);

            notifyIcon.MouseDown += OnMouseDown;
            notifyIcon.MouseUp += OnMouseUp;
            notifyIcon.MouseClick += OnMouseClick;
            notifyIcon.MouseDoubleClick += OnMouseDoubleClick;

            initialized = true;
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnRaiseEvent(MouseDownEvent, new MouseButtonEventArgs(
                InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
        }

        private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                ShowContextMenu();
            }
            OnRaiseEvent(MouseUpEvent, new MouseButtonEventArgs(
                InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
        }

        private void ShowContextMenu()
        {
            if (ContextMenu != null)
            {
                ContextMenuService.SetPlacement(ContextMenu, PlacementMode.MousePoint);
                ContextMenu.IsOpen = true;
            }
        }

        private void OnMouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnRaiseEvent(MouseDoubleClickEvent, new MouseButtonEventArgs(
                InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
        }

        private void OnMouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            OnRaiseEvent(MouseClickEvent, new MouseButtonEventArgs(
                InputManager.Current.PrimaryMouseDevice, 0, ToMouseButton(e.Button)));
        }

        private void OnRaiseEvent(RoutedEvent handler, MouseButtonEventArgs e)
        {
            e.RoutedEvent = handler;
            RaiseEvent(e);
        }

        public BalloonTipIcon BalloonTipIcon
        {
            get { return (BalloonTipIcon)GetValue(BalloonTipIconProperty); }
            set { SetValue(BalloonTipIconProperty, value); }
        }

        public string BalloonTipText
        {
            get { return (string)GetValue(BalloonTipTextProperty); }
            set { SetValue(BalloonTipTextProperty, value); }
        }

        public string BalloonTipTitle
        {
            get { return (string)GetValue(BalloonTipTitleProperty); }
            set { SetValue(BalloonTipTitleProperty, value); }
        }

        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (initialized)
            {
                switch (e.Property.Name)
                {
                    case "Icon":
                        notifyIcon.Icon = FromImageSource(Icon);
                        break;
                    case "Text":
                        notifyIcon.Text = Text;
                        break;
                    case "Visibility":
                        notifyIcon.Visible = FromVisibility(Visibility);
                        break;
                }
            }
        }

        public void ShowBalloonTip(int timeout)
        {
            notifyIcon.BalloonTipTitle = BalloonTipTitle;
            notifyIcon.BalloonTipText = BalloonTipText;
            notifyIcon.BalloonTipIcon = (System.Windows.Forms.ToolTipIcon)BalloonTipIcon;
            notifyIcon.ShowBalloonTip(timeout);
        }

        public void ShowBalloonTip(int timeout, string tipTitle, string tipText, BalloonTipIcon tipIcon)
        {
            notifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, (System.Windows.Forms.ToolTipIcon)tipIcon);
        }

        public event MouseButtonEventHandler MouseClick
        {
            add { AddHandler(MouseClickEvent, value); }
            remove { RemoveHandler(MouseClickEvent, value); }
        }

        public event MouseButtonEventHandler MouseDoubleClick
        {
            add { AddHandler(MouseDoubleClickEvent, value); }
            remove { RemoveHandler(MouseDoubleClickEvent, value); }
        }

        public event MouseButtonEventHandler MouseDown
        {
            add { AddHandler(MouseDownEvent, value); }
            remove { RemoveHandler(MouseDownEvent, value); }
        }

        public event MouseButtonEventHandler MouseUp
        {
            add { AddHandler(MouseUpEvent, value); }
            remove { RemoveHandler(MouseUpEvent, value); }
        }

        #region IAddChild Members

        void IAddChild.AddChild(object value)
        {
            throw new InvalidOperationException();
        }

        void IAddChild.AddText(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            Text = text;
        }

        #endregion

        #region Conversion members

        private static System.Drawing.Icon FromImageSource(ImageSource icon)
        {
            if (icon == null)
            {
                return null;
            }
            Uri iconUri = new Uri(icon.ToString());
            return new System.Drawing.Icon(System.Windows.Application.GetResourceStream(iconUri).Stream);
        }

        private static bool FromVisibility(Visibility visibility)
        {
            return visibility == Visibility.Visible;
        }

        private MouseButton ToMouseButton(System.Windows.Forms.MouseButtons button)
        {
            switch (button)
            {
                case System.Windows.Forms.MouseButtons.Left:
                    return MouseButton.Left;
                case System.Windows.Forms.MouseButtons.Right:
                    return MouseButton.Right;
                case System.Windows.Forms.MouseButtons.Middle:
                    return MouseButton.Middle;
                case System.Windows.Forms.MouseButtons.XButton1:
                    return MouseButton.XButton1;
                case System.Windows.Forms.MouseButtons.XButton2:
                    return MouseButton.XButton2;
            }
            throw new InvalidOperationException();
        }

        #endregion
    }
}
