using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace Robot
{
    public class Circumscribed : IRobotMove
    {
        Vector3 nextRobotPos;
        float velocity = 0.05f;

        int target0;
        int target1;

        float radius;
        float radian;
        Vector3 target;
        Vector3 preCenter;
        float rVel = 0.05f ;

        // Use this for initialization
        public Circumscribed()
        {
            this.nextRobotPos = Vector3.zero;
            this.target = Vector3.zero;
            this.preCenter = Vector3.zero;
        }

        public Vector3 UpDate(Vector3 robotPos, List<Vector3> list_pos)
        {
            if(list_pos.Count > 0)
            {
                this.MoveCircle(list_pos);
            }

            this.nextRobotPos = this.target;
            return nextRobotPos;
        }


        void MoveCircle(List<Vector3> list_pos)
        {
            //半径
            float R = (this.MaxLength(list_pos, ref this.target0, ref this.target1) / 2); //目標値
            if(R > this.radius)
            {
                this.radius += this.rVel * Time.deltaTime;

            }
            else
            {
                this.radius -= this.rVel * Time.deltaTime;
            }

            //回転中心
            Vector3 centerPos = (list_pos[this.target0] + list_pos[this.target1]) / 2;

            //回転角度
            //角速度は人の重心（回転中心からの相対）
            Vector3 centerofHuman = this.CenterPosition(list_pos);
            //回転角度算出
            this.radian = centerofHuman.magnitude * Time.deltaTime;

            //目標位置算出(時計回り）
            this.target = centerPos + (Quaternion.AngleAxis(this.radian, Vector3.up) * ((this.target - this.preCenter) / (this.target - this.preCenter).magnitude * this.radius));

            //位置保存
            this.preCenter = centerofHuman;       

        }


        //計算用関数
        public float MaxLength(List<Vector3> list, ref int humannum0, ref int humannum1)
        {
            float length = 0;
            if (list.Count == 1)
            {
                humannum0 = 0;
                humannum1 = 0;
                return 0;
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    for (int j = 0; j < list.Count; j++)
                    {
                        if ((list[j] - list[i]).magnitude > length)
                        {
                            length = (list[j] - list[i]).magnitude;
                            humannum0 = i;
                            humannum1 = j;
                        }
                    }
                }
                return length;
            }

        }
        public float MinLength(List<Vector3> list, ref int humannum0, ref int humannum1)
        {
            float length = 0;
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i; j < list.Count; j++)
                {
                    if ((list[j] - list[i]).magnitude < length || length == 0)
                    {
                        length = (list[j] - list[i]).magnitude;
                        humannum0 = i;
                        humannum1 = j;
                    }

                }
            }
            return length;
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
    }
}


