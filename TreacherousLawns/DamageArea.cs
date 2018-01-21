/// <summary>
/// Damage area
/// 
/// Programmer: Carl Childers
/// Date: 6/27/2015
/// 
/// Damages things that touch this object's trigger collider.
/// </summary>

using UnityEngine;
using System.Collections;

public class DamageArea : MonoBehaviour {

	public float DamageOverTime = 1.0f;
	public Defense.EDamageType DamageType = Defense.EDamageType.DT_Normal;


	void OnTriggerStay2D(Collider2D other) {
		Damage dmg = new Damage(DamageOverTime, DamageType, true);
		other.BroadcastMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);
	}
}
