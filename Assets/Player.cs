using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {
	public int p;
	public int[] lava;
	public int[] humans;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	int getLava(){
		return lava [p - 1];
	}
	int getHumans(){
		return humans [p - 1];
	}
	void takeLava(int x){
		lava [p - 1] = lava [p - 1] - x;
	}
	void takeHumans(int x){
		humans[p - 1] = humans [p - 1] - x;
	}
	void placeLava(int x){
		lava [p - 1] = lava [p - 1] + x;
	}
	void placeHumans(int x){
		humans[p - 1] = humans [p - 1] + x;
	}



}
