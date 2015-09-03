using UnityEngine;
using System.Collections;
/*
 *[TODO] Work of flocking troops
 *[TODO] Make each character look like the correct character
 *[TODO] Give each character the correct animation depending on the task assigned
 *[TODO] Delay Grunts at resource to allow for gathering animation
*/
public class Unit : MonoBehaviour {
	Vector3 mouseClick;
    int speed=20;
	float duration;
	float coolDown;//To prevent rapidly clicking the same point over and over and therefore causing issues
    Vector3[] path;
	PathRequestController request;
	bool gathering;
	bool returning;
	bool selected;
	bool wasSelected;
	bool clicked;
	GameObject mainBuilding;
	Vector3 resourcePoint;
	public GameObject glowSelection;
	private GameObject glow;
	public enum Type{Grunt, Archer, Warrior};
	public enum State{Gathering,Attacking,Moving,Idle};
	public Type unitClass;
	public State state;
	void Awake(){
		duration = 0.4f;
		glow = null;
		mainBuilding = GameObject.FindGameObjectWithTag ("Home Base");
		gathering = false;
		state = State.Idle;
		wasSelected = false;
	}
	void Update(){

		if (renderer.isVisible && Input.GetMouseButton (0)) {
			if(!clicked){
				Vector3 cameraPosition = Camera.mainCamera.WorldToScreenPoint (transform.position);
				cameraPosition.y=Screen.height-cameraPosition.y;
				selected=AICamera.selectedArea.Contains (cameraPosition) ;
				if(selected && !UnitMonitor.selectedUnits.Contains(this.gameObject) && UnitMonitor.LimitNotReached() ){
					UnitMonitor.AddUnit(this.gameObject);
					wasSelected=true;
				}
				//If either of the shift buttons are pushed then dont deselct it just add it
				else if(!selected && wasSelected && !UnitMonitor.isShiftPressed()){
					UnitMonitor.RemoveUnit(this.gameObject);
					wasSelected=false;
				}
			}

			if(wasSelected && glow == null){
				glow = (GameObject)GameObject.Instantiate(glowSelection);
				glow.transform.parent=transform;
				glow.transform.localPosition=new Vector3(0,-GetComponent<MeshFilter>().mesh.bounds.extents.y,0);
			}
			else if(!wasSelected && glow!=null){
				GameObject.Destroy (glow);
				glow=null;
			}
		}

		if (Input.GetMouseButtonDown(1) && wasSelected && Time.time > coolDown)
		{
			coolDown = Time.time + duration;
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

	void OnMouseDown(){
		clicked = true;
		wasSelected = true;
	}

	void OnMouseUp(){
		if(clicked){
			wasSelected = true;
		}
		clicked = false;
	}

	public void StartGathering(){
		var returnPoint = mainBuilding.transform.Find ("ReturnPoint");
		if(returnPoint){
			if(!returning){
				state = State.Moving;
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
        if (success && !gathering) {
			path = newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		} else if (success && gathering && !returning) {
			path=newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine("FollowPath");
		} else if(success && returning){
			path=newPath;
			Debug.Log ("Waiting");
			StopCoroutine ("FollowPath");
			StartCoroutine("WaitAndFollow");
		}
    }

	IEnumerator WaitAndFollow(){	
		yield return new WaitForSeconds (2);
		StartCoroutine ("FollowPath");

	}

    IEnumerator FollowPath()
    {
		if (path != null && path.Length>0) {
			path=SmoothPath(path);
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
				waypoint.y = transform.position.y;//So that the units always remain the same height
				Debug.DrawLine (transform.position, waypoint);
				transform.position = Vector3.MoveTowards (transform.position, waypoint, speed * Time.deltaTime);
				yield return null;
				if(transform.position.x == path[path.Length-1].x && transform.position.z == path[path.Length-1].z){
					if (gathering) {
						StartGathering();
					}
				}
			}

		}


    }
	//Chaikin path smoothing algorithm
	Vector3[] SmoothPath(Vector3[] pathToSmooth){
		Debug.Log (pathToSmooth.Length+"");
		if (pathToSmooth.Length > 1) {
			Vector3[] smoothPath = new Vector3[(pathToSmooth.Length - 2) * 2 + 2];
			smoothPath [0] = pathToSmooth [0];
			smoothPath [smoothPath.Length - 1] = pathToSmooth [pathToSmooth.Length - 1];//End point is the same
			int position = 1;
			for (int i =0; i<pathToSmooth.Length - 2; i++) {
				smoothPath [position] = pathToSmooth [i] + (pathToSmooth [i + 1] - pathToSmooth [i]) * 0.75f;
				smoothPath [position + 1] = pathToSmooth [i + 1] + (pathToSmooth [i + 2] - pathToSmooth [i + 1]) * 0.25f;
				position += 2;
			}
			return smoothPath;
		}else{
			return pathToSmooth;
		}
	}


}
