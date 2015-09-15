using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selected : MonoBehaviour {
	bool sel = true;
	bool colide = false;
	int current = 0;
	List<GameObject> objects = new List<GameObject> ();
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
				print ("THIS MANY"+objects.Count);
				sel = false;
				Instantiate (buildings[current],this.transform.position,Quaternion.identity);
				Destroy (this.gameObject);
			}else{
				print (objects.Count);
			}

		}
		if (Input.GetMouseButton (1)) {
			Destroy (this.gameObject);
		}


	}

	void select(){
		sel = true;

	}

	void OnTriggerEnter(Collision coll){
		print ("TRIGGERED");
		if (coll.gameObject.name == "Terrain") {

		} else {
			print ("added");
			objects.Add (coll.gameObject);
			colide = true;
			
		}
	
	}
	void OnTriggerExit(Collision coll){
	
		if (coll.gameObject.name == "Terrain") {
			
		} else {
			print ("removed");
			objects.Remove(coll.gameObject);
			colide = false;
			
		}
		
	}



}
