using UnityEngine;
using System.Collections;

public class build : MonoBehaviour {
	public int percentage =2000;
	public GameObject prefab;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (percentage >= 100) {
			Instantiate (prefab,this.transform.position,Quaternion.identity);
			Destroy (this.gameObject);
			print ("runs");

		}
	}

	public void building(int x){
		percentage += x;

	}
}
