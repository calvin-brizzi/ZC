using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour {
	public int amountOfMaterial;
	public enum ResourceType {Nothing, Lava, Stone};
	public ResourceType type;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (amountOfMaterial <= 0 && this.gameObject!=null) {
			Debug.Log (amountOfMaterial<=0 && this.gameObject!=null);
			Destroy (this.gameObject);
			GameObject a=GameObject.FindGameObjectWithTag("A*");
			a.GetComponent<Grid>().ReCreateGrid();
		}
	}

	public void ReduceAmountOfMaterial(int gatherSpeed){
		if (amountOfMaterial != 0) {
			amountOfMaterial--;
		}
	}
}
