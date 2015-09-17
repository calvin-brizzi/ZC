using UnityEngine;
using System.Collections;

public class Attack : MonoBehaviour {
	int damage =10;
	void DoDamage(GameObject unit){
		unit.GetComponent<Unit>().health-=damage;
	}
}
