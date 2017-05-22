using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Pointel.Windows.Views.Common.Editor.Helper
{
    public class CommonDocumentPaginator : DocumentPaginator
    {
        private Size targetPageSize;
        private Thickness textMargin;
        private Thickness headerFooterMargin;
        private DocumentPaginator originalDocumentPaginator;
        private Typeface typeface;
        private bool showPageNumber = true;
        private int pageCount;
        private IList<PageRange> pageRanges = new List<PageRange>();
        private Dictionary<int, int> originalPageNumberByNewPageNumber = new Dictionary<int, int>();
        private bool useRange;
        public override bool IsPageCountValid
        {
            get
            {
                return this.originalDocumentPaginator.IsPageCountValid;
            }
        }
        public override int PageCount
        {
            get
            {
                return this.pageCount;
            }
        }
        public override Size PageSize
        {
            get
            {
                return this.originalDocumentPaginator.PageSize;
            }
            set
            {
                this.originalDocumentPaginator.PageSize = value;
            }
        }
        public override IDocumentPaginatorSource Source
        {
            get
            {
                return this.originalDocumentPaginator.Source;
            }
        }
        public CommonDocumentPaginator(DocumentPaginator paginator, Size pageSize, Thickness textMargin, Thickness headerFooterMargin, IList<PageRange> ranges, bool showPageNumber)
        {
            this.targetPageSize = pageSize;
            this.textMargin = textMargin;
            this.headerFooterMargin = headerFooterMargin;
            this.originalDocumentPaginator = paginator;
            this.originalDocumentPaginator.PageSize = new Size(Math.Max(1.0, this.targetPageSize.Width - textMargin.Left - textMargin.Right), Math.Max(1.0, this.targetPageSize.Height - textMargin.Top - textMargin.Bottom));
            this.SetOriginalPageNumber(ranges);
            this.showPageNumber = showPageNumber;
        }
        public override DocumentPage GetPage(int pageNumber)
        {
            int num = pageNumber;
            if (pageNumber >= 0 && pageNumber < this.originalPageNumberByNewPageNumber.Count && this.originalPageNumberByNewPageNumber.Count > 0)
            {
                num = this.originalPageNumberByNewPageNumber[pageNumber];
            }
            DocumentPage page = this.originalDocumentPaginator.GetPage(num);
            if (page == DocumentPage.Missing)
            {
                this.ComputeNewPageCount();
                return page;
            }
            ContainerVisual containerVisual = new ContainerVisual();
            DrawingVisual drawingVisual = null;
            if (this.showPageNumber)
            {
                drawingVisual = new DrawingVisual();
                using (DrawingContext drawingContext = drawingVisual.RenderOpen())
                {
                    if (this.typeface == null)
                    {
                        this.typeface = new Typeface("Times New Roman");
                    }
                    FormattedText formattedText = new FormattedText(num + 1 + (this.IsPageCountValid ? ("/" + this.originalDocumentPaginator.PageCount) : ""), CultureInfo.CurrentCulture, FlowDirection.LeftToRight, this.typeface, 14.0, Brushes.Black);
                    VerticalAlignment verticalAlignment = VerticalAlignment.Bottom;
                    HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;
                    Point origin = default(Point);
                    origin.Y = ((verticalAlignment == VerticalAlignment.Bottom) ? (this.targetPageSize.Height - formattedText.Height - this.headerFooterMargin.Bottom) : this.headerFooterMargin.Top);
                    switch (horizontalAlignment)
                    {
                        case HorizontalAlignment.Left:
                            origin.X = this.headerFooterMargin.Left;
                            formattedText.TextAlignment = TextAlignment.Left;
                            break;
                        case HorizontalAlignment.Center:
                            origin.X = this.targetPageSize.Width / 2.0;
                            formattedText.TextAlignment = TextAlignment.Center;
                            break;
                        case HorizontalAlignment.Right:
                            origin.X = this.targetPageSize.Width - this.headerFooterMargin.Right;
                            formattedText.TextAlignment = TextAlignment.Right;
                            break;
                    }
                    drawingContext.DrawText(formattedText, origin);
                }
            }
            ContainerVisual containerVisual2 = new ContainerVisual();
            containerVisual2.Children.Add(page.Visual);
            containerVisual2.Transform = new TranslateTransform(this.textMargin.Left, this.textMargin.Top);
            if (this.showPageNumber)
            {
                containerVisual.Children.Add(drawingVisual);
            }
            containerVisual.Children.Add(containerVisual2);
            return new DocumentPage(containerVisual, this.targetPageSize, this.Move(page.BleedBox), this.Move(page.ContentBox));
        }
        private void ComputeNewPageCount()
        {
            if (this.originalDocumentPaginator.IsPageCountValid)
            {
                int num = this.originalDocumentPaginator.PageCount;
                if (this.useRange)
                {
                    using (Dictionary<int, int>.ValueCollection.Enumerator enumerator = this.originalPageNumberByNewPageNumber.Values.GetEnumerator())
                    {
                        while (enumerator.MoveNext())
                        {
                            int current = enumerator.Current;
                            if (current < num)
                            {
                                this.pageCount++;
                            }
                        }
                        return;
                    }
                }
                this.pageCount = num;
            }
        }
        private void SetOriginalPageNumber(IList<PageRange> ranges)
        {
            this.pageRanges.Clear();
            this.pageCount = 0;
            this.useRange = (ranges != null);
            if (ranges != null && ranges.Count > 0)
            {
                this.pageRanges = ranges;
                for (int i = 0; i < this.pageRanges.Count; i++)
                {
                    PageRange pageRange = this.pageRanges[i];
                    if (pageRange.PageFrom > 0 || pageRange.PageTo > 0)
                    {
                        pageRange.PageFrom = Math.Max(1, pageRange.PageFrom);
                        pageRange.PageTo = Math.Max(1, pageRange.PageTo);
                        if (pageRange.PageFrom > pageRange.PageTo)
                        {
                            int pageFrom = pageRange.PageFrom;
                            pageRange.PageFrom = pageRange.PageTo;
                            pageRange.PageTo = pageFrom;
                        }
                        for (int j = pageRange.PageFrom; j <= pageRange.PageTo; j++)
                        {
                            this.originalPageNumberByNewPageNumber[j - 1] = 0;
                        }
                    }
                }
                List<int> list = new List<int>(this.originalPageNumberByNewPageNumber.Keys);
                list.Sort((int x, int y) => x.CompareTo(y));
                this.originalPageNumberByNewPageNumber.Clear();
                for (int k = 0; k < list.Count; k++)
                {
                    this.originalPageNumberByNewPageNumber[k] = list[k];
                }
            }
            this.ComputeNewPageCount();
        }
        private Rect Move(Rect rect)
        {
            if (rect.IsEmpty)
            {
                return rect;
            }
            return new Rect(rect.Left + this.textMargin.Left, rect.Top + this.textMargin.Top, Math.Max(1.0, rect.Width - this.textMargin.Left), Math.Max(1.0, rect.Height - this.textMargin.Top));
        }
    }
}
