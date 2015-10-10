using UnityEngine;
using System.Collections;

public class SetupMap : MonoBehaviour {
	int team;

	// Use this for initialization
	void Start () {
		team = Camera.main.GetComponent<AICamera> ().team; 
		if (team == 1) {
			
			this.camera.cullingMask = (1 << 0) | (1 << 5) |(1<<14);
		} else {
			this.camera.cullingMask = (1 << 0) | (1 << 5)  |(1<<15);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
