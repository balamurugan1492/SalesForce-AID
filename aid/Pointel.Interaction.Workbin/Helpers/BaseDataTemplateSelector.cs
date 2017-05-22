using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Pointel.Interaction.Workbin.Controls;

namespace Pointel.Interaction.Workbin.Helpers
{
    public class BaseDataTemplateSelector : DataTemplateSelector
    {
        #region Member Variables
        #endregion

        #region Constructors

        /*
         * The default constructor
         */
        public BaseDataTemplateSelector()
        {
        }

        #endregion

        #region Properties
        #endregion

        #region Functions
        protected WorkbinUserControl GetWindow1(DependencyObject inContainer)
        {
            DependencyObject c = inContainer;

            while (true)
            {
                DependencyObject p = VisualTreeHelper.GetParent(c);

                if (c is WorkbinUserControl)
                {
                    //mSectionControl = c;
                    return c as WorkbinUserControl;
                }
                else
                {
                    c = p;
                }
            }
        }
        #endregion
    }
}
