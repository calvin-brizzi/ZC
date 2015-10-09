using UnityEngine;
using System.Collections;

public class build : MonoBehaviour {
	public int percentage =2000;
	public GameObject prefab;
	public GameObject daddy;
	public int t;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (percentage >= 100) {
			GameObject x = (GameObject)Instantiate (prefab,daddy.gameObject.transform.position,Quaternion.identity);
			x.GetComponent<DestructableBuilding>().team = t;
			Destroy (daddy.gameObject);
			print ("runs");
			GameObject a=GameObject.FindGameObjectWithTag("A*");
			a.GetComponent<Grid>().ReCreateGrid();
		}
	}

	public void builder(){
		print (percentage);
		percentage += 10;

	}
}
