using FacadeCreatorApi;
using FacadeCreatorApi.models;
using FacadeCreatorApi.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacadeCreator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Scenes scenes = new Scenes(this, null);
            scenes.addFigure(new Facade(1, 1, 600, 2200), 0, 0);
            scenes.addFigure(new Facade(1, 1, 600, 2200), 700, 0);
            scenes.addFigure(new BkgImage(ImageConversion.getImage("C:\\InSitu\\Textures\\images\\595E677B_0002_01162.jpg")), 0, 0);
            scenes.addFigure(new BkgImage(ImageConversion.getImage("C:\\InSitu\\Textures\\images\\595E677B_0002_01162.jpg")), 700, 0);
            scenes.scalingToAllFigureisVisibleMode();
            this.Refresh();
        }
        private void update(object sender, PaintEventArgs e)
        {
            //Graphics graph = e.Graphics;
            //Image img = ImageConversion.inverseBlackWhiteImage(new Bitmap("C:\\InSitu\\Textures\\images\\595E677B_0002_01162.jpg"));
            //graph.DrawImage(img, new Point(0, 0));
            //Graphics graphics = this.CreateGraphics();
            //Pen pen = new Pen(Color.Black, 3);
            //graphics.ScaleTransform(2, 2);           
            //graphics.DrawRectangle(pen, 10, 10, 20, 30);
            //graphics.ScaleTransform(0.1f, 0.1f);
            //    pen.Dispose();
            //graphics.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {

        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {

        }
    }
}
