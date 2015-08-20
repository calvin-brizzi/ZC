using UnityEngine;
using System.Collections;

public class Selected : MonoBehaviour {
	bool sel = true;
	bool colide = false;

	// Use this for initialization
	void Start () {

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

			sel = false;
		}


	}

	void select(){
		sel = true;

	}

	void OnCollisionEnter(Collision coll){
		if (coll.gameObject.name == "Terrain") {

		} else {
			print(coll.gameObject == null);
			
		}
	
	}



}
