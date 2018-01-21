/// <summary>
/// Move to position
/// 
/// Programmer: Carl Childers
/// Date: 12/2/2015
/// 
/// Moves an object to a given position relative to its starting position.
/// Has an optional delay and sound to play when reaching the target position.
/// </summary>

using UnityEngine;
using System.Collections;

public class MoveToPosition : MonoBehaviour {

	public Vector3 RelativePosition; // relative to the starting position
	public float MoveDuration = 1.0f; // how long it takes to move to the target position
	public float Delay = 1.0f;
	public AudioClip RestSound;

	Vector3 StartingPosition; // absolute starting and target positions
	float MoveAlpha;
	float DelayCounter;


	void Awake() {
		if (MoveDuration <= 0) {
			enabled = false;
			return;
		}

		StartingPosition = transform.position;
		DelayCounter = Delay;
	}
	
	// Update is called once per frame
	void Update () {
		DelayCounter -= Time.deltaTime;
		if (DelayCounter <= 0) {
			MoveAlpha = Mathf.Min(MoveAlpha + (Time.deltaTime / MoveDuration), 1.0f);
			transform.position = StartingPosition + RelativePosition * MoveAlpha;
			if (MoveAlpha == 1.0f) {
				// reached target position
				if (RestSound != null) {
					AudioSource.PlayClipAtPoint(RestSound, transform.position);
				}
				enabled = false;
			}
		}
	}

	// Used if the delay is changed after creating this object.
	// Otherwise, changing the delay will have no effect.
	public void SetDelay(float inDelay) {
		Delay = inDelay;
		DelayCounter = Delay;
	}
}
