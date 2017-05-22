/*
 *======================================================
 * Pointel.Interaction.Workbin.Helpers.IEmailCaseData
 *======================================================
 * Project    : Agent Interaction Desktop
 * Created on : 3-Feb-2015
 * Author     : Sakthikumar
 * Owner      : Pointel Solutions
 *======================================================
 */

using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Pointel.Interaction.Workbin.Helpers
{
    public class Importer
    {
        [ImportMany(typeof(UserControl))]
        public IEnumerable<UserControl> win { get; set; }
    }
}
