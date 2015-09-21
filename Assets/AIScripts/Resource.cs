using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour {
	public int amountOfMaterial;
	public enum ResourceType {Nothing, Lava, Stone};
	public ResourceType type;
	
	// Update is called once per frame
	void FixedUpdate () {
		//If the resource has no more material left destroy it
		if (amountOfMaterial <= 0 && this.gameObject!=null) {
			Debug.Log (amountOfMaterial<=0 && this.gameObject!=null);
			Destroy (this.gameObject);
			GameObject a=GameObject.FindGameObjectWithTag("A*");//Gets the a* pathfinding object so it can recreate the grid
			a.GetComponent<Grid>().ReCreateGrid();
		}
	}
	//Subtracts the material amount from its current available stores
	public void ReduceAmountOfMaterial(int gatherSpeed){
		if (amountOfMaterial != 0) {
			amountOfMaterial--;
		}
	}
}
