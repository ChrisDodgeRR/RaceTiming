using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace RedRat.RaceTimingWinApp.ExtendedListView
{
    public abstract class ListViewColumn
    {
        public event EventHandler<ListViewColumnMouseEventArgs> Click;

        protected ListViewColumn( int columnIndex )
        {
            if ( columnIndex < 0 )
            {
                throw new ArgumentException( null, "columnIndex" );
            }

            ColumnIndex = columnIndex;
        }

        public virtual ListViewExtender Extender { get; protected internal set; }
        public int ColumnIndex { get; private set; }

        public virtual Font Font
        {
            get { return Extender == null ? null : Extender.Font; }
        }

        public ListView ListView
        {
            get { return Extender == null ? null : Extender.ListView; }
        }

        public abstract void Draw( DrawListViewSubItemEventArgs e );

        public virtual void MouseClick( MouseEventArgs e, ListViewItem item, ListViewItem.ListViewSubItem subItem )
        {
            if ( Click != null )
            {
                Click( this, new ListViewColumnMouseEventArgs( e, item, subItem ) );
            }
        }

        public virtual void Invalidate( ListViewItem item, ListViewItem.ListViewSubItem subItem )
        {
            if ( Extender != null )
            {
                Extender.ListView.Invalidate( subItem.Bounds );
            }
        }
    }



 
}