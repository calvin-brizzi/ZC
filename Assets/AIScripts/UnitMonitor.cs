using UnityEngine;
using System.Collections;
//Stores a list of all selected objects
public class UnitMonitor : MonoBehaviour {
	public static ArrayList selectedUnits = new ArrayList();
	static int MAX_UNITS_SELECTED = 25; 
	//Removes unit from selection
	public static void RemoveUnit(GameObject unit){
		for (int i =0; i<selectedUnits.Count; i++) {
			GameObject unitInArrayList = selectedUnits[i] as GameObject;
			if(unit == unitInArrayList){
				selectedUnits.RemoveAt(i);
			}
		}
	}
	//Adds a unit to the selection
	public static void AddUnit(GameObject unit){
		if (!selectedUnits.Contains (unit)) {
			selectedUnits.Add (unit);
		}
	}
	//Checks to see if the max amount of units have been selected
	public static bool LimitNotReached(){
		return MAX_UNITS_SELECTED>selectedUnits.Count;
	}
	//Checks to see if the shift key is pressed
	public static bool isShiftPressed(){
		return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
	}

}
