using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Not 人型
/// 重心移動と逆向きに移動
/// 1.常に一定の速度(1/100 [m/s])
/// 2.人の移動速度で変化
/// 
/// </summary>
public class ShadowRobot1 : MonoBehaviour {

    //public GameObject model;
    public GameObject robot;
    List<Human> List_Human;
    List<Human> List_Human_Last;
    CIPCReceiver cipc;
    bool IsChangeVelocity;
    // Use this for initialization
    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    void Start()
    {
        this.List_Human = new List<Human>();
        this.List_Human_Last = new List<Human>();
        this.cipc = GameObject.FindGameObjectWithTag("CIPC").GetComponent<CIPCReceiver>();
        this.IsChangeVelocity = false;
        
    }

    // Update is called once per frame
    void Update()
    {
        this.List_Human_Last = this.List_Human;
        this.List_Human = this.cipc.List_Humans;
        if (this.List_Human.Count != 0)
        {
            Vector3 centerVec = this.CenterPosition(this.List_Human);
            Vector3 vec = - centerVec - this.robot.transform.position;
            vec =  vec / vec.magnitude;

            if (this.IsChangeVelocity)
            {
                float vel = this.Velocity();
                this.robot.transform.position += vec / vel / 1000;
               
            }
            else this.robot.transform.position += vec/100;
            this.robot.transform.LookAt(centerVec);
        }
        
    }

    //動きのレパートリー
    //重心位置算出
    Vector3 CenterPosition(List<Human> list_human)
    {
        int human_Num = list_human.Count;
        Vector3 vector = new Vector3();
        for (int i = 0; i < list_human.Count; i++)
        {
            vector += list_human[i].bones[0].position;
        }
        vector = new Vector3(vector.x / human_Num, 0, vector.z / human_Num);
        //vector = vector / vector.magnitude;
        return vector;
    }
    //間に割って入る
    Vector3 Warikomi(List<Human> list_human)
    {
        Vector3 vector = new Vector3();

        float[] length = new float[list_human.Count];
        for (int i = 0; i < length.Length; i++)
        {
            float localLength = 100f;
            for(int j = 0; j < length.Length; j++)
            {
                if (i != j)
                {
                    if (localLength > (list_human[j].bones[0].position - list_human[i].bones[0].position).magnitude)
                        localLength = (list_human[j].bones[0].position - list_human[i].bones[0].position).magnitude;
                }              
            }
            length[i] = localLength;
        }

            return vector;
    }
    //よける
    Vector3 Escape(List<Human> list_human, Human robot)
    {
        Vector3 vector = new Vector3();
        float[] length = new float[list_human.Count];
        for (int i = 0; i < list_human.Count; i++)
        {
            length[i] = (list_human[i].bones[0].position - robot.bones[0].position).magnitude;
        }
            return vector;
    }

    //移動速度変化
    float Velocity()
    {
        float velocity = 0f;
        int human_Num = this.List_Human.Count;
        int human_Num_Last = this.List_Human_Last.Count;
        int count;
        if (human_Num < human_Num_Last) count = human_Num;
        else count = human_Num_Last;
        Vector3 vector = new Vector3();
        for (int i = 0; i < count; i++)
        {
            vector = (this.List_Human_Last[i].bones[0].position - this.List_Human[i].bones[0].position) ;
            velocity += vector.magnitude;
        }
        vector /= count;
        //vector = vector / vector.magnitude;
        return velocity;
    }

    



    public void ChangeVel(bool IsChage)
    {
        this.IsChangeVelocity = IsChage;
    }

    
}
