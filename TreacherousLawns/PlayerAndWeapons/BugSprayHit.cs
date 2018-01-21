/// <summary>
/// Bug spray hit
/// 
/// Programmer: Carl Childers
/// Date: 5/24/2015
/// 
/// Bug spray projectile script.  Only lasts for one frame.
/// </summary>

using UnityEngine;
using System.Collections;

public class BugSprayHit : MonoBehaviour {

	public float DPS = 1.0f; // damage per second
	public Defense.EDamageType DamageType = Defense.EDamageType.DT_Poison;
	//public float BugMult = 4.0f;
	//public string BugTag = "Bug";

	
	void Update() {
		Destroy(this.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other) {
		/*
		float dmg = DPS * Time.deltaTime;
		if (other.tag == BugTag) {
			dmg *= BugMult;
		}

		other.SendMessage("Damage", dmg, SendMessageOptions.DontRequireReceiver);
		*/

		Damage dmg = new Damage(DPS, DamageType, true);
		other.BroadcastMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);
	}
}
