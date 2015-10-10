using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {
	public int p;
	public int[] lava;
	public int[] humans;
	public int[] housing;
	public GameObject lavaUI;
	public GameObject humansUI;
	public GameObject houseUI;
	// Use this for initialization
	void Start () {
		Text x;
		x = lavaUI.GetComponent<Text>();
		x.text = "hello";

	}
	
	// Update is called once per frame
	void Update () {
		p = this.GetComponent<AICamera> ().team;
		lavaUI.GetComponent<Text> ().text = lava[p-1]+"";
		humansUI.GetComponent<Text> ().text = humans[p-1]+"";
		houseUI.GetComponent<Text> ().text = housing[p-1]+"";
	}

	public int getHousing(int y){
		return housing [y];
	}

	public void addHousing(int x,int y){
		housing [y] += x;
	}

	public void allocateHousing(int x,int y){
		housing [y] -= x;
	}
	public int getLava(int y){
		return lava [y];
	}
	public int getHumans(int y){
		return humans [y];
	}
	public void takeLava(int x,int y){
		lava [y] = lava [y] - x;
	}
	public void takeHumans(int x,int y){
		humans[y] = humans [y] - x;
	}
	public void placeLava(int x,int y){
		lava [y] = lava [y] + x;
	}
	public void placeHumans(int x,int y){
		humans[y] = humans [y] + x;
	}



}
