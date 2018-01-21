/// <summary>
/// Eight way move.
/// 
/// Programmer: Carl Childers
/// Date: 3/29/2014
/// 
/// Makes a game object move in the direction of the held down movement keys.
/// </summary>

using UnityEngine;
using System.Collections;

public class EightWayMove : MonoBehaviour {

	public float MovementSpeed = 10; // used if there is no rigid body
	public float MovementThrust = 100; // used if there is a 2d rigid body
	
	void Update()
	{
		if (rigidbody2D == null)
		{
			Vector3 moveDelta = GetMoveDelta ();
			transform.position += moveDelta;
		}
	}

	void FixedUpdate()
	{
		if (rigidbody2D != null)
		{
			Vector3 moveDelta = GetMoveDelta ();
			rigidbody2D.AddForce(new Vector2(moveDelta.x, moveDelta.y));
		}
	}

	Vector3 GetMoveDelta()
	{
		Vector3 moveDelta = new Vector2();
		moveDelta.x = Input.GetAxis ("Horizontal");
		moveDelta.y = Input.GetAxis ("Vertical");

		if (moveDelta.magnitude > 1)
			moveDelta = Vector3.Normalize(moveDelta);
		if (rigidbody2D == null)
			moveDelta *= (MovementSpeed * Time.deltaTime);
		else
			moveDelta *= (MovementThrust * Time.deltaTime);

		return moveDelta;
	}
}
