using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IGeometric;

namespace CEllipse
{
    [Serializable]
    public class Ellipse : Geometric
    {
        public Ellipse(int c1, int c2, int c3, int c4, Color Color1, Color Color2)
        {
            x1 = c1;
            x2 = c3;
            y1 = c2;
            y2 = c4;
            Pclr = Color1;
            Fclr = Color2;
        }
        public Ellipse(int c1, int c2, int c3, int c4)
        {
            x1 = c1;
            x2 = c3;
            y1 = c2;
            y2 = c4;
        }
        public Ellipse()
        {

        }
        override public void Draw(PictureBox picture)
        {
            Graphics elem = picture.CreateGraphics();
            Pen pen = new Pen(Pclr);
            SolidBrush brush = new SolidBrush(Fclr);
            elem.FillEllipse(brush, x1, y1, x2 - x1, y2 - y1);
            elem.DrawEllipse(pen, x1, y1, x2 - x1, y2 - y1);
        }
    }
}
