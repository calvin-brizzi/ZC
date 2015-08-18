using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneMan : MonoBehaviour {

	public static SceneMan Man;
	public List<IUpdatable> UpdatableObjects;

	// Use this for initialization
	void Awake () {
		Man = this;
		UpdatableObjects = new List<IUpdatable>();
	}
}
