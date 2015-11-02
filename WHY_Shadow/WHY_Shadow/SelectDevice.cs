using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//
using AForge.Video;
using AForge.Video.DirectShow;

namespace WHY_Shadow
{
    public delegate void EventHandler_0(VideoCaptureDevice device);
    public delegate void EventHandler_1();

    public partial class SelectDevice : Form
    {

        //宣言
        #region
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoDevice;
        VideoCapabilities[] videoCapabilities;
        public event EventHandler_0 _changeCamera;
        public event EventHandler_1 _changeKinect;
        int cameraDeviceNum;
        #endregion

        public SelectDevice()
        {
            InitializeComponent();
            this.cameraDeviceNum = 0;
            #region
            this.videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            
            if (this.videoDevices.Count != 0)
            {
                //リストクリア
                this.listDevices.Items.Clear();
                //デバイス追加
                foreach (FilterInfo device in videoDevices)
                {
                    this.listDevices.Items.Add(device.Name);
                    this.cameraDeviceNum++;
                }
                this.listDevices.Items.Add("kinect");
                this.listDevices.SelectedIndex = 0;
            }
            else
            {
                this.listDevices.Items.Clear();
                this.listDevices.Items.Add("Kinect");
                this.listDevices.SelectedIndex = 0;
            }
            #endregion
            this.Show();

        }


        private void button1_Click(object sender, EventArgs e)
        {
              if(this.cameraDeviceNum < this.listDevices.SelectedIndex || this.cameraDeviceNum ==0){
                  this._changeKinect();
              }
              else
              {
                  this.videoDevice = new VideoCaptureDevice(videoDevices[this.listDevices.SelectedIndex].MonikerString);
                  this._changeCamera(this.videoDevice);
              }                  
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //videoDevice = new VideoCaptureDevice(videoDevices[this.listDevices.SelectedIndex].MonikerString);
            //videoCapabilities = videoDevice.VideoCapabilities;
        }
    
    }
}
