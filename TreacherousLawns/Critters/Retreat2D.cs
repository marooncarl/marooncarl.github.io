/// <summary>
/// Retreat 2D
/// 
/// Programmer: Carl Childers
/// Date: 7/11/2015
/// 
/// Retreat from a target
/// </summary>

using UnityEngine;
using System.Collections;

public class Retreat2D : MonoBehaviour {

	public Transform Target;
	public float MoveThrust = 10f;
	public float TurnThrust = 5f;
	public float DegreeTolerance = 2f;


	void Awake() {
		if (rigidbody2D == null) {
			enabled = false;
			return;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (Target != null) {
			float destDegrees = Mathf.Atan2(transform.position.y - Target.position.y, transform.position.x - Target.position.x) * Mathf.Rad2Deg;
			float currDegrees = rigidbody2D.rotation - 90;

			float degDiff = destDegrees - currDegrees;
			if (degDiff > 180f) {
				degDiff -= 360f;
			} else if (degDiff < -180f) {
				degDiff += 360f;
			}

			if (Mathf.Abs(degDiff) >= DegreeTolerance) {
				rigidbody2D.AddTorque(-Mathf.Sign(degDiff) * Mathf.Min(TurnThrust, Mathf.Abs(degDiff)) * Time.deltaTime);
			}

			float currentRot = (rigidbody2D.rotation - 90) * Mathf.Deg2Rad;
			Vector2 rotVect = new Vector2( -Mathf.Cos (currentRot), -Mathf.Sin (currentRot) );
			rotVect = rotVect.normalized;
			
			rigidbody2D.AddForce(rotVect * MoveThrust * Time.deltaTime);
		}
	}
}
