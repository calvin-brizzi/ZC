using UnityEngine;
using System.Collections;

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
	int damage;
	float damageDelay;
	float damageWait;
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
	public enum TargetType{Unit,Building};
	public enum State{Gathering,Attacking,Moving,Idle,Dead};
	public Resource.ResourceType resourceType;
	public Type unitClass;
	public TargetType targetType;
	public State state;
	int layerMask;
	void Awake(){
		//Sets the damge values depending on the class
		if (unitClass == Type.Grunt) {
			damage = 1;
		}else if (unitClass == Type.Warrior) {
			damage = 10;
		}else if (unitClass == Type.Archer) {
			damage =5;
		}
		//
		health = 100;
		attacking = false;
		gameObject.renderer.material.mainTexture = textures [team-1];
		//Since only grunt unit has a gathering animation
		if(unitClass == Type.Grunt){
			gatherStateHash = Animator.StringToHash("Base Layer.Gather");
		}
		//Gets the indexes of the animations so can be played when needed;
		attackStateHash = Animator.StringToHash("Base Layer.Attack");
		idleStateHash = Animator.StringToHash("Base Layer.Idle");
		runStateHash = Animator.StringToHash("Base Layer.Run");
		deathStateHash = Animator.StringToHash("Base Layer.Death");
		//Gets the animator controller for each unit
		anim = GetComponent<Animator>();
		//Sets variables 
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
		damageDelay=1f;
	 	damageWait = 0;

	}

	void FixedUpdate(){
		if(state!=State.Dead){ // If the unit is not dead
			if (attacking && target != null) {
				transform.LookAt(target.transform);//Makes unit look at current attack target
			}
			if(health<=0){//Checks to see if target is dead
				state=State.Dead;
			}
			CheckState ();//Checks the state of the target
			int number = CheckForEnemies ();
			int targetHealth = 0;
			//Gets the health of the target
			if(target!=null){
				if(targetType==TargetType.Unit){
					targetHealth=target.gameObject.GetComponent<Unit>().health;
				}else if(targetType==TargetType.Building){
					targetHealth=target.gameObject.GetComponent<DestructableBuilding>().health;
				}
			}
			//If target is out of range or dead remove it as target
			if (target != null && !instructedAttack && Vector3.Distance(target.transform.position,transform.position)>=((float)attackRange+4) || targetHealth<=0) {
				target = null;
				attacking =false;
				if(state!=State.Moving){
					state=State.Idle;
				}
			}
			//FOR GATHERING//
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
			//////////
			if (renderer.isVisible && Input.GetMouseButton (0)) { // Helps the selection of troops either multiple or single troop selection
				if(!clicked){
					Vector3 cameraPosition = Camera.mainCamera.WorldToScreenPoint (transform.position);
					cameraPosition.y=Screen.height-cameraPosition.y;
					selected=AICamera.selectedArea.Contains (cameraPosition) ;
					if(selected && !UnitMonitor.selectedUnits.Contains(this.gameObject) && UnitMonitor.LimitNotReached()&& this.team == AICamera.team){
						UnitMonitor.AddUnit(this.gameObject);
						wasSelected=true;
					}

					//If either of the shift buttons are pushed then dont deselct it just add it
					else if(!selected && wasSelected && !UnitMonitor.isShiftPressed()){
						UnitMonitor.RemoveUnit(this.gameObject);
						wasSelected=false;
					}
				}
				//Create the particle effect object that shows which object is selected
				if(wasSelected && glow == null){
					glow = (GameObject)GameObject.Instantiate(glowSelection);
					glow.transform.parent=transform;
					glow.transform.localPosition=new Vector3(0,0,0);
				}
				//If unselected remove it
				else if(!wasSelected && glow!=null){
					GameObject.Destroy (glow);
					glow=null;
				}

			}
			print (wasSelected);//Debugging
			if (Input.GetMouseButtonDown(1) && wasSelected) // Detects a players right click  and moves the selected troops top that position
			{
				path = null;
				StopCoroutine("FollowPath");//Stops the players movement
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

				if (Physics.Raycast(ray, out hit))
				{
					mouseClick = hit.point;
					notOverrideable = true;
					instructedAttack=false;
					print(hit.collider.gameObject.tag);
					Transform attackPoint = null;
					//If enemy unit attack
					if(hit.collider.gameObject.tag=="Unit" && hit.collider.gameObject.GetComponent<Unit>().team!=this.team){
						//Atack
						this.target = hit.collider.gameObject;
						targetType=TargetType.Unit;
						attacking = true;
						instructedAttack = true;
						notOverrideable=false;
						print ("Attack unit");
					}
					//If enemy building attack
					if(hit.collider.gameObject.tag=="Building" && hit.collider.gameObject.GetComponent<DestructableBuilding>().team!=this.team){
						//Atack
						this.target = hit.collider.gameObject;
						targetType=TargetType.Building;
						attacking = true;
						instructedAttack = true;
						notOverrideable=false;
						attackPoint = hit.transform.Find("AttackPoint");
						print ("Attack Building");
					}
					//If resource and grunt start gathering
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
					//If not grunt just stop moving
					else if(hit.collider.gameObject.tag=="Resource"||hit.collider.gameObject.tag=="Home Base"  && !unitClass.Equals(Type.Grunt)){
						state=State.Idle;
					}
					//Return to homebase and deposit goods
					else if(hit.collider.gameObject.tag=="Home Base"){
						var returnPoint = hit.transform.Find("ReturnPoint");
						attacking=false;
						if(unitClass.Equals(Type.Grunt)){
							depositing =true;
							MoveUnit(transform.position,returnPoint.position);
						}
					}
					else{//Just move the unit
						gathering=false;
						returning = false;
						collectGoods=false;
						currentLoad = 0;
						print("Moving");
						if(targetType==TargetType.Building && attackPoint!=null){
							MoveUnit(transform.position,attackPoint.position);
						}else{
							MoveUnit(transform.position,mouseClick);
						}
					}
				}
			}
		}
	}
	//Perimeter check to see if enemies are close by and if so start attacking them
	int CheckForEnemies(){
		int returnAmount = 0;
		if (!attacking && state!=State.Moving) {
			int count = 0;
			Collider[] nearbyEnemy = Physics.OverlapSphere (transform.position, attackRange+4,layerMask); // Returns an array of all enemies in attackrange+4 area
			for (var i =0; i< nearbyEnemy.Length; i++) {
				Unit unit;
				unit = nearbyEnemy [i].GetComponent<Unit> () as Unit;
				if (unit != null && unit.team != this.team) {
					targetType = TargetType.Unit;
					Attack (nearbyEnemy [i].gameObject);
					count ++;//Numbers of enemies
				}
			}
			returnAmount =count;
		}
		return returnAmount;
	}
	//Checks the state of the unit and plays the correct animation
	void CheckState(){
		if (state == State.Moving) {
			anim.Play(runStateHash);
		}else if (state == State.Attacking) {
			anim.Play(attackStateHash);
			if(target!=null){
				DoDamage();
			}
		}else if (state == State.Gathering) {
			anim.Play(gatherStateHash);
		}else if (state == State.Idle) {
			anim.Play(idleStateHash);
		}else if (state == State.Dead) {
			anim.Play(deathStateHash);
			StopCoroutine("FollowPath");
			Invoke ("RemoveUnit",1);
		}
	}

	void DoDamage(){
		if (Time.time >= damageWait) {
			if (targetType == TargetType.Building) {
				target.gameObject.GetComponent<DestructableBuilding> ().health -= damage;
			} else if (targetType == TargetType.Unit) {
				target.gameObject.GetComponent<Unit> ().health -= damage;
			}
			damageWait = damageDelay+Time.time;
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
	//Starts an attack on a specific unit
	void Attack(GameObject targetObj){
		this.target = targetObj;
		targetType = TargetType.Unit;
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
	//If the PathRequestController finds a path follow it
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
	//Moves the unit along the path
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
				if(target!= null && attacking && (Vector3.Distance(target.transform.position,transform.position)<=((float)attackRange+4)|| (targetType==TargetType.Building && Vector3.Distance(target.transform.Find("AttackPoint").position,transform.position)<=((float)attackRange+30)))&& ! notOverrideable){
					attacking = false;
					state = State.Attacking;
					instructedAttack=false;//So when opponent moves unit does keep attacking
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
					else if(target!=null && attacking && Vector3.Distance(transform.position,target.transform.position)<=attackRange+30&& ! notOverrideable){
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
