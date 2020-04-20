using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IGeometric;

namespace CRectangles
{
    [Serializable]
    public class Rectangle: Geometric
    {
        public Rectangle(int c1, int c2, int c3, int c4, Color Color1, Color Color2)
        {
            x1 = c1;
            x2 = c3;
            y1 = c2;
            y2 = c4;
            Pclr = Color1;
            Fclr = Color2;
        }
        public Rectangle(int c1, int c2, int c3, int c4)
        {
            x1 = c1;
            x2 = c3;
            y1 = c2;
            y2 = c4;
        }
        public Rectangle()
        {
        }
        public override void Draw(PictureBox picture)
        {
            Graphics elem = picture.CreateGraphics();
            Pen pen = new Pen(Pclr);
            SolidBrush brush = new SolidBrush(Fclr);
            Point[] pointArray = new Point[] { new Point(x1, y1), new Point(x2, y1), new Point(x2, y2), new Point(x1, y2) };
            elem.FillPolygon(brush, pointArray);
            elem.DrawPolygon(pen, pointArray);
        }
    }
}
