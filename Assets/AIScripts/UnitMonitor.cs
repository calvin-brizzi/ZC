using UnityEngine;
using System.Collections;

public class UnitMonitor : MonoBehaviour {
	static int selectedUnits=0;
	static int MAX_SELECTION=10;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	public static void incrementSelected(){
		selectedUnits++;
		Debug.Log (selectedUnits);
	}

	public static void decrementSelected(){
		selectedUnits--;
		Debug.Log (selectedUnits);
	}

	public static bool reachedMaximum(){

		return selectedUnits == MAX_SELECTION;
	}
}
