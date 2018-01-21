// Programmer: Carl Childers
// Date: 10/10/2017
//
// Drags the main camera to the given target position.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragCameraToTarget : MonoBehaviour {

	public Vector3 TargetPosition;
	public float MoveFactor = 10;
	public float FinishThreshold = 0.01f;

	Transform cameraTransform;
	float lastMoveDistance;


	void Start()
	{
		cameraTransform = Camera.main.transform;

		// Check if already at target position
		if ((TargetPosition - cameraTransform.position).magnitude < FinishThreshold)
		{
			cameraTransform.position = TargetPosition;
			enabled = false;
		}
	}

	void Update()
	{
		Vector3 moveDelta = (TargetPosition - cameraTransform.position) * Time.deltaTime * MoveFactor;
		moveDelta = moveDelta.normalized * Mathf.Min(moveDelta.magnitude, lastMoveDistance + Time.deltaTime * MoveFactor);
		cameraTransform.position += moveDelta;
		lastMoveDistance = moveDelta.magnitude;
		if ((TargetPosition - cameraTransform.position).magnitude < FinishThreshold)
		{
			cameraTransform.position = TargetPosition;
			enabled = false;
		}
	}

	public void DragImmediate()
	{
		cameraTransform.position = TargetPosition;
		enabled = false;
	}
}
