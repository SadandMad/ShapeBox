using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CRectangle;
using Rectangle = CRectangle.Rectangle;

namespace CEqTriangle
{
    [Serializable]
    class EqTriangle : Rectangle
    {
        public EqTriangle(int c1, int c2, int c3, int c4, Color Color1, Color Color2)
        {
            x1 = c1;
            x2 = c3;
            y1 = c2;
            y2 = c4;
            Pclr = Color1;
            Fclr = Color2;
        }
        public EqTriangle(int c1, int c2, int c3, int c4)
        {
            x1 = c1;
            x2 = c3;
            y1 = c2;
            y2 = c4;
        }
        public EqTriangle()
        {
        }
        public override void Draw(PictureBox picture)
        {
            Graphics elem = picture.CreateGraphics();
            Pen pen = new Pen(Pclr);
            // SolidBrush brush = new SolidBrush(Fclr);
            elem.DrawLine(pen, (x1 + x2) / 2, y1, x2, y2);
            elem.DrawLine(pen, x2, y2, x1, y2);
            elem.DrawLine(pen, x1, y2, (x1 + x2) / 2, y1);
        }
    }
}
