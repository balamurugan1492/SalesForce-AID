using System;
using System.Text.RegularExpressions;
using Pointel.Statistics.Core.Utility;
using Pointel.Logger.Core;
using Pointel.Statistics.Core;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;


namespace StatTickerFive.Helpers
{
    /// <summary>
    /// 
    /// </summary>
    class ValidationClass
    {
        #region Declarations
        string portValidation;
        private static Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        StatisticsBase stat = new StatisticsBase();
        #endregion

        /// <summary>
        /// Logins the validation.
        /// </summary>
        /// <param name="IP">The IP.</param>
        /// <param name="Port">The port.</param>
        /// <returns></returns>
        /// <exception cref="System.Exception">Please Enter Valid IP address</exception>
        public OutputValues LoginValidation(string Port, string place)
        {
            OutputValues result = new OutputValues();
            try
            {
                portValidation = Settings.portValidation;

                Regex regexPort = new Regex(portValidation);

                Match matchPort = regexPort.Match(Port.Trim());

                if (!matchPort.Success)
                {
                    throw new Exception("Please Enter Valid Port address");
                }
                result.MessageCode = "2001";
                result.Message = "Success";

            }
            catch (Exception ex)
            {
                logger.Error("ValidationClass Class : LoginValidation Method : Exception Caught " + ex.Message);
                result.Message = ex.Message;
                result.MessageCode = "2000";
            }
            finally
            {
                GC.Collect();
            }
            return result;
        }
    }

    #region CustomValidator
    class CustomValidator : Behavior<TextBox>
    {
        #region Declaration
        public string emptyField = "Enter a value here";
        public string decimalFormat = "Value in decimal format";
        public string timeFormat = "Value in Time format";
        public string ThresholdMessage2 = "Value Greater than Threshold2";
        public string ThresholdMessage1 = "Value less than Threshold2";

        private static ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "STF");
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("text", typeof(string), typeof(CustomValidator), new PropertyMetadata(string.Empty));

        static readonly DependencyPropertyKey IsWatermarkedPropertyKey = DependencyProperty.RegisterAttachedReadOnly("IsWatermarked", typeof(bool), typeof(CustomValidator), new FrameworkPropertyMetadata(false));

        public static readonly DependencyProperty IsWatermarkedProperty = IsWatermarkedPropertyKey.DependencyProperty;
        #endregion


        public static bool GetIsWatermarked(TextBox tb)
        {
            return (bool)tb.GetValue(IsWatermarkedProperty);
        }

        /// <summary>
        /// Retrieves the current watermarked state of the TextBox.
        /// </summary>
        public bool IsWatermarked
        {
            get { return GetIsWatermarked(AssociatedObject); }
            private set { AssociatedObject.SetValue(IsWatermarkedPropertyKey, value); }
        }

        /// <summary>
        /// The watermark text
        /// </summary>
        public string Text
        {
            get { return (string)base.GetValue(TextProperty); }
            set { base.SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.GotFocus += OnGotFocus;
            AssociatedObject.LostFocus += OnLostFocus;

            AssociatedObject.Loaded += OnLoaded;
            AssociatedObject.TextChanged += OnTextChanged;
            // OnLostFocus(null, null);
        }

        /// <summary>
        /// Called when the behavior is being detached from its AssociatedObject, but before it has actually occurred.
        /// </summary>
        /// <remarks>
        /// Override this to unhook functionality from the AssociatedObject.
        /// </remarks>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.GotFocus -= OnGotFocus;
            AssociatedObject.LostFocus -= OnLostFocus;
        }

