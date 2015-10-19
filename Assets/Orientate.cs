using UnityEngine;
using System.Collections;

public class Orientate : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (VarMan.Instance.pNum != 2) {
			Quaternion target = Quaternion.Euler (0,90,0);
			this.transform.eulerAngles = new Vector3(
				this.transform.eulerAngles.x,
				this.transform.eulerAngles.y + 180,
				this.transform.eulerAngles.z
				);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
