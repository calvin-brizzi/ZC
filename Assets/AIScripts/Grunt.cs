using UnityEngine;
using System.Collections;

public class Grunt : MonoBehaviour {
	public static int MAX_CAPACITY=30;
	public static int currentLoad=0;

	public static void Gather(){
		if (!Full ()) {
			currentLoad++;
		}
	}

	public static bool Full(){
		return MAX_CAPACITY == currentLoad;
	}
}
