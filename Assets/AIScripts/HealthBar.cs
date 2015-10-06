using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	public GameObject healthBar;
	public int MAX_HEALTH = 100;
	public void SetHealthBar(float health){
		healthBar.transform.localScale = new Vector3 (Mathf.Clamp(health/MAX_HEALTH,0f ,1f),healthBar.transform.localScale.y,healthBar.transform.localScale.z);
	}
}
