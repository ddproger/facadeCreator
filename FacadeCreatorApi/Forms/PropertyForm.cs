using FacadeCreatorApi.models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FacadeCreatorApi.Forms
{
    public partial class PropertyForm : Form
    {
        private Figure figure;
        public int width
        {
            get
            {
                int val = 0;
                Int32.TryParse(txtWidth.Text, out val);
                return val;                
            }
        }
        public int height
        {
            get
            {
                int val = 0;
                Int32.TryParse(txtHeight.Text, out val);
                return val;
            }
        }
        public PropertyForm()
        {
            InitializeComponent();
        }
        public PropertyForm(Figure fig)
        {
            InitializeComponent();
            figure = fig;
            txtHeight.Text = fig.height.ToString();
            txtWidth.Text = fig.width.ToString();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (height <= 0 || height > 5000)
            {
                MessageBox.Show("Высота не находится в диапазоне от 1 до 5000");
                return;
            }else if(width<=0||width>5000)
            {
                MessageBox.Show("Ширина не находится в диапазоне от 1 до 5000");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtWidthKeyUped(object sender, KeyEventArgs e)
        {
            if (chSavingResolution.Checked && width != 0)
            {
                txtHeight.Text = ((Int32)((width+0.5) * figure.getResolution())).ToString();
            }
        }
        private void txtHeightKeyUped(object sender, KeyEventArgs e)
        {
            if (chSavingResolution.Checked && height != 0)
            {
                txtWidth.Text = ((Int32)((height+0.5) / figure.getResolution())).ToString();
            }
        }
    }
}
