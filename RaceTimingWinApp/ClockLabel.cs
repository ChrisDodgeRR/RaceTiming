using System;
using System.Drawing;
using System.Windows.Forms;

namespace RedRat.RaceTimingWinApp
{
    public class ClockLabel : Label
    {
        public ClockLabel()
        {
            Text = "00:00:00";
            ForeColor = Color.Blue;
            BackColor = Color.Gray;
            Margin = new Padding(0);
            TextAlign = ContentAlignment.MiddleCenter;
            AutoSize = false;
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // This isn't working yet.

            var graphics = e.Graphics;
            var labelWidth = ClientRectangle.Width;
            var labelHeight = ClientRectangle.Height;
            var extent = graphics.MeasureString(Text, Font);

            //Console.WriteLine("width: {0}, height: {1}, extent: {2}", labelWidth, labelHeight, extent);

            var newFont = Font;
            while (labelWidth > extent.Width && labelHeight > extent.Height )
            {
                newFont = new Font(Font.FontFamily, newFont.Size + 1);
                //Console.WriteLine("BIGGER: width: {0}, height: {1}, extent: {2}, em: {3}", labelWidth, labelHeight, extent, newFont.Size);
                extent = graphics.MeasureString(Text, newFont);
            }
            while (labelWidth < extent.Width || labelHeight < extent.Height)
            {
                newFont = new Font(Font.FontFamily, newFont.Size - 1);
                //Console.WriteLine("SMALLER: width: {0}, height: {1}, extent: {2}, em: {3}", labelWidth, labelHeight, extent, newFont.Size);
                extent = graphics.MeasureString(Text, newFont);
            }

            Font = newFont;

            base.OnPaint(e);
        }
    }
}
