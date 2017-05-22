using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pointel.Interactions.IPlugins
{
    public interface IEmailAttribute
    {
        string InteractionID
        {
            get;
        }
        string ParentInteractionID
        {
            get;
        }
    }
}
