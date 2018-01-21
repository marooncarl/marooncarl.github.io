// Programmer: Carl Childers
// Date: 9/23/2017
//
// A type of pile item that is stick-like and sticks into the pile without bouncing.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsPileStick : PhysicsPileItem {

	public float MaxRotation = 5;
	public float RotationRate = 10;
	public Vector3 PivotPointOffset;

	bool landed = false;
	float rotTarget, rotAmount;

	Vector3 pivotPoint;


	public override void SetInitialRotation(JuggleObjectType inType)
	{
		// Set rotation from velocity
		Vector2 velocityNormal = MyRigidbody.velocity.normalized;
		float radians = Mathf.Atan2(velocityNormal.y, velocityNormal.x);
		float degrees = (radians * Mathf.Rad2Deg) - 90;
		transform.rotation = Quaternion.Euler(0, 0, degrees);

		// Set target rotation for after the landing
		rotTarget = Random.Range(0, MaxRotation);
		if (degrees > 0)
		{
			if (degrees < 180)
				rotTarget *= -1;
		}
		else
		{
			if (degrees < -180)
				rotTarget *= -1;
		}
	}

	protected override void UpdatePileItem()
	{
		// Do not become static or change layer when stopped

		if (landed)
		{
			// Update rotation if landed
			float deltaRot;
			float prevRot = rotAmount;
			if (rotTarget > rotAmount)
			{
				rotAmount = Mathf.Min(rotAmount + RotationRate * Time.deltaTime, rotTarget);
			}
			else
			{
				rotAmount = Mathf.Max(rotAmount - RotationRate * Time.deltaTime, rotTarget);
			}
			deltaRot = rotAmount - prevRot;
			transform.RotateAround(pivotPoint, Vector3.forward, deltaRot);

			if (rotTarget == rotAmount)
			{
				enabled = false;
			}
		}
	}

	protected override void Collided(Collision2D coll)
	{
		base.Collided(coll);

		MyRigidbody.bodyType = RigidbodyType2D.Static;
		landed = true;
		if (rotTarget == 0)
		{
			enabled = false;
		}
		else
		{
			CalculatePivotPoint();
		}
	}

	void CalculatePivotPoint()
	{
		pivotPoint = transform.position + (transform.rotation * PivotPointOffset);
	}
}
