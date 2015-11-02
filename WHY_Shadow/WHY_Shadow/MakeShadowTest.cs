using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//
using Microsoft.Kinect;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.CPlusPlus;

namespace WHY_Shadow
{
    class MakeShadowTest:IProccesingImage
    {
        //宣言
        #region
        ShadowPackage package;
        //画像情報
        int sizeofMat;
        int imgW;
        int imgH;
        byte[] bodyIndex;
        //各人の情報
        struct _playrer
        {
            public Mat mat;
            public int ID;
            public Point bodyPt;
            public Joint basePt;
        }
        _playrer[] players;
        Mat[] mats;
        Mat[] dstMats;
        Scalar[] shadowCalor;
        Random rnd;
        Point[] pt;  
        Point3f[] ptScreen;
        Point3f center;
        Point3f moveCenter;
        Vec3f vecofCenter;
        KinectSensor kinect;
        Joint[] joints;
        //フィールドの情報
        int Xmax;
        int Ymax;
        int Zmax;
        Parameter param;
        #endregion
        //影の名前
        public String GetShadowName()
        {
            return "Maho Shadwo";
        }
        public MakeShadowTest()
        {
            this.sizeofMat = 6;
            this.package = new ShadowPackage();
            this.rnd = new Random();
            this.center = new Point3f();
            this.shadowCalor = new Scalar[sizeofMat];
            this.players = new _playrer[sizeofMat];
            this.pt = new Point[sizeofMat];
            this.kinect = KinectSensor.GetDefault();
            this.joints = new Joint[this.sizeofMat];
            this.moveCenter = new Point3f();
            this.vecofCenter = new Vec3f();
            this.param = new Parameter();
        }
        public void SetOriginalImage(ShadowPackage srcPacage)
        {   
            this.package = srcPacage;
            this.imgW = this.package.srcMat.Width;
            this.imgH = this.package.srcMat.Height;
            this.mats = new Mat[this.sizeofMat];
            this.dstMats = new Mat[this.sizeofMat];
            for (int i = 0; i < this.sizeofMat; i++)
            {
                this.center.X = 0;
                this.center.Y = 0;
                this.center.Z = 0;
                this.mats[i] = new Mat(this.package.srcMat.Height, this.package.srcMat.Width, MatType.CV_8UC1);
                this.dstMats[i] = new Mat(this.package.srcMat.Height, this.package.srcMat.Width, MatType.CV_8UC3);
                this.players[i].mat = new Mat(this.package.srcMat.Height, this.package.srcMat.Width, MatType.CV_8UC1);
            }

        }
        //加工
        public void Processing()
        {  
            int channel = 1;
            int depth = this.package.srcMat.Depth();
            OpenCvSharp.CPlusPlus.Point[] PtsList;
            
            #region
            //骨格を取得した時のみ加工実行
            if (this.package.K_bodies != null)
            {
                //追跡している腰骨格の取得
                this.joints = this.BodyPosition(this.package.K_bodies).ToArray<Joint>();
                //中心の計算とスクリーン座標ptscreen取得
                #region
                for (int i = 0; i < joints.Length; i++)
                {
                    //合算する
                    this.center.X += joints[i].Position.X;
                    this.center.Y += joints[i].Position.Y;
                    this.center.Z += joints[i].Position.Z;
                    //スクリーン点を取得
                    var point = this.kinect.CoordinateMapper.MapCameraPointToDepthSpace(joints[i].Position);
                    this.pt[i].X = (int)point.X;
                    this.pt[i].Y = (int)point.Y;
                }
                //平均をとる
                this.center.X /= joints.Length;
                this.center.Y /= joints.Length;
                this.center.Z /= joints.Length;
                #endregion

                //画像を作る
                #region
                unsafe
                {
                    byte*[] Ptr = new byte*[this.sizeofMat];
                    for (int i = 0; i < this.sizeofMat; i++)
                    {
                        //二値化
                        Ptr[i] = this.mats[i].DataPointer;
                        #region
                        for (int j = 0; j < this.package.K_bodyIndexBuffer.Length; j++)
                        {
                            if (this.package.K_bodyIndexBuffer[j] == i)
                            {
                                *(Ptr[i] + j * channel) = 255;
                            }
                            else
                            {
                                *(Ptr[i] + j * channel) = 0;
                            }
                        }
                        #endregion
                        //ボーンの位置確認 IDに割り当て（ボーン基準）              
                        #region
                        for (int j = 0; j < this.joints.Length; j++)
                        {
                            int point = this.pt[j].Y * this.imgW + this.pt[j].X;
                            try
                            {
                                if (this.package.K_bodyIndexBuffer[point] == i)
                                {
                                    this.players[j].mat = mats[i];
                                    this.players[j].bodyPt = pt[j];
                                    this.players[j].basePt = joints[j];
                                    this.players[j].ID = j;
                                    //Console.Write("get" + j + "\n");
                                }
                            }
                            catch { }
                            
                        }
                       
                        #endregion
                    }               
                }
                #endregion

                //輪郭を円で表示
                #region
                for (int i = 0; i < this.joints.Length; i++)
                {
                    //色と決定
                    float b, g, r;
                    #region
                    if (this.joints.Length == 1)
                    {
                        b = this.players[0].basePt.Position.X * 1000;
                        g = this.players[0].basePt.Position.Y * 1000;
                        r = this.players[0].basePt.Position.Z * 1000;
                    }
                    else
                    {
                        b = (Math.Abs(this.center.X - this.players[i].basePt.Position.X) * 1000);
                        g = (Math.Abs(this.center.Y - this.players[i].basePt.Position.Y) * 1000);
                        r = (Math.Abs(this.center.Z - this.players[i].basePt.Position.Z) * 1000);
                    }
                    #endregion
                    //粒子の方向の決定
                    #region
                    int range = (int) (10* Math.Sqrt((this.center.X - this.joints[i].Position.X) * (this.center.X - this.joints[i].Position.X) +
                                          (this.center.Y - this.joints[i].Position.Y) * (this.center.Y - this.joints[i].Position.Y) +
                                          (this.center.Z - this.joints[i].Position.Z) * (this.center.Z - this.joints[i].Position.Z)));
                    #endregion
                    //描画
                    #region
                    PtsList = this.FindContFromMat(255, this.players[i].mat, imgW).ToArray<OpenCvSharp.CPlusPlus.Point>();
                    for (int j = 0; j < PtsList.Length; j++)
                    {
                        Cv.Circle(dstMats[0].ToCvMat(),
                                   new CvPoint(PtsList[j].X + rnd.Next(-10,range), PtsList[j].Y + rnd.Next(-10,range)),
                                   2,
                                   new Scalar(g,b,r),
                                   -1);
                    }
                    #endregion
                    Console.Write(range+"\n");
                }    
                #endregion
            }
            #endregion
            dstMats[0].BoxFilter(depth, new Size(15, 15), new Point(-1, -1), true);
            //dstMats[0].Blur(new Size(20, 20), new Point(-1, -1));
            this.package.srcMat = (this.dstMats[0]);
        }

