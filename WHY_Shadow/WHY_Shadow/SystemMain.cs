using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
//追加
using Microsoft.Kinect;
using System.Threading;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;
//
using AForge.Video;
using AForge.Video.DirectShow;


namespace WHY_Shadow
{
    //データ構造体
    //画像データ授受の型
    public struct ShadowPackage
    {
        public Mat srcMat;
        public byte[] K_colors;
        public Body[] K_bodies;
        public ushort[] K_DdepthBuffer;
        public byte[] K_bodyIndexBuffer;
        public String cameraKind;
    };

    //イベント
    #region
    delegate void ShowInputImage(Mat inputImage);
    delegate void ShowOutputImage(Mat outputBackImage, Mat outputFloorImage,Mat image);
    delegate ShadowPackage GetCalibedImage();
    delegate void CaliblationUpdate(ShadowPackage proccesedMat);
    delegate Mat Resize(Mat srcImg, CvPoint2D32f[] src_Pt, CvPoint2D32f[] dst_Pt);
    #endregion

    class SystemMain{
        
        //宣言
        #region
        ShadowPackage inputPackage;
        ShadowPackage processedPackage;

        System.Threading.Timer inputimgTimer;
        System.Threading.Timer processimgTimer;
        System.Threading.Timer outputimgTimer;


        RecordedData recKinect ;
        KinectDevice kinectDevice;
        //イベント 
        public event ShowInputImage _showInputImage;
        public event ShowOutputImage _showOutputImage;
        public event GetCalibedImage _getCalibedBackImage;
        public event GetCalibedImage _getCalibedFloorImage;
        public event CaliblationUpdate _calibrationUpdate;
        public event SumMat _sumMat;
        //加工前画像製造クラス
        //KinectImage kinectImage;

        #endregion

        //コンストラクタ
        public SystemMain()
        {
            this.recKinect = new RecordedData();
            this.kinectDevice = new KinectDevice();
            //this.camera._showImageEvent += this.CastingImage;
            this.inputPackage = new ShadowPackage();
            this.processedPackage = new ShadowPackage();

            this.inputimgTimer = new System.Threading.Timer(new TimerCallback(this.CastingImage));
            this.processimgTimer = new System.Threading.Timer(new TimerCallback(this.ProcessImage));
            this.outputimgTimer = new System.Threading.Timer(new TimerCallback(this.Calibration));
            this.inputimgTimer.Change(0,100);
            this.processimgTimer.Change(0,100);
            this.outputimgTimer.Change(0,100);
        }

        //画像の表示とキャリブレーション
        void CastingImage(object org)
        {
            try
            {
                // 画像の取得
                if(this.kinectDevice.GetImage().srcMat != null)
                {

                    this.inputPackage = this.kinectDevice.GetImage();
                    try
                    {
                        if (this.recKinect.GetImage().srcMat != null)
                        {
                            //Console.WriteLine("LLL");
                            if(this.recKinect.GetImage().srcMat.Size() == this.kinectDevice.GetImage().srcMat.Size())
                                this.inputPackage.srcMat = this._sumMat(this.recKinect.GetImage().srcMat, this.kinectDevice.GetImage().srcMat,false);
                          
                            
                        }
                    }
                    catch { }

                    this._showInputImage(this.inputPackage.srcMat);



                }

                //Console.WriteLine("img");
            }
            catch { }
            
        }

        void ProcessImage(object orgs)
        {
            try
            {
                if(this.inputPackage.srcMat != null)
                {
                    //画像の加工
                    this.processedPackage = this.inputPackage;
                    //this.shadows.ElementAt(this.shadowIndex).SetOriginalImage(this.inputPackage);
                    //this.shadows.ElementAt(this.shadowIndex).Processing();
                }

            }
            catch { }
        }

        void Calibration(object orgs)
        {
            try
            {
                if (this.processedPackage.srcMat != null)
                {
                    //Console.WriteLine("oooo");
                    //キャリブレーションして表示
                    this._calibrationUpdate(this.processedPackage);
                    this._showOutputImage(this._getCalibedBackImage().srcMat, this._getCalibedFloorImage().srcMat, this.inputPackage.srcMat);
                }
            }
            catch { }
        }

        //終了時の処理
        public void OnClose()
        {
            //kinectの終了
            this.recKinect.OnClose();
            this.kinectDevice.OnClose();
            
        }
       
    }
}
