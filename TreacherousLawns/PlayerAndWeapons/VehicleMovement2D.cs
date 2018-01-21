/// <summary>
/// Vehicle movement 2D
/// 
/// Programmer: Carl Childers
/// Date: 4/12/2015
/// 
/// 2D vehicle movement from basic movement input.
/// Moves forwards or backwards depending on input and rotation, and tries to rotate to match movement direction
/// Requires a rigid body 2D.
/// </summary>

using UnityEngine;
using System.Collections;

public class VehicleMovement2D : MonoBehaviour {

	public float TurnRate = 15f, IdleTurnRate = 0f;
	public float Thrust = 1000f;
	public bool CanTurnInPlace = false;
	public bool FixedAngleWhenNotTurning = false;
	public float TurnExpirePeriod = 1.0f;
	public string HorAxisName = "Horizontal";
	public string VertAxisName = "Vertical";

	public string WallTag = "Wall";

	public float TimeBetweenImpactEffects = 0.2f; // So impact effects don't trigger too often

	float LastImpactTime;
	float LastTurnTime;
	//float TargetAngle;


	void Awake() {
		if (rigidbody2D == null) {
			enabled = false;
			return;
		}
	}

	void FixedUpdate() {
		ApplyThrust ();
		Turn ();
	}

	void ApplyThrust() {
		float currentRot = (rigidbody2D.rotation - 90) * Mathf.Deg2Rad;
		Vector2 rotVect = new Vector2( -Mathf.Cos (currentRot), -Mathf.Sin (currentRot) );
		rotVect = rotVect.normalized;

		rigidbody2D.AddForce (rotVect * Input.GetAxis (VertAxisName) * Thrust * Time.deltaTime);
	}

	void Turn() {
		float currentTurnRate = TurnRate;
		if (rigidbody2D.velocity.magnitude < 1) {
			if (!CanTurnInPlace) {
				currentTurnRate = 0;
			} else {
				currentTurnRate = IdleTurnRate;
			}
		}

		float torque = currentTurnRate * -Input.GetAxis(HorAxisName) * Time.deltaTime;
		if (Mathf.Abs(torque) > 0) {
			LastTurnTime = Time.time;
		}
		if (FixedAngleWhenNotTurning) {
			if (Time.time - LastTurnTime > TurnExpirePeriod) {
				rigidbody2D.fixedAngle = true;
			} else {
				rigidbody2D.fixedAngle = false;
			}
		}

		if (Mathf.Abs(torque) > 0)
		{
			rigidbody2D.AddTorque(torque);
		}
	}

	void OnCollisionEnter2D(Collision2D coll) {
		//print(coll.relativeVelocity);

		if (coll.gameObject.tag == WallTag && Time.time - LastImpactTime > TimeBetweenImpactEffects &&
		    		coll.relativeVelocity.magnitude > 2.0f)
		{
			foreach (ContactPoint2D pt in coll.contacts) {

				//print(pt.normal);
				// only play the sound for a head on collision
				float contactDot = Vector2.Dot(pt.normal, coll.relativeVelocity);
				if (Mathf.Abs(contactDot) > 0.4f) {
					coll.transform.BroadcastMessage("PlayImpactEffects", new Vector3(pt.point.x, pt.point.y, Camera.main.transform.position.z),
					                           SendMessageOptions.DontRequireReceiver);
					Vector3 impactVel = new Vector3(coll.relativeVelocity.x, coll.relativeVelocity.y, 0);
					impactVel *= 0.5f;
					coll.transform.BroadcastMessage("ImpactShake", impactVel, SendMessageOptions.DontRequireReceiver);

					LastImpactTime = Time.time;
					break;
				}
			}
		}
	}
}
