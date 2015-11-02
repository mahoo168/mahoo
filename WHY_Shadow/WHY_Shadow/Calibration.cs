using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//追加
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace WHY_Shadow
{
    //
    delegate System.Windows.Point[] GetEllipsePositionForCalib(int i);

    /// <summary>
    /// Calibration main
    /// </summary>
    class Calibration
    {
        //宣言
        #region
        int imageWidth;
        int imageHeight;
        int canvasWidth;
        int canvasHeight;
        int imgChannles;
        Mat backDstImg;
        Mat floorDstImg;
        int fragCutImg;
        CvPoint2D32f[] floorIn_Pt;
        CvPoint2D32f[] floorOut_Pt;
        CvPoint2D32f[] backIn_Pt;
        CvPoint2D32f[] backOut_Pt;
        //インスタンス
        CutRect cutRect;
        //イベント
        public event GetEllipsePositionForCalib _getPtForCalib;
        #endregion

        //コンストラクタ
        public Calibration(int canvasW, int canvasH)
        {

            this.canvasWidth = canvasW;
            this.canvasHeight = canvasH;
            
            this.fragCutImg = 1;
            this.floorIn_Pt = new CvPoint2D32f[4];
            this.floorOut_Pt = new CvPoint2D32f[4];
            this.backIn_Pt = new CvPoint2D32f[4];
            this.backOut_Pt = new CvPoint2D32f[4];

            this.backDstImg = new Mat();
            this.floorDstImg = new Mat();
        }


        //キャリブレーション
        public void CaliblationUpdate(ShadowPackage proccesedMatPckage)
        {
            Mat srcImg = new Mat();
            
            //入力画像
            srcImg = proccesedMatPckage.srcMat.Clone();
            
            this.imgChannles = srcImg.Channels();
            #region
            if (this.fragCutImg == 1)
            {
                this.cutRect = new CutRect(srcImg);
                this.fragCutImg = 0;
                this.imageWidth = srcImg.Width;
                this.imageHeight = srcImg.Height;
            }
            #endregion


            //座標の取得
            this.backIn_Pt = this.changePt(this._getPtForCalib(0));
            this.backOut_Pt = this.changePt(this._getPtForCalib(1));
            this.floorIn_Pt = this.changePt(this._getPtForCalib(2));
            this.floorOut_Pt = this.changePt(this._getPtForCalib(3));
            this.backOut_Pt = this.changePtRange(this.backOut_Pt);
            this.floorOut_Pt = this.changePtRange(this.floorOut_Pt);
           

            //back
            #region
            
            this.backIn_Pt = this.changePtRange(this.backIn_Pt);
            this.backDstImg = this.PerspectiveProject(srcImg, this.backIn_Pt, this.backOut_Pt).Clone();
            this.backDstImg = cutRect.CutImage(this.backDstImg,this.backOut_Pt).Clone();
            #endregion

            //floor
            #region
            
            this.floorIn_Pt = this.changePtRange(this.floorIn_Pt);
            this.floorDstImg = this.PerspectiveProject(srcImg, this.floorIn_Pt, this.floorOut_Pt).Clone();
            this.floorDstImg = cutRect.CutImage(this.floorDstImg, this.floorOut_Pt).Clone();
            #endregion

            srcImg.Dispose();

        }

        //キャリブレーション後の画像を返す
        public ShadowPackage GetCalibedBackImage()
        {
            ShadowPackage backPackage = new ShadowPackage();
            backPackage.srcMat = this.backDstImg;
            return backPackage;
        }

        public ShadowPackage GetCalibedFloorImage()
        {
            ShadowPackage floorPackage = new ShadowPackage();
            floorPackage.srcMat = this.floorDstImg;
            return floorPackage;
        }

        //透視変換
        public Mat PerspectiveProject(Mat srcImg, CvPoint2D32f[] src_Pt, CvPoint2D32f[] dst_Pt)
        {
            Mat dstImg = new Mat();
            dstImg = srcImg.Clone();
            //透視変換
            CvMat perspective_matrix = Cv.GetPerspectiveTransform(src_Pt, dst_Pt);
            Cv.WarpPerspective(srcImg.ToCvMat(), dstImg.ToCvMat(), perspective_matrix, Interpolation.Cubic, new CvScalar(255, 0, 0));

            return dstImg;
        }

        //座標系変換
        CvPoint2D32f[] changePt(System.Windows.Point[] Pt)
        {
            CvPoint2D32f[] dstPt = new CvPoint2D32f[Pt.Length];
            for (int i = 0; i < Pt.Length; i++)
            {
                dstPt[i].X = (int)Pt[i].X;
                dstPt[i].Y = (int)Pt[i].Y;
            }
            return dstPt;
        }
        CvPoint2D32f[] changePtRange(CvPoint2D32f[] srcPt)
        {
            CvPoint2D32f[] dstPt = new CvPoint2D32f[srcPt.Length];

            for (int i = 0; i < srcPt.Length; i++)
            {
                dstPt[i] = new CvPoint2D32f(srcPt[i].X * this.imageWidth / this.canvasWidth,
                                               srcPt[i].Y * this.imageHeight / this.canvasHeight);
            }
                return dstPt;
        }

        //合成
        public Mat SumMat(Mat srcMat_0, Mat srcMat_1,bool Is)
        {
            Mat dstMat = srcMat_0.Clone();
            unsafe
            {
                byte* srcPtr_0 = srcMat_0.DataPointer;
                byte* srcPtr_1 = srcMat_1.DataPointer;
                byte* dstPtr = dstMat.DataPointer;

                for (int i = 0; i < this.imageHeight * this.imageWidth; i++)
                {
                    if (Is)
                    {
                        if ((*(srcPtr_0 + i * this.imgChannles) == 0) && (*(srcPtr_1 + i * this.imgChannles) == 0))
                        {
                            for (int j = 0; j < this.imgChannles; j++)
                            {
                                *(dstPtr + i * this.imgChannles + j) = 0;
                            }
                        }
                        else if (*(srcPtr_0 + i * this.imgChannles) != 0)
                        {
                            for (int j = 0; j < this.imgChannles; j++)
                            {
                                *(dstPtr + i * this.imgChannles + j) = *(srcPtr_0 + i * this.imgChannles + j);
                            }
                        }
                        else if (*(srcPtr_1 + i * this.imgChannles) != 0)
                        {
                            for (int j = 0; j < this.imgChannles; j++)
                            {
                                *(dstPtr + i * this.imgChannles + j) = *(srcPtr_1 + i * this.imgChannles + j);
                            }
                        }

                    }
                    else
                    {
                        if ((*(srcPtr_0 + i * this.imgChannles) > 200) && (*(srcPtr_1 + i * this.imgChannles) > 200))
                        {
                            for (int j = 0; j < this.imgChannles; j++)
                            {
                                *(dstPtr + i * this.imgChannles + j) = 255;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < this.imgChannles; j++)
                            {
                                *(dstPtr + i * this.imgChannles + j) = 0;
                            }
                        }
                    }
                    
                    
                    

                }
            }
            return dstMat;
        }

    }


    /// <summary>
    /// 画像切抜関数
    /// </summary>
    class CutRect
    {
        //メンバ
        Mat srcMat;
        Mat dstMat;
        Mat countMat;

        int imgWidth;
        int imgHeight;
        int channels;
        int imgDepth;
        List<OpenCvSharp.CPlusPlus.Point> CvPoints;
        //イベント
        //public event ZerosMat _zerosMat;

        public CutRect(Mat srcImg)
        {
            //加工画像の情報
            this.srcMat = new Mat();
            this.srcMat = srcImg.Clone();
            this.imgWidth =  this.srcMat.Width;
            this.imgHeight =  this.srcMat.Height;
            this.channels = this.srcMat.Channels();
            this.imgDepth = this.srcMat.Depth();
            //出力用画像
            this.dstMat = new Mat(this.imgHeight, this.imgWidth, MatType.MakeType(this.imgDepth, this.channels));
            this.countMat = new Mat(this.imgHeight, this.imgWidth, MatType.CV_8UC1);
            
        }

        public Mat CutImage(Mat srcImg, CvPoint2D32f[] srcPt)
        {
            try
            {
                this.srcMat = srcImg.Clone();



                this.CvPoints = new List<OpenCvSharp.CPlusPlus.Point>();
                for (int i = 0; i < srcPt.Length; i++)
                {
                    this.CvPoints.Add(new OpenCvSharp.CPlusPlus.Point(srcPt[i].X, srcPt[i].Y));
                }

                this.countMat.FillConvexPoly(this.CvPoints, new Scalar(255), LineType.Link8, 0);

                //
                unsafe
                {
                    byte* countPtr = this.countMat.DataPointer;
                    byte* srcPtr = this.srcMat.DataPointer;
                    byte* dstPtr = this.dstMat.DataPointer;

                    for (int i = 0; i < (this.imgWidth * this.imgHeight); i++)
                    {
                        if (*(countPtr + i) == 255)
                        {
                            for (int j = 0; j < this.channels; j++)
                            {
                                *(dstPtr + i * this.channels + j) = *(srcPtr + i * this.channels + j);

                            }
                        }
                        else
                        {
                            for (int j = 0; j < this.channels; j++)
                            {
                                *(dstPtr + i * this.channels + j) = 0;

                            }
                        }

                        *(countPtr + i) = 0;
                    }
                }
            }
            catch { }

            
            return this.dstMat;
            

        }
    }
}

