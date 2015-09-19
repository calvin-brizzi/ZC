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
		Instantiate (brute, this.transform.position, Quaternion.identity);
		brute.GetComponent<Unit>().MoveUnit(this.transform.position,this.transform.position);
	}

	// Update is called once per frame
	void Update () {
		
	}
}
