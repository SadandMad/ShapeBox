using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CRectangles
{
    [Serializable]
    class Parallelogramm : Rectangle
    {
        public int Pdx;
        public Parallelogramm(int c1, int c2, int c3, int c4, Color Color1, Color Color2)
        {
            x1 = c1;
            x2 = c3;
            y1 = c2;
            y2 = c4;
            Pdx = (c3 - c1) / 5;
            Pclr = Color1;
            Fclr = Color2;
        }
        public Parallelogramm(int c1, int c2, int c3, int c4)
        {
            x1 = c1;
            x2 = c3;
            y1 = c2;
            Pdx = (c3 - c1) / 5;
            y2 = c4;
        }
        public Parallelogramm()
        {
        }
        public override void Draw(PictureBox picture)
        {
            Graphics elem = picture.CreateGraphics();
            Pen pen = new Pen(Pclr);
            SolidBrush brush = new SolidBrush(Fclr);
            Point[] pointArray = new Point[] { new Point(x1 + Pdx, y1), new Point(x2, y1), new Point(x2-Pdx, y2), new Point(x1, y2)};
            elem.FillPolygon(brush, pointArray);
            elem.DrawPolygon(pen, pointArray);
        }
    }
}