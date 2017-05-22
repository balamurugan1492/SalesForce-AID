using System.ComponentModel.Composition;
using System.Windows;

namespace Pointel.Softphone.Language
{
    [ExportMetadata("Culture", "es-ES")]
    [Export(typeof(ResourceDictionary))]
    public partial class SpanishLanguage : ResourceDictionary
    {
        public SpanishLanguage()
        {
            InitializeComponent();
        }
    }
}