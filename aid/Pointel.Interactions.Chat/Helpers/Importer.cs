﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace Pointel.Interactions.Chat.Helpers
{
    public class Importer
    {
        [ImportMany(typeof(UserControl))]
        public IEnumerable<UserControl> win { get; set; }
    }
}
