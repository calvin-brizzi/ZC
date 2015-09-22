using UnityEngine;
using System.Collections;

public class Spawn : MonoBehaviour {
	public GameObject brute;
	public GameObject grunt;
	public GameObject archer;
	// Use this for initialization
	void Start () {
	
	}

	public void spawnBrute(){
		print ("TRYS");
		Vector3 spawnpoint = this.transform.position;
		spawnpoint.z = spawnpoint.z - 50;
		Instantiate (brute, spawnpoint, Quaternion.identity);
	}
	public void spawnArcher(){
		print ("TRYS");
		Vector3 spawnpoint = this.transform.position;
		spawnpoint.z = spawnpoint.z - 50;
		Instantiate (archer, spawnpoint, Quaternion.identity);
	}
	public void spawnGrunt(){
		print ("TRYS");
		Vector3 spawnpoint = this.transform.position;
		spawnpoint.z = spawnpoint.z - 50;
		Instantiate (grunt, spawnpoint, Quaternion.identity);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
