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
            Margin = new Padding(0);
            TextAlign = ContentAlignment.MiddleCenter;
            AutoSize = false;

            SizeChanged += LabelSizeChanged;
        }

        void LabelSizeChanged(object sender, System.EventArgs e)
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
    }
}
