using UnityEngine;
using System.Collections;

public class CamerControl : MonoBehaviour {
	public float mag;
	public GameObject[] sides;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos;
		if (Input.GetKey (KeyCode.LeftArrow)) {
			pos = this.transform.position;
			Vector3 temp = transform.position;
			temp.x = temp.x - mag;
			if(temp.x>-650){
				transform.position = temp;
			}
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			pos = this.transform.position;
			Vector3 temp = transform.position;
			temp.x = temp.x +mag;
			if(temp.x<450){
				transform.position = temp;
			}
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			pos = this.transform.position;
			Vector3 temp = transform.position;
			temp.z = temp.z +mag;
			if(temp.z<450){
				transform.position = temp;
			}
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			pos = this.transform.position;
			Vector3 temp = transform.position;
			temp.z = temp.z - mag;
			if(temp.z>-700){
				transform.position = temp;
			}
			

		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			pos = this.transform.position;
			this.transform.position = this.transform.position - this.transform.forward*20;
			if(pos.y>175){
				this.transform.position = pos;
			}
		}
		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			pos = this.transform.position;
			this.transform.position = this.transform.position + this.transform.forward*20;

			if(pos.y<50){
				this.transform.position = pos;
			}
		}
	}
}
