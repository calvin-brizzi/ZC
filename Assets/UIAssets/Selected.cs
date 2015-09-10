using UnityEngine;
using System.Collections;

public class Selected : MonoBehaviour {
	bool sel = true;
	bool colide = false;
	int current = 0;
	public GameObject[] buildings;

	// Use this for initialization
	void Start () {

	}
	public void set(int x){
		current = x;
	}

	// Update is called once per frame
	void Update () {
		if (sel) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			if(Physics.Raycast (ray,out hit,2400)){
				Vector3 raise = hit.point;

				transform.position = raise;

			}
		}

		if (Input.GetMouseButton (0)) {
			if(!colide){
				sel = false;
				Instantiate (buildings[current],this.transform.position,Quaternion.identity);
				Destroy (this.gameObject);
			}

		}
		if (Input.GetMouseButton (1)) {
			Destroy (this.gameObject);
		}


	}

	void select(){
		sel = true;

	}

	void OnCollisionEnter(Collision coll){
	
		if (coll.gameObject.name == "Terrain") {

		} else {
		
			colide = true;
			
		}
	
	}
	void OnCollisionExit(Collision coll){
	
		if (coll.gameObject.name == "Terrain") {
			
		} else {

			colide = false;
			
		}
		
	}



}
