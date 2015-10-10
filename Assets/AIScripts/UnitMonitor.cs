using UnityEngine;
using System.Collections;

//Stores a list of all selected objects
public class UnitMonitor : MonoBehaviour {
	public static ArrayList selectedUnits = new ArrayList();
	static int MAX_UNITS_SELECTED = 20; 
	public static int unitCount;
	//Removes unit from selection
	public static void RemoveUnit(GameObject unit){
		for (int i =0; i<selectedUnits.Count; i++) {
			GameObject unitInArrayList = selectedUnits[i] as GameObject;
			if(unit == unitInArrayList){
				selectedUnits.RemoveAt(i);
				unitCount-=1;
			}
		}
	}
	//Adds a unit to the selection
	public static void AddUnit(GameObject unit){
		if (!selectedUnits.Contains (unit)) {
			selectedUnits.Add (unit);
			unitCount+=1;
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

	public static void CreateWaypointGrid(){
		print (selectedUnits.Count);
		for (int i =0; i<selectedUnits.Count; i++) {
			GameObject unitInArrayList = selectedUnits [i] as GameObject;
			Vector3[] path = unitInArrayList.GetComponent<Unit> ().path;
			if(path!=null){
				unitInArrayList.GetComponent<Unit> ().path[path.Length-1]= path[path.Length-1]+new Vector3(0.2f*unitCount,0,0.2f*unitCount);
			}
		}
	}

}
