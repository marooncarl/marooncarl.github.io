/// <summary>
/// Lawn player animation
/// 
/// Programmer: Carl Childers
/// Date: 12/23/2015
/// 
/// Controls animation for the lawn player character.
/// </summary>

using UnityEngine;
using System.Collections;

public class LawnPlayerAnimation : MonoBehaviour {

	/*
	public enum LawnRiderState {
		Driving,
		Down
	};
	*/

	public string AnimPropertyName = "RiderState";
	public float HitAnimDuration = 1.0f;

	//LawnRiderState MyState;
	Animator MyAnimator;

	int CurrentAnimState;
	// 0 = Driving
	// 1 = Hit / Dead
	// 2 = Turning right
	// 3 = Turning left
	// 4 = Stretching (Win anim)

	bool IsDead, IsLevelClear;

	
	void Awake () {
		MyAnimator = GetComponent<Animator>();
		//MyState = LawnRiderState.Driving;
		IsDead = false;
		IsLevelClear = false;

		CurrentAnimState = 0;
	}

	void EndHitAnim() {
		//MyState = LawnRiderState.Driving;
		CurrentAnimState = 0;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimPropertyName, CurrentAnimState);
		}
	}

	void Update() {
		// Update turn animations

		if (!IsDead && !IsLevelClear && CurrentAnimState != 1)
		{
			int PrevAnimState = CurrentAnimState;

			float horAxis = Input.GetAxis("Horizontal");
			if (horAxis > 0)
				CurrentAnimState = 2;
			else if (horAxis < 0)
				CurrentAnimState = 3;
			else
				CurrentAnimState = 0;

			if (PrevAnimState != CurrentAnimState && MyAnimator != null) {
				MyAnimator.SetInteger(AnimPropertyName, CurrentAnimState);
			}
		}
	}
	
	// Messages

	void TakeDamage(Damage inDmg) {
		if (IsDead || IsLevelClear) {
			return;
		}

		//MyState = LawnRiderState.Down;
		CurrentAnimState = 1;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimPropertyName, CurrentAnimState);
		}
		CancelInvoke("EndHitAnim");
		Invoke("EndHitAnim", HitAnimDuration);
	}
	
	void Died() {
		if (IsLevelClear) {
			return;
		}

		CancelInvoke("EndHitAnim");
		IsDead = true;
		//MyState = LawnRiderState.Down;
		CurrentAnimState = 1;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimPropertyName, CurrentAnimState);
		}
	}

	// The level was cleared
	void LevelCompleted() {
		IsLevelClear = true;
		CancelInvoke("EndHitAnim");

		CurrentAnimState = 4;
		if (MyAnimator != null) {
			MyAnimator.SetInteger(AnimPropertyName, CurrentAnimState);
		}
	}
}
