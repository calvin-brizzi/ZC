/* Adds humans to the resources every few steps */

using UnityEngine;
using System.Collections;

public class HubaHuba : MonoBehaviour {
	float nextHuman;
	float currentTime;
	float delay;
	// Use this for initialization
	void Start () {
		delay = 2f;
		nextHuman = Time.time + delay;
	}
	
	// Update is called once per frame
	void Update () {
		currentTime = Time.time;
		if (currentTime > nextHuman) {
			nextHuman = currentTime + delay;
            VarMan.Instance.humans += 1;
		}

	}
}
