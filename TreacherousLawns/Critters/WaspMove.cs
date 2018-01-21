/// <summary>
/// Wasp move
/// 
/// Programmer: Carl Childers
/// Date: 5/16/2015
/// 
/// Movement and attack patterns for wasps.
/// </summary>

using UnityEngine;
using System.Collections;

public class WaspMove : MonoBehaviour {

	public float MoveSpeed = 12.0f, AttackMoveSpeed = 20.0f;
	public float TurnSpeed = 10.0f;
	public float MinHoverRange = 1.0f, MaxHoverRange = 3.0f;
	public float HoverCircleRadius = 0.1f;
	public float SmallHoverPeriod = 0.5f;
	public float MinReturnTime = 5.0f, MaxReturnTime = 10.0f;
	public float AttackCooldown = 3.0f;
	public float MinHesitationTime = 1.0f, MaxHesitationTime = 2.0f; // attack hesitation
	public float MinAttackTurnTime = 1.0f, MaxAttackTurnTime = 2.0f;
	public float ReturnDistance = 0.25f;
	public float AttackHitDistance = 0.5f;
	public float AttackMaxRange = 4.0f;
	public float NormalSoundVolume = 0.1f, AttackSoundVolume = 0.2f, NormalSoundPitch = 1.0f, AttackSoundPitch = 1.2f;
	public int NormalSoundPriority = 255, AttackSoundPriority = 128;
	public int WallLayer = 9;
	public bool HasMultipleAttacks = false;

	protected AudioSource MyAudio;

	protected Transform MyNest;
	protected Vector3 NestPosition;
	protected Transform Target;
	protected float ReturnTime;
	protected float HoverRange;
	protected float AttackDirection; // direction from target from which to hover
	protected Vector3 HoverCenterPosition, HoverTargetPosition;
	protected float HoverCounter; // used to update HoverTargetPosition around HoverCenterPosition
	protected Vector3 AttackPosition;
	protected Quaternion TargetRotation;
	protected float AttackCounter;
	protected float AttackHesitationCounter;


	enum EWaspState
	{
		WS_Roaming,
		WS_Returning,
		WS_Hovering,
		WS_Attacking,
		WS_AttackReturn
	};

	EWaspState MyState;


	void Awake() {
		MyAudio = GetComponent<AudioSource>();

		AttackDirection = 0;
		HoverRange = MinHoverRange;
		ReturnTime = Random.Range(MinReturnTime, MaxReturnTime);
		MyState = EWaspState.WS_Roaming;
		AttackCounter = 0;
		HoverCounter = SmallHoverPeriod;
		HoverCenterPosition = transform.position;
		HoverTargetPosition = transform.position;
		AttackPosition = transform.position;
		NestPosition = transform.position;
		AttackHesitationCounter = Random.Range(MinHesitationTime, MaxHesitationTime);

		UpdateAudio();
	}

	void Start() {
		Invoke("ReturnToNest", ReturnTime);
	}

	// Update is called once per frame
	void Update () {
		switch (MyState) {
		case EWaspState.WS_Roaming:
			UpdateRoaming();
			break;
		case EWaspState.WS_Returning:
			UpdateReturning();
			break;
		case EWaspState.WS_Hovering:
			UpdateHovering();
			break;
		}
	}

