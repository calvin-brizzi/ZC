using UnityEngine;
using System.Collections;

public class UnitMonitor : MonoBehaviour {
	public static ArrayList selectedUnits = new ArrayList();
	static int MAX_UNITS_SELECTED = 25; 
	public static void RemoveUnit(GameObject unit){
		for (int i =0; i<selectedUnits.Count; i++) {
			GameObject unitInArrayList = selectedUnits[i] as GameObject;
			if(unit == unitInArrayList){
				selectedUnits.RemoveAt(i);
			}
		}
	}

	public static void AddUnit(GameObject unit){
		if (!selectedUnits.Contains (unit)) {
			selectedUnits.Add (unit);
		}
	}

	public static bool LimitNotReached(){
		return MAX_UNITS_SELECTED>selectedUnits.Count;
	}

	public static bool isShiftPressed(){
		return (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift));
	}


}
