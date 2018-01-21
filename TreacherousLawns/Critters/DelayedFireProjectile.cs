/// <summary>
/// Delayed fire projectile
/// 
/// Programmer: Carl Childers
/// Date: 10/26/2015
/// 
/// Shoots a projectile after a delay, with an animation or effects giving off a warning for the attack.
/// </summary>

using UnityEngine;
using System.Collections;

public class DelayedFireProjectile : MonoBehaviour {

	public Transform ProjectilePrefab;
	public Transform DelayEffect; 				// optional effect signaling the object is about to fire
	public AudioClip WarningSound, FireSound;
	public bool UseAnimation = true;
	public string AttackAnimParam = "Attacking";
	public float DelayDuration = 0.8f;
	public float ProjectileForce = 100.0f;
	public float ProjectileLeadDistance = 1.0f;
	public bool IsActive = false;
	public bool ShouldLoop = false;
	public int NumberOfLoops = 2;				// 0 = forever

	Animator MyAnimator;

	float DelayCounter;
	int LoopsLeft;
	
	void Awake () {

		MyAnimator = GetComponent<Animator>();

		DelayCounter = 0;
		LoopsLeft = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (!IsActive) {
			return;
		}

		if (DelayCounter > 0) {
			DelayCounter = Mathf.Max(DelayCounter - Time.deltaTime, 0);
			if (DelayCounter == 0) {
				FireProjectile();

				if (ShouldLoop && (NumberOfLoops < 1 || LoopsLeft > 0)) {
					StartAttackLoop();
				} else {
					IsActive = false;
					SendMessage("FinishedAttack");
				}
			}
		}
	}

	void FireProjectile() {
		if (ProjectilePrefab != null) {
			float angle = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
			Vector2 projNormal = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			
			Vector3 projPosition = transform.position;
			projPosition.x += projNormal.x * ProjectileLeadDistance;
			projPosition.y += projNormal.y * ProjectileLeadDistance;

			Transform newProj = (Transform)Instantiate(ProjectilePrefab, projPosition, transform.rotation);

			// Set projectile velocity
			Vector2 projVelocity = projNormal * ProjectileForce;
			
			if (newProj.rigidbody2D != null) {
				newProj.rigidbody2D.AddForce(projVelocity);
			}
		}

		if (FireSound != null) {
			AudioSource.PlayClipAtPoint(FireSound, transform.position);
		}
		// End fire anim
		if (MyAnimator != null && UseAnimation) {
			MyAnimator.SetInteger(AttackAnimParam, 0);
		}
	}

	void BeginAttackDelay() {
		DelayCounter = DelayDuration;
		if (WarningSound != null) {
			AudioSource.PlayClipAtPoint(WarningSound, transform.position);
		}
		if (DelayEffect != null) {
			Instantiate(DelayEffect, transform.position, transform.rotation);
		}
		if (MyAnimator != null && UseAnimation) {
			MyAnimator.SetInteger(AttackAnimParam, 1);
		}
	}

	void StartAttackLoop() {
		BeginAttackDelay();

		LoopsLeft--;
	}

	// Messages

	public void StartAttack() {
		IsActive = true;
		BeginAttackDelay();

		LoopsLeft = NumberOfLoops - 1;
	}
}
