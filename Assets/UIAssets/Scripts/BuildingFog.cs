using UnityEngine;
using System.Collections;

public class BuildingFog : MonoBehaviour {

	int team;
	public GameObject daddy;
	string test;
	// Use this for initialization
	void Start () {
		team = daddy.GetComponent<DestructableBuilding> ().team;
		if (team == 1) {
			test = "P1";
		} else {
			test = "P2";
		}
	}

	// Update is called once per frame
	void Update () {
		Collider[] hitColliders = Physics.OverlapSphere (this.transform.position, 15f);
		int i = 0;
		int count = 0;
		int check=0;
		while (i<hitColliders.Length) {
			try{
				check= hitColliders[i].gameObject.GetComponent<DestructableBuilding>().team;
				//print ("Collided with a  building");
			}catch{

				try{
					check= hitColliders[i].gameObject.GetComponent<Unit>().team;
					//print ("collided with unit");
				}catch{
					check = 0;
				}
			}
			//print (check);
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
		
			this.gameObject.layer = LayerMask.NameToLayer("P3");
		} else {
			//print ("REVERT");
			this.gameObject.layer = LayerMask.NameToLayer(test);
		}
	}
}
