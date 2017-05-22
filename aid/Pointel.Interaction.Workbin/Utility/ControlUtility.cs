/*
 * ======================================================
 * Pointel.Interaction.Workbin.Utility.ControlUtility
 * ======================================================
 * Project    : Agent Interaction Desktop
 * Created on : 3-Feb-2015
 * Author     : Sakthikumar
 * Owner      : Pointel Solutions
 * ======================================================
 */

using System.Globalization;
using System.Windows.Controls;
using System.Windows.Media;

namespace Pointel.Interaction.Workbin.Utility
{
    public class ControlUtility
    {
        public static bool IsContentExceed(TextBox textBoxControl)
        {
            FormattedText ft = new FormattedText(textBoxControl.Text, CultureInfo.CurrentCulture, System.Windows.FlowDirection.LeftToRight,
                                        new Typeface(textBoxControl.FontFamily, textBoxControl.FontStyle, textBoxControl.FontWeight, textBoxControl.FontStretch),
                                        textBoxControl.FontSize, textBoxControl.Foreground);
            return textBoxControl.Width < ft.Width;
        }
    }
}
