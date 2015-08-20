using UnityEngine;
using System.Collections;
/*[TODO] Work on path smotthing
 *[TODO] Work of flocking troops
 *[TODO] Make each character look like the correct character
 *[TODO] Give each character the correct animation depending on the task assigned
 *[TODO] Delay Grunts at resource to allow for gathering animation
*/
public class Unit : MonoBehaviour {
	Vector3 mouseClick;
    int speed=20;
    Vector3[] path;
	PathRequestController request;
	bool gathering;
	bool returning;
	GameObject mainBuilding;
	Vector3 resourcePoint;
	public enum Type{Grunt, Archer, Warrior};
	public Type unitClass;
	void Awake(){
		mainBuilding = GameObject.FindGameObjectWithTag ("Home Base");
		gathering = false;
	}
	void Update(){
		if (Input.GetMouseButtonDown(1))
		{
			path = null;
			StopCoroutine("FollowPath");
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit))
			{
				mouseClick = hit.point;
				if(hit.collider.gameObject.tag=="Resource"  && unitClass.Equals(Type.Grunt)){
					var gatherPoint = hit.transform.Find("GatherPoint");
					gathering = true;
					if(gatherPoint){
						resourcePoint = gatherPoint.position;
						PathRequestController.RequestPath(transform.position,gatherPoint.position,OnPathFound);
					}
				}
				else{
					gathering=false;
					PathRequestController.RequestPath(transform.position,mouseClick,OnPathFound);
				}
			}
		}
	}

	public void StartGathering(){
		var returnPoint = mainBuilding.transform.Find ("ReturnPoint");
		if(returnPoint){
			if(!returning){
				Debug.Log("Moving");
				PathRequestController.RequestPath(resourcePoint,returnPoint.position,OnPathFound);
				returning=true;
			}
			else{
				PathRequestController.RequestPath(returnPoint.position,resourcePoint,OnPathFound);
				returning = false;
			}
		}
	}

    public void OnPathFound(Vector3[] newPath, bool success)
    {

        if (success)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath()
    {
		if (path != null && path.Length>0) {
			int targetPosition = 0;
			Vector3 waypoint = path [0];
			while (true) {
				if (path!=null && transform.position == waypoint) {
					targetPosition++;
					if (path!=null && targetPosition >= path.Length) {
						path = new Vector3[0];
						yield break;
					}
					waypoint = path [targetPosition];
				}
				waypoint.y = 1;
				Debug.DrawLine (transform.position, waypoint);
				transform.position = Vector3.MoveTowards (transform.position, waypoint, speed * Time.deltaTime);
				yield return null;
				if(transform.position.x == path[path.Length-1].x && transform.position.z == path[path.Length-1].z){
					if (gathering) {
						Debug.Log ("Gathering");
						StartGathering();
					}
				}
			}

		}


    }

	Vector3[] SmoothPath(Vector3[] pathToSmooth){
		Debug.Log (pathToSmooth.Length);
		Vector3[] smoothPath = new Vector3[(pathToSmooth.Length ) * 2 - 4];
		smoothPath [0] = pathToSmooth [0];
		smoothPath [smoothPath.Length - 1] = pathToSmooth [pathToSmooth.Length - 1];//End point is the same
		int position = 1;
		for (int i =0; i<pathToSmooth.Length - 2; i++) {
			smoothPath[position] = pathToSmooth[i] +(pathToSmooth[i+1]-pathToSmooth[i]);
			smoothPath[position+1] = pathToSmooth[i+1] + (pathToSmooth[i+2]-pathToSmooth[i+1])*0.25f;
			position+=2;
		}
		return smoothPath;
	}

}
