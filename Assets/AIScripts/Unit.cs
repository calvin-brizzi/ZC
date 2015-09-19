﻿using UnityEngine;
using System.Collections;
/*
 *[TODO] Add attack ability
 *[TODO] Work of flocking troops
 *[TODO] Make each character look like the correct character
 *[TODO] Give each character the correct animation depending on the task assigned
 *[TODO] Change the colour of the glow depending on team
 *[TODO] Stop the units from attacking when out of range
 *[TODO] Stop the units on the other team from being able to be selected
*/
public class Unit : MonoBehaviour {
	public GameObject marker;
	int idleStateHash;
	int runStateHash;
	public int health;
	int attackStateHash; 
	int gatherStateHash; 
	int deathStateHash; 
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
	bool notOverrideable;
	bool depositing;
	bool wasSelected;
	bool clicked;
	bool collectGoods;
	bool attacking;
	bool instructedAttack = false;
	int NumberOfEnemies=0;
	GameObject mainBuilding;
	Vector3 resourcePoint;
	Animator anim;
	GameObject target;
	public Texture[] textures;
	public int attackRange;
	public int team;
	public GameObject glowSelection;
	private GameObject glow;
	public enum Type{Grunt, Archer, Warrior};
	public enum State{Gathering,Attacking,Moving,Idle,Dead};
	public Resource.ResourceType resourceType;
	public Type unitClass;
	public State state;
	int layerMask;
	void Awake(){
		health = 100;
		attacking = false;
//		gameObject.renderer.material.mainTexture = textures [team-1];
		if(unitClass == Type.Grunt){
			gatherStateHash = Animator.StringToHash("Base Layer.Gather");
		}
		attackStateHash = Animator.StringToHash("Base Layer.Attack");
		idleStateHash = Animator.StringToHash("Base Layer.Idle");
		runStateHash = Animator.StringToHash("Base Layer.Run");
		deathStateHash = Animator.StringToHash("Base Layer.Death");
		anim = GetComponent<Animator>();
		collectedAmount = 0;
		duration = 0.01f;
		speed = 20;
		MAX_LOAD = 100;
		currentLoad = 0;
		gatherSpeed = 1;
		glow = null;
		mainBuilding = GameObject.FindGameObjectWithTag ("Home Base");
		gathering = false;
		state = State.Idle;
		wasSelected = false;
		layerMask=1<<10;

	}
	void FixedUpdate(){
		if(state!=State.Dead){
			if (attacking && target != null) {
				transform.LookAt(target.transform);
			}
			if(health<=0){
				state=State.Dead;
			}
			CheckState ();
			int number = CheckForEnemies ();
			int targetHealth = 0;
			if(target!=null){
				targetHealth=target.gameObject.GetComponent<Unit>().health;
			}
			if (target != null && !instructedAttack && Vector3.Distance(target.transform.position,transform.position)>=((float)attackRange+4) || targetHealth<=0) {
				target = null;
				attacking =false;
				if(state!=State.Moving){
					state=State.Idle;
				}
			}
			if ((MAX_LOAD==currentLoad)||(collectGoods && gathering && currentResource == null)) { // If the unit has reached its max load return to base or If the resource is destroyed and the grunt has not filled its capacity
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
			//print (wasSelected);
			if (Input.GetMouseButtonDown(1) && wasSelected) // Detects a players right click  and moves the selected troops top that position
			{
				path = null;
				StopCoroutine("FollowPath");
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hit))
				{
					mouseClick = hit.point;
					notOverrideable = true;
					instructedAttack=false;
					if(hit.collider.gameObject.tag=="Unit" && hit.collider.gameObject.GetComponent<Unit>().team!=this.team){
						//Atack
						this.target = hit.collider.gameObject;
						attacking = true;
						instructedAttack = true;
						notOverrideable=false;
						print ("Attack unit");
					}
					if(hit.collider.gameObject.tag=="Resource"  && unitClass.Equals(Type.Grunt)){
						currentResource = hit.transform.gameObject;
						if(resourceType!=Resource.ResourceType.Nothing){
							collectedAmount=0;
							currentLoad=0;
						}
						resourceType = currentResource.GetComponent<Resource>().type;
						var gatherPoint = hit.transform.Find("GatherPoint");
						gathering = true;
						attacking=false;
						if(gatherPoint){
							collectGoods=false;
							returning = false;
							resourcePoint = gatherPoint.position;
							MoveUnit(transform.position,gatherPoint.position);
						}
					}
					else if(hit.collider.gameObject.tag=="Home Base"){
						var returnPoint = hit.transform.Find("ReturnPoint");
						attacking=false;
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
						print("Moving");
						MoveUnit(transform.position,mouseClick);
					}
				}
			}
		}
	}

	int CheckForEnemies(){
		int returnAmount = 0;
		if (!attacking && state!=State.Moving) {
			int count = 0;
			Collider[] nearbyEnemy = Physics.OverlapSphere (transform.position, attackRange,layerMask);
			for (var i =0; i< nearbyEnemy.Length; i++) {
				Unit unit;
				unit = nearbyEnemy [i].GetComponent<Unit> () as Unit;
				if (unit != null && unit.team != this.team) {
					Attack (nearbyEnemy [i].gameObject);
					count ++;
				}
			}
			returnAmount =count;
		}
		return returnAmount;
	}
	void CheckState(){
		if (state == State.Moving) {
			anim.Play(runStateHash);
		}else if (state == State.Attacking) {
			anim.Play(attackStateHash);
			if(target!=null){
				target.gameObject.GetComponent<Unit>().health-=1;
			}
		}else if (state == State.Gathering) {
			anim.Play(gatherStateHash);
		}else if (state == State.Idle) {
			anim.Play(idleStateHash);
		}else if (state == State.Dead) {
			anim.Play(deathStateHash);
			Invoke ("RemoveUnit",1);
		}
	}
	void RemoveUnit(){
		Destroy(this.gameObject);
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
	public void Gather(){
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

	void Attack(GameObject targetObj){
		this.target = targetObj;
		state = State.Attacking;
		attacking = true;
		transform.LookAt (target.transform);
	}

	void StopAttacking(){
		this.target = null;
		state = State.Idle;
		attacking = false;
	}
	
	//Moves the unit from one point to another
	public void MoveUnit(Vector3 from, Vector3 to){
		PathRequestController.RequestPath(from,to,OnPathFound);
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
		if (this.health > 0 && path != null && path.Length>0) {
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
				transform.LookAt(waypoint);
				yield return null;
				if(target!=null){
					print ((target!= null)+ "&&" + attacking +"&&"+ (Vector3.Distance(target.transform.position,transform.position)<=((float)attackRange+4))+"&&"+ ! notOverrideable);
				}
				if(target!= null && attacking && Vector3.Distance(target.transform.position,transform.position)<=((float)attackRange+4)&& ! notOverrideable){
					attacking = false;
					state = State.Attacking;
					print ("Attacking");
					StopCoroutine ("FollowPath");
				}

				if(transform.position.x == path[path.Length-1].x && transform.position.z == path[path.Length-1].z){
					notOverrideable=false;
					if (gathering) { //So that the gathering movement happens automatically
						Gather();
					}
					else if(depositing){ // This is for when a grunt is carrying good and the player deposits it manuallly
						depositing =false;
						collectedAmount=0;
					}
					else if(target!=null && attacking && Vector3.Distance(transform.position,target.transform.position)<=attackRange+4&& ! notOverrideable){
						Attack(target);
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