        /// <summary>
        /// This method is called when the textbox gains focus. It removes the watermark.
        /// </summary>
        /// <param name=”sender”></param>
        /// <param name=”e”></param>
        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (string.Compare(AssociatedObject.Text, this.Text, StringComparison.OrdinalIgnoreCase) == 0)
            {
                AssociatedObject.Text = string.Empty;
                IsWatermarked = false;
            }
            //if ((AssociatedObject.Text == emptyField))
            //{
            //    //AssociatedObject.Text = emptyField;
            //    IsWatermarked = true;
            //}

        }

        /// <summary>
        /// This method is called when focus is lost from the TextBox. It puts the watermark
        /// into place if no text is in the textbox.
        /// </summary>
        /// <param name=”sender”></param>
        /// <param name=”e”></param>
        private void OnLostFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                logger.Debug("CustomValidator : OnLostFocus() Method - Entry");
                TextBox tempTxtBox = sender as TextBox;

                if (tempTxtBox.Name == "txtTValue2")
                {
                    logger.Info("CustomValidator : OnLostFocus() Method - Threshold 2 validation starts");
                    string selectedFormat = Settings.GetInstance().SelectedFormat;
                    decimal d;
                    DateTime datetime;
                    if (string.IsNullOrEmpty(AssociatedObject.Text.Trim().ToString()))
                    {
                        Text = emptyField;
                        AssociatedObject.Text = this.Text;
                        IsWatermarked = true;
                        //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                        logger.Info("CustomValidator : OnLostFocus() Method - Control" + tempTxtBox.Name + " is Null");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(selectedFormat))
                        {
                            if (selectedFormat[0].ToString() == "D")
                            {
                                if (!decimal.TryParse(AssociatedObject.Text, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out d))
                                {
                                    Text = decimalFormat;
                                    AssociatedObject.Text = this.Text;
                                    IsWatermarked = true;
                                    //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                                    logger.Info("CustomValidator : OnLostFocus() Method - Control Value should be in decimal format");
                                }
                                else if (int.Parse(tempTxtBox.Text) <= int.Parse(Settings.GetInstance().TValue1))
                                {
                                    Text = ThresholdMessage2;
                                    AssociatedObject.Text = this.Text;
                                    IsWatermarked = true;
                                    //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                                    logger.Info("CustomValidator : OnLostFocus() Method - Threshold 2 Value should be in greater than threshold 1");
                                }
                            }
                            if (selectedFormat[0].ToString() == "T")
                            {
                                if (!DateTime.TryParseExact(AssociatedObject.Text, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out datetime))
                                {
                                    Text = timeFormat;
                                    AssociatedObject.Text = this.Text;
                                    IsWatermarked = true;
                                    //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                                    logger.Info("CustomValidator : OnLostFocus() Method - Control Value should be in time format");
                                }
                                else
                                {
                                    DateTime t1 = DateTime.Parse(Settings.GetInstance().TValue1);
                                    DateTime t2 = DateTime.Parse(tempTxtBox.Text);
                                    if ((DateTime.Compare(t1, t2) > 0) || (DateTime.Compare(t1, t2) == 0))
                                    {
                                        Text = ThresholdMessage2;
                                        AssociatedObject.Text = this.Text;
                                        IsWatermarked = true;
                                        //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                                        logger.Info("CustomValidator : OnLostFocus() Method - Threshold 2 Value should be in greater than threshold 1");
                                    }
                                }
                            }
                        }

                    }
                    logger.Info("CustomValidator : OnLostFocus() Method - Threshold 2 validation ends");
                }
                else if (tempTxtBox.Name == "txtTvalue1")
                {
                    logger.Info("CustomValidator : OnLostFocus() Method - Threshold 2 validation starts");
                    string selectedFormat = Settings.GetInstance().SelectedFormat;
                    decimal d;
                    DateTime datetime;
                    if (string.IsNullOrEmpty(AssociatedObject.Text.Trim().ToString()))
                    {
                        Text = emptyField;
                        AssociatedObject.Text = this.Text;
                        IsWatermarked = true;
                        //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                        logger.Info("CustomValidator : OnLostFocus() Method - Control" + tempTxtBox.Name + " Value is Null");
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(selectedFormat))
                        {
                            if (selectedFormat[0].ToString() == "D")
                            {
                                if (!decimal.TryParse(AssociatedObject.Text, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out d))
                                {
                                    Text = decimalFormat;
                                    AssociatedObject.Text = this.Text;
                                    IsWatermarked = true;
                                    //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                                    logger.Info("CustomValidator : OnLostFocus() Method - Control" + tempTxtBox.Name + " Value should be in DateTime format");
                                }
                                else if (int.Parse(tempTxtBox.Text) >= int.Parse(Settings.GetInstance().TValue2))
                                {
                                    Text = ThresholdMessage1;
                                    AssociatedObject.Text = this.Text;
                                    IsWatermarked = true;
                                    //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                                    logger.Info("CustomValidator : OnLostFocus() Method - Threshold 2 Value should be in less than threshold 1");
                                }
                            }
                            if (selectedFormat[0].ToString() == "T")
                            {
                                if (!DateTime.TryParseExact(AssociatedObject.Text, "HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out datetime))
                                {
                                    Text = timeFormat;
                                    AssociatedObject.Text = this.Text;
                                    IsWatermarked = true;
                                    //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                                    logger.Info("CustomValidator : OnLostFocus() Method - Control" + tempTxtBox.Name + " Value should be in DateTime format");
                                }
                                else
                                {
                                    DateTime t2 = DateTime.Parse(Settings.GetInstance().TValue2);
                                    DateTime t1 = DateTime.Parse(tempTxtBox.Text);
                                    if ((DateTime.Compare(t1, t2) > 0) || (DateTime.Compare(t1, t2) == 0))
                                    {
                                        Text = ThresholdMessage1;
                                        AssociatedObject.Text = this.Text;
                                        IsWatermarked = true;
                                        //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                                        logger.Info("CustomValidator : OnLostFocus() Method - Threshold 2 Value should be in less than threshold 1");
                                    }
                                }
                            }


                        }

                    }
                    logger.Info("CustomValidator : OnLostFocus() Method - Threshold 2 validation ends");
                }
                else
                {
                    if (string.IsNullOrEmpty(AssociatedObject.Text.Trim().ToString()))
                    {
                        Text = emptyField;
                        AssociatedObject.Text = this.Text;
                        IsWatermarked = true;
                        //Settings.GetInstance().editVMObj.IsSavebtnEnable = false;
                        logger.Info("CustomValidator : OnLostFocus() Method - Control" + tempTxtBox.Name + " Value is Null");
                    }
                }
                logger.Debug("ThresholdValidator : OnLostFocus() Method - Exit");
            }
            catch (Exception ex)
            {
                logger.Error("ThresholdValidator : OnLostFocus() Method - Execption caught : " + ex.Message);
            }


        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrEmpty(AssociatedObject.Text.Trim().ToString()))
            {
                AssociatedObject.Text = emptyField;
                IsWatermarked = true;
            }
            else
            {
            IsWatermarked = false;
            }

        }
        private void OnTextChanged(object sender, RoutedEventArgs e)
        {
            

            if ((AssociatedObject.Text == emptyField))//||(string.IsNullOrEmpty(AssociatedObject.Text.Trim().ToString()
            {
                this.Text = AssociatedObject.Text;
                //AssociatedObject.Text = emptyField;
                IsWatermarked = true;
            }
            //if (string.IsNullOrEmpty(AssociatedObject.Text.Trim().ToString()))
            //{
            //    AssociatedObject.Text = emptyField;
            //    IsWatermarked = true;
            //}

            //if((AssociatedObject.Text != string.Empty)||(AssociatedObject.Text != null)||(AssociatedObject.Text != emptyField)||(AssociatedObject.Text != decimalFormat)||(AssociatedObject.Text != timeFormat)||(AssociatedObject.Text != ThresholdMessage1)||(AssociatedObject.Text != ThresholdMessage2))
            //{
            //    IsWatermarked = false;
            //}
            //else
            //{
            //    IsWatermarked = false;
            //}

        }
        public void ControlLoad(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(AssociatedObject.Text.Trim().ToString()))
            {
                AssociatedObject.Text = emptyField;
                IsWatermarked = true;
            }
            else
            {
                IsWatermarked = false;
            }
        }
    }
    #endregion
}
