using UnityEngine;
using System.Collections;
/*[TODO] Add what type of material it is carrying
 *[TODO] Add attack ability
 *[TODO] Work of flocking troops
 *[TODO] Make each character look like the correct character
 *[TODO] Give each character the correct animation depending on the task assigned
 *[TODO] Make grunt check if the second resource is the same as the one it current holds if so just add else remove
 *[TODO] Add a rally point marker
*/
public class Unit : MonoBehaviour {
	Vector3 mouseClick;
    int speed;
	int currentLoad;
	int MAX_LOAD;
	int gatherSpeed;
	int collectedAmount;
	float duration;
	float coolDown;//To prevent rapidly clicking the same point over and over and therefore causing issues
    Vector3[] path;
	PathRequestController request;
	GameObject currentResource;
	bool gathering;
	bool returning;
	bool selected;
	bool depositing;
	bool wasSelected;
	bool clicked;
	bool collectGoods;
	GameObject mainBuilding;
	Vector3 resourcePoint;
	public GameObject glowSelection;
	private GameObject glow;
	public enum Type{Grunt, Archer, Warrior};
	public enum State{Gathering,Attacking,Moving,Idle};
	public enum ResourceType{Lava,Rock};
	public Type unitClass;
	public State state;
	void Awake(){
		collectedAmount = 0;
		duration = 0.4f;
		speed = 20;
		MAX_LOAD = 100;
		currentLoad = 0;
		gatherSpeed = 1;
		glow = null;
		mainBuilding = GameObject.FindGameObjectWithTag ("Home Base");
		gathering = false;
		state = State.Idle;
		//Play idle animation
		wasSelected = false;
	}
	void Update(){
		if (MAX_LOAD==currentLoad) { // If the unit has reached its max load return to base
			collectGoods=false;
			collectedAmount=currentLoad;
			currentLoad=0;
			StartCoroutine("FollowPath");
		}
		if (unitClass == Type.Grunt && collectGoods && MAX_LOAD!=currentLoad && currentResource!=null){// While gathering goods increase current load
			currentLoad+=gatherSpeed;
			currentResource.GetComponent<Resource>().ReduceAmountOfMaterial(gatherSpeed);
			collectedAmount=currentLoad;
		}
		if (collectGoods && gathering && currentResource == null) {
			collectGoods=false;
			collectedAmount=currentLoad;
			currentLoad=0;
			StartCoroutine("FollowPath");
		}
		if (renderer.isVisible && Input.GetMouseButton (0)) { // Helps the selection of troops either multiple or single troop selection
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

		if (Input.GetMouseButtonDown(1) && wasSelected && Time.time > coolDown) // Detects a players right click  and moves the selected troops top that position
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
					currentResource = hit.transform.gameObject;
					var gatherPoint = hit.transform.Find("GatherPoint");
					gathering = true;
					if(gatherPoint){
						collectGoods=false;
						returning = false;
						resourcePoint = gatherPoint.position;
						MoveUnit(transform.position,gatherPoint.position);
					}
				}
				else if(hit.collider.gameObject.tag=="Home Base"){
					var returnPoint = hit.transform.Find("ReturnPoint");
					if(unitClass.Equals(Type.Grunt)){
						depositing =true;
						MoveUnit(transform.position,returnPoint.position);
					}
				}
				else{
					gathering=false;
					returning = false;
					collectGoods=false;
					currentLoad = 0;
					MoveUnit(transform.position,mouseClick);
				}
			}
		}
	}
	//Moves the unit from one point to another
	void MoveUnit(Vector3 from, Vector3 to){
		PathRequestController.RequestPath(from,to,OnPathFound);
	}
	//Handles selection of the troops
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
	//Starts the gathering movement of the grunt
	public void StartGathering(){
		var returnPoint = mainBuilding.transform.Find ("ReturnPoint");
		if(returnPoint){
			if(!returning){//While the unit is collecting goods
				MoveUnit(resourcePoint,returnPoint.position);
				returning=true;
			}
			else{//When gathering and the unit has reached the home base to return the collected goods
				MoveUnit(returnPoint.position,resourcePoint);
				collectedAmount=0;
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
		} else if (success && gathering && !returning ) {
			path=newPath;
			if(currentResource==null){
				gathering = false;
			}
			StopCoroutine ("FollowPath");
			StartCoroutine("FollowPath");
		} else if(success && returning && currentResource!=null){
			path=newPath;
			StopCoroutine ("FollowPath");
			collectGoods=true;
			state=State.Gathering;
			//Play gathering animation
		}
    }

    IEnumerator FollowPath()
    {
		if (path != null && path.Length>0) {
			state=State.Moving;
			//Play moving animation
			path = SmoothPath(path);
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
					if (gathering) { //So that the gathering movement happens automatically
						StartGathering();
					}
					else if(depositing){ // This is for when a grunt is carrying good and the player deposits it manuallly
						depositing =false;
						collectedAmount=0;
					}
					else{
						state=State.Idle;
					}
				}
			}

		}
    }
	//Chaikin path smoothing algorithm
	Vector3[] SmoothPath(Vector3[] pathToSmooth){
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
