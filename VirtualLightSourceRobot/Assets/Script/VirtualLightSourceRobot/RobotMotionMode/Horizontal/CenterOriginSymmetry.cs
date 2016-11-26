using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Robot
{
    public class CenterOriginSymmetry : IRobotMove
    {

        List<Vector3> list_humanpos;

        Vector3 centerPos;
        Vector3 nextRobotPos;
        public float velocity = 0.05f ;

        // Use this for initialization
        public CenterOriginSymmetry()
        {
            this.centerPos = new Vector3();
            this.nextRobotPos = new Vector3();
         
        }

        public Vector3 UpDate(Vector3 robotPos, List<Vector3> list_pos)
        {
            //重心位置計算
            this.centerPos = this.CenterPosition(list_pos);

            //移動
            this.nextRobotPos = Symmetry(robotPos);
            return this.nextRobotPos;
        }

        //重心位置算出
        Vector3 CenterPosition(List<Vector3> list_Pos)
        {
            int human_Num = list_Pos.Count;
            Vector3 vector = new Vector3();
            for (int i = 0; i < list_Pos.Count; i++)
            {
                vector += list_Pos[i];
            }
            vector /= human_Num;
            return vector;
        }

        //動き
        Vector3 Symmetry(Vector3 robotPos)
        {
            //位置
            Vector3 vec = -this.centerPos - robotPos;
            vec.y = 0;
            vec /= vec.magnitude; //方向ベクトル化
            return robotPos + (vec * velocity * Time.deltaTime);
        }

        
    }

}

