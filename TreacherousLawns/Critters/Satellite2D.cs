/// <summary>
/// Satellite 2D
/// 
/// Programmer: Carl Childers
/// Date: 8/30/2015
/// 
/// Causes a rigid body to move in a circle around either another transform or a point in space.
/// </summary>

using UnityEngine;
using System.Collections;

public class Satellite2D : MonoBehaviour {

	public Transform OrbitTransform; // overrides OrbitAroundStart
	public Vector3 OrbitPoint;
	public float OrbitThrust = 1.0f;
	public bool MovingClockwise = false;
	public bool UseAutoRadius = true; // if true, the radius is determined from the distance between starting position and orbit point
	public float DesiredRadius = 1.0f;
	public bool OrbitAroundStart = false; // if true, use starting position as the orbit position and change position in Awake()
											// overrides UseAutoRadius
	public float StartAngleDegrees = 0; // used if orbiting around start

	//float OrbitRadius; // calculated from starting position
	//float DeltaRadians; // determines how much to move each update
	float CurrentAngleRadians; // records where around the orbit point the object is


	void Awake() {
		if (rigidbody2D == null) {
			enabled = false;
			return;
		}

		/*
		if (OrbitTransform != null) {
			OrbitRadius = (transform.position - OrbitTransform.position).magnitude;
		} else {
			OrbitRadius = (transform.position - OrbitPoint).magnitude;
		}
		// calculate how many radians to move each update
		float maxSpeed;
		if (rigidbody2D.drag > 0) {
			maxSpeed = 2 * (OrbitThrust / rigidbody2D.drag);
		} else {
			maxSpeed = OrbitThrust;
		}
		float circumference = 2 * Mathf.PI * OrbitRadius;
		if (circumference > 0) {
			float degrees = (maxSpeed / circumference) * 360.0f;
			DeltaRadians = degrees * Mathf.Deg2Rad;
		} else {
			DeltaRadians = 0;
		}
		*/

		if (OrbitTransform != null) {
			OrbitPoint = OrbitTransform.position;
		} else if (OrbitAroundStart) {
			OrbitPoint = transform.position;

			Vector3 newPos = OrbitPoint;
			newPos.x += DesiredRadius * Mathf.Cos(StartAngleDegrees * Mathf.Deg2Rad);
			newPos.y += DesiredRadius * Mathf.Sin(StartAngleDegrees * Mathf.Deg2Rad);

			transform.position = newPos;
		}

		Vector3 deltaPos = transform.position - OrbitPoint;
		if (UseAutoRadius && !OrbitAroundStart) {
			DesiredRadius = deltaPos.magnitude;
		}

		CurrentAngleRadians = Mathf.Atan2(deltaPos.y, deltaPos.x);
		SetFacingDirection();
	}

	void FixedUpdate () {

		if (OrbitTransform != null) {
			OrbitPoint = OrbitTransform.position;
		}
		Vector3 deltaPos = transform.position - OrbitPoint;
		if (deltaPos.magnitude > 0) {
			CurrentAngleRadians = Mathf.Atan2(deltaPos.y, deltaPos.x);
		}
		SetFacingDirection();

		// move forward
		Vector3 forceVect = transform.rotation * Vector3.up;
		rigidbody2D.AddForce(new Vector2(forceVect.x, forceVect.y) * Time.deltaTime * OrbitThrust);

		//print(rigidbody2D.velocity.magnitude);
		//print("Current Angle Radians: " + CurrentAngleRadians);
	}

	void SetFacingDirection() {
		/*
		Vector3 orbitCenter;
		if (OrbitTransform != null) {
			orbitCenter = OrbitTransform.position;
		} else {
			orbitCenter = OrbitPoint;
		}
		float radians = Mathf.Atan2(transform.position.y - orbitCenter.y, transform.position.x - orbitCenter.x);
		float targetRadians;
		if (MovingClockwise) {
			targetRadians = radians - DeltaRadians * Time.deltaTime;
		} else {
			targetRadians = radians + DeltaRadians * Time.deltaTime;
		}
		Vector3 targetPoint = new Vector3(orbitCenter.x + OrbitRadius * Mathf.Cos(targetRadians),
		                                  orbitCenter.y + OrbitRadius * Mathf.Sin(targetRadians),
		                                  orbitCenter.z);

		float lookRadians = Mathf.Atan2(targetPoint.y - transform.position.y, targetPoint.x - transform.position.x);
		transform.rotation = Quaternion.Euler(0, 0, lookRadians * Mathf.Rad2Deg);
		*/

		float lookRadians = CurrentAngleRadians - (Mathf.PI / 2f);
		Vector3 deltaPos = transform.position - OrbitPoint;
		float correctFactor = Mathf.Sign(deltaPos.magnitude - DesiredRadius) * 20
				* Mathf.Min(Mathf.Abs(deltaPos.magnitude - DesiredRadius), 3) * Mathf.Deg2Rad;

		if (MovingClockwise) {
			lookRadians -= Mathf.PI / 2f + correctFactor;
		} else {
			lookRadians += Mathf.PI / 2f + correctFactor;
		}
		transform.rotation = Quaternion.Euler(0, 0, lookRadians * Mathf.Rad2Deg);
	}
}
