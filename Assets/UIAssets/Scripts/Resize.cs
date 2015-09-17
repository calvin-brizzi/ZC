using UnityEngine;
using System.Collections;

public class Resize : MonoBehaviour {
	public GameObject[] units;
	public GameObject[] buildings;
	public GameObject[] buildOptions;
	public GameObject HUD;
	// Use this for initialization
	void Start () {
		HUD.transform.SetAsFirstSibling ();
		print (Screen.width);
		int width = Screen.width;
		int height = Screen.height;

		RectTransform temp = HUD.transform.GetComponent<RectTransform>();
		float x = 0.35f;
		float y = 0.1f;


		Vector3 tempP = HUD.transform.position;
		Vector3 tempS = HUD.transform.localScale;
		tempP.x = Screen.width * 0.475f;
		tempP.y = Screen.height * 0.102f;
		//tempP.x = Screen.width * 0.54f;
		//tempP.y = Screen.height * 0.102f;
		tempS.x = 9.670751f * (Screen.width / 942f);
		tempS.y = 4.038905f * (Screen.height / 497f);


		print ("prints this "+Screen.width * 0.0642250531f);
		HUD.transform.position = tempP;
		HUD.transform.localScale = tempS;


		for (int i =0; i<units.Length; i++) {
			Vector3 tempPosition = units[i].transform.position;
			Vector3 tempScale =units[i].transform.localScale;

			tempPosition.x = Mathf.RoundToInt(width*x);
			tempPosition.y =  Mathf.RoundToInt(height*y);
			x = x+0.18f;
			units[i].transform.position = tempPosition;

			print (width/942f);
			tempScale.x = width/942f;
			tempScale.y = (height/497f)*2.5f;
			units[i].transform.localScale = tempScale;
		

		}
		x = 0.35f;
		for (int i =0; i<buildOptions.Length; i++) {
			Vector3 tempPosition = buildOptions[i].transform.position;
			Vector3 tempScale =buildOptions[i].transform.localScale;
			
			tempPosition.x = Mathf.RoundToInt(width*x);
			tempPosition.y =  Mathf.RoundToInt(height*y);
			x = x+0.18f;
			buildOptions[i].transform.position = tempPosition;

			
			tempScale.x = width/942f;
			tempScale.y = (height/497f)*2.5f;

			buildOptions[i].transform.localScale = tempScale;
			
		}
		x = 0.35f;
		for (int i =0; i<buildings.Length; i++) {
			Vector3 tempPosition = buildings[i].transform.position;
			Vector3 tempScale =buildings[i].transform.localScale;
			
			tempPosition.x = Mathf.RoundToInt(width*x);
			tempPosition.y =  Mathf.RoundToInt(height*y);
			x = x+0.18f;
			buildings[i].transform.position = tempPosition;
			
			
			tempScale.x = width/942f;
			tempScale.y = (height/497f)*2.5f;
			
			buildings[i].transform.localScale = tempScale;
			
		}
	}
	
	// Update is called once per frame
	void Update () {

	}
}
