/// <summary>
/// Turn by movement 2D
/// 
/// Programmer: Carl Childers
/// Date: 4/12/2015
/// 
/// Script that makes a game object w/ a 2D rigid body turn to face the direction it's moving in.
/// </summary>

using UnityEngine;
using System.Collections;

public class TurnByMovement2D : MonoBehaviour {

	void FixedUpdate() {
		if (Vector2.SqrMagnitude(rigidbody2D.velocity) > 0) {
			float newAngle = Mathf.Atan2 (rigidbody2D.velocity.y, rigidbody2D.velocity.x) * Mathf.Rad2Deg - 90;
			rigidbody2D.MoveRotation(newAngle);
			print (rigidbody2D.rotation);
		}
	}
}