        //parametarformの表示非表示
        public void CloseForm()
        {
            if(this.param != null)
            this.param.Close();
        }
        public void ShowForm()
        {
            if (this.param.IsDisposed )
            {
                this.param = new Parameter();
                this.param.Show();
                
            }
            else
            {
                this.param.Show();
            }
        }

        //境界線を探す    
        List<OpenCvSharp.CPlusPlus.Point> FindContFromMat(int id, Mat mat, int imgW)
        {
            List<OpenCvSharp.CPlusPlus.Point> Pts = new List<OpenCvSharp.CPlusPlus.Point>();
            int length = mat.Width * mat.Height;
            unsafe
            {
                byte* matPtr = mat.DataPointer;
                #region
                for (int i = 1; i < (length - 1); i++)
                {
                    if (*(matPtr+ i) == id)
                    {
                        if (i < imgW)
                        {
                            if (*(matPtr + i - 1) != id || *(matPtr + i + 1) != id || *(matPtr + i + imgW) != id)
                            {
                                Pts.Add(new OpenCvSharp.CPlusPlus.Point(i % imgW, (int)i / imgW));
                            }
                        }
                        else if (i > (length - imgW))
                        {
                            if (*(matPtr + i - 1) != id || *(matPtr + i + 1) != id || *(matPtr + i - imgW) != id )
                            {
                                Pts.Add(new OpenCvSharp.CPlusPlus.Point(i % imgW, (int)i / imgW));
                            }
                        }
                        else
                        {
                            if (*(matPtr + i - 1) != id || *(matPtr + i + 1) != id || *(matPtr + i - imgW) != id  || *(matPtr + i + imgW) != id)
                            {
                                Pts.Add(new OpenCvSharp.CPlusPlus.Point(i % imgW, (int)i / imgW));
                            }
                        }

                    }
                }
                #endregion

            }
            
            return Pts;
        }
        //関節位置
        List<Joint> BodyPosition(Body[] bodies)
        {
            List<Joint> joints = new List<Joint>();
            //Point3f pt = new Point3f();
            int i = 0 ;
            foreach (var body in bodies.Where(b => b.IsTracked) )//
            {
                foreach (var joint in body.Joints)
                {
                    if (joint.Value.JointType == JointType.SpineBase)
                    {
                        //var position = this.kinect.CoordinateMapper.MapCameraPointToDepthSpace(joint.Value.Position);
                        //pt.X = joint.Value.Position.X;;
                        //pt.Y = joint.Value.Position.X;
                        //pt.Z = joint.Value.Position.X;
                        //Console.Write("hit\n");
                        joints.Add(joint.Value);
                        i++;
                    }
                }
            }
            return joints;
        }
        //画像の取得
        public ShadowPackage GetProccesedImage()
        {
            //ShadowPackage package = new ShadowPackage();

            return this.package;
        }
    }
}
