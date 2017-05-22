
namespace Pointel.Integration.PlugIn.Common
{
    /// <summary>
    /// This class used to collect agent information - 29-07-2014 - smoorhty
    /// </summary>
    public class AgentInfo
    {
        #region Field Declaration
        private string lastName;
        private string firstName;
        private string employeeID;
        private string userName;
        private string loginID;
        private string extensionDN;
        private string queue;
        #endregion Field Declaration

        #region Properties
        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        /// <value>
        /// The last name.
        /// </value>
        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        /// <value>
        /// The first name.
        /// </value>
        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        /// <value>
        /// The employee identifier.
        /// </value>
        public string EmployeeID
        {
            get { return employeeID; }
            set { employeeID = value; }
        }

        /// <summary>
        /// Gets or sets the name of the user.
        /// </summary>
        /// <value>
        /// The name of the user.
        /// </value>
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        /// <summary>
        /// Gets or sets the login identifier.
        /// </summary>
        /// <value>
        /// The login identifier.
        /// </value>
        public string LoginID
        {
            get { return loginID; }
            set { loginID = value; }
        }

        /// <summary>
        /// Gets or sets the extension dn.
        /// </summary>
        /// <value>
        /// The extension dn.
        /// </value>
        public string ExtensionDN
        {
            get { return extensionDN; }
            set { extensionDN = value; }

        }
        public string Queue
        {
            get { return queue; }
            set { queue = value; }
        }

        #endregion Properties
    }
}
