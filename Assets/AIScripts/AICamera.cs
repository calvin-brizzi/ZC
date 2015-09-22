using UnityEngine;
using System.Collections;

public class AICamera : MonoBehaviour {
	public Texture2D selectionTexture=null;
	public int team;
	public static Rect selectedArea = new Rect(0,0,0,0);
	private Vector3 initialClick = -Vector3.one;


	void Awake(){
		team = 1;
	}
	// Update is called once per frame
	void Update () {
		CheckSelection ();

	}
	//Checks for a selection area and creates one when the player clicks and drags
	void CheckSelection(){
		//Checks if the player has clicked to start selection
		if(Input.GetMouseButtonDown(0)){
			initialClick = Input.mousePosition;
		}
		//If ended selection remove selection area box
		else if(Input.GetMouseButtonUp(0)){
			initialClick = - Vector3.one;
		}
		//Generates the selection area
		if(Input.GetMouseButton(0)){
			selectedArea = new Rect(initialClick.x,(Screen.height-initialClick.y),Input.mousePosition.x-initialClick.x,(Screen.height-Input.mousePosition.y) - (Screen.height-initialClick.y));
			if(selectedArea.width<0){
				selectedArea.x += selectedArea.width;
				selectedArea.width = -selectedArea.width;
			}
			if(selectedArea.height<0){
				selectedArea.y += selectedArea.height;
				selectedArea.height = -selectedArea.height;
			}
		}
	}
	//Draws the selection area
	void OnGUI(){
		if(initialClick != -Vector3.one){
			GUI.color = new Color(1,1,1,0.3f);
			GUI.DrawTexture(selectedArea, selectionTexture);
		}
	}
}
