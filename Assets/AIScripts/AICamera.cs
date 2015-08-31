using UnityEngine;
using System.Collections;

public class AICamera : MonoBehaviour {
	public Texture2D selectionTexture=null;
	public static Rect selectedArea = new Rect(0,0,0,0);
	private Vector3 initialClick = -Vector3.one;
	
	// Update is called once per frame
	void Update () {
		CheckSelection ();
	}

	void CheckSelection(){
		if(Input.GetMouseButtonDown(0)){
			initialClick = Input.mousePosition;
		}
		else if(Input.GetMouseButtonUp(0)){

			initialClick = - Vector3.one;
		}
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

	void OnGUI(){
		if(initialClick != -Vector3.one){
			GUI.color = new Color(1,1,1,0.3f);
			GUI.DrawTexture(selectedArea, selectionTexture);
		}
	}
}
