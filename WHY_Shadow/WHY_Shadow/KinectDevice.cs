using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using OpenCvSharp.CPlusPlus;
using Microsoft.Kinect;
using System.Drawing;

namespace WHY_Shadow
{   

    public class KinectDevice
    {
        public event ShowImageEventHandler _showImageEvent;
        
        //宣言とインスタンス
        #region
        KinectSensor kinect;
        //カラーイメージ
        ColorImageFormat colorImageFormat;
        ColorFrameReader colorFrameReader;
        FrameDescription colorFrameDescription;
        byte[] colors;
        //骨格情報
        BodyFrameReader bodyFrameReader;
        //深度情報
        DepthFrameReader depthFrameReader;
        FrameDescription depthFrameDescription;
        ushort[] depthBuffer;
        //BodyIndex
        BodyIndexFrameReader bodyIndexFrameReader;
        FrameDescription bodyIndexFrameDes;
        byte[] bodyIndexBuffer;
        //データ格納
        ShadowPackage package;
        Mat kinectImage;
        int imageWidth;
        int imageHeight;
        int imageBytePerPixel;
        #endregion

        public KinectDevice()
        {
            //kinect設定
            this.kinect = KinectSensor.GetDefault();
            //設定とハンドラ
            //colorImage
            #region
            this.colorImageFormat = ColorImageFormat.Bgra;
            this.colorFrameDescription = this.kinect.ColorFrameSource.CreateFrameDescription(this.colorImageFormat);
            this.colorFrameReader = this.kinect.ColorFrameSource.OpenReader();
            this.colorFrameReader.FrameArrived += ColorFrame_Arrived;
            this.colors = new byte[this.colorFrameDescription.Width
                                           * this.colorFrameDescription.Height
                                           * this.colorFrameDescription.BytesPerPixel];
            #endregion
            //骨格情報
            #region
            this.bodyFrameReader = this.kinect.BodyFrameSource.OpenReader();
            this.bodyFrameReader.FrameArrived += BodyFrame_Arrived;
            #endregion
            //震度情報
            #region
            this.depthFrameReader = this.kinect.DepthFrameSource.OpenReader();
            this.depthFrameReader.FrameArrived += DepthFrame_Arrived;
            this.depthFrameDescription = this.kinect.DepthFrameSource.FrameDescription;
            this.depthBuffer = new ushort[this.depthFrameDescription.LengthInPixels];
            #endregion
            //BodyIndex
            #region
            this.bodyIndexFrameDes = this.kinect.BodyIndexFrameSource.FrameDescription;
            this.bodyIndexFrameReader = this.kinect.BodyIndexFrameSource.OpenReader();
            this.bodyIndexFrameReader.FrameArrived += this.BodyIndexFrame_Arrived;
            this.bodyIndexBuffer = new byte[this.bodyIndexFrameDes.Width *
                                                this.bodyIndexFrameDes.Height * this.bodyIndexFrameDes.BytesPerPixel];
            #endregion
            //kinect開始
            this.package = new ShadowPackage();
            this.imageWidth = this.bodyIndexFrameDes.Width; 
            this.imageHeight = this.bodyIndexFrameDes.Height; 
            this.imageBytePerPixel = (int)this.bodyIndexFrameDes.BytesPerPixel;
            this.kinectImage = new Mat(this.imageHeight, this.imageWidth, MatType.CV_8UC1);
            this.kinect.Open();
        }

        //カラーイメージ取得時のイベント
        void ColorFrame_Arrived(object sender, ColorFrameArrivedEventArgs e)
        {
            ColorFrame colorFrame = e.FrameReference.AcquireFrame();
            //フレームがなければ終了、あれば格納
            if (colorFrame == null) return;
            colorFrame.CopyConvertedFrameDataToArray(this.colors, this.colorImageFormat);
            this.package.K_colors = this.colors;
            //破棄
            colorFrame.Dispose();
        }

        //骨格情報取得時のイベント
        void BodyFrame_Arrived(object sender, BodyFrameArrivedEventArgs e)
        {
            BodyFrame bodyFrame = e.FrameReference.AcquireFrame();
            if (bodyFrame == null) return;
            Body[] bodies = new Body[this.kinect.BodyFrameSource.BodyCount]; //bodycountに骨格情報の数
            bodyFrame.GetAndRefreshBodyData(bodies);
            this.package.K_bodies = bodies;
            //破棄
            bodyFrame.Dispose();
        }   

        //深度情報
        void DepthFrame_Arrived(object sender, DepthFrameArrivedEventArgs e)
        {
            DepthFrame depthFrame = e.FrameReference.AcquireFrame();
            //フレームがなければ終了、あれば格納
            if (depthFrame == null) return;
            int[] depthBitdata = new int[depthBuffer.Length];
            depthFrame.CopyFrameDataToArray(this.depthBuffer);
            this.package.K_DdepthBuffer = this.depthBuffer;
            //破棄
            depthFrame.Dispose();
        }

        //bodyIndexframe取得時のイベント
        void BodyIndexFrame_Arrived(object sender, BodyIndexFrameArrivedEventArgs e)
        {
            BodyIndexFrame bodyIndexFrame = e.FrameReference.AcquireFrame();
            if (bodyIndexFrame == null) return;
            bodyIndexFrame.CopyFrameDataToArray(bodyIndexBuffer);  //人がいないところ0xff いるところ0-6？
            this.package.K_bodyIndexBuffer = this.bodyIndexBuffer;
            this.KinectImagetoMat(this.kinectImage, this.bodyIndexBuffer);
            //this._showImageEvent();
            //破棄
            bodyIndexFrame.Dispose();

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
                    if (buffer[i] == 255)
                    {
                        for (int j = 0; j < channel; j++)
                        {
                            *(matPtr + i * channel + j) = 255;
                        }
                    }
                    else
                    {
                        for (int j = 0; j < channel; j++)
                        {
                            *(matPtr + i * channel + j) = 0;
                        }
                    }

                }
            }
        }

        public ShadowPackage GetImage()
        {
            this.package.srcMat = this.kinectImage.Clone();
            return this.package;
        }


        //終了時の処理
        public void OnClose()
        {
            //カラーリーダーの終了
            if (this.colorFrameReader != null)
            {
                this.colorFrameReader.Dispose();
                this.colorFrameReader = null;
            }
            //ボディリーダーの終了
            if (this.bodyFrameReader != null)
            {
                this.bodyFrameReader.Dispose();
                this.bodyFrameReader = null;
            }
            //ディプスリーダーの終了
            if (this.depthFrameReader != null)
            {
                this.depthFrameReader.Dispose();
                this.depthFrameReader = null;
            }
            //BodyIndexFrame
            if (this.bodyIndexFrameReader != null)
            {
                this.bodyIndexFrameReader.Dispose();
                this.bodyIndexFrameReader = null;
            }
            //キネクトの終了
            if (this.kinect != null)
            {
                this.kinect.Close();
            }
        }

    }

        
}
