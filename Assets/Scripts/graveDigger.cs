using UnityEngine;
using System.Collections;

public class graveDigger : MonoBehaviour {
	int team;
	// Use this for initialization
	void Start () {
		team = this.GetComponent<DestructableBuilding> ().team;
		Camera.main.GetComponent<Player> ().addHousing (10,team-1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnDestroy(){
		Camera.main.GetComponent<Player> ().allocateHousing (10,team-1);
	}
}
