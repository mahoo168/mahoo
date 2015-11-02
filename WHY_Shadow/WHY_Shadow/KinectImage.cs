using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows.Media.Imaging;
//
using OpenCvSharp.CPlusPlus;
using Microsoft.Kinect;
using System.Drawing;


namespace WHY_Shadow
{
    public delegate void ShowImageEventHandler();

    class KinectImage : ICamera
    {
        public event ShowImageEventHandler _showImageEvent;

        //宣言
        #region
        KinectSensor kinect;
        BodyIndexFrameReader bodyIndexFrameReader;     
        FrameDescription bodyIndexFrameDes;
        byte[] bodyIndexBuffer;
        int imageWidth;
        int imageHeight;
        int imageBytePerPixel;
        Int32Rect bitmapRec;
        int bitmapStride;
        Mat kinectImage;
        ShadowPackage kinectImgPackage;
        List<IProccesingImage> processing;
        #endregion 

        //セットアップ
        public KinectImage()
        #region
        {
            //キネクト
            this.kinect = KinectSensor.GetDefault();
            
            //bodyIndexFrameの処理
            this.bodyIndexFrameDes = this.kinect.BodyIndexFrameSource.FrameDescription;
            this.bodyIndexFrameReader = this.kinect.BodyIndexFrameSource.OpenReader();
            this.bodyIndexFrameReader.FrameArrived += this.BodyIndexFrame_Arrived;
            //画像情報
            this.kinectImgPackage = new ShadowPackage();
            this.imageWidth =  this.bodyIndexFrameDes.Width;  // imgW;
            this.imageHeight = this.bodyIndexFrameDes.Height; // imgH;

            this.imageBytePerPixel = (int)this.bodyIndexFrameDes.BytesPerPixel;
            this.bitmapRec = new Int32Rect(0, 0, this.imageWidth, this.imageHeight);
            this.bitmapStride = (int)(this.imageWidth * this.imageBytePerPixel);
           
            this.bodyIndexBuffer = new byte[this.imageWidth *
                                                this.imageHeight * this.imageBytePerPixel];
            this.kinectImage = new Mat(this.imageHeight, this.imageWidth, MatType.CV_8UC1);
            //キネクト開始
            this.kinect.Open();
            
        }
        #endregion


        //frame取得時のイベント
        public void BodyIndexFrame_Arrived(object sender, BodyIndexFrameArrivedEventArgs e)
        #region
        {
            BodyIndexFrame bodyIndexFrame = e.FrameReference.AcquireFrame();
            if (bodyIndexFrame == null) return;
            bodyIndexFrame.CopyFrameDataToArray(bodyIndexBuffer);  //人がいないところ0xff いるところ0-6？
            this.KinectImagetoMat(this.kinectImage, this.bodyIndexBuffer);
            //this._showImageEvent();
            bodyIndexFrame.Dispose();
        }
        #endregion

        public void FrameArrived() { }

        //frameを渡す
        public ShadowPackage GetImage()
        {
            this.kinectImgPackage.srcMat = this.kinectImage;

            return this.kinectImgPackage;
        }

        //bufferをもとに白黒Matデータ作成
        private void KinectImagetoMat(Mat mat, byte[] buffer)
        {
            
            int channel = mat.Channels();
            int depth = mat.Depth();
            unsafe
            {
                byte* matPtr = mat.DataPointer;
                for (int i = 0; i < this.imageWidth * this.imageHeight; i++)
                {
                    if(buffer[i] ==255){
                        for (int j = 0; j < channel;j++){
                             *(matPtr + i * channel + j) = 255;
                        }     
                    }
                    else{
                        for (int j = 0; j < channel; j++)
                        {
                            *(matPtr + i * channel + j) = 0;
                        }   
                    }
                    
                }
            }
        }

        //終了時の処理
        public void OnClose()
        #region
        {
            if (this.bodyIndexFrameReader != null)
            {
                this.bodyIndexFrameReader.Dispose();
                this.bodyIndexFrameReader = null;
            }
            if (this.kinect != null)
            {
                this.kinect.Close();
            }
        }
        #endregion

    }
}

