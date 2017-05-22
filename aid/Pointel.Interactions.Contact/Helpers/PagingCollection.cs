using System.Windows.Media.Imaging;
using Genesyslab.Platform.Commons.Collections;

namespace Pointel.Interactions.Contact.Helpers
{
    public class PagingCollection
    {
        public BitmapImage Img { get; set; }

        public string status { get; set; }

        public string subject { get; set; }

        public string startdate { get; set; }

        public string enddate { get; set; }

        public string fromaddress { get; set; }

        public string toaddress { get; set; }

        public string ccaddress { get; set; }

        public string bccaddress { get; set; }

        public string message { get; set; }

        public string key { get; set; }

        public string value { get; set; }

        public string receiveddate { get; set; }

        public string interactionid { get; set; }

        public KeyValueCollection KeyValueCaseInfo { get; set; }

        public string note { get; set; }

        public string disposition { get; set; }

        public string contactId { get; set; }

        public string type { get; set; }

        public string customerSegement { get; set; }
    }

    public class PagingCollectionContactDirectory
    {
        public BitmapImage Img { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }

        public string emailaddress { get; set; }

        public string contactId { get; set; }

        public string phonenumber { get; set; }
    }

    public class pagingCollectionContactDirectoryFilter
    {
        public BitmapImage Img { get; set; }

        public string firstname { get; set; }

        public string lastname { get; set; }

        public string emailaddress { get; set; }

        public string contactId { get; set; }

        public string phonenumber { get; set; }
    }

    public class PagingCollectionHistory
    {
        public BitmapImage Img { get; set; }

        public string status { get; set; }

        public string subject { get; set; }

        public string startdate { get; set; }

        public string enddate { get; set; }

        public string fromaddress { get; set; }

        public string toaddress { get; set; }

        public string ccaddress { get; set; }

        public string bccaddress { get; set; }

        public string message { get; set; }

        public KeyValueCollection KeyValueCaseInfo { get; set; }

        public string note { get; set; }

        public string disposition { get; set; }

        public string interactionid { get; set; }

        public string contactId { get; set; }

        public string type { get; set; }

        public string customerSegement { get; set; }
    }
}