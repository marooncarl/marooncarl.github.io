/// <summary>
/// Mole behaviour
/// 
/// Programmer: Carl Childers
/// Date: 12/19/2015
/// 
/// Moles dig through the ground, pop up, attack, go back in, and can be hit by
/// attacks while above ground.
/// </summary>


using UnityEngine;
using System.Collections;

public class MoleBehaviour : MonoBehaviour {
	
	public enum EMoleState {
		Digging,
		Idle,
		Attacking,
		Hit
	}

	public string AnimatorStateName = "MoleState";
	public string TargetTag = "";
	public string DigEffectTag = "Digging";
	public float TargetDistance = 1.0f;
	public float ReturnDistance = 2.0f;
	public float MinDigTime = 1.25f;
	public float MinUndergroundTime = 0.5f;
	public float DigOutDuration = 0.5f;
	public float ReturnDuration = 0.3333f;
	public float StunnedDuration = 1.49f;
	public float AttackDuration = 0.6666f;
	public float PreAttackTime = 0.5f;
	public float TimeBetweenAttacks = 1.0f;
	public float AttackPower = 6;
	public Defense.EDamageType MyDamageType = Defense.EDamageType.DT_Normal;
	public float StartMoveDistance = 3f;
	public AudioClip DigOutSound, DigInSound;
	public Transform DigOutEffect;
	public AudioClip AttackHitSound;
	public AudioClip[] KOSounds;
	public Transform DamagedEffect;
	public float KnockedBackForce = 1000;
	public float KOMass = 10;
	public float AttackMoveForce = 1000;

	public float DigOutCheckRadius = 0.5f;
	public int[] DigOutCheckLayers;
	public float DigCollideCheckRadius = 0.5f;
	public int DigCollideLayer = 17;

	EMoleState MyState;

	Transform Target;
	ParticleSystem DigEffect;
	float TargetDistSq;
	float ReturnDistSq;
	float LastDigTime;
	bool KnockedOut;
	Vector3 PreviousPosition; // used to check whether the mole moved each frame
	float DigCounter; // used to keep track whether the mole is in a digging animation
	float AttackCounter;
	bool StartedMovingToTarget;
	float StartMoveDistSq;

	int DigOutCheckLayerMask;

	Animator MyAnimator;
	MoveTowardsKinematic MoveScript;
	TurnTowardsTarget2D TurnScript;


	void Awake () {
		MyState = EMoleState.Digging;
		MyAnimator = GetComponent<Animator>();
		MoveScript = GetComponent<MoveTowardsKinematic>();
		TurnScript = GetComponent<TurnTowardsTarget2D>();

		foreach (ParticleSystem ps in GetComponentsInChildren<ParticleSystem>()) {
			if (ps.tag == DigEffectTag) {
				DigEffect = ps;
				break;
			}
		}

		// Find target
		GameObject targetObj = GameObject.FindWithTag(TargetTag);
		if (targetObj != null) {
			Target = targetObj.transform.root;
			MoveScript.Target = Target;
			TurnScript.Target = Target;
		}

		TargetDistSq = TargetDistance * TargetDistance;
		ReturnDistSq = ReturnDistance * ReturnDistance;
		LastDigTime = Time.time;
		KnockedOut = false;
		DigCounter = 0;
		AttackCounter = 0;
		PreviousPosition = transform.position;
		StartedMovingToTarget = false;
		StartMoveDistSq = StartMoveDistance * StartMoveDistance;

		foreach (int i in DigOutCheckLayers) {
			DigOutCheckLayerMask = DigOutCheckLayerMask | (1 << i);
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch (MyState) {
		case EMoleState.Digging:
			UpdateDigging();
			break;
		case EMoleState.Idle:
			UpdateIdle();
			break;
		}

		if (DigCounter > 0) {
			DigCounter -= Time.deltaTime;
		}
		PreviousPosition = transform.position;
	}

	void UpdateDigging() {
		if (!StartedMovingToTarget) {
			if (Target != null) {
				if ((Target.position - transform.position).sqrMagnitude < StartMoveDistSq) {
					StartedMovingToTarget = true;
					MoveScript.enabled = true;
				}
			}
		} else {
			if (!KnockedOut && Target != null && Time.time - LastDigTime > MinUndergroundTime) {
				if ((Target.position - transform.position).sqrMagnitude < TargetDistSq) {
					// Check if there's nothing in the way of getting out
					Collider2D coll = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), DigOutCheckRadius,
					                                          DigOutCheckLayerMask);
					if (coll == null) {
						DigOutOfGround();
						return;
					}
				}
			}
		}

