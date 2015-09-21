using UnityEngine;
using System.Collections;

public class DestructableBuilding : MonoBehaviour {
	public int team;
	public int health;
	// Update is called once per frame
	void Update () {
		//If health<0 destroy and recreate grid
		if(health<=0){
			Destroy (this.gameObject);
			GameObject a=GameObject.FindGameObjectWithTag("A*");
			a.GetComponent<Grid>().ReCreateGrid();
		}
	}
}
