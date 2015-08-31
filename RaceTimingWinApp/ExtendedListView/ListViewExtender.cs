using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace RedRat.RaceTimingWinApp.ExtendedListView
{
    public class ListViewExtender : IDisposable
    {
        private readonly Dictionary<int, ListViewColumn> columns = new Dictionary<int, ListViewColumn>();

        public ListViewExtender( ListView listView )
        {
            if ( listView == null )
            {
                throw new ArgumentNullException( "listView" );
            }

            if ( listView.View != View.Details )
            {
                throw new ArgumentException( null, "listView" );
            }

            ListView = listView;
            ListView.OwnerDraw = true;
            ListView.DrawItem += OnDrawItem;
            ListView.DrawSubItem += OnDrawSubItem;
            ListView.DrawColumnHeader += OnDrawColumnHeader;
            ListView.MouseMove += OnMouseMove;
            ListView.MouseClick += OnMouseClick;

            Font = new Font( ListView.Font.FontFamily, ListView.Font.Size - 2 );
        }

        public virtual Font Font { get; private set; }
        public ListView ListView { get; private set; }

        protected virtual void OnMouseClick(object sender, MouseEventArgs e)
        {
            ListViewItem item;
            ListViewItem.ListViewSubItem sub;
            var column = GetColumnAt(e.X, e.Y, out item, out sub);
            if (column != null)
            {
                column.MouseClick(e, item, sub);
            }
        }

        public ListViewColumn GetColumnAt(int x, int y, out ListViewItem item, out ListViewItem.ListViewSubItem subItem)
        {
            subItem = null;
            item = ListView.GetItemAt(x, y);
            if ( item == null )
            {
                return null;
            }

            subItem = item.GetSubItemAt(x, y);
            if ( subItem == null )
            {
                return null;
            }

            for (var i = 0; i < item.SubItems.Count; i++)
            {
                if (item.SubItems[i] == subItem)
                    return GetColumn(i);
            }
            return null;
        }

        protected virtual void OnMouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item;
            ListViewItem.ListViewSubItem sub;
            var column = GetColumnAt(e.X, e.Y, out item, out sub);
            if (column != null)
            {
                column.Invalidate(item, sub);
                return;
            }
            if (item != null)
            {
                ListView.Invalidate(item.Bounds);
            }
        }

        protected virtual void OnDrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
        }

        protected virtual void OnDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            var column = GetColumn(e.ColumnIndex);
            if (column == null)
            {
                e.DrawDefault = true;
                return;
            }

            column.Draw(e);
        }

        protected virtual void OnDrawItem(object sender, DrawListViewItemEventArgs e)
        {
            // do nothing
        }

        public void AddColumn(ListViewColumn column)
        {
            if ( column == null )
            {
                throw new ArgumentNullException( "column" );
            }

            column.Extender = this;
            columns[column.ColumnIndex] = column;
        }

        public ListViewColumn GetColumn(int index)
        {
            ListViewColumn column;
            return columns.TryGetValue(index, out column) ? column : null;
        }

        public IEnumerable<ListViewColumn> Columns
        {
            get { return columns.Values; }
        }

        public virtual void Dispose()
        {
            if ( Font == null ) return;
            Font.Dispose();
            Font = null;
        }
    }
}