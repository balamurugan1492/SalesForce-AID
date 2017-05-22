using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Text;

namespace Pointel.Interaction.Workbin.Helpers
{
    public enum IQTYPES
    {
        Category,
        Condition,
        Queue
    }

    public class IQMenuItem : TreeViewItemBase
    {


        public IQMenuItem()
        {
            this.Items = new ObservableCollection<IQMenuItem>();
            Color = "#FFFFFF";
        }

        public string DisplayName { get; set; }

        public string Condition { get; set; }

        public string ImagePath { get; set; }

        public IQTYPES IQTYPE { get; set; }

        public string DisplayedColoumns { get; set; }

        public string CaseDataBussinessAttribute { get; set; }

        public List<string> CaseDataToFilter
        {
            get;
            set;
        }

        public string QuickSearchAttributes { get; set; }

        public string Category { get; set; }

        public int SnapShotID = 0;

        public ObservableCollection<IQMenuItem> Items { get; set; }
    }
}
