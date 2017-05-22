using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Pointel.Interactions.Contact.Controls;

namespace Pointel.Interactions.Contact.Helpers
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
        protected ContactHistory GetWindow1(DependencyObject inContainer)
        {
            DependencyObject c = inContainer;
            
            while (true)
            {
                DependencyObject p = VisualTreeHelper.GetParent(c);

                if (c is ContactHistory)
                {
                    //mSectionControl = c;
                    return c as ContactHistory;
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
