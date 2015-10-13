using UnityEngine;
using System.Collections;

public class TowerShoot : MonoBehaviour {
	public int team;
	public GameObject currentTarget;
	float shoot;
	// Use this for initialization
	void Start () {
		team = VarMan.Instance.pNum;
		shoot = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		float currentTime = Time.time;
		if (currentTarget) {
			if(currentTime>shoot){
				if(currentTarget.GetComponent<Unit>().health>0){
					currentTarget.GetComponent<Unit>().health -=10;
				}

				shoot = currentTime + 0.5f;
			}
			Vector3 dist = this.transform.position - currentTarget.transform.position;
			if(dist.magnitude>10){
				currentTarget = null;
			}
		} else {
			Collider[] hitColliders = Physics.OverlapSphere (this.transform.position, 10f);

			for (int i =0; i<hitColliders.Length; i++) {
				int c = 0;

				if(hitColliders[i].gameObject.tag == "Grunt"){
					c = hitColliders [i].gameObject.GetComponent<Unit> ().team;

				}
					//c = hitColliders [i].gameObject.GetComponent<Unit> ().team;
				if (c != 0) {
					print (c+" this is c");
					if (c != team) {
						currentTarget = hitColliders [i].gameObject;
					}
				}
			}
		}
	}
}
