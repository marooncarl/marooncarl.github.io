/// <summary>
/// Turn towards target
/// 
/// Programmer: Carl Childers
/// Date: 12/19/2015
/// 
/// Used for a stationary critter that needs to rotate in place to look towards
/// a target.
/// </summary>

using UnityEngine;
using System.Collections;

public class TurnTowardsTarget2D : MonoBehaviour {

	public Transform Target;
	public float TurnSpeedDegrees = 720;

	// Update is called once per frame
	void Update () {
		if (Target != null) {
			float targetAngle = Mathf.Atan2(Target.position.y - transform.position.y, Target.position.x - transform.position.x)
				* Mathf.Rad2Deg - 90;
			float deltaAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.z, targetAngle);
			deltaAngle = Mathf.Sign(deltaAngle) * Mathf.Min(Mathf.Abs(deltaAngle), TurnSpeedDegrees * Time.deltaTime);
			transform.rotation *= Quaternion.Euler(0, 0, deltaAngle);
		}
	}

	// Messages
	void SetTarget(Transform inTarget) {
		Target = inTarget;
	}
}
