namespace Pointel.Tools
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Interop;
    using System.Windows.Media;
    using System.Windows.Media.Effects;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;

    public class WindowResizer
    {
        #region Fields

        private Dictionary<UIElement, short> downElements = new Dictionary<UIElement, short>();
        private Dictionary<UIElement, short> leftElements = new Dictionary<UIElement, short>();
        private bool resizeDown = false;
        private bool resizeLeft = false;
        private PointAPI resizePoint = new PointAPI();
        private bool resizeRight = false;
        private Size resizeSize = new Size();
        private bool resizeUp = false;
        private Point resizeWindowPoint = new Point();
        private Dictionary<UIElement, short> rightElements = new Dictionary<UIElement, short>();
        private Window target = null;
        private Dictionary<UIElement, short> upElements = new Dictionary<UIElement, short>();

        #endregion Fields

        #region Constructors

        public WindowResizer(Window target)
        {
            this.target = target;

            if (target == null)
            {
                throw new Exception("Invalid Window handle");
            }
        }

        #endregion Constructors

        #region Delegates

        private delegate void RefreshDelegate();

        #endregion Delegates

        #region Methods

        public void addResizerDown(UIElement element)
        {
            connectMouseHandlers(element);
            downElements.Add(element, 0);
        }

        public void addResizerLeft(UIElement element)
        {
            connectMouseHandlers(element);
            leftElements.Add(element, 0);
        }

        public void addResizerLeftDown(UIElement element)
        {
            connectMouseHandlers(element);
            leftElements.Add(element, 0);
            downElements.Add(element, 0);
        }

        public void addResizerLeftUp(UIElement element)
        {
            connectMouseHandlers(element);
            leftElements.Add(element, 0);
            upElements.Add(element, 0);
        }

        public void addResizerRight(UIElement element)
        {
            connectMouseHandlers(element);
            rightElements.Add(element, 0);
        }

        public void addResizerRightDown(UIElement element)
        {
            connectMouseHandlers(element);
            rightElements.Add(element, 0);
            downElements.Add(element, 0);
        }

        public void addResizerRightUp(UIElement element)
        {
            connectMouseHandlers(element);
            rightElements.Add(element, 0);
            upElements.Add(element, 0);
        }

        public void addResizerUp(UIElement element)
        {
            connectMouseHandlers(element);
            upElements.Add(element, 0);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out PointAPI lpPoint);

        private void connectMouseHandlers(UIElement element)
        {
            element.MouseLeftButtonDown += new MouseButtonEventHandler(element_MouseLeftButtonDown);
            element.MouseEnter += new MouseEventHandler(element_MouseEnter);
            element.MouseLeave += new MouseEventHandler(setArrowCursor);
        }

        private void element_MouseEnter(object sender, MouseEventArgs e)
        {
            bool resizeRight = false;
            bool resizeLeft = false;
            bool resizeUp = false;
            bool resizeDown = false;

            UIElement sourceSender = (UIElement)sender;
            if (leftElements.ContainsKey(sourceSender))
            {
                resizeLeft = true;
            }
            if (rightElements.ContainsKey(sourceSender))
            {
                resizeRight = true;
            }
            if (upElements.ContainsKey(sourceSender))
            {
                resizeUp = true;
            }
            if (downElements.ContainsKey(sourceSender))
            {
                resizeDown = true;
            }

            if ((resizeLeft && resizeDown) || (resizeRight && resizeUp))
            {
                setNESWCursor(sender, e);
            }
            else if ((resizeRight && resizeDown) || (resizeLeft && resizeUp))
            {
                setNWSECursor(sender, e);
            }
            else if (resizeLeft || resizeRight)
            {
                setWECursor(sender, e);
            }
            else if (resizeUp || resizeDown)
            {
                setNSCursor(sender, e);
            }
        }

        private void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            GetCursorPos(out resizePoint);
            resizeSize = new Size(target.Width, target.Height);
            resizeWindowPoint = new Point(target.Left, target.Top);

            #region updateResizeDirection
            resizeLeft = resizeRight = resizeUp = resizeDown = false;
            UIElement sourceSender = (UIElement)sender;
            if (leftElements.ContainsKey(sourceSender))
            {
                resizeLeft = true;
            }
            if (rightElements.ContainsKey(sourceSender))
            {
                resizeRight = true;
            }
            if (upElements.ContainsKey(sourceSender))
            {
                resizeUp = true;
            }
            if (downElements.ContainsKey(sourceSender))
            {
                resizeDown = true;
            }
            #endregion

            Thread t = new Thread(new ThreadStart(updateSizeLoop));
            t.Name = "Mouse Position Poll Thread";
            t.Start();
        }

        private void setArrowCursor(object sender, MouseEventArgs e)
        {
            if (!resizeDown && !resizeLeft && !resizeRight && !resizeUp)
            {
                target.Cursor = Cursors.Arrow;
            }
        }

        private void setArrowCursor()
        {
            target.Cursor = Cursors.Arrow;
        }

        private void setNESWCursor(object sender, MouseEventArgs e)
        {
            target.Cursor = Cursors.SizeNESW;
        }

        private void setNSCursor(object sender, MouseEventArgs e)
        {
            target.Cursor = Cursors.SizeNS;
        }

        private void setNWSECursor(object sender, MouseEventArgs e)
        {
            target.Cursor = Cursors.SizeNWSE;
        }

        private void setWECursor(object sender, MouseEventArgs e)
        {
            target.Cursor = Cursors.SizeWE;
        }

        private void updateMouseDown()
        {
            if (Mouse.LeftButton == MouseButtonState.Released)
            //if (!_isdown)
            {
                resizeRight = false;
                resizeLeft = false;
                resizeUp = false;
                resizeDown = false;
            }
        }

        private void updateSize()
        {
            PointAPI p = new PointAPI();
            GetCursorPos(out p);

            if (resizeRight)
            {
                double width = this.resizeSize.Width - (resizePoint.X - p.X);
                if (target.MinWidth != 0)
                {
                    if (width > 0 && (target.MinWidth + 1) <= width)
                        target.Width = width;
                }
                else if (width > 0)
                    target.Width = width;
            }

            if (resizeDown)
            {
                double height = resizeSize.Height - (resizePoint.Y - p.Y);

                if (target.MinHeight != 0)
                {
                    if (height > 0 && (target.MinHeight + 1) <= height)
                        target.Height = height;
                }
                else if (height > 0)
                    target.Height = height;
            }

            if (resizeLeft)
            {
                target.Width = resizeSize.Width + (resizePoint.X - p.X);
                double left = resizeWindowPoint.X - (resizePoint.X - p.X);
                if (left > 0)
                    target.Left = left;
            }

            if (resizeUp)
            {

                target.Height = resizeSize.Height + (resizePoint.Y - p.Y);
                double top = resizeWindowPoint.Y - (resizePoint.Y - p.Y);
                if (top > 0)
                    target.Top = top;
            }
        }

        private void updateSizeLoop()
        {
            try
            {
                while (resizeDown || resizeLeft || resizeRight || resizeUp)
                {
                    target.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, new RefreshDelegate(updateSize));
                    target.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, new RefreshDelegate(updateMouseDown));
                    Thread.Sleep(10);
                }

                target.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Render, new RefreshDelegate(setArrowCursor));
            }
            catch (Exception)
            {
            }
        }

        #endregion Methods

        #region Nested Types

        private struct PointAPI
        {
            #region Fields

            public int X;
            public int Y;

            #endregion Fields
        }

        #endregion Nested Types
    }
}