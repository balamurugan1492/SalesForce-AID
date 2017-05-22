using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Pointel.Interactions.Outbound.CustomControls
{
    class TimerLabel : Label
    {
        #region Fields

        // Initialize dependency properties
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(TimerLabel), new UIPropertyMetadata(null));

        public static DispatcherTimer t;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TimerLabel()
        {
            // Initialize as look less control
            try
            {
                //DefaultStyleKeyProperty.OverrideMetadata(typeof(TimerLabel), new FrameworkPropertyMetadata(typeof(TimerLabel)));
            }
            catch { }
            t = new DispatcherTimer();
            t.Interval = new TimeSpan(0, 0, 1);
            t.Tick += new System.EventHandler(t_Tick);
            t.Start();
        }

        #endregion Constructors

        #region Destructor

        /// <summary>
        /// Finalizes an instance of the <see cref="TimerLabel"/> class.
        /// </summary>
        ~TimerLabel()
        {
            if (t != null)
            {
                if (t.IsEnabled)
                    t.Stop();
                t = null;
            }
        }

        #endregion Distructor

        #region General Function

        public void t_Tick(object sender, System.EventArgs e)
        {
            Application.Current.Dispatcher.Invoke((Action)(delegate
            {
                int duration;
                int Num;
                bool isNum = int.TryParse(Text, out Num);

                if (isNum)
                {
                    TimeSpan time = TimeSpan.FromSeconds(Convert.ToInt32(Text) + 1);
                    Text = "[" + string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds) + "]";
                }
                else
                {
                    if (Text.Contains(":"))
                    {
                        string temp = Text.Replace('[', ' ').TrimStart();
                        temp = temp.Replace(']', ' ').TrimEnd();
                        string[] d = temp.Split(':');
                        duration = Convert.ToInt32(d[0]) * 3600 + Convert.ToInt32(d[1]) * 60 + Convert.ToInt32(d[2]);
                        TimeSpan time = TimeSpan.FromSeconds(duration + 1);
                        Text = "[" + string.Format("{0:D2}:{1:D2}:{2:D2}", time.Hours, time.Minutes, time.Seconds) + "]";
                    }
                }
            }));
        }

        public void Stop_CustomTimer()
        {
            if (t != null)
            {
                if (t.IsEnabled)
                    t.Stop();
                t = null;
            }
        }

        #endregion General Function

        #region Custom Control Properties

        /// <summary>
        /// Get's or set's the Content of the Agent.Interaction.Desktop.CustomControls.TimerLabel.
        /// </summary>
        [Description("Get's or set's the Content of the Pointel.Interactions.Chat.CustomControls.TimerLabel"), Category("Common Properties")]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        #endregion Custom Control Properties
    }
}
