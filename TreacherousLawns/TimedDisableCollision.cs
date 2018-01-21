/// <summary>
/// Timed disable collision
/// 
/// Programmer: Carl Childers
/// Date: 6/28/2015
/// 
/// Disables any colliders attached to this object.  Used in case the object should destroy soon and stop colliding with things
/// immediately, but has some effects that need to finish first.
/// </summary>

using UnityEngine;
using System.Collections;

public class TimedDisableCollision : MonoBehaviour {

	public float DisableTime = 1.0f;

	
	void Awake () {
		Invoke("Disable", DisableTime);
	}
	
	void Disable() {
		if (collider != null) {
			collider.enabled = false;
		}
		if (collider2D != null) {
			collider2D.enabled = false;
		}
	}
}
