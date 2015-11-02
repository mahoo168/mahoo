using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WHY_Shadow
{
    public partial class ImageWindow : Form
    {
        //宣言
        #region

        #endregion

        public ImageWindow(String name)
        {
            InitializeComponent();
            this.Text = name;
            this.ControlBox = false;
    
            this.imageArea.SizeMode = PictureBoxSizeMode.StretchImage;
            this.imageArea.Size = new Size(this.Width, this.Height);
        }

        public void SetImage(Bitmap image)
        {
            this.imageArea.Image = image;
        }

        public void ShowWindow()
        {
            this.Show();
            this.imageArea.Show();
        }

        public void HideWindow()
        {
            this.WindowState = FormWindowState.Minimized;
        }

        public void ToFullScreen()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.imageArea.Size = new Size(this.Width, this.Height);
        }

        public void ToWindow()
        {
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.WindowState = FormWindowState.Normal;
            this.imageArea.Size = new Size(this.Width, this.Height);
        }

        public void OnClose()
        {
            this.Close();
        }

       

    }

}
