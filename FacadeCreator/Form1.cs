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
            Scenes scenes = new Scenes(this,null);
            scenes.addFigure(new Facade(1, 1, 600, 2200), 0, 0);
            scenes.addFigure(new BkgImage(new Bitmap("C:\\InSitu\\Textures\\images\\image44.jpg")), 0, 0);
        }
        private void update(object sender, PaintEventArgs e)
        {
            //Graphics graphics = this.CreateGraphics();
            //Pen pen = new Pen(Color.Black, 3);
            //graphics.ScaleTransform(2, 2);           
            //graphics.DrawRectangle(pen, 10, 10, 20, 30);
            //graphics.ScaleTransform(0.1f, 0.1f);
            //    pen.Dispose();
            //graphics.Dispose();
        }

    }
}
