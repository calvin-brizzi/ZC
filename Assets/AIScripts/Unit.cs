using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class Unit : MonoBehaviour
{
	// All out public resources
	public int health;
	public int speed;
	public float attackRange;
	public int team;

	public GameObject marker;
	public bool TargetReached = false;
	public Texture[] textures;
	public Vector3[] path;

	public Resource.ResourceType resourceType;
	public Type unitClass;
	public TargetType targetType;
	public State state;

	public GameObject glowSelection;

	// Some Enums with states
	public enum Type
	{
		Grunt,
		Archer,
		Warrior}
	;
	
	public enum TargetType
	{
		Unit,
		Building}
	;
	
	public enum State
	{
		Gathering,
		Attacking,
		Building,
		Moving,
		Idle,
		Dead}
	;

	//Move into sound controller script
	public AudioClip selectionConfirmation;
	public AudioClip attackConfirmation;
	public AudioClip moveConfirmation;
	public AudioClip death;
	public AudioClip attackHitSoundBuilding;
	public AudioClip attackHitSoundUnit;
	public AudioClip bowDraw;
	public AudioClip gatherConfirmation;


	int idleStateHash;
	int runStateHash;
	int attackStateHash;
	int gatherStateHash;
	int deathStateHash;
	Vector3 mouseClick;

	int damage;
	float damageDelay;
	float damageWait;
	int currentLoad;
	int MAX_LOAD;
	int gatherSpeed;
	int collectedAmount;
	float duration;
	int deathcount = 0;

	//To prevent rapidly clicking the same point over and over and therefore causing issues
	float coolDown;

	PathRequestController request;
	GameObject currentResource;
	GameObject currentConstruction;
	bool building;
	bool gathering;
	bool returning;
	bool selected;
	bool notOverrideable;
	bool depositing;
	bool wasSelected;
	bool clicked;
	bool collectGoods;
	bool attacking;
	bool patrolPointSelection;
	bool patroling;
	bool instructedAttack = false;
	int NumberOfEnemies = 0;
	int patrolPointCount=0;
	GameObject mainBuilding;
	Vector3 resourcePoint;
	Vector3 patrolPoint1;
	Vector3 patrolPoint2;
	Animator anim;
	GameObject target;

	private GameObject glow;
	int layerMask;
	public float StoppingDistance;
	Vector3 direction;
    NetworkView nv;

	void Awake ()
	{
		//StoppingDistance = 10;
		//Sets the damge values depending on the class
		transform.FindChild("Health Bar").gameObject.renderer.enabled=false;
		transform.FindChild("Health Bar").transform.FindChild("Bar").gameObject.renderer.enabled=false;
		if (unitClass == Type.Grunt) {
			damage = 1;
		} else if (unitClass == Type.Warrior) {
			damage = 10;
		} else if (unitClass == Type.Archer) {
			damage = 5;
		}

		health = 100;
		attacking = false;
		//gameObject.renderer.material.mainTexture = textures [team - 1];
	 	
		//Since only grunt unit has a gathering animation
		if (unitClass == Type.Grunt) {
			gatherStateHash = Animator.StringToHash ("Base Layer.Idle");
		}

		//Gets the indexes of the animations so can be played when needed;
		attackStateHash = Animator.StringToHash ("Base Layer.Attack");
		idleStateHash = Animator.StringToHash ("Base Layer.Idle");
		runStateHash = Animator.StringToHash ("Base Layer.Run");
		deathStateHash = Animator.StringToHash ("Base Layer.Death");

		//Gets the animator controller for each unit
		anim = GetComponent<Animator> ();

		//Sets variables 
		collectedAmount = 0;
		duration = 0.01f;
		MAX_LOAD = 300;
		currentLoad = 0;
		gatherSpeed = 1;
		glow = null;
		mainBuilding = GameObject.FindGameObjectWithTag ("Home Base");
		gathering = false;
		state = State.Idle;
		wasSelected = false;
		layerMask = 1 << 10;
		damageDelay = 1f;
		damageWait = 0;
        nv = this.transform.GetComponent<NetworkView>();

	}

	void FixedUpdate ()
	{
		if (state != State.Dead) { // Unit alive

			if (building) {


				if (currentConstruction) {
					Vector3 dist = this.transform.position - currentConstruction.transform.position;
					print (dist.magnitude);
					if (dist.magnitude < 2) {
						currentConstruction.GetComponent<build> ().percentage += 1;
						currentConstruction.GetComponent<build> ().t = team;
					}
				}
			}

			if (Input.GetKeyDown (KeyCode.P) && !patrolPointSelection) {
				patrolPointSelection = true;
				print ("Set patrol points");
				patroling = false;
				patrolPointCount = 0;
			}

			if (TargetReached && !attacking && state!=State.Idle) {
				state = State.Idle;
				nv.RPC("setState", RPCMode.All, (int)State.Idle);
			}
			if (state!=State.Idle && attacking && target != null) {
				transform.LookAt (target.transform);//Makes unit look at current attack target

			}

			if (health <= 0) {//Checks to see if target is dead
				state = State.Dead;
                nv.RPC("setState", RPCMode.All, (int)State.Dead);
				audio.PlayOneShot (death);
			}

			CheckState ();//Checks the state of the target
			if(state!=State.Attacking){
				int number = CheckForEnemies ();
			}
			int targetHealth = 0;

			//Gets the health of the target
			if (target != null) {
				if (targetType == TargetType.Unit) {
					targetHealth = target.gameObject.GetComponent<Unit> ().health;
				} else if (targetType == TargetType.Building) {
					targetHealth = target.gameObject.GetComponent<DestructableBuilding> ().health;
				}
			}
			//If target is out of range or dead remove it as target
			if (state!=State.Idle && target != null && !instructedAttack && Vector3.Distance (target.transform.position, transform.position) >= ((float)attackRange) || targetHealth <= 0) {
				target = null;
				attacking = false;
				if (state != State.Moving) {
					state = State.Idle;
                    nv.RPC("setState", RPCMode.All, (int)State.Idle);
				}
			}

			//Gathering
			if ((MAX_LOAD == currentLoad) || (collectGoods && gathering && currentResource == null)) { // If the unit has reached its max load return to base or If the resource is destroyed and the grunt has not filled its capacity
				collectGoods = false;
				collectedAmount = currentLoad;
				currentLoad = 0;
				StartCoroutine ("FollowPath");
			}

			if (unitClass == Type.Grunt && collectGoods && MAX_LOAD > currentLoad && currentResource != null) {// While gathering goods increase current load
				currentLoad += gatherSpeed;
				Debug.Log (currentLoad);
				currentResource.GetComponent<Resource> ().ReduceAmountOfMaterial (gatherSpeed);
				collectedAmount = currentLoad;
			}

			if (Input.GetMouseButton (0)&& !EventSystem.current.IsPointerOverGameObject ()) {
				// Helps the selection of troops either multiple or single troop selection
				if (!clicked) {
					Vector3 cameraPosition = Camera.mainCamera.WorldToScreenPoint (transform.position);
					cameraPosition.y = Screen.height - cameraPosition.y;
					selected = AICamera.selectedArea.Contains (cameraPosition);
					GameObject aiCamera = GameObject.FindGameObjectWithTag ("MainCamera");

					if (renderer.isVisible && selected && !UnitMonitor.selectedUnits.Contains (this.gameObject) && UnitMonitor.LimitNotReached () && this.team == aiCamera.GetComponent<AICamera> ().team) {
						UnitMonitor.AddUnit (this.gameObject);
						wasSelected = true;
						audio.PlayOneShot (selectionConfirmation);
					} else if (!selected && wasSelected && !UnitMonitor.isShiftPressed ()) {
						//If either of the shift buttons are pushed then dont deselct it just add it
						UnitMonitor.RemoveUnit (this.gameObject);
						wasSelected = false;
						TargetReached = false;
					}
				}
				//Create the particle effect object that shows which object is selected
				if (wasSelected && glow == null) {
					glow = (GameObject)GameObject.Instantiate (glowSelection);
					glow.transform.parent = transform;
					glow.transform.localPosition = new Vector3 (0, 0, 0);
					if(transform.FindChild("Health Bar")){
						transform.FindChild("Health Bar").gameObject.renderer.enabled=true;
						transform.FindChild("Health Bar").transform.FindChild("Bar").gameObject.renderer.enabled=true;
					}
				}

				//If unselected remove it
				else if (renderer.isVisible && !wasSelected && glow != null) {
					GameObject.Destroy (glow);
					glow = null;
					if(transform.FindChild("Health Bar")){
						transform.FindChild("Health Bar").gameObject.renderer.enabled=false;
						transform.FindChild("Health Bar").transform.FindChild("Bar").gameObject.renderer.enabled=false;
					}
				}
			}
			//Makes the units setup a patrol point and patrol between two points
			if (Input.GetMouseButtonDown (1) && patrolPointSelection && wasSelected) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				if (Physics.Raycast (ray, out hit)) {
					mouseClick = hit.point;
					if (patrolPointCount == 0) {
						patrolPointCount = 0;
						patrolPoint1 = mouseClick;
						audio.PlayOneShot (moveConfirmation);
						MoveUnit (transform.position, patrolPoint1);
						patrolPointCount++;
					} else if (patrolPointCount == 1) {
						patrolPoint2 = mouseClick;
						patrolPointSelection = false;
						patroling = true;
						patrolPointCount = 0;
						audio.PlayOneShot (moveConfirmation);
					}

					if (patroling && !patrolPointSelection) {
						Patrol(patrolPoint1,patrolPoint2);
					}
				}
			} else if (Input.GetMouseButtonDown (1) && wasSelected && !patrolPointSelection) { 
				// Detects a players right click  and moves the selected troops top that position
				path = null;
				//Stops the players movement
				StopCoroutine ("FollowPath");
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				Debug.Log ("click");

				if (Physics.Raycast (ray, out hit)) {
					TargetReached = false;
					mouseClick = hit.point;
					notOverrideable = true;
					instructedAttack = false;
					Transform attackPoint = null;
					patroling = false;
					//If enemy unit attack
					if ((hit.collider.gameObject.tag == "Unit" || hit.collider.gameObject.tag == "Grunt") && hit.collider.gameObject.GetComponent<Unit> ().team != this.team) {
						audio.PlayOneShot (attackConfirmation);
						this.target = hit.collider.gameObject;
						targetType = TargetType.Unit;
						attacking = true;
						instructedAttack = true;
						notOverrideable = false;
						print ("Attack unit");
					}

					//If enemy building attack
					if ((hit.collider.gameObject.tag == "Building" || hit.collider.gameObject.tag == "Home Base" || hit.collider.gameObject.tag == "School") && hit.collider.gameObject.GetComponent<DestructableBuilding> ().team != this.team) {
						audio.PlayOneShot (attackConfirmation);
						this.target = hit.collider.gameObject;
						targetType = TargetType.Building;
						attacking = true;
						instructedAttack = true;
						notOverrideable = false;
						attackPoint = hit.transform.Find ("AttackPoint");
						print ("Attack Building");
					}

					//If resource and grunt start gathering

					if (hit.collider.gameObject.tag == "Scafold" && unitClass.Equals (Type.Grunt)) {

						var buildPoint = hit.transform.FindChild ("BuildPoint");
						if (buildPoint) {
							building = true;
							print (buildPoint.localPosition);
							MoveUnit (transform.position, buildPoint.position);
							currentConstruction = buildPoint.gameObject;
						}


					} else if (hit.collider.gameObject.tag == "Resource" && unitClass.Equals (Type.Grunt)) {
						audio.PlayOneShot (gatherConfirmation);
						currentResource = hit.transform.gameObject;

						if (resourceType != Resource.ResourceType.Nothing) {
							collectedAmount = 0;
							currentLoad = 0;
						}

						resourceType = currentResource.GetComponent<Resource> ().type;
						var gatherPoint = hit.transform.Find ("GatherPoint");
						gathering = true;
						attacking = false;

						if (gatherPoint) {
							collectGoods = false;
							returning = false;
							resourcePoint = gatherPoint.position;
							MoveUnit (transform.position, gatherPoint.position);
						}
					} else if (hit.collider.gameObject.tag == "Resource" || (hit.collider.gameObject.tag == "Home Base" && !unitClass.Equals (Type.Grunt) && hit.collider.gameObject.GetComponent<DestructableBuilding> ().team == this.team)) {
						//If not grunt just stop moving
						state = State.Idle;
                        nv.RPC("setState", RPCMode.All, (int)State.Idle);
					} else if (hit.collider.gameObject.tag == "Home Base" && unitClass.Equals (Type.Grunt) && hit.collider.gameObject.GetComponent<DestructableBuilding> ().team == this.team) {
						//Return to homebase and deposit goods
						var returnPoint = hit.transform.Find ("ReturnPoint");
						attacking = false;

						if (unitClass.Equals (Type.Grunt)) {
							depositing = true;
							MoveUnit (transform.position, returnPoint.position);
						}
					} else {
						//Just move the unit
						if (!attacking) {
							audio.PlayOneShot (moveConfirmation);
						}
						building = false;
						gathering = false;
						returning = false;
						collectGoods = false;
						currentLoad = 0;

						if (targetType == TargetType.Building && attackPoint != null) {
							MoveUnit (transform.position, attackPoint.position);
						} else {
							MoveUnit (transform.position, mouseClick);
						}
					}
				}
			}
		} else {
			CheckState();
		}
	}

	void OnCollisionStay (Collision col)
	{
		//Deals with collisdion between units on the same team
		if (!TargetReached && col.gameObject.GetComponent<Unit> () != null &&!patroling) {
			if (col.gameObject.GetComponent<Unit> ().TargetReached == true && (col.gameObject.GetComponent<Unit> ().state == State.Idle && state == State.Moving)) {
				if(Vector3.Distance(col.gameObject.GetComponent<Unit> ().path[path.Length-1],path[path.Length-1])==0){
					TargetReached = true;
				}
			}
		}
	}
	void Patrol(Vector3 point1, Vector3 point2){
		MoveUnit (point1, point2);
		var temp = point1;
		patrolPoint1 = point2;
		patrolPoint2 = temp;
	}
	int CheckForEnemies ()
	{
		//Perimeter check to see if enemies are close by and if so start attacking them
		int returnAmount = 0;
		if (!attacking && state != State.Moving && state != State.Gathering) {
			int count = 0;
			Collider[] nearbyEnemy = Physics.OverlapSphere (transform.position, attackRange,layerMask); 
			// Returns an array of all enemies in attackrange area
			//print ("Here");
			for (var i =0; i< nearbyEnemy.Length; i++) {
				Unit unit;
				unit = nearbyEnemy [i].GetComponent<Unit> () as Unit;

				if (unit != null && unit.team != this.team) {
					targetType = TargetType.Unit;
					Attack (nearbyEnemy [i].gameObject);
					count ++;//Numbers of enemies
				}
			}
			returnAmount = count;
		}
		return returnAmount;
	}

	//Checks the state of the unit and plays the correct animation
	void CheckState ()
	{
		if (state == State.Moving) {
			anim.Play (runStateHash);
		} else if (state == State.Attacking) {
			anim.Play (attackStateHash);
			if (target != null) {
				DoDamage ();
			}
		} else if (state == State.Gathering) {
			anim.Play (gatherStateHash);
		} else if (state == State.Idle) {
			anim.Play (idleStateHash);
		} else if (state == State.Dead) {
			if(deathcount == 0){
				anim.Play (deathStateHash);
				StopCoroutine ("FollowPath");
				RemoveUnit();
			} 
			deathcount++;
			if (deathcount  > 160){
				RemoveUnit();
			}
		}
	}

	void DoDamage ()
	{
		if (Time.time >= damageWait) {
			if (targetType == TargetType.Building) {
				if (attackHitSoundBuilding != null) {
					audio.PlayOneShot (attackHitSoundBuilding);
				}
				target.gameObject.GetComponent<DestructableBuilding> ().health -= damage;
			} else if (targetType == TargetType.Unit) {
				if (attackHitSoundUnit != null) {
					audio.PlayOneShot (attackHitSoundUnit);
				}
				target.gameObject.GetComponent<Unit> ().health -= damage;
				target.gameObject.GetComponent<HealthBar> ().SetHealthBar (target.gameObject.GetComponent<Unit> ().health);
			}
			damageWait = damageDelay + Time.time;
		}
	}

	void RemoveUnit ()
	{
		Destroy (this.gameObject);
	}
	void OnMouseDown ()
	{
		//Handles selection of the troops
		GameObject aiCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		if (this.team == aiCamera.GetComponent<AICamera> ().team) {
			clicked = true;
			wasSelected = true;
			audio.PlayOneShot (selectionConfirmation);
		}
	}

	void OnMouseUp ()
	{
		GameObject aiCamera = GameObject.FindGameObjectWithTag ("MainCamera");
		if (this.team == aiCamera.GetComponent<AICamera> ().team) {
			if (clicked) {
				wasSelected = true;
			}
			clicked = false;
		}
	}

	//Starts the gathering movement of the grunt
	public void Gather ()
	{
		var returnPoint = mainBuilding.transform.Find ("ReturnPoint");

		if (returnPoint) {
			if (!returning) {//While the unit is collecting goods
				MoveUnit (resourcePoint, returnPoint.position);
				returning = true;
			} else {//When gathering and the unit has reached the home base to return the collected goods
				MoveUnit (returnPoint.position, resourcePoint);
				//Add coollectedAmount to the total resources
				AddResources ();
				collectedAmount = 0;
				returning = false;
			}
		}
	}

	void AddResources ()
	{
		//Increase lava resource by x amount
		Camera.main.GetComponent<Player> ().lava [team - 1] += collectedAmount;
	}

	//Starts an attack on a specific unit
	void Attack (GameObject targetObj)
	{
		print ("Attack");
		this.target = targetObj;
		targetType = TargetType.Unit;
		state = State.Attacking;
        nv.RPC("setState", RPCMode.All, (int)State.Attacking);
		attacking = true;
		transform.LookAt (target.transform);
	}

	//Moves the unit from one point to another
	public void MoveUnit (Vector3 from, Vector3 to)
	{
		PathRequestController.RequestPath (from, to, OnPathFound);
		TargetReached = false;
	}

	//If the PathRequestController finds a path follow it
	public void OnPathFound (Vector3[] newPath, bool success)
	{
		if (success && !gathering) {
			path = newPath;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		} else if (success && gathering && !returning) {
			path = newPath;
			if (currentResource == null) {
				gathering = false;
			}
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		} else if (success && returning && currentResource != null) {
			path = newPath;
			StopCoroutine ("FollowPath");
			collectGoods = true;
            state = State.Gathering;
            nv.RPC("setState", RPCMode.All, (int)State.Gathering);
		}
	}

	//Moves the unit along the path
	IEnumerator FollowPath ()
	{
		if (this.health > 0 && path != null && path.Length > 0) {
			if (!attacking && !gathering && !returning) {
				//UnitMonitor.CreateWaypointGrid ();
			}
            state = State.Moving;
            nv.RPC("setState", RPCMode.All, (int)State.Moving);
			//Play moving animation
			path = SmoothPath (path);
			int targetPosition = 0;
			Vector3 waypoint = path [0];
			while (true && !TargetReached) {
				if (path != null && transform.position == waypoint) {
					targetPosition++;
					if (path != null && targetPosition >= path.Length) {
						path = new Vector3[0];
						yield break;
					}
					waypoint = path [targetPosition];
				}
				waypoint.y = transform.position.y;//So that the units always remain the same height
				direction = waypoint-transform.position;
				var distance = direction.magnitude;
				direction /= distance;
				transform.position = Vector3.MoveTowards (transform.position, waypoint, speed * Time.deltaTime);
				//rigidbody.MovePosition(direction*speed * Time.deltaTime);
				transform.LookAt (waypoint);
				yield return null;
				print (attacking );
				if (target != null && attacking && (Vector3.Distance (target.transform.position, transform.position) <= ((float)attackRange) || (targetType == TargetType.Building && Vector3.Distance (target.transform.Find ("AttackPoint").position, transform.position) <= ((float)attackRange+2))) && ! notOverrideable) {
					attacking = false;
					state = State.Attacking;
                    nv.RPC("setState", RPCMode.All, (int)State.Attacking);
					if (targetType != TargetType.Building) {
						instructedAttack = false;//So when opponent moves unit does keep attacking
					}
					StopCoroutine ("FollowPath");
				}

				if (Vector3.Distance(transform.position,path [path.Length - 1])<StoppingDistance) {
					notOverrideable = false;
					if(!patroling){
						TargetReached = true;
					}
					if (gathering) { //So that the gathering movement happens automatically
						Gather ();
					}

					else if(patroling){
						Patrol(patrolPoint1,patrolPoint2);
						print ("Changepoint");
					} else if (depositing) { // This is for when a grunt is carrying good and the player deposits it manuallly
						depositing = false;
						//Add the collectedAmount to the total resources
						AddResources ();
                        state = State.Idle;
                        nv.RPC("setState", RPCMode.All, (int)State.Idle);
						collectedAmount = 0;
					} else if (target != null && attacking && Vector3.Distance (transform.position, target.transform.position) <= attackRange && ! notOverrideable) {
						Attack (target);
					} else {
						state = State.Idle;
                        nv.RPC("setState", RPCMode.All, (int)State.Idle);

					}
				}
			}

		}
	}

	//Chaikin path smoothing algorithm
	Vector3[] SmoothPath (Vector3[] pathToSmooth)
	{
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
		} else {
			return pathToSmooth;
		}
	}

	// TODO CALVIN
	// Add RPC to set state and take damage
    [RPC]
    void setState(int i) {
        state = (State) i;
    }
}
