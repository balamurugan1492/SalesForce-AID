/*
* =======================================
* Pointel.Configuration.Manager.Core
* =======================================
* Project    : Agent Interaction Desktop
* Created on : 
* Author     : Manikandan
* Owner      : Pointel Solutions
* =======================================
*/

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Pointel.Interactions.Email.Helpers
{
    public class Importer
    {
        [ImportMany(typeof(UserControl))]
        public IEnumerable<UserControl> win { get; set; }
    }
}
