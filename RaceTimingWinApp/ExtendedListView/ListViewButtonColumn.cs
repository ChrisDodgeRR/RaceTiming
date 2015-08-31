using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace RedRat.RaceTimingWinApp.ExtendedListView
{
    public class ListViewButtonColumn : ListViewColumn
    {
        private Rectangle hot = Rectangle.Empty;

        public ListViewButtonColumn(int columnIndex)
            : base(columnIndex)
        {}

        public bool FixedWidth { get; set; }
        public bool DrawIfEmpty { get; set; }

        public override ListViewExtender Extender
        {
            get { return base.Extender; }
            protected internal set
            {
                base.Extender = value;
                if (FixedWidth)
                {
                    base.Extender.ListView.ColumnWidthChanging += OnColumnWidthChanging;
                }
            }
        }

        protected virtual void OnColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if ( e.ColumnIndex != ColumnIndex ) return;

            e.Cancel = true;
            e.NewWidth = ListView.Columns[e.ColumnIndex].Width;
        }

        public override void Draw(DrawListViewSubItemEventArgs e)
        {
            if (hot != Rectangle.Empty)
            {
                if (hot != e.Bounds)
                {
                    ListView.Invalidate(hot);
                    hot = Rectangle.Empty;
                }
            }

            if ( ( !DrawIfEmpty ) && ( string.IsNullOrEmpty( e.SubItem.Text ) ) )
            {
                return;
            }

            var mouse = e.Item.ListView.PointToClient(Control.MousePosition);
            if ((ListView.GetItemAt(mouse.X, mouse.Y) == e.Item) &&
                 (e.Item.GetSubItemAt(mouse.X, mouse.Y) == e.SubItem))
            {
                ButtonRenderer.DrawButton(e.Graphics, e.Bounds, e.SubItem.Text, Font, true, PushButtonState.Hot);
                hot = e.Bounds;
            }
            else
            {
                ButtonRenderer.DrawButton(e.Graphics, e.Bounds, e.SubItem.Text, Font, false, PushButtonState.Default);
            }
        }
    }
}
