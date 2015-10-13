using UnityEngine;
using System.Collections;

public class build : MonoBehaviour {
	public int percentage =2000;
	public GameObject prefab;
	public GameObject daddy;
	public GameObject secondary;
	public int t;
	// Use this for initialization
	void Start () {
		t = Camera.main.GetComponent<AICamera> ().team;
	}
	
	// Update is called once per frame
	void Update () {
		if (percentage >= 100) {
			Vector3 gm = daddy.gameObject.transform.position;
			gm.y = 0.08f;
			GameObject x = (GameObject)Network.Instantiate (prefab,gm,Quaternion.identity,0);
			x.GetComponent<DestructableBuilding>().team = t;
            try
            {
                x.transform.GetComponent<Unit>().team = VarMan.Instance.pNum;
            }
            catch {
                Debug.Log("Not a tower");
            }
			print (gameObject.transform.position);
			Destroy (daddy.gameObject);
			print ("runs");
			if(secondary){
				Destroy (secondary.gameObject);
			}
			GameObject a=GameObject.FindGameObjectWithTag("A*");
			a.GetComponent<Grid>().ReCreateGrid();
		}
	}

	public void builder(){
		print (percentage);
		percentage += 10;

	}
}
