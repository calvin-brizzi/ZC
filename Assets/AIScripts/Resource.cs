using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour {
	public int amountOfMaterial;
	public enum Type {Lava, Stone};
	public Type type;
	
	// Update is called once per frame
	void Update () {
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
