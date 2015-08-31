using System.Windows.Forms;

namespace RedRat.RaceTimingWinApp.ExtendedListView
{
    public class ListViewColumnMouseEventArgs : MouseEventArgs
    {
        public ListViewColumnMouseEventArgs(MouseEventArgs e, ListViewItem item, ListViewItem.ListViewSubItem subItem)
            : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        {
            Item = item;
            SubItem = subItem;
        }

        public ListViewItem Item { get; private set; }
        public ListViewItem.ListViewSubItem SubItem { get; private set; }
    }
}
