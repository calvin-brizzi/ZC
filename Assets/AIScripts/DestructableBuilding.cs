using UnityEngine;
using System.Collections;

public class DestructableBuilding : MonoBehaviour {
	public int team;
	public int health;
	// Update is called once per frame
	void Update () {
		if(health<=0){
			Destroy (this.gameObject);
			GameObject a=GameObject.FindGameObjectWithTag("A*");
			a.GetComponent<Grid>().ReCreateGrid();
		}
	}

	void RecreateGrid(){

		print ("hi");
	}
}
