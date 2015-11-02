using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//追加
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using OpenCvSharp.Extensions;

namespace WHY_Shadow
{
    //
    delegate void EventHandler();
    delegate Mat SumMat(Mat srcMat_0, Mat srcMat_1, bool Is);
    //モード
    public enum CaliblationMode
    {
        back, floor, calibration,
    };
    public enum WSCSmode
    {
        wscs2, wscs3,
    };
    public enum InOutPutMode
    {
        input, output,
    }
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        //宣言
        #region
        //モード
        CaliblationMode caliblationMode;
        WSCSmode wscsMode;
        //変数
        System.Windows.Point[] backPt_In;
        System.Windows.Point[] backPt_Out;
        System.Windows.Point[] floorPt_In;
        System.Windows.Point[] floorPt_Out;
        int ellipseR;
        Ellipse[] InputE;
        Ellipse[] OutputE;
        //インスタンス
        //フォーム
        ImageWindow[] imageForms;
        ImageWindow wscs2;
        ImageWindow wscs3_0;
        ImageWindow wscs3_1;
        //機能
        SystemMain systemMain;
        Calibration calibration;
        //イベント
        private event EventHandler _OnClose;
        private event EventHandler _ToFullScreen;
        private event EventHandler _ToWindow;
        private event EventHandler _ShowWindow;
        private event EventHandler _HideWindow;
        private event SumMat _sumMat;
        #endregion

        //コンストラクタ
        public MainWindow()
        {
            InitializeComponent();

            this.ellipseR = 5;
            this.InputE = new Ellipse[4];
            this.OutputE = new Ellipse[4];
            //インスタンス
            #region
            this.systemMain = new SystemMain();
            this.calibration = new Calibration((int)this.OutputImage.Width, (int)this.OutputImage.Height);
            #endregion
            //window 追加はここから
            #region
            this.imageForms = new ImageWindow[3];
            this.imageForms[0] = (this.wscs2 = new ImageWindow("wscs2"));
            this.imageForms[1] = (this.wscs3_0 = new ImageWindow("wscs3_back"));
            this.imageForms[2] = (this.wscs3_1 = new ImageWindow("wscs3_floor"));
            #endregion
            //Ellipse　追加はここから
            #region
            this.InputE[0] = this.InputE_0;
            this.InputE[1] = this.InputE_1;
            this.InputE[2] = this.InputE_2;
            this.InputE[3] = this.InputE_3;

            this.OutputE[0] = this.OutputE_0;
            this.OutputE[1] = this.OutputE_1;
            this.OutputE[2] = this.OutputE_2;
            this.OutputE[3] = this.OutputE_3;
            //Pt初期化
            #region
            this.backPt_In = new System.Windows.Point[this.InputE.Length];
            this.backPt_Out = new System.Windows.Point[this.OutputE.Length];
            this.floorPt_In = new System.Windows.Point[this.InputE.Length];
            this.floorPt_Out = new System.Windows.Point[this.OutputE.Length];
            this.backPt_In = this.GetEllipsePosition(InOutPutMode.input);
            this.floorPt_In = this.backPt_In;
            this.backPt_Out = this.GetEllipsePosition(InOutPutMode.output);
            this.floorPt_Out = this.backPt_Out;
            #endregion
            #endregion
            //イベント登録 
            this._sumMat += this.calibration.SumMat;
            //SystemMain
            #region
            this.systemMain._calibrationUpdate += this.calibration.CaliblationUpdate;
            this.systemMain._getCalibedBackImage += this.calibration.GetCalibedBackImage;
            this.systemMain._getCalibedFloorImage += this.calibration.GetCalibedFloorImage;
            this.systemMain._showInputImage += this.SetInputImage;
            this.systemMain._showOutputImage += this.SetOutputImage;
            this.systemMain._sumMat += this.calibration.SumMat;
            #endregion
            //Calibration
            #region
            this.calibration._getPtForCalib += this.GetEllipsePositionForCalib;
            #endregion
            //window
            #region
            for (int i = 0; i < this.imageForms.Length; i++)
            {
                this._OnClose += this.imageForms[i].OnClose;
            }
            for (int i = 0; i < this.imageForms.Length; i++)
            {
                this._ToFullScreen += this.imageForms[i].ToFullScreen;
            }
            for (int i = 0; i < this.imageForms.Length; i++)
            {
                this._ToWindow += this.imageForms[i].ToWindow;
            }
            for (int i = 0; i < this.imageForms.Length; i++)
            {
                 this._ShowWindow += this.imageForms[i].ShowWindow;
                
            }
            for (int i = 0; i < this.imageForms.Length; i++)
            {
                this._HideWindow += this.imageForms[i].HideWindow;
            }
            #endregion
            //shadowlist
            //this.MakeShadowList(this.systemMain.ShadowList);
            this.wscsMode = WSCSmode.wscs3;
            this.DrawLine(this.GetEllipsePosition(InOutPutMode.input), InOutPutMode.input);
            this.DrawLine(this.GetEllipsePosition(InOutPutMode.output), InOutPutMode.output);

        }

