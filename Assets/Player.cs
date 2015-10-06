using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public int p;
	public int[] lava;
	public int[] humans;
	public GameObject lavaUI;
	public GameObject humansUI;
	public GameObject houseUI;
	// Use this for initialization
	void Start () {
		Text x;
		x = lavaUI.GetComponent<Text>();
		x.text = "hello";
		lavaUI.GetComponent<Text> ().text = lava[p-1]+"";
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
