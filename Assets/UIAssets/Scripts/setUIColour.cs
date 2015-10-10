using UnityEngine;
using System.Collections;

public class setUIColour : MonoBehaviour {
	public GameObject daddy;
	int team;
	public Sprite[] tex;
	// Use this for initialization
	void Start () {
		team = daddy.GetComponent<DestructableBuilding> ().team;

		SpriteRenderer rend = GetComponent<SpriteRenderer>();
		rend.sprite = tex[team - 1];
	}
	
	// Update is called once per frame
	void Update () {

	}
}
