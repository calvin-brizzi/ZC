using UnityEngine;
using System.Collections;

public class cameraPositioning : MonoBehaviour {
	public GameObject redBase;
	public GameObject blueBase;
	// Use this for initialization
	void Start () {
		Vector3 temp = this.transform.position;
		temp.y = temp.y + 10;
		if (VarMan.Instance.pNum != 1) {
			temp.x = redBase.transform.position.x;
			temp.z = redBase.transform.position.z;
		} else {
			temp.x = blueBase.transform.position.x;
			temp.z = blueBase.transform.position.z;
		}
		this.transform.position = temp;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
