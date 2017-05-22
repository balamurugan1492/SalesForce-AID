
namespace Pointel.Interactions.Contact.Core.Listener
{
    internal class ContactManager
    {
        #region Field Declaration
        private Pointel.Logger.Core.ILog logger = Pointel.Logger.Core.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType, "AID");
        private static IContactDetailService messageToClientDetail = null;
        private static ContactManager contactListener; 
        #endregion

        #region GetInstance
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        public static ContactManager GetInstance()
        {
            if (contactListener == null)
            {
                contactListener = new ContactManager();

                return contactListener;
            }
            else
            {
                return contactListener;
            }
        }
        #endregion

        public void Subscribe(IContactDetailService listener)
        {
            messageToClientDetail = listener;
        }
    }
}
