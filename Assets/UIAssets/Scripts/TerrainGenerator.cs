using UnityEngine;
using System.Collections;

public class TerrainGenerator : MonoBehaviour {
	public int startX,endX,startZ,endZ,y;
	public int quantity;
	public Transform obj;

	// Use this for initialization
	void Start () {
		for (int i = 0; i<quantity; i++) {
			int x = Random.Range (startX, endX);
			int z = Random.Range (startZ, endZ);
			Instantiate (obj,new Vector3(x,y,z),Quaternion.identity);
		}
		this.GetComponent<GridMaker> ().add ();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
