using FacadeCreatorApi;
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
            EVD.Plugin Evd = new EVD.Plugin();
            Evd.OnAppStartAfter(123);
            Scenes scenes = new Scenes(this);
            
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
