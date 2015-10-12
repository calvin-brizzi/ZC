using UnityEngine;
using System.Collections;

public class CamerControl : MonoBehaviour {
	public float speed;
    int team;

    void Awake() { 
        team = VarMan.Instance.pNum;
        if (team == 2) {
            transform.rotation =  Quaternion.Euler(30, 0, 0);
            
            
        }
    }

	// Update is called once per frame
	void Update () {



        if (team == 2)
        {
            Vector3 pos;
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                pos = this.transform.position;
                Vector3 temp = transform.position;
                temp.x = temp.x - speed;
                if (temp.x > -1000)
                {
                    transform.position = temp;
                }
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                pos = this.transform.position;
                Vector3 temp = transform.position;
                temp.x = temp.x + speed;
                if (temp.x < 1000)
                {
                    transform.position = temp;
                }
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                pos = this.transform.position;
                Vector3 temp = transform.position;
                temp.z = temp.z + speed;
                if (temp.z < 1000)
                {
                    transform.position = temp;
                }
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                pos = this.transform.position;
                Vector3 temp = transform.position;
                temp.z = temp.z - speed;
                if (temp.z > -1000)
                {
                    transform.position = temp;
                }


            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                pos = this.transform.position;
                this.transform.position = this.transform.position - this.transform.forward * 4;
                if (pos.y > 16)
                {
                    this.transform.position = pos;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                pos = this.transform.position;
                this.transform.position = this.transform.position + this.transform.forward * 4;

                if (pos.y < 7)
                {
                    this.transform.position = pos;
                }
            }
        }
        else {
            Vector3 pos;
            if (Input.GetKey(KeyCode.RightArrow))
            {
                pos = this.transform.position;
                Vector3 temp = transform.position;
                temp.x = temp.x - speed;
                if (temp.x > -1000)
                {
                    transform.position = temp;
                }
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                pos = this.transform.position;
                Vector3 temp = transform.position;
                temp.x = temp.x + speed;
                if (temp.x < 1000)
                {
                    transform.position = temp;
                }
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                pos = this.transform.position;
                Vector3 temp = transform.position;
                temp.z = temp.z + speed;
                if (temp.z < 1000)
                {
                    transform.position = temp;
                }
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                pos = this.transform.position;
                Vector3 temp = transform.position;
                temp.z = temp.z - speed;
                if (temp.z > -1000)
                {
                    transform.position = temp;
                }


            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                pos = this.transform.position;
                this.transform.position = this.transform.position - this.transform.forward * 4;
                if (pos.y > 16)
                {
                    this.transform.position = pos;
                }
            }
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                pos = this.transform.position;
                this.transform.position = this.transform.position + this.transform.forward * 4;

                if (pos.y < 7)
                {
                    this.transform.position = pos;
                }
            }
        
        }

        if (Input.GetKeyDown(KeyCode.Escape)) {
            Network.Disconnect();
            Application.LoadLevel(0);
        }
	}
}