		if (transform.position != PreviousPosition) {
			//print("Checking for underground things");
			// Check for things to collide with underground, like underground nests
			Collider2D coll = Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y), DigCollideCheckRadius,
			                                          (1 << DigCollideLayer));
			if (coll != null) {
				//print("Hit nest");
				coll.transform.root.BroadcastMessage("Bombed", SendMessageOptions.DontRequireReceiver);
			}
		}

		if (DigEffect != null) {

			if (DigCounter > 0) {
				if (!DigEffect.isPlaying) {
					DigEffect.Play();
				}
				if (audio != null && !audio.isPlaying) {
					audio.Play();
				}
			} else {
				if (transform.position != PreviousPosition) {
					if (!DigEffect.isPlaying) {
						DigEffect.Play();
					}
					if (audio != null && !audio.isPlaying) {
						audio.Play();
					}
				} else {
					if (DigEffect.isPlaying) {
						DigEffect.Stop();
					}
					if (audio != null && audio.isPlaying) {
						audio.Stop();
					}
				}
			}
		}
	}

	void UpdateIdle() {
		if (!KnockedOut) {
			if (Time.time - LastDigTime > MinDigTime) {
				if (Target == null || (Target.position - transform.position).sqrMagnitude > ReturnDistSq) {
					ReturnToGround();
					return;
				}
			}

			AttackCounter -= Time.deltaTime;
			if (AttackCounter <= 0) {
				BeginAttack();
				AttackCounter = TimeBetweenAttacks;
			}
		}
	}

	void EnableTurnInPlace() {
		if (!KnockedOut) {
			TurnScript.enabled = true;
		}
	}

	void EnableMovement() {
		MoveScript.enabled = true;
	}

	void DigOutOfGround() {
		if (MyState != EMoleState.Digging) {
			return;
		}

		MyState = EMoleState.Idle;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimatorStateName, 1);
		}
		if (DigEffect != null) {
			DigEffect.Stop();
		}
		audio.Stop();
		collider2D.enabled = true;
		MoveScript.enabled = false;
		Invoke("EnableTurnInPlace", DigOutDuration);
		DigCounter = DigOutDuration;
		LastDigTime = Time.time;
		AttackCounter = PreAttackTime;
		if (DigOutSound != null) {
			AudioSource.PlayClipAtPoint(DigOutSound, transform.position);
		}
		if (DigOutEffect != null) {
			Instantiate(DigOutEffect, transform.position, Quaternion.identity);
		}
	}

	void ReturnToGround() {
		if (MyState == EMoleState.Digging) {
			return;
		}

		MyState = EMoleState.Digging;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimatorStateName, 0);
		}
		collider2D.enabled = false;
		TurnScript.enabled = false;
		if (!KnockedOut) {
			Invoke("EnableMovement", ReturnDuration);
		}
		DigCounter = ReturnDuration;
		LastDigTime = Time.time;
		if (DigInSound != null) {
			AudioSource.PlayClipAtPoint(DigInSound, transform.position);
		}
		if (DigOutEffect != null) {
			Instantiate(DigOutEffect, transform.position, Quaternion.identity);
		}
	}

	void BeginAttack() {
		MyState = EMoleState.Attacking;
		//TurnScript.enabled = false;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimatorStateName, 2);
		}
		Invoke("FinishAttack", AttackDuration);
	}

	void FinishAttack() {
		MyState = EMoleState.Idle;
		//TurnScript.enabled = true;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimatorStateName, 1);
		}
		SendMessage("RefreshAttack");
	}

	void AttackMoveForward() {
		if (rigidbody2D != null)
		{
			float angle = (transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
			Vector2 force = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			force *= AttackMoveForce;
			rigidbody2D.AddForce(force, ForceMode2D.Impulse);
		}
	}

	// Used so the mole doesn't loop the hit animation
	void SetIdleAnimation() {
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimatorStateName, 1);
		}
	}

	// Messages
	// Handle death behaviour
	// Instead of actually dying, mole gets stunned for a bit and then goes back underground for an extended period, or forever.
	// This goes to the stunned animation
	void Died(Defense.EDamageType inDmgType) {
		if (KnockedOut) {
			return;
		}
		if (MyState == EMoleState.Attacking) {
			FinishAttack();
		}

		MyState = EMoleState.Hit;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimatorStateName, 3);
		}
		if (DigEffect != null) {
			DigEffect.Stop();
		}
		collider2D.enabled = true;
		TurnScript.enabled = false;
		MoveScript.enabled = false;

		KnockedOut = true;
		Invoke("ReturnToGround", StunnedDuration);
		Invoke("SetIdleAnimation", StunnedDuration / 2.0f);

		// Get knocked back
		if (rigidbody2D != null) {
			float angle = (transform.rotation.eulerAngles.z + 270);
			angle += 45 * Random.Range(-1, 1);
			angle *= Mathf.Deg2Rad;
			Vector2 force = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
			//print("Force dir: " + force);
			//print("Angle: " + angle);
			force *= KnockedBackForce;
			rigidbody2D.AddForce(force, ForceMode2D.Impulse);
			rigidbody2D.mass = KOMass;
		}

		if (KOSounds.Length > 0) {
			AudioClip PickedClip = KOSounds[ Random.Range(0, KOSounds.Length) ];
			AudioSource.PlayClipAtPoint(PickedClip, transform.position);
		}

		if (DamagedEffect != null) {
			Instantiate(DamagedEffect, transform.position, DamagedEffect.rotation);
		}
	}

	void HitTarget(Transform hitObject) {
		//print("Hit Target");
		Damage dmg = new Damage(AttackPower, MyDamageType, false);
		hitObject.BroadcastMessage("TakeDamage", dmg, SendMessageOptions.DontRequireReceiver);
		if (AttackHitSound != null) {
			AudioSource.PlayClipAtPoint(AttackHitSound, transform.position);
		}
	}
}
