using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using IGeometric;
using CGeometric;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;

namespace ООП_1
{
    public partial class MainForm : Form
    {
        [Serializable]
        public class GeometricSerializer
        {
            public List<Geometric> Glist = new List<Geometric>();
        }

        internal int mx, my;
        internal readonly string pluginPath = System.String.Join("\\", new string[] { System.IO.Directory.GetCurrentDirectory(), "Plugins" });
        internal List<GeometricInterface> GIlist = new List<GeometricInterface>();
        GeometricSerializer GS = new GeometricSerializer();
        internal bool mouseFl = false;

        public MainForm()
        {
            InitializeComponent();
            RefreshPlugins();
        }

        public void RefreshPlugins()
        {
            comboBox1.Items.Clear();
            GIlist.Clear();
            DirectoryInfo pluginDirectory = new DirectoryInfo(pluginPath);
            if (!pluginDirectory.Exists)
                pluginDirectory.Create();
            string[] pluginFiles = Directory.GetFiles(pluginPath, "*.dll");
            foreach (string file in pluginFiles)
            {
                Assembly asm = Assembly.LoadFrom(file);
                var types = asm.GetTypes();
                foreach (Type type in types)
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
                elem.Draw(pictureBox1);
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
        private void button6_Click(object sender, EventArgs e)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Geometrics.dat"), FileMode.OpenOrCreate))
            {
                GS = (GeometricSerializer)formatter.Deserialize(fs, null);
                MessageBox.Show("Успешно загружено!");
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            mx = e.X;
            my = e.Y;
            mouseFl = true;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseFl)
            {
                SolidBrush brush = new SolidBrush(Color.White);
                Graphics graph = pictureBox1.CreateGraphics();
                graph.FillRectangle(brush, 1, 1, pictureBox1.Width-1, pictureBox1.Height-1);
                
                foreach (Geometric elem in GS.Glist)
                    elem.Draw(pictureBox1);
                foreach (GeometricInterface elem in GIlist)
                {
                    if (System.String.Compare(elem.GetType().Name, (string)comboBox1.SelectedItem) == 0)
                    {
                        Type type = elem.GetType();
                        object figure = Activator.CreateInstance(type, new Object[] { mx, my, e.X, e.Y, colorDialog1.Color, colorDialog2.Color });
                        MethodInfo method = type.GetMethod("Draw");
                        method.Invoke(figure, new Object[] { pictureBox1 });
                    }
                }
            }
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            mouseFl = false;
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