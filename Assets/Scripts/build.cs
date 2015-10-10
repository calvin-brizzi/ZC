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
			Vector3 gm = daddy.gameObject.transform.position;
			gm.y = 0.08f;
			GameObject x = (GameObject)Instantiate (prefab,gm,Quaternion.identity);
			x.GetComponent<DestructableBuilding>().team = t;
			print (gameObject.transform.position);
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
