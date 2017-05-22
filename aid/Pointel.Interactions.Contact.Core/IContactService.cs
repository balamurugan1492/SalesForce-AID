using Genesyslab.Platform.Contacts.Protocols;

namespace Pointel.Interactions.Contact.Core
{
    public interface IContactService
    {
        /// <summary>
        /// Notifies the contact protocol.
        /// </summary>
        /// <param name="ucsProtocol">The ucs protocol.</param>
        void NotifyContactProtocol(UniversalContactServerProtocol ucsProtocol);
    }
}
