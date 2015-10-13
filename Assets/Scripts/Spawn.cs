using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawn : MonoBehaviour {
	public GameObject[] options;
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
		if (p == 1) {
			brute = options [0];
			grunt = options [1];
			archer = options [2];
		}
	}

	public void spawnBrute(){
		print ("Spawning Brute");

		int resource = VarMan.Instance.humans;
		int housing = VarMan.Instance.housing;
		if (resource >= 5&&housing>=2) {
			VarMan.Instance.humans-=5;
			VarMan.Instance.housing-=2;
			queue.Add (1);
			if (start == -1) {
				start = Time.time;
				delay = 2;
				spawnTime = start + delay;
			}
		}

	}
	public void spawnArcher(){
		print ("Spawning Archer");


		int resource = VarMan.Instance.humans;
		int housing = VarMan.Instance.housing;
		if (resource >= 5&&housing>=2) {
			VarMan.Instance.humans-=5;
			VarMan.Instance.housing-=2;
			queue.Add (2);
			if (start == -1) {
				start = Time.time;
				delay = 2;
				spawnTime = start + delay;
			}
		}

	}
	public void spawnGrunt(){
		print ("Spawning Grunt");


		int resource = VarMan.Instance.humans;
		int housing = VarMan.Instance.housing;
		if (resource >= 2&&housing>=2) {
			VarMan.Instance.humans-=2;
			VarMan.Instance.housing-=2;
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
				if(cls == 0){
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
