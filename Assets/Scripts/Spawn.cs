using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {
	public GameObject brute;
	public GameObject grunt;
	public GameObject archer;
	public GameObject spawner;
	List<int> queue = new List<int>();
	float start = -1;
	float spawnTime;
	Vector3 spawnpoint ;
	float delay;
	int p;
	// Use this for initialization
	void Start () {
		spawnpoint = spawner.transform.position;
		//spawnpoint.z = spawnpoint.z - 50;
		p = this.GetComponent<DestructableBuilding> ().team;
	}

	public void spawnBrute(){
		print ("TRYS");

		int resource = Camera.main.GetComponent<Player> ().getHumans (p -1);
		if (resource >= 5) {
			Camera.main.GetComponent<Player> ().takeHumans (5, p - 1);
			queue.Add (1);
			if (start == -1) {
				start = Time.time;
				delay = 2;
				spawnTime = start + delay;
			}
		}

	}
	public void spawnArcher(){
		print ("TRYS");


		int resource = Camera.main.GetComponent<Player> ().getHumans (p -1);
		if (resource >= 5) {
			Camera.main.GetComponent<Player> ().takeHumans (5, p - 1);
			queue.Add (2);
			if (start == -1) {
				start = Time.time;
				delay = 2;
				spawnTime = start + delay;
			}
		}

	}
	public void spawnGrunt(){
		print ("TRYS");


		int resource = Camera.main.GetComponent<Player> ().getHumans (p -1);
		if (resource >= 2) {
			Camera.main.GetComponent<Player> ().takeHumans (2, p - 1);
			queue.Add (0);
			if (start == -1) {
				start = Time.time;
				delay = 1;
				spawnTime = start + delay;
			}
		}

	}
	// Update is called once per frame
	void Update () {
		float current = Time.time;
		if (start != -1) {
			if (current > spawnTime) {
				int cls = (int)queue[0];
				queue.RemoveAt(0);
				if(cls ==0){
					Network.Instantiate (grunt, spawnpoint, Quaternion.identity,0);
				}else if(cls ==1){
					Network.Instantiate (brute, spawnpoint, Quaternion.identity,0);
				}else{
					Network.Instantiate (archer, spawnpoint, Quaternion.identity,0);
				}
				if(queue.Count>0){
					cls = (int)queue[0];
					if(cls ==0){
						delay = 1;
						spawnTime = current+delay;
					}else if(cls ==1){
						delay = 2;
						spawnTime = current+delay;
					}else{
						delay = 2;
						spawnTime = current+delay;
					}
				}else{
					start = -1;
				}
			}
		}
	}
}
