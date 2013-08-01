using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace MapEditor
{
    public enum PlayerType
    {
        Player1, Player2, Neutral
    }
    public partial class Editor : Form
    {
        Graphics graphics;
        List<Planet> planets;
        int NextID = 0;
        
        public Editor()
        {
            InitializeComponent();
        }

        

        private void Editor_Load(object sender, EventArgs e)
        {
            graphics = Graphics.FromHwnd(BG.Handle);
            planets = new List<Planet>();
            PlanetProperty.PropertyValueChanged += new PropertyValueChangedEventHandler(PlanetProperty_PropertyValueChanged);
            
            BG.MouseClick += new MouseEventHandler(BG_MouseClick);
            BG.MouseMove += new MouseEventHandler(BG_MouseMove);
        }

        void BG_MouseMove(object sender, MouseEventArgs e)
        {
            if ((e.Button & MouseButtons.Left)==MouseButtons.Left)
            {
                Planet p = (PlanetProperty.SelectedObject as Planet);
                if (p == null)
                    return;
                if (Microsoft.Xna.Framework.Vector2.Distance(new Microsoft.Xna.Framework.Vector2(e.X,e.Y), p.Position/2) < p.PlanetSize/2 * 64)
                {
                    p.Position = new Microsoft.Xna.Framework.Vector2(e.X*2, e.Y*2);
                    PlanetProperty.SelectedObject = p;
                    ReDraw();
                }
            }
        }

        void PlanetProperty_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            ReDraw();
        }
        void BG_MouseClick(object sender, MouseEventArgs e)
        {
            Microsoft.Xna.Framework.Vector2 clickPosition = new Microsoft.Xna.Framework.Vector2(e.X, e.Y)*2;
            if (clickPosition.X < 800 && clickPosition.Y < 480)
            {
                foreach (Planet p in planets)
                {
                    if (Microsoft.Xna.Framework.Vector2.Distance(clickPosition, p.Position) < p.PlanetSize/2 * 64)
                    {
                        PlanetProperty.SelectedObject = p;
                        break;
                    }
                }
            }
        }
        private void ReDraw()
        {
            graphics.Clear(Color.Transparent);
            graphics.DrawImage(Properties.Resources.BG,Point.Empty);
            foreach (Planet p in planets)
            {
                Pen pen = new Pen(Color.White);
                Pen text = new Pen(Color.Black);
                switch (p.Owner)
                {
                    case PlayerType.Player1:
                        pen.Color = Color.LightGreen;
                        text.Color = Color.DarkGreen;
                        break;
                    case PlayerType.Player2:
                        pen.Color = Color.DarkRed;
                        text.Color = Color.LightPink;
                        break;
                }
                Microsoft.Xna.Framework.Vector2 real = p.Position/2 - new Microsoft.Xna.Framework.Vector2(p.PlanetSize * 64, p.PlanetSize * 64)/2;
                graphics.FillEllipse(pen.Brush, real.X, real.Y, p.PlanetSize/2 * 128, p.PlanetSize/2 * 128);
                graphics.DrawString(p.Forces.ToString(), Font, text.Brush, p.Position.X/2, p.Position.Y/2);
            }
        }
        List<Planet> LoadMap(string map)
        {
            List<Planet> mapPlanets = new List<Planet>();
            XmlReader xmlReader = XmlReader.Create(map);
            xmlReader.ReadStartElement();
            while (xmlReader.Read())
            {
                /*Read planet header*/
                if (xmlReader.LocalName == "Planet" && xmlReader.IsStartElement())
                {
                    /*Read the ID element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("ID");
                    int ID = xmlReader.ReadContentAsInt();

                    /*Read the owner element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Owner");
                    string owner = xmlReader.ReadContentAsString();

                    /*Read the forces element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Forces");
                    int forces = xmlReader.ReadContentAsInt();

                    /*Read the growth element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Growth");
                    int growth = xmlReader.ReadContentAsInt();

                    /*Read the growth cooldown element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("GrowthCooldown");
                    int growthcd = xmlReader.ReadContentAsInt();

                    /*Read the size element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Size");
                    float size = xmlReader.ReadContentAsFloat();

                    /*Read the Position element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Position");
                    Microsoft.Xna.Framework.Vector2 Position = new Microsoft.Xna.Framework.Vector2();

                    /*Read the X element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("X");
                    Position.X = xmlReader.ReadContentAsInt();

                    /*Read the Y element*/
                    xmlReader.Read();
                    xmlReader.MoveToElement();
                    xmlReader.ReadStartElement("Y");
                    Position.Y = xmlReader.ReadContentAsInt();

                    Planet p = new Planet();
                    p.ID = ID;
                    p.Position = Position;
                    p.Growth = growth;
                    p.GrowthCounter =growthcd;
                    p.Owner = (PlayerType)Enum.Parse(typeof(PlayerType), owner, false);
                    p.Forces=forces;
                    p.PlanetSize = size;
                    mapPlanets.Add(p);
                }

            }

            return mapPlanets;
        }

        private void btn_Create_Click(object sender, EventArgs e)
        {
            Planet p = new Planet();
            p.Owner = PlayerType.Neutral;
            p.ID = ++NextID;
            planets.Add(p);
            PlanetProperty.SelectedObject = p;
        }
        private void btn_Load_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.CheckPathExists = true;
            open.CheckFileExists = true;
            open.AddExtension = true;
            open.Filter = "XML files (*.xml)|*.xml";
            DialogResult result= open.ShowDialog();
            
            if (result == DialogResult.OK)
            {
                planets.Clear();
                planets.AddRange(LoadMap(open.FileName));
                foreach (Planet p in planets)
                {
                    if (p.ID > NextID)
                        NextID = p.ID;
                }
                ReDraw();
            }
        }
        private void btn_Save_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "XML file (*.xml)|*.xml";
            DialogResult result= save.ShowDialog();

            if (result == DialogResult.OK)
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create(save.FileName,settings);
                writer.WriteStartElement("Map");
                foreach (Planet p in planets)
                {
                    writer.WriteStartElement("Planet");

                    writer.WriteElementString("ID", p.ID.ToString());
                    writer.WriteElementString("Owner", p.Owner.ToString());
                    writer.WriteElementString("Forces", p.Forces.ToString());
                    writer.WriteElementString("Growth", p.Growth.ToString());
                    writer.WriteElementString("GrowthCooldown", p.GrowthCounter.ToString());
                    writer.WriteElementString("Size", p.PlanetSize.ToString());

                    writer.WriteStartElement("Position");
                    writer.WriteElementString("X", p.Position.X.ToString());
                    writer.WriteElementString("Y", p.Position.Y.ToString());
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
                writer.WriteFullEndElement();
                writer.Flush();
                writer.Close();
            }
        }
        private void btn_Update_Click(object sender, EventArgs e)
        {
            ReDraw();
        }
        private void btn_Delete_Click(object sender, EventArgs e)
        {
            if (PlanetProperty.SelectedObject is Planet)
            {
                planets.Remove((Planet)PlanetProperty.SelectedObject);
                PlanetProperty.SelectedObject = null;
                ReDraw();
            }
        }
    }
}
