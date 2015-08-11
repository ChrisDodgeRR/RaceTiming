using System;
using System.Drawing;
using System.Windows.Forms;

namespace RedRat.RaceTimingWinApp
{
    public class ClockLabel : Label
    {
        public const string labelFormat = "hh\\:mm\\:ss";

        public ClockLabel()
        {
            Text = TimeSpan.FromSeconds( 0 ).ToString( labelFormat );
            ForeColor = Color.Blue;
            Margin = new Padding( 0 );
            TextAlign = ContentAlignment.MiddleCenter;
            AutoSize = false;

            SizeChanged += LabelSizeChanged;
        }

        private void LabelSizeChanged( object sender, System.EventArgs e )
        {
            Redraw();
        }

        public void Redraw()
        {
            // This could probably be improved...
            var newFont = Font;
            var textSize = TextRenderer.MeasureText(Text, Font);

            while (ClientRectangle.Width > textSize.Width && ClientRectangle.Height > textSize.Height)
            {
                newFont = new Font(Font.FontFamily, newFont.Size + 1);
                textSize = TextRenderer.MeasureText(Text, newFont);
            }
            while (ClientRectangle.Width < textSize.Width || ClientRectangle.Height < textSize.Height)
            {
                newFont = new Font(Font.FontFamily, newFont.Size - 1);
                textSize = TextRenderer.MeasureText(Text, newFont);
            }

            Font = newFont;            
        }

        public void ClockChangeEventListener( object sender, TimeSpan time )
        {
            Invoke( new Action( () => { Text = time.ToString( labelFormat ); } ) );
        }
    }
}
