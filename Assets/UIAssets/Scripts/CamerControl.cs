using UnityEngine;
using System.Collections;

public class CamerControl : MonoBehaviour {
	public float speed;

	// Update is called once per frame
	void Update () {
		Vector3 pos;
		if (Input.GetKey (KeyCode.LeftArrow)) {
			pos = this.transform.position;
			Vector3 temp = transform.position;
			temp.x = temp.x - speed;
			if(temp.x>-1000){
				transform.position = temp;
			}
		}
		if (Input.GetKey (KeyCode.RightArrow)) {
			pos = this.transform.position;
			Vector3 temp = transform.position;
			temp.x = temp.x +speed;
			if(temp.x<1000){
				transform.position = temp;
			}
		}
		if (Input.GetKey (KeyCode.UpArrow)) {
			pos = this.transform.position;
			Vector3 temp = transform.position;
			temp.z = temp.z + speed;
			if(temp.z<1000){
				transform.position = temp;
			}
		}
		if (Input.GetKey (KeyCode.DownArrow)) {
			pos = this.transform.position;
			Vector3 temp = transform.position;
			temp.z = temp.z - speed;
			if(temp.z>-1000){
				transform.position = temp;
			}
			

		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
			pos = this.transform.position;
			this.transform.position = this.transform.position - this.transform.forward*4;
			if(pos.y>16){
				this.transform.position = pos;
			}
		}
		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			pos = this.transform.position;
			this.transform.position = this.transform.position + this.transform.forward*4;

			if(pos.y<7){
				this.transform.position = pos;
			}
		}
	}
}
