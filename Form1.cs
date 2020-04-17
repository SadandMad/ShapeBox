using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using IGeometric;
using CGeometric;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Linq;

namespace ООП_1
{
    public partial class MainForm : Form
    {
        [Serializable]
        public class GeometricSerializer
        {
            public List<Geometric> Glist = new List<Geometric>();

            public void Save()
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Geometrics.dat"), FileMode.OpenOrCreate))
                {
                    foreach (Geometric elem in Glist)
                    {
                        formatter.Serialize(fs, elem);
                    }
                    MessageBox.Show("Успешно сохранено!");
                }
                Glist.Clear();
            }
            public void Load()
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Geometrics.dat"), FileMode.OpenOrCreate))
                {
                    bool suc = true;
                    while (fs.Position<fs.Length)
                    {
                        try
                        {
                            Geometric elem = (Geometric)formatter.Deserialize(fs, null);
                            Glist.Add(elem);
                        }
                        catch (SerializationException)
                        {
                            MessageBox.Show("Похоже, одной из фигур больше нет в списке плагинов.");
                            suc = false;
                            break;
                        }
                    }
                    if (suc)
                        MessageBox.Show("Успешно загружено!");
                }
            }
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
            bool fl = true;
            foreach (string file in pluginFiles)
            {
                Assembly asm = Assembly.LoadFrom(file);
                try
                {
                    var types = asm.GetTypes().
                                Where(t => t.GetInterfaces().
                                Where(i => i.FullName == typeof(GeometricInterface).FullName).Any());
                    foreach (Type type in types)
                    {
                        var plugin = asm.CreateInstance(type.FullName) as GeometricInterface;
                        GIlist.Add(plugin);
                        comboBox1.Items.Add(type.Name);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    if (fl)
                    {
                        string mes = ex.LoaderExceptions[0].Message;
                        MessageBox.Show("Ого! Похоже, вам не хватает плагина "+ mes.Substring(mes.IndexOf('"'), mes.IndexOf(',')-mes.IndexOf('"')) +"\".");
                        fl = false;
                    }
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
            GS.Save();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            GS.Load();
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

        private void button7_Click(object sender, EventArgs e)
        {
            RefreshPlugins();
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