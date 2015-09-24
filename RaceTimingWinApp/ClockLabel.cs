using System;
using System.Drawing;
using System.Threading;
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
            MinimumSize = new Size(200, 100);   // Stop window minimization setting ClientRectangle to 0 height.

            SizeChanged += LabelSizeChanged;
        }

        private void LabelSizeChanged( object sender, EventArgs e )
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
            Invoke( new Action( () => SetTimeLabel(time) ) );
        }

        public void SetTimeLabel( TimeSpan time )
        {
            Text = time.ToString( labelFormat );
        }

        public void Blink()
        {
            var currentBackColor = BackColor;
            Invoke(new Action( () => BackColor = Color.Green ));
            Thread.Sleep(150);
            Invoke(new Action(() => BackColor = currentBackColor));
        }
    }
}
