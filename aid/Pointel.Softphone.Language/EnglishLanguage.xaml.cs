using System.ComponentModel.Composition;
using System.Windows;

namespace Pointel.Softphone.Language
{
    [ExportMetadata("Culture", "en-US")]
    [Export(typeof(ResourceDictionary))]
    public partial class EnglishLanguage : ResourceDictionary
    {
        public EnglishLanguage()
        {
            InitializeComponent();
        }
    }
}