	void UpdateRoaming() {
		transform.Translate(Vector3.up * MoveSpeed * Time.deltaTime);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, TurnSpeed * Time.deltaTime);
	}

	void UpdateReturning() {
		transform.Translate(Vector3.up * MoveSpeed * Time.deltaTime);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, TargetRotation, TurnSpeed * Time.deltaTime);

		bool isNestBlocked = false;
		if (MyNest != null) {
			CritterNest nestScript = MyNest.GetComponent<CritterNest>();
			if (nestScript != null && nestScript.GetIsBlocked()) {
				isNestBlocked = true;
			}
		}

		if (MyNest != null && !isNestBlocked) {
			if ((MyNest.position - transform.position).magnitude < ReturnDistance) {
				MyNest.SendMessage("CritterReturned", SendMessageOptions.DontRequireReceiver);
				Destroy(gameObject);
				return;
			}
		} else {
			if ((NestPosition - transform.position).magnitude < ReturnDistance) {
				// Something happened to the nest
				ReturnTime = Random.Range(MinReturnTime, MaxReturnTime) / 3.0f;
				MyState = EWaspState.WS_Roaming;
				Invoke("ReturnToNest", ReturnTime);
				return;
			}
		}
		ReturnToNest(); // keep turning towards the next, since it moved and the target rotation may be wrong
	}

	void UpdateHovering() {
		if (Target != null) {
			float currentHoverSpeed = AttackMoveSpeed;
			if ((HoverTargetPosition - transform.position).magnitude < HoverCircleRadius * 2) {
				currentHoverSpeed *= 0.1f;
			}

			transform.position = Vector3.MoveTowards(transform.position, HoverTargetPosition, currentHoverSpeed * Time.deltaTime);
			TurnTowardsTarget(Target.position);

			HoverCounter -= Time.deltaTime;
			if (HoverCounter <= 0) {
				ChangeHoverTargetPosition();
			}
				
			AttackCounter -= Time.deltaTime;
			if (AttackCounter <= 0) {
				// The wasp is now able to attack, but may hesitate
				AttackHesitationCounter -= Time.deltaTime;
				if (AttackHesitationCounter <= 0) {
					// Hold off the attack if there is a wall in the way or if the target is far away
					if (CanAttackFromHere(transform.position, Target.position)) {
						BeginAttack();
						return;
					} else {
						if ((transform.position - HoverTargetPosition).sqrMagnitude < 0.01f) {
							FindGoodAttackingPosition();
							return;
						}
					}
				}
			}

			if ((transform.position - HoverTargetPosition).sqrMagnitude < 0.01f) {
				if ((transform.position - Target.position).sqrMagnitude > MaxHoverRange) {
					AttackTurn();
					return;
				}
			}
		}
	}

	void ChangeHoverTargetPosition() {
		float angle = Random.Range(0, 2 * Mathf.PI);
		HoverTargetPosition = HoverCenterPosition + new Vector3(Mathf.Cos(angle) * HoverCircleRadius, Mathf.Sin(angle) * HoverCircleRadius, 0);
		HoverCounter = SmallHoverPeriod;
	}

	// Used when the script becomes enabled again and the critter may be somewhere else
	public void HoverAtCurrentPosition() {
		HoverCenterPosition = transform.position;
		ChangeHoverTargetPosition();
	}

	void SetNest(Transform inNest) {
		MyNest = inNest;
		NestPosition = MyNest.position;
	}

	// Start moving in a given direction
	// inDir - direction in degrees
	void MoveInDirection(float inDir) {
		TargetRotation = Quaternion.Euler(0, 0, inDir);
		transform.rotation = TargetRotation;
	}

	void SetTarget(Transform inTarget) {
		Target = inTarget;
		if (Target != null) {
			MyState = EWaspState.WS_Hovering;
			//AttackCounter = Random.Range(MinAttackInterval, MaxAttackInterval);
			Invoke("AttackTurn", Random.Range(MinAttackTurnTime, MaxAttackTurnTime));

			AttackDirection = (Mathf.Atan2(transform.position.y - inTarget.position.y, transform.position.x - inTarget.position.x) * Mathf.Rad2Deg) - 90;
			AttackDirection += Random.Range(-5, 5);

			HoverRange = Mathf.Clamp((transform.position - inTarget.position).magnitude + 0.5f, MinHoverRange, MaxHoverRange);

			FindGoodAttackingPosition();
		}

		UpdateAudio();
	}

	void SetRandomAttackDirection() {
		AttackDirection = Random.Range(0, 360);
		HoverRange = Random.Range(MinHoverRange, MaxHoverRange);

		// set hover position
		float angle = (AttackDirection + 90) * Mathf.Deg2Rad;
		HoverCenterPosition = Target.position + new Vector3( Mathf.Cos(angle) * HoverRange, Mathf.Sin(angle) * HoverRange, 0);
		ChangeHoverTargetPosition();
	}

	void ReturnToNest() {
		if (Target == null) {
			Vector3 dir;
			if (MyNest != null) {
				dir = MyNest.position - transform.position;
			} else {
				dir = NestPosition - transform.position;
			}
			float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90;
			TargetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
			MyState = EWaspState.WS_Returning;
		}
	}

	void AttackTurn() {
		if (Target != null) {
			AttackDirection = (Mathf.Atan2(transform.position.y - Target.position.y, transform.position.x - Target.position.x) * Mathf.Rad2Deg) - 90;
			AttackDirection += Random.Range (-30, 30);

			HoverRange = Mathf.Clamp(HoverRange + Random.Range(-0.5f, 0.5f), MinHoverRange, MaxHoverRange);

			FindGoodAttackingPosition();

			Invoke("AttackTurn", Random.Range(MinAttackTurnTime, MaxAttackTurnTime));
		}
	}

	void TurnTowardsTarget(Vector3 inTargetPos) {
		Vector3 dir = inTargetPos - transform.position;
		float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90;
		Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, TurnSpeed * Time.deltaTime);
	}

	void BeginAttack() {
		// Let another script handle attacking
		enabled = false;

		if (HasMultipleAttacks)
			SendMessage("ChooseAttack");
		else
			SendMessage("StartAttack");

		// After attack is finished, should return to hovering next
		MyState = EWaspState.WS_Hovering;
		ChangeHoverTargetPosition();
		AttackCounter = AttackCooldown;
	}

	bool CanAttackFromHere(Vector3 fromPosition, Vector3 toPosition) {

		Vector2 deltaPosition = new Vector2(toPosition.x - fromPosition.x, toPosition.y - fromPosition.y);

		if (deltaPosition.sqrMagnitude > AttackMaxRange * AttackMaxRange) {
			return false;
		}

		RaycastHit2D hit = Physics2D.Raycast(new Vector2(fromPosition.x, fromPosition.y),
		                                     deltaPosition.normalized, deltaPosition.magnitude, (1 << WallLayer));
		if (hit.collider != null) {
			return false;
		}

		return true;
	}


	void FindGoodAttackingPosition() {

		if (Target == null) {
			return;
		}

		bool foundPos = false;
		Vector3 pickedPos = HoverCenterPosition;

		// First, try to get in front of or behind the target, whichever one is closer first.
		// If both fail, then try to the left and to the right

		float targetFacingDir = (Target.transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad;
		HoverRange = Random.Range(MinHoverRange, MaxHoverRange);
		
		Vector3 frontPos = Target.transform.position;
		frontPos.x += Mathf.Cos(targetFacingDir) * HoverRange;
		frontPos.y += Mathf.Sin(targetFacingDir) * HoverRange;

		Vector3 backPos = Target.transform.position;
		backPos.x += Mathf.Cos(targetFacingDir + Mathf.PI) * HoverRange;
		backPos.y += Mathf.Sin(targetFacingDir * Mathf.PI) * HoverRange;

		if ((frontPos - transform.position).sqrMagnitude < (backPos - transform.position).sqrMagnitude) {
			// try front, then back
			foundPos = GetGoodHoveringPosition(Target.position, HoverRange, targetFacingDir, out pickedPos);
			if (!foundPos) {
				foundPos = GetGoodHoveringPosition(Target.position, HoverRange, targetFacingDir + Mathf.PI, out pickedPos);
			}
		} else {
			// try back, then front
			foundPos = GetGoodHoveringPosition(Target.position, HoverRange, targetFacingDir + Mathf.PI, out pickedPos);
			if (!foundPos) {
				foundPos = GetGoodHoveringPosition(Target.position, HoverRange, targetFacingDir, out pickedPos);
			}
		}

		if (!foundPos) {
			Vector3 leftPos = Target.transform.position;
			leftPos.x += Mathf.Cos(targetFacingDir + Mathf.PI / 2.0f) * HoverRange;
			leftPos.y += Mathf.Sin(targetFacingDir + Mathf.PI / 2.0f) * HoverRange;

			Vector3 rightPos = Target.transform.position;
			rightPos.x += Mathf.Cos(targetFacingDir - Mathf.PI / 2.0f) * HoverRange;
			rightPos.y += Mathf.Sin(targetFacingDir - Mathf.PI / 2.0f) * HoverRange;

			if ((leftPos - transform.position).sqrMagnitude < (rightPos - transform.position).sqrMagnitude) {
				// try left, then right
				foundPos = GetGoodHoveringPosition(Target.position, HoverRange, targetFacingDir + Mathf.PI / 2.0f, out pickedPos);
				if (!foundPos) {
					foundPos = GetGoodHoveringPosition(Target.position, HoverRange, targetFacingDir - Mathf.PI / 2.0f, out pickedPos);
				}
			} else {
				// try right, then left
				foundPos = GetGoodHoveringPosition(Target.position, HoverRange, targetFacingDir - Mathf.PI / 2.0f, out pickedPos);
				if (!foundPos) {
					foundPos = GetGoodHoveringPosition(Target.position, HoverRange, targetFacingDir + Mathf.PI / 2.0f, out pickedPos);
				}
			}
		}

		if (foundPos) {
			HoverCenterPosition = pickedPos;
			ChangeHoverTargetPosition();
		}
	}

	// Tries to get a good hovering position around centerPos, usind dirRadians as the base direction
	// The actual direction is within a random range centered around dirRadians
	// Returns whether successful.  If so, the found position is put in outPos.
	bool GetGoodHoveringPosition(Vector3 centerPos, float radius, float dirRadians, out Vector3 outPos) {

		for (int i = 0; i < 10; ++i) {
			float pickedDir = dirRadians - (Mathf.PI / 4.0f) + Random.value * (Mathf.PI / 2.0f);
			Vector3 pickedPos = centerPos;
			pickedPos.x += Mathf.Cos(pickedDir) * radius;
			pickedPos.y += Mathf.Sin(pickedDir) * radius;
			if (CanAttackFromHere(pickedPos, Target.position)) {
				outPos = pickedPos;
				return true;
			}
		}

		outPos = transform.position;
		return false;
	}

	void UpdateAudio() {
		if (MyAudio != null) {
			if (Target != null) {
				MyAudio.volume = AttackSoundVolume;
				MyAudio.pitch = AttackSoundPitch;
				MyAudio.priority = AttackSoundPriority;
			} else {
				MyAudio.volume = NormalSoundVolume;
				MyAudio.pitch = NormalSoundPitch;
				MyAudio.priority = NormalSoundPriority;
			}
		}
	}

	// Messages

	void TargetSighted(Transform inTarget) {
		if (Target == null) {
			SetTarget(inTarget);
		}
	}
}
