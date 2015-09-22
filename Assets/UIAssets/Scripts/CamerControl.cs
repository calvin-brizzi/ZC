using UnityEngine;
using System.Collections;

public class CamerControl : MonoBehaviour {
	public float mag;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey (KeyCode.LeftArrow)) {
			Vector3 temp = transform.position;
			temp.x = temp.x - mag;
			transform.position = temp;
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			Vector3 temp = transform.position;
			temp.x = temp.x +mag;
			transform.position = temp;
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			Vector3 temp = transform.position;
			temp.z = temp.z +mag;
			transform.position = temp;
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			Vector3 temp = transform.position;
			temp.z = temp.z - mag;
			transform.position = temp;
		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			this.transform.position = this.transform.position - this.transform.forward*20;
		}
		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			this.transform.position = this.transform.position + this.transform.forward*20;
		}
	}
}
