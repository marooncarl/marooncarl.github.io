/// <summary>
/// Charging attack
/// 
/// Programmer: Carl Childers
/// Date: 10/26/2015
/// 
/// Makes an object move back a bit, and then charge forward.  2D.
/// </summary>

using UnityEngine;
using System.Collections;

public class ChargingAttack : MonoBehaviour {

	public float ChargeDamage = 1.0f;
	public Defense.EDamageType ChargeDamageType;
	public AudioClip HitSound;
	public float WindUpSpeed = 1.0f;
	public float ChargeSpeed = 8.0f;
	public float DeccelRate = 8.0f;
	public float WindUpDuration = 0.8f;
	public float ChargeDuration = 0.5f; // object starts slowing down after charge is over
	public string HitCheckTag = "Player";
	public float HitCheckRadius = 0.5f;
	public bool IsActive = false;

	float CurrentSpeed;
	float WindUpCounter, ChargeCounter;
	bool IsCharging;
	bool HasHitEnemy;


	void Awake () {

		CurrentSpeed = 0;
		WindUpCounter = 0;
		ChargeCounter = 0;
		IsCharging = false;
		HasHitEnemy = false;
	}
	
	// Update is called once per frame
	void Update () {

		if (!IsActive) {
			return;
		}

		if (WindUpCounter > 0) {
			// Winding up
			WindUpCounter = Mathf.Max(WindUpCounter - Time.deltaTime, 0);
			if (WindUpCounter == 0) {
				CurrentSpeed = ChargeSpeed;
				IsCharging = true;
			}
		} else if (ChargeCounter > 0) {
			// Charging
			ChargeCounter = Mathf.Max(ChargeCounter - Time.deltaTime, 0);
			/*
			if (ChargeCounter == 0) {
				IsCharging = false;
			}
			*/
		} else {
			// Slow down to a halt
			CurrentSpeed = Mathf.Sign(CurrentSpeed) * Mathf.Max(Mathf.Abs(CurrentSpeed) - DeccelRate * Time.deltaTime, 0);
			if (CurrentSpeed == 0) {
				IsActive = false;
				IsCharging = false;
				SendMessage("FinishedAttack");
			}
		}

		transform.Translate(Vector3.up * CurrentSpeed * Time.deltaTime);

		if (IsCharging && !HasHitEnemy) {
			// Check for hit

			GameObject[] targs = GameObject.FindGameObjectsWithTag(HitCheckTag);
			foreach (GameObject t in targs)
			{
				float distSq = (t.transform.position - transform.position).sqrMagnitude;
				if (distSq < HitCheckRadius * HitCheckRadius)
				{
					Damage dmg = new Damage(ChargeDamage, ChargeDamageType, false);
					t.SendMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);
					if (HitSound != null) {
						AudioSource.PlayClipAtPoint(HitSound, transform.position);
					}

					HasHitEnemy = true;
					CurrentSpeed *= -1;
					ChargeCounter = 0;
				}
			}
		}
	}

	// Messages
	void StartAttack() {
		IsActive = true;
		IsCharging = false;
		HasHitEnemy = false;
		CurrentSpeed = -WindUpSpeed;
		WindUpCounter = WindUpDuration;
		ChargeCounter = ChargeDuration;
	}
}
