/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Sakthikumar
* Owner      : Pointel Solutions
* =======================================
*/

using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pointel.Interactions.Email.Utility
{
    /// <summary>
    /// Class ControlUtility.
    /// </summary>
    public class ControlUtility
    {
        /// <summary>
        /// Determines whether [is content exceed] [the specified text box control].
        /// </summary>
        /// <param name="textBoxControl">The text box control.</param>
        /// <returns><c>true</c> if [is content exceed] [the specified text box control]; otherwise, <c>false</c>.</returns>
        public static bool IsContentExceed(TextBox textBoxControl)
        {
            FormattedText ft = new FormattedText(textBoxControl.Text, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
                                        new Typeface(textBoxControl.FontFamily, textBoxControl.FontStyle, textBoxControl.FontWeight, textBoxControl.FontStretch),
                                        textBoxControl.FontSize, textBoxControl.Foreground);
            return textBoxControl.ActualWidth < ft.Width;
        }
    }
}
