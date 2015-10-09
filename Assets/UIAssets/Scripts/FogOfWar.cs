using UnityEngine;
using System.Collections;

public class FogOfWar : MonoBehaviour {
	int team;
	public GameObject daddy;
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
	void changeAll(GameObject x,string y){
		foreach (Transform child in x.transform) {
			child.gameObject.layer = LayerMask.NameToLayer(y);
			print ("CHANGING");
		}
	}

	// Update is called once per frame
	void Update () {
		Collider[] hitColliders = Physics.OverlapSphere (this.transform.position, 5f);
		int i = 0;
		int count = 0;
		int check;
		while (i<hitColliders.Length) {
			try{
				check= hitColliders[i].gameObject.GetComponent<DestructableBuilding>().team;
			}catch{
				check = 0;
			}
			try{
				check= hitColliders[i].gameObject.GetComponent<Unit>().team;
			}catch{
				check = 0;
			}
			if(check!=0){
				if(check!=team){
					count++;
				//hitColliders[i].gameObject.layer = LayerMask.NameToLayer("P3");
				}
			}
			i++;
			
		}
		print ("IN RANGE "+count+" "+test);
		if (count > 0) {
			changeAll (this.gameObject,"P3");
			this.gameObject.layer = LayerMask.NameToLayer("P3");
		} else {
			print ("REVERT");
			this.gameObject.layer = LayerMask.NameToLayer(test);
		}
	}
}
