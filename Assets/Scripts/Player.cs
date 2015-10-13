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
        p = VarMan.Instance.pNum ;
		lavaUI.GetComponent<Text> ().text = VarMan.Instance.lava+"";
		humansUI.GetComponent<Text> ().text = VarMan.Instance.humans+"";
		houseUI.GetComponent<Text> ().text = VarMan.Instance.housing+"";
	}

	public int getHousing(int y){
		return VarMan.Instance.housing;
	}

	public void addHousing(int x,int y){
		VarMan.Instance.housing +=10;
	}

	public void allocateHousing(int x,int y){
		VarMan.Instance.housing -= x;
	}
	public int getLava(int y){
		return VarMan.Instance.lava;
	}
	public int getHumans(int y){
		return VarMan.Instance.humans;
	}
	public void takeLava(int x,int y){
		VarMan.Instance.lava = VarMan.Instance.lava - x;
	}
	public void takeHumans(int x,int y){
		VarMan.Instance.humans = VarMan.Instance.humans - x;
	}
	public void placeLava(int x,int y){
		VarMan.Instance.lava = VarMan.Instance.lava + x;
	}
	public void placeHumans(int x,int y){
		VarMan.Instance.humans = VarMan.Instance.humans + x;
	}



}
