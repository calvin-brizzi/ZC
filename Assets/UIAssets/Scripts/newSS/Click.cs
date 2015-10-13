using UnityEngine;
using System.Collections;

public class Click : MonoBehaviour {
	public Canvas can;
	public GameObject[] units;
	public GameObject[] grunt;
	public GameObject [] buildings;
	public GameObject [] builder;
	public GameObject obj;
	GameObject currentSchool;
	int p = 0;

	// Use this for initialization
	void Start () {
		p = this.GetComponent<AICamera> ().team;
		for(int i = 0;i<units.Length;i++){
			units[i].SetActive (false);
		}
		for(int i = 0;i<grunt.Length;i++){
			grunt[i].SetActive (false);
		}
		for(int i = 0;i<buildings.Length;i++){
			buildings[i].SetActive (false);
		}
		for(int i = 0;i<builder.Length;i++){
			builder[i].SetActive (false);
		}
	}
	public void makeBrute(){
		print ("ATTEMPTS");
		currentSchool.GetComponent<Spawn> ().spawnBrute ();

	}
	public void makeArcher(){
		print ("ATTEMPTS");
		currentSchool.GetComponent<Spawn> ().spawnArcher ();
		
	}
	public void makeGrunt(){
		print ("ATTEMPTS");
		currentSchool.GetComponent<Spawn> ().spawnGrunt ();
		
	}
	public void build(){
		print ("WORKS BITCH");
		for(int i = 0;i<units.Length;i++){
			units[i].SetActive (false);
		}
		for(int i = 0;i<grunt.Length;i++){
			grunt[i].SetActive (false);
		}
		for(int i = 0;i<buildings.Length;i++){
			buildings[i].SetActive (false);
		}
		for(int i = 0;i<builder.Length;i++){
			builder[i].SetActive (true);
		}
	}
	public void tower(){
		print ("TRYS");
		int resource = this.GetComponent<Player> ().getLava (p - 1);
		if (resource > 500) {
			this.GetComponent<Player>().takeLava(500,p-1);
			Instantiate (obj, new Vector3 (0, 0, 0), Quaternion.identity);
			GameObject temp = GameObject.FindGameObjectWithTag ("Build");
			print (temp.transform.name);
			temp.transform.SendMessage ("set", 3, SendMessageOptions.DontRequireReceiver);
		}
		
	}
	public void loveHotel(){
		print ("TRYS");
		int resource = this.GetComponent<Player> ().getLava (p - 1);
		if (resource > 600) {
			this.GetComponent<Player>().takeLava(600,p-1);
			Instantiate (obj, new Vector3 (0, 0, 0), Quaternion.identity);
			GameObject temp = GameObject.FindGameObjectWithTag ("Build");
			print (temp.transform.name);
			temp.transform.SendMessage ("set", 0, SendMessageOptions.DontRequireReceiver);
		}

	}
	public void zombieSchool(){

		int resource = this.GetComponent<Player> ().getLava (p - 1);
		if (resource > 700) {
			this.GetComponent<Player>().takeLava(700,p-1);
			Instantiate (obj, new Vector3 (0, 0, 0), Quaternion.identity);
			GameObject temp = GameObject.FindGameObjectWithTag ("Build");
			print (temp.transform.name);
			temp.transform.SendMessage ("set", 2, SendMessageOptions.DontRequireReceiver);
		}
		
	}
	public void graveyard(){
		int resource = this.GetComponent<Player> ().getLava (p - 1);
		if(resource>400){
			this.GetComponent<Player>().takeLava(400,p-1);
			Instantiate (obj,new Vector3(0,0,0),Quaternion.identity);
			GameObject temp = GameObject.FindGameObjectWithTag ("Build");
			print (temp.transform.name);
			temp.transform.SendMessage ("set",1,SendMessageOptions.DontRequireReceiver);
		}
		
	}
    //public void unit(int x){
    //    if (x == 0) {
    //        Transform point = currentSchool.transform.GetChild(2);
    //        print ("GRUNT");
    //    } else if (x == 1) {
    //        Transform point = currentSchool.transform.GetChild(2);
    //        print ("GRUNT");

    //    } else if (x == 2) {
    //        Transform point = currentSchool.transform.GetChild(2);
    //        print ("GRUNT");
    //    }

    //}



	// Update is called once per frame
	void Update () {
		//Vector3 fwd = transform.TransformDirection (Vector3.forward)*0;
		RaycastHit hit;
		if (Input.GetMouseButtonDown (0)) {
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			Debug.DrawRay (ray.origin,ray.direction*100,Color.yellow);
			if(Physics.Raycast (ray,out hit,1700)){
				//print("AHH");

				if(hit.transform.tag == "Unit"&&hit.transform.GetComponent<Unit>().team==p){
;
					for(int i = 0;i<grunt.Length;i++){
						grunt[i].SetActive (false);
					}
					for(int i = 0;i<units.Length;i++){
						units[i].SetActive (true);
					}

					for(int i = 0;i<buildings.Length;i++){
						buildings[i].SetActive (false);
					}
					for(int i = 0;i<builder.Length;i++){
						builder[i].SetActive (false);
					}

				}
				if(hit.transform.tag == "School"&&hit.transform.GetComponent<DestructableBuilding>().team==p){
					currentSchool = hit.transform.gameObject;
					//print ("CURRENT SCHOOL");
					for(int i = 0;i<units.Length;i++){
						units[i].SetActive (false);
					}
					for(int i = 0;i<grunt.Length;i++){
						grunt[i].SetActive (false);
					}
					for(int i = 0;i<buildings.Length;i++){
						buildings[i].SetActive (true);
					}
					for(int i = 0;i<builder.Length;i++){
						builder[i].SetActive (false);
					}

				}
				if(hit.transform.tag == "Grunt"&&hit.transform.GetComponent<Unit>().team==p){
					//print ("THIS WORKS THO");
					for(int i = 0;i<units.Length;i++){
						units[i].SetActive (false);
					}
					for(int i = 0;i<grunt.Length;i++){
						grunt[i].SetActive (true);
					}
					for(int i = 0;i<buildings.Length;i++){
						buildings[i].SetActive (false);
					}
					for(int i = 0;i<builder.Length;i++){
						builder[i].SetActive (false);
					}

				}
				//print(hit.transform.name);
				//hit.transform.SendMessage ("selected");
			}
			else{
				print("NO");
			}
		}
	}
}
