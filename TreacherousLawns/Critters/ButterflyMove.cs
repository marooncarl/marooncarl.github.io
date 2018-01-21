/// <summary>
/// Butterfly move
/// 
/// Programmer: Carl Childers
/// Date: 5/1/2015
/// 
/// Makes the object move forward, turn around to get where its going, and head towards objects with the Flower tag.
/// Doesn't use rigid body physics, since butterflies don't need to collide.
/// </summary>


using UnityEngine;
using System.Collections;

public class ButterflyMove : MonoBehaviour {

	public float MoveSpeed = 10, TurnSpeed = 10;
	public float SightRadius = 2;
	public float MinStopTime = 3, MaxStopTime = 10;
	public float MinTurnTime = 0.5f, MaxTurnTime = 1.75f;
	public float RandomMoveMaxRadius = 2;							// moves to a random point within a circle of this radius.
																	// The circle is in the direction of the target flower.
	public float LandingThreshold = 0.1f, MaxLandingRadius = 0.5f;
	public float TurnPauseDuration = 1.0f;
	public string FlowerTag = "Flowers";
	public string RestingParameter = "IsResting";


	Animator animator;

	bool IsMoving, FoundLandingSpot;
	Transform TargetFlower, PreviousFlower;
	Vector3 TargetPosition;
	float PreviousDistanceFromTarget;		// used to detect if the butterfly got farther away from its intended target
	bool ShouldSearchForFlower = true;		// used so it doesn't continue searching after failing to find a flower
											// since that means all flowers were destroyed and there won't be any more
	bool ShouldPreventTurning = false;		// used to temporarily stop turning


	void Awake() {
		animator = GetComponent<Animator>();
	}

	void Start() {
		StartMoving();
	}
	
	// Update is called once per frame
	void Update () {
		if (IsMoving) {
			transform.Translate(Vector3.up * MoveSpeed * Time.deltaTime);

			if (!ShouldPreventTurning) {
				Vector3 dir = TargetPosition - transform.position;
				float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90;

				Quaternion targetRot = Quaternion.AngleAxis(angle, Vector3.forward);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, TurnSpeed * Time.deltaTime);
			}

			if (TargetFlower != null)
			{
				if (!FoundLandingSpot && (TargetFlower.position - transform.position).magnitude < SightRadius) {
					Vector2 randVect = Random.insideUnitCircle * MaxLandingRadius;
					TargetPosition = TargetFlower.position + new Vector3(randVect.x, randVect.y);
					FoundLandingSpot = true;
					PreviousDistanceFromTarget = 99999;
				}

				if (FoundLandingSpot) {
					float currDist = (TargetPosition - transform.position).magnitude;
					if (currDist < LandingThreshold) {
						ReachedTargetFlower();
					} else if (currDist > PreviousDistanceFromTarget) {
						// the butterfly got farther away from its target, so pause turning a bit so that it can try again
						// and avoid moving in circles forever
						ShouldPreventTurning = true;
						Invoke("AllowTurning", TurnPauseDuration);
					}

					PreviousDistanceFromTarget = currDist;
				}
			}
		}

		if (ShouldSearchForFlower && (TargetFlower == null || TargetFlower.gameObject.tag != FlowerTag))
		{
			StartMoving();
		}
	}

	Transform GetRandomFlowerTile() {
		GameObject[] allFlowers = GameObject.FindGameObjectsWithTag(FlowerTag);

		if (allFlowers.Length > 0) {
			int randIndex = Random.Range(0, allFlowers.Length);
			if (allFlowers[randIndex].transform == PreviousFlower && allFlowers.Length > 1) {
				// try ten more times to get a new flower
				for (byte i = 0; i < 10; ++i) {
					randIndex = Random.Range(0, allFlowers.Length);

					if (allFlowers[randIndex].transform != PreviousFlower) {
						return allFlowers[randIndex].transform;
					}
				}
			}

			return allFlowers[randIndex].transform;
		}
		return null;
	}

	void ReachedTargetFlower() {
		PreviousFlower = TargetFlower;
		IsMoving = false;
		if (animator != null) {
			animator.SetBool(RestingParameter, true);
		}
		Invoke("StartMoving", Random.Range(MinStopTime, MaxStopTime));
	}

	void StartMoving() {
		IsMoving = true;
		FoundLandingSpot = false;
		TargetFlower = GetRandomFlowerTile();
		if (TargetFlower == null) {
			ShouldSearchForFlower = false;
		}
		if (animator != null) {
			animator.SetBool(RestingParameter, false);
		}
		PickRandomPosition();
	}

	// Randomize movement towards next flower
	void PickRandomPosition() {
		if (!FoundLandingSpot) {
			Vector3 dir;
			if (TargetFlower != null) {
				dir = (TargetFlower.position - transform.position).normalized;
			} else {
				dir = transform.rotation * Vector3.up;
			}
			Vector3 circleCenter = transform.position + dir * RandomMoveMaxRadius;
			Vector2 randVect = Random.insideUnitCircle * RandomMoveMaxRadius;
			TargetPosition = circleCenter + new Vector3(randVect.x, randVect.y);
			Invoke("PickRandomPosition", Random.Range(MinTurnTime, MaxTurnTime));
		}
	}

	void AllowTurning() {
		ShouldPreventTurning = false;
	}
}
