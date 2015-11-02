using UnityEngine;
using System.Collections;

public class Light : MonoBehaviour {

    public Camera camera0;
    public Camera camera1;
    public GameObject target0;
    public GameObject target1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        this.camera0.transform.LookAt(target0.transform);
        this.camera1.transform.LookAt(new Vector3(0,0,0));
	}
}
