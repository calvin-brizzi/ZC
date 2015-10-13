using UnityEngine;
using System.Collections;

public class DestructableBuilding : MonoBehaviour {
	public int team;
	public int health;
	public Texture[] tex;
	public bool initial = false;
	// Update is called once per frame
	void Start (){
		if(!initial){
			Renderer rend = GetComponent<Renderer> ();
			rend.material.mainTexture = tex [team - 1];
		}
	}
	void Update () {
		//If health<0 destroy and recreate grid
		if(health<=0){
			Network.Destroy(this.gameObject);
			//Destroy (this.gameObject);
			GameObject a=GameObject.FindGameObjectWithTag("A*");
			a.GetComponent<Grid>().ReCreateGrid();
		}
	}
}
