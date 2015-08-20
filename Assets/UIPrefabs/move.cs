using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {
	public int X;
	public int Y;
	public bool moveCommand = true;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void selected(){
		if (moveCommand == false) {
			moveCommand = true;
		} else {
			moveCommand = false;
		}
	}
}
