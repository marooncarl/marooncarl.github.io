/// <summary>
/// Shake camera
/// 
/// Programmer: Carl Childers
/// Date: 10/23/2015
/// 
/// Shakes the camera, with a variable magnitude and decay rate.
/// This should execute after the script that sets the camera position, since it assumes the camera has
/// already been positioned where it is supposed to be.
/// </summary>

using UnityEngine;
using System.Collections;

public class ShakeCamera : MonoBehaviour {

	public float DecayRate = 1.0f;
	public bool ModifyZ = false;
	public float StartingMagnitude = 0f; // for testing, mostly

	float CurrentMagnitude;


	void Awake() {
		CurrentMagnitude = StartingMagnitude;

		if (camera == null) {
			enabled = false;
			return;
		}
	}

	void LateUpdate() {

		if (CurrentMagnitude != 0) {

			Vector3 offset = Vector3.zero;
			if (ModifyZ) {
				while (offset == Vector3.zero) {
					offset = Random.insideUnitSphere;
					offset.Normalize();
					offset *= CurrentMagnitude;
				}
			} else {
				while (offset == Vector3.zero) {
					offset = Random.insideUnitCircle;
					offset.Normalize();
					offset *= CurrentMagnitude;
				}
			}

			Vector3 newPosition = camera.transform.position + offset;
			camera.transform.position = newPosition;

			CurrentMagnitude = Mathf.Max(0, CurrentMagnitude - DecayRate * Time.deltaTime);

		}
	}

	// Messages

	public void SetShakeMagnitude(float inMag) {
		CurrentMagnitude = Mathf.Max(0, inMag);
	}

	public void SetShakeDecayRate(float inRate) {
		DecayRate = Mathf.Max(0, inRate);
	}
}
