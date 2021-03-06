﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PointCloud : MonoBehaviour
{
    #region
    public GameObject cube;
    private List<GameObject> ListCube = new List<GameObject>();
    public Windows.Kinect.KinectSensor sensor;
    private ushort[] RawData;
    private Windows.Kinect.CameraSpacePoint[] cameraSpacePoints;
    public Windows.Kinect.DepthFrameReader depthreader { get; set; }
    private int imageWidth;
    private int imageHeight;

    public GameObject kinect;

    public enum _Mode { Random, Boder, Check, Constant, }
    public _Mode mode;
    public bool IsContour;

    Vector3 hidePositon;

    //パラメータ
    public int maxCubeNum = 5000;
    public Vector2 rangex;
    public Vector2 rangez;
    public float roophight;
    [Range(1, 100)]
    public int range;
    public bool IsReset = false;

    //監視
    public int particulsnum;
    [HideInInspector]
    public Vector3 centerPos;

 
    #endregion

    // Use this for initialization
    void Start()
    {
        this.imageHeight = 424;
        this.imageWidth = 512;

        this.sensor = Windows.Kinect.KinectSensor.GetDefault();
        this.depthreader = this.sensor.DepthFrameSource.OpenReader();
        this.RawData = new ushort[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        this.ListCube = new List<GameObject>();

        this.cameraSpacePoints = new Windows.Kinect.CameraSpacePoint[this.depthreader.DepthFrameSource.FrameDescription.LengthInPixels];
        this.depthreader.FrameArrived += depthreader_FrameArrived;
        this.sensor.Open();
        
        this.centerPos = new Vector3();
        this.hidePositon = new Vector3(0, -100, 0);
    }

    void depthreader_FrameArrived(object sender, Windows.Kinect.DepthFrameArrivedEventArgs e)
    {
        using (var frame = e.FrameReference.AcquireFrame())
        {
            frame.CopyFrameDataToArray(this.RawData);
            sensor.CoordinateMapper.MapDepthFrameToCameraSpace(this.RawData, this.cameraSpacePoints);

        }
    }


    // Update is called once per frame
    void Update()
    {

        int cubeCount = 0;
        //this.centerPos = Vector3.zero;
        switch (this.mode)
        {
            case _Mode.Random: this.Contemporary(ref cubeCount); break;
            case _Mode.Boder: this.Border(ref cubeCount); break;
            case _Mode.Check: this.Check(ref cubeCount); break;
            case _Mode.Constant: this.Constant(ref cubeCount); break;
        }
        //this.Contour(ref cubeCount);

        ////重心位置算出
        //this.centerPos /= cubeCount;
        //this.centerPos += this.kinect.transform.position;
        //this.CenterPos();

        //隠す処理
        if (this.ListCube.Count > (cubeCount + 100))
        {
            for (int i = cubeCount; i < this.ListCube.Count; i++)
            {
                //消す   重い>(´；ω；｀)
                //Destroy(this.ListCube[cubeCount]);
                //this.ListCube.RemoveAt(cubeCount);

                //フィールド外に隠す（軽いけど場所によっては影がでるかも？）
                this.ListCube[i].transform.position = this.hidePositon;
            }

        }

        this.particulsnum = this.ListCube.Count;

        //Reset
        if (this.IsReset) this.Reset();
    }

    void CubeControll(int cubeCount, Vector3 point)
    {
        if (cubeCount < this.ListCube.Count)
        {
            this.ListCube[cubeCount].transform.localPosition = point;
        }
        else
        {
            this.ListCube.Add(Instantiate(this.cube));
            this.ListCube[this.ListCube.Count - 1].transform.parent = this.kinect.transform;
            this.ListCube[this.ListCube.Count - 1].transform.localPosition = point;
        }
    }

    void Contemporary(ref int cubeCount)
    {
        int pointCount = 0;
        for (int i = this.imageWidth; i < cameraSpacePoints.Length - this.imageWidth; i++)
        {
            //奥の壁排除
            if (this.cameraSpacePoints[i].Z > this.rangez.x && this.cameraSpacePoints[i].Z < this.rangez.y && cubeCount < this.maxCubeNum)
            {
                //床排除と左右の壁排除                       
                if (this.cameraSpacePoints[i].Y < this.roophight && this.cameraSpacePoints[i].Y > -this.kinect.transform.position.y && this.cameraSpacePoints[i].X > this.rangex.x && this.cameraSpacePoints[i].X < this.rangex.y)
                {
                    //三次元位置に変更
                    Vector3 point = new Vector3(-this.cameraSpacePoints[i].X, this.cameraSpacePoints[i].Y, this.cameraSpacePoints[i].Z);

                    //条件クリアした粒子
                    //輪郭
                    if (this.IsContour &&
                       (Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i + this.imageWidth].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i - this.imageWidth].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i + 1].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i - 1].Z) > 0.5))
                    {
                        this.CubeControll(cubeCount, point);
                        cubeCount++;
                    }
                    else if (pointCount % this.range == 0)
                    {
                        this.CubeControll(cubeCount, point);

                        cubeCount++;
                    }
                    pointCount++;

                }
            }

        }
    }
    void Border(ref int cubeCount)
    {
        for (int i = this.imageWidth; i < cameraSpacePoints.Length - this.imageWidth; i++)
        {
            //奥の壁排除
            if (this.cameraSpacePoints[i].Z > this.rangez.x && this.cameraSpacePoints[i].Z < this.rangez.y && cubeCount < this.maxCubeNum)
            {
                //三次元位置に変更
                Vector3 point = new Vector3(-this.cameraSpacePoints[i].X, this.cameraSpacePoints[i].Y, this.cameraSpacePoints[i].Z);
                //床排除と左右の壁排除                       
                if (point.y < this.roophight && point.y > -this.kinect.transform.position.y && point.x > this.rangex.x && point.x < this.rangex.y)
                {
                    //条件クリアした粒子
                    //輪郭
                    if (this.IsContour &&
                       (Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i + this.imageWidth].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i - this.imageWidth].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i + 1].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i - 1].Z) > 0.5))
                    {
                        this.CubeControll(cubeCount, point);
                        cubeCount++;
                    }
                    //輪郭の内側
                    //ボーダー状にする
                    else if ((int)(i / this.imageWidth) % this.range == 0)
                    {

                        this.CubeControll(cubeCount, point);
                        cubeCount++;
                    }
                }
            }

        }
    }
    void Check(ref int cubeCount)
    {
        for (int i = this.imageWidth; i < cameraSpacePoints.Length - this.imageWidth; i++)
        {
            //奥の壁排除
            if (this.cameraSpacePoints[i].Z > this.rangez.x && this.cameraSpacePoints[i].Z < this.rangez.y && cubeCount < this.maxCubeNum)
            {
                //三次元位置に変更
                Vector3 point = new Vector3(-this.cameraSpacePoints[i].X, this.cameraSpacePoints[i].Y, this.cameraSpacePoints[i].Z);

                //床排除と左右の壁排除                       
                if (point.y < this.roophight && point.y > -this.kinect.transform.position.y && point.x > this.rangex.x && point.x < this.rangex.y)
                {

                    //輪郭
                    if (this.IsContour &&
                       (Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i + this.imageWidth].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i - this.imageWidth].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i + 1].Z) > 0.5 ||
                        Mathf.Abs(this.cameraSpacePoints[i].Z - this.cameraSpacePoints[i - 1].Z) > 0.5))
                    {
                        this.CubeControll(cubeCount, point);
                        cubeCount++;
                    }
                    //輪郭の内側
                    //チェック状にする
                    else
                    {

                        int x = i % this.imageWidth;
                        int y = i / this.imageHeight;

                        if (x % this.range == 0 && y % this.range == 0)
                        {
                            //条件クリアした粒子

                            this.CubeControll(cubeCount, point);
                            cubeCount++;
                        }

                    }

                }
            }


        }
    }
    void Constant(ref int cubeCount)
    {
        //順番に走査
        //奥行き１m毎にレンジ調整
        try
        {
            for (float i = this.rangez.x; i < this.rangez.y; i += 1f)
            {
                int pointCount = 0;
                int constRange = (int)(this.range * Mathf.Pow((int)(Mathf.Abs(this.kinect.transform.position.z) - this.rangez.x) / i, 2));
                //Debug.Log(i.ToString() + ":" + constRange);

                for (int j = this.imageWidth; j < cameraSpacePoints.Length - this.imageWidth; j++)
                {

                    //奥の壁排除
                    if (this.cameraSpacePoints[j].Z > this.rangez.x && this.cameraSpacePoints[j].Z < i && cubeCount < this.maxCubeNum)
                    {
                        //三次元位置に変更
                        Vector3 point = new Vector3(-this.cameraSpacePoints[j].X, this.cameraSpacePoints[j].Y, this.cameraSpacePoints[j].Z);

                        //床排除と左右の壁排除                       
                        if (point.y < this.roophight && point.y > -this.kinect.transform.position.y && point.x > this.rangex.x && point.x < this.rangex.y)
                        {
                            //条件クリアした粒子
                            //輪郭
                            if (this.IsContour &&
                                (Mathf.Abs(this.cameraSpacePoints[j].Z - this.cameraSpacePoints[j + this.imageWidth].Z) > 0.5 ||
                                  Mathf.Abs(this.cameraSpacePoints[j].Z - this.cameraSpacePoints[j - this.imageWidth].Z) > 0.5 ||
                                  Mathf.Abs(this.cameraSpacePoints[j].Z - this.cameraSpacePoints[j + 1].Z) > 0.5 ||
                                  Mathf.Abs(this.cameraSpacePoints[j].Z - this.cameraSpacePoints[j - 1].Z) > 0.5))
                            {
                                this.CubeControll(cubeCount, point);
                                cubeCount++;


                            }
                            else if (pointCount % constRange == 0)
                            {
                                this.CubeControll(cubeCount, point);
                                cubeCount++;
                            }

                            pointCount++;

                        }
                    }

                }

            }

        }
        catch { }


    }

    void CenterPos()
    {
        this.centerPos = Vector3.zero;
        for (int i = 0; i < this.ListCube.Count; i++)
        {
            this.centerPos += this.ListCube[i].transform.position;
        }
        this.centerPos /= this.ListCube.Count;
    }
    void Reset()
    {
        try
        {
            for (int i = 0; i < this.ListCube.Count; i++)
            {
                Destroy(this.ListCube[0]);
                this.ListCube.RemoveAt(0);
            }
            this.IsReset = false;
        }
        catch { }

    }

}
