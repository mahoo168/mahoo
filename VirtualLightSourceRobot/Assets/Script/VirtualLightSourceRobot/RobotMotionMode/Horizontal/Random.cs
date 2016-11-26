using UnityEngine;
using System.Collections;

public class Random : MonoBehaviour {

    public GameObject robot;
    GameObject robotLight;

    Vector3 vecBody;
    Vector3 vecLight;
    int interval;
    public int velParameter;
    public float fieldWidth;
    public float fieldHeight;
    public bool IsHeight;
    public bool IsLight;

    Vector2 rangex;
    Vector2 rangez;
    float preTime;

	// Use this for initialization
	void Start ()
    {
        //init
        this.Chenge();
        
        this.interval = 50;

        this.vecLight = Vector3.zero;
        this.preTime  = 0;

        this.rangex = new Vector2(-fieldWidth / 2, fieldWidth / 2);
        this.rangez = new Vector2(-this.fieldHeight / 2, this.fieldHeight / 2);

        this.robotLight = this.robot.transform.FindChild("RobotLight").gameObject;

    }
	
	// Update is called once per frame
	void Update ()
    {
        //ランダム値の変更するかどうかの確認
        float time = Time.realtimeSinceStartup;
        if ((int)time % this.interval == 0 && (time - this.preTime) > 1f)
        {
            this.preTime = time;
            this.Chenge();
            this.interval = UnityEngine.Random.Range(3, 10);
        }

        if (!this.IsHeight) this.vecBody.y = 0;
        if (!this.IsLight)  this.vecLight = Vector3.zero;

        //ロボットの動き
        Vector3 pos = this.robot.transform.position + this.vecBody;
        if (pos.x > this.rangex.x && pos.x < this.rangex.y && pos.z > this.rangez.x && pos.z < this.rangez.y)
        {
            this.robot.transform.position += this.vecBody;
        }
        else
        {
            this.Chenge();
            //Debug.Log("change" + pos);
        }

        //光源の動き
        if (this.IsLight)
        {
            Vector3 lightPos = this.robotLight.transform.position + this.vecLight;
            if (lightPos.y > 0.5f && lightPos.y < 3)
                this.robotLight.transform.position += this.vecLight;
            else this.Chenge();

        }
    }

    void Chenge()
    {
        this.vecBody = new Vector3(UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50), UnityEngine.Random.Range(-50, 50));
        this.vecBody /= (this.vecBody.magnitude * this.velParameter);
        this.vecLight.y = this.vecBody.y;
    }
}
