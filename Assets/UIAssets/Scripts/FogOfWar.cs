using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour {
	int team;
	public GameObject daddy;
	public GameObject[] children;
	string test;
	// Use this for initialization
	void Start () {

		team = daddy.GetComponent<Unit> ().team;
		if (team == 1) {
			test = "P1";
		} else {
			test = "P2";
		}
	}


	// Update is called once per frame
	void Update () {

		Collider[] hitColliders = Physics.OverlapSphere (this.transform.position, 45f);
		int i = 0;
		int count = 0;
		int check;
		while (i<hitColliders.Length) {
			try{
				check= hitColliders[i].gameObject.GetComponent<DestructableBuilding>().team;
				//print ("Collided with a building");
			}catch{
				try{
					check= hitColliders[i].gameObject.GetComponent<Unit>().team;
					//print ("collided with unit");
				}catch{
					check = 0;
				}
			}

			if(check!=0){
				if(check!=team){
					count++;
				//hitColliders[i].gameObject.layer = LayerMask.NameToLayer("P3");
				}
			}
			i++;
			
		}
		//print ("IN RANGE "+count+" "+test);
		if (count > 0) {
			for(int h =0;h<children.Length;h++){
				children[h].gameObject.layer = LayerMask.NameToLayer("P3");
			}

		} else {
			//print ("REVERT");
			for(int h =0;h<children.Length;h++){
				children[h].gameObject.layer = LayerMask.NameToLayer(test);
			}
		}

	}
}
