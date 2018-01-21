/// <summary>
/// Wander 2D
/// 
/// Programmer: Carl Childers
/// Date: 7/11/2015
/// 
/// Allows a creature w/ rigid body in a 2D environment to wander around within a certain area
/// </summary>

using UnityEngine;
using System.Collections;

public class Wander2D : MonoBehaviour {

	public float MoveThrust = 10f;
	public float TurnThrust = 5f;
	public float MinTimeBetweenStops = 5.0f;
	public float MaxTimeBetweenStops = 8.0f;
	public float MinStopDuration = 2.0f;
	public float MaxStopDuration = 4.0f;
	public float WanderAreaRadius = 2.0f;
	public float DestinationThreshold = 0.5f;
	public float AngleThreshold = 10.0f;
	public bool StartStopped = false;
	public bool CanTurnInPlace = false;
	public string RestingParameter = "IsResting";
	//public float WallCheckDistance = 1;
	public int WallLayer = 9;

	Animator animator;

	bool IsStopped;

	Vector3 StartPosition;
	Vector3 TargetPosition;

	float TurnCounter; // countdown until either starting or stopping turning
	float StopCounter; // countdown until either starting or stopping movement

	float LastTargetPickTime; // recorded so a target position isn't picked too frequently


	void Awake() {
		if (rigidbody2D == null) {
			print(this + ": No rigid body 2D!");
			enabled = false;
			return;
		}

		animator = GetComponent<Animator>();

		StartPosition = transform.position;
		PickTargetPosition();

		IsStopped = StartStopped;
		
		ResetStopCounter();

		if (animator != null) {
			//print("Resting: " + IsStopped);
			animator.SetBool(RestingParameter, IsStopped);
		}
	}

	void FixedUpdate() {
		if (!IsStopped) {
			float currentRot = (rigidbody2D.rotation - 90) * Mathf.Deg2Rad;
			Vector2 rotVect = new Vector2( -Mathf.Cos (currentRot), -Mathf.Sin (currentRot) );
			rotVect = rotVect.normalized;
			
			rigidbody2D.AddForce(rotVect * MoveThrust * Time.deltaTime);
		}

		if (!IsStopped) {
			// Need to constantly calculate target angle because the critter turns while moving, so the angle it should aim towards
			// will be a little different
			float targetAngle = Mathf.Atan2(TargetPosition.y - transform.position.y, TargetPosition.x - transform.position.x) * Mathf.Rad2Deg - 90;
			float currentAngle = transform.rotation.eulerAngles.z;
			if (Mathf.Abs(targetAngle - currentAngle) > AngleThreshold) {
				rigidbody2D.AddTorque(TurnThrust * Time.deltaTime * Mathf.Sign(Mathf.DeltaAngle(currentAngle, targetAngle)));
			}
		}
	}

	// Update is called once per frame
	void Update () {
		if (!IsStopped && (TargetPosition - transform.position).magnitude < DestinationThreshold) {
			PickTargetPosition();
		}

		// Update stop counter
		StopCounter -= Time.deltaTime;
		if (StopCounter <= 0) {
			IsStopped = !IsStopped;
			ResetStopCounter();

			if (animator != null) {
				//print("Resting: " + IsStopped);
				animator.SetBool(RestingParameter, IsStopped);
			}

			if (IsStopped) {
				if (Time.time - LastTargetPickTime > 0.5f) {
					PickTargetPosition();
				}
				SendMessage("StoppedMoving", SendMessageOptions.DontRequireReceiver);
			} else {
				SendMessage("StartedMoving", SendMessageOptions.DontRequireReceiver);
			}
		}

		// Avoid moving towards walls
		if (Time.time - LastTargetPickTime > 0.5f) {
			int layerMask = (1 << WallLayer);
			Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);
			Vector2 dir2D = new Vector2(TargetPosition.x, TargetPosition.y) - pos2D;
			RaycastHit2D hit = Physics2D.Raycast(pos2D, dir2D, dir2D.magnitude, layerMask);
			if (hit.collider != null) {
				PickTargetPosition();
			}
		}

		// Debug
		//Debug.DrawLine(transform.position, TargetPosition);
	}

	void PickTargetPosition() {
		Vector2 randPos = Random.insideUnitCircle * WanderAreaRadius;
		TargetPosition = new Vector3(StartPosition.x + randPos.x, StartPosition.y + randPos.y, StartPosition.z);
		LastTargetPickTime = Time.time;
	}

	void ResetStopCounter() {
		if (IsStopped) {
			StopCounter = Random.Range(MinStopDuration, MaxStopDuration);
		} else {
			StopCounter = Random.Range(MinTimeBetweenStops, MaxTimeBetweenStops);
		}
	}

	public void StartMoving() {
		IsStopped = false;
		ResetStopCounter();

		if (animator != null) {
			//print("Resting: " + IsStopped);
			animator.SetBool(RestingParameter, false);
		}

		SendMessage("StartedMoving", SendMessageOptions.DontRequireReceiver);
	}

	public void StopMoving() {
		IsStopped = true;
		ResetStopCounter();
		
		if (animator != null) {
			//print("Resting: " + IsStopped);
			animator.SetBool(RestingParameter, true);
		}

		SendMessage("StoppedMoving", SendMessageOptions.DontRequireReceiver);
	}
}
