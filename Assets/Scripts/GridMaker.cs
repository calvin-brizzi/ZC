using UnityEngine;
using System.Collections;

public class GridMaker : MonoBehaviour {
	int x = 6;
	int count = 0;
	public GameObject A;

	public void add(){
		count++;
		print (count);
		if (x == count) {
			GameObject a=GameObject.FindGameObjectWithTag("A*");
			a.GetComponent<Grid>().ReCreateGrid();
		}
	}
}
