/* Adds humans to the resources every few steps */

using UnityEngine;
using System.Collections;

public class HubaHuba : MonoBehaviour {
	int team;
	float nextHuman;
	float currentTime;
	float delay;
	// Use this for initialization
	void Start () {
		delay = 2f;
		nextHuman = Time.time + delay;
		team = this.GetComponent<DestructableBuilding> ().team;

	}
	
	// Update is called once per frame
	void Update () {
		Time x;
		currentTime = Time.time;
		if (currentTime > nextHuman) {
			nextHuman = currentTime + delay;
			//Camera.main.GetComponent<Player> ().placeHumans(1,team-1);
		}

	}
}
