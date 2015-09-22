using UnityEngine;
using System.Collections;

public class GridMaker : MonoBehaviour {
	int x = 6;
	int count = 0;
	public GameObject A;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void add(){
		count++;
		print (count);
		if (x == count) {
			A.GetComponent<Grid>().CreateGrid();

		}
	}
}
