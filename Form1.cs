using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using IGeometric;
using CGeometric;
using CLine;
using CEllipse;
using Rectangle = CRectangle.Rectangle;
using System.Text.Json;
using System.Collections;
using System.Text.Encodings.Web;
using System.Runtime.Serialization.Formatters.Binary;

namespace ООП_1
{
    public partial class MainForm : Form
    {
        [Serializable]
        public class GeometricSerializer
        {
            public ArrayList Glist = new ArrayList();
        }

        internal int mx, my;
        internal readonly string pluginPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
        internal List<GeometricInterface> GIlist = new List<GeometricInterface>();
        GeometricSerializer GS = new GeometricSerializer();

        public MainForm()
        {
            InitializeComponent();
            RefreshPlugins();
        }

        private void RefreshPlugins()
        {
            comboBox1.Items.Clear();
            GIlist.Clear();

            DirectoryInfo pluginDirectory = new DirectoryInfo(pluginPath);
            if (!pluginDirectory.Exists)
                pluginDirectory.Create();
    
            var pluginFiles = Directory.GetFiles(pluginPath, "*.dll");
            foreach (var file in pluginFiles)
            {
                Assembly asm = Assembly.LoadFrom(file);
                var types = asm.GetTypes(); 
                foreach (var type in types)
                {
                    var plugin = asm.CreateInstance(type.FullName) as GeometricInterface;
                    GIlist.Add(plugin);
                    comboBox1.Items.Add(type.Name);
                }
            }
        }
        
        private void button1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == DialogResult.Cancel)
                return;
            button1.BackColor = colorDialog1.Color;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.Cancel)
                return;
            button2.BackColor = colorDialog2.Color;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (Geometric elem in GS.Glist)
            {
                elem.Draw(pictureBox1);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = null;
        }
        private void button5_Click(object sender, EventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Geometrics.dat"), FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, GS);
                MessageBox.Show("Успешно сохранено!");
            }
            GS.Glist.Clear();
        }
        /*private async void button5_Click(object sender, EventArgs e)
        {
            using (FileStream fs = new FileStream(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Geometrics.dat"), FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync<GeometricSerializer>(fs, GS);
                MessageBox.Show("Успешно сохранено!");
            }
            GS.Glist.Clear();
        }*/
        private void button6_Click(object sender, EventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Geometrics.dat"), FileMode.OpenOrCreate))
            {
                GS = (GeometricSerializer)formatter.Deserialize(fs, null);
                MessageBox.Show("Успешно загружено!");
            }
        }
        /*private async void button6_Click(object sender, EventArgs e)
        {
            using (FileStream fs = new FileStream(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Geometrics.dat"), FileMode.OpenOrCreate))
            {
                GS = await JsonSerializer.DeserializeAsync<GeometricSerializer>(fs);
                MessageBox.Show("Успешно загружено!");
            }
        }*/

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mx = e.X;
            my = e.Y;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            GS.Glist.Add(new Line { x1 = 10, y1 = 10, x2 = 50, y2 = 50 });
            GS.Glist.Add(new Line { x1 = 10, y1 = 15, x2 = 50, y2 = 60, Pclr = Color.Red });
            GS.Glist.Add(new Rectangle(70, 10, 85, 90, Color.Black, Color.Red));
            GS.Glist.Add(new Ellipse { x1 = 10, y1 = 10, x2 = 50, y2 = 50 });
            GS.Glist.Add(new Rectangle(120, 90, 200, 150));
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            foreach (GeometricInterface elem in GIlist)
            {
                if (System.String.Compare(elem.GetType().Name, (string) comboBox1.SelectedItem) == 0)
                {
                    Type type = elem.GetType();
                    object figure = Activator.CreateInstance(type, new Object[] { mx, my, e.X, e.Y, colorDialog1.Color, colorDialog2.Color });
                    MethodInfo method = type.GetMethod("Draw");
                    method.Invoke(figure, new Object[] { pictureBox1 });
                    GS.Glist.Add((Geometric)figure);
                }
            }
        }
    }
}