        //画像の表示
        void SetInputImage(Mat inputImage)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                this.InputImage.Source = WriteableBitmapConverter.ToWriteableBitmap(inputImage);
                //this.DrawLine(this.GetEllipsePosition(InOutPutMode.input), InOutPutMode.input);

            }));
        }
        void SetOutputImage(Mat outputImage_0, Mat outputImage_1, Mat Image)
        {
            
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                //this.DrawLine(this.GetEllipsePosition(InOutPutMode.output), InOutPutMode.output);

                //windowの画像設定 ここから
                #region
                if (this.caliblationMode == CaliblationMode.back) this.wscs2.SetImage(outputImage_0.ToBitmap());
                else if (this.caliblationMode == CaliblationMode.floor) this.wscs2.SetImage(outputImage_1.ToBitmap());
                else this.wscs2.SetImage(this._sumMat(outputImage_0,outputImage_1,true).ToBitmap());
                this.wscs3_0.SetImage(outputImage_0.ToBitmap());
                this.wscs3_1.SetImage(outputImage_1.ToBitmap());
                #endregion

            
                switch (this.caliblationMode)
                {
                    case CaliblationMode.back:
                        this.OutputImage.Source = WriteableBitmapConverter.ToWriteableBitmap(outputImage_0); break;
                    case CaliblationMode.floor:
                        this.OutputImage.Source = WriteableBitmapConverter.ToWriteableBitmap(outputImage_1); break;
                    case CaliblationMode.calibration:
                        this.OutputImage.Source = WriteableBitmapConverter.ToWriteableBitmap(Image); break;
                }
            }));
            
        }

        //ボタン関係        
        //モード切替
        #region
        void ChangeCalibrationMode(object sender, RoutedEventArgs e)
        {
            switch (caliblationMode)
            {
                case CaliblationMode.back:
                    caliblationMode = CaliblationMode.floor;
                    this.ToFloor();
                    
                    break;

                case CaliblationMode.floor:
                    caliblationMode = CaliblationMode.calibration;
                    this.SetVisiblity(Visibility.Collapsed);
                    break;

                case CaliblationMode.calibration:
                    caliblationMode = CaliblationMode.back;
                    this.ToBack();
                    this.SetVisiblity(Visibility.Visible);
                    break;

            }
            mode.Text = caliblationMode.ToString();
        }
        void Change_WSCSmode(object sender, RoutedEventArgs e)
        {
            switch (this.wscsMode)
            {
                case WSCSmode.wscs2:
                    //wscs3にする
                    this.wscsMode = WSCSmode.wscs3;
                    break;

                case WSCSmode.wscs3:
                    //wscs2にする
                    this.wscsMode = WSCSmode.wscs2;
                    break;
            }
            WSCS.Text = this.wscsMode.ToString();
        }
        void ToBack()
        {
            
            //Back座標で表示
            this.SetEllipsePositon(this.backPt_In, InOutPutMode.input);
            this.SetEllipsePositon(this.backPt_Out, InOutPutMode.output);
            this.DrawLine(this.backPt_In, InOutPutMode.input);
            this.DrawLine(this.backPt_Out, InOutPutMode.output);
        }
        void ToFloor()
        {
            
            //Floor座標で表示
            this.SetEllipsePositon(this.floorPt_In, InOutPutMode.input);
            this.SetEllipsePositon(this.floorPt_Out, InOutPutMode.output);
            this.DrawLine(this.floorPt_In, InOutPutMode.input);
            this.DrawLine(this.floorPt_Out, InOutPutMode.output);
        }
        void SetVisiblity(Visibility visibility)
        {
            this.InputLine.Visibility = visibility;
            this.OutputLine.Visibility = visibility;
            for (int i = 0; i < this.InputE.Length; i++)
            {
                this.InputE[i].Visibility = visibility;
            }
            for (int i = 0; i < this.OutputE.Length; i++)
            {
                this.OutputE[i].Visibility = visibility;
            }
        }
        #endregion
        //ImageWindow変更
        #region
        void ToFullScreen(object sender, RoutedEventArgs e)
        {
            //windowの動き設定　ここから
            switch (this.wscsMode)
            {
                case WSCSmode.wscs2:
                    this.wscs2.ToFullScreen();
                    this.wscs3_0.HideWindow();
                    this.wscs3_1.HideWindow(); 
                    break;
                case WSCSmode.wscs3:
                    this.wscs2.HideWindow();
                    this.wscs3_0.ToFullScreen();
                    this.wscs3_1.ToFullScreen();
                    break;
            }
            //this._ToFullScreen();
        }
        void ToWindow(object sender, RoutedEventArgs e)
        {
            this._ToWindow();
        }
        void ShowWindow(object sender, RoutedEventArgs e)
        {
            this._ShowWindow();
        }
        void Hide(object sender, RoutedEventArgs e)
        {
            this._HideWindow();
        }
        #endregion
        //Ellpse関係
        //Ellipseの動き
        #region
        bool isDrag = false;
        System.Windows.Point mouseOffset;
        System.Windows.Point elOffset;
        public void Ellipse_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UIElement el = sender as UIElement;
            if (el != null && el.Visibility == Visibility.Visible)
            {
                isDrag = true;
                mouseOffset = Mouse.GetPosition(Main);
                elOffset = new System.Windows.Point(Canvas.GetLeft(el), Canvas.GetTop(el));
                el.CaptureMouse();
            }
        }
        public void Ellipse_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrag)
            {
                UIElement el = sender as UIElement;
                el.ReleaseMouseCapture();
                isDrag = false;            
            }

        }
        public void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDrag)
            {
                System.Windows.Point pt = Mouse.GetPosition(Main);
                UIElement el = sender as UIElement;
                Canvas.SetLeft(el, elOffset.X + pt.X - mouseOffset.X);
                Canvas.SetTop(el, elOffset.Y + pt.Y - mouseOffset.Y);
                switch (this.caliblationMode)
                {
                    case CaliblationMode.back:
                        //Back座標の保存
                        this.backPt_In = this.GetEllipsePosition(InOutPutMode.input);
                        this.backPt_Out = this.GetEllipsePosition(InOutPutMode.output);
                        break;
                    case CaliblationMode.floor:
                        //Floor座標の保存
                        this.floorPt_In = this.GetEllipsePosition(InOutPutMode.input);
                        this.floorPt_Out = this.GetEllipsePosition(InOutPutMode.output);
                        break;
                }
                this.DrawLine(this.GetEllipsePosition(InOutPutMode.input), InOutPutMode.input);
                this.DrawLine(this.GetEllipsePosition(InOutPutMode.output), InOutPutMode.output);
            }
        }
        #endregion
        //Ellipseの座標
        void SetEllipsePositon(System.Windows.Point[] Pt, InOutPutMode mode)
        {
            switch (mode)
            {
                case InOutPutMode.input:
                    for (int i = 0; i < this.InputE.Length; i++)
                    {
                        Canvas.SetLeft(this.InputE[i], Pt[i].X - this.ellipseR); 
                        Canvas.SetTop(this.InputE[i], Pt[i].Y - this.ellipseR);
                    }
                    break;
                case InOutPutMode.output:
                    for (int i = 0; i < this.OutputE.Length; i++)
                    {
                        Canvas.SetLeft(this.OutputE[i], Pt[i].X - this.ellipseR);
                        Canvas.SetTop(this.OutputE[i], Pt[i].Y - this.ellipseR);
                    }
                    break;
                
            }

        }
        System.Windows.Point[] GetEllipsePosition(InOutPutMode mode)
        {
            

            System.Windows.Point[] Pt = new System.Windows.Point[4];
            //this.Dispatcher.BeginInvoke(new Action(() =>
            //{
                switch (mode)
                {
                    case InOutPutMode.input:
                        for (int i = 0; i < this.InputE.Length; i++)
                        {
                            Pt[i] = new System.Windows.Point(Canvas.GetLeft(this.InputE[i]) + this.ellipseR, Canvas.GetTop(this.InputE[i]) + this.ellipseR);
                        }
                        break;
                    case InOutPutMode.output:
                        for (int i = 0; i < this.OutputE.Length; i++)
                        {
                            Pt[i] = new System.Windows.Point(Canvas.GetLeft(this.OutputE[i]) + this.ellipseR, Canvas.GetTop(this.OutputE[i]) + this.ellipseR);
                        }
                        break;
                 }
            //}));
            return Pt;
            
        }
        System.Windows.Point[] GetEllipsePositionForCalib(int i)
        {
            
            switch (i)
            {
                case 0:
                    return this.backPt_In; 
                    break;
                case 1:
                    return this.backPt_Out; 
                    break;
                case 2:
                    return this.floorPt_In; 
                    break;
                case 3:
                    return this.floorPt_Out; 
                    break;
                default :
                    return null;
            }
        }
        //四角形表示       
        public void DrawLine(System.Windows.Point[] Pt, InOutPutMode mode)
        {
            switch (mode)
            {
                case InOutPutMode.input:
                    this.InputLine.Points.Clear();
                    for (int i = 0; i < Pt.Length; i++)
                    {
                        this.InputLine.Points.Add(new System.Windows.Point(Pt[i].X, Pt[i].Y));
                        
                    }
                    break;
                case InOutPutMode.output:
                    this.OutputLine.Points.Clear();
                    for (int i = 0; i < Pt.Length; i++)
                    {
                        this.OutputLine.Points.Add(new System.Windows.Point(Pt[i].X, Pt[i].Y));
                    }
                    break;
            }
        }
       
        //終了時の処理
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            this._OnClose();
            this.systemMain.OnClose();
        }

    }
}
