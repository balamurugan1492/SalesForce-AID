using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;
using Pointel.Windows.Views.Common.Editor.Helper;

namespace Pointel.Windows.Views.Common.Editor.Controls
{
    /// <summary>
    /// Interaction logic for PrintPreviewWindow.xaml
    /// </summary>
    public partial class PrintPreviewWindow : Window
    {
        private DropShadowBitmapEffect _shadowEffect = new DropShadowBitmapEffect();
        private DocumentPaginator documentPaginator;
        //private PageRangeSelection pageRangeSelection;
        //private IList<PageRange> PagesRange; 
        //private MemoryStream memoryStream;
        //private Uri packageUri;
        //private XpsDocument xpsDocument;
        //private XpsSerializationManagerAsync xpsSerializationManager;
        double num;
        double num2;
        public PrintPreviewWindow(FlowDocument flowDoc,Paragraph generalContent)
        {
            InitializeComponent();
            num = 816.0;
            num2 = 1056.0;
            docViewer.FitToWidth();
            Thickness thickness = new Thickness(1);
            Thickness textMargin = new Thickness(thickness.Left * 96.0, thickness.Top * 96.0, thickness.Right * 96.0, thickness.Bottom * 96.0);
            Thickness headerFooterMargin = new Thickness(19.200000000000003);

            System.IO.MemoryStream s = new System.IO.MemoryStream();
            TextRange source = new TextRange(flowDoc.ContentStart, flowDoc.ContentEnd);
            source.Save(s, DataFormats.Xaml);
            FlowDocument copy = new FlowDocument();
            TextRange dest = new TextRange(copy.ContentStart, copy.ContentEnd);
            dest.Load(s, DataFormats.Xaml);
            copy.PagePadding = new Thickness(0.0);
            copy.ColumnWidth = double.PositiveInfinity;
            copy.Blocks.InsertBefore(copy.Blocks.FirstBlock, generalContent);

            IList<PageRange> ranges = null;
            this.documentPaginator = new CommonDocumentPaginator(((IDocumentPaginatorSource)copy).DocumentPaginator, new Size(num, num2), textMargin, headerFooterMargin, ranges, true);
            if (!this.documentPaginator.IsPageCountValid)
            {
                documentPaginator.ComputePageCountCompleted += new AsyncCompletedEventHandler(this.documentPaginator_ComputePageCountCompleted);
                documentPaginator.ComputePageCountAsync();
                return;
            }
            CreateDocument(documentPaginator);
            //XpsDocument xpsDoc = LoadAsXPS(new CommonDocumentPaginator(((IDocumentPaginatorSource)flowDoc).DocumentPaginator, new Size(595, 842), new Size(48, 48)));
            //docViewer.Document = xpsDoc.GetFixedDocumentSequence();  
        }
        private void documentPaginator_ComputePageCountCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (this.documentPaginator != null)
            {
                this.CreateDocument(this.documentPaginator);
            }
        }
        private void CreateDocument(DocumentPaginator documentPaginator)
        {
            docViewer.Document = LoadAsXPS(documentPaginator).GetFixedDocumentSequence();
            //this.memoryStream = new MemoryStream();
            //Package package = Package.Open(this.memoryStream, FileMode.Create, FileAccess.ReadWrite);
            //this.packageUri = new Uri("pack://" + Guid.NewGuid().ToString() + ".xps");
            //PackageStore.RemovePackage(packageUri);
            //PackageStore.AddPackage(this.packageUri, package);
            //this.xpsDocument = new XpsDocument(package, CompressionOption.NotCompressed,this.packageUri.AbsolutePath);
            //this.xpsSerializationManager = new XpsSerializationManagerAsync(new XpsPackagingPolicy(this.xpsDocument), false);
            //this.xpsSerializationManager.SaveAsXaml(documentPaginator);
            //FixedDocumentSequence fixedDocumentSequence = xpsDocument.GetFixedDocumentSequence();
            //xpsDocument.Close();
            //this.docViewer.Document = fixedDocumentSequence;
        }

        private void OnPrintClick(object sender, RoutedEventArgs e)
        {
            PrintDialog pd = new PrintDialog();
            if (pd.ShowDialog() != true) return;

            IDocumentPaginatorSource idocument = docViewer.Document as IDocumentPaginatorSource;
            pd.PrintDocument(idocument.DocumentPaginator, "Printing Flow Document...");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            docViewer.Document = null;
        }
        public XpsDocument LoadAsXPS(DocumentPaginator paginator)
        {
            MemoryStream stream = new MemoryStream();
            Package docPackage = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite);
            string inMemoryPackageName = string.Format("memorystream://{0}.xps", Guid.NewGuid());
            Uri packageUri = new Uri(inMemoryPackageName);
            PackageStore.AddPackage(packageUri, docPackage);
            XpsDocument xpsDoc = new XpsDocument(docPackage);
            xpsDoc.Uri = packageUri;
            XpsDocument.CreateXpsDocumentWriter(xpsDoc).Write(paginator);
            return xpsDoc;
        }
        public static MemoryStream FlowDocumentToXPS(FlowDocument flowDocument, int width, int height)
        {
            MemoryStream stream = new MemoryStream();
            using (Package package = Package.Open(stream, FileMode.Create, FileAccess.ReadWrite))
            {
                using (XpsDocument xpsDoc = new XpsDocument(package, CompressionOption.Maximum))
                {
                    XpsSerializationManager rsm = new XpsSerializationManager(new XpsPackagingPolicy(xpsDoc), false);
                    DocumentPaginator paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;
                    paginator.PageSize = new System.Windows.Size(width, height);

                    rsm.SaveAsXaml(paginator);
                    rsm.Commit();
                }
            }
            stream.Position = 0;
            return stream;
        }

        private void btnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
            {
                this.WindowState = WindowState.Maximized;
                btnMaximize.Style = FindResource("RestoreButton") as Style;
            }
            else
            {
                this.WindowState = WindowState.Normal;
            }
        }
        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.Close();
            }
            catch //(Exception exception)
            {
                // _logger.Error(" Error occurred while btnExit_Click() :" + exception.ToString());
            }
        }
        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Minimized;
                Topmost = false;
            }
            catch //(Exception exception)
            {
                //_logger.Error(" Error occurred while btnMinimize_Click() :" + exception.ToString());
            }
        }
        private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {

                DragMove();
                if (Left < 0)
                    Left = 0;
                if (Top < 0)
                    Top = 0;
                if (Left > SystemParameters.WorkArea.Right - Width)
                    Left = SystemParameters.WorkArea.Right - Width;
                if (Top > SystemParameters.WorkArea.Bottom - Height)
                    Top = SystemParameters.WorkArea.Bottom - Height;

            }
            catch //(Exception commonException)
            {

            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            MainBorder.BitmapEffect = _shadowEffect;
            MainBorder.BorderBrush = (System.Windows.Media.Brush)(new BrushConverter().ConvertFromString("#0070C5"));
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            MainBorder.BorderBrush = System.Windows.Media.Brushes.Black;
            MainBorder.BitmapEffect = null;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _shadowEffect.ShadowDepth = 0;
            _shadowEffect.Opacity = 0.5;
            _shadowEffect.Softness = 0.5;
            _shadowEffect.Color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#003660");
        }
    }
}
