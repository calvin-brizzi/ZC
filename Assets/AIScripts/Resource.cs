using UnityEngine;
using System.Collections;

public class Resource : MonoBehaviour {
	int amountOfMaterial = 20;
	public enum Type {Lava, Stone};
	public Type type;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (amountOfMaterial);
		if (amountOfMaterial <= 0 && this.gameObject!=null) {
			Debug.Log (amountOfMaterial<=0 && this.gameObject!=null);
			Destroy (this.gameObject);
			GameObject a=GameObject.FindGameObjectWithTag("A*");
			a.GetComponent<Grid>().ReCreateGrid();
		}
	}

	public void ReduceAmountOfMaterial(int gatherSpeed){
		amountOfMaterial--;
	}
}
