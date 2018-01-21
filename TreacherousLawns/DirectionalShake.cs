/// <summary>
/// Directional shake
/// 
/// Programmer: Carl Childers
/// Date: 12/16/2015
/// 
/// Receives messages that cause the object to move in a direction, then back and forth
/// until it comes to rest.
/// </summary>

using UnityEngine;
using System.Collections;

public class DirectionalShake : MonoBehaviour {

	public float DecaySpeed = 5.0f;
	public float TargetDecayPercent = 0.5f;
	public float MaxImpactSpeed = 1.0f;
	
	Vector3 StartingPosition;
	Vector3 ShakeDirection; // Normal vector indicating direction
	float ShakeAlpha; // Represents actual distance and can be positive or negative, but doesn't indicate absolute direction
	float ShakeSpeed, TargetSpeed;


	void Awake() {
		StartingPosition = transform.position;
		ShakeDirection = Vector3.zero;
		ShakeAlpha = 0;
		ShakeSpeed = 0;
		TargetSpeed = 0;
	}

	void Update() {
		float prevAlpha = ShakeAlpha;
		ShakeAlpha += ShakeSpeed * Time.deltaTime;

		/*
		ShakeSpeed = Mathf.Sign(ShakeAlpha) * (Mathf.Abs(ShakeSpeed) - CurrentDecay * Time.deltaTime);

		if (Mathf.Sign(prevAlpha) != 0 && Mathf.Sign(prevAlpha) != Mathf.Sign(ShakeAlpha)) {
			CurrentDecay += AdditionalDecay;
		}
		*/

		if (TargetSpeed > ShakeSpeed) {
			ShakeSpeed = Mathf.Min(ShakeSpeed + DecaySpeed * Time.deltaTime, TargetSpeed);
		} else if (TargetSpeed < ShakeSpeed) {
			ShakeSpeed = Mathf.Max(ShakeSpeed - DecaySpeed * Time.deltaTime, TargetSpeed);
		}

		if (Mathf.Sign(prevAlpha) != 0 && Mathf.Sign(prevAlpha) != Mathf.Sign(ShakeAlpha)) {
			TargetSpeed *= -TargetDecayPercent;
		}


		transform.position = StartingPosition + ShakeDirection * ShakeAlpha;
		
	}

	// Messages

	// Starts shaking
	// inVector specifies direction and magnitude
	void ImpactShake(Vector3 inVector) {
		ShakeDirection = inVector.normalized;
		ShakeAlpha = 0;
		ShakeSpeed = Mathf.Min(inVector.magnitude, MaxImpactSpeed);
		TargetSpeed = -ShakeSpeed;
	}
}
