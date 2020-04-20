using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IGeometric
{
    [Serializable]
    public abstract class Geometric : GeometricInterface
    {
        public string Name;
        public int x1 = 0;
        public int y1 = 0;
        public int x2 = 0;
        public int y2 = 0;
        public Color Pclr = Color.Black;
        public Color Fclr = Color.Transparent;
        public abstract void Draw(PictureBox picture);
    }
}