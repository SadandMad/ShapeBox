﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using IGeometric;
using System.Runtime.Serialization.Formatters.Binary;
using System.Drawing;
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
                using (FileStream fs = new FileStream(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "Geometrics.dat"), FileMode.Create))
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
        internal GeometricSerializer GS = new GeometricSerializer();
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
                        try
                        {
                            var plugin = asm.CreateInstance(type.FullName) as GeometricInterface;
                            GIlist.Add(plugin);
                            comboBox1.Items.Add(type.Name);
                        }
                        catch (MissingMethodException)
                        {
                            MessageBox.Show("Похоже, какой-то из плагинов не является фигурой. Извините...");
                        }
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
            button1.ForeColor = Color.FromArgb(button1.BackColor.R > 127 ? 0 : 255, button1.BackColor.G > 127 ? 0 : 255, button1.BackColor.B > 127 ? 0 : 255);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (colorDialog2.ShowDialog() == DialogResult.Cancel)
                return;
            button2.BackColor = colorDialog2.Color;
            button2.ForeColor = Color.FromArgb(button2.BackColor.R > 127 ? 0 : 255, button2.BackColor.G > 127 ? 0 : 255, button2.BackColor.B > 127 ? 0 : 255);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            foreach (Geometric elem in GS.Glist)
                elem.Draw(pictureBox1);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            pictureBox1.CreateGraphics().FillRectangle(new SolidBrush(Color.White), 0, 0, pictureBox1.Width, pictureBox1.Height);
            GS.Glist.Clear();
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
                pictureBox1.CreateGraphics().FillRectangle(new SolidBrush(Color.White), 0, 0, pictureBox1.Width, pictureBox1.Height);
                
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

        private void button8_Click(object sender, EventArgs e)
        {
            if (GS.Glist.Count > 0)
            {
                GS.Glist.RemoveAt(GS.Glist.Count - 1);
                pictureBox1.CreateGraphics().FillRectangle(new SolidBrush(Color.White), 0, 0, pictureBox1.Width, pictureBox1.Height);
                foreach (Geometric elem in GS.Glist)
                    elem.Draw(pictureBox1);
            }
            else
                MessageBox.Show("А шо удалять-то?");
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