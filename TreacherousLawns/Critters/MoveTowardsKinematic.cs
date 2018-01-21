/// <summary>
/// Move towards kinematic
/// 
/// Programmer: Carl Childers
/// Date: 12/19/2015
/// 
/// Moves towards a target without using rigid body physics.  2D
/// </summary>


using UnityEngine;
using System.Collections;

public class MoveTowardsKinematic : MonoBehaviour {

	public Transform Target;
	public float MoveSpeed = 2.0f;
	public float TargetDistance = 1.0f;
	public float TurnSpeedDegrees = 180f;
	public float AngleThreshold = 45f;

	float TargetDistSq;


	void Awake() {
		TargetDistSq = TargetDistance * TargetDistance;
	}
	
	// Update is called once per frame
	void Update () {
		if (Target != null && (Target.position - transform.position).sqrMagnitude > TargetDistSq) {
			float targetAngle = Mathf.Atan2(Target.position.y - transform.position.y, Target.position.x - transform.position.x)
									* Mathf.Rad2Deg - 90;
			float deltaAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.z, targetAngle);
			deltaAngle = Mathf.Sign(deltaAngle) * Mathf.Min(Mathf.Abs(deltaAngle), TurnSpeedDegrees * Time.deltaTime);
			transform.rotation *= Quaternion.Euler(0, 0, deltaAngle);

			deltaAngle = Mathf.DeltaAngle(transform.rotation.eulerAngles.z, targetAngle);
			if (deltaAngle < AngleThreshold) {
				transform.Translate(0, MoveSpeed * Time.deltaTime, 0);
			}
		}
	}

	// Messages
	void SetTarget(Transform inTarget) {
		Target = inTarget;
	}
}
