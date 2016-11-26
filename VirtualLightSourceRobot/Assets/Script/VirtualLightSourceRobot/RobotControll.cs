using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Robot
{
    

    /// <summary>
    /// スクリプト管理
    /// </summary>
    /// 
    public class RobotContorll: MonoBehaviour
    {
        List<Vector3> list_humanpos;

        public GameObject robot;
        public GameObject robotLight;

        List<IRobotMove> list_robotActionV;
        List<IRobotMove> list_robotActionH;

        Vector3 preRobotPos;
        Vector3 preLightPos;

        #region
        //平面方向
        enum _VerticleMode
        {
            fixedMode, COSM, Circumscribe, Random,
        }
        _VerticleMode body_V;
        _VerticleMode light_V;

        //垂直方向
        enum _HorizontalMode
        {
            SinMove, Random,
        }
        _HorizontalMode body_H;
        _HorizontalMode light_H;
        #endregion

        // Use this for initialization
        void Start()
        {
            this.list_humanpos = new List<Vector3>();

            //移動リストの作成
            this.list_robotActionV = new List<IRobotMove>();
            this.list_robotActionH = new List<IRobotMove>();

            //モード
            this.body_H = new _HorizontalMode();
            this.body_V = new _VerticleMode();
            this.light_H = new _HorizontalMode();
            this.light_V = new _VerticleMode();
        }

        // Update is called once per frame
        void Update()
        {
            //データ取得

            //ロボット本体の動き
            Vector3 robotPos = new Vector3();
            //水平方向
            robotPos = this.list_robotActionH[(int)this.body_H].UpDate(this.robot.transform.position, this.list_humanpos);
            //垂直方向
            robotPos.y = this.list_robotActionV[(int)this.body_V].UpDate(this.robot.transform.position, this.list_humanpos).y;
            this.robot.transform.position = robotPos;

            //光源の動き
            //水平方向           
            //垂直方向
            
        }

        void Init()
        {
            this.robot.transform.position = this.preRobotPos;
            this.robot.transform.FindChild("RobotLight").transform.position = this.preLightPos;
        }

    }


}
